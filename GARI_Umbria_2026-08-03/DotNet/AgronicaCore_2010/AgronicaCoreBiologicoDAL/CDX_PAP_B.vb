Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CDX_PAP_B_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal ID_PAP As Integer, _
                            ByVal ID_PAP_B As Integer, _
                            ByVal Num_Appezzamento As Integer, _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Campo_Cod As Integer, _
                            ByVal Appezza As Integer, _
                            ByVal Specie_Cod As String, _
                            ByVal Varieta_Cod As String, _
                            ByVal Specie_Des As String, _
                            ByVal Varieta_Des As String, _
                            ByVal Veg_Cod As Integer, _
                            ByVal Cul_Cod As Integer, _
                            ByVal AppSup_Ettari As Integer, _
                            ByVal AppSup_Are As Integer, _
                            ByVal AppSup_Centiare As Integer, _
                            ByVal Tipo_Agricoltura As String, _
                            ByVal Consociazione As String, _
                            ByVal Successione As String, _
                             ByVal Anno_Impianto As Integer, _
                            ByVal Qta_Prevista As Decimal, _
                            ByVal Forza_Lavoro As Decimal, _
                              ByVal Num_Appezzamento_Str As String, _
                              ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal Validazione As Integer, _
                            ByVal Data_Validazione As Date, _
                            ByVal UserName_Validazione As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreBiologicoDAL.CDX_PAP_B_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

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

            StrSQL.Append(" INSERT INTO CDX_PAP_B " & vbCrLf)

            StrSQL.Append("              (ID_PAP, ID_PAP_B, Num_Appezzamento, Piva, Sa_Cod, Campo_Cod, ")
            StrSQL.Append("               Appezza, Specie_Cod, Varieta_Cod, Specie_Des, Varieta_Des, Veg_Cod, Cul_Cod, ")
            StrSQL.Append("               AppSup_Ettari, AppSup_Are, AppSup_Centiare, ")
            StrSQL.Append("                Tipo_Agricoltura, Consociazione, Successione, Anno_Impianto, ")
            StrSQL.Append("               Qta_Prevista, Forza_Lavoro, Num_Appezzamento_Str, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " & vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(" " & Agro_SQL_SaveNum(Trim(ID_PAP)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(ID_PAP_B)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(Num_Appezzamento)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Campo_Cod) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Specie_Cod)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Varieta_Cod)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Specie_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Varieta_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Veg_Cod) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Cul_Cod) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(AppSup_Ettari) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(AppSup_Are) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(AppSup_Centiare) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Tipo_Agricoltura)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Consociazione)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Successione)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Anno_Impianto) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Qta_Prevista) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Forza_Lavoro) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Num_Appezzamento_Str)) & "' " & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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
    Public Function Cancella(ByVal ID_PAP As Int32, _
                            ByVal ID_PAP_B As Int32, _
                              ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
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
                StrSQL.Append(" UPDATE CDX_PAP_B ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   ID_PAP = " & Agro_SQL_SaveNum(ID_PAP) & "  ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM CDX_PAP_B ")
                StrSQL.Append(" WHERE ID_PAP = " & Agro_SQL_SaveNum(ID_PAP) & "  ")
            End If

            If ID_PAP_B <> 0 Then
                StrSQL.Append(" AND ID_PAP_B = " & Agro_SQL_SaveNum(ID_PAP_B) & "  ")
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
