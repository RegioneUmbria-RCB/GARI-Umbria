Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Dpi_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Dim hashTable As Hashtable = New Hashtable()

    Public Function Leggi_CriteriIntervento(ByVal Id_CRIT As Int32,
                                            ByVal DFT_Cod As Int32,
                                            ByVal Dfr_Cod As Int32,
                                            ByVal ID_TCI As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable
        '

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_CriteriIntervento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct CriteriIntervento.*, TipiCriteriIntervento.Descrizione as TCI_Descrizione, VincoliXCriteriIntervento.IDVincolo " &
                    " FROM  ( CriteriIntervento Inner Join TipiCriteriIntervento ON ( CriteriIntervento.ID_TCI = TipiCriteriIntervento.ID_TCI ) Left outer Join VincoliXCriteriIntervento ON ( CriteriIntervento.ID_Crit =  VincoliXCriteriIntervento.Id_Crit))" &
                    " WHERE CriteriIntervento.Id_CRIT > 0 ")

            If Id_CRIT <> 0 Then
                StrSQL.AppendLine(" AND CriteriIntervento.Id_CRIT =  " & Agro_SQL_SaveNum(Id_CRIT) & "  ")
            End If

            If DFT_Cod <> 0 Then
                StrSQL.AppendLine(" AND CriteriIntervento.DFT_Cod =  " & Agro_SQL_SaveNum(DFT_Cod) & "  ")
            End If

            If Dfr_Cod <> 0 Then
                StrSQL.AppendLine(" AND CriteriIntervento.DFR_Cod =  " & Agro_SQL_SaveNum(Dfr_Cod) & "  ")
            End If

            If ID_TCI <> 0 Then
                StrSQL.AppendLine(" AND TipiCriteriIntervento.ID_TCI =  " & Agro_SQL_SaveNum(ID_TCI) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY CriteriIntervento.DFT_Cod, CriteriIntervento.DFR_Cod, CriteriIntervento.ID_Tci, CriteriIntervento.Posizione ASC ")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    '============================================================================
    'Lettura dei disciplinari privati o pubblici data PivaSuperUser
    '============================================================================
    Public Function Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI(ByVal piva As String, ByVal Flag_Privato_Pubblico As Int16, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing

        Try

            DT = EseguiQuery_Lettura(objParametri, Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI_GetQuery(piva, Flag_Privato_Pubblico), NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Private Function Leggi_DPI_DataPivaSuperUSer_PrivatoPubblico_DammiDPI_GetQuery(ByVal piva As String, ByVal Flag_Privato_Pubblico As Int16) As String
        'Return _
        '    "SELECT dbo.F_ElementoDaCodiceDescrizione (dpi.DPI_COD_Regolamento, dpi.NomeEsteso) from disciplinari Dpi inner join RegolamentiXpiva_superUser_OperazioniAutorizzate R on Dpi.DPI_COD_Regolamento = R.Cod_Regolamento and dpi.Flag_Privato_Pubblico = " & Flag_Privato_Pubblico & " where operazione=2 and R.Flag_Privato_Pubblico = " & Flag_Privato_Pubblico & " and R.Piva_SuperUser = '" & piva & "'"

        Dim TabellaDatoFlagPrivatoPubblico As String
        If Flag_Privato_Pubblico = 1 Then
            TabellaDatoFlagPrivatoPubblico = "StrutturaGerarchicaDPI_Pubblici"
        Else
            TabellaDatoFlagPrivatoPubblico = "StrutturaGerarchicaDPI"
        End If

        Dim rval As String =
            "SELECT dpi.DPI_COD_Regolamento, dpi.NomeEsteso " &
            "from " & TabellaDatoFlagPrivatoPubblico & " st " &
            "inner join Disciplinari DPI on DPI.DPI_Cod_Regolamento = st.cod_Regolamento and dpi.Flag_Privato_Pubblico = " & Flag_Privato_Pubblico &
            " WHERE st.Impresa in ( " &
                "select  '" & Agro_SQL_SaveText(piva) & "' as impresa )"

        Return rval

    End Function



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    '============================================================================
    'Lettura degli IAF validi per una specie vegetale e x regolamento
    '============================================================================

    Public Function Leggi_IAF(
                                ByVal Cod_Disciplinare As Int32,
                                ByVal Veg_Cod As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_IAF()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT iaf.* ")
            StrSQL.AppendLine(" FROM RaggruppamentiDPIXSpecieVegetali rcxsv ")
            StrSQL.AppendLine(" INNER JOIN RaggruppamentiColturaliDPI rc ON rc.id_rcdpi = rcxsv.id_rcdpi ")
            StrSQL.AppendLine(" INNER JOIN DifesaTestata dt ON dt.id_rcdpi = rc.id_rcdpi ")
            StrSQL.AppendLine(" INNER JOIN RegolamentiXDifesaTestata rdt ON dt.dft_cod = rdt.dft_cod ")
            StrSQL.AppendLine(" INNER Join ImpegniAggiuntiviFacoltativiXDifesaTestata iafxdt ON dt.DFT_COD = iafxdt.DFT_COD ")
            StrSQL.AppendLine(" INNER Join ImpegniAggiuntiviFacoltativi iaf ON iaf.IAF_Cod = iafxdt.IAF_Cod ")

            StrSQL.AppendLine(" WHERE flag_controllo = 0 ")
            'StrSQL.AppendLine(" AND rdt.cod_regolamento = 127 ")
            'StrSQL.AppendLine(" AND rcxsv.veg_cod = 64 ")
            'StrSQL.AppendLine(" --AND rcxsv.grfi_cod = 0 ")

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND rcxsv.Veg_Cod = " & Agro_SQL_SaveNum_NULL(Veg_Cod))
            End If


            'StrSQL.AppendLine(" AND   Regolamenti.ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
            '              " AND   Regolamenti.ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
            '              " AND   Regolamenti.Flag_Revocato = 0 ")

            If Cod_Disciplinare <> 0 Then
                StrSQL.AppendLine(" AND rdt.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
            End If

            'If Id_RcDpi <> 0 Then
            '    StrSQL.AppendLine(" AND RaggruppamentiColturaliDPIxRegolamenti.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            'End If

            'If Veg_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            'End If

            'If Reg_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND DisciplinarixAttivitaRegolamenti.Cod_Reg In ( Select Distinct DisciplinarixAttivitaRegolamenti.Cod_Reg from DisciplinarixAttivitaRegolamenti, Regolamenti Where Regolamenti.Reg_Cod_Agronica = " & Agro_SQL_SaveNum(Reg_Cod) & " And  Regolamenti.Cod_Regolamento = DisciplinarixAttivitaRegolamenti.Cod_Reg  ) ")
            'End If

            'If Flag_Privato_Pubblico <> 0 Then
            '    StrSQL.AppendLine(" AND Regolamenti.Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            'End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY rdt.Cod_Regolamento ")
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

    Public Function Leggi_IAF_Controlli(
                                ByVal Bio As Boolean,
                                ByVal Cod_Disciplinare As Int32,
                                ByVal Veg_Cod As Int32,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_IAF()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT iaf.* ")

            StrSQL.AppendLine(" FROM ImpegniAggiuntiviFacoltativi iaf  ")

            If Bio = False Then
                StrSQL.AppendLine(" INNER Join ImpegniAggiuntiviFacoltativiXDifesaTestata iafxdt ON iaf.IAF_Cod = iafxdt.IAF_Cod ")
                StrSQL.AppendLine(" INNER Join DifesaTestata dt ON dt.DFT_COD = iafxdt.DFT_COD ")
                StrSQL.AppendLine(" INNER Join RegolamentiXDifesaTestata rdt ON dt.dft_cod = rdt.dft_cod ")
                StrSQL.AppendLine(" INNER Join RaggruppamentiColturaliDPI rc ON dt.id_rcdpi = rc.id_rcdpi ")
                StrSQL.AppendLine(" INNER Join RaggruppamentiDPIXSpecieVegetali rcxsv ON rc.id_rcdpi = rcxsv.id_rcdpi ")
            End If

            StrSQL.AppendLine(" WHERE flag_controllo = 1 ")

            If Bio = False Then
                If Veg_Cod <> 0 Then
                    StrSQL.AppendLine(" AND rcxsv.Veg_Cod = " & Agro_SQL_SaveNum_NULL(Veg_Cod))
                End If
                If Cod_Disciplinare <> 0 Then
                    StrSQL.AppendLine(" AND rdt.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
                End If
            Else
                StrSQL.AppendLine(" AND flag_bio = 1 ")
                If Veg_Cod <> 0 Then
                    StrSQL.AppendLine(" AND controllo_Veg_Cod = '" & Agro_SQL_SaveText(Veg_Cod) & "' ")
                End If
            End If




            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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


    Public Function Leggi_Enti(
                                ByVal IDEnte As String,
                                 ByVal Reg_Istat As String,
                               ByVal Pro_Istat As String,
                               ByVal Com_Istat As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Enti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT IDEnte, NomeEnte, Reg_Istat, Pro_Istat, Com_Istat ")
            StrSQL.AppendLine(" FROM Enti  ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")


            If IDEnte <> "" Then
                StrSQL.AppendLine(" AND IDEnte IN (" & Agro_SQL_Save_Clausola_IN(IDEnte, False) & ")")
            End If

            If Reg_Istat <> "" Then
                StrSQL.AppendLine(" AND Reg_Istat IN  (" & Agro_SQL_SaveText(Reg_Istat) & ") ")
            End If

            If Pro_Istat <> "" Then
                StrSQL.AppendLine(" AND Pro_Istat IN  (" & Agro_SQL_SaveText(Pro_Istat) & ") ")
            End If

            If Com_Istat <> "" Then
                StrSQL.AppendLine(" AND Com_Istat IN  (" & Agro_SQL_SaveText(Com_Istat) & ") ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY NomeEnte ")
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
    'Lettura dei disciplinari validi per una specie vegetale e x regolamento
    '============================================================================

    Public Function Leggi_Disciplinari(ByVal Cod_Disciplinare As Int32,
                                       ByVal Id_RcDpi As Int32,
                                       ByVal Veg_Cod As Int32,
                                       ByVal Reg_Cod As Int32,
                                       ByVal Flag_Privato_Pubblico As Int32,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Disciplinari()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Regolamenti.* FROM Regolamenti ")

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" ,SpecieVegetali ")
                StrSQL.AppendLine(" ,RaggruppamentiDPIxSpecieVegetali ")
                StrSQL.AppendLine(" ,RaggruppamentiColturaliDPIxRegolamenti ")
            ElseIf Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" ,RaggruppamentiColturaliDPIxRegolamenti ")
            End If

            StrSQL.AppendLine("  , DisciplinarixAttivitaRegolamenti")
            StrSQL.AppendLine("  WHERE DisciplinarixAttivitaRegolamenti.Cod_Reg_Dpi  =  Regolamenti.Cod_Regolamento ")

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine("  AND RaggruppamentiColturaliDPIxRegolamenti.Cod_Regolamento =  DisciplinarixAttivitaRegolamenti.Cod_Reg_Dpi  ")
            End If


            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Id_RCDPI = RaggruppamentiColturaliDPIxRegolamenti.Id_RCDPI ")
                StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod ")
                'StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod_AUX ")
            End If


            StrSQL.AppendLine(" AND   Regolamenti.ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                          " AND   Regolamenti.ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                          " AND   Regolamenti.Flag_Revocato = 0 ")

            If Cod_Disciplinare <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiColturaliDPIxRegolamenti.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.AppendLine(" AND DisciplinarixAttivitaRegolamenti.Cod_Reg In ( Select Distinct DisciplinarixAttivitaRegolamenti.Cod_Reg from DisciplinarixAttivitaRegolamenti, Regolamenti Where Regolamenti.Reg_Cod_Agronica = " & Agro_SQL_SaveNum(Reg_Cod) & " And  Regolamenti.Cod_Regolamento = DisciplinarixAttivitaRegolamenti.Cod_Reg  ) ")
            End If

            If Flag_Privato_Pubblico <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Regolamenti.Cod_Regolamento ASC ")
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

    Public Function Leggi_Disciplinari_Cached(ByVal Cod_Disciplinare As Integer,
                                              ByVal Id_RcDpi As Integer,
                                              ByVal Veg_Cod As Integer,
                                              ByVal Reg_Cod As Integer,
                                              ByVal Flag_Privato_Pubblico As Integer,
                                              ByVal Validita_Inizio As Date,
                                              ByVal Validita_Fine As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Disciplinari_Cached()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim key = "key_" & Cod_Disciplinare & "_" & Id_RcDpi & "_" & Veg_Cod & "_" & Reg_Cod & "_" & Flag_Privato_Pubblico

            If hashTable.Contains(key) Then
                Return hashTable(key)
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Regolamenti.* FROM Regolamenti ")

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" ,SpecieVegetali ")
                StrSQL.AppendLine(" ,RaggruppamentiDPIxSpecieVegetali ")
                StrSQL.AppendLine(" ,RaggruppamentiColturaliDPIxRegolamenti ")
            ElseIf Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" ,RaggruppamentiColturaliDPIxRegolamenti ")
            End If

            StrSQL.AppendLine("  , DisciplinarixAttivitaRegolamenti")
            StrSQL.AppendLine("  WHERE DisciplinarixAttivitaRegolamenti.Cod_Reg_Dpi  =  Regolamenti.Cod_Regolamento ")

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine("  AND RaggruppamentiColturaliDPIxRegolamenti.Cod_Regolamento =  DisciplinarixAttivitaRegolamenti.Cod_Reg_Dpi  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Id_RCDPI = RaggruppamentiColturaliDPIxRegolamenti.Id_RCDPI ")
                StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod ")
            End If


            StrSQL.AppendLine(" AND   Regolamenti.ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                          " AND   Regolamenti.ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                          " AND   Regolamenti.Flag_Revocato = 0 ")

            If Cod_Disciplinare <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiColturaliDPIxRegolamenti.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.AppendLine(" AND DisciplinarixAttivitaRegolamenti.Cod_Reg In ( Select Distinct DisciplinarixAttivitaRegolamenti.Cod_Reg from DisciplinarixAttivitaRegolamenti, Regolamenti Where Regolamenti.Reg_Cod_Agronica = " & Agro_SQL_SaveNum(Reg_Cod) & " And  Regolamenti.Cod_Regolamento = DisciplinarixAttivitaRegolamenti.Cod_Reg  ) ")
            End If

            If Flag_Privato_Pubblico <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Regolamenti.Cod_Regolamento ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            hashTable.Add(key, DT)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    '============================================================================
    'Lettura dei disciplinari validi per una specie vegetale e x regolamento e x testata
    '============================================================================

    Public Function Leggi_Disciplinari_xTestata(ByVal Cod_Disciplinare As Int32,
                                                ByVal Id_RcDpi As Int32,
                                                ByVal Veg_Cod As Int32,
                                                ByVal Reg_Cod As Int32,
                                                ByVal Flag_Privato_Pubblico As Int32,
                                                ByVal TipoTestata As Int32,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Disciplinari_xTestata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Regolamenti.*, DifesaTestata.TipoTestata, DifesaTestata.ID_RCDPI, DifesaTestata.Stato_Cod, ")
            StrSQL.AppendLine(" RaggruppamentiDPIXSpecieVegetali.VEG_COD, RaggruppamentiDPIXSpecieVegetali.GRFI_COD, RaggruppamentiDPIXSpecieVegetali.Flag_Protetto, ")
            StrSQL.AppendLine(" RaggruppamentiColturaliDPI.Nome ")

            StrSQL.AppendLine(" FROM DifesaTestata INNER JOIN ")
            StrSQL.AppendLine(" RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine(" Regolamenti ON RegolamentiXDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO INNER JOIN ")
            StrSQL.AppendLine(" RaggruppamentiDPIXSpecieVegetali ON DifesaTestata.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI INNER JOIN ")
            StrSQL.AppendLine(" RaggruppamentiColturaliDPI ON RaggruppamentiDPIXSpecieVegetali.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.AppendLine(" WHERE Regolamenti.ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                        " AND   Regolamenti.ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                        " AND   Regolamenti.Flag_Revocato = 0 ")

            StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")

            If Cod_Disciplinare <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" AND DifesaTestata.ID_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiDPIXSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.AppendLine(" AND DisciplinarixAttivitaRegolamenti.Cod_Reg In ( Select Distinct DisciplinarixAttivitaRegolamenti.Cod_Reg from DisciplinarixAttivitaRegolamenti, Regolamenti Where Regolamenti.Reg_Cod_Agronica = " & Agro_SQL_SaveNum(Reg_Cod) & " And  Regolamenti.Cod_Regolamento = DisciplinarixAttivitaRegolamenti.Cod_Reg  ) ")
            End If

            If Flag_Privato_Pubblico <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Regolamenti.ValidoDal DESC, NomeEsteso ASC")
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


    Public Function Leggi_Disciplinari_xTestata_conRegolamentoConcimazione(ByVal Cod_Disciplinare As Int32,
                                                ByVal Id_RcDpi As Int32,
                                                ByVal Veg_Cod As Int32,
                                                ByVal Reg_Cod As Int32,
                                                ByVal Flag_Privato_Pubblico As Int32,
                                                ByVal TipoTestata As Int32,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Disciplinari_xTestata_conRegolamentoConcimazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Regolamenti.*, ")

            Select Case TipoTestata
                Case enum_Disciplinare_Tipo_Testata.Difesa, enum_Disciplinare_Tipo_Testata.Diserbo, enum_Disciplinare_Tipo_Testata.Fitoregolatore
                    StrSQL.AppendLine(" DifesaTestata.TipoTestata, DifesaTestata.ID_RCDPI, DifesaTestata.Stato_Cod, ")
                    StrSQL.AppendLine(" RaggruppamentiDPIXSpecieVegetali.VEG_COD, RaggruppamentiDPIXSpecieVegetali.GRFI_COD, RaggruppamentiDPIXSpecieVegetali.Flag_Protetto, RaggruppamentiColturaliDPI.Nome,")
                Case Else
                    StrSQL.AppendLine(" 4 AS TipoTestata, 0 AS ID_RCDPI,  ")
                    StrSQL.AppendLine(enum_WWorflow_WAnagraficaStati.Approvazione_DPi_Pubblicato & " AS Stato_Cod, ")
                    StrSQL.AppendLine(" 0 AS VEG_COD, 0 AS GRFI_COD, 0 AS Flag_Protetto, '' AS Nome,")
            End Select

            StrSQL.AppendLine(" PUA_Regolamenti.* ")

            StrSQL.AppendLine(" FROM Regolamenti (NOLOCK)")

            Select Case TipoTestata
                Case enum_Disciplinare_Tipo_Testata.Difesa, enum_Disciplinare_Tipo_Testata.Diserbo, enum_Disciplinare_Tipo_Testata.Fitoregolatore
                    StrSQL.AppendLine(" INNER JOIN RegolamentiXDifesaTestata (NOLOCK) ON RegolamentiXDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO  ")
                    StrSQL.AppendLine(" INNER JOIN DifesaTestata (NOLOCK) ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD ")
                    StrSQL.AppendLine(" INNER JOIN RaggruppamentiDPIXSpecieVegetali (NOLOCK) ON DifesaTestata.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
                    StrSQL.AppendLine(" INNER JOIN RaggruppamentiColturaliDPI (NOLOCK) ON RaggruppamentiDPIXSpecieVegetali.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            End Select

            StrSQL.AppendLine(" LEFT JOIN PUA_Regolamenti (NOLOCK) ON Regolamenti.PUA_Regolamento_Cod = PUA_Regolamenti.Regolamento_Cod ")


            StrSQL.AppendLine(" WHERE Regolamenti.ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                        " AND   Regolamenti.ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                        " AND   Regolamenti.Flag_Revocato = 0 ")

            If Flag_Privato_Pubblico <> 0 Then
                StrSQL.AppendLine(" AND Regolamenti.Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            End If

            Select Case TipoTestata

                Case enum_Disciplinare_Tipo_Testata.Difesa, enum_Disciplinare_Tipo_Testata.Diserbo, enum_Disciplinare_Tipo_Testata.Fitoregolatore

                    StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")

                    If Cod_Disciplinare <> 0 Then
                        StrSQL.AppendLine(" AND Regolamenti.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
                    End If

                    If Id_RcDpi <> 0 Then
                        StrSQL.AppendLine(" AND DifesaTestata.ID_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND RaggruppamentiDPIXSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                    End If

                    If Reg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND DisciplinarixAttivitaRegolamenti.Cod_Reg In ( Select Distinct DisciplinarixAttivitaRegolamenti.Cod_Reg from DisciplinarixAttivitaRegolamenti, Regolamenti Where Regolamenti.Reg_Cod_Agronica = " & Agro_SQL_SaveNum(Reg_Cod) & " And  Regolamenti.Cod_Regolamento = DisciplinarixAttivitaRegolamenti.Cod_Reg  ) ")
                    End If

            End Select


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Regolamenti.ValidoDal DESC, NomeEsteso ASC")
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
    'Lettura dei disciplinari nel metaschema locale
    '============================================================================

    Public Function Leggi_DPI_Regolamenti(ByVal Flag_Privato_Pubblico As Int32,
                                          ByVal COD_REGOLAMENTO As Int32,
                                          ByVal ID_TR As Int32,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_DPI_Regolamenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * FROM DPI_Regolamenti ")
            StrSQL.AppendLine(" WHERE ValidoDal <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                          " AND   ValidoAl >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                          " AND   Flag_Revocato = 0 ")

            If Flag_Privato_Pubblico <> -1 Then
                StrSQL.AppendLine(" AND Flag_Privato_Pubblico =  " & Agro_SQL_SaveNum(Flag_Privato_Pubblico) & "  ")
            End If

            If ID_TR <> 0 Then
                StrSQL.AppendLine(" AND ID_TR =  " & Agro_SQL_SaveNum(ID_TR) & "  ")
            End If

            If COD_REGOLAMENTO <> 0 Then
                StrSQL.AppendLine(" AND COD_REGOLAMENTO =  " & Agro_SQL_SaveNum(COD_REGOLAMENTO) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY NomeEsteso ASC ")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################



    Public Function Leggi_EpocheDifesa(ByVal Id_RcDpi As Int32,
                                       ByVal Disciplinare_Cod As Int32,
                                       ByVal Modulo As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_EpocheDifesa()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct DifesaRighe.Modulo, DifesaRighe.Da_Data, DifesaRighe.A_Data, DifesaRighe.DescrizionePeriodoDa, DifesaRighe.DescrizionePeriodoA " &
                          " FROM   DifesaRighe INNER JOIN " &
                          " DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN " &
                          " RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD ")

            StrSQL.AppendLine(" WHERE  DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                          " AND    DifesaRighe.Modulo Is Not Null " &
                          " AND    DifesaTestata.TipoTestata =  0 ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Data ASC")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################



    Public Function Leggi_EpocheDiserbo(ByVal Id_RcDpi As Int32,
                                       ByVal Disciplinare_Cod As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_EpocheDiserbo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT DifesaTestata.Id_RCDPI, Epoche.EP_COD, Epoche.Descrizione, Epoche.Tipo ")

            StrSQL.AppendLine(" FROM   Epoche INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata INNER JOIN ")
            StrSQL.AppendLine("        DifesaTestata ON RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD ON Epoche.EP_COD >= DifesaRighe.Da_EP_COD AND  ")
            StrSQL.AppendLine("        Epoche.EP_COD <= DifesaRighe.A_EP_COD ")

            StrSQL.AppendLine(" WHERE  DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                          " AND    DifesaTestata.TipoTestata =  1 ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Epoche.Ep_Cod ASC")
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


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################



    Public Function Leggi_GruppiAvversitaDPI(ByVal Id_GaDPI As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_GruppiAvversitaDPI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct GruppiAvversitaDPI.* " &
                          " FROM  GruppiAvversitaDPI ")

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" WHERE GruppiAvversitaDPI.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY GruppiAvversitaDPI.Descrizione ASC")
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



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_GruppiPrincipiAttivi(ByVal Gpa_Cod As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_GruppiPrincipiAttivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT GPA_COD, ISNULL(GPA_DES,'') AS GPA_DES " &
                          " FROM   GruppiPrincipiAttivi ")

            If Gpa_Cod <> 0 Then
                StrSQL.AppendLine(" WHERE GPA_Cod =  " & Agro_SQL_SaveNum(Gpa_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY GPA_Cod ASC")
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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################



    Public Function Leggi_GruppiPrincipiAttivixPrincipiAttivi(ByVal Gpa_Cod As Int32,
                                                              ByVal Pa_Cod As Int32,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                              Optional parametrizza As Boolean = True
                                                              ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_GruppiPrincipiAttivixPrincipiAttivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT Distinct GruppiPrincipiAttivixPrincipiAttivi.PA_COD, GruppiPrincipiAttivixPrincipiAttivi.GPA_COD, GruppiPrincipiAttivi.GPA_Des " &
                          " FROM   GruppiPrincipiAttivixPrincipiAttivi, GruppiPrincipiAttivi " &
                          " WHERE  GruppiPrincipiAttivixPrincipiAttivi.Gpa_Cod = GruppiPrincipiAttivi.Gpa_Cod  ")

            If Gpa_Cod <> 0 Then
                StrSQL.AppendLine(" And GruppiPrincipiAttivi.GPA_Cod =  " & Agro_SQL_SaveNum(Gpa_Cod) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" And GruppiPrincipiAttivixPrincipiAttivi.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, parametrizza, objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY GruppiPrincipiAttivixPrincipiAttivi.GPA_Cod, GruppiPrincipiAttivixPrincipiAttivi.PA_Cod ASC")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_Infestanti(ByVal Id_RcDpi As Int32,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal TipoTestata As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal Pa_Cod As Int32,
                                            ByVal Gru_Pa_Cod As Int32,
                                            ByVal Id_PAA As Int32,
                                            ByVal strAvversita As String,
                                            ByVal Modulo As Int32,
                                            ByVal Ep_Cod As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Infestanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.* ")


            StrSQL.AppendLine(" FROM   PA_Ausiliari INNER JOIN ")
            StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
            StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD  ")

            StrSQL.AppendLine(" WHERE  DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                          " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(strAvversita))

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru > 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod > 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Ep_Cod, DifesaRighe.Modulo, DifesaRighe.Da_Data, Infestanti.DFT_Cod, Infestanti.DFR_Cod  ASC")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_Infestanti_conDescrizioniAvversita(ByVal Id_RcDpi As Int32,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal TipoTestata As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal Pa_Cod As Int32,
                                            ByVal Gru_Pa_Cod As Int32,
                                            ByVal Id_PAA As Int32,
                                            ByVal strAvversita As String,
                                            ByVal Modulo As Int32,
                                            ByVal Ep_Cod As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Infestanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.*, ")
            StrSQL.AppendLine(" ISNULL(Avversita.AV_DES_VOL,'') AS AV_DES_VOL, ISNULL(Avversita.AV_DES_LAT,'') AS AV_DES_LAT, ")
            StrSQL.AppendLine(" ISNULL(GruppoAvversita.AV_GRU_DES,'') AS AV_GRU_DES, ISNULL(GruppoAvversita.AV_GRU_DES_LAT,'') AS AV_GRU_DES_LAT ")

            StrSQL.AppendLine(" FROM   PA_Ausiliari INNER JOIN ")
            StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
            StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD LEFT OUTER JOIN ")
            StrSQL.AppendLine("        GruppoAvversita ON Infestanti.AV_GRU = GruppoAvversita.AV_GRU LEFT OUTER JOIN ")
            StrSQL.AppendLine("        Avversita ON Infestanti.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" WHERE  DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                          " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Ep_Cod, DifesaRighe.Modulo, DifesaRighe.Da_Data, Infestanti.DFT_Cod, Infestanti.DFR_Cod  ASC")
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

    'a differenza della precedente il parametro Id_RcDpi è una stringa
    'si possono leggere più Id_RcDpi
    Public Function Leggi_Infestanti_conDescrizioniAvversita_2(ByVal Id_RcDpi As String,
                                          ByVal Disciplinare_Cod As Int32,
                                          ByVal TipoTestata As Int32,
                                          ByVal Id_GaDPI As Int32,
                                          ByVal Av_Gru As Int32,
                                          ByVal Av_Cod As Int32,
                                          ByVal Pa_Cod As Int32,
                                          ByVal Gru_Pa_Cod As Int32,
                                          ByVal Id_PAA As Int32,
                                          ByVal strAvversita As String,
                                          ByVal Modulo As Int32,
                                          ByVal Ep_Cod As Int32,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Infestanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.*, ")
            StrSQL.AppendLine(" ISNULL(Avversita.AV_DES_VOL,'') AS AV_DES_VOL, ISNULL(Avversita.AV_DES_LAT,'') AS AV_DES_LAT, ")
            StrSQL.AppendLine(" ISNULL(GruppoAvversita.AV_GRU_DES,'') AS AV_GRU_DES, ISNULL(GruppoAvversita.AV_GRU_DES_LAT,'') AS AV_GRU_DES_LAT ")

            StrSQL.AppendLine(" FROM   PA_Ausiliari INNER JOIN ")
            StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
            StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD LEFT OUTER JOIN ")
            StrSQL.AppendLine("        GruppoAvversita ON Infestanti.AV_GRU = GruppoAvversita.AV_GRU LEFT OUTER JOIN ")
            StrSQL.AppendLine("        Avversita ON Infestanti.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" WHERE  PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Id_RcDpi <> "" Then
                StrSQL.AppendLine(" AND DifesaTestata.Id_RCDPI IN  (" & Agro_SQL_SaveText(Id_RcDpi) & ")  ")
            End If

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
                If Gru_Pa_Cod <> 0 Then
                    StrSQL.AppendLine(" OR Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
                End If
                StrSQL.AppendLine(" )  ")
            End If

            '(23/01/2017) fede eliminato filtro su gruppo sostanze perché spostato sopra nella singola (nelle etichette non c'è mai il gruppo)
            'If Gru_Pa_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            'End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Ep_Cod, DifesaRighe.Modulo, DifesaRighe.Da_Data, Infestanti.DFT_Cod, Infestanti.DFR_Cod  ASC")
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


    Public Function Leggi_Infestanti_conDescrizioniAvversita_Deroghe(ByVal Id_RcDpi As String,
                                          ByVal Disciplinare_Cod As Int32,
                                          ByVal TipoTestata As Int32,
                                          ByVal Id_GaDPI As Int32,
                                          ByVal Av_Gru As Int32,
                                          ByVal Av_Cod As Int32,
                                          ByVal Pa_Cod As Int32,
                                          ByVal Gru_Pa_Cod As Int32,
                                          ByVal Id_PAA As Int32,
                                          ByVal strAvversita As String,
                                          ByVal Modulo As Int32,
                                          ByVal Ep_Cod As Int32,
                                                ByVal Data As Date,
                                                ByVal strListaComuni As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Infestanti_conDescrizioniAvversita_Deroghe()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            '--------------------------------------------------------------------------
            ' AVVERSITA SENZA DEROGHE
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.*, ")
            StrSQL.AppendLine(" ISNULL(Avversita.AV_DES_VOL,'') AS AV_DES_VOL, ISNULL(Avversita.AV_DES_LAT,'') AS AV_DES_LAT, ")
            StrSQL.AppendLine(" ISNULL(GruppoAvversita.AV_GRU_DES,'') AS AV_GRU_DES, ISNULL(GruppoAvversita.AV_GRU_DES_LAT,'') AS AV_GRU_DES_LAT ")

            StrSQL.AppendLine(" FROM   PA_Ausiliari INNER JOIN ")
            StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
            StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD LEFT OUTER JOIN ")
            StrSQL.AppendLine("        GruppoAvversita ON Infestanti.AV_GRU = GruppoAvversita.AV_GRU LEFT OUTER JOIN ")
            StrSQL.AppendLine("        Avversita ON Infestanti.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" WHERE  PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Id_RcDpi <> "" Then
                StrSQL.AppendLine(" AND DifesaTestata.Id_RCDPI IN  (" & Agro_SQL_Save_Clausola_IN(Id_RcDpi) & ")  ")
            End If

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
                If Gru_Pa_Cod <> 0 Then
                    StrSQL.AppendLine(" OR Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
                End If
                StrSQL.AppendLine(" )  ")
            End If

            '(23/01/2017) fede eliminato filtro su gruppo sostanze perché spostato sopra nella singola (nelle etichette non c'è mai il gruppo)
            'If Gru_Pa_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            'End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If

            StrSQL.AppendLine(" AND PA_Ausiliari.deroga = 0 ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            ' DEROGHE SU TUTTA LA TESTATA (regione)
            StrSQL.AppendLine(" UNION  ")

            StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.*, ")
            StrSQL.AppendLine(" ISNULL(Avversita.AV_DES_VOL,'') AS AV_DES_VOL, ISNULL(Avversita.AV_DES_LAT,'') AS AV_DES_LAT, ")
            StrSQL.AppendLine(" ISNULL(GruppoAvversita.AV_GRU_DES,'') AS AV_GRU_DES, ISNULL(GruppoAvversita.AV_GRU_DES_LAT,'') AS AV_GRU_DES_LAT ")

            StrSQL.AppendLine(" FROM   PA_Ausiliari  ")

            StrSQL.AppendLine(" inner Join derogheterritoriali  on derogheterritoriali.der_cod=pa_ausiliari.derogariferimento ")
            StrSQL.AppendLine(" inner Join derogheterritorialixenti  on derogheterritorialixenti.der_cod=derogheterritoriali.der_cod ")
            StrSQL.AppendLine(" inner Join enti  on enti.idente=derogheterritorialixenti.IDEnte INNER JOIN ")

            StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
            StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
            StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
            StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD LEFT OUTER JOIN ")
            StrSQL.AppendLine("        GruppoAvversita ON Infestanti.AV_GRU = GruppoAvversita.AV_GRU LEFT OUTER JOIN ")
            StrSQL.AppendLine("        Avversita ON Infestanti.AV_COD = Avversita.AV_COD ")

            StrSQL.AppendLine(" WHERE  PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Id_RcDpi <> "" Then
                StrSQL.AppendLine(" AND DifesaTestata.Id_RCDPI IN  (" & Agro_SQL_Save_Clausola_IN(Id_RcDpi) & ")  ")
            End If

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
                If Gru_Pa_Cod <> 0 Then
                    StrSQL.AppendLine(" OR Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
                End If
                StrSQL.AppendLine(" )  ")
            End If

            '(23/01/2017) fede eliminato filtro su gruppo sostanze perché spostato sopra nella singola (nelle etichette non c'è mai il gruppo)
            'If Gru_Pa_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            'End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If

            StrSQL.AppendLine(" And PA_Ausiliari.deroga = 1 ")
            StrSQL.AppendLine(" And enti.pro_istat='000' ")
            StrSQL.AppendLine(" AND enti.com_istat='000' ")
            StrSQL.AppendLine(" AND (derogheterritoriali.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validadal is null) ")
            StrSQL.AppendLine(" AND (derogheterritoriali.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validaal is null)  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            ' DEROGHE SU PROVINCE COMUNI

            If strListaComuni <> "" And strListaComuni <> "000000" Then

                StrSQL.AppendLine(" UNION  ")

                StrSQL.AppendLine(" SELECT Distinct Infestanti.*, DifesaRighe.*, PA_Ausiliari.*, ")
                StrSQL.AppendLine(" ISNULL(Avversita.AV_DES_VOL,'') AS AV_DES_VOL, ISNULL(Avversita.AV_DES_LAT,'') AS AV_DES_LAT, ")
                StrSQL.AppendLine(" ISNULL(GruppoAvversita.AV_GRU_DES,'') AS AV_GRU_DES, ISNULL(GruppoAvversita.AV_GRU_DES_LAT,'') AS AV_GRU_DES_LAT ")

                StrSQL.AppendLine(" FROM   PA_Ausiliari  ")

                StrSQL.AppendLine(" inner Join derogheterritoriali  on derogheterritoriali.der_cod=pa_ausiliari.derogariferimento ")
                StrSQL.AppendLine(" inner Join derogheterritorialixenti  on derogheterritorialixenti.der_cod=derogheterritoriali.der_cod ")
                StrSQL.AppendLine(" inner Join enti  on enti.idente=derogheterritorialixenti.IDEnte INNER JOIN ")

                StrSQL.AppendLine("        Infestanti ON PA_Ausiliari.DFT_COD = Infestanti.DFT_COD AND PA_Ausiliari.DFR_COD = Infestanti.DFR_COD INNER JOIN ")
                StrSQL.AppendLine("        DifesaRighe ON Infestanti.DFT_COD = DifesaRighe.DFT_COD AND Infestanti.DFR_COD = DifesaRighe.DFR_COD INNER JOIN  ")
                StrSQL.AppendLine("        DifesaTestata ON DifesaRighe.DFT_COD = DifesaTestata.DFT_COD INNER JOIN ")
                StrSQL.AppendLine("        RegolamentiXDifesaTestata ON DifesaTestata.DFT_COD = RegolamentiXDifesaTestata.DFT_COD LEFT OUTER JOIN ")
                StrSQL.AppendLine("        GruppoAvversita ON Infestanti.AV_GRU = GruppoAvversita.AV_GRU LEFT OUTER JOIN ")
                StrSQL.AppendLine("        Avversita ON Infestanti.AV_COD = Avversita.AV_COD ")

                StrSQL.AppendLine(" WHERE  PA_Ausiliari.PASenzaControlli =  0  " & "  ")

                If Id_RcDpi <> "" Then
                    StrSQL.AppendLine(" AND DifesaTestata.Id_RCDPI IN  (" & Agro_SQL_Save_Clausola_IN(Id_RcDpi) & ")  ")
                End If

                If Disciplinare_Cod <> 0 Then
                    StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
                End If

                If Id_GaDPI <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
                End If

                If TipoTestata <> -1 Then
                    StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
                End If

                If Pa_Cod <> 0 Then
                    StrSQL.AppendLine(" AND (Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
                    If Gru_Pa_Cod <> 0 Then
                        StrSQL.AppendLine(" OR Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
                    End If
                    StrSQL.AppendLine(" )  ")
                End If

                '(23/01/2017) fede eliminato filtro su gruppo sostanze perché spostato sopra nella singola (nelle etichette non c'è mai il gruppo)
                'If Gru_Pa_Cod <> 0 Then
                '    StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
                'End If

                If Id_PAA <> 0 Then
                    StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
                End If

                If Ep_Cod <> 0 Then
                    StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                    StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                End If

                If Modulo <> 0 Then
                    StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
                End If


                If Trim(strAvversita) <> "" Then

                    StrSQL.AppendLine(" AND " & strAvversita)

                Else
                    'Posso Applicare il filtro sulle singole avversità
                    If Av_Gru <> 0 Then
                        StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                    End If

                    If Av_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                    End If

                End If

                StrSQL.AppendLine(" AND PA_Ausiliari.deroga = 1 ")
                StrSQL.AppendLine(" AND (derogheterritoriali.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validadal is null) ")
                StrSQL.AppendLine(" AND (derogheterritoriali.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validaal is null)  ")

                Dim FiltroEnti As String = ""
                Dim ArrayComuni() As String = Split(strListaComuni, ",")
                If Not ArrayComuni Is Nothing Then
                    For c = 0 To ArrayComuni.Length - 1
                        FiltroEnti &= " AND ((enti.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and enti.com_istat='000') " &
                                      "      OR (enti.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and enti.com_istat='" & ArrayComuni(c).Substring(3, 3) & "'))"
                    Next
                End If
                StrSQL.AppendLine("  " & FiltroEnti)


            End If


            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Ep_Cod, DifesaRighe.Modulo, DifesaRighe.Da_Data, Infestanti.DFT_Cod, Infestanti.DFR_Cod  ASC")
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


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    ''============================================================================
    ''Lettura degli Infestanti x Consultazione (Left outer join PA_Ausiliari)
    ''============================================================================

    'Public Function Leggi_InfestantixConsultazione(ByVal Id_RcDpi As Int32, _
    '                                        ByVal Disciplinare_Cod As Int32, _
    '                                        ByVal TipoTestata As Int32, _
    '                                        ByVal Id_GaDPI As Int32, _
    '                                        ByVal Av_Gru As Int32, _
    '                                        ByVal Av_Cod As Int32, _
    '                                        ByVal Pa_Cod As Int32, _
    '                                        ByVal Gru_Pa_Cod As Int32, _
    '                                        ByVal Id_PAA As Int32, _
    '                                        ByVal Modulo As Int32, _
    '                                        ByVal Ep_Cod As Int32, _
    '                                             ByVal xFiltroAggiuntivo As String, _
    '                                             ByVal xOrderBy As String, _
    '                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                  ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_InfestantixConsultazione()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        StrSQL.Length = 0

    '        StrSQL.AppendLine(" SELECT DISTINCT dbo.Infestanti.*, dbo.DifesaRighe.*, dbo.PA_Ausiliari.*" & _
    '                        " FROM dbo.DifesaTestata INNER JOIN " & _
    '                        " dbo.Infestanti ON dbo.DifesaTestata.DFT_COD = dbo.Infestanti.DFT_COD INNER JOIN " & _
    '                        " dbo.RegolamentiXDifesaRighe ON dbo.Infestanti.DFT_COD = dbo.RegolamentiXDifesaRighe.DFT_COD AND " & _
    '                        " dbo.Infestanti.DFR_COD = dbo.RegolamentiXDifesaRighe.DFR_COD INNER JOIN " & _
    '                        " dbo.DifesaRighe ON dbo.Infestanti.DFT_COD = dbo.DifesaRighe.DFT_COD AND " & _
    '                        " dbo.Infestanti.DFR_COD = dbo.DifesaRighe.DFR_COD LEFT OUTER JOIN " & _
    '                        " dbo.PA_Ausiliari ON dbo.Infestanti.DFT_COD = dbo.PA_Ausiliari.DFT_COD AND dbo.Infestanti.DFR_COD = dbo.PA_Ausiliari.DFR_COD " & _
    '                        " Where DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & " AND " & _
    '                        " (dbo.PA_Ausiliari.PASenzaControlli = 0 OR dbo.PA_Ausiliari.PASenzaControlli IS NULL)")

    '        If Disciplinare_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND RegolamentixDifesaRighe.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
    '        End If

    '        If Id_GaDPI <> 0 Then
    '            StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
    '        End If

    '        If Av_Gru <> 0 Then
    '            StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
    '        End If

    '        If Av_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
    '        End If

    '        If TipoTestata <> -1 Then
    '            StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
    '        End If

    '        If Pa_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
    '        End If

    '        If Gru_Pa_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
    '        End If

    '        If Id_PAA <> 0 Then
    '            StrSQL.AppendLine(" AND Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
    '        End If

    '        If Ep_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
    '            StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
    '        End If

    '        If Modulo <> 0 Then
    '            StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
    '        End If


    '        '--------------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------

    '        If xOrderBy <> "" Then
    '            strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.AppendLine(" ORDER BY DifesaRighe.Da_Ep_Cod, DifesaRighe.Modulo, DifesaRighe.Da_Data, Infestanti.DFT_Cod, Infestanti.DFR_Cod  ASC")
    '        End If








    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT


    'End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################



    Public Function Leggi_LimitazioniUso(ByVal Id_Lu As Int32,
                                            ByVal strId_Paa As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine("  SELECT Distinct LimitazioniUso.* , LimitazioniUsoXVincoli.IDVincolo " &
                            " FROM   ( LimitazioniUso Left Outer Join LimitazioniUsoXVincoli ON  LimitazioniUso.Id_Lu = LimitazioniUsoXVincoli.Id_Lu ) " &
                            " Where  LimitazioniUso.ID_LU > 0 ")

            If Id_Lu <> 0 Then
                StrSQL.AppendLine(" AND LimitazioniUso.ID_LU =  " & Agro_SQL_SaveNum(Id_Lu) & "  ")
            End If

            If Trim(strId_Paa) <> "" Then
                StrSQL.AppendLine(" AND LimitazioniUso.ID_PAA IN " & Agro_SQL_SaveText(strId_Paa) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY LimitazioniUso.ID_LU ASC")
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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################
    'Lettura delle Limitazioni Uso Valide per controllo

    ' 1. Le limitazioni uso che soddisfano i requisiti
    ' 2. Le limitazioni uso che fanno parte dello stesso Gruppo Esclusività (Ge_PAA_Cod)
    Public Function Leggi_LimitazioniUso_Da_Controllare(ByVal Disciplinare_Cod As Int32,
                                                    ByVal Id_RcDpi As Int32,
                                                    ByVal TipoTestata As Int32,
                                                    ByVal Pa_Cod As Int32,
                                                    ByVal Gru_Pa_Cod As Int32,
                                                    ByVal Id_GaDPI As Int32,
                                                    ByVal Av_Gru As Int32,
                                                    ByVal Av_Cod As Int32,
                                                    ByVal Id_PAA As Int32,
                                                    ByVal Ge_Paa_Cod As Int32,
                                                    ByVal strAvversita As String,
                                                    ByVal Modulo As Int32,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUso_Da_Controllare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim StrSQL1 As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" FROM   RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, Infestanti, LimitazioniUso, PA_Ausiliari " &
                        " Where  LimitazioniUso.ID_PAA = PA_Ausiliari.ID_PAA " &
                        " And    PA_Ausiliari.DFT_Cod = DifesaRighe.DFT_Cod " &
                        " And    PA_Ausiliari.DFR_Cod = DifesaRighe.DFR_Cod " &
                        " And    PA_Ausiliari.DFT_Cod = DifesaTestata.DFT_Cod " &
                        " And    RegolamentixDifesaTestata.DFT_Cod = DifesaTestata.DFT_Cod " &
                        " And    Infestanti.DFT_Cod = DifesaRighe.DFT_Cod " &
                        " And    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod " &
                        " And    LimitazioniUso.Flag_Controllo = 1 " &
                        " And    PA_Ausiliari.PASenzaControlli =  0  " & "  ")


            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentixDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" AND DifesaTestata.Id_RcDpi =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Ausiliari.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

            End If

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND LimitazioniUso.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ge_Paa_Cod <> 0 Then
                StrSQL.AppendLine(" AND LimitazioniUso.Ge_Paa_Cod =  " & Agro_SQL_SaveNum(Ge_Paa_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'Costruzione Query Finale
            StrSQL1.Length = 0
            StrSQL1.AppendLine(" ( SELECT Distinct LimitazioniUso.*  " & StrSQL.ToString & " ) UNION ALL ( SELECT Distinct LimitazioniUso.* From LimitazioniUso Where LimitazioniUso.Ge_PAA_Cod IN (Select LimitazioniUso.Ge_PAA_Cod " & StrSQL.ToString & " AND GE_PAA_Cod is not null ) ) ")



            ''--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    StrSQL1.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL1.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL1.AppendLine(" ORDER BY LimitazioniUso.Id_Lu, LimitazioniUso.ID_PAA ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL1.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    'a differenza della precedente considera le deroghe e le relative date
    Public Function Leggi_LimitazioniUso_Da_Controllare_Deroghe(ByVal Disciplinare_Cod As Int32,
                                                    ByVal Id_RcDpi As Int32,
                                                    ByVal TipoTestata As Int32,
                                                    ByVal Pa_Cod As Int32,
                                                    ByVal Gru_Pa_Cod As Int32,
                                                    ByVal Id_GaDPI As Int32,
                                                    ByVal Av_Gru As Int32,
                                                    ByVal Av_Cod As Int32,
                                                    ByVal Id_PAA As Int32,
                                                    ByVal Ge_Paa_Cod As Int32,
                                                    ByVal strAvversita As String,
                                                    ByVal Modulo As Int32,
                                                        ByVal Data As Date,
                                                        ByVal strListaComuni As String,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUso_Da_Controllare_Deroghe()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StrSQLTot As New System.Text.StringBuilder
        Dim StrSQLFrom As New System.Text.StringBuilder
        Dim StrSQLFromxDeroghe As New System.Text.StringBuilder
        Dim StrSQLWhere As New System.Text.StringBuilder
        Dim StrSQLWherexDeroghe1 As New System.Text.StringBuilder
        Dim StrSQLWherexDeroghe2 As New System.Text.StringBuilder
        Dim StrSQLWherexDeroghe3 As New System.Text.StringBuilder

        Try


            StrSQLFrom.Length = 0
            StrSQLFromxDeroghe.Length = 0
            StrSQLWhere.Length = 0
            StrSQLWherexDeroghe1.Length = 0 'limitazioni NON Deroga
            StrSQLWherexDeroghe2.Length = 0 'limitazioni Deroga su tutta la testata
            StrSQLWherexDeroghe3.Length = 0 'limitazioni Deroga su specifici comuni (passati da Gias)

            StrSQLFrom.AppendLine(" FROM   LimitazioniUso l ")
            StrSQLFrom.AppendLine(" inner Join PA_Ausiliari p on l.ID_PAA = p.ID_PAA ")
            StrSQLFrom.AppendLine(" inner Join DifesaRighe dr on p.DFT_Cod = dr.DFT_Cod  And    p.DFR_Cod = dr.DFR_Cod   ")
            StrSQLFrom.AppendLine(" inner Join DifesaTestata dt on p.DFT_Cod = dt.DFT_Cod   ")
            StrSQLFrom.AppendLine(" inner Join RegolamentixDifesaTestata rdt on rdt.DFT_Cod = dt.DFT_Cod  ")
            '(23/04/2019 fede) i fitoregolatori non hanno il record nella tabella infestanti
            If TipoTestata = 2 Then
                StrSQLFrom.AppendLine(" left outer Join Infestanti i on dr.DFT_Cod = i.DFT_Cod  And  dr.DFR_Cod =i.DFR_Cod  ")
            Else
                StrSQLFrom.AppendLine(" inner Join Infestanti i on i.DFT_Cod = dr.DFT_Cod  And    i.DFR_Cod = dr.DFR_Cod  ")
            End If

            StrSQLFromxDeroghe.AppendLine(" inner Join derogheterritoriali der  on der.der_cod=l.derogariferimento    ")
            StrSQLFromxDeroghe.AppendLine(" inner Join derogheterritorialixenti dere on dere.der_cod=der.der_cod   ")
            StrSQLFromxDeroghe.AppendLine(" inner Join enti e on e.idente=dere.IDEnte  ")


            StrSQLWhere.AppendLine(" WHERE  l.Flag_Controllo = 1 ")
            StrSQLWhere.AppendLine(" And    p.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND rdt.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQLWhere.AppendLine(" AND dt.Id_RcDpi =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQLWhere.AppendLine(" AND dt.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND p.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND p.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQLWhere.AppendLine(" AND i.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQLWhere.AppendLine(" AND dr.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQLWhere.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Cod > 0 Then
                    StrSQLWhere.AppendLine(" AND i.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

                If Av_Gru > 0 Then
                    StrSQLWhere.AppendLine(" AND i.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

            End If

            If Id_PAA <> 0 Then
                StrSQLWhere.AppendLine(" AND l.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ge_Paa_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND l.Ge_Paa_Cod =  " & Agro_SQL_SaveNum(Ge_Paa_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQLWhere.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQLWherexDeroghe1.AppendLine(" AND  l.deroga = 0 ")

            StrSQLWherexDeroghe2.AppendLine(" AND  l.deroga = 1 ")
            StrSQLWherexDeroghe2.AppendLine(" AND  e.pro_istat='000' ")
            StrSQLWherexDeroghe2.AppendLine(" AND  e.com_istat='000' ")
            StrSQLWherexDeroghe2.AppendLine(" AND  (der.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR der.Validadal is null) ")
            StrSQLWherexDeroghe2.AppendLine(" AND  (der.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR der.Validaal is null)  ")




            'Costruzione Query Finale
            StrSQLTot.Length = 0

            StrSQLTot.AppendLine(" ( SELECT Distinct l.*  " & StrSQLFrom.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe1.ToString & " ) UNION ALL ( SELECT Distinct l.* From LimitazioniUso l Where l.Ge_PAA_Cod IN (Select l.Ge_PAA_Cod " & StrSQLFrom.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe1.ToString & " AND GE_PAA_Cod is not null ) ) ")

            StrSQLTot.AppendLine(" UNION ")

            StrSQLTot.AppendLine(" ( SELECT Distinct l.*  " & StrSQLFrom.ToString & StrSQLFromxDeroghe.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe2.ToString & " ) UNION ALL ( SELECT Distinct l.* From LimitazioniUso l Where l.Ge_PAA_Cod IN (Select l.Ge_PAA_Cod " & StrSQLFrom.ToString & StrSQLFromxDeroghe.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe2.ToString & " AND GE_PAA_Cod is not null ) ) ")

            If strListaComuni <> "" And strListaComuni <> "000000" Then

                StrSQLTot.AppendLine(" UNION ")

                Dim FiltroEnti As String = ""
                Dim ArrayComuni() As String = Split(strListaComuni, ",")
                If Not ArrayComuni Is Nothing Then
                    For c = 0 To ArrayComuni.Length - 1
                        FiltroEnti &= " AND ((e.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and e.com_istat='000') " &
                                      "      OR (e.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and e.com_istat='" & ArrayComuni(c).Substring(3, 3) & "'))"
                    Next
                End If

                StrSQLWherexDeroghe3.AppendLine(" AND  l.deroga = 1 ")

                StrSQLWherexDeroghe3.AppendLine(" AND  (der.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR der.Validadal is null) ")
                StrSQLWherexDeroghe3.AppendLine(" AND  (der.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR der.Validaal is null)  ")

                StrSQLWherexDeroghe3.AppendLine("  " & FiltroEnti)

                StrSQLTot.AppendLine(" ( SELECT Distinct l.*  " & StrSQLFrom.ToString & StrSQLFromxDeroghe.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe3.ToString & " ) UNION ALL ( SELECT Distinct l.* From LimitazioniUso l Where l.Ge_PAA_Cod IN (Select l.Ge_PAA_Cod " & StrSQLFrom.ToString & StrSQLFromxDeroghe.ToString & StrSQLWhere.ToString & StrSQLWherexDeroghe3.ToString & " AND GE_PAA_Cod is not null ) ) ")


            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQLTot.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQLTot.AppendLine(" ORDER BY l.Id_Lu, l.ID_PAA ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQLTot.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    'a differenza della precedente considera le deroghe e le relative date
    Public Function Leggi_LimitazioniUsoxFormulati_Deroghe(ByVal ID_Lu As Int32,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUsoxFormulati_Deroghe()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" Select * From LimitazioniUsoxFormulati ")
            StrSQL.AppendLine(" WHERE  LimitazioniUsoxFormulati.Id_LU = " & Agro_SQL_SaveNum(ID_Lu) & "  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Id_Lu, Fr_Cod ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    Public Function Leggi_LimitazioniUso_Localizzazione(ByVal Disciplinare_Cod As Int32,
                                                        ByVal Fr_Cod As Int32,
                                                        ByVal Id_RcDpi As Int32,
                                                        ByVal TipoTestata As Int32,
                                                        ByVal strPa_Cod As String,
                                                        ByVal Gru_Pa_Cod As Int32,
                                                        ByVal Id_GaDPI As Int32,
                                                        ByVal Av_Gru As Int32,
                                                        ByVal Av_Cod As Int32,
                                                        ByVal Id_PAA As Int32,
                                                        ByVal Ge_Paa_Cod As Int32,
                                                        ByVal strAvversita As String,
                                                        ByVal Data As Date,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUso_Localizzazione()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StrSQLTot As New System.Text.StringBuilder
        Dim StrSQLFrom As New System.Text.StringBuilder
        Dim StrSQLFromxDeroghe As New System.Text.StringBuilder
        Dim StrSQLWhere As New System.Text.StringBuilder

        Try

            StrSQLFrom.Length = 0
            StrSQLWhere.Length = 0

            StrSQLFrom.AppendLine(" FROM  LimitazioniUso l ")
            StrSQLFrom.AppendLine(" left outer Join TipiLocalizzazione on (l.tl_cod = TipiLocalizzazione.tl_cod)  ")
            StrSQLFrom.AppendLine(" inner Join PA_Ausiliari p on l.ID_PAA = p.ID_PAA ")
            StrSQLFrom.AppendLine(" inner Join DifesaRighe dr on p.DFT_Cod = dr.DFT_Cod  And    p.DFR_Cod = dr.DFR_Cod   ")
            StrSQLFrom.AppendLine(" inner Join DifesaTestata dt on p.DFT_Cod = dt.DFT_Cod   ")
            StrSQLFrom.AppendLine(" inner Join RegolamentixDifesaTestata rdt on rdt.DFT_Cod = dt.DFT_Cod  ")

            If TipoTestata = 2 Then
                StrSQLFrom.AppendLine(" left outer Join Infestanti i on dr.DFT_Cod = i.DFT_Cod  And  dr.DFR_Cod =i.DFR_Cod  ")
            Else
                StrSQLFrom.AppendLine(" inner Join Infestanti i on i.DFT_Cod = dr.DFT_Cod  And    i.DFR_Cod = dr.DFR_Cod  ")
            End If

            StrSQLFrom.AppendLine(" inner Join formulatixprincipiattivi on (formulatixprincipiattivi.pa_cod = p.pa_cod)  ")

            StrSQLWhere.AppendLine(" WHERE  l.Flag_Controllo = 1 ")
            StrSQLWhere.AppendLine(" And    p.PASenzaControlli =  0  " & "  ")

            'FIltro le localizzazioni valorizzate
            StrSQLWhere.AppendLine(" And (l.tl_cod <> 0 or l.flag_localizzato = 1)  ")

            If Disciplinare_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND rdt.Cod_Regolamento =  " & Math.Abs(Disciplinare_Cod) & "  ")
            End If

            If Fr_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND formulatixprincipiattivi.Fr_Cod =  " & Agro_SQL_SaveNum(Fr_Cod) & "  ")
            End If

            If Id_RcDpi <> 0 Then
                StrSQLWhere.AppendLine(" AND dt.Id_RcDpi =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQLWhere.AppendLine(" AND dt.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Trim(strPa_Cod) <> "" Then
                StrSQLWhere.AppendLine(" AND p.PA_Cod In (" & Agro_SQL_SaveText(strPa_Cod) & ")  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND p.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQLWhere.AppendLine(" AND i.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If


            If Trim(strAvversita) <> "" Then

                StrSQLWhere.AppendLine(" AND " & strAvversita)

            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Cod > 0 Then
                    StrSQLWhere.AppendLine(" AND i.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

                If Av_Gru > 0 Then
                    StrSQLWhere.AppendLine(" AND i.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

            End If

            If Id_PAA <> 0 Then
                StrSQLWhere.AppendLine(" AND l.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If

            If Ge_Paa_Cod <> 0 Then
                StrSQLWhere.AppendLine(" AND l.Ge_Paa_Cod =  " & Agro_SQL_SaveNum(Ge_Paa_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQLWhere.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            'Costruzione Query Finale
            StrSQLTot.Length = 0

            Dim strSelect As String = " Select Distinct isnull(TipiLocalizzazione.tl_cod, 0) as TL_Cod, Flag_Localizzato, Case When l.flag_localizzato = 1 Then 'Localizzato' Else isnull(descrizione, '') End AS TL_DES "

            StrSQLTot.AppendLine(" ( " & strSelect & " " & StrSQLFrom.ToString & StrSQLWhere.ToString & " ) ")

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQLTot.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQLTot.AppendLine(" ORDER BY  tl_cod Desc, Flag_Localizzato Desc ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQLTot.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################
    Public Function Leggi_Miscela(ByVal Id_RcDpi As Int32,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal Id_Paa_Misc As Int32,
                                            ByVal TipoTestata As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal Pa_Cod As Int32,
                                            ByVal Gru_Pa_Cod As Int32,
                                            ByVal Modulo As Int32,
                                            ByVal Ep_Cod As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Miscela()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.* " &
                          " FROM   DifesaTestata, DifesaRighe, Infestanti, RegolamentixDifesaRighe, PA_Ausiliari " &
                          " WHERE  DifesaTestata.DFT_Cod = Infestanti.DFT_Cod  " &
                          " And    DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                          " And    Infestanti.DFT_Cod = RegolamentixDifesaRighe.DFT_Cod  " &
                          " And    Infestanti.DFR_Cod = RegolamentixDifesaRighe.DFR_Cod  " &
                          " And    Infestanti.DFT_Cod = PA_Ausiliari.DFT_Cod  " &
                          " And    Infestanti.DFR_Cod = PA_Ausiliari.DFR_Cod  " &
                          " And    Infestanti.DFT_Cod = DifesaRighe.DFT_Cod  " &
                          " And    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod  " &
                          " And    PA_Ausiliari.PASenzaControlli =  0  " & "  " &
                          " And    Pa_Ausiliari.Id_Paa =  " & Agro_SQL_SaveNum(Id_Paa_Misc) & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" And RegolamentixDifesaRighe.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" And Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Av_Gru <> 0 Then
                StrSQL.AppendLine(" And Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.AppendLine(" And Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" And DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" And Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" And Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" And DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" And DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" And DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Pa_Ausiliari.Pa_Cod Asc")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    Public Function Leggi_NotexEpochexRighe(ByVal DFT_Cod As Int32,
                                            ByVal Dfr_Cod As Int32,
                                            ByVal IEp_Cod As Int32,
                                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_NotexEpochexRighe()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct NotexEpochexDifesaRighe.* " &
                        " FROM   NotexEpochexDifesaRighe " &
                        " Where  NotexEpochexDifesaRighe.DFT_Cod = " & Agro_SQL_SaveNum(DFT_Cod) & "  " &
                        " And    NotexEpochexDifesaRighe.DFR_Cod = " & Agro_SQL_SaveNum(Dfr_Cod))

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ID_NED, NotaNum  ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    Public Function Leggi_NotexRighexVincoli(ByVal Dfr_Cod As Int32,
                                             ByVal IDVincolo As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_NotexRighexVincoli()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct NotexDifesaRighexVincoli.* " &
                            " FROM   NotexDifesaRighexVincoli " &
                            " Where  NotexDifesaRighexVincoli.DFR_Cod = " & Agro_SQL_SaveNum(Dfr_Cod))


            If IDVincolo <> 0 Then
                StrSQL.AppendLine(" And NotexDifesaRighexVincoli.IDVincolo =  " & Agro_SQL_SaveNum(IDVincolo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ID_Nota, NotaNum ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    Public Function Leggi_NotexTestataxVincoli(ByVal Dft_Cod As Int32,
                                               ByVal IDVincolo As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_NotexTestataxVincoli()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("  SELECT Distinct NotexDifesaTestataxVincoli.* " &
                          " FROM   NotexDifesaTestataxVincoli " &
                          " Where  NotexDifesaTestataxVincoli.DFT_Cod = " & Agro_SQL_SaveNum(Dft_Cod))


            If IDVincolo <> 0 Then
                StrSQL.AppendLine(" And NotexDifesaTestataxVincoli.IDVincolo =  " & Agro_SQL_SaveNum(IDVincolo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ID_Nota, NotaNum ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################



    Public Function Leggi_PrincipiAttivi(ByVal Dft_Cod As Int32,
                                            ByVal Dfr_Cod As Int32,
                                            ByVal Pa_Cod As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("  SELECT Distinct PA_Ausiliari.* " &
                        " FROM   PA_Ausiliari " &
                        " Where  PA_Ausiliari.DFT_COD = " & Agro_SQL_SaveNum(Dft_Cod) &
                        " And    PA_Ausiliari.DFR_COD = " & Agro_SQL_SaveNum(Dfr_Cod) &
                        " And    PA_Ausiliari.PASenzaControlli =  0  " & "  ")


            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" And PA_Ausiliari.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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



    Public Function Leggi_PrincipiAttividaLimitazioniUso_Da_GPAI(ByVal Gpai_PA_Ausiliari_Cod As Int32,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByVal xOrderBy As String,
                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttividaLimitazioniUso_Da_GPAI()"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.Length = 0


            StrSQL.AppendLine(" Select Distinct Principiattivi.Pa_Cod, Principiattivi.Pa_Des, GPAI_PA_Ausiliari_Des ")
            StrSQL.AppendLine(" From Principiattivi, PA_Ausiliari, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")
            StrSQL.AppendLine(" Where Principiattivi.Pa_Cod = PA_Ausiliari.Pa_Cod ")
            StrSQL.AppendLine(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.Id_paa = PA_Ausiliari.Id_paa ")
            StrSQL.AppendLine(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.Gpai_PA_Ausiliari_Cod = GruppiPAInterazioni_PA_Ausiliari.Gpai_PA_Ausiliari_Cod ")
            StrSQL.AppendLine(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.Gpai_PA_Ausiliari_Cod = " & Agro_SQL_SaveNum(Gpai_PA_Ausiliari_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Pa_Cod ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    Public Function Leggi_PrincipiAttivi_codDescrizioni(ByVal Dft_Cod As Int32,
                                                        ByVal Dfr_Cod As Int32,
                                                        ByVal Pa_Cod As Int32,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi_codDescrizioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PA_Ausiliari.*, ISNULL(PrincipiAttivi.PA_DES,'') AS PA_DES, ISNULL(GruppiPrincipiAttivi.GPA_DES,'') AS GPA_DES " &
                          " FROM   PA_Ausiliari LEFT OUTER JOIN " &
                          " GruppiPrincipiAttivi ON PA_Ausiliari.GRU_PA_COD = GruppiPrincipiAttivi.GPA_COD LEFT OUTER JOIN " &
                          " PrincipiAttivi ON PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD  " &
                          " WHERE 1=1 ")

            If Dft_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Ausiliari.DFT_COD =  " & Agro_SQL_SaveNum(Dft_Cod) & "  ")
            End If

            If Dfr_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Ausiliari.Dfr_Cod =  " & Agro_SQL_SaveNum(Dfr_Cod) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PA_Ausiliari.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_PrincipiAttivi_Da_Infestanti(ByVal Id_RcDpi As Int32,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal TipoTestata As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal strAvversita As String,
                                            ByVal Ep_Cod As Int32,
                                            ByVal Modulo As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi_Da_Infestanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.* ")

            StrSQL.AppendLine(" FROM   RegolamentiXDifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaRighe ON DifesaTestata.DFT_COD = DifesaRighe.DFT_COD ON  ")
            StrSQL.AppendLine(" RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" PA_Ausiliari ON DifesaRighe.DFR_COD = PA_Ausiliari.DFR_COD AND DifesaRighe.DFT_COD = PA_Ausiliari.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" Infestanti ON DifesaRighe.DFR_COD = Infestanti.DFR_COD AND DifesaRighe.DFT_COD = Infestanti.DFT_COD ")

            StrSQL.AppendLine("  WHERE  DifesaTestata.ID_RCDPI = " & Agro_SQL_SaveNum(Id_RcDpi) &
                           " AND    DifesaTestata.TipoTestata = " & Agro_SQL_SaveNum(TipoTestata) &
                           " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If


            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If
            End If


            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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

    'introdotta gestione deroghe
    Public Function Leggi_PrincipiAttivi_Da_Infestanti_Deroghe(ByVal Id_RcDpi As Int32,
                                                               ByVal Disciplinare_Cod As Int32,
                                                               ByVal TipoTestata As Int32,
                                                               ByVal Id_GaDPI As Int32,
                                                               ByVal Av_Gru As Int32,
                                                               ByVal Av_Cod As Int32,
                                                               ByVal strAvversita As String,
                                                               ByVal Ep_Cod As Int32,
                                                               ByVal Modulo As Int32,
                                                               ByVal Data As Date,
                                                               ByVal strListaComuni As String,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByVal xOrderBy As String,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                               Optional ByVal Stato_Impianto As Integer = 0
                                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi_Da_Infestanti_Deroghe()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            '--------------------------------------------------------------------------
            ' SOSTANZE SENZA DEROGHE
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.*, IsNull(DifesaRighe.ID_FP, 0) as ID_FP ")

            StrSQL.AppendLine(" FROM   RegolamentiXDifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaRighe ON DifesaTestata.DFT_COD = DifesaRighe.DFT_COD ON  ")
            StrSQL.AppendLine(" RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" PA_Ausiliari ON DifesaRighe.DFR_COD = PA_Ausiliari.DFR_COD AND DifesaRighe.DFT_COD = PA_Ausiliari.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" Infestanti ON DifesaRighe.DFR_COD = Infestanti.DFR_COD AND DifesaRighe.DFT_COD = Infestanti.DFT_COD ")

            StrSQL.AppendLine("  WHERE  DifesaTestata.ID_RCDPI = " & Agro_SQL_SaveNum(Id_RcDpi) &
                           " AND    DifesaTestata.TipoTestata = " & Agro_SQL_SaveNum(TipoTestata) &
                           " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            '=============================================================================================
            'Marco 02/05/2022 Filtro Stato_Impianto
            '---------------------------------------------------------------------------------------------
            Select Case Stato_Impianto

                Case 0 'Nessun vincolo 

                Case -1 'Stati Impianti Eterogenei

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp = 0 ) ")

                Case 102 'Produzione

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 2)) ")

                Case Else 'Allevamento

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 1)) ")

            End Select
            '=============================================================================================



            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                If Av_Gru > 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod > 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If
            End If


            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If

            StrSQL.AppendLine(" AND PA_Ausiliari.deroga = 0 ")



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            ' DEROGHE SU TUTTA LA TESTATA (regione)
            StrSQL.AppendLine(" UNION  ")

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.*,  IsNull(DifesaRighe.ID_FP, 0) as ID_FP ")

            StrSQL.AppendLine(" FROM   RegolamentiXDifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaRighe ON DifesaTestata.DFT_COD = DifesaRighe.DFT_COD ON  ")
            StrSQL.AppendLine(" RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" PA_Ausiliari ON DifesaRighe.DFR_COD = PA_Ausiliari.DFR_COD AND DifesaRighe.DFT_COD = PA_Ausiliari.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" Infestanti ON DifesaRighe.DFR_COD = Infestanti.DFR_COD AND DifesaRighe.DFT_COD = Infestanti.DFT_COD ")

            StrSQL.AppendLine(" inner Join derogheterritoriali  on derogheterritoriali.der_cod=pa_ausiliari.derogariferimento ")
            StrSQL.AppendLine(" inner Join derogheterritorialixenti  on derogheterritorialixenti.der_cod=derogheterritoriali.der_cod ")
            StrSQL.AppendLine(" inner Join enti  on enti.idente=derogheterritorialixenti.IDEnte ")

            StrSQL.AppendLine("  WHERE  DifesaTestata.ID_RCDPI = " & Agro_SQL_SaveNum(Id_RcDpi) &
                           " And    DifesaTestata.TipoTestata = " & Agro_SQL_SaveNum(TipoTestata) &
                           " And    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" And RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" And Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            '=============================================================================================
            'Marco 02/05/2022 Filtro Stato_Impianto
            '---------------------------------------------------------------------------------------------
            Select Case Stato_Impianto

                Case 0 'Nessun vincolo 

                Case -1 'Stati Impianti Eterogenei

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp = 0 ) ")

                Case 102 'Produzione

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 2)) ")

                Case Else 'Allevamento

                    StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 1)) ")

            End Select
            '=============================================================================================

            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" And " & strAvversita)

            Else
                If Av_Gru > 0 Then
                    StrSQL.AppendLine(" And Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod > 0 Then
                    StrSQL.AppendLine(" And Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If
            End If


            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" And DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" And DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" And DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If

            StrSQL.AppendLine(" And PA_Ausiliari.deroga = 1 ")
            StrSQL.AppendLine(" And enti.pro_istat='000' ")
            StrSQL.AppendLine(" AND enti.com_istat='000' ")
            StrSQL.AppendLine(" AND (derogheterritoriali.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validadal is null) ")
            StrSQL.AppendLine(" AND (derogheterritoriali.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validaal is null)  ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            ' DEROGHE SU PROVINCE COMUNI

            If strListaComuni <> "" And strListaComuni <> "000000" Then

                StrSQL.AppendLine(" UNION  ")

                StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.*, IsNull(DifesaRighe.ID_FP, 0) as ID_FP ")

                StrSQL.AppendLine(" FROM   RegolamentiXDifesaTestata INNER JOIN  ")
                StrSQL.AppendLine(" DifesaTestata INNER JOIN  ")
                StrSQL.AppendLine(" DifesaRighe ON DifesaTestata.DFT_COD = DifesaRighe.DFT_COD ON  ")
                StrSQL.AppendLine(" RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD LEFT OUTER JOIN  ")
                StrSQL.AppendLine(" PA_Ausiliari ON DifesaRighe.DFR_COD = PA_Ausiliari.DFR_COD AND DifesaRighe.DFT_COD = PA_Ausiliari.DFT_COD LEFT OUTER JOIN  ")
                StrSQL.AppendLine(" Infestanti ON DifesaRighe.DFR_COD = Infestanti.DFR_COD AND DifesaRighe.DFT_COD = Infestanti.DFT_COD ")

                StrSQL.AppendLine(" inner Join derogheterritoriali  on derogheterritoriali.der_cod=pa_ausiliari.derogariferimento ")
                StrSQL.AppendLine(" inner Join derogheterritorialixenti  on derogheterritorialixenti.der_cod=derogheterritoriali.der_cod ")
                StrSQL.AppendLine(" inner Join enti  on enti.idente=derogheterritorialixenti.IDEnte ")

                StrSQL.AppendLine("  WHERE  DifesaTestata.ID_RCDPI = " & Agro_SQL_SaveNum(Id_RcDpi) &
                           " AND    DifesaTestata.TipoTestata = " & Agro_SQL_SaveNum(TipoTestata) &
                           " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

                If Disciplinare_Cod <> 0 Then
                    StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
                End If

                If Id_GaDPI <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
                End If
                '=============================================================================================
                'Marco 02/05/2022 Filtro Stato_Impianto
                '---------------------------------------------------------------------------------------------
                Select Case Stato_Impianto

                    Case 0 'Nessun vincolo 

                    Case -1 'Stati Impianti Eterogenei

                        StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp = 0 ) ")

                    Case 102 'Produzione

                        StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 2)) ")

                    Case Else 'Allevamento

                        StrSQL.AppendLine("  And (difesarighe.id_fp Is null Or difesarighe.id_fp In (0, 1)) ")

                End Select
                '=============================================================================================

                If Trim(strAvversita) <> "" Then
                    StrSQL.AppendLine(" AND " & strAvversita)

                Else
                    If Av_Gru > 0 Then
                        StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                    End If

                    If Av_Cod > 0 Then
                        StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                    End If
                End If


                If Ep_Cod <> 0 Then
                    StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                    StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                End If

                If Modulo <> 0 Then
                    StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
                End If

                StrSQL.AppendLine(" AND PA_Ausiliari.deroga = 1 ")
                StrSQL.AppendLine(" AND (derogheterritoriali.Validadal <=  " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validadal is null) ")
                StrSQL.AppendLine(" AND (derogheterritoriali.Validaal >= " & Agro_SQL_SaveDate(Data) & "  OR derogheterritoriali.Validaal is null)  ")

                Dim FiltroEnti As String = ""
                Dim ArrayComuni() As String = Split(strListaComuni, ",")
                If Not ArrayComuni Is Nothing Then
                    For c = 0 To ArrayComuni.Length - 1
                        FiltroEnti &= " AND ((enti.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and enti.com_istat='000') " &
                                      "      OR (enti.pro_istat='" & ArrayComuni(c).Substring(0, 3) & "' and enti.com_istat='" & ArrayComuni(c).Substring(3, 3) & "'))"
                    Next
                End If
                StrSQL.AppendLine("  " & FiltroEnti)


            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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

    'a differenza della precedente 
    '- Id_RcDpi è una stringa di valori
    '- La testata non è obbligatoria (TipoTestata=-1)
    Public Function Leggi_PrincipiAttivi_Da_Infestanti_2(ByVal Id_RcDpi As String,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal TipoTestata As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal strAvversita As String,
                                            ByVal Ep_Cod As Int32,
                                            ByVal Modulo As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi_Da_Infestanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.* ")

            StrSQL.AppendLine(" FROM   RegolamentiXDifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaTestata INNER JOIN  ")
            StrSQL.AppendLine(" DifesaRighe ON DifesaTestata.DFT_COD = DifesaRighe.DFT_COD ON  ")
            StrSQL.AppendLine(" RegolamentiXDifesaTestata.DFT_COD = DifesaTestata.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" PA_Ausiliari ON DifesaRighe.DFR_COD = PA_Ausiliari.DFR_COD AND DifesaRighe.DFT_COD = PA_Ausiliari.DFT_COD LEFT OUTER JOIN  ")
            StrSQL.AppendLine(" Infestanti ON DifesaRighe.DFR_COD = Infestanti.DFR_COD AND DifesaRighe.DFT_COD = Infestanti.DFT_COD ")

            StrSQL.AppendLine("  WHERE  DifesaTestata.ID_RCDPI IN (" & Agro_SQL_SaveText(Id_RcDpi) & ")")

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND    DifesaTestata.TipoTestata = " & Agro_SQL_SaveNum(TipoTestata))
            End If

            StrSQL.AppendLine(" AND    PA_Ausiliari.PASenzaControlli =  0")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If


            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)

            Else
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If
            End If


            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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




    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    '============================================================================
    'Lettura del Raggruppamento Colturale DPI
    '============================================================================
    Public Function Leggi_RaggruppamentiDPI(ByVal Id_RcDpi As Int32,
                                            ByVal Veg_Cod As Int32,
                                            ByVal Grfi_Cod As Int32,
                                            ByVal Flag_Protetto As Int32,
                                            ByVal Cod_Regolamento As Int32,
                                            ByVal TipoTestata As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_RaggruppamentiDPI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Leggi_RaggruppamentiDPI = Leggi_RaggruppamentiDPI2(Id_RcDpi,
                                                         Veg_Cod,
                                                         Grfi_Cod,
                                                         Flag_Protetto,
                                                         Cod_Regolamento,
                                                         TipoTestata,
                                                         xFiltroAggiuntivo,
                                                         xOrderBy,
                                                         objParametri)


            If Leggi_RaggruppamentiDPI.Rows.Count = 0 AndAlso Flag_Protetto = 1 Then

                Leggi_RaggruppamentiDPI = Leggi_RaggruppamentiDPI2(Id_RcDpi,
                                                      Veg_Cod,
                                                      Grfi_Cod,
                                                      -1,
                                                      Cod_Regolamento,
                                                      TipoTestata,
                                                      xFiltroAggiuntivo,
                                                      xOrderBy,
                                                      objParametri)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Leggi_RaggruppamentiDPI


    End Function



    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Leggi_RaggruppamentiDPI2(ByVal Id_RcDpi As Int32,
                                            ByVal Veg_Cod As Int32,
                                            ByVal Grfi_Cod As Int32,
                                            ByVal Flag_Protetto As Int32,
                                            ByVal Cod_Regolamento As Int32,
                                            ByVal TipoTestata As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_RaggruppamentiDPI2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("  SELECT Distinct RaggruppamentiColturaliDPI.*, RaggruppamentiDPIxSpecieVegetali.*  " &
                        " FROM   RaggruppamentiDPIxSpecieVegetali, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIxRegolamenti, " &
                        "        DifesaTestata, DifesaRighe, SpecieVegetali, RegolamentixDifesaTestata " &
                        " WHERE  RaggruppamentiDPIxSpecieVegetali.Id_RCDPI = RaggruppamentiColturaliDPI.Id_RCDPI  " &
                        " AND    RaggruppamentiColturaliDPIxRegolamenti.Id_RcDPi = RaggruppamentiColturaliDPI.Id_RcDPI " &
                        " AND    RaggruppamentiColturaliDPIxRegolamenti.Id_RcDPi = DifesaTestata.Id_RcDPI " &
                        " AND    RaggruppamentiDPIxSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod  " &
                        " AND    DifesaRighe.Dft_Cod = DifesaTestata.Dft_Cod " &
                        " AND    RegolamentixDifesaTestata.Cod_Regolamento = RaggruppamentiColturaliDPIxRegolamenti.Cod_Regolamento " &
                        " AND    RegolamentixDifesaTestata.Dft_Cod = DifesaTestata.Dft_Cod ")
            '" AND    RaggruppamentiDPIxSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod_AUX  " & _

            If Id_RcDpi <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            Select Case Grfi_Cod

                Case 0 'Do nothing

                    If Flag_Protetto <> -1 Then
                        StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Flag_Protetto = " & Agro_SQL_SaveNum(Flag_Protetto))
                    End If

                Case 8
                    'Lettura dei raggruppamenti con finalità = "coltura protetta" (bypass del grfi_cod)
                    StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Flag_Protetto =  1")

                Case Else
                    'Lettura dei raggruppamenti con finalità <> "coltura protetta"
                    StrSQL.AppendLine(" AND ( RaggruppamentiDPIxSpecieVegetali.Grfi_Cod =  " & Agro_SQL_SaveNum(Grfi_Cod) & " OR RaggruppamentiDPIxSpecieVegetali.Grfi_Cod = 0 ) ")

                    Select Case Flag_Protetto
                        Case -1, 0, 2
                            StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Flag_Protetto <> 1")

                        Case Else 'Coltura Protetta
                            StrSQL.AppendLine(" AND RaggruppamentiDPIxSpecieVegetali.Flag_Protetto = 1")

                    End Select

            End Select

            If Cod_Regolamento <> 0 Then
                StrSQL.AppendLine(" AND RaggruppamentiColturaliDPIxRegolamenti.Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Regolamento) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            Else
                'sSql = sSql & " AND Not RaggruppamentiColturaliDPI.Id_Padre is Null "
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY RaggruppamentiColturaliDPI.Nome ASC ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################

    Public Function Leggi_SoglieIntervento(ByVal Id_RcDpi As Int32,
                                        ByVal Disciplinare_Cod As Int32,
                                        ByVal Id_GaDPI As Int32,
                                        ByVal Av_Gru As Int32,
                                        ByVal Av_Cod As Int32,
                                        ByVal Pa_Cod As Int32,
                                        ByVal Gru_Pa_Cod As Int32,
                                        ByVal strAvversita As String,
                                        ByVal Modulo As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_SoglieIntervento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct SoglieIntervento.*,  Infestanti.Av_Cod, Infestanti.Av_Gru " &
                        " FROM   SoglieIntervento, DifesaTestata, DifesaRighe, Infestanti, RegolamentiXDifesaTestata, PA_Ausiliari, CriteriIntervento " &
                        " WHERE  DifesaTestata.DFT_Cod = Infestanti.DFT_Cod  " &
                        " AND    DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                        " AND    Infestanti.DFT_Cod = RegolamentiXDifesaTestata.DFT_Cod  " &
                        " AND    Infestanti.DFT_Cod = PA_Ausiliari.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = PA_Ausiliari.DFR_Cod  " &
                        " AND    Infestanti.DFT_Cod = DifesaRighe.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    SoglieIntervento.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    SoglieIntervento.DFR_Cod = CriteriIntervento.DFR_Cod  " &
                        " AND    CriteriIntervento.Flag_Ammesso = 1  " &
                        " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  " &
                        " AND    DifesaTestata.TipoTestata = 0 ")


            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)
            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If

            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY SoglieIntervento.Dfr_Cod ASC ")
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

    Public Function Leggi_SogliaIntervento(ByVal Si_Cod As Int32,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_SogliaIntervento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct SoglieIntervento.*,  Infestanti.Av_Cod, Infestanti.Av_Gru " &
                        " FROM   SoglieIntervento, DifesaRighe, Infestanti " &
                        " WHERE  Infestanti.DFT_Cod = DifesaRighe.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    SoglieIntervento.DFR_Cod = DifesaRighe.DFR_Cod  ")

            StrSQL.AppendLine(" AND SoglieIntervento.Si_Cod =  " & Agro_SQL_SaveNum(Si_Cod) & "  ")

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


    ''#############################################################################################################
    ''#############################################################################################################
    ''#############################################################################################################


    '============================================================================
    'Lettura delle Soglie Intervento + Criteri di Intervento
    '============================================================================

    Public Function Leggi_SoglieInterventoxConsultazione(ByVal Id_RcDpi As Int32,
                                            ByVal Disciplinare_Cod As Int32,
                                            ByVal Id_GaDPI As Int32,
                                            ByVal Av_Gru As Int32,
                                            ByVal Av_Cod As Int32,
                                            ByVal Pa_Cod As Int32,
                                            ByVal Gru_Pa_Cod As Int32,
                                            ByVal strAvversita As String,
                                            ByVal Modulo As Int32,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_SoglieInterventoxConsultazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct SoglieIntervento.*,  CriteriIntervento.Descrizione, Infestanti.Av_Cod, Infestanti.Av_Gru " &
                        " FROM   SoglieIntervento, DifesaTestata, DifesaRighe, Infestanti, RegolamentiXDifesaTestata, PA_Ausiliari, CriteriIntervento " &
                        " WHERE  DifesaTestata.DFT_Cod = Infestanti.DFT_Cod  " &
                        " AND    DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                        " AND    Infestanti.DFT_Cod = RegolamentiXDifesaTestata.DFT_Cod  " &
                        " AND    Infestanti.DFT_Cod = PA_Ausiliari.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = PA_Ausiliari.DFR_Cod  " &
                        " AND    Infestanti.DFT_Cod = DifesaRighe.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    SoglieIntervento.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    SoglieIntervento.DFR_Cod = CriteriIntervento.DFR_Cod  " &
                        " AND    CriteriIntervento.Flag_Ammesso = 1  " &
                        " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  " &
                        " AND    DifesaTestata.TipoTestata = 0 ")


            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentiXDifesaTestata.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)
            Else
                'Posso Applicare il filtro sulle singole avversità
                If Av_Gru <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
                End If

                If Av_Cod <> 0 Then
                    StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SoglieIntervento.Dfr_Cod ASC")
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
    'Verifica se il principio attivo è valido
    '============================================================================

    Public Function Verifica_PrincipioAttivo(ByVal Id_PAA As Int32,
                                            ByVal Reg_Cod As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Verifica_PrincipioAttivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct AttivitaRegolamentiXVincoli.* " &
                        " FROM   AttivitaRegolamentiXVincoli, Regolamenti, VincolixPAAusiliari " &
                        " WHERE  Regolamenti.Reg_COD_Agronica = " & Reg_Cod &
                        " AND    Regolamenti.Cod_Regolamento = AttivitaRegolamentiXVincoli.Cod_Regolamento " &
                        " AND    VincolixPAAusiliari.IdVincolo = AttivitaRegolamentiXVincoli.IdVincolo " &
                        " AND    VincolixPAAusiliari.Id_Paa =  " & Id_PAA)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '============================================================================
    'Lettura delle Limitazioni Bloccate
    '============================================================================
    Public Function Leggi_LimitazioniUsoxLimitazioniUsoDaBloccare(ByVal Id_Lu As Int32,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUsoxLimitazioniUsoDaBloccare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct LimitazioniUsoxLimitazioniUsoDaBloccare.* " &
                        " FROM   LimitazioniUsoxLimitazioniUsoDaBloccare " &
                        " WHERE  LimitazioniUsoxLimitazioniUsoDaBloccare.Lu_Cod = " & Agro_SQL_SaveNum(Id_Lu))

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY LimitazioniUsoxLimitazioniUsoDaBloccare.Lu_Cod  ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function



    '============================================================================
    'Lettura delle Limitazioni legate alle Cultivar
    '============================================================================
    Public Function Leggi_LimitazioniUsoxCultivar(ByVal Id_Lu As Int32,
                                                  ByVal cul_cod As Int32,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LimitazioniUsoxCultivar()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * " &
                        " FROM   LimitazioniUsoXCultivar " &
                        " WHERE  ID_LU = " & Agro_SQL_SaveNum(Id_Lu))

            If cul_cod <> 0 Then
                StrSQL.AppendLine(" AND cul_cod = " & Agro_SQL_SaveNum(cul_cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY LimitazioniUsoXCultivar.cul_cod  ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '============================================================================
    'Lettura delle Cultivar Sensibili
    '============================================================================
    Public Function Leggi_Cultivar_Sensibili(ByVal Av_Cod As Int32,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Cultivar_Sensibili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct CultivarxAvversita.* " &
                            " FROM   CultivarxAvversita " &
                            " WHERE  CultivarxAvversita.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod))

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY CultivarxAvversita.Id_CA  ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '============================================================================
    'Lettura delle Note Trattamento
    '============================================================================
    Public Function Leggi_NoteTrattamento(ByVal NoteTrattamento_Cod As Int32,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_NoteTrattamento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct NoteTrattamento.* " &
                        " FROM   NoteTrattamento " &
                        " WHERE  NoteTrattamento.NoteTrattamento_Cod = " & Agro_SQL_SaveNum(NoteTrattamento_Cod))
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY NoteTrattamento.NoteTrattamento_Cod  ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '============================================================================
    'Lettura delle Linee Intervento
    '============================================================================
    Public Function Leggi_LineeIntervento(ByVal Ge_Paa_Cod As Int32,
                                          ByVal Id_PAA As Int32,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_LineeIntervento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct LimitazioniUso.LineaIntervento " &
                        " FROM   LimitazioniUso " &
                        " Where  LimitazioniUso.Ge_Paa_Cod =  " & Agro_SQL_SaveNum(Ge_Paa_Cod) & " ")

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" AND LimitazioniUso.Id_Paa =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY LimitazioniUso.LineaIntervento ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '============================================================================
    'Lettura degli Principi Attivi Da Gruppi Interazioni
    '============================================================================
    Public Function Leggi_PrincipiAttivi_Da_GPAI(ByVal Gpai_PA_Ausiliari_Cod As Int32,
                                                 ByVal Id_RcDpi As Int32,
                                                 ByVal TipoTestata As Int32,
                                                 ByVal Id_GaDPI As Int32,
                                                 ByVal Av_Gru As Int32,
                                                 ByVal Av_Cod As Int32,
                                                 ByVal strAvversita As String,
                                                 ByVal strId_Paa As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PrincipiAttivi_Da_GPAI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.PA_Cod, PA_Ausiliari.Id_PAA " &
                        " FROM   PA_Ausiliari, Infestanti, DifesaTestata, GruppiPAInterazioni_PA_AusiliarixPA_Ausiliari, LimitazioniUso " &
                        " WHERE  PA_Ausiliari.DFT_COD =  Infestanti.DFT_Cod " &
                        " AND    PA_Ausiliari.DFR_COD =  Infestanti.DFR_Cod " &
                        " AND    DifesaTestata.DFT_COD =  Infestanti.DFT_Cod " &
                        " AND    DifesaTestata.ID_RCDPI = " & Id_RcDpi &
                        " AND    DifesaTestata.TipoTestata = " & TipoTestata & "  " &
                        " AND    PA_Ausiliari.Id_PAA =  LimitazioniUso.Id_Paa  " & "  " &
                        " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  ")

            If Gpai_PA_Ausiliari_Cod <> 0 Then
                StrSQL.AppendLine(" AND GruppiPAInterazioni_PA_AusiliarixPA_Ausiliari.Gpai_PA_Ausiliari_Cod =  " & Agro_SQL_SaveNum(Gpai_PA_Ausiliari_Cod) &
                              " AND GruppiPAInterazioni_PA_AusiliarixPA_Ausiliari.Id_PAA = PA_Ausiliari.ID_PAA ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDPI =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Av_Gru <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If Trim(strAvversita) <> "" Then
                StrSQL.AppendLine(" AND " & strAvversita)
            End If

            If Trim(strId_Paa) <> "" Then
                StrSQL.AppendLine(" AND " & strId_Paa)
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            ' Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '============================================================================
    'Lettura dei dettagli sui principi attivi presenti una miscela
    '============================================================================
    Public Function Leggi_PA_Miscela(ByVal Id_RcDpi As Int32,
                                     ByVal Disciplinare_Cod As Int32,
                                     ByVal Id_Paa_Misc As Int32,
                                     ByVal TipoTestata As Int32,
                                     ByVal Id_GaDPI As Int32,
                                     ByVal Av_Gru As Int32,
                                     ByVal Av_Cod As Int32,
                                     ByVal Pa_Cod As Int32,
                                     ByVal Gru_Pa_Cod As Int32,
                                     ByVal Modulo As Int32,
                                     ByVal Ep_Cod As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PA_Miscela()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Distinct PA_Ausiliari.* " &
                        " FROM   DifesaTestata, DifesaRighe, Infestanti, RegolamentixDifesaRighe, PA_Ausiliari " &
                        " WHERE  DifesaTestata.DFT_Cod = Infestanti.DFT_Cod  " &
                        " AND    DifesaTestata.Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                        " AND    Infestanti.DFT_Cod = RegolamentixDifesaRighe.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = RegolamentixDifesaRighe.DFR_Cod  " &
                        " AND    Infestanti.DFT_Cod = PA_Ausiliari.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = PA_Ausiliari.DFR_Cod  " &
                        " AND    Infestanti.DFT_Cod = DifesaRighe.DFT_Cod  " &
                        " AND    Infestanti.DFR_Cod = DifesaRighe.DFR_Cod  " &
                        " AND    PA_Ausiliari.PASenzaControlli =  0  " & "  " &
                        " AND    Pa_Ausiliari.Id_Paa_Misc =  " & Agro_SQL_SaveNum(Id_Paa_Misc) & "  ")

            If Disciplinare_Cod <> 0 Then
                StrSQL.AppendLine(" AND RegolamentixDifesaRighe.Cod_Regolamento =  " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  ")
            End If

            If Id_GaDPI <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Id_GaDpi =  " & Agro_SQL_SaveNum(Id_GaDPI) & "  ")
            End If

            If Av_Gru <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Av_Gru =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.AppendLine(" AND Infestanti.Av_Cod =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If TipoTestata <> -1 Then
                StrSQL.AppendLine(" AND DifesaTestata.TipoTestata =  " & Agro_SQL_SaveNum(TipoTestata) & "  ")
            End If

            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Pa_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            If Gru_Pa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pa_Ausiliari.Gru_Pa_Cod =  " & Agro_SQL_SaveNum(Gru_Pa_Cod) & "  ")
            End If

            If Ep_Cod <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Da_Ep_Cod <=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
                StrSQL.AppendLine(" AND DifesaRighe.A_Ep_Cod >=  " & Agro_SQL_SaveNum(Ep_Cod) & "  ")
            End If

            If Modulo <> 0 Then
                StrSQL.AppendLine(" AND DifesaRighe.Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Pa_Ausiliari.Pa_Cod Asc")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            ' Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    'dati N-iaf_cod (iaf_cod1, iafcod2, ...)
    Public Function Leggi_ImpegniAggiuntiviFacoltativi(ByVal Iaf_Cod As String,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_ImpegniAggiuntiviFacoltativi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * " &
                        " FROM   ImpegniAggiuntiviFacoltativi " &
                        " WHERE  flag_bio = 0 ")


            If Iaf_Cod <> "" Then
                StrSQL.AppendLine(" AND Iaf_Cod IN " & Agro_SQL_Save_Clausola_IN(Iaf_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Iaf_Cod ")
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

    Public Function Leggi_VolumiIrrorazioneMax(ByVal Id_RcDpi As Int32,
                                        ByVal Disciplinare_Cod As Int32,
                                        ByVal Foral_Cod As String,
                                        ByVal FasiProduttive As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_VolumiIrrorazioneMax()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * " &
                        " FROM   VolumiIrrorazioneMax " &
                        " WHERE  Id_RCDPI =  " & Agro_SQL_SaveNum(Id_RcDpi) &
                        " AND    Cod_Regolamento  =" & Agro_SQL_SaveNum(Disciplinare_Cod))

            If Foral_Cod <> "" Then
                StrSQL.AppendLine(" AND Foral_Cod IN (" & Agro_SQL_Save_Clausola_IN(Foral_Cod) & ")  ")
            End If

            If FasiProduttive <> "" Then
                StrSQL.AppendLine(" AND (ID_FP IN (" & Agro_SQL_Save_Clausola_IN(FasiProduttive) & ")  ")
                StrSQL.AppendLine("     OR ID_FP IS NULL)  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY volumi DESC ")
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

    Public Function Leggi_Formulati_NonUtilizzabili_DaRegolamentiXIndicazioniEscluse(
                                ByVal Cod_Disciplinare As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_Formulati_NonUtilizzabili_DaRegolamentiXIndicazioniEscluse()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT DISTINCT Fr_Cod ")
            StrSQL.AppendLine(" FROM RegolamentiXIndicazioniEscluse ri ")
            StrSQL.AppendLine(" INNER JOIN FormulatixIndicazioni fi ON ri.Indicazione_Cod = fi.Indicazione_Cod collate Latin1_General_CI_AS ")

            If Cod_Disciplinare <> 0 Then
                StrSQL.AppendLine(" WHERE Cod_Regolamento =  " & Agro_SQL_SaveNum(Cod_Disciplinare) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_PA_Ausiliari(ByVal Id_PAA As Int32,
                                       ByVal Dft_Cod As Int32,
                                       ByVal Dfr_Cod As Int32,
                                       ByVal Pa_Cod As Int32,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_R.Leggi_PA_Ausiliari()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("  SELECT Distinct PA_Ausiliari.*, " &
                        " isnull(Dose_Max_Anno, 0) as Dose_Max_Anno2, " &
                        " isnull(Dose_UDM, 0) as Dose_UDM2, " &
                        " isnull(PercPa, 0) as PercPa2, " &
                        " isnull(PesoPa, 0) as PesoPa2 " &
                        " FROM   PA_Ausiliari " &
                        " Where 1 = 1 ")

            If Id_PAA <> 0 Then
                StrSQL.AppendLine(" And PA_Ausiliari.Id_PAA =  " & Agro_SQL_SaveNum(Id_PAA) & "  ")
            End If
            If Dft_Cod <> 0 Then
                StrSQL.AppendLine(" And PA_Ausiliari.DFT_COD =  " & Agro_SQL_SaveNum(Dft_Cod) & "  ")
            End If
            If Dfr_Cod <> 0 Then
                StrSQL.AppendLine(" And PA_Ausiliari.DFR_COD =  " & Agro_SQL_SaveNum(Dfr_Cod) & "  ")
            End If
            If Pa_Cod <> 0 Then
                StrSQL.AppendLine(" And PA_Ausiliari.PA_Cod =  " & Agro_SQL_SaveNum(Pa_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PA_Ausiliari.PA_Cod ASC")
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



End Class
