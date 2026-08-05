Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class PortinnestixSpecieVegetali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Veg_Cod As Integer, _
                            ByVal Port_Cod As Integer, _
                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.PortinnestixSpecieVegetali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Append(" SELECT Veg_Cod, Port_Cod ")
                    StrSQL.Append(" FROM  PortinnestixSpecieVegetali " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Port_Cod <> 0 Then
                        StrSQL.Append(" AND Port_Cod = " & Agro_SQL_SaveNum(Port_Cod) + vbCrLf)
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  PortinnestixSpecieVegetali " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

                    If Port_Cod <> "" Then
                        StrSQL.Append(" AND Port_Cod = " & Agro_SQL_SaveNum(Port_Cod) + vbCrLf)
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                    '=====================================================================

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  PortinnestixSpecieVegetali , SpecieVegetali , Portinnesti ")
                    StrSQL.Append(" WHERE PortinnestixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   PortinnestixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Portinnesti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Portinnesti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   PortinnestixSpecieVegetali.PORT_COD = Portinnesti.PORT_COD ")
                    StrSQL.Append(" AND   PortinnestixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")

                    If Port_Cod <> 0 Then
                        StrSQL.Append(" AND PortinnestixSpecieVegetali.PORT_COD =  " & Agro_SQL_SaveNum(Port_Cod) & "  ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND PortinnestixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   SpecieVegetali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   SpecieVegetali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, Portinnesti.PORT_DES ASC ")
                    End If


                    '=====================================================================

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


    '##############################################################################################
    Public Function LeggiPortinnesti(ByVal Veg_Cod As Integer, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.PortinnestixSpecieVegetali_R.LeggiPortinnesti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

             

            StrSQL.Append(" SELECT p.Port_Cod, p.Port_Des ")
            StrSQL.Append(" FROM  Portinnesti p " + vbCrLf)
            StrSQL.Append(" inner join PortinnestixSpecieVegetali PS on p.Port_Cod = ps.Port_Cod  " + vbCrLf)
            StrSQL.Append(" WHERE 1=1" & vbCrLf)

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " + vbCrLf)
            End If

            StrSQL.Append(" order by p.Port_Des  " + vbCrLf)
             

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




    '###################################################################################
    Public Function Esiste_VegCodXPortCod(ByVal Veg_Cod As Integer, _
                                            ByVal Port_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.PortinnestixSpecieVegetali_R.Esiste_VegCodXPortCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risp As Boolean = False

        Try

            DT = Leggi(Veg_Cod, _
                        Port_Cod, _
                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                risp = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function



    '##############################################################################################
    Public Function PortDes_from_PortCod(ByVal PortCod As Integer, _
                                         ByVal VegCod As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As String

        Dim DT As DataTable


        DT = Leggi(CInt(VegCod), _
                        CInt(PortCod), _
                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                             "", _
                             "", _
                             objParametri)

        If DT.Rows.Count > 0 Then
            Return DT.Rows(0).Item("port_des")
        End If


    End Function



End Class




Public Class Portinnesti
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Port_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Portinnesti.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0



            StrSQL.Append(" SELECT * " + vbCrLf)
            StrSQL.Append(" FROM  Portinnesti " + vbCrLf)
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

            If Port_Cod <> 0 Then
                StrSQL.Append(" AND Port_Cod = " & Agro_SQL_SaveNum(Port_Cod) + vbCrLf)
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '###################################################################################
    Public Function Descrizione(ByVal Port_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.portinnesto.Descrizione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim risp As String = ""

        Try

            DT = Leggi(Port_Cod, _
                                                "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count = 1 Then
                risp = DT.Rows(0).Item("Port_Des")
            Else
                Throw New Exception("Port_Cod non esiste")
            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return risp

    End Function




End Class