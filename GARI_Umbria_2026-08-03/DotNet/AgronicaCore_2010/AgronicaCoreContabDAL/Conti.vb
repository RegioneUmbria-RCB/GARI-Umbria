
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider







'leggi


Public Class Conti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal PIVA As String, _
                      ByVal Cod_Conto As Integer, _
                      ByVal Cod_Contatto As String, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Conti_R.Leggi"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Conti ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")



                    If PIVA <> "" Then
                        StrSQL.Append(" AND ( Piva = '" & Agro_SQL_SaveText(PIVA) & "' Or Piva = 'AAAAAAAAAAA' ) ")
                    End If

                    If Cod_Conto <> 0 Then

                        StrSQL.Append(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")

                    End If

                    If Cod_Contatto <> "" Then

                        StrSQL.Append(" AND Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   ")
                    End If



                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Conti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Conti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva ASC, Cod_Conto ASC, Conto_Descr ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni




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


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Conti_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Scrivi(ByVal PIVA As String, _
                ByVal Cod_Conto As Int32, _
                ByVal Conto_Descr As String, _
                ByVal Flag_UE As Integer, _
                ByVal Cod_Contatto As String, _
                ByVal Extra_Str As String, _
                ByVal Extra_Int As Long, _
                ByVal Extra_Date As Date, _
                    ByVal Validita_Inizio As Date, _
                    ByVal Validita_Fine As Date, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean



        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Scrivi_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Append(" INSERT INTO Conti ")
            StrSQL.Append(" (Piva, Cod_Conto, Conto_Descr, Flag_UE, Cod_Contatto, Extra_Str, Extra_Int, Extra_Date, ")

            StrSQL.Append("                   Inviato,            DataInvio, ")
            StrSQL.Append("                Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append(" ) ")

            StrSQL.Append("VALUES ( '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append("    , " & Agro_SQL_SaveNum(Cod_Conto))
            StrSQL.Append("   ,'" & Agro_SQL_SaveText(Conto_Descr) & "' ")
            StrSQL.Append("   ," & Agro_SQL_SaveNum(Flag_UE))
            StrSQL.Append("   ,'" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            StrSQL.Append("   ,'" & Agro_SQL_SaveText(Extra_Str) & "' ")
            StrSQL.Append("   ," & Agro_SQL_SaveNum(Extra_Int))
            StrSQL.Append("   ," & Agro_SQL_SaveDate(Extra_Date))
            StrSQL.Append("        , 0  ")
            StrSQL.Append("       , Null  ")
            StrSQL.Append("        , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("       , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("      ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("       ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("      , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("     , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(" )  ")

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


    Public Function Modifica(ByVal Cod_Conto As Integer, _
                    ByVal Conto_Descr As String, _
                    ByVal Flag_UE As Integer, _
                    ByVal Cod_Contatto As String, _
                    ByVal Extra_Str As String, _
                    ByVal Extra_Int As Long, _
                    ByVal Extra_Date As Date, _
                    ByVal Validita_Inizio As Date, _
                    ByVal Validita_Fine As Date, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Conti_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Append(" UPDATE Conti SET ")
            StrSQL.Append("    Conto_Descr       = '" & Agro_SQL_SaveText(Conto_Descr) & "'  ")
            StrSQL.Append("  ,Flag_UE           =  " & Agro_SQL_SaveNum(Flag_UE) & "   ")
            StrSQL.Append("   ,Cod_Contatto      = '" & Agro_SQL_SaveText(Cod_Contatto) & "'  ")
            StrSQL.Append("   ,Extra_Str         = '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            StrSQL.Append("   ,Extra_Int         =  " & Agro_SQL_SaveNum(Extra_Int) & "   ")
            StrSQL.Append("   ,Extra_Date        =  " & Agro_SQL_SaveDate(Extra_Date) & "  ")

            StrSQL.Append("   ,Inviato           = 0 ")
            StrSQL.Append("   ,DataInvio         = Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("  ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.Append("  ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("  ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  WHERE Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "    ")




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




    Public Function Cancella(ByVal PIVA As String, _
                             ByVal Cod_Conto As Integer, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Conti_W.Cancella()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Append(" UPDATE  Conti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Inviato > 0 ")
            Else



                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM  Conti ")
                StrSQL.Append(" WHERE Inviato = 0 ")


            End If



            If PIVA <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Cod_Conto <> 0 Then
                StrSQL.Append(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
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


End Class
