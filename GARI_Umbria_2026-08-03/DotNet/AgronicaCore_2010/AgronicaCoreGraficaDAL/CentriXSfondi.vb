Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class CentriXSfondi_Write
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal SfondoCod As Integer, _
                            ByVal iXNO As Decimal, _
                            ByVal iYNO As Decimal, _
                            ByVal iXSE As Decimal, _
                            ByVal iYSE As Decimal, _
                            ByVal oXNO As Decimal, _
                            ByVal oYNO As Decimal, _
                            ByVal oXSE As Decimal, _
                            ByVal oYSE As Decimal, _
                            ByVal LatMin As Decimal, _
                            ByVal LngMin As Decimal, _
                            ByVal LatMax As Decimal, _
                            ByVal LngMax As Decimal, _
                            ByVal FileBitmap As String, _
                            ByVal PathBitmap As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CentriXSfondi_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

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

            StrSQL.Append("INSERT INTO CentriXSfondi(Piva,Sa_Cod,SfondoCod,iXNO,iYNO,iXSE,iYSE,oXNO,oYNO,oXSE,oYSE,")
            StrSQL.Append(" LatMin,LngMin, LatMax, LngMax,")
            StrSQL.Append("FileBitmap,PathBitmap,Inviato,DataInvio,Data_Creazione,Data_Modifica,UserName_Creazione,UserName_Modifica,Validita_Inizio,Validita_Fine) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SfondoCod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(iXNO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(iYNO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(iXSE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(iYSE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(oXNO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(oYNO) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(oXSE) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(oYSE) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(LatMin) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LngMin) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LatMax) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LngMax) & "  ")

            StrSQL.Append("         , '" & Agro_SQL_SaveText(Trim(FileBitmap)) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Trim(PathBitmap)) & "' ")
            StrSQL.Append("         , 0, Null  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")


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



    '##############################################################################################
    Public Function ModificaLatLngMin( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal SfondoCod As Integer, _
                            ByVal LatMin As Decimal, _
                            ByVal LngMin As Decimal, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CentriXSfondi_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append(" Update CentriXSfondi ")
            StrSQL.Append(" Set LatMin = " & Agro_SQL_SaveNum(LatMin) & " , LngMin = " & Agro_SQL_SaveNum(LngMin))
            StrSQL.Append(" WHERE 1 = 1 ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append(" AND SfondoCod = " & Agro_SQL_SaveNum(SfondoCod))

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

    '##############################################################################################
    Public Function ModificaLatLngMax( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal SfondoCod As Integer, _
                            ByVal LatMax As Decimal, _
                            ByVal LngMax As Decimal, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CentriXSfondi_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append(" Update CentriXSfondi ")
            StrSQL.Append(" Set LatMax = " & Agro_SQL_SaveNum(LatMax) & " ,LngMax = " & Agro_SQL_SaveNum(LngMax))
            StrSQL.Append(" WHERE 1 = 1 ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append(" AND SfondoCod = " & Agro_SQL_SaveNum(SfondoCod))


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
    '##############################################################################################
    Public Function ModificaFileNamePath( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal SfondoCod As Integer, _
                            ByVal FileBitmap As String, _
                            ByVal PathBitmap As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CentriXSfondi_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append(" Update CentriXSfondi ")
            StrSQL.Append(" Set FileBitmap = '" & Agro_SQL_SaveText(FileBitmap) & "' ,PathBitmap = '" & Agro_SQL_SaveNum(PathBitmap) & "'")
            StrSQL.Append(" WHERE 1 = 1 ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.Append(" AND SfondoCod = " & Agro_SQL_SaveNum(SfondoCod))

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



    Public Function Cancella( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal SfondoCod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.CentriXSfondi_Write.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE CentriXSfondi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= '" & Agro_SQL_SaveText(Date.Now) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If
                If SfondoCod <> 0 Then
                    StrSQL.Append(" AND     SfondoCod = " & Agro_SQL_SaveNum(SfondoCod) & " ")
                End If
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    CentriXSfondi ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If
                If SfondoCod <> 0 Then
                    StrSQL.Append(" AND     SfondoCod = " & Agro_SQL_SaveNum(SfondoCod) & " ")
                End If
                StrSQL.Append(" AND     Inviato >= 0 ")

            End If
            '---------------------------------------------

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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class CentriXSfondi_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function LeggiXML( _
                          ByVal PIVA As String, _
                          ByVal Sa_Cod As Integer, _
                          ByVal SfondoCod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.CentriXSfondi_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim rval As String

        Try
            LeggiDammiQuery(PIVA, Sa_Cod, SfondoCod, xSelezioneVariabile, xFiltroAggiuntivo, xOrderBy, objParametri, True, StrSQL)

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Lettura_XML(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return rval

    End Function


    '#############################################################################################
    Public Function Leggi( _
                          ByVal PIVA As String, _
                          ByVal Sa_Cod As Integer, _
                          ByVal SfondoCod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal LeggiXmlDaSQLServer As Boolean = False _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.CentriXSfondi_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            LeggiDammiQuery(PIVA, Sa_Cod, SfondoCod, xSelezioneVariabile, xFiltroAggiuntivo, xOrderBy, objParametri, LeggiXmlDaSQLServer, StrSQL)
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
    Private Sub LeggiDammiQuery(ByVal PIVA As String, ByVal Sa_Cod As Integer, ByVal SfondoCod As Integer, ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal LeggiXmlDaSQLServer As Boolean, ByRef StrSQL As System.Text.StringBuilder)
        Select Case xSelezioneVariabile

            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                '
                '
                '
                '

            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                '---------------------------------------------
                StrSQL.Length = 0

                If LeggiXmlDaSQLServer Then
                    StrSQL.Append(" SELECT  ")
                    StrSQL.Append(" 0 as TipoOperazioneDB " & vbCrLf)
                    StrSQL.Append(" , PIVA " & vbCrLf)
                    StrSQL.Append(" , sa_cod " & vbCrLf)
                    StrSQL.Append(" , sfondocod " & vbCrLf)
                    StrSQL.Append(" , ixno " & vbCrLf)
                    StrSQL.Append(" , iyno " & vbCrLf)
                    StrSQL.Append(" , ixse " & vbCrLf)
                    StrSQL.Append(" , iyse " & vbCrLf)
                    StrSQL.Append(" , oxno " & vbCrLf)
                    StrSQL.Append(" , oyno " & vbCrLf)
                    StrSQL.Append(" , oxse " & vbCrLf)
                    StrSQL.Append(" , oyse " & vbCrLf)

                    StrSQL.Append(" , LatMin " & vbCrLf)
                    StrSQL.Append(" , LngMin " & vbCrLf)
                    StrSQL.Append(" , LatMax " & vbCrLf)
                    StrSQL.Append(" , LngMax " & vbCrLf)

                    StrSQL.Append(" , filebitmap " & vbCrLf)
                    StrSQL.Append(" , pathbitmap " & vbCrLf)
                    StrSQL.Append(" , inviato " & vbCrLf)
                    StrSQL.Append(" , datainvio " & vbCrLf)
                    StrSQL.Append(" , data_creazione " & vbCrLf)
                    StrSQL.Append(" , data_modifica " & vbCrLf)
                    StrSQL.Append(" , username_creazione " & vbCrLf)
                    StrSQL.Append(" , username_modifica " & vbCrLf)
                    StrSQL.Append(" , validita_inizio " & vbCrLf)
                    StrSQL.Append(" , validita_fine " & vbCrLf)
                Else
                    StrSQL.Append(" SELECT CentriXSfondi.* ")
                End If


                StrSQL.Append(" FROM  CentriXSfondi ")
                StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                If PIVA <> "" Then

                    StrSQL.Append(" AND   Piva='" & Agro_SQL_SaveText(PIVA) & "'")

                    If Sa_Cod Then

                        StrSQL.Append(" AND   Sa_cod=" & Sa_Cod)

                    End If

                End If

                If SfondoCod <> 0 Then

                    StrSQL.Append(" AND SfondoCod=" & SfondoCod)

                End If


                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------
                Select Case objParametri.FlagVisibilita
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                        StrSQL.Append(" AND   CentriXSfondi.Inviato >=0 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                        StrSQL.Append(" AND   CentriXSfondi.Inviato =-1 ")
                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select
                '--------------------------------------------------------------------------
                If xOrderBy <> "" Then
                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If LeggiXmlDaSQLServer Then
            StrSQL.Append(" for xml auto,root('ListaSfondi') ")
        End If
    End Sub
End Class