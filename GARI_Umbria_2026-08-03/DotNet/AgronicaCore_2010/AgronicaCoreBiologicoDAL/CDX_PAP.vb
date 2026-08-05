Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CDX_PAP_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Stampa_PAP(ByVal ID_PAP As Int32,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAP_R.Stampa_PAP()"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT dbo.Imprese.rag_soc, dbo.CDX_PAP.ID_PAP, dbo.CDX_PAP.Data_Firma_Doc, dbo.CDX_PAP.Piva, dbo.CDX_PAP.CUAA, " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP.SupTotDoc_Ettari, dbo.CDX_PAP.SupTotDoc_Are, dbo.CDX_PAP.SupTotDoc_Centiare, dbo.CDX_PAP.NumTotAppezzamenti,  " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP.Flag_Prima_Variazione, dbo.CDX_PAP.Protocollo_Data, dbo.CDX_PAP.Protocollo, dbo.CDX_PAP.Regione_Cod, " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP.Sede_Regionale_Cod, dbo.CDX_PAP.Organismo_Sigla, dbo.CDX_PAP.Anno, dbo.CDX_PAP_B.ID_PAP_B, " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B.Num_Appezzamento_Str as Num_Appezzamento, dbo.CDX_PAP_B.Sa_Cod, dbo.CDX_PAP_B.Campo_Cod, dbo.CDX_PAP_B.Appezza, " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B.Specie_Cod, dbo.CDX_PAP_B.Varieta_Cod, dbo.CDX_PAP_B.Specie_Des, dbo.CDX_PAP_B.Varieta_Des, dbo.CDX_PAP_B.Veg_Cod,  " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B.Cul_Cod, dbo.CDX_PAP_B.AppSup_Ettari, dbo.CDX_PAP_B.AppSup_Are, dbo.CDX_PAP_B.AppSup_Centiare,  " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B.Tipo_Agricoltura, dbo.CDX_PAP_B.Successione, dbo.CDX_PAP_B.Consociazione, dbo.CDX_PAP_B.Anno_Impianto,  " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B.Qta_Prevista, dbo.CDX_PAP_B.Forza_Lavoro, " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP.Data_Creazione, CDX_PAP.Data_Modifica, dbo.CDX_PAP.Username_Creazione, CDX_PAP.Username_Modifica, CDX_PAP.validita_inizio, CDX_PAP.validita_fine, CDX_PAP_B.Validazione, CDX_PAP_B.UserName_Validazione, CDX_PAP_B.Data_Validazione,CDX_PAP_B.Num_Appezzamento_Str " & vbCrLf)
            strSQL.Append(" FROM dbo.CDX_PAP INNER JOIN " & vbCrLf)
            strSQL.Append(" dbo.Imprese ON dbo.CDX_PAP.Piva = dbo.Imprese.PIVA LEFT OUTER JOIN " & vbCrLf)
            strSQL.Append(" dbo.CDX_PAP_B ON dbo.CDX_PAP.ID_PAP = dbo.CDX_PAP_B.ID_PAP " & vbCrLf)
            strSQL.Append(" WHERE 1=1 " & vbCrLf)

            '----- Condizioni
            If ID_PAP <> 0 Then
                strSQL.Append(" AND dbo.CDX_PAP.ID_PAP = " & Agro_SQL_SaveNum(ID_PAP) & "  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   CDX_PAP.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   CDX_PAP.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSQL.Append(" ORDER BY Num_Appezzamento ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class CDX_PAP_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByVal ID_PAP As Integer,
                            ByVal Anno As Integer,
                            ByVal Organismo_Sigla As String,
                            ByVal Sede_Regionale_Cod As String,
                            ByVal Regione_Cod As String,
                            ByVal Protocollo As String,
                            ByVal Protocollo_Data As Date,
                            ByVal Flag_Prima_Variazione As String,
                            ByVal Piva As String,
                            ByVal CUAA As String,
                            ByVal SupTotDoc_Ettari As Decimal,
                            ByVal SupTotDoc_Are As Decimal,
                            ByVal SupTotDoc_Centiare As Decimal,
                            ByVal NumTotAppezzamenti As Integer,
                            ByVal Data_Firma_Doc As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Validazione As Integer,
                            ByVal Data_Validazione As Date,
                            ByVal UserName_Validazione As String,
                            ByRef objParametri As AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CDX_PAP " & vbCrLf)

            StrSQL.Append("              (ID_PAP, Anno, Organismo_Sigla, Sede_Regionale_Cod, Regione_Cod, ")
            StrSQL.Append("               Protocollo, Protocollo_Data, Flag_Prima_Variazione, Piva, CUAA, ")
            StrSQL.Append("               SupTotDoc_Ettari, SupTotDoc_Are, SupTotDoc_Centiare, NumTotAppezzamenti, ")
            StrSQL.Append("               Data_Firma_Doc, ")
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " & vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")


            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(ID_PAP)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(Anno)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Organismo_Sigla)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Sede_Regionale_Cod)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Regione_Cod)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Protocollo)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveDate(Trim(Protocollo_Data)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Flag_Prima_Variazione)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CUAA)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(SupTotDoc_Ettari)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(SupTotDoc_Are)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(SupTotDoc_Centiare)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(NumTotAppezzamenti)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveDate(Trim(Data_Firma_Doc)) & " " & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trim(Validazione)) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trim(Data_Validazione)) & " " & vbCrLf)
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Trim(UserName_Validazione)) & "' " & vbCrLf)
            '  StrSQL.Append("         , 0  ")
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


    '#################################################################
    Public Function Cancella(ByVal ID_PAP As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If ID_PAP = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PAP obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE CDX_PAP ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   ID_PAP = " & Agro_SQL_SaveNum(ID_PAP) & "  ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM CDX_PAP ")
                StrSQL.Append(" WHERE ID_PAP = " & Agro_SQL_SaveNum(ID_PAP) & "  ")
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
