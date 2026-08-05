
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CAC_Codifica_Veg_Cod_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###################################################################
    Public Function Leggi_Prodotti_SpecieVegetali(ByVal Veg_Cod_Coltiva As String,
  ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
  ByRef objParametri As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod.Leggi_Prodotti_SpecieVegetali()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT CAC.ID, CAC.Veg_Cod_Coltiva as Chiave, CAC.Veg_Cod_Coltiva_2, CAC.Data_Modifica AS Data_Modifica, CAC.Descrizione AS Descrizione,  ")
            StrSQL.AppendLine(" CAC.Veg_Cod_Coltiva, SpecieVegetali.Veg_Des AS veg_des, ")
            StrSQL.AppendLine(" (CAST(CAC.Veg_Cod_Gias AS varchar(20)) + '|' + CAST(CAC.id_cod AS varchar(20))) AS veg_cod, ")
            'StrSQL.AppendLine(" CAC.id_cod AS Id_Cod,  ")
            StrSQL.AppendLine(" ISNULL(CAC.grfi_cod,0) As grfi_cod, ISNULL(GruppoFinalita.Grfi_Des,'Non Specificato') As grfi_des, ")
            StrSQL.AppendLine(" ISNULL(CAC.Grva_Cod,0) AS Grva_Cod, ISNULL(GruppoVarietale.Grva_Des, 'Non Specificato') AS Grva_Des, ")
            StrSQL.AppendLine(" ISNULL(CAC.Foral_Cod, 0) AS Foral_Cod, ISNULL(FormeAllevamento.Foral_Des, 'Nessuno') AS Foral_Des, ")
            StrSQL.AppendLine(" ISNULL(CAC.Port_Cod,0) AS Port_Cod, ISNULL(Portinnesti.Port_Des, 'Non Specificato') AS Port_Des, ")
            StrSQL.AppendLine(" CAC.Reg_Cod AS Reg_Cod, Regolamenti.Reg_Des AS Reg_Des, ")
            StrSQL.AppendLine(" CAC.Tipo_Codifica, ")

            StrSQL.AppendLine(" ISNULL(CAC.Raggruppamento_Varietale_Cod,0) AS InfoAgg_Cod,  ")
            StrSQL.AppendLine(" ISNULL(CAC_Codifica_InfoAggiuntive.InfoAgg_Des,'Non Specificato') AS InfoAgg_Des, ")

            StrSQL.AppendLine(" CAC.Metodo_Produzione AS Metodo_Produzione_Cod,   ")
            StrSQL.AppendLine(" CASE CAC.Metodo_Produzione WHEN 1 THEN 'Integrato'  ")
            StrSQL.AppendLine("  WHEN 2 THEN 'In Conversione' ")
            StrSQL.AppendLine("  WHEN 3 THEN 'Biologico' ")
            StrSQL.AppendLine("  ELSE '' ")
            StrSQL.AppendLine("  END AS Metodo_Produzione_Des ")

            StrSQL.AppendLine("FROM CAC_Codifica_Veg_Cod CAC  ")

            'Join su SpecieVegetali per veg_des
            StrSQL.AppendLine("LEFT JOIN SpecieVegetali ")
            StrSQL.AppendLine("ON CAC.Veg_Cod_Gias = SpecieVegetali.veg_cod  ")

            'Join su GruppoFinalita per grifi_des
            StrSQL.AppendLine("LEFT JOIN GruppoFinalita ")
            StrSQL.AppendLine("ON CAC.Grfi_Cod = GruppoFinalita.grfi_cod ")

            'Join su GruppoVarietale per Grva_Des
            StrSQL.AppendLine("LEFT JOIN GruppoVarietale   ")
            StrSQL.AppendLine("ON CAC.Grva_Cod = GruppoVarietale.Grva_Cod   ")

            'Join su Codici_Anagrafe per descrizione id_cod
            StrSQL.AppendLine("LEFT JOIN Codici_Anagrafe   ")
            StrSQL.AppendLine("ON CAC.Id_Cod = Codici_Anagrafe.codice  ")

            'Join su FormeAllevamento per Foral_Des
            StrSQL.AppendLine("LEFT JOIN FormeAllevamento  ")
            StrSQL.AppendLine("ON CAC.Foral_Cod = FormeAllevamento.Foral_Cod ")

            'Join su Portinnesti per Port_Des
            StrSQL.AppendLine("LEFT JOIN Portinnesti ")
            StrSQL.AppendLine("ON CAC.Port_Cod = Portinnesti.Port_Cod  ")

            'Join su CAC_Codifica_InfoAggiuntive per InfoAgg_Des
            StrSQL.AppendLine("LEFT JOIN CAC_Codifica_InfoAggiuntive   ")
            StrSQL.AppendLine("ON CAC.Raggruppamento_Varietale_Cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod ")
            'StrSQL.AppendLine("WHERE argomento_cod = 2  ")

            'Join su Regolamenti per Reg_Des
            StrSQL.AppendLine("LEFT JOIN Regolamenti ")
            StrSQL.AppendLine("ON CAC.Reg_Cod = Regolamenti.Reg_Cod ")

            ''Join su [CAC_Codifica_ProdottiAziendali] per Codifica
            'StrSQL.AppendLine("LEFT JOIN CAC_Codifica_ProdottiAziendali ")
            'StrSQL.AppendLine("ON CAC.Tipo_Codifica = CAC_Codifica_ProdottiAziendali.Tipo_Codifica  ")


            'If Not String.IsNullOrEmpty(Veg_Cod_Coltiva) Then
            ' StrSQL.AppendLineLine(" WHERE CAC.Veg_Cod_Coltiva = '" + Agro_SQL_SaveText(Veg_Cod_Coltiva) & "' ")
            'End If

            If Tipo_Codifica <> -1 Then
                StrSQL.AppendLine(" WHERE  (CAC.Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & ")   ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            Return DT

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Function


    '###################################################################
    Public Function Leggi(
                            ByVal Veg_Cod_Coltiva As String,
                            ByVal Veg_Cod_GIAS As Int32,
                            ByVal Descrizione As String,
                            ByVal Data_Modifica As Date,
                            ByVal Id_Cod As Int32,
                            ByVal Grfi_Cod As Int32,
                            ByVal Reg_Cod As Int32,
                            ByVal Grva_Cod As Int32,
                            ByVal Metodo_Produzione As Int32,
                            ByVal Raggruppamento_Varietale As Int32,
                            ByVal Veg_Cod_Coltiva_2 As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod_Coltiva  = ''
        '   Veg_Cod_GIAS As  = 0
        '   Descrizione As  = 0
        '   Data_Modifica = #1/1/1900#
        '   Id_Cod As  = 0
        '   Grfi_Cod As  = 0
        '   Reg_Cod As  = 0
        '   Grva_Cod As  = 0
        '   Metodo_Produzione As  = 0
        '   Raggruppamento_Varietale As  = 0
        '   Veg_Cod_Coltiva_2  = ''
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Veg_Cod_Coltiva, Veg_Cod_Gias ")
                    StrSQL.Append(" FROM    CAC_Codifica_Veg_Cod ")
                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Coltiva <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "'   ")
                    End If

                    If Veg_Cod_GIAS <> 0 Then
                        StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_GIAS) & "   ")
                    End If

                    If Descrizione <> "" Then
                        StrSQL.Append(" AND Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
                    End If

                    If Data_Modifica <> #1/1/1900# Then
                        StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If

                    If Reg_Cod <> 0 Then
                        StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
                    End If

                    If Grva_Cod <> 0 Then
                        StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
                    End If

                    If Metodo_Produzione <> 0 Then
                        StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
                    End If

                    If Raggruppamento_Varietale <> 0 Then
                        StrSQL.Append(" AND Raggruppamento_Varietale = " & Agro_SQL_SaveNum(Raggruppamento_Varietale) & "   ")
                    End If

                    If Veg_Cod_Coltiva_2 <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva_2 = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva_2) & "'   ")
                    End If

                    If Tipo_Codifica <> 0 Then
                        StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod_Coltiva ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    CAC_Codifica_Veg_Cod ")

                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Coltiva <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "'   ")
                    End If

                    If Veg_Cod_GIAS <> 0 Then
                        StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_GIAS) & "   ")
                    End If

                    If Descrizione <> "" Then
                        StrSQL.Append(" AND Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
                    End If

                    If Data_Modifica <> #1/1/1900# Then
                        StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If

                    If Reg_Cod <> 0 Then
                        StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
                    End If

                    If Grva_Cod <> 0 Then
                        StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
                    End If

                    If Metodo_Produzione <> 0 Then
                        StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
                    End If

                    If Raggruppamento_Varietale <> 0 Then
                        StrSQL.Append(" AND Raggruppamento_Varietale = " & Agro_SQL_SaveNum(Raggruppamento_Varietale) & "   ")
                    End If

                    If Veg_Cod_Coltiva_2 <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva_2 = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva_2) & "'   ")
                    End If

                    If Tipo_Codifica <> 0 Then
                        StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
                    End If
                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod_Coltiva ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  CAC_Codifica_Veg_Cod.*, Veg_Des ")
                    StrSQL.Append(" FROM    CAC_Codifica_Veg_Cod ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_cod = CAC_Codifica_Veg_Cod.Veg_Cod_Gias ")

                    StrSQL.Append(" WHERE   1=1 ")

                    If Veg_Cod_Coltiva <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "'   ")
                    End If

                    If Veg_Cod_GIAS <> 0 Then
                        StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_GIAS) & "   ")
                    End If

                    If Descrizione <> "" Then
                        StrSQL.Append(" AND Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'   ")
                    End If

                    If Data_Modifica <> #1/1/1900# Then
                        StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & "   ")
                    End If

                    If Grfi_Cod <> 0 Then
                        StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
                    End If

                    If Reg_Cod <> 0 Then
                        StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
                    End If

                    If Grva_Cod <> 0 Then
                        StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
                    End If

                    If Metodo_Produzione <> 0 Then
                        StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
                    End If

                    If Raggruppamento_Varietale <> 0 Then
                        StrSQL.Append(" AND Raggruppamento_Varietale = " & Agro_SQL_SaveNum(Raggruppamento_Varietale) & "   ")
                    End If

                    If Veg_Cod_Coltiva_2 <> "" Then
                        StrSQL.Append(" AND Veg_Cod_Coltiva_2 = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva_2) & "'   ")
                    End If

                    If Tipo_Codifica <> 0 Then
                        StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
                    End If
                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Cod_Coltiva ASC")
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


    '###################################################################
    Public Function VegCodGias_from_VegCodCliente(ByVal Veg_Cod_Cliente As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.VegCodGias_from_VegCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Veg_Cod_Gias As Integer = 0

        Try
            '---------------------------------------------


            DT = Leggi(Veg_Cod_Cliente,
                        0,
                        "",
                        AGRODATAINIZIO,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0, "",
                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        "",
                        "",
                        objParametri,
                        Tipo_Codifica)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Veg_Cod_Gias = DT.Rows(0).Item("Veg_Cod_Gias")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

        Return Veg_Cod_Gias

    End Function

    '###################################################################
    Public Sub VegGrfiCodGias_from_VegCodCliente(ByVal Veg_Cod_Cliente As String,
                                                       ByVal Tipo_Codifica As Integer,
                                                       ByRef veg_cod As Integer,
                                                       ByVal grfi_cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.VegGrfiCodGias_from_VegCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        veg_cod = 0
        grfi_cod = 0

        Try
            '---------------------------------------------


            DT = Leggi(Veg_Cod_Cliente,
                        0,
                        "",
                        AGRODATAINIZIO,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0, "",
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri,
                        Tipo_Codifica)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                veg_cod = DT.Rows(0).Item("Veg_Cod_Gias")
                grfi_cod = DT.Rows(0).Item("Grfi_Cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

    End Sub

    '###################################################################
    Public Function GrfiCodGias_from_VegCodCliente(ByVal Veg_Cod_Cliente As String,
                                                   ByVal Tipo_Codifica As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.GrfiCodGias_from_VegCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Grfi_Cod As Integer = 0

        Try
            '---------------------------------------------


            DT = Leggi(Veg_Cod_Cliente,
                        0,
                        "",
                        AGRODATAINIZIO,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0, "",
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri,
                        Tipo_Codifica)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Grfi_Cod = DT.Rows(0).Item("Grfi_Cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

        Return Grfi_Cod

    End Function

    '###################################################################
    Public Function VegCodCliente_from_VegCodGias(ByVal Veg_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.VegCodCliente_from_VegCodGias()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Veg_Cod_Cliente As String = ""

        Try
            '---------------------------------------------


            DT = Leggi("",
                       Veg_Cod,
                        "",
                        AGRODATAINIZIO,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0, "",
                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        "",
                        "",
                        objParametri,
                        Tipo_Codifica)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Veg_Cod_Cliente = DT.Rows(0).Item("Veg_Cod_Coltiva")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

        Return Veg_Cod_Cliente

    End Function

    '###################################################################
    Public Sub VegCodVegDes_from_VegCodCliente(ByVal Veg_Cod_Cliente As String,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Veg_Des As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R.VegCodVegDes_from_VegCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Veg_Cod = 0
        Veg_Des = ""

        Try

            '---------------------------------------------

            DT = Leggi(Veg_Cod_Cliente,
                          0,
                          "",
                          AGRODATAINIZIO,
                          0,
                          0,
                          0,
                          0,
                          0,
                          0, "",
                          enumSelezioneVariabile.Selezione_JoinDescrizioni,
                          "",
                          "",
                          objParametri,
                          Tipo_Codifica)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                Veg_Des = DT.Rows(0).Item("Veg_Des")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

    End Sub


    Public Sub ConvertiSpecie(ByVal Veg_Cod_Cliente As String,
                              ByRef Veg_Cod As Integer,
                              ByRef Id_Cod As Integer,
                              ByRef Grfi_Cod As Integer,
                              ByRef Grva_Cod As Integer,
                              ByRef Reg_Cod As Integer,
                              ByRef Metodo_Produzione As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "CAC_Codifica_Veg_Cod.CAC_Codifica_Veg_Cod_R.ConvertiSpecie()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Veg_Cod ")
            StrSQL.Append(" WHERE Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "'")

            If Tipo_Codifica <> 0 Then
                StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            Veg_Cod = 0
            Id_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Reg_Cod = 0
            Metodo_Produzione = 0

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                    Id_Cod = DT.Rows(0).Item("Id_Cod")
                    Grfi_Cod = DT.Rows(0).Item("Grfi_Cod")
                    Grva_Cod = DT.Rows(0).Item("Grva_Cod")
                    Reg_Cod = DT.Rows(0).Item("Reg_Cod")
                    Metodo_Produzione = DT.Rows(0).Item("Metodo_Produzione")
                End If
            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing

    End Sub


End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CAC_Codifica_Veg_Cod_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Prodotti_SpecieVegetali(ByVal Veg_Cod_Coltiva As String,
                                                   ByVal Veg_Cod_Gias As Integer,
                                                   ByVal Descrizione As String,
                                                   ByVal Id_Cod As Integer,
                                                   ByVal Grfi_Cod As Integer,
                                                   ByVal Reg_Cod As Integer,
                                                   ByVal Grva_Cod As Integer,
                                                   ByVal Metodo_Produzione As Integer,
                                                   ByVal Raggruppamento_Varietale_Cod As String,
                                                   ByVal Veg_Cod_Coltiva_2 As String,
                                                   ByVal Port_Cod As Integer,
                                                   ByVal Foral_Cod As Integer,
                                                   ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_Specie,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_W.Scrivi_Prodotti_SpecieVegetali()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_Veg_Cod ")
            StrSQL.Append("  ( " & vbCrLf)
            StrSQL.Append("  [Veg_Cod_Coltiva] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.Append("  ,[Id_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Grfi_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Reg_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Grva_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Metodo_Produzione] " & vbCrLf)
            StrSQL.Append("  ,[Raggruppamento_Varietale_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Coltiva_2] " & vbCrLf)
            StrSQL.Append("  ,[Port_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Foral_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Codifica] " & vbCrLf)
            StrSQL.Append("   ) ")

            StrSQL.Append(" VALUES (" & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Veg_Cod_Gias) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveDateTime(Date.Now) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Id_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grfi_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Reg_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grva_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Metodo_Produzione) & " " & vbCrLf)
            StrSQL.Append(", '" & Agro_SQL_SaveText(Raggruppamento_Varietale_Cod) & "' " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Veg_Cod_Coltiva_2) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Port_Cod) & "" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Foral_Cod) & "" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo_Codifica) & "" & vbCrLf)
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

    Public Function Scrivi(ByVal Descrizione As String _
                            , ByVal Grfi_Cod As Integer _
                            , ByVal Grva_Cod As Integer _
                            , ByVal Id_Cod As Integer _
                            , ByVal Metodo_Produzione As Integer _
                            , ByVal Raggruppamento_Varietale As Integer _
                            , ByVal Reg_Cod As Integer _
                            , ByVal Veg_Cod_Coltiva As String _
                            , ByVal Veg_Cod_Coltiva_2 As String _
                            , ByVal Veg_Cod_Gias As Integer _
                            , ByVal Data_Modifica As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_Veg_Cod ")
            StrSQL.Append("  ( " & vbCrLf)
            StrSQL.Append("  [Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Grfi_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Grva_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Id_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Metodo_Produzione] " & vbCrLf)
            StrSQL.Append("  ,[Raggruppamento_Varietale_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Reg_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Coltiva] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Coltiva_2] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.Append("   ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grfi_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grva_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Id_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Metodo_Produzione) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Raggruppamento_Varietale) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Reg_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Veg_Cod_Coltiva_2) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Veg_Cod_Gias) & " " & vbCrLf)
            StrSQL.Append("," & Agro_SQL_SaveDate(Data_Modifica) & "" & vbCrLf)
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

    Public Function Modifica_Prodotti_SpecieVegetali(ByVal ID As Integer,
                                                     ByVal Veg_Cod_Coltiva As String,
                                                     ByVal Veg_Cod_Coltiva_Old As String,
                                                     ByVal Veg_Cod_Gias As Integer,
                                                     ByVal Descrizione As String,
                                                     ByVal Id_Cod As Integer,
                                                     ByVal Grfi_Cod As Integer,
                                                     ByVal Reg_Cod As Integer,
                                                     ByVal Grva_Cod As Integer,
                                                     ByVal Metodo_Produzione As Integer,
                                                     ByVal Raggruppamento_Varietale_Cod As String,
                                                     ByVal Veg_Cod_Coltiva_2 As String,
                                                     ByVal Port_Cod As Integer,
                                                     ByVal Foral_Cod As Integer,
                                                     ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_W.Modifica_Prodotti_SpecieVegetali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0


            strSql.AppendLine("UPDATE CAC_Codifica_Veg_Cod SET ")

            strSql.AppendLine("   Veg_Cod_Coltiva           = '" + Agro_SQL_SaveText(Trim(Veg_Cod_Coltiva)) + "' ")
            strSql.AppendLine(" , Veg_Cod_Gias              = " + Agro_SQL_SaveNum(Trim(Veg_Cod_Gias)) + " ")

            strSql.AppendLine(" , Descrizione               = '" + Agro_SQL_SaveText(Trim(Descrizione)) + "' ")

            strSql.AppendLine(" , Data_Modifica             = " + Agro_SQL_SaveDateTime(Date.Now))

            strSql.AppendLine(" , Id_Cod                    = " + Agro_SQL_SaveNum(Trim(Id_Cod)) + " ")

            strSql.AppendLine(" , Grfi_Cod                  = " + Agro_SQL_SaveNum(Trim(Grfi_Cod)) + " ")
            strSql.AppendLine(" , Reg_Cod                   = " + Agro_SQL_SaveNum(Trim(Reg_Cod)) + " ")
            strSql.AppendLine(" , Grva_Cod                  = " + Agro_SQL_SaveNum(Trim(Grva_Cod)) + " ")
            strSql.AppendLine(" , Metodo_Produzione         = " + Agro_SQL_SaveNum(Trim(Metodo_Produzione)) + " ")
            strSql.AppendLine(" , Raggruppamento_Varietale_Cod  = '" + Agro_SQL_SaveText(Trim(Raggruppamento_Varietale_Cod)) + "' ")
            strSql.AppendLine(" , Veg_Cod_Coltiva_2         = '" + Agro_SQL_SaveText(Trim(Veg_Cod_Coltiva_2)) + "' ")

            strSql.AppendLine(" , Port_Cod                  =  " + Agro_SQL_SaveNum(Trim(Port_Cod)) + " ")
            strSql.AppendLine(" , Foral_Cod                 =  " + Agro_SQL_SaveNum(Trim(Foral_Cod)) + " ")
            strSql.AppendLine(" , Tipo_Codifica             =  " + Agro_SQL_SaveNum(Trim(Tipo_Codifica)) + " ")

            strSql.AppendLine(" WHERE   Veg_Cod_Coltiva     = '" + Agro_SQL_SaveText(Trim(Veg_Cod_Coltiva_Old)) + "'  ")
            strSql.AppendLine(" AND   Tipo_Codifica     =  " + Agro_SQL_SaveNum(Trim(Tipo_Codifica)) + " ")
            strSql.AppendLine(" AND   ID     =  " + Agro_SQL_SaveNum(ID) + " ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Cancella_Prodotti_SpecieVegetali(ByVal ID As Integer,
                                                     ByVal Veg_Cod_Coltiva As String,
                                                     ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_W.Cancella_Prodotti_SpecieVegetali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            '---------------------------------------------

            strSql.Length = 0
            strSql.AppendLine(" DELETE ")
            strSql.AppendLine(" FROM    CAC_Codifica_Veg_Cod ")
            strSql.AppendLine(" WHERE   (Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Coltiva) & "')  ")

            If Tipo_Codifica <> 0 Then
                strSql.AppendLine(" AND     (Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & ")   ")
            End If

            If ID <> 0 Then
                strSql.AppendLine(" AND     (ID = " & Agro_SQL_SaveNum(ID) & ")   ")
            End If

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

End Class