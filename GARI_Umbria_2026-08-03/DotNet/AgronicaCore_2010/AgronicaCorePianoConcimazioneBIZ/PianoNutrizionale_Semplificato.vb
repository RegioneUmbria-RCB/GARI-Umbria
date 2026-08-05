Public Class PianoNutrizionale_Semplificato
    Public Sub New()

    End Sub

    Public Function CalcolaPianoNutrizionaleSemplificato(ByVal SchedeInput As PianoNutrizionaleSemplificato_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                        As PianoNutrizionaleSemplificato_output

        Dim strErrore As String = ""
        Dim SchedeOutput As New PianoNutrizionaleSemplificato_output

        If SchedeInput.Resa = 0 Then
            strErrore &= "Parametro input Resa obbligatorio" & vbCrLf
        End If

        If strErrore <> "" Then
            SchedeOutput.MessaggioErrore = strErrore
            Return SchedeOutput
        End If


        '####################################
        '#########    FABBISOGNO    #########  ?????????? da aggiustare
        '####################################

        Dim FABBISOGNO As Integer = 0

        Select Case SchedeInput.Resa
            Case < 70
                FABBISOGNO = 100
            Case 70 To 79.99
                FABBISOGNO = 120
            Case >= 80
                FABBISOGNO = 140
        End Select

        '#################################
        '#########    CODAORG    #########
        '#################################

        'dim CODAORG As Decimal = pc_MatriciOrganicheXFrequenza.n (lettura della tabella pc_MatriciOrganicheXFrequenza passando id_mat_o = FertilizzanteOrganico_Precedente_Tipo e regolamento_cod)
        Dim CODAORG As Decimal = 0
        If SchedeInput.FertilizzanteOrganico_Precedente_Tipo > 0 Then
            Dim Dt_CODAORG As DataTable
            Dim obj_CODAORG As New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R
            Dt_CODAORG = obj_CODAORG.Leggi(SchedeInput.Regolamento_Cod,
                           SchedeInput.FertilizzanteOrganico_Precedente_Tipo, 0,
                           "", "", objParametri)

            If Dt_CODAORG.Rows.Count > 0 Then
                CODAORG = Dt_CODAORG.Rows(0).Item("N")
            End If
        End If



        '####################################
        '#########  CODPRECESSIONE  #########
        '####################################

        'dim CODPRECESSIONE As Decimal = SBAGLIATO? Non c'è un UNICA precessione? --> somme di pc_precessionecolturalexfrequenza.n  (lettura della tabella pc_precessionecolturalexfrequenza passando PrecessioneSpecie_Cod, PrecessioneSpecie_Cod_AnnoPrecedente, PrecessioneSpecie_Cod_DueAnniPrima, PrecessioneSpecie_Cod_TreAnniPrima e regolamento_cod)

        Dim CODPRECESSIONE As Integer = 0

        If SchedeInput.PrecessioneSpecie_Cod > 0 Then
            Dim obj_PRECESSIONE As New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturalexFrequenza_R
        Dim Dt_PRECESSIONE As DataTable
        Dt_PRECESSIONE = obj_PRECESSIONE.Leggi(SchedeInput.Regolamento_Cod,
                                   SchedeInput.PrecessioneSpecie_Cod, 1,
                                   "", "", objParametri)

            If Dt_PRECESSIONE.Rows.Count > 0 Then
                CODPRECESSIONE += Dt_PRECESSIONE.Rows(0).Item("N")
            End If
        End If



        '###################################
        '#########  CONSDAPIOGGIA  #########
        '###################################

        'dim CONSDAPIOGGIA As Decimal = select case pioggia_autunnale 
        '<100
        '   consdapioggia =0
        ' tra 100 e 200  
        '   consdapioggia = 20
        ' sopra 200  
        '   consdapioggia = 30

        Dim CONSDAPIOGGIA As Decimal
        Select Case SchedeInput.PioggiaMM_Autunno
            Case < 100
                CONSDAPIOGGIA = 0
            Case 100 To 200
                CONSDAPIOGGIA = 20
            Case > 200
                CONSDAPIOGGIA = 30
        End Select



        '#######################
        '#########  N  #########
        '#######################

        Dim N As Decimal = CODAORG + CODPRECESSIONE + CONSDAPIOGGIA + FABBISOGNO

        SchedeOutput.N_Ammesso = N

        Return SchedeOutput

    End Function

End Class
#Region "INPUT"
Public Class PianoNutrizionaleSemplificato_input

    Public Regolamento_Cod As Integer

    'Public Argilla As Decimal
    'Public Sabbia As Decimal
    'Public Limo As Decimal

    'Public NTot As Decimal
    'Public NOrg As Decimal
    'Public P2O5 As Decimal
    'Public K2O As Decimal

    Public Resa As Decimal

    Public PrecessioneSpecie_Cod As Integer
    Public FertilizzanteOrganico_Precedente_Tipo As Integer

    Public PioggiaMM_Autunno As Decimal
    'Public PioggiaMM_Primavera As Decimal

    Sub New()



    End Sub

End Class



#End Region

#Region "OUTPUT"
Public Class PianoNutrizionaleSemplificato_output

    Public N_Ammesso As Decimal
    Public P_Ammesso As Decimal
    Public K_Ammesso As Decimal

    Public MessaggioErrore As String

    Public Sub New()
        N_Ammesso = 0
        P_Ammesso = 0
        K_Ammesso = 0

        MessaggioErrore = ""
    End Sub

End Class

#End Region

