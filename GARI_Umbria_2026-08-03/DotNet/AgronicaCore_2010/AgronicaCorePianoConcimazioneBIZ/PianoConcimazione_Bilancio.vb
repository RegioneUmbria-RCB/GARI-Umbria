Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.ComponentModel

Public Class PianoConcimazione_Bilancio

    Public Sub New()

    End Sub

#Region "NECESSITA"

    Public Function Fabbisogno(ByVal Regolamento_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Grfi_Cod As Integer,
                           ByVal Elemento As String,
                           ByVal Resa As Decimal,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_Fabbisogni As AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R


        Dim dRet As Decimal = 0
        Dim QtaLiscivabile As Decimal = 0
        Dim PercPerdita As Decimal = 0

        Try

            objPC_Fabbisogni = New AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R

            Dim Dt As DataTable
            Dt = objPC_Fabbisogni.Leggi(Regolamento_Cod,
                                        Veg_Cod,
                                        Grfi_Cod, "", "",
                                        objParametri)

            objPC_Fabbisogni = Nothing

            If Dt.Rows.Count <> 1 Then
                Throw New Exception("Problema di dati nella tabella PC_Fabbisogni")
            End If

            If Not IsDBNull(Dt.Rows(0).Item(Elemento)) AndAlso IsNumeric(Dt.Rows(0).Item(Elemento)) Then

                dRet = Dt.Rows(0).Item(Elemento) * Resa * 10

            End If

            Dt.Dispose()
            Dt = Nothing

        Catch ex As Exception

            dRet = 0
        Finally

            objPC_Fabbisogni = Nothing

        End Try

        Return dRet

    End Function

    Public Function PerditeLisciviazione_N(ByVal Regolamento_Cod As Integer,
                                      ByVal IdFase As Integer,
                                      ByVal Piovosita As Decimal,
                                      ByVal AzotoTerreno As Decimal,
                                      ByVal IdGruppoTessitura As Integer,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

        Dim dRet As Decimal = 0
        Dim QtaLiscivabile As Decimal = 0
        Dim PercPerdita As Decimal = 0

        Try

            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

            Dim Dt As DataTable
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                               IdFase, 0, "", "",
                                               objParametri)

            objPC_FasiCicloColturale = Nothing

            If Dt.Rows.Count <> 1 Then
                Throw New Exception("Problema di dati nella tabella PC_FasiCicloColturale")
            End If

            ' Qta liscivabile
            If Not IsDBNull(Dt.Rows(0).Item("c_tempo")) AndAlso IsNumeric(Dt.Rows(0).Item("c_tempo")) Then
                If CDec(Dt.Rows(0).Item("c_tempo")) < 1 Then

                    Dt.Dispose()
                    Dt = Nothing

                    objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

                    Dim DtGr As DataTable
                    DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                    objPC_GruppiTessitura = Nothing

                    Dim Peso20 As Integer = 0

                    If DtGr.Rows.Count > 0 Then
                        Peso20 = DtGr.Rows(0).Item("Peso20")
                    End If

                    DtGr.Dispose()
                    DtGr = Nothing

                    If AzotoTerreno > 2.1 Then
                        AzotoTerreno = 2.1
                    End If

                    QtaLiscivabile = Peso20 * AzotoTerreno * 0.01

                Else
                    QtaLiscivabile = 30
                End If

            End If

            ' Percentuale perdita
            If Piovosita - 150 < 0 Then
                PercPerdita = 0
            ElseIf Piovosita - 150 > 100 Then
                PercPerdita = 100
            Else
                PercPerdita = Piovosita - 150
            End If

            dRet = QtaLiscivabile * PercPerdita / 100

        Catch ex As Exception

            dRet = 0

        End Try

        Return dRet


    End Function

    Public Function PerditeLisciviazione_N(ByVal Regolamento_Cod As Integer,
                                      ByVal IdFase As Integer,
                                      ByVal Piovosita As Decimal,
                                      ByVal PiovositaFeb As Decimal,
                                      ByVal AzotoTerreno As Decimal,
                                      ByVal IdGruppoTessitura As Integer,
                                          ByRef Lisciviazione_N_Inverno As Decimal,
                                          ByRef Lisciviazione_N_Febbraio As Decimal,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

        Dim dRet As Decimal = 0
        Dim QtaLiscivabile As Decimal = 0
        Dim PercPerdita As Decimal = 0

        Try

            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

            Dim Dt As DataTable
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                               IdFase, 0, "", "",
                                               objParametri)

            objPC_FasiCicloColturale = Nothing

            If Dt.Rows.Count <> 1 Then
                Throw New Exception("Problema di dati nella tabella PC_FasiCicloColturale")
            End If

            ' Qta liscivabile
            If Not IsDBNull(Dt.Rows(0).Item("c_tempo")) AndAlso IsNumeric(Dt.Rows(0).Item("c_tempo")) Then
                If CDec(Dt.Rows(0).Item("c_tempo")) < 1 Then

                    Dt.Dispose()
                    Dt = Nothing

                    objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

                    Dim DtGr As DataTable
                    DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                    objPC_GruppiTessitura = Nothing

                    Dim Peso20 As Integer = 0

                    If DtGr.Rows.Count > 0 Then
                        Peso20 = DtGr.Rows(0).Item("Peso20")
                    End If

                    DtGr.Dispose()
                    DtGr = Nothing

                    If AzotoTerreno > 2.1 Then
                        AzotoTerreno = 2.1
                    End If

                    QtaLiscivabile = Peso20 * AzotoTerreno * 0.01

                Else
                    QtaLiscivabile = 30
                End If

            End If

            ' Percentuale perdita
            If Piovosita - 150 < 0 Then
                PercPerdita = 0
            ElseIf Piovosita - 150 > 100 Then
                PercPerdita = 100
            Else
                PercPerdita = Piovosita - 150
            End If

            Lisciviazione_N_Inverno = QtaLiscivabile * PercPerdita / 100

            Lisciviazione_N_Febbraio = 0
            Dim PioggiaMancantePerditaCompleta As Decimal = 0
            Dim RimanenzaPioggiaFebbraio As Decimal = 0
            Dim Lisciviazione_N_Tardiva As Integer = 0
            If Piovosita > 150 Then
                PioggiaMancantePerditaCompleta = 250 - Piovosita
                RimanenzaPioggiaFebbraio = PiovositaFeb - PioggiaMancantePerditaCompleta
                Lisciviazione_N_Febbraio = RimanenzaPioggiaFebbraio / 10

                If RimanenzaPioggiaFebbraio > 0 Then
                    Lisciviazione_N_Tardiva = QtaLiscivabile - Lisciviazione_N_Inverno
                End If
            End If


            'dRet = QtaLiscivabile * PercPerdita / 100

            dRet = Lisciviazione_N_Inverno + Lisciviazione_N_Febbraio + Lisciviazione_N_Tardiva

        Catch ex As Exception

            Lisciviazione_N_Inverno = 0
            Lisciviazione_N_Febbraio = 0

            dRet = 0

        End Try

        Return dRet


    End Function

    Public Function PerditeLiscivazione_K(ByVal Regolamento_Cod As Integer,
                                          ByVal Argilla As Decimal,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_Liscivazione As AgronicaCoreMetaSchemaDAL.PC_Liscivazione_R

        Dim QtaLiscivabile As Decimal = 0
        Dim PercPerdita As Decimal = 0

        Try
            objPC_Liscivazione = New AgronicaCoreMetaSchemaDAL.PC_Liscivazione_R

            PercPerdita = objPC_Liscivazione.Leggi_Perdita(Regolamento_Cod, Argilla, objParametri)

        Catch ex As Exception

            PercPerdita = 0

        End Try

        objPC_Liscivazione = Nothing

        Return PercPerdita


    End Function

    Public Function Immobilizzazioni_N(ByVal Regolamento_Cod As Integer,
                                   ByVal Pre_Cod As Integer,
                                   ByVal IdFase As Integer,
                                   ByVal IdGruppoTessitura As Integer,
                                   ByVal IdDisp As Integer,
                                   ByVal SO As Decimal,
                                   ByVal CN As Decimal,
                                   ByVal N As Decimal,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_Precessione As AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R
        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim objPC_RapportoCN As AgronicaCoreMetaSchemaDAL.PC_RapportoCN_R
        Dim objPC_CoefficenteMinimoSoN As AgronicaCoreMetaSchemaDAL.PC_CoefficenteMinimoSoN_R
        Dim objPC_Perdite As AgronicaCoreMetaSchemaDAL.PC_PerditeImmobilizzazioniDispersioni_R

        Dim NResiduo As Decimal = 0
        Dim ConteggiaPrec As Boolean = False
        Dim c_tempo As Decimal
        Dim Peso20 As Decimal

        Dim Nm_utile As Decimal
        Dim Nm As Decimal
        Dim Np As Decimal
        Dim CoeffMinCN As Decimal
        Dim dblRet As Decimal

        Try

            Dim Dt As DataTable
            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                IdFase,
                                                0, "", "",
                                                objParametri)

            If Dt.Rows.Count = 1 Then
                If Dt.Rows(0).Item("ConteggiaPrecessione") = 1 Then
                    ConteggiaPrec = True
                End If
                c_tempo = Dt.Rows(0).Item("c_tempo")
            End If
            Dt = Nothing

            If ConteggiaPrec Then
                objPC_Precessione = New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R
                Dt = objPC_Precessione.Leggi(Regolamento_Cod, 0, Pre_Cod, "", "", objParametri)

                If Dt.Rows.Count = 1 Then
                    NResiduo = Dt.Rows(0).Item("N_Residuo")
                End If
                Dt = Nothing
            End If


            objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

            Dim DtGr As DataTable
            DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

            objPC_GruppiTessitura = Nothing

            If DtGr.Rows.Count > 0 Then
                Peso20 = DtGr.Rows(0).Item("Peso20")
            End If

            DtGr.Dispose()
            DtGr = Nothing

            objPC_RapportoCN = New AgronicaCoreMetaSchemaDAL.PC_RapportoCN_R
            objPC_CoefficenteMinimoSoN = New AgronicaCoreMetaSchemaDAL.PC_CoefficenteMinimoSoN_R

            Dim RappCN As Integer = objPC_RapportoCN.Leggi_Rapporto(Regolamento_Cod, CN, "", objParametri)
            CoeffMinCN = objPC_CoefficenteMinimoSoN.Leggi_CoeffMinimo(Regolamento_Cod, RappCN, IdGruppoTessitura, "", objParametri)

            'Nm=((((J19*G19/100)*1000)*M15/100)*0,05)

            Nm = ((Peso20 * Math.Min(SO, 3) * 10) * CoeffMinCN / 100) * 0.05

            Nm_utile = Nm * c_tempo

            'Np=((J19*H19/1000)*1000)*0,01

            Np = (Peso20 * Math.Min(CDec("2,1"), N)) * 0.01


            Dim B As Decimal = Nm_utile
            If c_tempo < 1 Then
                B += Np
            End If

            ' disp ossigeno da interfaccia
            objPC_Perdite = New AgronicaCoreMetaSchemaDAL.PC_PerditeImmobilizzazioniDispersioni_R
            Dim DtPerdite As DataTable

            DtPerdite = objPC_Perdite.Leggi(Regolamento_Cod,
                                            IdDisp,
                                            IdGruppoTessitura, "", "", objParametri)

            Dim fc_D As Decimal = 0
            If DtPerdite.Rows.Count = 1 Then
                fc_D = DtPerdite.Rows(0).Item("fc_D")
            End If

            Dim D As Decimal = B * fc_D
            Dim E As Decimal = IIf(NResiduo < 0, NResiduo, 0)

            dblRet = D + Math.Abs(E)


        Catch ex As Exception
            dblRet = 0
        Finally
            objPC_Precessione = Nothing
            objPC_FasiCicloColturale = Nothing
            objPC_GruppiTessitura = Nothing
            objPC_RapportoCN = Nothing
            objPC_CoefficenteMinimoSoN = Nothing
            objPC_Perdite = Nothing

        End Try

        Return dblRet


    End Function

    Public Function Arricchimenti_P(ByVal Regolamento_Cod As Integer,
                                    ByVal P2O5 As Decimal,
                                    ByVal CalcTot As Decimal,
                                    ByVal IdGruppoTessitura As Int32,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_GrigliaP As AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim Dt As DataTable


        Dim dblRet As Decimal = 0

        Try

            objPC_GrigliaP = New AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R

            Dt = objPC_GrigliaP.Leggi(Regolamento_Cod,
                                      P2O5,
                                      0,
                                      "", "",
                                      objParametri)

            Dim Id_Dotazione As Integer = 0
            If Dt.Rows.Count = 1 Then
                Id_Dotazione = Dt.Rows(0).Item("Id_Dotazione")
            End If
            Dt = Nothing


            If Id_Dotazione > 2 Then
                ' dotazione normale o più
                dblRet = 0
            Else
                ' dotazione scarsa, molto scarsa

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                Dim Immobilizzazione As Decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                    Immobilizzazione = DtGr.Rows(0).Item("ImmobilizzazioneP")
                End If

                ' minimo di griP della dotazione normale
                Dt = objPC_GrigliaP.Leggi(Regolamento_Cod,
                                          0,
                                          3,
                                          "", "Minimo",
                                          objParametri)

                Dim MinDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MinDotazioneNormale = Dt.Rows(0).Item("Minimo")
                End If

                Dim Deficit As Decimal
                Deficit = (P2O5 - MinDotazioneNormale) * Peso30 / 1000

                ' Fattore immobilizzazione C (L10)
                Dim FattImm As Decimal = 0
                FattImm = Immobilizzazione + (0.02 * CalcTot)

                dblRet = -Deficit * FattImm

            End If

        Catch ex As Exception
            dblRet = 0
        Finally
            objPC_GrigliaP = Nothing
            objPC_GruppiTessitura = Nothing
        End Try


        Return dblRet


    End Function

    Public Function Arricchimenti_K(ByVal Regolamento_Cod As Integer,
                                    ByVal K2O As Decimal,
                                    ByVal Argilla As Decimal,
                                    ByVal IdGruppoTessitura As Int32,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_GrigliaK As AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim Dt As DataTable


        Dim dblRet As Decimal = 0

        Try

            objPC_GrigliaK = New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R

            Dt = objPC_GrigliaK.Leggi(Regolamento_Cod,
                                      K2O,
                                      IdGruppoTessitura,
                                      0,
                                      "", "",
                                      objParametri)

            Dim Id_Dotazione As Integer = 0
            If Dt.Rows.Count = 1 Then
                Id_Dotazione = Dt.Rows(0).Item("Id_Dotazione")
            End If
            Dt = Nothing


            If Id_Dotazione > 2 Then
                ' dotazione normale o più
                dblRet = 0
            Else
                ' dotazione scarsa, molto scarsa

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                Dim Immobilizzazione As Decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                    Immobilizzazione = DtGr.Rows(0).Item("ImmobilizzazioneP")
                End If

                ' minimo massimo di griP della dotazione normale
                Dt = objPC_GrigliaK.Leggi(Regolamento_Cod,
                                          0,
                                          IdGruppoTessitura,
                                          3,
                                          "", "Minimo",
                                          objParametri)

                Dim MinDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MinDotazioneNormale = Dt.Rows(0).Item("Minimo")
                End If

                Dim Deficit As Decimal
                Deficit = (K2O - MinDotazioneNormale) * Peso30 / 1000

                ' Fattore immobilizzazione G (K19)
                Dim FattImm As Decimal = 0
                FattImm = 1 + (0.018 * Argilla)

                dblRet = -Deficit * FattImm

            End If

        Catch ex As Exception
            dblRet = 0
        Finally
            objPC_GrigliaK = Nothing
            objPC_GruppiTessitura = Nothing
        End Try


        Return dblRet


    End Function

    Public Function Anticipazioni_P(ByVal Regolamento_Cod As Integer,
                                    ByVal IdFase As Decimal,
                                    ByVal Anticipazioni_Anni As Integer,
                                    ByVal P2O5 As Decimal,
                                    ByVal IdGruppoTessitura As Int32,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Grfi_Cod As Integer,
                                    ByVal Resa As Decimal,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim Dt As DataTable
        Dim objPC_Fasi As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GrigliaP As AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim objPC_Fabbisogni As AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R


        Dim dblRet As Decimal = 0
        Dim AnticipazioniCiclo As Integer

        Try

            objPC_Fasi = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

            Dt = objPC_Fasi.Leggi(Regolamento_Cod,
                               IdFase,
                               0,
                               "", "",
                               objParametri)

            If Dt.Rows.Count = 1 Then
                If Dt.Rows(0).Item("Anticipazioni") = 1 Then
                    AnticipazioniCiclo = 1
                End If
            End If

            If Anticipazioni_Anni = 0 Or AnticipazioniCiclo = 0 Then
                dblRet = 0
            Else

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                Dim Immobilizzazione As Decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                    Immobilizzazione = DtGr.Rows(0).Item("ImmobilizzazioneP")
                End If

                objPC_GrigliaP = New AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R

                ' massimo di griP della dotazione normale
                Dt = objPC_GrigliaP.Leggi(Regolamento_Cod,
                                          0,
                                          3,
                                          "", "Massimo DESC",
                                          objParametri)

                Dim MaxDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MaxDotazioneNormale = Dt.Rows(0).Item("Massimo")
                End If

                Dim Deficit As Decimal '(G23)
                Deficit = (P2O5 - MaxDotazioneNormale) * Peso30 / 1000


                Dim FabbP As Decimal
                objPC_Fabbisogni = New AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R
                Dt = objPC_Fabbisogni.Leggi(Regolamento_Cod, Veg_Cod, Grfi_Cod, "", "", objParametri)
                If Dt.Rows.Count = 1 Then
                    FabbP = Dt.Rows(0).Item("P2O5")
                End If

                Dim AsportazioniFuture As Decimal = FabbP * Resa * 10 * Anticipazioni_Anni    'H22

                If Deficit > 0 Then
                    dblRet = AsportazioniFuture - Deficit
                Else
                    dblRet = AsportazioniFuture
                End If

            End If


        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_Fasi = Nothing
            objPC_GrigliaP = Nothing
            objPC_GruppiTessitura = Nothing
            objPC_Fabbisogni = Nothing

        End Try


        Return dblRet


    End Function

    Public Function Anticipazioni_K(ByVal Regolamento_Cod As Integer,
                                    ByVal IdFase As Decimal,
                                    ByVal Anticipazioni_Anni As Integer,
                                    ByVal K2O As Decimal,
                                    ByVal IdGruppoTessitura As Int32,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Grfi_Cod As Integer,
                                    ByVal Resa As Decimal,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim Dt As DataTable
        Dim objPC_Fasi As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GrigliaK As AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim objPC_Fabbisogni As AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R


        Dim dblRet As Decimal = 0
        Dim AnticipazioniCiclo As Integer

        Try

            objPC_Fasi = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

            Dt = objPC_Fasi.Leggi(Regolamento_Cod,
                               IdFase,
                               0,
                               "", "",
                               objParametri)

            If Dt.Rows.Count = 1 Then
                If Dt.Rows(0).Item("Anticipazioni") = 1 Then
                    AnticipazioniCiclo = 1
                End If
            End If

            If Anticipazioni_Anni = 0 Or AnticipazioniCiclo = 0 Then
                dblRet = 0
            Else

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                Dim Immobilizzazione As Decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                    Immobilizzazione = DtGr.Rows(0).Item("ImmobilizzazioneP")
                End If

                objPC_GrigliaK = New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R

                ' massimo di griK della dotazione normale
                Dt = objPC_GrigliaK.Leggi(Regolamento_Cod,
                                          0,
                                          IdGruppoTessitura,
                                          3,
                                          "", "Massimo DESC",
                                          objParametri)

                Dim MaxDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MaxDotazioneNormale = Dt.Rows(0).Item("Massimo")
                End If

                Dim Deficit As Decimal '(F24)
                Deficit = (K2O - MaxDotazioneNormale) * Peso30 / 1000


                Dim FabbP As Decimal
                objPC_Fabbisogni = New AgronicaCoreMetaSchemaDAL.PC_Fabbisogni_R
                Dt = objPC_Fabbisogni.Leggi(Regolamento_Cod, Veg_Cod, Grfi_Cod, "", "", objParametri)
                If Dt.Rows.Count = 1 Then
                    FabbP = Dt.Rows(0).Item("K2O")
                End If

                Dim AsportazioniFuture As Decimal = FabbP * Resa * 10 * Anticipazioni_Anni    'G23

                If Deficit > 0 Then
                    dblRet = AsportazioniFuture - Deficit
                Else
                    dblRet = AsportazioniFuture
                End If

            End If


        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_Fasi = Nothing
            objPC_GrigliaK = Nothing
            objPC_GruppiTessitura = Nothing
            objPC_Fabbisogni = Nothing

        End Try


        Return dblRet


    End Function

#End Region

#Region "DISPONIBILITA"

    Public Function FertilitaSuolo_N(ByVal Regolamento_Cod As Integer,
                                       ByVal IdFase As Integer,
                                       ByVal IdGruppoTessitura As Integer,
                                       ByVal SO As Decimal,
                                       ByVal CN As Decimal,
                                       ByVal N As Decimal,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
        Dim objPC_RapportoCN As AgronicaCoreMetaSchemaDAL.PC_RapportoCN_R
        Dim objPC_CoefficenteMinimoSoN As AgronicaCoreMetaSchemaDAL.PC_CoefficenteMinimoSoN_R

        Dim NResiduo As Decimal = 0
        Dim ConteggiaPrec As Boolean = False
        Dim c_tempo As Decimal
        Dim Peso20 As Decimal

        Dim Nm_utile As Decimal
        Dim Nm As Decimal
        Dim Np As Decimal
        Dim CoeffMinCN As Decimal
        Dim dblRet As Decimal

        Try

            Dim Dt As DataTable
            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                IdFase,
                                                0, "", "",
                                                objParametri)

            If Dt.Rows.Count = 1 Then
                c_tempo = Dt.Rows(0).Item("c_tempo")
            End If
            Dt = Nothing


            objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

            Dim DtGr As DataTable
            DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

            objPC_GruppiTessitura = Nothing

            If DtGr.Rows.Count > 0 Then
                Peso20 = DtGr.Rows(0).Item("Peso20")
            End If

            DtGr.Dispose()
            DtGr = Nothing

            objPC_RapportoCN = New AgronicaCoreMetaSchemaDAL.PC_RapportoCN_R
            objPC_CoefficenteMinimoSoN = New AgronicaCoreMetaSchemaDAL.PC_CoefficenteMinimoSoN_R

            Dim RappCN As Integer = objPC_RapportoCN.Leggi_Rapporto(Regolamento_Cod, CN, "", objParametri)
            CoeffMinCN = objPC_CoefficenteMinimoSoN.Leggi_CoeffMinimo(Regolamento_Cod, RappCN, IdGruppoTessitura, "", objParametri)

            'Nm=((((J19*G19/100)*1000)*M15/100)*0,05)

            Nm = ((Peso20 * Math.Min(SO, 3) * 10) * CoeffMinCN / 100) * (5 / 100)

            Nm_utile = Nm * c_tempo

            'Np=((J19*H19/1000)*1000)*0,01

            Np = (Peso20 * Math.Min(CDec("2,1"), N)) * 0.01


            If c_tempo < 1 Then
                dblRet = Np + Nm_utile
            Else
                dblRet = Nm_utile
            End If


        Catch ex As Exception
            dblRet = 0
        Finally
            '  objPC_Precessione = Nothing
            objPC_FasiCicloColturale = Nothing
            objPC_GruppiTessitura = Nothing
            objPC_RapportoCN = Nothing
            objPC_CoefficenteMinimoSoN = Nothing
            '   objPC_Perdite = Nothing

        End Try

        Return dblRet


    End Function

    Public Function FertilitaSuolo_P(ByVal Regolamento_Cod As Integer,
                                     ByVal P2O5 As Decimal,
                                     ByVal IdGruppoTessitura As Int32,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim Dt As DataTable
        Dim objPC_GrigliaP As AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

        Dim dblRet As Decimal = 0

        Try

            objPC_GrigliaP = New AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
            ' massimo di griP della dotazione normale
            Dt = objPC_GrigliaP.Leggi(Regolamento_Cod,
                                      P2O5,
                                      0,
                                      "", "",
                                      objParametri)

            Dim IdDotazione As Decimal = 0
            If Dt.Rows.Count > 0 Then
                IdDotazione = Dt.Rows(0).Item("Id_Dotazione")
            End If

            '=SE(O(G10="normale";G10="scarso";G10="molto scarso");0;G23)
            '(08/03/2018 fede)
            If IdDotazione > 4 Then
                'If IdDotazione > 3 Then

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                ' Dim Immobilizzazione As decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                End If

                ' massimo di griP della dotazione normale 
                '(23/05/2018 fede)(fissata max dotazione normale = elevata = 4)
                Dt = objPC_GrigliaP.Leggi(Regolamento_Cod,
                                          0,
                                            4,
                                            "", "",
                                            objParametri)

                Dim MaxDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MaxDotazioneNormale = Dt.Rows(0).Item("Massimo")
                End If

                Dim Deficit As Decimal '(G23)
                Deficit = (P2O5 - MaxDotazioneNormale) * Peso30 / 1000

                dblRet = Deficit
            Else

                dblRet = 0
            End If


        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_GrigliaP = Nothing
            objPC_GruppiTessitura = Nothing

        End Try


        Return dblRet


    End Function

    Public Function FertilitaSuolo_K(ByVal Regolamento_Cod As Integer,
                                     ByVal K2O As Decimal,
                                     ByVal IdGruppoTessitura As Int32,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim Dt As DataTable
        Dim objPC_GrigliaK As AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
        Dim objPC_GruppiTessitura As AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R

        Dim dblRet As Decimal = 0

        Try

            objPC_GrigliaK = New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R

            ' massimo di griP della dotazione normale
            Dt = objPC_GrigliaK.Leggi(Regolamento_Cod,
                                      K2O,
                                      IdGruppoTessitura,
                                      0,
                                      "", "",
                                      objParametri)

            Dim IdDotazione As Decimal = 0
            If Dt.Rows.Count > 0 Then
                IdDotazione = Dt.Rows(0).Item("Id_Dotazione")
            End If

            If IdDotazione > 3 Then

                objPC_GruppiTessitura = New AgronicaCoreMetaSchemaDAL.PC_GruppiTessitura_R
                Dim DtGr As DataTable
                DtGr = objPC_GruppiTessitura.Leggi(IdGruppoTessitura, "", "", objParametri)

                objPC_GruppiTessitura = Nothing

                Dim Peso30 As Decimal
                If DtGr.Rows.Count > 0 Then
                    Peso30 = DtGr.Rows(0).Item("Peso30")
                End If

                objPC_GrigliaK = New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R

                ' massimo di griK della dotazione normale
                Dt = objPC_GrigliaK.Leggi(Regolamento_Cod,
                                          0,
                                          IdGruppoTessitura,
                                          3,
                                          "", "Massimo DESC",
                                          objParametri)

                Dim MaxDotazioneNormale As Decimal = 0
                If Dt.Rows.Count > 0 Then
                    MaxDotazioneNormale = Dt.Rows(0).Item("Massimo")
                End If

                Dim Deficit As Decimal '(F24)
                Deficit = (K2O - MaxDotazioneNormale) * Peso30 / 1000

                dblRet = Deficit

            Else

                dblRet = 0

            End If

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_GrigliaK = Nothing
            objPC_GruppiTessitura = Nothing

        End Try


        Return dblRet


    End Function

    Public Function Precessione_N(ByVal Regolamento_Cod As Integer,
                                  ByVal IdFase As Integer,
                                  ByVal Pre_Cod As Integer,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_Precessione As AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R
        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

        Dim NResiduo As Decimal = 0
        Dim ConteggiaPrec As Boolean = False

        Dim dblRet As Decimal

        Try

            Dim Dt As DataTable
            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                IdFase,
                                                0, "", "",
                                                objParametri)

            If Dt.Rows.Count = 1 Then
                If Dt.Rows(0).Item("ConteggiaPrecessione") = 1 Then
                    ConteggiaPrec = True
                End If
            End If
            Dt = Nothing

            If ConteggiaPrec Then
                objPC_Precessione = New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R
                Dt = objPC_Precessione.Leggi(Regolamento_Cod, 0, Pre_Cod, "", "", objParametri)

                If Dt.Rows.Count = 1 Then
                    NResiduo = Dt.Rows(0).Item("N_Residuo")
                End If
                Dt = Nothing
            End If

            If NResiduo > 0 Then
                dblRet = NResiduo
            Else
                dblRet = 0
            End If

        Catch ex As Exception
            dblRet = 0
        Finally
            objPC_Precessione = Nothing
            objPC_FasiCicloColturale = Nothing

        End Try

        Return dblRet


    End Function

    Public Function FertilitaResidua_N(ByVal Regolamento_Cod As Integer,
                                       ByVal IdFase As Integer,
                                       ByVal Id_Mat_O As Integer,
                                       ByVal Id_Fre As Integer,
                                       ByVal Qta As Decimal,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_MatricixFrequenza As AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R

        Dim NResiduo As Decimal = 0
        Dim C_Tempo As Decimal = 0

        Dim dblRet As Decimal

        Try

            Dim Dt As DataTable
            objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
            Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                IdFase,
                                                0, "", "",
                                                objParametri)

            If Dt.Rows.Count = 1 Then
                C_Tempo = Dt.Rows(0).Item("c_tempo")
            End If
            Dt = Nothing

            objPC_MatricixFrequenza = New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R
            Dt = objPC_MatricixFrequenza.Leggi(Regolamento_Cod,
                                               Id_Mat_O,
                                               Id_Fre,
                                               "", "",
                                               objParametri)


            If Dt.Rows.Count = 1 Then
                NResiduo = Dt.Rows(0).Item("N")
            End If
            Dt = Nothing

            dblRet = NResiduo * Qta * C_Tempo

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_FasiCicloColturale = Nothing
            objPC_MatricixFrequenza = Nothing

        End Try

        Return dblRet

    End Function

    Public Function ApportiNaturali_N(ByVal Regolamento_Cod As Integer,
                                       ByVal IdFase As Integer,
                                       ByVal Protetta As Boolean,
                                       ByVal Ubicazione_Cod As Integer,
                                       ByVal Perc_Fissazione_Precessioni As Decimal,
                                       ByVal BilancioN As Decimal,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
        Dim objPC_Ubicazione As AgronicaCoreMetaSchemaDAL.PC_Ubicazione_R

        Dim NResiduo As Decimal = 0
        Dim C_Tempo As Decimal = 0
        Dim DepAnno As Decimal = 0

        Dim DepAtmosferiche As Decimal = 0
        Dim AzotoFissazione As Decimal = 0

        Dim dblRet As Decimal

        Try

            Dim Dt As DataTable

            If Not Protetta Then

                objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
                Dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                    IdFase,
                                                    0, "", "",
                                                    objParametri)

                If Dt.Rows.Count = 1 Then
                    C_Tempo = Dt.Rows(0).Item("c_tempo")
                End If
                Dt = Nothing

                objPC_Ubicazione = New AgronicaCoreMetaSchemaDAL.PC_Ubicazione_R
                Dt = objPC_Ubicazione.Leggi(Regolamento_Cod, Ubicazione_Cod, "", "", objParametri)
                If Dt.Rows.Count = 1 Then
                    DepAnno = Dt.Rows(0).Item("Deposizione_Anno")
                End If
                Dt = Nothing

                DepAtmosferiche = DepAnno * C_Tempo

            End If

            AzotoFissazione = (BilancioN - DepAtmosferiche) * Perc_Fissazione_Precessioni / 100

            dblRet = DepAtmosferiche + AzotoFissazione

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_FasiCicloColturale = Nothing
            objPC_Ubicazione = Nothing

        End Try

        Return dblRet


    End Function

#End Region

#Region "APPORTI"

    Public Function ApportoAmmesso_N(ByVal Regolamento_Cod As Integer,
                                     ByVal IdFase As Integer,
                                     ByVal Veg_Cod As Integer,
                                     ByVal Grfi_Cod As Integer,
                                     ByVal TotN As Decimal,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim objFattori As AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
        Dim objPC_FasiCicloColturale As AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R

        Dim NResiduo As Decimal = 0
        Dim C_Tempo As Decimal = 0
        Dim DepAnno As Decimal = 0

        Dim DepAtmosferiche As Decimal = 0
        Dim AzotoFissazione As Decimal = 0

        Dim dblRet As Decimal = 0

        Try

            Dim dt As DataTable

            If IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto Then
                dblRet = 0

            ElseIf IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento,
                                      "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing

            ElseIf IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento, "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing
            Else

                objPC_FasiCicloColturale = New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturale_R
                dt = objPC_FasiCicloColturale.Leggi(Regolamento_Cod,
                                                    IdFase,
                                                    0, "", "",
                                                    objParametri)

                If dt.Rows.Count >= 1 Then
                    Dim N As Decimal
                    N = dt.Rows(0).Item("N")
                    dblRet = N * TotN
                End If
                dt = Nothing

            End If

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_FasiCicloColturale = Nothing
            objFattori = Nothing

        End Try

        Return dblRet


    End Function

    Public Function ApportoAmmesso_P(ByVal Regolamento_Cod As Integer,
                                     ByVal IdFase As Integer,
                                     ByVal Veg_Cod As Integer,
                                     ByVal Grfi_Cod As Integer,
                                     ByVal P2O5 As Decimal,
                                     ByVal TotP As Decimal,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim objFattori As AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
        Dim objPC_GrigliaP As AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R

        Dim dblRet As Decimal = 0

        Try

            Dim DtGriglia As DataTable
            objPC_GrigliaP = New AgronicaCoreMetaSchemaDAL.PC_GrigliaP2O5_R
            DtGriglia = objPC_GrigliaP.Leggi(Regolamento_Cod, P2O5, 0, "", "", objParametri)
            Dim IdGriglia As Integer
            If DtGriglia.Rows.Count > 0 Then
                IdGriglia = DtGriglia.Rows(0).Item("Id_Griglia")
            End If

            Dim dt As DataTable

            If IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento And IdGriglia > 2 Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento, "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing

            ElseIf IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento And IdGriglia > 2 Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento, "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing
            Else
                dblRet = TotP

            End If

            If dblRet < 0 Then
                dblRet = 0
            ElseIf dblRet > 250 Then
                dblRet = 250
            End If

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_GrigliaP = Nothing
            objFattori = Nothing

        End Try

        Return dblRet


    End Function

    Public Function ApportoAmmesso_K(ByVal Regolamento_Cod As Integer,
                                     ByVal IdFase As Integer,
                                     ByVal Veg_Cod As Integer,
                                     ByVal Grfi_Cod As Integer,
                                     ByVal K2O As Decimal,
                                     ByVal IdGruppoTessitura As Int32,
                                     ByVal TotK As Decimal,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal


        Dim objFattori As AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
        Dim objPC_GrigliaK As AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R

        Dim dblRet As Decimal = 0

        Try

            Dim DtGriglia As DataTable
            objPC_GrigliaK = New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
            DtGriglia = objPC_GrigliaK.Leggi(Regolamento_Cod, K2O, IdGruppoTessitura, 0, "", "", objParametri)
            Dim IdGriglia As Integer
            If DtGriglia.Rows.Count > 0 Then
                IdGriglia = DtGriglia.Rows(0).Item("Id_Griglia")
            End If

            Dim dt As DataTable
            If IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento And IdGriglia > 2 Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento, "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing

            ElseIf IdFase = enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento And IdGriglia > 2 Then

                objFattori = New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
                dt = objFattori.Leggi(Regolamento_Cod,
                                      enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento, "", "",
                                      Veg_Cod,
                                      Grfi_Cod, "", "",
                                      objParametri)

                If dt.Rows.Count >= 1 Then
                    dblRet = dt.Rows(0).Item("Valore")
                End If
                dt = Nothing
            Else
                dblRet = TotK

            End If

            If dblRet < 0 Then
                dblRet = 0
            ElseIf dblRet > 300 Then
                dblRet = 300
            End If

        Catch ex As Exception
            dblRet = 0
        Finally

            objPC_GrigliaK = Nothing
            objFattori = Nothing

        End Try

        Return dblRet


    End Function

#End Region

#Region "BILANCIO"

    Public Function CalcolaBilancio(ByVal BilancioInput As PianoConcimazioneBilancio_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As PianoConcimazioneBilancio_output

        Dim strErrore As String = ""

        Dim BilancioOutput As New PianoConcimazioneBilancio_output
        Dim Necessita As Necessita
        Dim Disponibilita As Disponibilita

        If BilancioInput.Regolamento_Cod = 0 Then
            strErrore &= "Parametro input Regolamento obbligatorio" & vbCrLf
        End If

        If BilancioInput.Argilla = 0 Then
            strErrore &= "Parametro input Argilla obbligatorio" & vbCrLf
        End If

        If BilancioInput.Sabbia = 0 Then
            strErrore &= "Parametro input Sabbia obbligatorio" & vbCrLf
        End If

        If BilancioInput.Veg_Cod = 0 Then
            strErrore &= "Parametro input Specie Vegetale obbligatorio" & vbCrLf
        End If

        If strErrore <> "" Then
            BilancioOutput.MessaggioErrore = strErrore
            Return BilancioOutput
        End If

        Dim N, P, K As Decimal
        Dim TotN_Necessita, TotP_Necessita, TotK_Necessita As Decimal
        Dim TotN_Dispo, TotP_Dispo, TotK_Dispo As Decimal
        Dim AmmessoN, AmmessoP, AmmessoK As Decimal


        Dim objTessiture As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
        Dim IdGruppoTessitura As Integer = 0
        IdGruppoTessitura = objTessiture.Leggi_IdGruppoTessitura_Da_SabbiaArgilla(BilancioInput.Sabbia, BilancioInput.Argilla, objParametri)

        'PIANO CONCIMAZIONE 2017
        If BilancioInput.Regolamento_Cod >= enum_PUARegolamenti.PianoConcimazione_2017 Then

            If (BilancioInput.Mg <> 0 And BilancioInput.K2O <> 0) Then

                '(03/04/2017 fede) introdotta valutazione Mg/K e K/CSC per modifica valutazione K2O
                'se Mg/K > 6 e K/CSC < 2 --> correzione di Potassio 
                'Correzione di Potassio = MIN(K2O editato e valore Minimo tabella PC_GrigliaK2O con dotazione Media (=3))
                'calcolo Mg/K ( = Mg meq / K meq = Mg ppm * 0,008230 / K2O ppm * 0,002558 * 0,83333)
                Dim RappMgsuK As Decimal = 0
                RappMgsuK = (BilancioInput.Mg * 0.00823) / (BilancioInput.K2O * 0.002558 * 0.83333)

                'calcolo K/CSC ( = K meq / CSC = K2O ppm * 0,002558 * 0,83333 / CSC)
                Dim RappKsuCSC As Decimal = 0
                If BilancioInput.CSC = 0 Then
                    RappKsuCSC = 100
                Else
                    RappKsuCSC = ((BilancioInput.K2O * 0.002558 * 0.83333) / BilancioInput.CSC) * 100
                End If

                Dim K2ORif As Decimal = 0
                If Not (RappMgsuK <= 6 And RappKsuCSC >= 2) Then
                    Dim objPC_GrigliaK As New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
                    Dim Dt As DataTable
                    Dt = objPC_GrigliaK.Leggi(BilancioInput.Regolamento_Cod,
                                                  0,
                                                  IdGruppoTessitura,
                                                  3,
                                                  "", "Massimo DESC",
                                                  objParametri)

                    Dim MinDotazioneNormale As Decimal = 0
                    If Dt.Rows.Count > 0 Then
                        MinDotazioneNormale = Dt.Rows(0).Item("Minimo")
                    End If
                    K2ORif = Math.Min(BilancioInput.K2O, MinDotazioneNormale)
                    BilancioInput.K2O = K2ORif
                End If

            End If

        End If



        '##############################################################################################
        '##############################################################################################
        '#####  NECESSITA'      #######################################################################
        '##############################################################################################
        '##############################################################################################

        TotN_Necessita = 0
        TotP_Necessita = 0
        TotK_Necessita = 0

        ' ---- FABBISOGNO DELLA COLTURA        

        N = 0
        P = 0
        K = 0

        N = Fabbisogno(BilancioInput.Regolamento_Cod,
                       BilancioInput.Veg_Cod,
                       BilancioInput.Grfi_Cod,
                       "N",
                       BilancioInput.Resa,
                       objParametri)

        P = Fabbisogno(BilancioInput.Regolamento_Cod,
                       BilancioInput.Veg_Cod,
                       BilancioInput.Grfi_Cod,
                       "P2O5",
                       BilancioInput.Resa,
                       objParametri)

        K = Fabbisogno(BilancioInput.Regolamento_Cod,
                       BilancioInput.Veg_Cod,
                       BilancioInput.Grfi_Cod,
                       "K2O",
                       BilancioInput.Resa,
                       objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        Necessita = New Necessita
        Necessita.Descrizione = "Fabbisogno della coltura"
        Necessita.Valore_N = N
        Necessita.Valore_P = P
        Necessita.Valore_K = K
        BilancioOutput.ListaNecessita.Add(Necessita)

        ' ---- PERDITE PER LISCIVAZIONE

        N = 0
        P = 0
        K = 0

        Dim Lisciviazione_N_Inverno As Decimal = 0
        Dim Lisciviazione_N_Febbraio As Decimal = 0

        Select Case CInt(BilancioInput.Regolamento_Cod)

            Case Is >= enum_PUARegolamenti.PianoConcimazione_2016

                N = PerditeLisciviazione_N(BilancioInput.Regolamento_Cod,
                               BilancioInput.FaseCicloColturale_Cod,
                               BilancioInput.PioggiaMM,
                               BilancioInput.PioggiaMM_Febbraio,
                               BilancioInput.NTot,
                               IdGruppoTessitura,
                               Lisciviazione_N_Inverno, Lisciviazione_N_Febbraio, objParametri)

            Case Else

                N = PerditeLisciviazione_N(BilancioInput.Regolamento_Cod,
                         BilancioInput.FaseCicloColturale_Cod,
                         BilancioInput.PioggiaMM,
                         BilancioInput.NTot,
                         IdGruppoTessitura,
                         objParametri)

        End Select

        K = PerditeLiscivazione_K(BilancioInput.Regolamento_Cod,
                                          BilancioInput.Argilla,
                                          objParametri)

        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        Necessita = New Necessita
        Necessita.Descrizione = "Perdite per lisciviazione"
        Necessita.Valore_N = N
        Necessita.Valore_P = 0
        Necessita.Valore_K = K
        BilancioOutput.ListaNecessita.Add(Necessita)

        'Select Case CInt(BilancioInput.Regolamento_Cod)

        '    Case Is >= enum_PUARegolamenti.PianoConcimazione_2016

        '        Necessita = New Necessita
        '        Necessita.Descrizione = "   N 'pronto' perso nel periodo autunno invernale"
        '        Necessita.Valore_N = Lisciviazione_N_Inverno
        '        Necessita.Valore_P = 0
        '        Necessita.Valore_K = 0
        '        BilancioOutput.ListaNecessita.Add(Necessita)

        '        Necessita = New Necessita
        '        Necessita.Descrizione = "   N perso all'uscita dell'inverno"
        '        Necessita.Valore_N = Lisciviazione_N_Febbraio
        '        Necessita.Valore_P = 0
        '        Necessita.Valore_K = 0
        '        BilancioOutput.ListaNecessita.Add(Necessita)

        'End Select


        ' ---- IMMOBILIZZAZIONI E DISPERSIONI

        N = 0
        P = 0
        K = 0

        N = Immobilizzazioni_N(BilancioInput.Regolamento_Cod,
                               BilancioInput.Precessione_Veg_Cod,
                               BilancioInput.FaseCicloColturale_Cod,
                               IdGruppoTessitura,
                               BilancioInput.DisponibilitaOssigeno_Cod,
                               BilancioInput.SO,
                               BilancioInput.CN,
                               BilancioInput.NTot,
                               objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K


        Necessita = New Necessita
        Necessita.Descrizione = "Immobilizzazioni e dispersioni"
        Necessita.Valore_N = N
        Necessita.Valore_P = 0
        Necessita.Valore_K = 0
        BilancioOutput.ListaNecessita.Add(Necessita)

        ' ---- ARRICCHIMENTI
        N = 0
        P = 0
        K = 0

        P = Arricchimenti_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.P2O5,
                            BilancioInput.Caco3,
                            IdGruppoTessitura,
                            objParametri)

        K = Arricchimenti_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.K2O,
                            BilancioInput.Argilla,
                            IdGruppoTessitura,
                            objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        Necessita = New Necessita
        Necessita.Descrizione = "Arricchimenti"
        Necessita.Valore_N = 0
        Necessita.Valore_P = P
        Necessita.Valore_K = K
        BilancioOutput.ListaNecessita.Add(Necessita)


        ' ---- ANTICIPAZIONI ANNI FUTURI
        N = 0
        P = 0
        K = 0

        P = Anticipazioni_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.AnticipazioniAnni,
                            BilancioInput.P2O5,
                            IdGruppoTessitura,
                            BilancioInput.Veg_Cod,
                            BilancioInput.Grfi_Cod,
                            BilancioInput.Resa,
                            objParametri)

        K = Anticipazioni_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.AnticipazioniAnni,
                            BilancioInput.K2O,
                            IdGruppoTessitura,
                            BilancioInput.Veg_Cod,
                            BilancioInput.Grfi_Cod,
                            BilancioInput.Resa,
                            objParametri)

        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K


        Necessita = New Necessita
        Necessita.Descrizione = "Anticipazioni Anni futuri"
        Necessita.Valore_N = 0
        Necessita.Valore_P = P
        Necessita.Valore_K = K
        BilancioOutput.ListaNecessita.Add(Necessita)

        '##############################################################################################
        '##############################################################################################
        '#####  DISPONIBILITA'  #######################################################################
        '##############################################################################################
        '##############################################################################################

        TotN_Dispo = 0
        TotP_Dispo = 0
        TotK_Dispo = 0

        ' ---- FERTILITA' DEL SUOLO      

        N = 0
        P = 0
        K = 0

        N = FertilitaSuolo_N(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            IdGruppoTessitura,
                            BilancioInput.SO,
                            BilancioInput.CN,
                            BilancioInput.NTot,
                            objParametri)


        P = FertilitaSuolo_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.P2O5,
                            IdGruppoTessitura,
                            objParametri)

        K = FertilitaSuolo_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.K2O,
                            IdGruppoTessitura,
                            objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = "Fertilità del suolo"
        Disponibilita.Valore_N = N
        Disponibilita.Valore_P = P
        Disponibilita.Valore_K = K
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        ' ---- PRECESSIONE      

        N = 0
        P = 0
        K = 0

        N = Precessione_N(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.Precessione_Veg_Cod,
                            objParametri)


        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = "Precessione"
        Disponibilita.Valore_N = N
        Disponibilita.Valore_P = 0
        Disponibilita.Valore_K = 0
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        ' ---- FERTILITA' ORGANICA RESIDUA      

        N = 0
        P = 0
        K = 0

        N = FertilitaResidua_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Tipo,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Frequenza,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Qta,
                                objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = "Fertilità organica residua"
        Disponibilita.Valore_N = N
        Disponibilita.Valore_P = 0
        Disponibilita.Valore_K = 0
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        ' ---- APPORTI NATURALI      

        N = 0
        P = 0
        K = 0

        N = ApportiNaturali_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                BilancioInput.ColturaProtetta,
                                BilancioInput.Ubicazione_Cod,
                                BilancioInput.FissazioneN_Perc,
                                TotN_Necessita - TotN_Dispo,
                                objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = "Apporti naturali"
        Disponibilita.Valore_N = N
        Disponibilita.Valore_P = 0
        Disponibilita.Valore_K = 0
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        AmmessoN = 0
        AmmessoP = 0
        AmmessoK = 0


        AmmessoN = ApportoAmmesso_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                BilancioInput.Veg_Cod,
                                BilancioInput.Grfi_Cod,
                                TotN_Necessita - TotN_Dispo,
                                objParametri)

        AmmessoP = ApportoAmmesso_P(BilancioInput.Regolamento_Cod,
                                                BilancioInput.FaseCicloColturale_Cod,
                                                BilancioInput.Veg_Cod,
                                                BilancioInput.Grfi_Cod,
                                                BilancioInput.P2O5,
                                                TotP_Necessita - TotP_Dispo,
                                                objParametri)

        AmmessoK = ApportoAmmesso_K(BilancioInput.Regolamento_Cod,
                                                BilancioInput.FaseCicloColturale_Cod,
                                                BilancioInput.Veg_Cod,
                                                BilancioInput.Grfi_Cod,
                                                BilancioInput.K2O,
                                                IdGruppoTessitura,
                                                TotK_Necessita - TotK_Dispo,
                                                objParametri)

        BilancioOutput.N_Necessario = TotN_Necessita
        BilancioOutput.P_Necessario = TotP_Necessita
        BilancioOutput.K_Necessario = TotK_Necessita

        BilancioOutput.N_Disponibile = TotN_Dispo
        BilancioOutput.P_Disponibile = TotP_Dispo
        BilancioOutput.K_Disponibile = TotK_Dispo

        BilancioOutput.N_Calcolato = TotN_Necessita - TotN_Dispo
        BilancioOutput.P_Calcolato = TotP_Necessita - TotP_Dispo
        BilancioOutput.K_Calcolato = TotK_Necessita - TotK_Dispo

        BilancioOutput.N_Ammesso = AmmessoN
        BilancioOutput.P_Ammesso = AmmessoP
        BilancioOutput.K_Ammesso = AmmessoK


        '(09/08/2020 fede) aggiunto il FattoreCorrettivo_N (es.veneto)
        Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim ResaRif As Decimal
        Dim FattoreCorrettivo_N As Decimal

        N = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(BilancioInput.Veg_Cod,
                                                                          BilancioInput.Grfi_Cod,
                                                                          AgronicaCoreDataProvider.TipiEnumerativi.enum_Stato_Impianto.Impianto_Produzione,
                                                                          True,
                                                                          BilancioInput.Regolamento_Cod,
                                                                              ResaRif, FattoreCorrettivo_N,
                                                                              objParametri)


        BilancioOutput.Limite_Mas = N
        BilancioOutput.Resa_Rif = ResaRif
        BilancioOutput.FattoreCorrettivo_N = FattoreCorrettivo_N

        Return BilancioOutput

    End Function

    Public Class input
        Public Property LatLng As String
        Public Property Key As String

    End Class

    Public Function CalcolaBilancioApportiAmmessi(ByVal BilancioInput As PianoConcimazioneBilancio_ApportiAmmessi_input,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_DBServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As PianoConcimazioneBilancio_ApportiAmmessi_output

        Dim strErrore As String = ""

        Dim BilancioOutput As New PianoConcimazioneBilancio_ApportiAmmessi_output

        If BilancioInput.Lat IsNot Nothing AndAlso BilancioInput.Lng IsNot Nothing Then
            BilancioInput.Regione_Cod = getRegioneCodFromLatlng(BilancioInput.Lat, BilancioInput.Lng, objParametri, objParametri_DBServer)
        End If

        If IsNothing(BilancioInput.Regione_Cod) Then
            BilancioInput.Regione_Cod = "08" 'default ER
        End If

        Dim DtReg As DataTable
        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R
        DtReg = objReg.Leggi_DaRegione(BilancioInput.Regione_Cod, enum_PUARegolamenti_Tipo.PianoComcimazione,
                                            Today, Today, "", "", objParametri)
        If Not DtReg Is Nothing AndAlso DtReg.Rows.Count > 0 Then
            BilancioInput.Regolamento_Cod = DtReg.Rows(0).Item("Regolamento_Cod")
        End If

        If BilancioInput.Regolamento_Cod = 0 Then
            strErrore &= "Parametro input Regolamento obbligatorio" & vbCrLf
        End If

        If BilancioInput.Argilla = 0 Then
            strErrore &= "Parametro input Argilla obbligatorio" & vbCrLf
        End If

        If BilancioInput.Sabbia = 0 Then
            strErrore &= "Parametro input Sabbia obbligatorio" & vbCrLf
        End If

        If BilancioInput.CodiceSpecieCliente = "" Then
            strErrore &= "Parametro input Specie Vegetale obbligatorio" & vbCrLf
        End If

        If strErrore <> "" Then
            BilancioOutput.MessaggioErrore = strErrore
            Return BilancioOutput
        End If

        Dim N, P, K As Decimal
        Dim TotN_Necessita, TotP_Necessita, TotK_Necessita As Decimal
        Dim TotN_Dispo, TotP_Dispo, TotK_Dispo As Decimal
        Dim AmmessoN, AmmessoP, AmmessoK As Decimal

        'estraggo codifica specie
        Dim Veg_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 9 'produzione
        Dim Precessione_Veg_Cod As Integer = 21 'non definita
        Dim Codice_Cliente As Integer = 2116 'irrinet

        Dim objCodif As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R
        Dim DtCod As DataTable
        DtCod = objCodif.Leggi(Codice_Cliente, BilancioInput.CodiceSpecieCliente, "", 0, 0, 0, Today, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
        If Not DtCod Is Nothing AndAlso DtCod.Rows.Count > 0 Then
            Veg_Cod = DtCod.Rows(0).Item("veg_cod")
        Else
            strErrore &= "Parametro input Specie Vegetale non mappata" & vbCrLf
            BilancioOutput.MessaggioErrore = strErrore
            Return BilancioOutput
        End If

        If BilancioInput.Finalita_Cod <> 0 Then
            Grfi_Cod = BilancioInput.Finalita_Cod
        End If
        If BilancioInput.PrecessioneSpecie_Cod <> 0 Then
            Precessione_Veg_Cod = BilancioInput.PrecessioneSpecie_Cod
        End If

        Dim objTessiture As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
        Dim IdGruppoTessitura As Integer = 0
        IdGruppoTessitura = objTessiture.Leggi_IdGruppoTessitura_Da_SabbiaArgilla(BilancioInput.Sabbia, BilancioInput.Argilla, objParametri)

        'PIANO CONCIMAZIONE 2017
        If BilancioInput.Regolamento_Cod >= enum_PUARegolamenti.PianoConcimazione_2017 Then

            If (BilancioInput.Mg <> 0 And BilancioInput.K2O <> 0) Then

                '(03/04/2017 fede) introdotta valutazione Mg/K e K/CSC per modifica valutazione K2O
                'se Mg/K > 6 e K/CSC < 2 --> correzione di Potassio 
                'Correzione di Potassio = MIN(K2O editato e valore Minimo tabella PC_GrigliaK2O con dotazione Media (=3))
                'calcolo Mg/K ( = Mg meq / K meq = Mg ppm * 0,008230 / K2O ppm * 0,002558 * 0,83333)
                Dim RappMgsuK As Decimal = 0
                RappMgsuK = (BilancioInput.Mg * 0.00823) / (BilancioInput.K2O * 0.002558 * 0.83333)

                'calcolo K/CSC ( = K meq / CSC = K2O ppm * 0,002558 * 0,83333 / CSC)
                Dim RappKsuCSC As Decimal = 0
                If BilancioInput.CSC = 0 Then
                    RappKsuCSC = 100
                Else
                    RappKsuCSC = ((BilancioInput.K2O * 0.002558 * 0.83333) / BilancioInput.CSC) * 100
                End If

                Dim K2ORif As Decimal = 0
                If Not (RappMgsuK <= 6 And RappKsuCSC >= 2) Then
                    Dim objPC_GrigliaK As New AgronicaCoreMetaSchemaDAL.PC_GrigliaK2O_R
                    Dim Dt As DataTable
                    Dt = objPC_GrigliaK.Leggi(BilancioInput.Regolamento_Cod,
                                                  0,
                                                  IdGruppoTessitura,
                                                  3,
                                                  "", "Massimo DESC",
                                                  objParametri)

                    Dim MinDotazioneNormale As Decimal = 0
                    If Dt.Rows.Count > 0 Then
                        MinDotazioneNormale = Dt.Rows(0).Item("Minimo")
                    End If
                    K2ORif = Math.Min(BilancioInput.K2O, MinDotazioneNormale)
                    BilancioInput.K2O = K2ORif
                End If

            End If

        End If



        '##############################################################################################
        '##############################################################################################
        '#####  NECESSITA'      #######################################################################
        '##############################################################################################
        '##############################################################################################

        TotN_Necessita = 0
        TotP_Necessita = 0
        TotK_Necessita = 0

        ' ---- FABBISOGNO DELLA COLTURA        

        N = 0
        P = 0
        K = 0

        N = Fabbisogno(BilancioInput.Regolamento_Cod,
                       Veg_Cod,
                       Grfi_Cod,
                       "N",
                       BilancioInput.Resa,
                       objParametri)

        P = Fabbisogno(BilancioInput.Regolamento_Cod,
                       Veg_Cod,
                       Grfi_Cod,
                       "P2O5",
                       BilancioInput.Resa,
                       objParametri)

        K = Fabbisogno(BilancioInput.Regolamento_Cod,
                       Veg_Cod,
                       Grfi_Cod,
                       "K2O",
                       BilancioInput.Resa,
                       objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K


        ' ---- PERDITE PER LISCIVAZIONE

        N = 0
        P = 0
        K = 0

        Dim Lisciviazione_N_Inverno As Decimal = 0
        Dim Lisciviazione_N_Febbraio As Decimal = 0

        Select Case CInt(BilancioInput.Regolamento_Cod)

            Case Is >= enum_PUARegolamenti.PianoConcimazione_2016

                N = PerditeLisciviazione_N(BilancioInput.Regolamento_Cod,
                               BilancioInput.FaseCicloColturale_Cod,
                               BilancioInput.PioggiaMM,
                               BilancioInput.PioggiaMM_Febbraio,
                               BilancioInput.NTot,
                               IdGruppoTessitura,
                               Lisciviazione_N_Inverno, Lisciviazione_N_Febbraio, objParametri)

            Case Else

                N = PerditeLisciviazione_N(BilancioInput.Regolamento_Cod,
                         BilancioInput.FaseCicloColturale_Cod,
                         BilancioInput.PioggiaMM,
                         BilancioInput.NTot,
                         IdGruppoTessitura,
                         objParametri)

        End Select

        K = PerditeLiscivazione_K(BilancioInput.Regolamento_Cod,
                                  BilancioInput.Argilla,
                                  objParametri)

        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        ' ---- IMMOBILIZZAZIONI E DISPERSIONI

        N = 0
        P = 0
        K = 0

        N = Immobilizzazioni_N(BilancioInput.Regolamento_Cod,
                               Precessione_Veg_Cod,
                               BilancioInput.FaseCicloColturale_Cod,
                               IdGruppoTessitura,
                               BilancioInput.DisponibilitaOssigeno_Cod,
                               BilancioInput.SO,
                               BilancioInput.CN,
                               BilancioInput.NTot,
                               objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        ' ---- ARRICCHIMENTI
        N = 0
        P = 0
        K = 0

        P = Arricchimenti_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.P2O5,
                            BilancioInput.Caco3,
                            IdGruppoTessitura,
                            objParametri)

        K = Arricchimenti_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.K2O,
                            BilancioInput.Argilla,
                            IdGruppoTessitura,
                            objParametri)


        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K

        ' ---- ANTICIPAZIONI ANNI FUTURI
        N = 0
        P = 0
        K = 0

        P = Anticipazioni_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.AnticipazioniAnni,
                            BilancioInput.P2O5,
                            IdGruppoTessitura,
                            Veg_Cod,
                            Grfi_Cod,
                            BilancioInput.Resa,
                            objParametri)

        K = Anticipazioni_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.AnticipazioniAnni,
                            BilancioInput.K2O,
                            IdGruppoTessitura,
                            Veg_Cod,
                            Grfi_Cod,
                            BilancioInput.Resa,
                            objParametri)

        TotN_Necessita += N
        TotP_Necessita += P
        TotK_Necessita += K


        '##############################################################################################
        '##############################################################################################
        '#####  DISPONIBILITA'  #######################################################################
        '##############################################################################################
        '##############################################################################################

        TotN_Dispo = 0
        TotP_Dispo = 0
        TotK_Dispo = 0

        ' ---- FERTILITA' DEL SUOLO      

        N = 0
        P = 0
        K = 0

        N = FertilitaSuolo_N(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            IdGruppoTessitura,
                            BilancioInput.SO,
                            BilancioInput.CN,
                            BilancioInput.NTot,
                            objParametri)


        P = FertilitaSuolo_P(BilancioInput.Regolamento_Cod,
                            BilancioInput.P2O5,
                            IdGruppoTessitura,
                            objParametri)

        K = FertilitaSuolo_K(BilancioInput.Regolamento_Cod,
                            BilancioInput.K2O,
                            IdGruppoTessitura,
                            objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K


        ' ---- PRECESSIONE      

        N = 0
        P = 0
        K = 0

        N = Precessione_N(BilancioInput.Regolamento_Cod,
                            BilancioInput.FaseCicloColturale_Cod,
                            BilancioInput.PrecessioneSpecie_Cod,
                            objParametri)


        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K


        ' ---- FERTILITA' ORGANICA RESIDUA      

        N = 0
        P = 0
        K = 0

        N = FertilitaResidua_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Tipo,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Frequenza,
                                BilancioInput.FertilizzanteOrganico_ColturePrecedenti_Qta,
                                objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K

        ' ---- APPORTI NATURALI      

        N = 0
        P = 0
        K = 0

        N = ApportiNaturali_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                BilancioInput.ColturaProtetta,
                                BilancioInput.Ubicazione_Cod,
                                BilancioInput.FissazioneN_Perc,
                                TotN_Necessita - TotN_Dispo,
                                objParametri)

        TotN_Dispo += N
        TotP_Dispo += P
        TotK_Dispo += K


        AmmessoN = 0
        AmmessoP = 0
        AmmessoK = 0


        AmmessoN = ApportoAmmesso_N(BilancioInput.Regolamento_Cod,
                                BilancioInput.FaseCicloColturale_Cod,
                                Veg_Cod,
                                Grfi_Cod,
                                TotN_Necessita - TotN_Dispo,
                                objParametri)

        AmmessoP = ApportoAmmesso_P(BilancioInput.Regolamento_Cod,
                                                BilancioInput.FaseCicloColturale_Cod,
                                                Veg_Cod,
                                                Grfi_Cod,
                                                BilancioInput.P2O5,
                                                TotP_Necessita - TotP_Dispo,
                                                objParametri)

        AmmessoK = ApportoAmmesso_K(BilancioInput.Regolamento_Cod,
                                                BilancioInput.FaseCicloColturale_Cod,
                                                Veg_Cod,
                                                Grfi_Cod,
                                                BilancioInput.K2O,
                                                IdGruppoTessitura,
                                                TotK_Necessita - TotK_Dispo,
                                                objParametri)


        BilancioOutput.N_Ammesso = AmmessoN
        BilancioOutput.P_Ammesso = AmmessoP
        BilancioOutput.K_Ammesso = AmmessoK


        Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim Resa As Decimal
        N = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(Veg_Cod,
                                                                          Grfi_Cod,
                                                                          AgronicaCoreDataProvider.TipiEnumerativi.enum_Stato_Impianto.Impianto_Produzione,
                                                                          True,
                                                                          BilancioInput.Regolamento_Cod,
                                                                              Resa, 0,
                                                                              objParametri)

        BilancioOutput.Limite_Mas = N

        Return BilancioOutput

    End Function

    Private Shared Function getRegioneCodFromLatlng(Lat As Decimal, Lng As Decimal,
                                                    objParametri As AgronicaCoreParametri,
                                                    objParametri_DBServer As AgronicaCoreParametri
                                                    ) As String

        Dim Regione_Cod As String = Nothing
        Dim ProvinciaSigla As String = ""
        '------------------------------------
        'LEGGO DA CONFIGURAZIONE SITI LA CHIAVE DI GOOGLE
        '------------------------------------
        Try

            Dim url As String = ""

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Try
                url = objConfSiti.Leggi_Valore(0, "Google_GeocodingBaseUrl", "", "", objParametri_DBServer)
            Catch ex As Exception
                url = ""
            End Try
            If url = "" Then
                Return Nothing
            End If

            url += "&latlng=" + CStr(Lat).Replace(",", ".") + "," + CStr(Lng).Replace(",", ".")

            Dim hlpHttp As New Http
            Dim resp = hlpHttp.CallWS_RestSharp_JSON(url, "", "GET", "", Nothing)

            Dim output = JValue.Parse(JsonConvert.SerializeObject(resp.Content))

            For Each x In output.Item("results")
                For Each component In x.Item("address_components")
                    If component.Item("types").ToString().Contains("administrative_area_level_2") Then
                        ProvinciaSigla = component.Item("short_name")
                    End If
                    If ProvinciaSigla <> "" Then
                        Exit For
                    End If
                Next
                If ProvinciaSigla <> "" Then
                    Exit For
                End If
            Next

            If ProvinciaSigla <> "" Then
                Dim objListaProvince As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
                objListaProvince.Regione_from_Provincia(ProvinciaSigla, 0, Regione_Cod, "", objParametri)
            End If

        Catch ex As Exception
            Regione_Cod = Nothing
        End Try

        Return Regione_Cod
    End Function

#End Region

#Region "BILANCIO IBF"
    Private Shared Function Fabbisogno_IBF(ByVal BilancioInput As PianoConcimazioneBilancio_IBF_INPUT,
                                           ByRef Valori_Tabellati As Valori_Tabellati,
                                           ByRef Valori_Calcolati As Valori_Calcolati
                                           ) As Decimal


        '--------------------------------
        '   CALCOLO
        '--------------------------------
        If BilancioInput.COLQ_PercUmiditaPrevistaColturaPrincipale > 0 Then
            Valori_Calcolati.COLS_ResaUtilePrevistaSS_THa =
                BilancioInput.COLP_ResaUtilePrevistaColturaPrincipale_THa *
                ((100 - BilancioInput.COLQ_PercUmiditaPrevistaColturaPrincipale) / 100)

        Else Valori_Calcolati.COLS_ResaUtilePrevistaSS_THa =
                BilancioInput.COLP_ResaUtilePrevistaColturaPrincipale_THa *
                (1 - (Valori_Tabellati.COL13_UmiditaRaccolta_AsportazioneResidui_ColturaPrincipale / 100))
        End If

        Valori_Calcolati.COLT_ResaResiduiAereiPrevista_THa =
            (Valori_Calcolati.COLS_ResaUtilePrevistaSS_THa / Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_ColturaPrincipale) *
            (1 - Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_ColturaPrincipale)

        Valori_Calcolati.COLU_FabbisognoColturaPrincipale_KgNHa =
        ((Valori_Tabellati.COL3_NPercSS_AsportazioniProduzioneUtile_ColturaPrincipale * Valori_Calcolati.COLS_ResaUtilePrevistaSS_THa) * 10) +
        ((Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_ColturaPrincipale * Valori_Calcolati.COLT_ResaResiduiAereiPrevista_THa) * 10)

        Return Valori_Calcolati.COLU_FabbisognoColturaPrincipale_KgNHa

    End Function

    Private Shared Function Entrate_IBF(ByVal BilancioInput As PianoConcimazioneBilancio_IBF_INPUT,
                                        ByRef Valori_Tabellati As Valori_Tabellati,
                                        ByRef Valori_Calcolati As Valori_Calcolati,
                                        objParametri As AgronicaCoreParametri
                                        ) As Decimal


        '--------------------------------
        '   CALCOLO
        '--------------------------------
        Valori_Calcolati.COLAA_TAvgPeriodoRiferimento =
            BilancioInput.AVG_Temperatura_ColturaInCampo

        Valori_Calcolati.COLY_PeriodoRiferimentoMineralizzazioneSO_Mesi =
            BilancioInput.COLX_PeriodoRaccoltaColturaPrincipale - BilancioInput.COLW_PeriodoSeminaColturaPrincipale

        Valori_Calcolati.COLZ_PeriodoRiferimentoPrecessione_Mesi =
            BilancioInput.COLX_PeriodoRaccoltaColturaPrincipale - BilancioInput.COLV_PeriodoInterramentoResiduiPrecessione

        Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa =
            (((((Valori_Calcolati.COLAA_TAvgPeriodoRiferimento - 0.5) * 240) /
            ((BilancioInput.SoilDataset_K_ArgillaPerc + 20) *
            ((0.3 * BilancioInput.SoilDataset_J_LimoPerc) + 20)) / 12) * Valori_Calcolati.COLY_PeriodoRiferimentoMineralizzazioneSO_Mesi) / 100) *
            (10000 * 0.15 * BilancioInput.SoilDataset_M_BulkDensityTM * 1000) *
            (BilancioInput.SoilDataset_T_NPerc * 10 / 1000)

        Valori_Calcolati.COLAB_NNellaSO_Perc =
            (10000 * 0.3 * BilancioInput.SoilDataset_M_BulkDensityTM * 1000 *
            (BilancioInput.SoilDataset_T_NPerc * 10 / 1000) /
            ((10000 * 0.3 * BilancioInput.SoilDataset_M_BulkDensityTM * 1000 *
            (BilancioInput.SoilDataset_S_SOPerc * 10)) / 1000) * 100)

        If BilancioInput.COLK_PercUmiditaRaccoltaPrecessione > 0 Then
            Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa =
            (BilancioInput.COLJ_ResaStoricaPrecessione_THa / Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_Precessione) *
            ((100 - BilancioInput.COLK_PercUmiditaRaccoltaPrecessione) / 100)
        Else
            Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa =
            (BilancioInput.COLJ_ResaStoricaPrecessione_THa / Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_Precessione) *
            (1 - Valori_Tabellati.COL13_UmiditaRaccolta_AsportazioneResidui_Precessione / 100)
        End If

        If BilancioInput.COLH_PrecessioneAnnoPrecedente = 6 AndAlso
            BilancioInput.Grfi_Cod_PrecessioneAnnoPrecedente = 9 Then
            '6 = BARBABIETOLA - 9 = PRODUZIONE
            Select Case BilancioInput.COLI_ResiduiPrecessioneAsportati
                Case True
                    Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa =
                            Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa * 0.02
                Case False
                    Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa =
                            Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa *
                            (1 - Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_Precessione) + Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa * 0.02
            End Select
        Else
            Select Case BilancioInput.COLI_ResiduiPrecessioneAsportati
                Case True
                    Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa =
                            Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa * 0.15
                Case False
                    Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa =
                        Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa *
                        (1 - Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_Precessione) + Valori_Calcolati.COLM_BiomassaTotaleAereaPrecessioneSS_THa * 0.15
            End Select
        End If

        Valori_Calcolati.COLAG_EfficienzaAzoto = estraiEfficienzaDaMomentoDistribuzioneEpoca(BilancioInput.Regolamento_Cod, BilancioInput.N_KgHa, BilancioInput.COLAD_TipoConcimeOrganico, BilancioInput.COLAF_ModalitaDistribuzioneRelativoColturaEpoca, objParametri)
        'CORRETTO??
        Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa = BilancioInput.N_KgHa * Valori_Calcolati.COLAG_EfficienzaAzoto
        'Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa =
        '            BilancioInput.COLAC_ConcimeOrganico_THa * 1000 *
        '            Valori_Tabellati.COL5_PercSS / 100 *
        '            Valori_Tabellati.COL6_NPercSS / 100 *
        '            Valori_Calcolati.COLAG_EfficienzaAzoto

        If BilancioInput.COLH_PrecessioneAnnoPrecedente = 21 AndAlso
            BilancioInput.Grfi_Cod_PrecessioneAnnoPrecedente = 9 Then
            '21 = ERBA MEDICA - 9 = PRODUZIONE
            Valori_Calcolati.COLAJ_Precessione_KgNHa =
                4 * (((Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_Precessione * 10 * Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa) -
                (Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa * 10 * Valori_Tabellati.COL15_K1_CoefficienteIsoumico_Precessione * Valori_Calcolati.COLAB_NNellaSO_Perc / 100)) +
                Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa) * (Valori_Calcolati.COLZ_PeriodoRiferimentoPrecessione_Mesi) / 12
        Else
            Valori_Calcolati.COLAJ_Precessione_KgNHa =
                    (((Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_Precessione * 10 * Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa) -
                    (Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa * 10 * Valori_Tabellati.COL15_K1_CoefficienteIsoumico_Precessione * Valori_Calcolati.COLAB_NNellaSO_Perc / 100)) +
                    Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa) * Valori_Calcolati.COLZ_PeriodoRiferimentoPrecessione_Mesi / 12

        End If


        Return Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa + Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa + Valori_Calcolati.COLAJ_Precessione_KgNHa

    End Function

    Private Shared Function Uscite_IBF(ByVal BilancioInput As PianoConcimazioneBilancio_IBF_INPUT,
                                       ByRef Valori_Tabellati As Valori_Tabellati,
                                       ByRef Valori_Calcolati As Valori_Calcolati,
                                       ByRef FABBISOGNO_KgNHa As Decimal) As Decimal


        '--------------------------------
        '   CALCOLO
        '--------------------------------
        If (15 - BilancioInput.COLW_PeriodoSeminaColturaPrincipale) < 0 Then
            Valori_Calcolati.COLAK_NMineralizzazione_2802_KgNHa = 0
        Else
            Valori_Calcolati.COLAK_NMineralizzazione_2802_KgNHa =
                (((((BilancioInput.AVG_Temperatura_MeseSemina_Febbraio - 0.5) * 240) /
            ((BilancioInput.SoilDataset_K_ArgillaPerc + 20) * ((0.3 * BilancioInput.SoilDataset_J_LimoPerc) + 20)) / 12) *
            (15 - BilancioInput.COLW_PeriodoSeminaColturaPrincipale)) / 100 *
            (10000 * 0.15 * BilancioInput.SoilDataset_M_BulkDensityTM * 1000) *
            (BilancioInput.SoilDataset_T_NPerc * 10 / 1000))
        End If

        Valori_Calcolati.COLAL_NPrecessione_2802_KgNHa =
            ((Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_Precessione * 10 * Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa) -
            (Valori_Calcolati.COLN_ResiduiLasciatiDaPrecessioneSS_THa * 10 * Valori_Tabellati.COL15_K1_CoefficienteIsoumico_Precessione * Valori_Calcolati.COLAB_NNellaSO_Perc / 100)) *
            (15 - BilancioInput.COLV_PeriodoInterramentoResiduiPrecessione) / 12
        If Valori_Calcolati.COLAL_NPrecessione_2802_KgNHa < 0 Then
            Valori_Calcolati.COLAL_NPrecessione_2802_KgNHa = 0
        End If

        Dim KL_Pioggia As Decimal
        Select Case BilancioInput.PiovositaOttobreFebbraio
            Case < 250
                KL_Pioggia = 0
            Case 250 To 299
                KL_Pioggia = 16.7 / 100
            Case 300 To 349
                KL_Pioggia = 37.5 / 100
            Case 350 To 399
                KL_Pioggia = 50 / 100
            Case 400 To 449
                KL_Pioggia = 58.3 / 100
            Case 450 To 499
                KL_Pioggia = 64.3 / 100
            Case Else
                KL_Pioggia = 9999 / 100
        End Select
        Valori_Calcolati.COLAM_NLisciviazione_KgNHa =
            (Valori_Calcolati.COLAL_NPrecessione_2802_KgNHa + Valori_Calcolati.COLAK_NMineralizzazione_2802_KgNHa) * KL_Pioggia

        If BilancioInput.COLE_ColturaPrincipaleAnno = 55 AndAlso
            BilancioInput.Grfi_Cod_ColturaPrincipaleAnno = 9 Then
            '55 = RISO - 9 = PRODUZIONE
            Select Case BilancioInput.SoilDataset_N_pH
                Case < 7.5
                    Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa = 0
                Case 7.5 To 7.99
                    Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa =
                            (FABBISOGNO_KgNHa -
                            Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                            Valori_Calcolati.COLAJ_Precessione_KgNHa +
                            Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                            BilancioInput.COLAP_NDistribuito_KgNHa) * 0.5
                Case 8 To 8.49
                    Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa =
                            (FABBISOGNO_KgNHa -
                            Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                            Valori_Calcolati.COLAJ_Precessione_KgNHa +
                            Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                            BilancioInput.COLAP_NDistribuito_KgNHa) * 0.1
                Case >= 8.5
                    Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa =
                            (FABBISOGNO_KgNHa -
                            Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                            Valori_Calcolati.COLAJ_Precessione_KgNHa +
                            Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                            BilancioInput.COLAP_NDistribuito_KgNHa) * 0.15
            End Select
        Else
            Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa = 0
        End If

        If BilancioInput.COLE_ColturaPrincipaleAnno = 55 AndAlso
            BilancioInput.Grfi_Cod_ColturaPrincipaleAnno = 9 Then
            '55 = RISO - 9 = PRODUZIONE
            Select Case BilancioInput.SoilDataset_Z_CSC
                Case < 10
                    Valori_Calcolati.COLAO_NPercolazione_KgNHa =
                        (FABBISOGNO_KgNHa -
                        Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                        Valori_Calcolati.COLAJ_Precessione_KgNHa +
                        Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                        BilancioInput.COLAP_NDistribuito_KgNHa) * 0.1
                Case 10 To 17
                    Valori_Calcolati.COLAO_NPercolazione_KgNHa =
                        (FABBISOGNO_KgNHa -
                        Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                        Valori_Calcolati.COLAJ_Precessione_KgNHa +
                        Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                        BilancioInput.COLAP_NDistribuito_KgNHa) * 0.05
                Case Else
                    Valori_Calcolati.COLAO_NPercolazione_KgNHa = 0
            End Select
        Else
            Valori_Calcolati.COLAO_NPercolazione_KgNHa = 0
        End If

        Return Valori_Calcolati.COLAM_NLisciviazione_KgNHa + Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa + Valori_Calcolati.COLAO_NPercolazione_KgNHa

    End Function

    Private Shared Function Dose_Finale_Elemento_IBF(ByVal BilancioInput As PianoConcimazioneBilancio_IBF_INPUT,
                                                     ByRef Valori_Calcolati As Valori_Calcolati,
                                                     ByRef FABBISOGNO_KgNHa As Decimal) As Decimal

        '--------------------------------
        '   CALCOLO
        '--------------------------------
        If BilancioInput.COLE_ColturaPrincipaleAnno = 55 AndAlso
            BilancioInput.Grfi_Cod_PrecessioneAnnoPrecedente = 9 AndAlso
            BilancioInput.COLAQ_SeminaInterrataRisaia Then
            '55 = RISO - 9 = PRODUZIONE
            Valori_Calcolati.COLAR_NDistibuzione_KgHa =
                                Math.Abs(FABBISOGNO_KgNHa -
                                Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                                Valori_Calcolati.COLAJ_Precessione_KgNHa +
                                Valori_Calcolati.COLAM_NLisciviazione_KgNHa -
                                BilancioInput.COLAP_NDistribuito_KgNHa) * 1.125 +
                                Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa +
                                Valori_Calcolati.COLAO_NPercolazione_KgNHa
        Else
            Valori_Calcolati.COLAR_NDistibuzione_KgHa =
                            FABBISOGNO_KgNHa -
                            Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa -
                            Valori_Calcolati.COLAJ_Precessione_KgNHa +
                            Valori_Calcolati.COLAM_NLisciviazione_KgNHa +
                            Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa +
                            Valori_Calcolati.COLAO_NPercolazione_KgNHa -
                            BilancioInput.COLAP_NDistribuito_KgNHa
        End If


        Return Valori_Calcolati.COLAR_NDistibuzione_KgHa

    End Function

    Public Function CalcolaBilancio_IBF(ByVal BilancioInput As PianoConcimazioneBilancio_IBF_INPUT,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As PianoConcimazioneBilancio_output

        Dim BilancioOutput As New PianoConcimazioneBilancio_output
        Dim Necessita As Necessita
        Dim Disponibilita As Disponibilita
        Dim TotN_Necessita As Decimal
        Dim TotN_Dispo As Decimal

        Dim Valori_Tabellati As New Valori_Tabellati
        Dim Valori_Calcolati As New Valori_Calcolati

        estraiValori_CoefficienteAssorbimento(BilancioInput.Regolamento_Cod, BilancioInput.COLE_ColturaPrincipaleAnno, BilancioInput.Grfi_Cod_ColturaPrincipaleAnno, Valori_Tabellati.COL3_NPercSS_AsportazioniProduzioneUtile_ColturaPrincipale, Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_ColturaPrincipale, objParametri)
        estraiValori_CoefficienteAssorbimento(BilancioInput.Regolamento_Cod, BilancioInput.COLH_PrecessioneAnnoPrecedente, BilancioInput.Grfi_Cod_PrecessioneAnnoPrecedente, Valori_Tabellati.COL3_NPercSS_AsportazioniProduzioneUtile_Precessione, Valori_Tabellati.COL8_NPercSS_AsportazioneResidui_Precessione, objParametri)

        estraiValori_DatiColture_Raccolta(BilancioInput.Regolamento_Cod, BilancioInput.COLE_ColturaPrincipaleAnno, BilancioInput.Grfi_Cod_ColturaPrincipaleAnno, Valori_Tabellati.COL13_UmiditaRaccolta_AsportazioneResidui_ColturaPrincipale, Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_ColturaPrincipale, Valori_Tabellati.COL15_K1_CoefficienteIsoumico_ColturaPrincipale, objParametri)
        estraiValori_DatiColture_Raccolta(BilancioInput.Regolamento_Cod, BilancioInput.COLH_PrecessioneAnnoPrecedente, BilancioInput.Grfi_Cod_PrecessioneAnnoPrecedente, Valori_Tabellati.COL13_UmiditaRaccolta_AsportazioneResidui_Precessione, Valori_Tabellati.COL14_HarvestIndex_AsportazioneResidui_Precessione, Valori_Tabellati.COL15_K1_CoefficienteIsoumico_Precessione, objParametri)
        BilancioInput.SoilDataset_M_BulkDensityTM = estraiValore_BulkDensity(BilancioInput.Regolamento_Cod, BilancioInput.SoilDataset_I_SabbiaPerc, BilancioInput.SoilDataset_K_ArgillaPerc, objParametri)


        '----------------------------------------------------------------
        '                           FABBISOGNO
        '----------------------------------------------------------------
        Dim FABBISOGNO_KgNHa As Decimal = Fabbisogno_IBF(BilancioInput, Valori_Tabellati, Valori_Calcolati)

        TotN_Necessita += FABBISOGNO_KgNHa

        Necessita = New Necessita
        Necessita.Descrizione = Gias.FabbisognoDellaColtura
        Necessita.Valore_N = FABBISOGNO_KgNHa
        BilancioOutput.ListaNecessita.Add(Necessita)


        '----------------------------------------------------------------
        '                           ENTRATE            
        '----------------------------------------------------------------
        Dim TOT_ENTRATE As Decimal = Entrate_IBF(BilancioInput, Valori_Tabellati, Valori_Calcolati, objParametri)

        TotN_Dispo += Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = Gias.FertilitaDelSuolo
        Disponibilita.Valore_N = Valori_Calcolati.COLAI_MineralizzazioneSO_KgNHa
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        TotN_Dispo += Valori_Calcolati.COLAJ_Precessione_KgNHa

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = Gias.Precessione
        Disponibilita.Valore_N = Valori_Calcolati.COLAJ_Precessione_KgNHa
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)

        TotN_Dispo += Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa

        Disponibilita = New Disponibilita
        Disponibilita.Descrizione = Gias.FertilitaOrganicaResidua
        Disponibilita.Valore_N = Valori_Calcolati.COLAH_NCOConcimazioneOrganica_KgNHa
        BilancioOutput.ListaDisponibilita.Add(Disponibilita)


        '----------------------------------------------------------------
        '                           USCITE
        '----------------------------------------------------------------
        Dim TOT_USCITE As Decimal = Uscite_IBF(BilancioInput, Valori_Tabellati, Valori_Calcolati, FABBISOGNO_KgNHa)

        TotN_Necessita += Valori_Calcolati.COLAM_NLisciviazione_KgNHa

        Necessita = New Necessita
        Necessita.Descrizione = Gias.PerditePerLisciviazione
        Necessita.Valore_N = Valori_Calcolati.COLAM_NLisciviazione_KgNHa
        BilancioOutput.ListaNecessita.Add(Necessita)

        TotN_Necessita += Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa + Valori_Calcolati.COLAO_NPercolazione_KgNHa

        Necessita = New Necessita
        Necessita.Descrizione = Gias.ImmobilizzazioniDispersioni
        Necessita.Valore_N = Valori_Calcolati.COLAN_NVolatilizzazione_KgNHa + Valori_Calcolati.COLAO_NPercolazione_KgNHa
        BilancioOutput.ListaNecessita.Add(Necessita)


        '----------------------------------------------------------------
        '                  DOSE FINALE ELEMENTO
        '----------------------------------------------------------------
        Dim DOSE_FINALE_ELEMENTO As Decimal = Dose_Finale_Elemento_IBF(BilancioInput, Valori_Calcolati, FABBISOGNO_KgNHa)


        '----------------------------------------------------------------
        '                  OUTPUT
        '----------------------------------------------------------------
        BilancioOutput.N_Necessario = TotN_Necessita
        BilancioOutput.N_Disponibile = TotN_Dispo
        BilancioOutput.N_Calcolato = TotN_Necessita - TotN_Dispo


        BilancioOutput.N_Ammesso = DOSE_FINALE_ELEMENTO
        'BilancioOutput.N_Ammesso = ApportoAmmesso_N(BilancioInput.Regolamento_Cod,
        '                                            0,
        '                                            BilancioInput.COLE_ColturaPrincipaleAnno,
        '                                            BilancioInput.Grfi_Cod_ColturaPrincipaleAnno,
        '                                            TotN_Necessita - TotN_Dispo,
        '                                            objParametri)

        Dim objLimitiAzotoxSpecie As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim ResaRif As Decimal
        Dim FattoreCorrettivo_N As Decimal

        Dim N = objLimitiAzotoxSpecie.RecuperaAzotoResaFromVegCod_Regolamento(BilancioInput.COLE_ColturaPrincipaleAnno,
                                                                              BilancioInput.Grfi_Cod_ColturaPrincipaleAnno,
                                                                              enum_Stato_Impianto.Impianto_Produzione,
                                                                              True,
                                                                              BilancioInput.Regolamento_Cod,
                                                                              ResaRif, FattoreCorrettivo_N,
                                                                              objParametri)


        BilancioOutput.Limite_Mas = N
        BilancioOutput.Resa_Rif = ResaRif
        BilancioOutput.FattoreCorrettivo_N = FattoreCorrettivo_N

        Return BilancioOutput

    End Function

    Private Shared Sub estraiValori_DatiColture_Raccolta(Regolamento_Cod As Integer,
                                                         Veg_Cod As Integer,
                                                         Grfi_Cod As Integer,
                                                         ByRef UmiditaRaccolta As Decimal,
                                                         ByRef HarvestIndex As Decimal,
                                                         ByRef K1_CoefficienteIsoumico As Decimal,
                                                         objParametri As AgronicaCoreParametri)

        Dim objDatiColtureRaccolta As New AgronicaCoreMetaSchemaDAL.DatiColture_Raccolta_R
        Dim dt = objDatiColtureRaccolta.Leggi(Regolamento_Cod, Veg_Cod, Grfi_Cod,
                                              AGRODATAINIZIO, AGRODATAFINE,
                                              "", "",
                                              objParametri)

        If dt.Rows.Count > 0 Then
            Dim DatiColtureRaccolta = dt.Rows(0)
            UmiditaRaccolta = DatiColtureRaccolta.Item("Umidita_Raccolta")
            HarvestIndex = DatiColtureRaccolta.Item("Harvest_Index")
            K1_CoefficienteIsoumico = DatiColtureRaccolta.Item("Coefficiente_Isoumico")
        End If
    End Sub

    Private Shared Sub estraiValori_CoefficienteAssorbimento(Regolamento_Cod As Integer,
                                                           Veg_Cod As Integer,
                                                           Grfi_Cod As Integer,
                                                           ByRef NPercSS_AsportazioniProduzioneUtile As Decimal,
                                                           ByRef NPercSS_AsportazioneResidui As Decimal,
                                                           objParametri As AgronicaCoreParametri)

        Dim objCoefficienteAsorbimento As New AgronicaCoreMetaSchemaDAL.CoefficienteAsorbimento_R
        Dim dt = objCoefficienteAsorbimento.Leggi(Regolamento_Cod, Veg_Cod, Grfi_Cod,
                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                  "", "",
                                                  objParametri)

        If dt.Rows.Count > 0 Then
            Dim CoefficienteAsorbimento = dt.Rows(0)
            NPercSS_AsportazioniProduzioneUtile = CoefficienteAsorbimento.Item("Nutile")
            NPercSS_AsportazioneResidui = CoefficienteAsorbimento.Item("NResiduo")
        End If

    End Sub

    Private Shared Function estraiValore_BulkDensity(Regolamento_Cod As Integer,
                                                     Sabbia As Decimal,
                                                     Argilla As Decimal,
                                                     objParametri As AgronicaCoreParametri) As Decimal
        Dim BulkDensity As Decimal = 0

        Dim objTessiture As New AgronicaCoreMetaSchemaDAL.PC_TriangoloTessitura_R
        Dim Id_Tessitura As Integer = objTessiture.Leggi_IdTessitura(Sabbia, Argilla, objParametri)

        Dim objDensitaApparente As New AgronicaCoreMetaSchemaDAL.Densita_Apparente_R
        Dim dt = objDensitaApparente.Leggi(Regolamento_Cod, Id_Tessitura, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

        If dt.Rows.Count > 0 Then
            Dim DensitaApparente = dt.Rows(0)
            BulkDensity = DensitaApparente.Item("Densita")
        End If

        Return BulkDensity
    End Function

    Private Shared Function estraiEfficienzaDaMomentoDistribuzioneEpoca(Regolamento_Cod As Integer,
                                                                        Qta_N As Decimal,
                                                                        ConcimeOrganico As Integer,
                                                                        MomentoDistribuzioneEpoca As Integer,
                                                                        objParametri As AgronicaCoreParametri) As Decimal
        Dim Efficienza As Decimal = 1


        'DOSE = 1
        '   se Qta_N <= 125;
        'DOSE = 2
        '   se qta_n>125
        '(dose alta o bassa è riferito al quantitativo di N impiegato ed è considerata alta se tale quantitativo supera 125 kg/ha)
        Dim Dose As Integer = If(Qta_N <= 125, 1, 2)

        Dim objEfficienzaxTipiAllevamentixEffluenti As New AgronicaCoreMetaSchemaDAL.EfficienzaxTipiAllevamentixEffluenti_R
        Dim dt = objEfficienzaxTipiAllevamentixEffluenti.Leggi(0, MomentoDistribuzioneEpoca, 0, ConcimeOrganico, Dose, Regolamento_Cod, 0, objParametri)

        If dt.Rows.Count > 0 Then
            Dim EffluentiXFrequenza = dt.Rows(0)
            Efficienza = EffluentiXFrequenza.Item("Efficienza_Val")
        End If


        Return Efficienza

    End Function

#End Region

End Class

#Region "INPUT"
Public Class PianoConcimazioneBilancio_input

    Public Regolamento_Cod As Integer
    Public Regione_Cod As String 'istat

    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Resa As Decimal
    Public FaseCicloColturale_Cod As Integer

    Public AnticipazioniAnni As Integer

    Public Argilla As Decimal
    Public Sabbia As Decimal
    Public NTot As Decimal
    Public SO As Decimal
    Public CN As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public Mg As Decimal
    Public CSC As Decimal
    Public Caco3 As Decimal 'calcare totale

    Public DisponibilitaOssigeno_Cod As Integer

    Public PioggiaMM As Decimal
    Public PioggiaMM_Febbraio As Decimal

    Public Precessione_Veg_Cod As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Tipo As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Frequenza As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Qta As Decimal

    Public ColturaProtetta As Boolean
    Public Ubicazione_Cod As Integer
    Public FissazioneN_Perc As Decimal

    Sub New()

        Regione_Cod = "08" 'default ER

        Veg_Cod = 0

        PioggiaMM = 0
        PioggiaMM_Febbraio = 0

        AnticipazioniAnni = 0
        Grfi_Cod = 9 'produzione

        DisponibilitaOssigeno_Cod = 1 'imperfetta

        Precessione_Veg_Cod = 21 'non definita
        FertilizzanteOrganico_ColturePrecedenti_Tipo = 4 'nessuno
        FertilizzanteOrganico_ColturePrecedenti_Frequenza = 4 'saltuario
        FertilizzanteOrganico_ColturePrecedenti_Qta = 0

        ColturaProtetta = False
        Ubicazione_Cod = 1 'Pianura limitrofa a zone urbanizzate
        FissazioneN_Perc = 0

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
        Me.Veg_Cod = Dt_Piano.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")
        Me.Grfi_Cod = Dt_Piano.Rows(0).Item("PC_Dettagli_Finalita_GRFI_COD")
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Resa")) Then
            Me.Resa = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Resa"))
        Else
            Me.Resa = 0
        End If
        Me.FaseCicloColturale_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase"))

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Anticipazioni_Anni")) Then
            Me.AnticipazioniAnni = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Anticipazioni_Anni"))
        Else
            Me.AnticipazioniAnni = 0
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
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_So")) Then
            Me.SO = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_So"))
        Else
            Me.SO = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_CN")) Then
            Me.CN = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_CN"))
        Else
            Me.CN = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_p2o5")) Then
            Me.P2O5 = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_p2o5"))
        Else
            Me.P2O5 = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_k2o")) Then
            Me.K2O = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_k2o"))
        Else
            Me.K2O = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Mg")) Then
            Me.Mg = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Mg"))
        Else
            Me.Mg = 0
        End If
        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_CSC")) Then
            Me.CSC = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_CSC"))
        Else
            Me.CSC = 0
        End If

        'Disp_oss,pioggiaMM,Prec_veg,Fer_Org_tipo,Fer_Org_freq,Fer_Org_qta,Cult_Pro,Ubic_Cod,FissN_Prec

        Me.ColturaProtetta = IIf(Dt_Piano.Rows(0).Item("PC_Dettagli_Copertura") = 1, True, False)
        Me.Ubicazione_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Ubicazione_Cod"))

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_PercFissazioneN")) Then
            Me.FissazioneN_Perc = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_PercFissazioneN"))
        Else
            Me.FissazioneN_Perc = 0
        End If

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Caco3")) Then
            Me.Caco3 = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Caco3"))
        Else
            Me.Caco3 = 0
        End If

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Condizionidelterreno_id_terreno")) Then
            Me.DisponibilitaOssigeno_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Condizionidelterreno_id_terreno"))
        Else
            Me.DisponibilitaOssigeno_Cod = 0
        End If

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita")) Then
            Me.PioggiaMM = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita"))
        Else
            Me.PioggiaMM = 0
        End If

        If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita_Febbraio")) Then
            Me.PioggiaMM_Febbraio = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita_Febbraio"))
        Else
            Me.PioggiaMM_Febbraio = 0
        End If


        Me.Precessione_Veg_Cod = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod"))

        Me.FertilizzanteOrganico_ColturePrecedenti_Tipo = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer"))
        Me.FertilizzanteOrganico_ColturePrecedenti_Frequenza = CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Frequenza"))

        If IsNumeric(Dt_Piano.Rows(0).Item("PC_Dettagli_Quantita")) Then
            Me.FertilizzanteOrganico_ColturePrecedenti_Qta = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Quantita"))
        Else
            Me.FertilizzanteOrganico_ColturePrecedenti_Qta = 0
        End If

    End Sub

End Class


Public Class PianoConcimazioneBilancio_ApportiAmmessi_input

    Public Regolamento_Cod As Integer
    Public Regione_Cod As String 'istat

    Public CodiceSpecieCliente As String
    Public Finalita_Cod As Integer

    Public Resa As Decimal
    Public FaseCicloColturale_Cod As Integer

    Public AnticipazioniAnni As Integer

    Public Argilla As Decimal
    Public Sabbia As Decimal
    Public NTot As Decimal
    Public SO As Decimal
    Public CN As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public Mg As Decimal
    Public CSC As Decimal
    Public Caco3 As Decimal 'calcare totale
    Public CalcAtt As Decimal 'calcare attivo
    Public Ph As Decimal

    Public Lat As Decimal?
    Public Lng As Decimal?

    Public DisponibilitaOssigeno_Cod As Integer

    Public PioggiaMM As Decimal
    Public PioggiaMM_Febbraio As Decimal

    Public PrecessioneSpecie_Cod As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Tipo As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Frequenza As Integer
    Public FertilizzanteOrganico_ColturePrecedenti_Qta As Decimal

    Public ColturaProtetta As Boolean
    Public Ubicazione_Cod As Integer
    Public FissazioneN_Perc As Decimal

    Sub New()

        Regione_Cod = "08" 'default ER

        PioggiaMM = 0
        PioggiaMM_Febbraio = 0

        AnticipazioniAnni = 0
        Finalita_Cod = 9 'produzione

        DisponibilitaOssigeno_Cod = 1 'imperfetta

        PrecessioneSpecie_Cod = 21 'non definita
        FertilizzanteOrganico_ColturePrecedenti_Tipo = 4 'nessuno
        FertilizzanteOrganico_ColturePrecedenti_Frequenza = 4 'saltuario
        FertilizzanteOrganico_ColturePrecedenti_Qta = 0

        ColturaProtetta = False
        Ubicazione_Cod = 1 'Pianura limitrofa a zone urbanizzate
        FissazioneN_Perc = 0

        Ph = 0
        CalcAtt = 0

        Lat = Nothing
        Lng = Nothing

    End Sub


End Class

#End Region

#Region "OUTPUT"
Public Class PianoConcimazioneBilancio_output

    Public N_Ammesso As Decimal
    Public P_Ammesso As Decimal
    Public K_Ammesso As Decimal

    Public N_Calcolato As Decimal
    Public P_Calcolato As Decimal
    Public K_Calcolato As Decimal

    Public N_Necessario As Decimal
    Public P_Necessario As Decimal
    Public K_Necessario As Decimal

    Public N_Disponibile As Decimal
    Public P_Disponibile As Decimal
    Public K_Disponibile As Decimal

    Public Limite_Mas As Decimal
    Public Resa_Rif As Decimal
    Public FattoreCorrettivo_N As Decimal

    Public MessaggioErrore As String

    Public ListaNecessita As List(Of Necessita)
    Public ListaDisponibilita As List(Of Disponibilita)

    Public Sub New()

        N_Ammesso = 0
        P_Ammesso = 0
        K_Ammesso = 0

        N_Calcolato = 0
        P_Calcolato = 0
        K_Calcolato = 0

        N_Necessario = 0
        P_Necessario = 0
        K_Necessario = 0

        N_Disponibile = 0
        P_Disponibile = 0
        K_Disponibile = 0

        Limite_Mas = 0

        MessaggioErrore = ""

        ListaNecessita = New List(Of Necessita)
        ListaDisponibilita = New List(Of Disponibilita)


    End Sub

End Class

Public Class Necessita

    Public Descrizione As String
    Public Valore_N As Decimal
    Public Valore_P As Decimal
    Public Valore_K As Decimal

    Sub New()

        Descrizione = ""
        Valore_N = 0
        Valore_P = 0
        Valore_K = 0

    End Sub

End Class

Public Class Disponibilita

    Public Descrizione As String
    Public Valore_N As Decimal
    Public Valore_P As Decimal
    Public Valore_K As Decimal

    Sub New()

        Descrizione = ""
        Valore_N = 0
        Valore_P = 0
        Valore_K = 0

    End Sub

End Class

Public Class PianoConcimazioneBilancio_ApportiAmmessi_output

    Public N_Ammesso As Decimal
    Public P_Ammesso As Decimal
    Public K_Ammesso As Decimal

    Public Limite_Mas As Decimal

    Public MessaggioErrore As String

    Public Sub New()

        N_Ammesso = 0
        P_Ammesso = 0
        K_Ammesso = 0

        Limite_Mas = 0

        MessaggioErrore = ""

    End Sub
End Class

#End Region


#Region "PIANO CONCIMAZIONE IBF"

Public Class Valori_Tabellati
    '------------------------
    '   COLTURA PRINCIPALE
    '------------------------
    Public Property COL3_NPercSS_AsportazioniProduzioneUtile_ColturaPrincipale As Decimal   'TABELLA CoefficenteAssorbimento 
    Public Property COL8_NPercSS_AsportazioneResidui_ColturaPrincipale As Decimal  'TABELLA CoefficenteAssorbimento 

    Public Property COL13_UmiditaRaccolta_AsportazioneResidui_ColturaPrincipale As Decimal 'TABELLA DatiColture_Raccolta
    Public Property COL14_HarvestIndex_AsportazioneResidui_ColturaPrincipale As Decimal 'TABELLA DatiColture_Raccolta
    Public Property COL15_K1_CoefficienteIsoumico_ColturaPrincipale As Decimal 'TABELLA DatiColture_Raccolta

    '------------------------
    '   PRECESSIONE
    '------------------------
    Public Property COL3_NPercSS_AsportazioniProduzioneUtile_Precessione As Decimal   'TABELLA CoefficenteAssorbimento 
    Public Property COL8_NPercSS_AsportazioneResidui_Precessione As Decimal  'TABELLA CoefficenteAssorbimento 


    Public Property COL13_UmiditaRaccolta_AsportazioneResidui_Precessione As Decimal 'TABELLA DatiColture_Raccolta
    Public Property COL14_HarvestIndex_AsportazioneResidui_Precessione As Decimal 'TABELLA DatiColture_Raccolta
    Public Property COL15_K1_CoefficienteIsoumico_Precessione As Decimal 'TABELLA DatiColture_Raccolta


    Public Property COL5_PercSS As Decimal   'TODO FEDE
    Public Property COL6_NPercSS As Decimal   'TODO FEDE
    Public Property COL9_QuotaNDisponibileAnnoCorrente_ALTA As Decimal   'TODO FEDE
    Public Property COL10_QuotaNDisponibileAnnoCorrente_MEDIA As Decimal   'TODO FEDE
    Public Property COL11_QuotaNDisponibileAnnoCorrente_BASSA As Decimal   'TODO FEDE
    Public Property COL12_QuotaNDisponibileAnnoPrecedente_RESIDUALE As Decimal   'TODO FEDE
    Public Property EfficienzaMomentoDistribuzione As Decimal   'TODO FEDE


End Class

Public Class Valori_Calcolati

    '----------------
    '   FABBISOGNO
    '----------------
    Public Property COLU_FabbisognoColturaPrincipale_KgNHa As Decimal
    Public Property COLS_ResaUtilePrevistaSS_THa As Decimal
    Public Property COLT_ResaResiduiAereiPrevista_THa As Decimal


    '----------------
    '   ENTRATE
    '----------------
    Public Property COLAH_NCOConcimazioneOrganica_KgNHa As Decimal
    Public Property COLAI_MineralizzazioneSO_KgNHa As Decimal
    Public Property COLAJ_Precessione_KgNHa As Decimal
    Public Property COLAG_EfficienzaAzoto As Decimal
    Public Property COLAB_NNellaSO_Perc As Decimal
    Public Property COLM_BiomassaTotaleAereaPrecessioneSS_THa As Decimal
    Public Property COLAA_TAvgPeriodoRiferimento As Decimal
    Public Property COLY_PeriodoRiferimentoMineralizzazioneSO_Mesi As Integer
    Public Property COLZ_PeriodoRiferimentoPrecessione_Mesi As Integer
    Public Property COLN_ResiduiLasciatiDaPrecessioneSS_THa As Decimal

    '----------------
    '   USCITE
    '----------------
    Public Property COLAM_NLisciviazione_KgNHa As Decimal
    Public Property COLAN_NVolatilizzazione_KgNHa As Decimal
    Public Property COLAO_NPercolazione_KgNHa As Decimal
    Public Property COLAL_NPrecessione_2802_KgNHa As Decimal
    Public Property COLAK_NMineralizzazione_2802_KgNHa As Decimal


    '----------------
    '   DOSE FINALE ELEMENTO
    '----------------
    Public Property COLAR_NDistibuzione_KgHa As Decimal

End Class

Public Class PianoConcimazioneBilancio_IBF_INPUT
    Public Regolamento_Cod As Integer
    Public Regione_Cod As String 'istat

    Public COLE_ColturaPrincipaleAnno As Integer
    Public Grfi_Cod_ColturaPrincipaleAnno As Integer

    '-------------------------
    '   FABBISOGNO
    '-------------------------
    Public COLP_ResaUtilePrevistaColturaPrincipale_THa As Decimal
    Public COLQ_PercUmiditaPrevistaColturaPrincipale As Decimal

    '-------------------------
    '   ENTRATE / USCITE
    '-------------------------
    Public COLH_PrecessioneAnnoPrecedente As Integer
    Public Grfi_Cod_PrecessioneAnnoPrecedente As Integer

    Public COLAD_TipoConcimeOrganico As Integer
    Public N_KgHa As Decimal 'TODO FEDE

    Public COLI_ResiduiPrecessioneAsportati As Boolean
    Public COLJ_ResaStoricaPrecessione_THa As Decimal

    Public COLX_PeriodoRaccoltaColturaPrincipale As Integer
    Public COLW_PeriodoSeminaColturaPrincipale As Integer
    Public COLV_PeriodoInterramentoResiduiPrecessione As Integer

    Public COLAF_ModalitaDistribuzioneRelativoColturaEpoca As Integer

    Public COLAP_NDistribuito_KgNHa As Decimal

    Public COLK_PercUmiditaRaccoltaPrecessione As Decimal

    Public Frequenza As Integer 'Corrisponde ad 'Anno Precedente' in COLAB_ModalitaDistribuzioneRelativoColturaEpoca

    '----------------
    '   DOSE FINALE ELEMENTO
    '----------------
    Public COLAQ_SeminaInterrataRisaia As Boolean 'TODO FEDE

    Public SoilDataset_I_SabbiaPerc As Decimal
    Public SoilDataset_K_ArgillaPerc As Decimal
    Public SoilDataset_J_LimoPerc As Decimal
    Public SoilDataset_M_BulkDensityTM As Decimal
    Public SoilDataset_T_NPerc As Decimal
    Public SoilDataset_S_SOPerc As Decimal
    Public SoilDataset_N_pH As Decimal
    Public SoilDataset_Z_CSC As Decimal

    Public PiovositaOttobreFebbraio As Decimal
    Public AVG_Temperatura_ColturaInCampo As Decimal
    Public AVG_Temperatura_MeseSemina_Febbraio As Decimal


    Sub New()

        Regione_Cod = "08" 'default ER

    End Sub
End Class

#End Region