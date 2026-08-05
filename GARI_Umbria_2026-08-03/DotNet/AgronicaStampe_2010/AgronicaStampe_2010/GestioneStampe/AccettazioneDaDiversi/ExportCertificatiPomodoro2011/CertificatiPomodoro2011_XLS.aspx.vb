Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Agro_Math

Public Class CertificatiPomodoro2011_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Certificati Pomodoro "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Codice_Specie As String
    Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    Dim Piva_Produttore, Piva_Coop1 As String
    Dim Sa_Cod, Fabbricato_Cod As Integer
    'Dim RagSoc_Impresa As String
    'Dim Descr_Specie As String
    'Dim Descr_Magazzino As String

    Dim Flag_Bio As Boolean
    Dim Tipo_Peso As Integer
    Dim Peso_Lordo_Kg, Tara_Veicolo_Kg, Tara_Imballi_Kg, Peso_Netto_Kg, Netto_Pag_Kg, Scarto_Kg, Peso_Kg As Double
    Dim Peso_Lordo_Q, Tara_Veicolo_Q, Tara_Imballi_Q, Peso_Netto_Q, Netto_Pag_Q, Scarto_Q, Peso_Totale_Q As Double
    Dim Perc_Marcio, Perc_Verde, Perc_Inerti, Perc_Tot_Dif_Maggiori, Perc_Tot_Dif_Magg_Round As Double
    Dim Perc_Schiacciati, Perc_Immaturi, Perc_Scottature, Perc_Lesioni, Perc_Tot_Dif_Minori As Double
    Dim Grado_Brix, Indice_Prezzo_Grado_Brix As Double
    Dim Coefficiente, Franchigia, Premio_EuroTon_PomoBio As Double
    Dim Dif_MinoriXCoeff As Double
    Dim Magg_Rid_DifMaggiori As Double
    Dim Indice_Variazione_Prezzo As Double
    Dim Premio_Pomo_Tardivo, Premio_Pomo_Bio As Double
    Dim Prezzo_Unitario_Contratto, Prezzo_Unitario_Finale_Kg, Prezzo_Unitario_Finale_Tn As Double
    Dim Importo_Totale_Pag As Double

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Const R3_Contratto As Integer = 8
    Const R3_OP As Integer = 6
    Const R3_Produttore As Integer = 7
    Const R3_Coop As Integer = 7
    Const R3_CentroRaccolta As Integer = 2
    Const R3_Industria As Integer = 10
    'Const R3_Ass As Integer = 2
    Const R3_DDT As Integer = 5
    Const R3_Qta As Integer = 18 '14
    Const R3_Prezzo As Integer = 15

    Const R2_Numero As Integer = 4
    Const R2_Contratto As Integer = 36
    Const R2_DDT As Integer = 5
    Const R2_Scarico As Integer = 33


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0
        Dim Anno_Certificato As Integer


        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = CertificatiPomodoro.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_Utente_Username") = "" Then
            Dim strClose As String = "<script language='javascript'>window.close()</script>"
            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("pp")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

            Piva_Coop1 = Stringa_Decodifica(CStr(Request.QueryString("pc1")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)

            'Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
            '                                    AgroKey_EncoderDecoder, _
            '                                    Server)

            'Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
            '                                        AgroKey_EncoderDecoder, _
            '                                        Server)

            'RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
            '                                   AgroKey_EncoderDecoder, _
            '                                   Server)
            'If RagSoc_Impresa = "" Then
            '    RagSoc_Impresa = RagSoc_from_Piva(Server, Session, Page, Piva)
            'End If

            Codice_Specie = Stringa_Decodifica(CStr(Request.QueryString("cs")), _
                                      AgroKey_EncoderDecoder, _
                                      Server)

            If Codice_Specie = "0" Then
                Codice_Specie = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutte le specie
                'o il range di specie selezionato
            End If

            Str_FiltroSpecie = Session("Str_Codici_Specie")

            If Str_FiltroSpecie <> "" Then
                'è stato selezionato un range di codici
                Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_FiltroSpecie
            Else
                Str_FiltroSpecie = ""
            End If


            Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

            If Codice_Conferente = "0" Then
                Codice_Conferente = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti i conferenti
                'o il range di conferenti selezionato
            Else
                Codice_Conferente = CStr(Codice_Conferente)
            End If


            Str_FiltroConf = Session("Str_Codici_Conferenti")

            If Str_FiltroConf <> "" Then
                'è stato selezionato un range di codici
                Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
            Else
                Str_FiltroConf = ""
            End If



        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Dati da querystring: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim DT_Lettura As DataTable
        Dim DT_Stampa As DataTable
        Dim DR As DataRow
        Dim i As Integer
        Dim Temp_Id_Agenda As Integer = 0
        Dim Num_Cert_Letti As Integer

        Dim Numero_Bolla As String
        Dim x_Doc_Numero_Sin_Accettazione As String = ""
        Dim x_Doc_Numero_Accettazione As Integer = 0
        Dim x_Doc_Numero_Des_Accettazione As String = ""
        Dim x_Lunghezza_Sin_Accett As Integer
        Dim x_Lunghezza_Centro_Accett As Integer
        Dim x_Lunghezza_Des_Accett As Integer
        Dim x_CarattereFormattazione_Accett As String
        Dim x_Doc_Numero_Sin_Cert As String = ""
        Dim x_Doc_Numero_Cert As Integer = 0
        Dim x_Doc_Numero_Des_Cert As String = ""
        Dim Str_Bio_Conv As String
        Dim Str_Flag_Bio As String
        Dim Str_Surgelato As String
        Dim Str_Tipologia As String
        Dim x_Doc_Numero_Sin_DDTConf As String = ""
        Dim x_Doc_Numero_DDTConf As Integer = 0
        Dim x_Doc_Numero_Des_DDTConf As String = ""
        Dim Numero_DDTConf As String
        Dim Anno As Integer


        Dim obj_TabOP As New AgronicaCoreMetaSchemaDAL.Tabelle_OP_R

        Try

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT_Lettura = ADD.CertificatoPomodoro_Stampa(Piva, _
                                                        0, _
                                                        Sa_Cod, _
                                                        Fabbricato_Cod, _
                                                        Codice_Specie, _
                                                        Codice_Conferente, _
                                                        Str_FiltroSpecie, _
                                                        Str_FiltroConf, _
                                                        Data_Da, _
                                                        Data_A, _
                                                        True, _
                                                        True, _
                                                        Piva_Produttore, _
                                                        Piva_Coop1, _
                                                        "", "", _
                                                        objParametri_Server)


            If Not IsNothing(DT_Lettura) Then

                'DT_Stampa = DT_Lettura.Clone
                DT_Stampa = Crea_DT_Stampa()

                Num_Cert_Letti = DT_Lettura.Rows.Count

                For i = 0 To Num_Cert_Letti - 1

                    With DT_Lettura.Rows(i)

                        'se sono al primo giro o se è cambiato il certificato 
                        If (Temp_Id_Agenda <> .Item("Id_Agenda")) Then

                            'se NON sono al primo giro
                            'devo fare i calcoli del certificato precedente
                            If Temp_Id_Agenda <> 0 Then
                                Valorizza_Riga_Precedente(objADDFun, DT_Stampa, Anno)
                            End If

                            'memorizzo il nuovo id_agenda
                            Temp_Id_Agenda = .Item("Id_Agenda")

                            'nuovo certificato, va aggiunta una riga al DT
                            DR = DT_Stampa.NewRow()

                            x_Lunghezza_Sin_Accett = .Item("Lunghezza_Sin")
                            x_Lunghezza_Centro_Accett = .Item("Lunghezza_Centro")
                            x_Lunghezza_Des_Accett = .Item("Lunghezza_Des")
                            x_CarattereFormattazione_Accett = .Item("CarattereFormattazione")

                            'DR.Item("Num_Certificato = .Item("Numero_Certificato_OLD")
                            x_Doc_Numero_Sin_Cert = .Item("Doc_Numero_Sin_Cert")
                            x_Doc_Numero_Cert = .Item("Doc_Numero_Cert")
                            x_Doc_Numero_Des_Cert = .Item("Doc_Numero_Des_Cert")

                            'DR.Item("Data_Certificato") = .Item("Data_Certificato_OLD")
                            DR.Item("Data_Certificato") = CDate(.Item("Data_Certificato")).ToShortDateString
                            Anno = CDate(.Item("Data_Certificato")).Year

                            x_Doc_Numero_Sin_Accettazione = .Item("Doc_Numero_Sin")
                            x_Doc_Numero_Accettazione = .Item("Doc_Numero")
                            x_Doc_Numero_Des_Accettazione = .Item("Doc_Numero_Des")

                            Numero_Bolla = Ricava_NumeroDocumento_Con_Sequenza( _
                                                    x_Doc_Numero_Sin_Accettazione, _
                                                    x_Doc_Numero_Accettazione, _
                                                    x_Doc_Numero_Des_Accettazione, _
                                                    x_Lunghezza_Sin_Accett, _
                                                    x_Lunghezza_Centro_Accett, _
                                                    x_Lunghezza_Des_Accett, _
                                                    x_CarattereFormattazione_Accett)

                            DR.Item("Num_Bolla") = Numero_Bolla
                            DR.Item("Data_Bolla") = CDate(.Item("Data_Accett")).ToShortDateString

                            'nota, Marco non salva più nel movimento del certificato (cau_mov=4070)
                            'quindi il num_certificato è 0 (non so gli altri anni come faceva a venire stampato)
                            Select Case CDate(DR.Item("Data_Bolla")).Year
                              
                                Case 2011, 2012
                                    'DR.Item("Num_Certificato") = Ricava_NumeroDocumento_Senza_Sequenza( _
                                    '                        x_Doc_Numero_Sin_Cert, _
                                    '                        x_Doc_Numero_Cert, _
                                    '                        x_Doc_Numero_Des_Cert)

                                    DR.Item("Num_Certificato") = Ricava_NumeroDocumento_Con_Sequenza( _
                                                                    x_Doc_Numero_Sin_Cert, _
                                                                    x_Doc_Numero_Cert, _
                                                                    x_Doc_Numero_Des_Cert, _
                                                                    x_Lunghezza_Sin_Accett, _
                                                                    x_Lunghezza_Centro_Accett, _
                                                                    x_Lunghezza_Des_Accett, _
                                                                    x_CarattereFormattazione_Accett)
                                Case Else
                                    DR.Item("Num_Certificato") = Numero_Bolla
                            End Select

                            'DR.Item("Cod_OPR_Contratto") = .Item("Cod_Agrea_Contratto")

                            Anno_Certificato = CDate(DR.Item("Data_Certificato")).Year
                            Select Case Anno_Certificato
                                Case 2011
                                    DR.Item("Num_Contratto") = .Item("Contratto_Numero")

                                    If CDate(.Item("Data_Stipulazione")) = AGRODATAINIZIO Then
                                        DR.Item("Data_Contratto") = ""
                                    Else
                                        DR.Item("Data_Contratto") = CDate(CStr(.Item("Data_Stipulazione"))).ToShortDateString
                                    End If

                                Case Else
                                    If InStr(CStr(.Item("Clausola")), "|") > 0 Then
                                        DR.Item("Num_Clausola") = CStr(.Item("Clausola")).Split("|")(0)

                                        If IsDate(CStr(.Item("Clausola")).Split("|")(1)) Then
                                            DR.Item("Data_Clausola") = CDate(CStr(.Item("Clausola")).Split("|")(1)).ToShortDateString
                                        Else
                                            DR.Item("Data_Clausola") = ""
                                        End If
                                    Else
                                        DR.Item("Num_Clausola") = ""
                                        DR.Item("Data_Clausola") = ""
                                    End If

                                    If DR.Item("Num_Clausola") <> "" And DR.Item("Data_Clausola") <> "" Then
                                        DR.Item("Num_Contratto") = ""
                                        DR.Item("Data_Contratto") = ""
                                    Else
                                        DR.Item("Num_Contratto") = .Item("Contratto_Numero")

                                        If CDate(.Item("Data_Stipulazione")) = AGRODATAINIZIO Then
                                            DR.Item("Data_Contratto") = ""
                                        Else
                                            DR.Item("Data_Contratto") = CDate(CStr(.Item("Data_Stipulazione"))).ToShortDateString
                                        End If
                                    End If

                            End Select

                            DR.Item("Prodotto") = CStr(.Item("Veg_Des")).ToUpper '+ " " + CStr(TipologiaPomodoro_from_GrvaCod(.Item("Grva_Cod_Veg"))).ToUpper

                            If .Item("Flag_Surgelato") = 1 Then
                                Str_Surgelato = "SURGELATO"
                            Else
                                Str_Surgelato = ""
                            End If

                            Select Case CInt(.Item("Regolamento"))
                                Case enum_Cod_Regolamento.Regolamento_bio
                                    Str_Bio_Conv = "BIOLOGICO"
                                    Str_Flag_Bio = "X"
                                    Flag_Bio = True
                                Case Else
                                    Str_Bio_Conv = "LOTTA INTEGRATA"
                                    Str_Flag_Bio = ""
                                    Flag_Bio = False
                            End Select

                            'Str_Tipologia = CStr(TipologiaPomodoro_from_GrvaCod(.Item("Grva_Cod_Veg"))).ToUpper
                            DR.Item("Tipologia") = Str_Tipologia
                            If Str_Tipologia <> "" Then
                                DR.Item("Tipologia") += " "
                            End If
                            DR.Item("Tipologia") += Str_Surgelato
                            If Str_Surgelato <> "" Then
                                DR.Item("Tipologia") += " "
                            End If
                            DR.Item("Tipologia") += Str_Bio_Conv

                            DR.Item("Desc_appezza") += .Item("Descrizione_Appezzamento")
                            DR.Item("Varieta_Distretto") += .Item("Varieta_Distretto")

                            DR.Item("Codice_OP") = .Item("Codice_OP")
                            DR.Item("Piva_OP") = .Item("Cod_Contatto_Conferente")
                            DR.Item("Cod_Fisc_OP") = .Item("Codice_Fiscale_Conferente")
                            DR.Item("Rag_Soc_OP") = .Item("Rag_Soc_Conferente")
                            DR.Item("Codice_Unione_OP") = .Item("Codice_Unione_OP")
                            DR.Item("Rag_Soc_Unione_OP") = obj_TabOP.UnioneOPEstesa_from_CodiceUnioneOP(.Item("Codice_Unione_OP"), Session("ASG_objParametri_Server"))

                            DR.Item("Piva_Produttore") = .Item("Cod_Contatto_Produttore")
                            DR.Item("Cod_Fisc_Produttore") = .Item("Codice_Fiscale_Produttore")
                            DR.Item("Rag_Soc_Produttore") = .Item("Rag_Soc_Produttore")
                            DR.Item("Indirizzo_Produttore") = .Item("ind_des_Produttore")
                            DR.Item("frazione_Produttore") = .Item("frz_des_Produttore")
                            DR.Item("Comune_Prov_Produttore") = .Item("localita_Produttore") + " (" + .Item("comuni_prov_Produttore") + ")"
                            DR.Item("CAP_Produttore") = .Item("cap_Produttore")
                            DR.Item("Piva_Coop") = .Item("Cod_Contatto_Coop")
                            DR.Item("Cod_Fisc_Coop") = .Item("Codice_Fiscale_Coop")
                            DR.Item("Rag_Soc_Coop") = .Item("Rag_Soc_Coop")
                            DR.Item("Indirizzo_Coop") = .Item("ind_des_coop")
                            DR.Item("Frazione_Coop") = .Item("frz_des_coop")
                            DR.Item("Comune_Prov_Coop") = .Item("localita_coop") + " (" + .Item("comuni_prov_coop") + ")"
                            DR.Item("CAP_Coop") = .Item("cap_coop")
                            DR.Item("Frazione_Coop") = .Item("frz_des_coop")
                            DR.Item("Piva_CentroLav") = ""
                            DR.Item("Rag_Soc_CentroLav") = ""
                            DR.Item("Codice_Trasformatore") = .Item("Codice_Az_Trasf")
                            DR.Item("Codice_Stabilimento") = .Item("Codice_Stab_Reg")
                            DR.Item("Piva_Trasformatore") = .Item("Cod_Contatto_Trasf")
                            DR.Item("Cod_Fisc_Trasformatore") = .Item("Codice_Fiscale_Trasf")
                            DR.Item("Rag_Soc_Trasformatore") = .Item("Rag_Soc_Trasf")
                            DR.Item("Indirizzo_Trasformatore") = .Item("ind_des_Trasf")
                            DR.Item("Comune_Prov_Trasformatore") = .Item("localita_Trasf") + " (" + .Item("comuni_prov_Trasf") + ")"
                            DR.Item("CAP_Trasformatore") = .Item("cap_Trasf")
                            DR.Item("Codice_Ass_Industriale") = .Item("Codice_Ass_Ind")
                            DR.Item("Ass_Industriale") = obj_TabOP.AssociazioneIndutriale_from_CodiceAssociazioneIndutriale(.Item("Codice_Ass_Ind"), Session("ASG_objParametri_Server"))

                            x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                            x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                            x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")

                            Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                    x_Doc_Numero_Sin_DDTConf, _
                                                    x_Doc_Numero_DDTConf, _
                                                    x_Doc_Numero_Des_DDTConf)

                            DR.Item("Num_DDT") = Numero_DDTConf
                            DR.Item("Data_DDT") = CDate(.Item("Data_Conf")).ToShortDateString
                            DR.Item("Rag_Soc_Vettore") = .Item("Rag_Soc_Vettore")
                            DR.Item("Targa_Automezzo") = .Item("Targa_Automezzo")
                            DR.Item("Targa_Rimorchio") = .Item("Targa_Rimorchio")


                            DR.Item("Data_Accettazione") = CDate(.Item("Data_Accett")).ToShortDateString
                            DR.Item("Ora_Accettazione") = CDate(.Item("Ora_Accett")).ToShortTimeString
                            DR.Item("Num_Tagliando_Pesa") = "RCD" + CStr(.Item("Tagliando_Pesa"))

                            'DR.Item("Tondo_X") = ""
                            'DR.Item("Allungato_X") = ""
                            'TondoLungo_from_GrvaCod(.Item("Grva_Cod_Veg"), DR.Item("Tondo_X"), DR.Item("Allungato_X"))
                            DR.Item("Biologico") = Str_Flag_Bio


                            Tipo_Peso = .Item("Tipo_Peso")

                            Tara_Veicolo_Kg = .Item("Tara_Veicolo")
                            Tara_Veicolo_Kg = RoundNumber_ParteIntera(Tara_Veicolo_Kg)
                            Tara_Veicolo_Q = objADDFun.Qta_Ql_2Dec(Tara_Veicolo_Kg)

                            Tara_Imballi_Kg = .Item("Tara_Imballi")
                            Tara_Imballi_Kg = RoundNumber_ParteIntera(Tara_Imballi_Kg)
                            Tara_Imballi_Q = objADDFun.Qta_Ql_2Dec(Tara_Imballi_Kg)

                            Peso_Kg = .Item("Peso")
                            Peso_Kg = RoundNumber_ParteIntera(Peso_Kg)

                            Peso_Lordo_Kg = Calcola_PesoLordo(Tipo_Peso, Peso_Kg, Tara_Imballi_Kg)
                            Peso_Lordo_Kg = RoundNumber_ParteIntera(Peso_Lordo_Kg)
                            Peso_Lordo_Q = objADDFun.Qta_Ql_2Dec(Peso_Lordo_Kg)

                            Peso_Netto_Kg = Calcola_PesoNetto(Tipo_Peso, Peso_Kg, Tara_Imballi_Kg)
                            Peso_Netto_Kg = RoundNumber_ParteIntera(Peso_Netto_Kg)
                            Peso_Netto_Q = objADDFun.Qta_Ql_2Dec(Peso_Netto_Kg)

                            Peso_Totale_Q = objADDFun.Calcola_PesoTotale(2, Peso_Lordo_Kg, Tara_Veicolo_Kg)

                            'nel caso del pomodoro:
                            'peso lordo = peso totale
                            DR.Item("Peso_Lordo") = Format(Peso_Totale_Q, "#,###,##0.00")

                            DR.Item("Tara_Veicolo") = Format(Tara_Veicolo_Q, "#,###,##0.00")
                            DR.Item("Tara_Imballi") = Format(Tara_Imballi_Q, "#,###,##0.00")
                            DR.Item("Peso_Netto") = Format(Peso_Netto_Q, "#,###,##0.00")

                            'il totale dei difetti maggiori arrotondati con il giusto arrotondamento è salvato nel degrado
                            Perc_Tot_Dif_Magg_Round = .Item("Variazione_Raccolta")

                            If InStr(CStr(.Item("GradoBrix_IndicePrezzo")), "_") > 0 Then
                                Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(0))
                                Indice_Prezzo_Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(1))
                            Else
                                Grado_Brix = 0
                                Indice_Prezzo_Grado_Brix = 0
                            End If

                            DR.Item("Grado_Brix") = Format(Grado_Brix, "#,###,##0.000")
                            DR.Item("Indice_prezzo_Grado_Brix") = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

                            Coefficiente = .Item("Coefficiente_DifMinori")
                            Franchigia = .Item("Franchigia_DifMaggiori")
                            Premio_EuroTon_PomoBio = .Item("Premio_EuroTon_PomoBio")

                            Prezzo_Unitario_Contratto = CDbl(.Item("Prezzo_Unitario")) * 1000 'a tonnellata

                            Prezzo_Unitario_Finale_Kg = .Item("Prezzo_Unitario_Netto")

                            If .Item("Premio_PomoTardivo") = "" Then
                                Premio_Pomo_Tardivo = 0
                            Else
                                Premio_Pomo_Tardivo = CDbl(.Item("Premio_PomoTardivo"))
                            End If

                            Premio_Pomo_Bio = CDbl(.Item("Premio_Pomo_Bio"))

                            '   DR.Item("Premio_Pomo_Tardivo") = Format(Premio_Pomo_Tardivo, "#,###,##0.00")

                            'per ogni riga
                            Select Case .Item("tipo_cod")

                                Case enum_DanniRaccolta.Marcio
                                    Perc_Marcio = .Item("val_cod")

                                Case enum_DanniRaccolta.Verde
                                    Perc_Verde = .Item("val_cod")

                                Case enum_DanniRaccolta.Inerti
                                    Perc_Inerti = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiSchiacciati
                                    Perc_Schiacciati = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiImmaturi
                                    Perc_Immaturi = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiScottati
                                    Perc_Scottature = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiLesionati
                                    Perc_Lesioni = .Item("val_cod")

                            End Select

                            'aggiungo la riga al dt
                            DT_Stampa.Rows.Add(DR)

                            '----------------------------------------------------------
                        Else
                            'stesso certificato, valorizzo le colonne mancanti

                            'per ogni riga
                            Select Case .Item("tipo_cod")

                                Case enum_DanniRaccolta.Marcio
                                    Perc_Marcio = .Item("val_cod")

                                Case enum_DanniRaccolta.Verde
                                    Perc_Verde = .Item("val_cod")

                                Case enum_DanniRaccolta.Inerti
                                    Perc_Inerti = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiSchiacciati
                                    Perc_Schiacciati = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiImmaturi
                                    Perc_Immaturi = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiScottati
                                    Perc_Scottature = .Item("val_cod")

                                Case enum_DanniRaccolta.FruttiLesionati
                                    Perc_Lesioni = .Item("val_cod")

                            End Select


                        End If 'controllo se stesso certificato o no

                        If i = Num_Cert_Letti - 1 Then
                            Valorizza_Riga_Precedente(objADDFun, DT_Stampa, Anno)
                        End If

                    End With

                Next

            End If


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try



        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        'If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

        If Anno_Certificato = 2011 Then
            DT_Stampa.Columns.Remove("Num_Clausola")
            DT_Stampa.Columns.Remove("Data_Clausola")
        End If

        Crea_EXCEL(DT_Stampa)

        'End If



    End Sub


    '##############################################################
    'se cambiano le colonne, aggiornare le costanti a inizio pagina
    Private Function Crea_DT_Stampa() As DataTable


        Dim DT As New DataTable

        ' ""
        'Const R2_Numero As Integer = 4
        ' ""
        'Const R3_Contratto As Integer = 8
        DT.Columns.Add(New DataColumn("Num_Certificato", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Certificato", GetType(String)))
        DT.Columns.Add(New DataColumn("Num_Bolla", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Bolla", GetType(String)))
        ' "ESTREMI DEL CONTRATTO E DEI CONTRAENTI"
        'Const R2_Contratto As Integer = 36
        'DT.Columns.Add(New DataColumn("Cod_OPR_Contratto", GetType(String)))
        DT.Columns.Add(New DataColumn("Num_Contratto", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Contratto", GetType(String)))
        'riattivate clausole nel 2012
        DT.Columns.Add(New DataColumn("Num_Clausola", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_Clausola", GetType(String)))

        '-----------------------------------------------------------------
        ' "ORGANIZZAZIONE DEI PRODUTTORI"
        'Const R3_OP As Integer = 6
        DT.Columns.Add(New DataColumn("Codice_OP", GetType(String)))
        DT.Columns.Add(New DataColumn("Rag_Soc_OP", GetType(String)))
        DT.Columns.Add(New DataColumn("Piva_OP", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Fisc_OP", GetType(String)))
        DT.Columns.Add(New DataColumn("Codice_Unione_OP", GetType(String)))
        DT.Columns.Add(New DataColumn("Rag_Soc_Unione_OP", GetType(String)))
        ' "PRODUTTORE CONFERENTE"
        'Const R3_Produttore As Integer = 7
        DT.Columns.Add(New DataColumn("Rag_Soc_Produttore", GetType(String)))
        DT.Columns.Add(New DataColumn("Piva_Produttore", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Fisc_Produttore", GetType(String)))
        '21
        DT.Columns.Add(New DataColumn("Indirizzo_Produttore", GetType(String)))
        DT.Columns.Add(New DataColumn("CAP_Produttore", GetType(String)))
        DT.Columns.Add(New DataColumn("Frazione_Produttore", GetType(String)))
        DT.Columns.Add(New DataColumn("Comune_Prov_Produttore", GetType(String)))
        ' "COOPERATIVA"
        'Const R3_Coop As Integer = 7
        DT.Columns.Add(New DataColumn("Rag_Soc_Coop", GetType(String)))
        DT.Columns.Add(New DataColumn("Piva_Coop", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Fisc_Coop", GetType(String)))
        DT.Columns.Add(New DataColumn("Indirizzo_Coop", GetType(String)))
        DT.Columns.Add(New DataColumn("CAP_Coop", GetType(String)))
        DT.Columns.Add(New DataColumn("Frazione_Coop", GetType(String)))
        '31
        DT.Columns.Add(New DataColumn("Comune_Prov_Coop", GetType(String)))
        ' "CENTRO DI LAVORAZIONE / RACCOLTA"
        'Const R3_CentroRaccolta As Integer = 2
        DT.Columns.Add(New DataColumn("Piva_CentroLav", GetType(String)))
        DT.Columns.Add(New DataColumn("Rag_Soc_CentroLav", GetType(String)))
        ' "INDUSTRIA DESTINATARIA"
        'Const R3_Industria As Integer = 8
        DT.Columns.Add(New DataColumn("Codice_Trasformatore", GetType(String)))
        DT.Columns.Add(New DataColumn("Codice_Stabilimento", GetType(String)))
        DT.Columns.Add(New DataColumn("Rag_Soc_Trasformatore", GetType(String)))
        DT.Columns.Add(New DataColumn("Piva_Trasformatore", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Fisc_Trasformatore", GetType(String)))
        DT.Columns.Add(New DataColumn("Indirizzo_Trasformatore", GetType(String)))
        DT.Columns.Add(New DataColumn("CAP_Trasformatore", GetType(String)))
        '41
        DT.Columns.Add(New DataColumn("Comune_Prov_Trasformatore", GetType(String)))
        ' ""
        'Const R3_Ass As Integer = 2
        DT.Columns.Add(New DataColumn("Codice_Ass_Industriale", GetType(String)))
        DT.Columns.Add(New DataColumn("Ass_Industriale", GetType(String)))

        ' "ESTREMI DEL DOCUMENTO DI TRASPORTO"
        'Const R2_DDT As Integer = 5
        ' "DOCUMENTO TRASPORTO"
        'Const R3_DDT As Integer = 5
        DT.Columns.Add(New DataColumn("Num_DDT", GetType(String)))
        DT.Columns.Add(New DataColumn("Data_DDT", GetType(String)))
        DT.Columns.Add(New DataColumn("Rag_Soc_Vettore", GetType(String)))
        DT.Columns.Add(New DataColumn("Targa_Automezzo", GetType(String)))
        DT.Columns.Add(New DataColumn("Targa_Rimorchio", GetType(String)))
        ' "VERIFICA DI SCARICO"
        'Const R2_Scarico As Integer = 29
        ' "RIDUZIONI SU QUANTITA'"
        'Const R3_Qta As Integer = 18
        DT.Columns.Add(New DataColumn("Data_Accettazione", GetType(String)))
        DT.Columns.Add(New DataColumn("Ora_Accettazione", GetType(String)))
        '51
        DT.Columns.Add(New DataColumn("Num_Tagliando_Pesa", GetType(String)))
        'DT.Columns.Add(New DataColumn("Tondo_X", GetType(String)))
        'DT.Columns.Add(New DataColumn("Allungato_X", GetType(String)))
        DT.Columns.Add(New DataColumn("Prodotto", GetType(String)))
        DT.Columns.Add(New DataColumn("Tipologia", GetType(String)))
        DT.Columns.Add(New DataColumn("Varieta_Distretto", GetType(String)))
        DT.Columns.Add(New DataColumn("Desc_Appezza", GetType(String)))
        DT.Columns.Add(New DataColumn("Biologico", GetType(String)))

        DT.Columns.Add(New DataColumn("Peso_Lordo", GetType(String)))
        DT.Columns.Add(New DataColumn("Tara_Veicolo", GetType(String)))
        DT.Columns.Add(New DataColumn("Tara_Imballi", GetType(String)))
        DT.Columns.Add(New DataColumn("Peso_Netto", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Marcio", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Verde", GetType(String)))
        '61
        DT.Columns.Add(New DataColumn("Perc_Inerti", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Tot_Dif_Magg", GetType(String)))
        DT.Columns.Add(New DataColumn("Scarto", GetType(String)))
        DT.Columns.Add(New DataColumn("Netto_Pag", GetType(String)))
        '"VARIAZIONI SU PREZZO"
        'Const R3_Prezzo As Integer = 15
        DT.Columns.Add(New DataColumn("Grado_Brix", GetType(String)))
        DT.Columns.Add(New DataColumn("Indice_prezzo_Grado_Brix", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Schiacciati", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Immaturi", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Scottature", GetType(String)))
        DT.Columns.Add(New DataColumn("Perc_Lesioni", GetType(String)))
        '71
        DT.Columns.Add(New DataColumn("Perc_Tot_Dif_Minori", GetType(String)))
        DT.Columns.Add(New DataColumn("Tasso_Riduzione_1", GetType(String)))
        DT.Columns.Add(New DataColumn("Tasso_Riduzione_2", GetType(String)))
        DT.Columns.Add(New DataColumn("Indice_Var_Prezzo", GetType(String)))
        DT.Columns.Add(New DataColumn("Premio_Pomo_Tardivo", GetType(String)))
        DT.Columns.Add(New DataColumn("Premio_Pomo_Bio", GetType(String)))
        DT.Columns.Add(New DataColumn("Prezzo_Unitario_Contratto", GetType(String)))
        DT.Columns.Add(New DataColumn("Prezzo_Unitario_Finale", GetType(String)))
        DT.Columns.Add(New DataColumn("Importo_Totale_Pag", GetType(String)))
        '79


        Return DT


    End Function


    '##############################################################
    Private Sub Valorizza_Riga_Precedente(ByRef objADDFun As AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni, _
                                         ByRef DT_Stampa As DataTable, _
                                            ByVal Anno As Integer)

        Select Case Anno

            Case 2011

                objADDFun.Calcola_Dati_Certificato_Pomodoro_2011(Peso_Netto_Kg, _
                                                         Perc_Marcio, _
                                                         Perc_Verde, _
                                                         Perc_Inerti, _
                                                        Perc_Tot_Dif_Magg_Round, _
                                                           Perc_Schiacciati, _
                                                           Perc_Immaturi, _
                                                           Perc_Scottature, _
                                                           Perc_Lesioni, _
                                                           Indice_Prezzo_Grado_Brix, _
                                                           Coefficiente, _
                                                           Franchigia, _
                                                           Prezzo_Unitario_Finale_Kg, _
                                                             Scarto_Q, _
                                                           Netto_Pag_Q, _
                                                           Perc_Tot_Dif_Maggiori, _
                                                           Perc_Tot_Dif_Minori, _
                                                           Dif_MinoriXCoeff, _
                                                           Magg_Rid_DifMaggiori, _
                                                           Indice_Variazione_Prezzo, _
                                                           Prezzo_Unitario_Finale_Tn, _
                                                           Importo_Totale_Pag)

            Case 2012
                '/////////////////////////////////////////////
                '////////////      2012       ////////////////
                '/////////////////////////////////////////////
                objADDFun.Calcola_Dati_Certificato_Pomodoro_2012(Peso_Netto_Kg, _
                                                          Perc_Marcio, _
                                                          Perc_Verde, _
                                                          Perc_Inerti, _
                                                         Perc_Tot_Dif_Magg_Round, _
                                                            Perc_Schiacciati, _
                                                            Perc_Immaturi, _
                                                            Perc_Scottature, _
                                                            Perc_Lesioni, _
                                                            Indice_Prezzo_Grado_Brix, _
                                                            Coefficiente, _
                                                            Franchigia, _
                                                            Prezzo_Unitario_Finale_Kg, _
                                                              Scarto_Q, _
                                                            Netto_Pag_Q, _
                                                            Perc_Tot_Dif_Maggiori, _
                                                            Perc_Tot_Dif_Minori, _
                                                            Dif_MinoriXCoeff, _
                                                            Magg_Rid_DifMaggiori, _
                                                            Indice_Variazione_Prezzo, _
                                                            Prezzo_Unitario_Finale_Tn, _
                                                            Importo_Totale_Pag)

            Case 2013
                '/////////////////////////////////////////////
                '////////////      2013       ////////////////
                '/////////////////////////////////////////////
                objADDFun.Calcola_Dati_Certificato_Pomodoro_2013(Peso_Netto_Kg, _
                                     Perc_Marcio, _
                                     Perc_Verde, _
                                     Perc_Inerti, _
                                    Perc_Tot_Dif_Magg_Round, _
                                       Perc_Schiacciati, _
                                       Perc_Immaturi, _
                                       Perc_Scottature, _
                                       Perc_Lesioni, _
                                       Grado_Brix, _
                                       Indice_Prezzo_Grado_Brix, _
                                       Coefficiente, _
                                       Franchigia, _
                                       Prezzo_Unitario_Finale_Kg, _
                                         Scarto_Q, _
                                       Netto_Pag_Q, _
                                       Perc_Tot_Dif_Maggiori, _
                                       Perc_Tot_Dif_Minori, _
                                       Dif_MinoriXCoeff, _
                                       Magg_Rid_DifMaggiori, _
                                       Indice_Variazione_Prezzo, _
                                       Prezzo_Unitario_Finale_Tn, _
                                       Importo_Totale_Pag)

                DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Indice_prezzo_Grado_Brix") = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

            Case 2014
                '/////////////////////////////////////////////
                '////////////      2014      ////////////////
                '/////////////////////////////////////////////
                objADDFun.Calcola_Dati_Certificato_Pomodoro_2014(Peso_Netto_Kg, _
                                     Perc_Marcio, _
                                     Perc_Verde, _
                                     Perc_Inerti, _
                                    Perc_Tot_Dif_Magg_Round, _
                                       Perc_Schiacciati, _
                                       Perc_Immaturi, _
                                       Perc_Scottature, _
                                       Perc_Lesioni, _
                                       Grado_Brix, _
                                       Indice_Prezzo_Grado_Brix, _
                                       Coefficiente, _
                                       Franchigia, _
                                       Prezzo_Unitario_Finale_Kg, _
                                         Scarto_Q, _
                                       Netto_Pag_Q, _
                                       Perc_Tot_Dif_Maggiori, _
                                       Perc_Tot_Dif_Minori, _
                                       Dif_MinoriXCoeff, _
                                       Magg_Rid_DifMaggiori, _
                                       Indice_Variazione_Prezzo, _
                                       Prezzo_Unitario_Finale_Tn, _
                                       Importo_Totale_Pag)

                DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Indice_prezzo_Grado_Brix") = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

            Case 2015
                '/////////////////////////////////////////////
                '////////////      2015       ////////////////
                '/////////////////////////////////////////////
                objADDFun.Calcola_Dati_Certificato_Pomodoro_2015(Flag_Bio, _
                                    Prezzo_Unitario_Contratto, _
                                    Peso_Netto_Kg, _
                                     Perc_Marcio, _
                                     Perc_Verde, _
                                     Perc_Inerti, _
                                    Perc_Tot_Dif_Magg_Round, _
                                       Perc_Schiacciati, _
                                       Perc_Immaturi, _
                                       Perc_Scottature, _
                                       Perc_Lesioni, _
                                       Grado_Brix, _
                                       Indice_Prezzo_Grado_Brix, _
                                       Coefficiente, _
                                       Franchigia, _
                                       Prezzo_Unitario_Finale_Kg, _
                                         Scarto_Q, _
                                       Netto_Pag_Q, _
                                       Perc_Tot_Dif_Maggiori, _
                                       Perc_Tot_Dif_Minori, _
                                       Dif_MinoriXCoeff, _
                                       Magg_Rid_DifMaggiori, _
                                       Indice_Variazione_Prezzo, _
                                       Prezzo_Unitario_Finale_Tn, _
                                       Importo_Totale_Pag)

                DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Indice_prezzo_Grado_Brix") = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

            Case Else

                objADDFun.Calcola_Dati_Certificato_Pomodoro_2015(Flag_Bio, _
                                    Prezzo_Unitario_Contratto, _
                                    Peso_Netto_Kg, _
                                     Perc_Marcio, _
                                     Perc_Verde, _
                                     Perc_Inerti, _
                                    Perc_Tot_Dif_Magg_Round, _
                                       Perc_Schiacciati, _
                                       Perc_Immaturi, _
                                       Perc_Scottature, _
                                       Perc_Lesioni, _
                                       Grado_Brix, _
                                       Indice_Prezzo_Grado_Brix, _
                                       Coefficiente, _
                                       Franchigia, _
                                       Prezzo_Unitario_Finale_Kg, _
                                         Scarto_Q, _
                                       Netto_Pag_Q, _
                                       Perc_Tot_Dif_Maggiori, _
                                       Perc_Tot_Dif_Minori, _
                                       Dif_MinoriXCoeff, _
                                       Magg_Rid_DifMaggiori, _
                                       Indice_Variazione_Prezzo, _
                                       Prezzo_Unitario_Finale_Tn, _
                                       Importo_Totale_Pag)

                DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Indice_prezzo_Grado_Brix") = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")


        End Select


        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Marcio") = Format(Perc_Marcio, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Verde") = Format(Perc_Verde, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Inerti") = Format(Perc_Inerti, "#,###,##0.000")
        'DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Tot_Dif_Magg") = Format(Perc_Tot_Dif_Magg, "#,###,##0.000")
        'DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Tot_Dif_Magg_Round") = Format(Perc_Tot_Dif_Magg_Round, "#,###,##0.000")
        'stampo direttamente il totale arrotondato
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Tot_Dif_Magg") = Format(Perc_Tot_Dif_Magg_Round, "#,###,##0.000")

        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Schiacciati") = Format(Perc_Schiacciati, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Immaturi") = Format(Perc_Immaturi, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Scottature") = Format(Perc_Scottature, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Lesioni") = Format(Perc_Lesioni, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Perc_Tot_Dif_Minori") = Format(Perc_Tot_Dif_Minori, "#,###,##0.000")

        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Scarto") = Format(Scarto_Q, "#,###,##0.00")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Netto_Pag") = Format(Netto_Pag_Q, "#,###,##0.00")

        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Tasso_Riduzione_1") = Format(Dif_MinoriXCoeff, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Tasso_Riduzione_2") = Format(Magg_Rid_DifMaggiori, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Indice_Var_Prezzo") = Format(Indice_Variazione_Prezzo, "#,###,##0.000")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Prezzo_Unitario_Contratto") = Format(Prezzo_Unitario_Contratto, "#,###,##0.00")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Prezzo_Unitario_Finale") = Format(Prezzo_Unitario_Finale_Tn, "#,###,##0.00")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Importo_Totale_Pag") = Format(Importo_Totale_Pag, "#,###,##0.00")

        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Premio_Pomo_Bio") = Format(Premio_Pomo_Bio, "#,###,##0.00")
        DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Premio_Pomo_Tardivo") = Format(Premio_Pomo_Tardivo, "#,###,##0.00")

    End Sub


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable)

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0


        If IsNothing(Dt_Finale) Then
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
            Me.TableExcel.Rows.Add(Riga)
            Exit Sub
        End If

        Numero_Colonne = CInt(Dt_Finale.Columns.Count)


        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Try

            Dim i, j, k As Integer
            Dim Colonna_Dt As String

            '################################################################
            'PRIMA RIGA - TITOLO
            ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "white", "center", "middle")
            Me.TableExcel.Rows(0).Cells(0).InnerHtml = "INFORMAZIONI RELATIVE ALLA CONSEGNA ALLE INDUSTRIE DI PRODOTTI ORTOFRUTTICOLI SOGGETTI AD AIUTO COMUNITARIO"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "12px"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"
            '################################################################
            'SECONDA RIGA - SEZIONI
            Riga = New HtmlTableRow

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = R2_Numero
            Riga.Cells(0).Style.Item("font-weight") = "bold"
            Riga.Cells(0).Style.Item("font-size") = "12px"
            Riga.Cells(0).Style.Item("text-align") = "center"
            Riga.Cells(0).Style.Item("vertical-align") = "middle"
            Riga.Cells(0).InnerHtml = ""

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(1).ColSpan = R2_Contratto
            Riga.Cells(1).Style.Item("font-weight") = "bold"
            Riga.Cells(1).Style.Item("font-size") = "12px"
            Riga.Cells(1).Style.Item("text-align") = "center"
            Riga.Cells(1).Style.Item("vertical-align") = "middle"
            Riga.Cells(1).InnerHtml = "ESTREMI DEL CONTRATTO E DEI CONTRAENTI"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(2).ColSpan = R2_DDT
            Riga.Cells(2).Style.Item("font-weight") = "bold"
            Riga.Cells(2).Style.Item("font-size") = "12px"
            Riga.Cells(2).Style.Item("text-align") = "center"
            Riga.Cells(2).Style.Item("vertical-align") = "middle"
            Riga.Cells(2).InnerHtml = "ESTREMI DEL DOCUMENTO DI TRASPORTO"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(3).ColSpan = R2_Scarico
            Riga.Cells(3).Style.Item("font-weight") = "bold"
            Riga.Cells(3).Style.Item("font-size") = "12px"
            Riga.Cells(3).Style.Item("text-align") = "center"
            Riga.Cells(3).Style.Item("vertical-align") = "middle"
            Riga.Cells(3).InnerHtml = "VERIFICA DI SCARICO"

            Me.TableExcel.Rows.Add(Riga)

            '################################################################
            'TERZA RIGA - SOTTOSEZIONI

            Riga = New HtmlTableRow

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = R3_Contratto
            Riga.Cells(0).Style.Item("font-weight") = "bold"
            Riga.Cells(0).Style.Item("font-size") = "12px"
            Riga.Cells(0).Style.Item("text-align") = "center"
            Riga.Cells(0).Style.Item("vertical-align") = "middle"
            Riga.Cells(0).InnerHtml = ""

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(1).ColSpan = R3_OP
            Riga.Cells(1).Style.Item("font-weight") = "bold"
            Riga.Cells(1).Style.Item("font-size") = "12px"
            Riga.Cells(1).Style.Item("text-align") = "center"
            Riga.Cells(1).Style.Item("vertical-align") = "middle"
            Riga.Cells(1).InnerHtml = "ORGANIZZAZIONE DEI PRODUTTORI"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(2).ColSpan = R3_Produttore
            Riga.Cells(2).Style.Item("font-weight") = "bold"
            Riga.Cells(2).Style.Item("font-size") = "12px"
            Riga.Cells(2).Style.Item("text-align") = "center"
            Riga.Cells(2).Style.Item("vertical-align") = "middle"
            Riga.Cells(2).InnerHtml = "PRODUTTORE CONFERENTE"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(3).ColSpan = R3_Coop
            Riga.Cells(3).Style.Item("font-weight") = "bold"
            Riga.Cells(3).Style.Item("font-size") = "12px"
            Riga.Cells(3).Style.Item("text-align") = "center"
            Riga.Cells(3).Style.Item("vertical-align") = "middle"
            Riga.Cells(3).InnerHtml = "COOPERATIVA"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(4).ColSpan = R3_CentroRaccolta
            Riga.Cells(4).Style.Item("font-weight") = "bold"
            Riga.Cells(4).Style.Item("font-size") = "12px"
            Riga.Cells(4).Style.Item("text-align") = "center"
            Riga.Cells(4).Style.Item("vertical-align") = "middle"
            Riga.Cells(4).InnerHtml = "CENTRO DI LAVORAZIONE / RACCOLTA"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(5).ColSpan = R3_Industria
            Riga.Cells(5).Style.Item("font-weight") = "bold"
            Riga.Cells(5).Style.Item("font-size") = "12px"
            Riga.Cells(5).Style.Item("text-align") = "center"
            Riga.Cells(5).Style.Item("vertical-align") = "middle"
            Riga.Cells(5).InnerHtml = "INDUSTRIA DESTINATARIA E ASS.NE IND."

            'Riga.Cells.Add(New HtmlTableCell)
            'Riga.Cells(6).ColSpan = R3_Ass
            'Riga.Cells(6).Style.Item("font-weight") = "bold"
            'Riga.Cells(6).Style.Item("font-size") = "12px"
            'Riga.Cells(6).Style.Item("text-align") = "center"
            'Riga.Cells(6).Style.Item("vertical-align") = "middle"
            'Riga.Cells(6).InnerHtml = "ASS.NE"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(6).ColSpan = R3_DDT
            Riga.Cells(6).Style.Item("font-weight") = "bold"
            Riga.Cells(6).Style.Item("font-size") = "12px"
            Riga.Cells(6).Style.Item("text-align") = "center"
            Riga.Cells(6).Style.Item("vertical-align") = "middle"
            Riga.Cells(6).InnerHtml = "DDT"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(7).ColSpan = R3_Qta
            Riga.Cells(7).Style.Item("font-weight") = "bold"
            Riga.Cells(7).Style.Item("font-size") = "12px"
            Riga.Cells(7).Style.Item("text-align") = "center"
            Riga.Cells(7).Style.Item("vertical-align") = "middle"
            Riga.Cells(7).InnerHtml = "RIDUZIONI SU QUANTITA'"

            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(8).ColSpan = R3_Prezzo
            Riga.Cells(8).Style.Item("font-weight") = "bold"
            Riga.Cells(8).Style.Item("font-size") = "12px"
            Riga.Cells(8).Style.Item("text-align") = "center"
            Riga.Cells(8).Style.Item("vertical-align") = "middle"
            Riga.Cells(8).InnerHtml = "VARIAZIONI SU PREZZO"

            Me.TableExcel.Rows.Add(Riga)
            '################################################################

            If Not IsNothing(Dt_Finale) Then

                Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For k = 0 To Dt_Finale.Columns.Count - 1

                    Riga.Cells.Add(New HtmlTableCell)
                    ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("vertical-align") = "middle"
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"
                    'Riga.Cells(k).Height = "26"

                    'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                    Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                    Colonna_Dt = Riga.Cells(k).InnerHtml.ToLower

                    Select Case Colonna_Dt

                        Case "num_Certificato".ToLower
                            Riga.Cells(k).InnerHtml = "Numero<br/>Certificato"
                        Case "data_Certificato".ToLower
                            Riga.Cells(k).InnerHtml = "Data Certificato"
                        Case "num_bolla"
                            Riga.Cells(k).InnerHtml = "Numero<br/>Bolla"
                        Case "data_bolla"
                            Riga.Cells(k).InnerHtml = "Data Bolla"
                        Case "Cod_OPR_Contratto".ToLower
                            Riga.Cells(k).InnerHtml = "Cod. OPR<br/>Contratto"
                        Case "Num_Contratto".ToLower
                            Riga.Cells(k).InnerHtml = "Numero<br/>Contratto"
                        Case "Data_Contratto".ToLower
                            Riga.Cells(k).InnerHtml = "Data Contratto"
                        Case "Num_Clausola".ToLower
                            Riga.Cells(k).InnerHtml = "Numero<br/>Clausola"
                        Case "Data_Clausola".ToLower
                            Riga.Cells(k).InnerHtml = "Data Clausola"
                        Case "Prodotto".ToLower
                            Riga.Cells(k).InnerHtml = "Prodotto"
                        Case "Tipologia".ToLower
                            Riga.Cells(k).InnerHtml = "Tipologia"
                        Case "Codice_OP".ToLower
                            Riga.Cells(k).InnerHtml = "Codice OP"
                        Case "Piva_OP".ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA OP"
                        Case "Cod_Fisc_OP".ToLower
                            Riga.Cells(k).InnerHtml = "Cod. Fiscale OP"
                        Case "rag_soc_op"
                            Riga.Cells(k).InnerHtml = "Ragione Sociale OP"
                        Case "Codice_Unione_OP".ToLower
                            Riga.Cells(k).InnerHtml = "Codice Unione OP"
                        Case "Rag_Soc_Unione_OP".ToLower
                            Riga.Cells(k).InnerHtml = "Ragione Sociale Unione OP"
                        Case CStr("Piva_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("Rag_Soc_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Ragione Sociale Produttore"
                        Case "Cod_Fisc_Produttore".ToLower
                            Riga.Cells(k).InnerHtml = "Cod. Fiscale<br/>Produttore"
                        Case "indirizzo_Produttore".ToLower
                            Riga.Cells(k).InnerHtml = "Indirizzo<br/>Produttore"
                        Case "frazione_Produttore".ToLower
                            Riga.Cells(k).InnerHtml = "Frazione<br/>Produttore"
                        Case "comune_prov_Produttore".ToLower
                            Riga.Cells(k).InnerHtml = "Comune-Prov.<br/>Produttore"
                        Case "cap_Produttore".ToLower
                            Riga.Cells(k).InnerHtml = "CAP<br/>Produttore"
                        Case CStr("Piva_coop").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Cooperativa"
                        Case "Cod_Fisc_coop".ToLower
                            Riga.Cells(k).InnerHtml = "Cod. Fiscale<br/>Cooperativa"
                        Case CStr("Rag_Soc_coop").ToLower
                            Riga.Cells(k).InnerHtml = "Ragione Sociale Cooperativa"
                        Case "indirizzo_coop".ToLower
                            Riga.Cells(k).InnerHtml = "Indirizzo<br/>Cooperativa"
                        Case "frazione_coop".ToLower
                            Riga.Cells(k).InnerHtml = "Frazione<br/>Cooperativa"
                        Case "comune_prov_coop".ToLower
                            Riga.Cells(k).InnerHtml = "Comune-Prov.<br/>Cooperativa"
                        Case "cap_coop".ToLower
                            Riga.Cells(k).InnerHtml = "CAP<br/>Cooperativa"
                        Case CStr("Piva_CentroLav").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Centro Lavorazione"
                        Case CStr("Rag_Soc_centrolav").ToLower
                            Riga.Cells(k).InnerHtml = "Rag. Soc.<br/>Centro Lavorazione"
                        Case "codice_trasformatore"
                            Riga.Cells(k).InnerHtml = "Codice<br/>Trasformatore"
                        Case "Codice_Stabilimento".ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Stabilimento"
                        Case CStr("Piva_Trasformatore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Trasformatore"
                        Case CStr("Rag_Soc_Trasformatore").ToLower
                            Riga.Cells(k).InnerHtml = "Rag. Soc. Trasformatore"
                        Case "Cod_Fisc_Trasformatore".ToLower
                            Riga.Cells(k).InnerHtml = "Cod.Fiscale<br/>Trasformatore"
                        Case "indirizzo_Trasformatore".ToLower
                            Riga.Cells(k).InnerHtml = "Indirizzo<br/>Trasformatore"
                        Case "comune_prov_Trasformatore".ToLower
                            Riga.Cells(k).InnerHtml = "Comune-Prov.<br/>Trasformatore"
                        Case "cap_Trasformatore".ToLower
                            Riga.Cells(k).InnerHtml = "CAP<br/>Trasformatore"
                        Case "Codice_Ass_Industriale".ToLower
                            Riga.Cells(k).InnerHtml = "Codice Ass.ne<br/>Industriale"
                        Case "Ass_Industriale".ToLower
                            Riga.Cells(k).InnerHtml = "Ass.ne Industriale"
                        Case CStr("Rag_Soc_Vettore").ToLower
                            Riga.Cells(k).InnerHtml = "Vettore"
                        Case CStr("targa_automezzo").ToLower
                            Riga.Cells(k).InnerHtml = "Targa<br/>Automezzo"
                        Case CStr("targa_rimorchio").ToLower
                            Riga.Cells(k).InnerHtml = "Targa<br/>Rimorchio"
                        Case "num_ddt"
                            Riga.Cells(k).InnerHtml = "Numero DDT<br/>Consegna"
                        Case "data_ddt"
                            Riga.Cells(k).InnerHtml = "Data DDT<br/>Consegna"
                        Case "Data_Accettazione".ToLower
                            Riga.Cells(k).InnerHtml = "Data Scarico"
                        Case "Data_Accettazione".ToLower
                            Riga.Cells(k).InnerHtml = "Data Scarico"
                        Case "ora_Accettazione".ToLower
                            Riga.Cells(k).InnerHtml = "Ora Scarico"
                        Case "Num_Tagliando_Pesa".ToLower
                            Riga.Cells(k).InnerHtml = "Numero<br/>Tagliando Pesa"
                        Case "Tondo_X".ToLower
                            Riga.Cells(k).InnerHtml = "Tondo"
                        Case "Allungato_X".ToLower
                            Riga.Cells(k).InnerHtml = "Allungato"
                        Case "Desc_Appezza".ToLower
                            Riga.Cells(k).InnerHtml = "Appezzamento"
                        Case "Varieta_Distretto".ToLower
                            Riga.Cells(k).InnerHtml = "Varieta'"
                        Case "Biologico".ToLower
                            Riga.Cells(k).InnerHtml = "Biologico"
                        Case "Peso_Lordo".ToLower
                            Riga.Cells(k).InnerHtml = "Peso Lordo"
                        Case "Tara_Veicolo".ToLower
                            Riga.Cells(k).InnerHtml = "Tara Veicolo"
                        Case "Tara_Imballi".ToLower
                            Riga.Cells(k).InnerHtml = "Tara Imballi"
                        Case "Peso_Netto".ToLower
                            Riga.Cells(k).InnerHtml = "Peso Netto"
                        Case "Grado_Brix".ToLower
                            Riga.Cells(k).InnerHtml = "Residuo Ottico<br/>Grado Brix"
                        Case "Indice_prezzo_Grado_Brix".ToLower
                            Riga.Cells(k).InnerHtml = "Indice Prezzo<br/>Grado Brix"
                        Case "Perc_Marcio".ToLower
                            Riga.Cells(k).InnerHtml = "% Marcio"
                        Case "Perc_Verde".ToLower
                            Riga.Cells(k).InnerHtml = "% Verde"
                        Case "Perc_Inerti".ToLower
                            Riga.Cells(k).InnerHtml = "% Inerti"
                        Case "Perc_Tot_Dif_Magg".ToLower
                            Riga.Cells(k).InnerHtml = "Tasso Riduzione %"
                        Case "Perc_Schiacciati".ToLower
                            Riga.Cells(k).InnerHtml = "% Schiacciati"
                        Case "Perc_Immaturi".ToLower
                            Riga.Cells(k).InnerHtml = "% Immaturi"
                        Case "Perc_Scottature".ToLower
                            Riga.Cells(k).InnerHtml = "% Scottature"
                        Case "Perc_Lesioni".ToLower
                            Riga.Cells(k).InnerHtml = "% Lesioni"
                        Case "Perc_Tot_Dif_Minori".ToLower
                            Riga.Cells(k).InnerHtml = "Totale<br/>Percentuali"
                        Case "Netto_Pag".ToLower
                            Riga.Cells(k).InnerHtml = "Peso Netto a<br>Pagamento"
                        Case "Tasso_Riduzione_1".ToLower
                            Riga.Cells(k).InnerHtml = "Pari ad una<br/>Riduzione %"
                        Case "Tasso_Riduzione_2".ToLower
                            Riga.Cells(k).InnerHtml = "Ulteriore<br/>Riduzione %"
                        Case "Indice_Var_Prezzo".ToLower
                            Riga.Cells(k).InnerHtml = "Indice base 100<br/>Variazione Prezzo"
                        Case "Prezzo_Unitario_Contratto".ToLower
                            Riga.Cells(k).InnerHtml = "Prezzo Unitario<br/>da Contratto"
                        Case "Prezzo_Unitario_Finale".ToLower
                            Riga.Cells(k).InnerHtml = "Prezzo Unitario<br/>Finale"
                        Case "Importo_Totale_Pag".ToLower
                            Riga.Cells(k).InnerHtml = "Importo Totale<br/>a Pagamento"
                        Case "Premio_Pomo_Tardivo".ToLower
                            Riga.Cells(k).InnerHtml = "Premio Pomodoro<br/>Tardivo"
                        Case "Premio_Pomo_Bio".ToLower
                            Riga.Cells(k).InnerHtml = "Premio Pomodoro<br/>Biologico"
                    End Select

                Next

                Me.TableExcel.Rows.Add(Riga)
                '----------------------------------
                '------- FINE INTESTAZIONE --------
                '----------------------------------

                '----------------------------------
                '------ RIEMPIMENTO TABELLA -------
                '----------------------------------

                'scorro le righe
                For i = 0 To Dt_Finale.Rows.Count - 1

                    Riga = New HtmlTableRow

                    'scorro le colonne
                    For j = 0 To Dt_Finale.Columns.Count - 1

                        'aggiungo la cella
                        Riga.Cells.Add(New HtmlTableCell)
                        Riga.Cells(j).Style.Item("text-align") = "center"
                        Riga.Cells(j).Style.Item("vertical-align") = "middle"
                        'Riga.Cells(j).Height = "26"

                        Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                            Case CStr("Piva_OP").ToLower, _
                                CStr("Cod_Fisc_OP").ToLower, _
                                CStr("Piva_Produttore").ToLower, _
                                CStr("Cod_Fisc_Produttore").ToLower, _
                                CStr("Piva_Coop").ToLower, _
                                CStr("Cod_Fisc_Coop").ToLower, _
                                CStr("Piva_Trasformatore").ToLower, _
                                CStr("Cod_Fisc_Trasformatore").ToLower, _
                                CStr("Num_DDT").ToLower, _
                                CStr("Codice_Ass_Industriale").ToLower, _
                                CStr("Codice_OP").ToLower, _
                                CStr("Codice_Unione_OP").ToLower
                                'aggiungo uno spazio davanti x salvare gli zeri...
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            Case Else

                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        End Select

                    Next

                    'Aggiungo la Riga alla Tabella 
                    Me.TableExcel.Rows.Add(Riga)

                Next
                '-------------------------------
                '--------- FINE TABELLA --------
                '-------------------------------

            End If

        Catch ex As Exception

            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Creazione excel: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try


    End Sub




End Class

