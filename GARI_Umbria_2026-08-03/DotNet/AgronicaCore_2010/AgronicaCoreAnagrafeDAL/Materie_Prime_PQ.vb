
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Materie_Prime_PQ_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#############################################################################
    Public Function Scrivi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Tipo As String, _
                            ByVal Tipo_Cod As Int32, _
                            ByVal Udm_Cod As Int32, _
                            ByVal Valore_Des As String, _
                            ByVal Valore_Min As Decimal, _
                            ByVal Valore_Max As Decimal, _
                            ByVal ChkRegistri As Int16, _
                            ByVal ChkCalibri As Int16, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_creazione As DateTime = #2/1/1900#, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Materie_Prime_ParametriQualitativi ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,    Sa_Cod,     Mat_Cod,    Tipo,       Tipo_Cod,         ")
            StrSQL.Append("          Udm_Cod, Valore_Des, Valore_Min, Valore_Max, ChkRegistri,      ")
            StrSQL.Append("          ChkCalibri, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(PIVA) & "'   ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod))
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Tipo) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Valore_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore_Min))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore_Max))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ChkRegistri))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ChkCalibri))

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '############################################################################
    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Tipo As String, _
                            ByVal Tipo_Cod As Int32, _
                            ByVal Udm_Cod As Int32, _
                            ByVal Valore_Des As String, _
                            ByVal Valore_Min As Decimal, _
                            ByVal Valore_Max As Decimal, _
                            ByVal ChkRegistri As Int16, _
                            ByVal ChkCalibri As Int16, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Id_Trasformazione = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Trasformazione obbligatorio)")
            'End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Materie_Prime_ParametriQualitativi SET ")
            StrSQL.Append("    Udm_Cod             =  " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("   ,Valore_Des          = '" & Agro_SQL_SaveText(Valore_Des) & "'  ")
            StrSQL.Append("   ,Valore_Min          =  " & Agro_SQL_SaveNum(Valore_Min) & "  ")
            StrSQL.Append("   ,Valore_Max          =  " & Agro_SQL_SaveNum(Valore_Max) & "  ")
            StrSQL.Append("   ,ChkRegistri         =  " & Agro_SQL_SaveNum(ChkRegistri) & "  ")
            StrSQL.Append("   ,ChkCalibri          =  " & Agro_SQL_SaveNum(ChkCalibri) & "  ")
            StrSQL.Append("   ,UserName_Modifica   = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("   ,Validita_Inizio     =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine       =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE   Piva           = '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")

            'If Sa_Cod <> 0 Then
            '      sSql = sSql & " AND Materie_Prime_ParametriQualitativi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   "
            'End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   ")
            End If

            If Tipo_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function


    '#####################################################################################
    Public Function Cancella( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Long, _
                                ByVal Mat_Cod As Long, _
                                ByVal Tipo As String, _
                                ByVal Tipo_Cod As Long, _
                                ByVal Udm_Cod As Long, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Materie_Prime_ParametriQualitativi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Materie_Prime_ParametriQualitativi ")
                StrSQL.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            'If Sa_Cod <> 0 Then
            '      sSql = sSql & " AND Materie_Prime_ParametriQualitativi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   "
            'End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Tipo <> "" Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   ")
            End If

            If Tipo_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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




Public Class Materie_Prime_PQ_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Voci_di_Riepilogo(ByVal PIVA As String, _
                                        ByVal Cal_Cod As Integer, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal Tipo_Cod As Integer, _
                                        ByVal ChkRegistri As Integer, _
                                        ByVal ChkRegistri_Vinificazione As Integer, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R.Voci_di_Riepilogo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        ' ByVal ChkCalibri As Integer, _

        Try

            StrSQL.Length = 0
            'ci vuole il distinct se no la voce di riepilogo viene ripetuta per tutti i mat_cod
            StrSQL.Append(" SELECT DISTINCT Piva_SuperUser,Cal_Cod,Cal_Des, Piva,Tipo,Tipo_Cod ")
            '            StrSQL.Append(" -- ChkRegistri,ChkRegistri_Vinificazione ,ChkCalibri  ")
            StrSQL.Append("  ")
            StrSQL.Append("  FROM Materie_Prime_ParametriQualitativi  ")
            StrSQL.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.tipo_cod = Materie_Prime_Calibri.Cal_Cod ")
            StrSQL.Append("  ")
            StrSQL.Append("  ")
            StrSQL.Append("  ")

            StrSQL.Append(" WHERE   Materie_Prime_Calibri.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")

            If Cal_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_Calibri.Cal_Cod = " & Agro_SQL_SaveText(Cal_Cod) & " ")
            End If

            If PIVA <> "" Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveText(Mat_Cod) & " ")
            End If

            If Tipo_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveText(Tipo_Cod) & " ")
            End If

            If ChkRegistri <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = " & Agro_SQL_SaveText(ChkRegistri) & " ")
            End If

            If ChkRegistri_Vinificazione <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = " & Agro_SQL_SaveText(ChkRegistri_Vinificazione) & " ")
            End If

            'If ChkCalibri <> 0 Then
            '    StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.ChkCalibri = " & Agro_SQL_SaveText(ChkCalibri) & " ")
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Materie_Prime_ParametriQualitativi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Materie_Prime_ParametriQualitativi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cal_des, cal_cod ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    '##############################################################################################
    Public Function LeggiMateriePrimeByVoceDiRiepilogo(ByVal Cal_Cod As Integer, _
                                                        ByVal Id_Report As Integer, _
                                                        ByVal ChkRegistri As Integer, _
                                                        ByVal ChkRegistri_Vinificazione As Integer, _
                                                        ByVal PIVA As String, _
                                                        ByVal Elem_Cod As Integer, _
                                                        ByVal Mat_Cod As Integer, _
                                                        ByVal Flag_Pubblico As Boolean, _
                                                        ByVal xFiltroAggiuntivo As String, _
                                                        ByVal xOrderBy As String, _
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R.LeggiMateriePrimeByVoceDiRiepilogo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        ' ByVal ChkCalibri As Integer, _

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Piva_SuperUser, Materie_Prime_Calibri.Cal_Cod,Cal_Des, ")
            StrSQL.Append(" Tipo,Tipo_Cod,ChkRegistri,ChkRegistri_Vinificazione ,ChkCalibri,Materie_Prime_ParametriQualitativi.Udm_Cod,Valore_Des,Valore_Min,Valore_Max, ")
            StrSQL.Append(" Materie_Prime.Piva, Materie_Prime.Sa_Cod, Elem_Cod, Materie_Prime.Mat_Cod, Cod_Articolo,Mat_Des ,Cul_Cod,Veg_Cod,GRVA_COD_VEG, ")
            StrSQL.Append(" Regolamento , Udm_Cod_Extra ,Flag_Extra,Qta_Extra,Colore ")
            StrSQL.Append("  ")
            StrSQL.Append("  ")
            StrSQL.Append("  ")
            StrSQL.Append(" FROM Materie_Prime_ParametriQualitativi  ")
            StrSQL.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.tipo_cod = Materie_Prime_Calibri.Cal_Cod ")
            StrSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime_ParametriQualitativi.mat_cod = Materie_Prime.mat_cod ")
            StrSQL.Append(" INNER JOIN Materie_PrimexReport ON Materie_PrimexReport.mat_cod = Materie_Prime.mat_cod ")
            StrSQL.Append("  ")

            StrSQL.Append(" WHERE   Materie_Prime_Calibri.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & " ")

            If Cal_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime_Calibri.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " ")
            End If

            If ChkRegistri <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = " & Agro_SQL_SaveNum(ChkRegistri) & " ")
            End If

            If ChkRegistri_Vinificazione <> 0 Then
                StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = " & Agro_SQL_SaveNum(ChkRegistri_Vinificazione) & " ")
            End If

            If Elem_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If

            If Flag_Pubblico = True Then
                StrSQL.Append(" AND (  Materie_Prime.Piva = '" & Agro_SQL_SaveText(PIVA) & "' OR Materie_Prime.Sa_Cod = -1 ) ")
            Else
                If PIVA <> "" Then
                    StrSQL.Append(" AND   Materie_Prime.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Materie_Prime_ParametriQualitativi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Materie_Prime_ParametriQualitativi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cal_des, mat_des ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function




    '============================================================================
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Tipo"></param>
    ''' <param name="Tipo_Cod"></param>
    ''' <param name="Udm_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	17/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Mat_Cod As Integer, _
                            ByVal Tipo As String, _
                            ByVal Tipo_Cod As Integer, _
                            ByVal Udm_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  dbo.Materie_Prime_ParametriQualitativi.* ")
                    StrSQL.Append(" FROM    dbo.Materie_Prime_ParametriQualitativi ")
                    StrSQL.Append(" WHERE   Materie_Prime_ParametriQualitativi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    StrSQL.Append(" AND (Materie_Prime_ParametriQualitativi.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Materie_Prime_ParametriQualitativi.Sa_Cod = -1)  ")


                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Tipo <> "" Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   ")
                    End If

                    If Tipo_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Materie_Prime_ParametriQualitativi.Piva ASC, Materie_Prime_ParametriQualitativi.Mat_Cod ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT

    End Function


    '============================================================================
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinDescrizioni
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Tipo_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	18/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Calibri( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal Mat_Cod As Integer, _
                                ByVal Tipo_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R.Leggi_Calibri()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Distinct dbo.Materie_Prime_ParametriQualitativi.*, CalibriFrutti.Cal_Des ")
                    StrSQL.Append(" FROM    dbo.Materie_Prime_ParametriQualitativi, CalibriFrutti ")
                    StrSQL.Append(" WHERE   Materie_Prime_ParametriQualitativi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Tipo = 'calibro'  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Tipo_Cod = CalibriFrutti.Cal_Cod  ")

                    StrSQL.Append(" AND (Materie_Prime_ParametriQualitativi.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Materie_Prime_ParametriQualitativi.Sa_Cod = -1)  ")

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Tipo_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.CalibriFrutti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.CalibriFrutti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Materie_Prime_ParametriQualitativi.Mat_Cod, CalibriFrutti.Cal_Des ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT

    End Function

    '============================================================================
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinDescrizioni
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Tipo_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	18/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Indici( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal Mat_Cod As Integer, _
                                ByVal Tipo_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R.Leggi_Calibri()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Distinct dbo.Materie_Prime_ParametriQualitativi.*, IndiciMaturita.Ind_Mat_Des ")
                    StrSQL.Append(" FROM    dbo.Materie_Prime_ParametriQualitativi, IndiciMaturita ")
                    StrSQL.Append(" WHERE   Materie_Prime_ParametriQualitativi.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Tipo = 'indice'  ")
                    StrSQL.Append(" AND     Materie_Prime_ParametriQualitativi.Tipo_Cod = IndiciMaturita.Ind_Mat_Cod  ")

                    StrSQL.Append(" AND (Materie_Prime_ParametriQualitativi.Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Materie_Prime_ParametriQualitativi.Sa_Cod = -1)  ")

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Tipo_Cod <> 0 Then
                        StrSQL.Append(" AND Materie_Prime_ParametriQualitativi.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.IndiciMaturita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.Materie_Prime_ParametriQualitativi.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.IndiciMaturita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Materie_Prime_ParametriQualitativi.Mat_Cod ASC, IndiciMaturita.Ind_Mat_Des ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT

    End Function

End Class
