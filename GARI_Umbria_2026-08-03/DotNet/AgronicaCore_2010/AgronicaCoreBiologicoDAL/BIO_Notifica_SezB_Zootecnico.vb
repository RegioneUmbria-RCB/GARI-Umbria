Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class BIO_Notifica_SezB_Zootecnico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer, _
                            ByVal UnitaProduttiva_Piva As String, _
                              ByVal UnitaProduttiva_SaCod As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_SezB_Zootecnico.* ")
            StrSQL.Append(" FROM  BIO_Notifica_SezB_Zootecnico ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezB_Zootecnico.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If UnitaProduttiva_Piva <> "" Then
                StrSQL.Append(" AND UnitaProduttiva_Piva = '" & Agro_SQL_SaveText(UnitaProduttiva_Piva) & "'  ")
            End If

            If UnitaProduttiva_SaCod <> 0 Then
                StrSQL.Append(" AND UnitaProduttiva_SaCod = " & Agro_SQL_SaveNum(UnitaProduttiva_SaCod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezB_Zootecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezB_Zootecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class BIO_Notifica_SezB_Zootecnico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                            ByVal UnitaProduttiva_Piva As String,
                            ByVal UnitaProduttiva_SaCod As Integer,
                            ByVal UnitaProduttiva_RagSoc As String,
                            ByVal UnitaProduttiva_Via As String,
                            ByVal UnitaProduttiva_Numero As String,
                            ByVal UnitaProduttiva_CAP As String,
                            ByVal UnitaProduttiva_CodIstat_Provincia As String,
                            ByVal UnitaProduttiva_CodIstat_Comune As String,
                            ByVal UnitaProduttiva_Telefono As String,
                            ByVal UnitaProduttiva_Fax As String,
                            ByVal UnitaProduttiva_Email As String,
                            ByVal UnitaProduttiva_CodAUSL As String,
                            ByVal UnitaProduttiva_TotaleZootecniche As Integer,
                            ByVal AA_TotaleUnitaProduttive As Integer,
                            ByVal AA10_UBABiologico As Decimal,
                            ByVal AA10_UBAConvenzionale As Decimal,
                            ByVal AA10_IPCarne As Integer,
                            ByVal AA10_IPLatte As Integer,
                            ByVal AA10_IPRiproduzione As Integer,
                            ByVal AA10_IPAltro As Integer,
                            ByVal AA10_IPAltroDes As String,
                            ByVal AA20_UBABiologico As Decimal,
                            ByVal AA20_UBAConvenzionale As Decimal,
                            ByVal AA20_IPCarne As Integer,
                            ByVal AA20_IPLatte As Integer,
                            ByVal AA20_IPRiproduzione As Integer,
                            ByVal AA20_IPAltro As Integer,
                            ByVal AA20_IPAltroDes As String,
                            ByVal AA30_UBABiologico As Decimal,
                            ByVal AA30_UBAConvenzionale As Decimal,
                            ByVal AA30_IPCarne As Integer,
                            ByVal AA30_IPLatte As Integer,
                            ByVal AA30_IPRiproduzione As Integer,
                            ByVal AA30_IPAltro As Integer,
                            ByVal AA30_IPAltroDes As String,
                            ByVal AA40_UBABiologico As Decimal,
                            ByVal AA40_UBAConvenzionale As Decimal,
                            ByVal AA40_IPCarne As Integer,
                            ByVal AA40_IPLatte As Integer,
                            ByVal AA40_IPRiproduzione As Integer,
                            ByVal AA40_IPAltro As Integer,
                            ByVal AA40_IPAltroDes As String,
                            ByVal AA50_UBABiologico As Decimal,
                            ByVal AA50_UBAConvenzionale As Decimal,
                            ByVal AA50_IPCarne As Integer,
                            ByVal AA50_IPRiproduzione As Integer,
                            ByVal AA50_IPAltro As Integer,
                            ByVal AA50_IPAltroDes As String,
                            ByVal AA60_UBABiologico As Decimal,
                            ByVal AA60_UBAConvenzionale As Decimal,
                            ByVal AA60_IPRiproduzione As Integer,
                            ByVal AA60_IPAltro As Integer,
                            ByVal AA60_IPAltroDes As String,
                            ByVal AA61_UBABiologico As Decimal,
                            ByVal AA61_UBAConvenzionale As Decimal,
                            ByVal AA61_IPCarne As Integer,
                            ByVal AA70_UBABiologico As Decimal,
                            ByVal AA70_UBAConvenzionale As Decimal,
                            ByVal AA70_IPCarne As Integer,
                            ByVal AA70_IPUova As Integer,
                            ByVal AA70_IPRiproduzione As Integer,
                            ByVal AA70_IPAltro As Integer,
                            ByVal AA70_IPAltroDes As String,
                            ByVal AA80_UBABiologico As Integer,
                            ByVal AA80_UBAConvenzionale As Integer,
                            ByVal AA80_IPMiele As Integer,
                            ByVal AA80_IPPReale As Integer,
                            ByVal AA80_IPCera As Integer,
                            ByVal AA80_IPAltro As Integer,
                            ByVal AA80_IPAltroDes As String,
                            ByVal AA90_UBABiologico As Decimal,
                            ByVal AA90_UBAConvenzionale As Decimal,
                            ByVal AA90_AltroDes As String,
                            ByVal AA90_IPAltroDes As String,
                            ByVal AA_TotaleUBA As Decimal,
                            ByVal AA_TotaleFamiglie As Decimal,
                            ByVal AA_UBAxEttaroSAU As Decimal,
                            ByVal PZ_CARNE As Integer,
                            ByVal PZ_Carne_CarneFresca As Integer,
                            ByVal PZ_Carne_DerivatiCarne As Integer,
                            ByVal PZ_Carne_Macellazione As Integer,
                            ByVal PZ_Carne_Conservazione As Integer,
                            ByVal PZ_Carne_Sezionamento As Integer,
                            ByVal PZ_Carne_ProdottiSalumeria As Integer,
                            ByVal PZ_Carne_Confezionamento As Integer,
                            ByVal PZ_LATTE As Integer,
                            ByVal PZ_Latte_LatteAlimentare As Integer,
                            ByVal PZ_Latte_Caseificazione As Integer,
                            ByVal PZ_Latte_Burro As Integer,
                            ByVal PZ_Latte_Yogurt As Integer,
                            ByVal PZ_Latte_AltriDerivatiLatte As Integer,
                            ByVal PZ_Latte_Confezionamento As Integer,
                            ByVal PZ_Latte_Altro As Integer,
                            ByVal PZ_Latte_AltroDes As String,
                            ByVal PZ_UOVA As Integer,
                            ByVal PZ_Uova_Confezionamento As Integer,
                            ByVal PZ_Uova_Altro As Integer,
                            ByVal PZ_Uova_AltroDes As String,
                            ByVal PZ_PRODOTTIAPICOLTURA As Integer,
                            ByVal PZ_ProdottiApicoltura_Confezionamento As Integer,
                            ByVal PZ_ALTRO As Integer,
                            ByVal PZ_Altro_AltroDes As String,
                            ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezB_Zootecnico " & vbCrLf)

            StrSQL.Append("             (Notifica_SuperUser, " & vbCrLf)
            StrSQL.Append("             Notifica_ID, " & vbCrLf)
            StrSQL.Append("             UnitaProduttiva_Piva, UnitaProduttiva_SaCod, UnitaProduttiva_RagSoc, UnitaProduttiva_Via, UnitaProduttiva_Numero, UnitaProduttiva_CAP, " & vbCrLf)
            StrSQL.Append("             UnitaProduttiva_CodIstat_Provincia, UnitaProduttiva_CodIstat_Comune, UnitaProduttiva_Telefono, UnitaProduttiva_Fax, UnitaProduttiva_Email, " & vbCrLf)
            StrSQL.Append("             UnitaProduttiva_CodAUSL, UnitaProduttiva_TotaleZootecniche, AA_TotaleUnitaProduttive, AA10_UBABiologico, AA10_UBAConvenzionale, " & vbCrLf)
            StrSQL.Append("             AA10_IPCarne, AA10_IPLatte, AA10_IPRiproduzione, AA10_IPAltro, AA10_IPAltroDes, AA20_UBABiologico, AA20_UBAConvenzionale, AA20_IPCarne, " & vbCrLf)
            StrSQL.Append("             AA20_IPLatte, AA20_IPRiproduzione, AA20_IPAltro, AA20_IPAltroDes, AA30_UBABiologico, AA30_UBAConvenzionale, AA30_IPCarne, AA30_IPLatte, " & vbCrLf)
            StrSQL.Append("             AA30_IPRiproduzione, AA30_IPAltro, AA30_IPAltroDes, AA40_UBABiologico, AA40_UBAConvenzionale, AA40_IPCarne, AA40_IPLatte, " & vbCrLf)
            StrSQL.Append("             AA40_IPRiproduzione, AA40_IPAltro, AA40_IPAltroDes, AA50_UBABiologico, AA50_UBAConvenzionale, AA50_IPCarne, AA50_IPRiproduzione, " & vbCrLf)
            StrSQL.Append("             AA50_IPAltro, AA50_IPAltroDes, AA60_UBABiologico, AA60_UBAConvenzionale, AA60_IPRiproduzione, AA60_IPAltro, AA60_IPAltroDes, " & vbCrLf)
            StrSQL.Append("             AA61_UBABiologico, AA61_UBAConvenzionale, AA61_IPCarne, AA70_UBABiologico, AA70_UBAConvenzionale, AA70_IPCarne, AA70_IPUova, " & vbCrLf)
            StrSQL.Append("             AA70_IPRiproduzione, AA70_IPAltro, AA70_IPAltroDes, AA80_UBABiologico, AA80_UBAConvenzionale, AA80_IPMiele, AA80_IPPReale, AA80_IPCera, " & vbCrLf)
            StrSQL.Append("             AA80_IPAltro, AA80_IPAltroDes, AA90_UBABiologico, AA90_UBAConvenzionale, AA90_AltroDes, AA90_IPAltroDes, AA_TotaleUBA, AA_TotaleFamiglie, " & vbCrLf)
            StrSQL.Append("             AA_UBAxEttaroSAU, PZ_CARNE, PZ_Carne_CarneFresca, PZ_Carne_DerivatiCarne, PZ_Carne_Macellazione, PZ_Carne_Conservazione, " & vbCrLf)
            StrSQL.Append("             PZ_Carne_Sezionamento, PZ_Carne_ProdottiSalumeria, PZ_Carne_Confezionamento, PZ_LATTE, PZ_Latte_LatteAlimentare, PZ_Latte_Caseificazione, " & vbCrLf)
            StrSQL.Append("             PZ_Latte_Burro, PZ_Latte_Yogurt, PZ_Latte_AltriDerivatiLatte, PZ_Latte_Confezionamento, PZ_Latte_Altro, PZ_Latte_AltroDes, PZ_UOVA, " & vbCrLf)
            StrSQL.Append("             PZ_Uova_Confezionamento, PZ_Uova_Altro, PZ_Uova_AltroDes, PZ_PRODOTTIAPICOLTURA, PZ_ProdottiApicoltura_Confezionamento, PZ_ALTRO, " & vbCrLf)
            StrSQL.Append("             PZ_Altro_AltroDes, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("             '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "',  ")
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(Notifica_ID)) & "," & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Piva)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(UnitaProduttiva_SaCod)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_RagSoc)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Via)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Numero)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CAP)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodIstat_Provincia)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodIstat_Comune)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Telefono)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Fax)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Email)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(UnitaProduttiva_CodAUSL)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(UnitaProduttiva_TotaleZootecniche)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA_TotaleUnitaProduttive)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_IPLatte)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA10_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA10_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_IPLatte)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA20_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA20_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_IPLatte)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA30_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA30_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_IPLatte)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA40_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA40_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA50_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA50_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA50_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA50_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA50_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA50_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA60_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA60_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA60_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA60_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA60_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA61_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA61_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA61_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_IPCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_IPUova)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_IPRiproduzione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA70_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA70_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_IPMiele)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_IPPReale)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_IPCera)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA80_IPAltro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA80_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA90_UBABiologico)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA90_UBAConvenzionale)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA90_AltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(AA90_IPAltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA_TotaleUBA)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA_TotaleFamiglie)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(AA_UBAxEttaroSAU)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_CARNE)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_CarneFresca)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_DerivatiCarne)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_Macellazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_Conservazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_Sezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_ProdottiSalumeria)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Carne_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_LATTE)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_LatteAlimentare)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_Caseificazione)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_Burro)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_Yogurt)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_AltriDerivatiLatte)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Latte_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(PZ_Latte_AltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_UOVA)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Uova_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_Uova_Altro)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(PZ_Uova_AltroDes)) & "', " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_PRODOTTIAPICOLTURA)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_ProdottiApicoltura_Confezionamento)) & ", " & vbCrLf)
            StrSQL.Append("             " & Agro_SQL_SaveNum(Trim(PZ_ALTRO)) & ", " & vbCrLf)
            StrSQL.Append("             '" & Agro_SQL_SaveText(Trim(PZ_Altro_AltroDes)) & "' " & vbCrLf)

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")


            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")


            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
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


    '###########################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                             ByVal UnitaProduttiva_Piva As String,
                             ByVal UnitaProduttiva_SaCod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezB_Zootecnico_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE BIO_Notifica_SezB_Zootecnico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM BIO_Notifica_SezB_Zootecnico ")
                StrSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If
            '---------------------------------------------

            If UnitaProduttiva_Piva <> "" Then
                StrSQL.Append(" AND UnitaProduttiva_Piva = '" & Agro_SQL_SaveText(UnitaProduttiva_Piva) & "'  ")
            End If

            If UnitaProduttiva_SaCod <> 0 Then
                StrSQL.Append(" AND UnitaProduttiva_SaCod = " & Agro_SQL_SaveNum(UnitaProduttiva_SaCod) & "  ")
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
