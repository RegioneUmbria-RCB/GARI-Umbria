Public Class PianoConcimazione_Suolo

    Public Function So_Dotazione(ByVal SuoloInput As PianoConcimazione_Suolo_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim objTessiture As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
        Dim IdGruppoTessitura As Integer = 0
        IdGruppoTessitura = objTessiture.Leggi_IdGruppoTessitura_Da_SabbiaArgilla(SuoloInput.Sabbia, SuoloInput.Argilla, objParametri)

        Dim objPC_GrigliaSO As New AgronicaCoreMetaSchemaDAL.PC_GrigliaSO_R
        Dim Id_Dotazione As Integer = 0

        Id_Dotazione = objPC_GrigliaSO.Leggi_Dotazione(SuoloInput.Regolamento_Cod,
                                                       SuoloInput.SO,
                                                      IdGruppoTessitura,
                                                      "", "",
                                                      objParametri)

        Return Id_Dotazione

    End Function

    Public Function CalcAtt_Dotazione(ByVal SuoloInput As PianoConcimazione_Suolo_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim objPC_GrigliaCalcAtt As New AgronicaCoreMetaSchemaDAL.PC_GrigliaCalcAtt_R
        Dim Id_Dotazione As Integer = 0

        Id_Dotazione = objPC_GrigliaCalcAtt.Leggi_Dotazione(SuoloInput.Regolamento_Cod,
                                                       SuoloInput.CalcAtt,
                                                      "", "",
                                                      objParametri)

        Return Id_Dotazione

    End Function

    Public Function P2O5_Dotazione(ByVal SuoloInput As PianoConcimazione_Suolo_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim objPC_GrigliaP2O5 As New AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
        Dim Id_Dotazione As Integer = 0

        Id_Dotazione = objPC_GrigliaP2O5.Leggi_Dotazione(SuoloInput.Regolamento_Cod,
                                                       SuoloInput.P2O5,
                                                      "", "",
                                                      objParametri)

        Return Id_Dotazione

    End Function

    Public Function K2O_Dotazione(ByVal SuoloInput As PianoConcimazione_Suolo_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim objTessiture As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
        Dim IdGruppoTessitura As Integer = 0
        IdGruppoTessitura = objTessiture.Leggi_IdGruppoTessitura_Da_SabbiaArgilla(SuoloInput.Sabbia, SuoloInput.Argilla, objParametri)

        Dim objPC_GrigliaK2O As New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
        Dim Id_Dotazione As Integer = 0

        If SuoloInput.Regolamento_Cod >= AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti.PianoConcimazione_2017 Then

            If (SuoloInput.Mg <> 0 And SuoloInput.K2O <> 0) Then

                '(03/04/2017 fede) introdotta valutazione Mg/K e K/CSC per modifica valutazione K2O
                'se Mg/K > 6 e K/CSC < 2 --> correzione di Potassio 
                'Correzione di Potassio = MIN(K2O editato e valore Minimo tabella PC_GrigliaK2O con dotazione Media (=3))
                'calcolo Mg/K ( = Mg meq / K meq = Mg ppm * 0,008230 / K2O ppm * 0,002558 * 0,83333)
                Dim RappMgsuK As Decimal = 0
                RappMgsuK = (SuoloInput.Mg * 0.00823) / (SuoloInput.K2O * 0.002558 * 0.83333)

                'calcolo K/CSC ( = K meq / CSC = K2O ppm * 0,002558 * 0,83333 / CSC)
                Dim RappKsuCSC As Decimal = 0
                If SuoloInput.CSC = 0 Then
                    RappKsuCSC = 100
                Else
                    RappKsuCSC = ((SuoloInput.K2O * 0.002558 * 0.83333) / SuoloInput.CSC) * 100
                End If

                Dim K2ORif As Decimal = 0
                If Not (RappMgsuK <= 6 And RappKsuCSC >= 2) Then
                    Dim objPC_GrigliaK As New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
                    Dim Dt As DataTable
                    Dt = objPC_GrigliaK.Leggi(SuoloInput.Regolamento_Cod,
                                                  0,
                                                  IdGruppoTessitura,
                                                  3,
                                                  "", "Massimo DESC",
                                                  objParametri)

                    Dim MinDotazioneNormale As Decimal = 0
                    If Dt.Rows.Count > 0 Then
                        MinDotazioneNormale = Dt.Rows(0).Item("Minimo")
                    End If
                    K2ORif = Math.Min(SuoloInput.K2O, MinDotazioneNormale)
                    SuoloInput.K2O = K2ORif
                End If

            End If

        End If

        Id_Dotazione = objPC_GrigliaK2O.Leggi_Dotazione(SuoloInput.Regolamento_Cod,
                                                       SuoloInput.K2O,
                                                      IdGruppoTessitura,
                                                      "", "",
                                                      objParametri)

        Return Id_Dotazione

    End Function

End Class

Public Class PianoConcimazione_Suolo_input

    Public Regolamento_Cod As Integer

    'Public Veg_Cod As Integer
    'Public Grfi_Cod As Integer
    'Public Resa As Decimal
    'Public FaseCicloColturale_Cod As Integer

    'Public AnticipazioniAnni As Integer

    Public Argilla As Decimal
    Public Sabbia As Decimal
    'Public NTot As Decimal
    Public SO As Decimal
    'Public CN As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public Mg As Decimal
    Public CSC As Decimal
    Public CalcAtt As Decimal 'calcare attivo

    'Public DisponibilitaOssigeno_Cod As Integer

    'Public PioggiaMM As Decimal

    'Public Precessione_Veg_Cod As Integer

    'Public FertilizzanteOrganico_ColturePrecedenti_Tipo As Integer
    'Public FertilizzanteOrganico_ColturePrecedenti_Frequenza As Integer
    'Public FertilizzanteOrganico_ColturePrecedenti_Qta As Decimal

    'Public ColturaProtetta As Boolean
    'Public Ubicazione_Cod As Integer
    'Public FissazioneN_Perc As Decimal

    Sub New()



    End Sub

End Class
