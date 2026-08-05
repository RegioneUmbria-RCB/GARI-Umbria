Public Class PianoNutrizionale_Bilancio
    Public Sub New()

    End Sub
    Public Function CalcolaPianoNutrizionaleBilancio(ByVal BilancioInput As PianoNutrizionaleBilancio_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                        As PianoNutrizionaleBilancio_output


        'SE l'azienda è BIOLOGICA
        '   N = N_calcolato - 60
        'ALTRIMENTI
        '   SE NTot >= 40 
        '       SE NOrg >= 5 
        '           N = 0
        '       ALTRIMENTI (quindi NOrg < 5)
        '           N = 40
        '   SE NTot >= 30 
        '       SE NOrg >= 5 
        '           N = 40
        '       ALTRIMENTI (quindi NOrg < 5)
        '           N = 60
        '   ALTRIMENTI (quindi NTot < 30)
        '       N = N_Calcolato - 20


        Dim strErrore As String = ""
        Dim BilancioOutput As New PianoNutrizionaleBilancio_output

        If BilancioInput.Regolamento_Cod = 0 Then
            strErrore &= "Parametro input Regolamento obbligatorio" & vbCrLf
        End If

        If BilancioInput.NTot = 0 Then
            strErrore &= "Parametro input NORG obbligatorio" & vbCrLf
        End If
        If BilancioInput.NOrg = 0 Then
            strErrore &= "Parametro input NTOT obbligatorio" & vbCrLf
        End If

        If strErrore <> "" Then
            BilancioOutput.MessaggioErrore = strErrore
            Return BilancioOutput
        End If


        '#####################################
        '###########   CALCOLO N   ###########
        '#####################################

        Dim N As Decimal
        Dim N_Calcolato As Decimal = CalcolaN_PianoNutrizionale(BilancioInput, objParametri)

        Select Case BilancioInput.Biologico
            Case 1
                N = N_Calcolato - 60
            Case Else
                Select Case BilancioInput.NTot
                    Case >= 40
                        Select Case BilancioInput.NOrg
                            Case >= 5
                                N = 0
                            Case < 5
                                N = 40
                        End Select
                    Case >= 30
                        Select Case BilancioInput.NOrg
                            Case >= 5
                                N = 40
                            Case < 5
                                N = 60
                        End Select
                    Case < 30
                        N = N_Calcolato - 20
                End Select
        End Select



        ''###################################
        ''###########   FINALEN   ########### 'to do....
        ''###################################

        'Dim FINALEN As Integer = 0
        'Dim COTOTN As Integer = N_Calcolato
        'Dim PROVINCIA As String = "" '???

        'Select Case PROVINCIA
        '    Case "Zone Normali"
        '        Select Case COTOTN
        '            Case < 0
        '                FINALEN = 0
        '            Case = 10
        '                FINALEN = 20
        '            Case > 140
        '                FINALEN = 140
        '        End Select
        'End Select



        '######################################
        '###########   TESTOAZOTO   ########### 
        '######################################
        'Dim FINALEN As Integer = N
        'Dim TestoAzoto As String
        'Select Case FINALEN
        '    Case < 30
        '        TestoAzoto = "di cui: in presemina = 0; in copertura = " & FINALEN & "." & vbCrLf
        '    Case 40 To 90
        '        TestoAzoto = "di cui: in presemina = " & FINALEN & "; in copertura = 0."
        '    Case 100
        '        TestoAzoto = "di cui: in presemina = 70; in copertura = 30."
        '    Case 110
        '        TestoAzoto = "di cui: in presemina = 80; in copertura = 30."
        '    Case 120
        '        TestoAzoto = "di cui: in presemina = 90; in copertura = 30."
        '    Case 130
        '        TestoAzoto = "di cui: in presemina = 90; in copertura = 40."
        '    Case 140
        '        TestoAzoto = "di cui: in presemina = 90; in copertura = 50."
        '    Case 150
        '        TestoAzoto = "di cui: in presemina = 100; in <copertura = 50."
        '    Case 160
        '        TestoAzoto = "di cui: in presemina = 100; in copertura = 60."
        '    Case 170
        '        TestoAzoto = "di cui: in presemina = 110; in copertura = 60."
        '    Case 180
        '        TestoAzoto = "di cui: in presemina = 110; in copertura = 70."
        'End Select


        'BilancioOutput.Consiglio_N = "N = " & FINALEN & " [kg/ha] " & TestoAzoto

        BilancioOutput.Consiglio_N = Math.Round((Math.Round(N, 1, MidpointRounding.AwayFromZero)) / 10D) * 10   '.ARROTONDA ALLE DECINE! 95.12181191105D ---> 100   // 93.12181191105D ---> 90
        BilancioOutput.Frazionamento_N = ""

        If BilancioInput.CODAPIOVPRIM > 0 Then
            BilancioOutput.Integrazione_N = Math.Round((Math.Round(BilancioInput.CODAPIOVPRIM, 1, MidpointRounding.AwayFromZero)) / 10D) * 10   '.ARROTONDA ALLE DECINE! 95.12181191105D ---> 100   // 93.12181191105D ---> 90     //Math.Round(BilancioInput.CODAPIOVPRIM, 1, MidpointRounding.AwayFromZero)   
        End If

        '#######################################
        '###########   CONSIGLIO P   ########### 
        '#######################################

        'Dim FINALEP As String
        'Dim stringaA_P As String = ""
        'Dim stringaB_P As String = ""

        'Select Case BilancioInput.P2O5
        '    Case 1 To 10
        '        stringaA_P = "80-120"
        '        stringaB_P = "50"
        '    Case 11 To 20
        '        stringaA_P = "40-80" '(?)
        '        stringaB_P = "50"
        '    Case > 20
        '        stringaA_P = "-"
        '        stringaB_P = "40-70"
        '    Case 0
        '        stringaA_P = "MANCA UN VALORE! <StringaA_P>"
        '        stringaB_P = "MANCA UN VALORE! <StringaB_P>"
        'End Select

        'If stringaA_P = "-" Then
        '    FINALEP = "Localizzati: " & stringaB_P
        'Else
        '    FINALEP = stringaA_P & " di cui: localizzati: " & stringaB_P
        'End If

        'BilancioOutput.Consiglio_P = FINALEP


        ''#######################################
        ''###########   CONSIGLIO K   ########### 
        ''#######################################

        'Dim FINALEK As Decimal
        'Select Case BilancioInput.K2O
        '    Case Nothing
        '        '(?)
        '    Case < 80
        '        FINALEK = 120
        '    Case 80 To 120
        '        FINALEK = 80
        '    Case Else
        '        FINALEK = 0
        'End Select
        'BilancioOutput.Consiglio_K = FINALEK


        BilancioOutput.N_Ammesso = Math.Round((Math.Round(N, 1, MidpointRounding.AwayFromZero)) / 10D) * 10   '.ARROTONDA ALLE DECINE! 95.12181191105D ---> 100   // 93.12181191105D ---> 90
        'BilancioOutput.P_Ammesso = FINALEP
        'BilancioOutput.K_Ammesso = FINALEK

        Return BilancioOutput

    End Function
    Public Function CalcolaN_PianoNutrizionale(ByVal BilancioInput As PianoNutrizionaleBilancio_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim BilancioOutput As New PianoNutrizionaleBilancio_output
        Dim strErrore As String = ""


        '#################################
        '#########    CODANCL    #########
        '#################################
        'dim CODANCL As Decimal = ..... formula (utilizza NTot ed NOrg dell'oggetto input)

        Dim CODANCL As Decimal
        Dim CODANCL_A As Decimal
        Dim CODANCL_B As Decimal

        Select Case BilancioInput.NTot
            Case 0.1 To 35
                CODANCL_A = 249.51 * (2.71) ^ (-0.0699 * BilancioInput.NTot)
                Select Case BilancioInput.NOrg / BilancioInput.NTot
                    Case < 0.4
                        CODANCL_B = 3 * (BilancioInput.NTot / BilancioInput.NOrg)  'MAX =  30
                    Case Else
                        CODANCL_B = 0 ' (?)
                End Select
                CODANCL = CODANCL_A + CODANCL_B
            Case Else
                CODANCL = 0
        End Select



        '#################################
        '#########    CODAORG    #########
        '#################################

        'dim CODAORG As Decimal = pc_MatriciOrganicheXFrequenza.n (lettura della tabella pc_MatriciOrganicheXFrequenza passando id_mat_o = FertilizzanteOrganico_Precedente_Tipo e regolamento_cod)
        Dim CODAORG As Decimal = 0

        If BilancioInput.FertilizzanteOrganico_Precedente_Tipo > 0 Then
            Dim Dt_CODAORG As DataTable
        Dim obj_CODAORG As New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R
        Dt_CODAORG = obj_CODAORG.Leggi(BilancioInput.Regolamento_Cod,
                           BilancioInput.FertilizzanteOrganico_Precedente_Tipo, 0,
                           "", "", objParametri)


            If Dt_CODAORG.Rows.Count > 0 Then
                CODAORG = Dt_CODAORG.Rows(0).Item("N")
            End If
        End If



        '###################################
        '#########  COD_TESSITURA  ######### 
        '###################################

        'Dim COD_TESSITURA as decimal = dati sabbia e argilla leggo la tabella PC_TriangoloTessitura e ricavo id_tessitura
        '                               dato id_tessitura e regolamento_cod leggo PC_Tessiture 

        '                               tessitura < 4 OVVERO corrisponde a id_tessitura in (12,11,3,9,6)
        '                               molto forte (Argillosa -  Argilloso Limosa) 
        '                               forte (Limosa)
        '                               medio forte	(Franco Limosa Argillosa - Franco Limosa)

        'Id_Tessitura       Tessitura_Sim	    Tessitura_Des
        '1                  S	                Sabbioso
        '2                  SF	                Sabbioso Franco
        '3                  L	                Limoso
        '4                  FS	                Franco Sabbioso
        '5                  F	                Franco
        '6                  FL	                Franco Limoso
        '7                  FSA                 Franco Sabbioso Argilloso
        '8                  FA	                Franco Argilloso
        '9                  FLA	                Franco Limoso Argilloso
        '10                 AS	                Argilloso Sabbioso
        '11                 AL	                Argilloso Limoso
        '12                 A	                Argilloso

        Dim ID_Tessitura As Int32
        Dim obj_TESSITURA As New AgronicaCoreMetaSchemaDAL.PC_TriangoloTessitura_R

        ID_Tessitura = obj_TESSITURA.Leggi_IdTessitura(BilancioInput.Sabbia,
                                                       BilancioInput.Argilla,
                                                       objParametri)

        '####################################
        '#########  CODPRECESSIONE  #########
        '####################################

        'dim CODPRECESSIONE As Decimal = somme di pc_precessionecolturalexfrequenza.n  (lettura della tabella pc_precessionecolturalexfrequenza passando PrecessioneSpecie_Cod, PrecessioneSpecie_Cod_AnnoPrecedente, PrecessioneSpecie_Cod_DueAnniPrima, PrecessioneSpecie_Cod_TreAnniPrima e regolamento_cod)

        Dim CODPRECESSIONE As Integer = 0
        Dim obj_PRECESSIONE As New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturalexFrequenza_R


        ' ---- PRECESSIONE
        If BilancioInput.PrecessioneSpecie_Cod > 0 Then
            Select Case BilancioInput.PrecessioneSpecie_Cod
                Case BilancioInput.PrecessioneSpecie_Cod = 2 AndAlso ID_Tessitura = 12 Or 11 Or 3 Or 9 Or 6
                    CODPRECESSIONE = 20
                Case Else
                    Dim Dt_PRECESSIONE As DataTable
                    Dt_PRECESSIONE = obj_PRECESSIONE.Leggi(BilancioInput.Regolamento_Cod,
                                       BilancioInput.PrecessioneSpecie_Cod, 1,
                                       "", "", objParametri)

                    If Dt_PRECESSIONE.Rows.Count > 0 Then
                        CODPRECESSIONE += Dt_PRECESSIONE.Rows(0).Item("N")
                    End If
            End Select
        End If


        ' ---- PRECESSIONE ANNO PRECEDENTE
        If BilancioInput.PrecessioneSpecie_Cod_AnnoPrecedente > 0 Then
            Dim Dt_PRECESSIONE_MENO1 As DataTable
        Dt_PRECESSIONE_MENO1 = obj_PRECESSIONE.Leggi(BilancioInput.Regolamento_Cod,
                           BilancioInput.PrecessioneSpecie_Cod_AnnoPrecedente, 2,
                           "", "", objParametri)

            If Dt_PRECESSIONE_MENO1.Rows.Count > 0 Then
                CODPRECESSIONE += Dt_PRECESSIONE_MENO1.Rows(0).Item("N")
            End If
        End If


        If BilancioInput.PrecessioneSpecie_Cod_DueAnniPrima > 0 Then
            ' ---- PRECESSIONE 2 ANNI PRIMA
            Dim Dt_PRECESSIONE_MENO2 As DataTable
        Dt_PRECESSIONE_MENO2 = obj_PRECESSIONE.Leggi(BilancioInput.Regolamento_Cod,
                           BilancioInput.PrecessioneSpecie_Cod_DueAnniPrima, 3,
                           "", "", objParametri)

            If Dt_PRECESSIONE_MENO2.Rows.Count > 0 Then
                CODPRECESSIONE += Dt_PRECESSIONE_MENO2.Rows(0).Item("N")
            End If
        End If


        If BilancioInput.PrecessioneSpecie_Cod_TreAnniPrima > 0 Then
            ' ---- PRECESSIONE 3 ANNI PRIMA
            Dim Dt_PRECESSIONE_MENO3 As DataTable
        Dt_PRECESSIONE_MENO3 = obj_PRECESSIONE.Leggi(BilancioInput.Regolamento_Cod,
                           BilancioInput.PrecessioneSpecie_Cod_TreAnniPrima, 4,
                           "", "", objParametri)

            If Dt_PRECESSIONE_MENO3.Rows.Count > 0 Then
                CODPRECESSIONE += Dt_PRECESSIONE_MENO3.Rows(0).Item("N")
            End If
        End If



        '#################################################
        '#########  CONSDAPIOGGIA + KGIA + KTES  #########
        '#################################################

        ' ---- KGIA
        'Dim KGIA as decimal = PC_Ubicazione.coefficiente_giacitura (lettura PC_Ubicazione passando Ubicazione_Cod e regolamento_cod)
        Dim KGIA As Decimal = 0

        If BilancioInput.Ubicazione_Cod > 0 Then
            Dim Dt_KGIA As DataTable
            Dim obj_KGIA As New AgronicaCoreMetaSchemaDAL.PC_Ubicazione_R
            Dt_KGIA = obj_KGIA.Leggi(BilancioInput.Regolamento_Cod,
                           BilancioInput.Ubicazione_Cod,
                           "", "", objParametri)

            If Dt_KGIA.Rows.Count > 0 Then
                KGIA = Dt_KGIA.Rows(0).Item("Coefficiente_Giacitura")
            End If
        End If


        ' ---- KTES
        'Dim KTES as decimal = PC_Tessiture.coefficiente (lettura PC_tessiture passando id_tessitura e regolamento_cod, id_tessitura lo avevo ricavato ricavo da PC_TriangoloTessitura passando sabbia e argilla)
        Dim Dt_KTES As DataTable
        Dim obj_KTES As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R

        Dt_KTES = obj_KTES.Leggi(ID_Tessitura,
                           " Regolamento_Cod =" & BilancioInput.Regolamento_Cod, "", objParametri)

        Dim KTES As Decimal
        If Dt_KTES.Rows.Count > 0 Then
            KTES = Dt_KTES.Rows(0).Item("Coefficiente")
        End If

        ' ---- CONSDAPIOGGIA
        'dim CONSDAPIOGGIA As Decimal = select case pioggia_autunnale 
        '<200
        '   consdapioggia =0
        ' tra 200 500  formula
        '   consdapioggia = (0,1*pioggia_autunnale-20)*(1+KGIA+KTESS)
        ' sopra 500  formula

        '   consdapioggia = 30*(1+KGIA+KTESS)

        Dim CONSDAPIOGGIA As Decimal
        Select Case BilancioInput.PioggiaMM_Autunno
            Case 0 To 200
                CONSDAPIOGGIA = 0
            Case 200 To 500
                CONSDAPIOGGIA = (0.1 * BilancioInput.PioggiaMM_Autunno - 20) * (1 + KGIA + KTES)
            Case > 500
                CONSDAPIOGGIA = 30 * (1 + KGIA + KTES)
            Case Else
                CONSDAPIOGGIA = 0
                strErrore &= "SEGNALO ERRORE" & vbCrLf  'to do...
        End Select



        '##################################
        '#########  CODAPIOVPRIM  #########
        '##################################

        Dim CODAPIOVPRIM As Decimal

        Select Case BilancioInput.PioggiaMM_Primavera
            Case < 80
                CODAPIOVPRIM = 0
            Case 80 To 200
                CODAPIOVPRIM = (-0.0023 * BilancioInput.PioggiaMM_Primavera ^ 2 +
                                0.8864 * BilancioInput.PioggiaMM_Primavera - 56.1) *
                                (1 + KGIA + KTES)
            Case > 200
                CODAPIOVPRIM = 30 * (1 + KGIA + KTES)
            Case Else
                strErrore &= "SEGNALO ERRORE" & vbCrLf  'to do...
        End Select

        If CODAPIOVPRIM <> 0 Then
            BilancioInput.CODAPIOVPRIM = CODAPIOVPRIM
        End If

        If strErrore <> "" Then
            BilancioOutput.MessaggioErrore = strErrore
        End If

        '#######################
        '#########  A  #########
        '#######################

        'dim A As Decimal = CODANCL + CODAORG + CODPRECESSIONE + CONSDAPIOGGIA
        Dim A As Decimal = CODANCL + CODAORG + CODPRECESSIONE + CONSDAPIOGGIA

        '################################
        '#########    COTOTN    #########
        '################################

        'Dim COTOTN As Decimal = (A/10 + 0,5) *10
        Dim COTOTN As Decimal = (A / 10 + 0.5) * 10

        Dim N = COTOTN

        Return N

    End Function

End Class

#Region "INPUT"
Public Class PianoNutrizionaleBilancio_input

    Public Regolamento_Cod As Integer
    Public Biologico As Boolean

    Public Argilla As Decimal
    Public Sabbia As Decimal
    Public Limo As Decimal

    Public NTot As Decimal
    Public NOrg As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal

    Public PioggiaMM_Autunno As Decimal
    Public PioggiaMM_Primavera As Decimal

    Public PrecessioneSpecie_Cod As Integer
    Public PrecessioneSpecie_Cod_AnnoPrecedente As Integer
    Public PrecessioneSpecie_Cod_DueAnniPrima As Integer
    Public PrecessioneSpecie_Cod_TreAnniPrima As Integer

    Public FertilizzanteOrganico_Precedente_Tipo As Integer

    Public Ubicazione_Cod As Integer

    Public CODAPIOVPRIM As Decimal

    Sub New()



    End Sub

    ''' <summary>
    ''' Crea l'oggetto di input già pronto per il calcolo del bilancio
    ''' </summary>
    ''' <param name="PC_TestataCod"></param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="FiltroAggiuntivo"></param>
    ''' <param name="OrderBy"></param>
    ''' <remarks></remarks>
    Sub New(ByVal PC_TestataCod As Integer, ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal FiltroAggiuntivo As String = "", Optional ByVal OrderBy As String = "")

        Dim Dt_Piano As DataTable
        Dim objPCD_DAL As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R

        Dt_Piano = objPCD_DAL.Leggi(PC_TestataCod,
                               "", "", objParametriServer)

        Me.Regolamento_Cod = Dt_Piano.Rows(0).Item("Regolamento_Cod")
        Me.Biologico = Dt_Piano.Rows(0).Item("PC_Dettagli_bio")

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Limo")) Then
            Me.Limo = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Limo"))
        Else
            Me.Limo = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Sabbia")) Then
            Me.Sabbia = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Sabbia"))
        Else
            Me.Sabbia = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Argilla")) Then
            Me.Argilla = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Argilla"))
        Else
            Me.Argilla = 0
        End If

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_ntot")) Then
            Me.NTot = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_ntot"))
        Else
            Me.NTot = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_norg")) Then
            Me.NOrg = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_norg"))
        Else
            Me.NOrg = 0
        End If

        'If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_p2o5")) Then
        '    Me.P2O5 = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_p2o5"))
        'Else
        '    Me.P2O5 = 0
        'End If
        'If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_k2o")) Then
        '    Me.K2O = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_k2o"))
        'Else
        '    Me.K2O = 0
        'End If

        Me.Ubicazione_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Ubicazione_Cod"))


        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita")) Then
            Me.PioggiaMM_Autunno = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita"))
        Else
            Me.PioggiaMM_Autunno = 0
        End If

        If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita_Primavera")) Then
            Me.PioggiaMM_Primavera = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita_Primavera"))
        Else
            Me.PioggiaMM_Primavera = 0
        End If

        Me.PrecessioneSpecie_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod"))
        Me.PrecessioneSpecie_Cod_AnnoPrecedente = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_AnnoPrecedente"))
        Me.PrecessioneSpecie_Cod_DueAnniPrima = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_2anni_prima"))
        Me.PrecessioneSpecie_Cod_TreAnniPrima = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod_3anni_prima"))

        Me.FertilizzanteOrganico_Precedente_Tipo = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer"))


    End Sub
End Class



#End Region

#Region "OUTPUT"
Public Class PianoNutrizionaleBilancio_output

    Public N_Ammesso As Decimal
    Public P_Ammesso As Decimal
    Public K_Ammesso As Decimal

    Public Consiglio_N As String
    Public Frazionamento_N As String
    Public Integrazione_N As String

    'Public Consiglio_P As String
    'Public Consiglio_K As Integer

    Public MessaggioErrore As String

    Public Sub New()
        N_Ammesso = 0
        P_Ammesso = 0
        K_Ammesso = 0

        Consiglio_N = ""
        Frazionamento_N = ""
        Integrazione_N = ""

        'Consiglio_P = ""
        'Consiglio_K = 0

        MessaggioErrore = ""
    End Sub

End Class

#End Region
