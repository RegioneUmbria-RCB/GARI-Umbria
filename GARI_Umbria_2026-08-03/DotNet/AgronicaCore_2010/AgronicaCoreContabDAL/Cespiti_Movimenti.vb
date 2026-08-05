

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Cespiti_Movimenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                      Optional ByVal IdCodCespite As Long = 0, _
                      Optional ByVal xFiltroAggiuntivo As String = "", _
                      Optional ByVal xOrderBy As String = "", _
                      Optional ByVal ChiamataDaGiasLan As Boolean = False, _
                      Optional ByRef strSQLOutput As String = "") As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " + vbCrLf)
            Stb.Append(" FROM Cespiti_Movimenti " + vbCrLf)

            Stb.Append(" WHERE 1=1 " + vbCrLf)

            If IdCodCespite <> 0 Then
                Stb.Append("  AND id_cod_cespite = " & Agro_SQL_SaveNum(IdCodCespite) & " ")

            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            If ChiamataDaGiasLan Then
                strSQLOutput = Stb.ToString
            Else
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            End If    '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT



    End Function


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Cespiti_Movimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
              ByVal id_cod_cespite As Integer, _
              ByVal dat_mov As Date, _
              ByVal cau_mov As Integer, _
              ByVal esercizio As Integer, _
              Optional ByVal qta_N_mov As Integer = 0, _
              Optional ByVal val_qtaN_costo_acq As Double = 0, _
              Optional ByVal val_qta1_costo_acq As Double = 0, _
              Optional ByVal val_qta1_mov As Double = 0, _
              Optional ByVal val_qtaN_mov As Double = 0, _
              Optional ByVal val_qta1_amm As Double = 0, _
              Optional ByVal val_qtaN_amm As Double = 0, _
              Optional ByVal prc_amm As Double = 0, _
              Optional ByVal val_qta1_fondo_amm As Double = 0, _
              Optional ByVal val_qtaN_fondo_amm As Double = 0, _
              Optional ByVal val_qta1_residuo_amm As Double = 0, _
              Optional ByVal val_qtaN_residuo_amm As Double = 0, _
              Optional ByVal val_qta1_minus As Double = 0, _
              Optional ByVal val_qtaN_minus As Double = 0, _
              Optional ByVal val_qta1_plus As Double = 0, _
              Optional ByVal val_qtaN_plus As Double = 0, _
              Optional ByVal fis_val_qta1_ammortizzabile As Double = 0, _
              Optional ByVal fis_val_qtaN_ammortizzabile As Double = 0, _
              Optional ByVal fis_val_qta1_mov As Double = 0, _
              Optional ByVal fis_val_qtaN_mov As Double = 0, _
              Optional ByVal fis_val_qta1_amm As Double = 0, _
              Optional ByVal fis_val_qtaN_amm As Double = 0, _
              Optional ByVal fis_prc_amm As Double = 0, _
              Optional ByVal fis_val_qta1_fondo_amm As Double = 0, _
              Optional ByVal fis_val_qtaN_fondo_amm As Double = 0, _
              Optional ByVal fis_val_qta1_residuo_amm As Double = 0, _
              Optional ByVal fis_val_qtaN_residuo_amm As Double = 0, _
              Optional ByVal fis_val_qta1_minus As Double = 0, _
              Optional ByVal fis_val_qtaN_minus As Double = 0, _
              Optional ByVal fis_val_qta1_plus As Double = 0, _
              Optional ByVal fis_val_qtaN_plus As Double = 0, _
                 Optional ByVal Data_creazione As Date = #2/1/1900#, _
                 Optional ByVal Data_modifica As Date = #2/1/1900#, _
                 Optional ByVal username_creazione As String = "", _
                 Optional ByVal username_modifica As String = "" _
            ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0

            Stb.Append(" INSERT Cespiti_Movimenti " + vbCrLf)

            Stb.Append("              (")


            '-- Dati generali
            Stb.Append("   id_cod_cespite, " & vbCrLf)
            Stb.Append("   dat_mov, " & vbCrLf)
            Stb.Append("   cau_mov, " & vbCrLf)
            Stb.Append("   esercizio, " & vbCrLf)
            '-- Dati movimento
            Stb.Append("  qta_N_mov, " & vbCrLf)
            Stb.Append("  val_qtaN_costo_acq, " & vbCrLf)
            Stb.Append("  val_qta1_costo_acq, " & vbCrLf)

            Stb.Append("  val_qta1_mov,  " & vbCrLf)
            Stb.Append("  val_qtaN_mov,  " & vbCrLf)
            Stb.Append("  val_qta1_amm,  " & vbCrLf)
            Stb.Append("  val_qtaN_amm,  " & vbCrLf)
            Stb.Append("  prc_amm,  " & vbCrLf)
            Stb.Append("  val_qta1_fondo_amm, " & vbCrLf)
            Stb.Append("  val_qtaN_fondo_amm, " & vbCrLf)
            Stb.Append("  val_qta1_residuo_amm, " & vbCrLf)
            Stb.Append("  val_qtaN_residuo_amm, " & vbCrLf)
            Stb.Append("  val_qta1_minus, " & vbCrLf)
            Stb.Append("  val_qtaN_minus, " & vbCrLf)
            Stb.Append("  val_qta1_plus, " & vbCrLf)
            Stb.Append("  val_qtaN_plus, " & vbCrLf)

            Stb.Append("  fis_val_qta1_ammortizzabile,  " & vbCrLf)
            Stb.Append("  fis_val_qtaN_ammortizzabile,  " & vbCrLf)
            Stb.Append("  fis_val_qta1_mov,  " & vbCrLf)
            Stb.Append("  fis_val_qtaN_mov,  " & vbCrLf)
            Stb.Append("  fis_val_qta1_amm,  " & vbCrLf)
            Stb.Append("  fis_val_qtaN_amm,  " & vbCrLf)
            Stb.Append("  fis_prc_amm,  " & vbCrLf)
            Stb.Append("  fis_val_qta1_fondo_amm, " & vbCrLf)
            Stb.Append("  fis_val_qtaN_fondo_amm, " & vbCrLf)
            Stb.Append("  fis_val_qta1_residuo_amm, " & vbCrLf)
            Stb.Append("  fis_val_qtaN_residuo_amm, " & vbCrLf)
            Stb.Append("  fis_val_qta1_minus, " & vbCrLf)
            Stb.Append("  fis_val_qtaN_minus, " & vbCrLf)
            Stb.Append("  fis_val_qta1_plus, " & vbCrLf)
            Stb.Append("  fis_val_qtaN_plus, " & vbCrLf)

            Stb.Append("              Inviato,            datainvio, " & vbCrLf)
            Stb.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
            Stb.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
            Stb.Append("              Validita_Inizio,    Validita_Fine " & vbCrLf)
            Stb.Append("              ) " & vbCrLf)

            Stb.Append(" VALUES ( " & vbCrLf)

            Stb.Append(" " & Agro_SQL_SaveNum(id_cod_cespite) & ", " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveDate(dat_mov) & ", " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(cau_mov) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(esercizio) & ", " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(qta_N_mov) & ", " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_costo_acq) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_costo_acq) & " , " & vbCrLf)

            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_mov) & " ," & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_mov) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(prc_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_fondo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_fondo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_residuo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_residuo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_minus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_minus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qta1_plus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(val_qtaN_plus) & " , " & vbCrLf)

            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_ammortizzabile) & " ," & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_ammortizzabile) & " ," & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_mov) & " ," & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_mov) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_prc_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_fondo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_fondo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_residuo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_residuo_amm) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_minus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_minus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qta1_plus) & " , " & vbCrLf)
            Stb.Append(" " & Agro_SQL_SaveNum(fis_val_qtaN_plus) & " , " & vbCrLf)


            Stb.Append("          0 , ")
            Stb.Append("          Null , ")
            Stb.Append("          " & Agro_SQL_SaveDate(Date.Now) & " , ")
            Stb.Append("          " & Agro_SQL_SaveDate(Date.Now) & " , ")
            Stb.Append("         '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,")
            Stb.Append("         '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ,")
            Stb.Append("          " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " , ")
            Stb.Append("          " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '#################################################################

    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             Optional ByVal IdCodCespite As Long = 0, _
                             Optional ByVal CauMov As Integer = 0, _
                             Optional ByVal Esercizio As Integer = 0, _
                             Optional ByVal xFiltroAggiuntivo As String = "" _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE Cespiti_Movimenti ")
                Stb.Append(" SET ... ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else

                Stb.Append(" DELETE FROM Cespiti_Movimenti " + vbCrLf)
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If IdCodCespite <> 0 Then
                Stb.Append(" AND id_cod_cespite = " & Agro_SQL_SaveNum(IdCodCespite) & " ")
            End If
            If CauMov <> 0 Then
                Stb.Append(" AND cau_mov = " & Agro_SQL_SaveNum(CauMov) & " ")
            End If
            If Esercizio <> 0 Then
                Stb.Append(" AND esercizio = " & Agro_SQL_SaveNum(Esercizio) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class





