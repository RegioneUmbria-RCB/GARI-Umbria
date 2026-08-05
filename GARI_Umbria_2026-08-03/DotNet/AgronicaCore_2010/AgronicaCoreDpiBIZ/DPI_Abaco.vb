Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DPI_Abaco

    Public Function Abaco_Discplinari_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Disciplinari"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISCIPLINARE As Integer = 0
        Dim COD_REGIONE As Integer
        Dim DECO_REGIONE As String
        Dim COD_COLT_DISC As Integer
        Dim DECO_COLT_DISC As String
        Dim COD_COLT As Integer
        Dim DECO_COLT As String
        Dim COD_AVV_DISC As String
        Dim DECO_AVV_DISC As String
        Dim COD_AVV As Integer
        Dim DECO_AVV As String
        Dim COD_SA1 As Integer
        Dim DECO_SA1 As String
        Dim COD_SA2 As Integer
        Dim COD_SA3 As Integer
        Dim ANNO As Integer
        Dim NOTE As String

        Dim DFR_COD As Integer = 0
        Dim ID_PAA_MISC As Integer = 0

        Try

            'Cancellazione
            ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

            DT = ObjDPI_R.LeggixAbaco_Disciplinari(objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows

                    DFR_COD = dr("DFR_COD")

                    ID_DISCIPLINARE = dr("ID_PAA")
                    COD_REGIONE = dr("Regione_Cod")
                    DECO_REGIONE = dr("Regione")
                    COD_COLT_DISC = dr("Id_RCDPI")

                    DECO_COLT_DISC = dr("Nome")
                    If Len(DECO_COLT_DISC) > 50 Then
                        DECO_COLT_DISC = Left(DECO_COLT_DISC, 50)
                    End If

                    'If CInt(dr("Grfi_Cod")) <> 0 Then
                    '    DECO_COLT_DISC = DECO_COLT_DISC & " (" & dr("Grfi_Des") & ")"
                    'End If

                    COD_COLT = dr("Veg_Cod")
                    DECO_COLT = dr("Veg_Des")

                    If dr("Avversita_Cod") <> 0 Then
                        COD_AVV_DISC = 0
                        DECO_AVV_DISC = ""
                        COD_AVV = dr("Avversita_Cod")
                        DECO_AVV = dr("Avversita_Des_Lat")
                    ElseIf dr("Gruppo_Avversita_Cod") <> 0 Then
                        COD_AVV_DISC = dr("Gruppo_Avversita_Cod")
                        DECO_AVV_DISC = dr("Gruppo_Avversita_Des")
                        COD_AVV = 0
                        DECO_AVV = ""
                    Else
                        COD_AVV_DISC = 0
                        DECO_AVV_DISC = ""
                        COD_AVV = 0
                        DECO_AVV = ""
                    End If

                    ANNO = dr("Anno")
                    NOTE = dr("DescrizioneInfestanti").ToString


                    ObjDPI_W.Scrivi_Abaco_Disciplinari(ID_DISCIPLINARE,
                                                           COD_REGIONE,
                                                           DECO_REGIONE,
                                                           COD_COLT_DISC,
                                                           DECO_COLT_DISC,
                                                           COD_COLT,
                                                           DECO_COLT,
                                                           COD_AVV_DISC,
                                                           DECO_AVV_DISC,
                                                           COD_AVV,
                                                           DECO_AVV,
                                                           dr("Pa_Cod"),
                                                           dr("Pa_Des"),
                                                           0,
                                                           "",
                                                           0,
                                                           "",
                                                           ANNO,
                                                           NOTE,
                                                           objParametri_Matrice)



                    If dr("ID_PAA_MISC") <> 0 And ID_PAA_MISC <> dr("ID_PAA_MISC") Then

                        COD_SA1 = dr("Pa_Cod")
                        DECO_SA1 = dr("Pa_Des")
                        COD_SA2 = 0
                        COD_SA3 = 0

                        ID_PAA_MISC = dr("ID_PAA_MISC")
                        'Oltre a salvare il singolo principio attivo salvo la miscela
                        ID_DISCIPLINARE = ID_PAA_MISC

                        ObjDPI_W.Scrivi_Abaco_Disciplinari(ID_DISCIPLINARE,
                                                      COD_REGIONE,
                                                      DECO_REGIONE,
                                                      COD_COLT_DISC,
                                                      DECO_COLT_DISC,
                                                      COD_COLT,
                                                      DECO_COLT,
                                                      COD_AVV_DISC,
                                                      DECO_AVV_DISC,
                                                      COD_AVV,
                                                      DECO_AVV,
                                                      COD_SA1,
                                                      DECO_SA1,
                                                      0,
                                                      "",
                                                      0,
                                                      "",
                                                      ANNO,
                                                      NOTE,
                                                      objParametri_Matrice)

                    End If

                    If ID_PAA_MISC <> 0 And ID_PAA_MISC = dr("ID_PAA_MISC") Then

                        If COD_SA2 = 0 And COD_SA1 <> dr("Pa_Cod") Then
                            COD_SA2 = dr("Pa_Cod")
                            ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "COD_SA2", dr("Pa_Cod"), "Id_Disciplinare = " & ID_PAA_MISC, objParametri_Matrice)
                            ObjDPI_W.Modifica_CampoString(Suffisso_Tabella, "DECO_SA2", dr("Pa_Des"), "Id_Disciplinare = " & ID_PAA_MISC, objParametri_Matrice)
                        ElseIf COD_SA3 = 0 And COD_SA1 <> dr("Pa_Cod") And COD_SA2 <> dr("Pa_Cod") Then
                            COD_SA3 = dr("Pa_Cod")
                            ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "COD_SA3", dr("Pa_Cod"), "Id_Disciplinare = " & ID_PAA_MISC, objParametri_Matrice)
                            ObjDPI_W.Modifica_CampoString(Suffisso_Tabella, "DECO_SA3", dr("Pa_Des"), "Id_Disciplinare = " & ID_PAA_MISC, objParametri_Matrice)
                        Else
                            'Non più slot disponibili
                        End If

                    End If

                Next

                'Cancella Duplicati
                ObjDPI_W.Cancella_Duplicati_Abaco_Disciplinari(objParametri_Matrice)

            End If

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function



    Public Function Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_DOSE_MAX"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_DOSE_MAX As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        Dim CODICE_PA1 As Integer
        Dim CODICE_PA2 As Integer
        Dim CODICE_PA3 As Integer
        Dim DOSE As Integer
        Dim COD_UM_DOSE As Integer
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim PERC_SA As Decimal
        Dim DOSE_GL As Integer
        Dim Codice As Integer
        Dim CodRegione As Integer

        Dim DFR_COD As Integer = 0
        Dim ID_PAA_MISC As Integer = 0
        Dim objdpi As New AgronicaCoreDpiDAL.Dpi_R
        Dim DtMiscela As DataTable
        Dim bOk As Boolean
        Try

            'Cancellazione
            ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_DOSE_MAX(objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows

                    'Controllo variazione di riga o mi miscela
                    If DFR_COD <> dr("DFR_COD") Or (ID_PAA_MISC <> dr("ID_PAA_MISC") Or dr("ID_PAA_MISC") = 0) Then

                        DFR_COD = dr("DFR_COD")
                        ID_PAA_MISC = dr("ID_PAA_MISC")

                        COD_REGIONE = dr("Regione_Cod")
                        COD_COLT_DISC = dr("Id_RCDPI")
                        CODICE_PA1 = dr("Pa_Cod")
                        CODICE_PA2 = 0
                        CODICE_PA3 = 0

                        NOTE = dr("DescrizioneInfestanti").ToString
                        ANNO = dr("Anno")
                        DATA_INS = Date.Now
                        DATA_UPD = Date.Now

                        Select Case ID_PAA_MISC

                            Case 0

                                ID_DISC_DOSE_MAX = dr("ID_PAA")
                                DOSE = dr("Dose_Max_Anno")
                                COD_UM_DOSE = dr("Dose_UDM")
                                PERC_SA = dr("PercPa")
                                DOSE_GL = dr("PesoPa")
                                bOk = True

                            Case Else

                                'Lettura della Miscela
                                bOk = False
                                DtMiscela = objdpi.Leggi_PA_Ausiliari(ID_PAA_MISC, dr("DFT_Cod"), dr("DFR_Cod"), 0, "", "", objParametri_Matrice)

                                If DtMiscela.Rows.Count > 0 Then

                                    If DtMiscela.Rows(0)("Dose_Max_Anno2") <> 0 Then
                                        bOk = True
                                        ID_DISC_DOSE_MAX = dr("ID_PAA_MISC")
                                        DOSE = DtMiscela.Rows(0)("Dose_Max_Anno2")
                                        COD_UM_DOSE = DtMiscela.Rows(0)("Dose_UDM2")
                                        PERC_SA = DtMiscela.Rows(0)("PercPa2")
                                        DOSE_GL = DtMiscela.Rows(0)("PesoPa2")
                                    End If

                                End If


                        End Select

                        Codice = dr("Id_RCDPI")
                        CodRegione = dr("Regione_Cod")

                        If bOk Then

                            ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX(ID_DISC_DOSE_MAX,
                                                                               COD_REGIONE,
                                                                               COD_COLT_DISC,
                                                                               CODICE_PA1,
                                                                               CODICE_PA2,
                                                                               CODICE_PA3,
                                                                               DOSE,
                                                                               COD_UM_DOSE,
                                                                               NOTE,
                                                                               ANNO,
                                                                               DATA_INS,
                                                                               DATA_UPD,
                                                                               PERC_SA,
                                                                               DOSE_GL,
                                                                               Codice,
                                                                               CodRegione,
                                                                               objParametri_Matrice)

                        End If

                    ElseIf bOk And ID_PAA_MISC = dr("ID_PAA_MISC") Then

                        If CODICE_PA2 = 0 And CODICE_PA2 <> dr("Pa_Cod") Then
                            CODICE_PA2 = dr("Pa_Cod")
                            ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "CODICE_PA2", dr("Pa_Cod"), "ID_DISC_DOSE_MAX = " & ID_DISC_DOSE_MAX, objParametri_Matrice)
                        ElseIf CODICE_PA3 = 0 And CODICE_PA1 <> dr("Pa_Cod") And CODICE_PA2 <> dr("Pa_Cod") Then
                            CODICE_PA3 = dr("Pa_Cod")
                            ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "CODICE_PA3", dr("Pa_Cod"), "ID_DISC_DOSE_MAX = " & ID_DISC_DOSE_MAX, objParametri_Matrice)
                        Else
                            'Non più slot disponibili
                        End If

                    End If


                Next

                'Cancella Duplicati
                ObjDPI_W.Cancella_Duplicati_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX(objParametri_Matrice)



            End If


            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function






    Public Function Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_N_MAX_TRATT_X_AVV As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        Dim COD_AVVERSITA As String
        Dim NUM_MAX_TRATT As Integer
        Dim MAX_TRATT_PER As String
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim Codice As Integer
        Dim CodRegione As Integer

        Dim Tipo As String

        Try

            'Cancellazione
            ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

            For i = 1 To 2

                Tipo = IIf(i = 1, "A", "C")
                DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV(Tipo, objParametri_Matrice, Filtro_Aggiuntivo)

                If DT.Rows.Count > 0 Then

                    For Each dr In DT.Rows

                        ID_DISC_N_MAX_TRATT_X_AVV = dr("Chiave")
                        COD_REGIONE = dr("Regione_Cod")
                        COD_COLT_DISC = dr("Id_RCDPI")
                        COD_AVVERSITA = dr("Avversita_Cod")

                        NUM_MAX_TRATT = dr("NmaxTratt")
                        MAX_TRATT_PER = Tipo
                        NOTE = ""
                        ANNO = dr("Anno")
                        DATA_INS = Date.Now
                        DATA_UPD = Date.Now

                        Codice = dr("Id_RCDPI")
                        CodRegione = dr("Regione_Cod")

                        ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV(ID_DISC_N_MAX_TRATT_X_AVV,
                                                                                          COD_REGIONE,
                                                                                          COD_COLT_DISC,
                                                                                          COD_AVVERSITA,
                                                                                          NUM_MAX_TRATT,
                                                                                          MAX_TRATT_PER,
                                                                                          NOTE,
                                                                                          ANNO,
                                                                                          DATA_INS,
                                                                                          DATA_UPD,
                                                                                          Codice,
                                                                                          CodRegione,
                                                                                          objParametri_Matrice)



                    Next

                End If

            Next

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function


    Public Function Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        Dim ID_GRUPPO_AVV_DISC As Integer
        Dim NUM_MAX_TRATT As Integer
        Dim MAX_TRATT_PER As String
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim Codice As Integer
        Dim CodRegione As Integer

        Dim Tipo As String

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        For i = 1 To 2

            Tipo = IIf(i = 1, "A", "C")
            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV(Tipo, objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows

                    ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV = dr("Chiave")
                    COD_REGIONE = dr("Regione_Cod")
                    COD_COLT_DISC = dr("Id_RCDPI")
                    ID_GRUPPO_AVV_DISC = dr("Gruppo_Avversita_Cod")

                    NUM_MAX_TRATT = dr("NmaxTratt")
                    MAX_TRATT_PER = Tipo
                    NOTE = ""
                    ANNO = dr("Anno")
                    DATA_INS = Date.Now
                    DATA_UPD = Date.Now

                    Codice = dr("Id_RCDPI")
                    CodRegione = dr("Regione_Cod")

                    ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV(ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV,
                                                                                             COD_REGIONE,
                                                                                             COD_COLT_DISC,
                                                                                             ID_GRUPPO_AVV_DISC,
                                                                                             NUM_MAX_TRATT,
                                                                                             MAX_TRATT_PER,
                                                                                             NOTE,
                                                                                             ANNO,
                                                                                             DATA_INS,
                                                                                             DATA_UPD,
                                                                                             Codice,
                                                                                             CodRegione,
                                                                                             objParametri_Matrice)

                Next

            End If

        Next

        Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function


    Public Function Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_N_MAX_TRATT_X_GRUPPO_SA As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        Dim ID_GRUPPO_SA_DISC As Integer
        Dim NUM_MAX_TRATT As Integer
        Dim MAX_TRATT_PER As String
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim Codice As Integer
        Dim CodRegione As Integer

        Dim Tipo As String

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        For i = 1 To 2

            Tipo = IIf(i = 1, "A", "C")
            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA(Tipo, objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows

                    ID_DISC_N_MAX_TRATT_X_GRUPPO_SA = dr("ID_DISC_N_MAX_TRATT_X_GRUPPO_SA")
                    COD_REGIONE = dr("Regione_Cod")
                    COD_COLT_DISC = dr("Id_RCDPI")
                    ID_GRUPPO_SA_DISC = dr("GPAI_PA_Ausiliari_COD")
                    NUM_MAX_TRATT = dr("NmaxTratt")
                    MAX_TRATT_PER = Tipo
                    NOTE = ""
                    ANNO = dr("Anno")
                    DATA_INS = Date.Now
                    DATA_UPD = Date.Now

                    Codice = dr("Id_RCDPI")
                    CodRegione = dr("Regione_Cod")

                    ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA(ID_DISC_N_MAX_TRATT_X_GRUPPO_SA,
                                                                                        COD_REGIONE,
                                                                                        COD_COLT_DISC,
                                                                                        ID_GRUPPO_SA_DISC,
                                                                                        NUM_MAX_TRATT,
                                                                                        MAX_TRATT_PER,
                                                                                        NOTE,
                                                                                        ANNO,
                                                                                        DATA_INS,
                                                                                        DATA_UPD,
                                                                                        Codice,
                                                                                        CodRegione,
                                                                                        objParametri_Matrice)



                Next


            End If

        Next

        Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function


    Public Function Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_N_MAX_TRATT_X_SA As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        'Dim ID_GRUPPO_SA_DISC As Integer
        Dim NUM_MAX_TRATT As Integer
        Dim MAX_TRATT_PER As String
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim CODICE_PA1 As Integer
        Dim CODICE_PA2 As Integer
        Dim CODICE_PA3 As Integer
        Dim CodRegione As Integer

        Dim Tipo As String

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        For i = 1 To 2

            Tipo = IIf(i = 1, "A", "C")
            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA(Tipo, objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows

                    ID_DISC_N_MAX_TRATT_X_SA = dr("ID_DISC_N_MAX_TRATT_X_SA")
                    COD_REGIONE = dr("Regione_Cod")
                    COD_COLT_DISC = dr("Id_RCDPI")
                    NUM_MAX_TRATT = dr("NmaxTratt")
                    MAX_TRATT_PER = Tipo
                    CODICE_PA1 = dr("Codice")
                    CODICE_PA2 = 0
                    CODICE_PA3 = 0
                    NOTE = ""
                    ANNO = dr("Anno")
                    DATA_INS = Date.Now
                    DATA_UPD = Date.Now
                    CodRegione = dr("Regione_Cod")

                    ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA(ID_DISC_N_MAX_TRATT_X_SA,
                                                                                     COD_REGIONE,
                                                                                     COD_COLT_DISC,
                                                                                     NUM_MAX_TRATT,
                                                                                     MAX_TRATT_PER,
                                                                                     NOTE,
                                                                                     ANNO,
                                                                                     DATA_INS,
                                                                                     DATA_UPD,
                                                                                     CODICE_PA1,
                                                                                     CODICE_PA2,
                                                                                     CODICE_PA3,
                                                                                     CodRegione,
                                                                                     objParametri_Matrice)



                Next


            End If

        Next

        Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function



    Public Function Abaco_Vincoli_Disciplinari_DISC_SCADENZA_SA_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_DISC_SCADENZA_SA"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim DTAnno As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjRegolamenti_R As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_DISC_SCADENZA_SA As Integer = 0
        Dim COD_REGIONE As Integer
        Dim COD_COLT_DISC As Integer
        Dim CODICE_PA1 As Integer
        Dim CODICE_PA2 As Integer
        Dim CODICE_PA3 As Integer
        Dim SCADENZA As DateTime
        Dim NOTE As String
        Dim ANNO As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime
        Dim Codice As Integer
        Dim CodRegione As Integer
        Dim bOk As Boolean = False

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        'Lettura dei regolamenti
        DTAnno = ObjRegolamenti_R.Leggi(0, 1, Filtro_Aggiuntivo, "NomeEsteso", objParametri_Matrice)

        If DTAnno.Rows.Count <> 0 Then

            For Each drAnno In DTAnno.Rows

                For i = 1 To 2

                    ID_DISC_SCADENZA_SA = 0

                    Select Case i
                        Case 1
                            'No Miscela
                            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_SCADENZA_SA(drAnno("Cod_Regolamento"), drAnno("Anno"), objParametri_Matrice)

                        Case Else
                            'Miscele
                            DT = ObjDPI_R.LeggixVincoli_Disciplinari_DISC_SCADENZA_SA_MISCELE(drAnno("Cod_Regolamento"), drAnno("Anno"), objParametri_Matrice)
                    End Select


                    If DT.Rows.Count > 0 Then

                        For Each dr In DT.Rows

                            If ID_DISC_SCADENZA_SA <> dr("ID_DISC_SCADENZA_SA") Then

                                ID_DISC_SCADENZA_SA = dr("ID_DISC_SCADENZA_SA")
                                COD_REGIONE = dr("Regione_Cod")
                                COD_COLT_DISC = dr("Id_RCDPI")
                                CODICE_PA1 = dr("Pa_Cod")
                                CODICE_PA2 = 0
                                CODICE_PA3 = 0
                                bOk = False

                                If IsDate(dr("Data_Fine_UsoScorte")) Then

                                    'If CDate(dr("Data_Fine_UsoScorte")) >= CDate("01/01/" & drAnno("Anno")) AndAlso CDate(dr("Data_Fine_UsoScorte")) <= CDate("31/12/" & drAnno("Anno")) Then
                                    SCADENZA = dr("Data_Fine_UsoScorte")
                                    bOk = True
                                    'End If
                                End If

                                If Not bOk AndAlso IsDate(dr("Data_Revo")) Then  ' AndAlso CDate(dr("Data_Revo")) >= CDate("01/01/" & drAnno("Anno")) AndAlso CDate(dr("Data_Revo")) <= CDate("31/12/" & drAnno("Anno")) Then
                                    SCADENZA = dr("Data_Revo")
                                    bOk = True
                                End If


                                NOTE = ""
                                ANNO = dr("Anno")
                                DATA_INS = Date.Now
                                DATA_UPD = Date.Now

                                Codice = dr("Id_RCDPI")
                                CodRegione = dr("Regione_Cod")

                                If bOk Then

                                    ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_SCADENZA_SA(ID_DISC_SCADENZA_SA,
                                                                                        COD_REGIONE,
                                                                                       COD_COLT_DISC,
                                                                                       CODICE_PA1,
                                                                                       CODICE_PA2,
                                                                                       CODICE_PA3,
                                                                                       SCADENZA,
                                                                                       NOTE,
                                                                                       ANNO,
                                                                                       DATA_INS,
                                                                                       DATA_UPD,
                                                                                       Codice,
                                                                                       CodRegione,
                                                                                       objParametri_Matrice)


                                End If


                            Else

                                If CODICE_PA2 = 0 And CODICE_PA2 <> dr("Pa_Cod") Then
                                    CODICE_PA2 = dr("Pa_Cod")
                                    ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "CODICE_PA2", dr("Pa_Cod"), "ID_DISC_SCADENZA_SA = " & ID_DISC_SCADENZA_SA, objParametri_Matrice)
                                ElseIf CODICE_PA3 = 0 And CODICE_PA1 <> dr("Pa_Cod") And CODICE_PA2 <> dr("Pa_Cod") Then
                                    CODICE_PA3 = dr("Pa_Cod")
                                    ObjDPI_W.Modifica_CampoInteger(Suffisso_Tabella, "CODICE_PA2", dr("Pa_Cod"), "ID_DISC_SCADENZA_SA = " & ID_DISC_SCADENZA_SA, objParametri_Matrice)

                                Else
                                    'Non più slot disponibili
                                End If

                            End If


                        Next

                    End If


                Next

            Next


        End If

        Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function



    Public Function Abaco_Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_GRUPPO_AVV_DISC As Integer = 0
        Dim CODICE_AVV_COLT As String
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        DT = ObjDPI_R.LeggixVincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT(objParametri_Matrice, Filtro_Aggiuntivo)

        If DT.Rows.Count > 0 Then

            For Each dr In DT.Rows

                ID_GRUPPO_AVV_DISC = dr("Gruppo_Avversita_Cod")
                CODICE_AVV_COLT = dr("Avversita_Cod")
                DATA_INS = Date.Now
                DATA_UPD = Date.Now

                ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT(ID_GRUPPO_AVV_DISC,
                                                                                        CODICE_AVV_COLT,
                                                                                        DATA_INS,
                                                                                        DATA_UPD,
                                                                                        objParametri_Matrice)

            Next

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        End If

        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function



    Public Function Abaco_Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_GRUPPO_AVV_DISC As Integer = 0
        Dim COD_COLT_DISC As Integer
        Dim NOME As String
        Dim DESCRIZIONE As String
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime


        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        DT = ObjDPI_R.LeggixVincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI(objParametri_Matrice, Filtro_Aggiuntivo)

        If DT.Rows.Count > 0 Then

            For Each dr In DT.Rows

                ID_GRUPPO_AVV_DISC = dr("Gruppo_Avversita_Cod")
                COD_COLT_DISC = dr("Id_RCDPI")
                NOME = dr("Gruppo_Avversita_Des")
                DESCRIZIONE = dr("Gruppo_Avversita_Des_Lat")

                DATA_INS = Date.Now
                DATA_UPD = Date.Now

                ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI(ID_GRUPPO_AVV_DISC,
                                                                                         COD_COLT_DISC,
                                                                                         NOME,
                                                                                         DESCRIZIONE,
                                                                                         DATA_INS,
                                                                                         DATA_UPD,
                                                                                         objParametri_Matrice)

            Next

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        End If



        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function







    Public Function Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_GRUPPO_SA_DISC As Integer = 0
        Dim CODICE_PA As Integer
        Dim CODICE_PA2 As Integer
        Dim CODICE_PA3 As Integer
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        DT = ObjDPI_R.LeggixVincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT(objParametri_Matrice, Filtro_Aggiuntivo)

        If DT.Rows.Count > 0 Then

            For Each dr In DT.Rows

                ID_GRUPPO_SA_DISC = dr("GPAI_PA_Ausiliari_COD")
                CODICE_PA = dr("Pa_Cod")
                DATA_INS = Date.Now
                DATA_UPD = Date.Now

                ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT(ID_GRUPPO_SA_DISC,
                                                                                        CODICE_PA,
                                                                                        CODICE_PA2,
                                                                                        CODICE_PA3,
                                                                                        DATA_INS,
                                                                                        DATA_UPD,
                                                                                        objParametri_Matrice)

            Next

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        End If



        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function







    Public Function Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim ID_GRUPPO_SA_DISC As Integer = 0
        Dim NOME As String
        Dim DESCRIZIONE As String
        Dim DATA_INS As DateTime
        Dim DATA_UPD As DateTime

        'Dim Arrayp As String()

        'Cancellazione
        ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

        DT = ObjDPI_R.LeggixVincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI(objParametri_Matrice, Filtro_Aggiuntivo)

        If DT.Rows.Count > 0 Then

            For Each dr In DT.Rows

                ID_GRUPPO_SA_DISC = dr("GPAI_PA_Ausiliari_COD")

                'If InStr(dr("GPAI_PA_Ausiliari_DES"), "_") > 0 Then

                'Arrayp = Split(dr("GPAI_PA_Ausiliari_DES"), "_")
                'NOME = Arrayp(0)
                'If Len(NOME) > 50 Then
                ' NOME = Left(NOME, 50)
                'End If
                '   DESCRIZIONE = Replace(dr("GPAI_PA_Ausiliari_DES"), NOME, "")
                '  DESCRIZIONE = Right(DESCRIZIONE, Len(DESCRIZIONE) - 1)

                'Else

                NOME = Trim(Left(dr("GPAI_PA_Ausiliari_DES") & Space(50), 50))
                DESCRIZIONE = dr("GPAI_PA_Ausiliari_DES")

                'End If

                DATA_INS = Date.Now
                DATA_UPD = Date.Now


                ObjDPI_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI(ID_GRUPPO_SA_DISC,
                                                                                  NOME,
                                                                                  DESCRIZIONE,
                                                                                  DATA_INS,
                                                                                  DATA_UPD,
                                                                                  objParametri_Matrice)

            Next

            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)

        End If



        Try
        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function




    Public Function Abaco_UMDosi_Riempi(ByVal Filtro_Aggiuntivo As String, ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String

        Dim Suffisso_Tabella As String = "UMDosi"
        Dim Log_Errori As String = ""
        Dim DT As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R
        Dim ObjDPI_W As New AgronicaCoreDpiDAL.Dpi_Abaco_W

        Dim CODICE As Integer
        Dim DECODIFICA As String

        Try

            'Cancellazione
            ObjDPI_W.Cancella(Suffisso_Tabella, "", objParametri_Matrice)

            DT = ObjDPI_R.LeggixUMDosi(objParametri_Matrice, Filtro_Aggiuntivo)

            If DT.Rows.Count > 0 Then

                For Each dr In DT.Rows


                    CODICE = dr("Dose_UDM")
                    DECODIFICA = dr("Udm_Sim")

                    ObjDPI_W.Scrivi_Abaco_UMDosi(CODICE,
                                                 DECODIFICA,
                                                 objParametri_Matrice)

                Next


            End If


            Export_Excel(Suffisso_Tabella, objParametri_Matrice, Cartella_Export)


        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabella Abaco_:" & Suffisso_Tabella & "." & Chr(13) &
                   ex.Message
        End Try

        Return Log_Errori

    End Function

    Public Function Abaco_Tabelle_Utility_Export(ByRef objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String) As String
        Dim Log_Errori As String = ""

        Try

            Export_Excel_Utility("Lista_Regioni", "Reg as Codice, *, Regione_Des as Decodifica, '' as Sigla", objParametri_Matrice, Cartella_Export)
            Export_Excel_Utility("SpecieVegetali", "*", objParametri_Matrice, Cartella_Export)
            Export_Excel_Utility("Avversita", "*", objParametri_Matrice, Cartella_Export)
            Export_Excel_Utility("PrincipiAttivi", "*", objParametri_Matrice, Cartella_Export)

        Catch ex As Exception
            Log_Errori = "Si è verificato un errore durante il popolamento della tabelle utility." & Chr(13) &
                   ex.Message

        End Try

        Return Log_Errori

    End Function

    Private Sub Export_Excel(ByVal Suffisso_Tabella As String, objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String)

        Dim Dt As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R

        '===========================================================================================================================
        'Export Excel
        '---------------------------------------------------------------------------------------------------------------------------
        Dt = ObjDPI_R.LeggiTabellaAbacoDPI(Suffisso_Tabella, objParametri_Matrice)

        'percorso e nome del file da esportare
        Dim prefissoNomeFile = "Abaco_" & Suffisso_Tabella
        Dim nomeFileUnivoco As String = prefissoNomeFile & ".xlsx"
        Dim path_file As String = Cartella_Export & "\" & nomeFileUnivoco

        If Len(prefissoNomeFile) > 30 Then
            prefissoNomeFile = Left(prefissoNomeFile, 30)
        End If

        AgronicaCoreGestioneRichieste.Esporta.EsportaExcelAbacoDPI(Dt, path_file, prefissoNomeFile)
        '===========================================================================================================================

    End Sub

    Private Sub Export_Excel_Utility(ByVal Tabella As String, ByVal SelectList As String, objParametri_Matrice As AgronicaCoreParametri, ByVal Cartella_Export As String)

        Dim Dt As DataTable
        Dim ObjDPI_R As New AgronicaCoreDpiDAL.Dpi_Abaco_R

        '===========================================================================================================================
        'Export Excel
        '---------------------------------------------------------------------------------------------------------------------------
        Dt = ObjDPI_R.LeggiTabellaAbacoUtility(Tabella, SelectList, objParametri_Matrice)

        'percorso e nome del file da esportare
        Dim prefissoNomeFile = Tabella
        Dim nomeFileUnivoco As String = prefissoNomeFile & ".xlsx"
        Dim path_file As String = Cartella_Export & "\" & nomeFileUnivoco

        If Len(prefissoNomeFile) > 30 Then
            prefissoNomeFile = Left(prefissoNomeFile, 30)
        End If

        AgronicaCoreGestioneRichieste.Esporta.EsportaExcelAbacoDPI(Dt, path_file, prefissoNomeFile)
        '===========================================================================================================================

    End Sub

End Class
