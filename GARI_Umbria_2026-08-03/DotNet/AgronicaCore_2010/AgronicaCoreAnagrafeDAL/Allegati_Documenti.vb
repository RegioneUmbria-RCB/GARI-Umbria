Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Allegati_Documenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Allegati_Documenti_Piva As String,
                           ByVal Allegati_Documenti_Des As String,
                           ByVal Allegati_Documenti_CatCod As Integer,
                           ByVal Allegati_Documenti_NomeFile As String,
                           ByVal Allegati_Documenti_Numero As String,
                           ByVal Allegati_Documenti_Ente_Cod As Integer,
                           ByVal Sottocartella As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef OUTPUT_Allegati_Documenti_Cod As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Validazione_Data As Date = AGRODATAFINE,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal strXml As String = "",
                           Optional ByVal Codice_Detentore As String = "",
                           Optional ByVal Allegati_Documenti_Ente_Des As String = Nothing,
                           Optional ByVal Validazione_Flag As Integer = 0,
                           Optional ByVal Username_Upload As String = "",
                           Optional ByVal Data_Upload As DateTime = #2/1/1900#,
                           Optional ByVal File_Allegato_DB As Byte() = Nothing,
                           Optional ByVal SalvaAllegato As Integer = 0,
                           Optional ByVal Allegati_Documenti_Estensione As String = "",
                           Optional ByVal CompressoDaGIAS As Boolean = False,
                           Optional ByVal Pratica_Cod As Integer = 0,
                           Optional ByVal strJSON As String = "",
                           Optional ByVal Allegati_Documenti_Data As Date = AGRODATAINIZIO
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim _Data_Upload As String = ""
        Dim _Data_Creazione As String = ""
        Dim _Data_Modifica As String = ""

        Try

            If Data_Upload = #2/1/1900# Then
                Data_Upload = Date.Now
            End If
            _Data_Upload = Data_Upload.ToString("MM/dd/yyyy HH:mm:ss.fff tt")

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If
            _Data_Creazione = Data_creazione.ToString("MM/dd/yyyy HH:mm:ss.fff tt")

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If
            _Data_Modifica = Data_modifica.ToString("MM/dd/yyyy HH:mm:ss.fff tt")

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            'Dim fileByteArray As Byte() = Convert.FromBase64String(File_Allegato)

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.AppendLine("INSERT INTO Allegati_Documenti( ")
            StrSQL.AppendLine("            Allegati_Documenti_SuperUser, Allegati_Documenti_Piva,   ")
            StrSQL.AppendLine("            Allegati_Documenti_Des,  Allegati_Documenti_CatCod, Allegati_Documenti_NomeFile, Allegati_Documenti_Estensione, ")
            If Not IsNothing(Allegati_Documenti_Numero) Then
                StrSQL.AppendLine("            Allegati_Documenti_Numero, ")
            End If
            If CDate(Allegati_Documenti_Data) <> AGRODATAINIZIO Then
                StrSQL.AppendLine("            Allegati_Documenti_Data, ")
            End If
            If Not IsNothing(Allegati_Documenti_Ente_Cod) Then
                StrSQL.AppendLine("            Allegati_Documenti_Ente_Cod, ")
            End If
            StrSQL.AppendLine("            Sottocartella, ")

            If Not String.IsNullOrEmpty(strXml) Then
                StrSQL.AppendLine("            allegatiDocumentiXML,  ")
            End If
            If Not String.IsNullOrEmpty(strJSON) Then
                StrSQL.AppendLine("            allegatiDocumentiText,  ")
            End If

            StrSQL.AppendLine("            Validazione_Flag, Username_Upload, Data_Upload, ")

            If SalvaAllegato = 1 AndAlso File_Allegato_DB IsNot Nothing Then
                'Salvataggio su DB
                StrSQL.AppendLine("   File_Allegato_DB,   ")
            End If

            'Cartella compressa da GIAS
            If Not IsNothing(CompressoDaGIAS) Then
                StrSQL.AppendLine("   CompressoDaGIAS,   ")
            End If


            StrSQL.AppendLine("            Inviato, DataInvio, ")
            StrSQL.AppendLine("            Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("            Validita_Inizio,    Validita_Fine, Validazione_Data, ")
            StrSQL.AppendLine("            Codice_Detentore ")
            If Allegati_Documenti_Ente_Des IsNot Nothing Then
                StrSQL.AppendLine("        , Allegati_Documenti_Ente_Des ")
            End If
            If Pratica_Cod > 0 Then
                StrSQL.AppendLine("        , Pratica_Cod ")
            End If
            StrSQL.AppendLine("            ) ")


            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Allegati_Documenti_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Allegati_Documenti_CatCod) & "  ")

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.SqlDataProvider Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText_UNICODE(Allegati_Documenti_NomeFile, "N") & "' ")
            Else
                StrSQL.AppendLine("         ,N'" & Agro_SQL_SaveText(Allegati_Documenti_NomeFile) & "' ")
            End If

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Allegati_Documenti_Estensione) & "' ")
            If Not IsNothing(Allegati_Documenti_Numero) Then
                StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "' ")
            End If
            If CDate(Allegati_Documenti_Data) <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Allegati_Documenti_Data) & "  ")
            End If
            If Not IsNothing(Allegati_Documenti_Ente_Cod) Then
                StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Allegati_Documenti_Ente_Cod) & " ")
            End If
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Sottocartella) & "' ")

            If Not String.IsNullOrEmpty(strXml) Then
                StrSQL.AppendLine(" , " & Agro_SQL_SaveStringToXML(strXml) & " ")
            End If
            If Not String.IsNullOrEmpty(strJSON) Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(strJSON) & "' ")
            End If

            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Validazione_Flag) & " ")
            StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Username_Upload) & "' ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Data_Upload) & "  ")

            If SalvaAllegato = 1 AndAlso File_Allegato_DB IsNot Nothing Then
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    'Salvataggio su DB
                    StrSQL.AppendLine(" , ?")
                Else
                    StrSQL.AppendLine(" , @P1")
                End If
            End If

            If Not IsNothing(CompressoDaGIAS) Then
                If CompressoDaGIAS Then
                    'Cartella compressa da GIAS
                    StrSQL.AppendLine(" , 1  ")
                Else
                    StrSQL.AppendLine(" , 0  ")
                End If
            End If

            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.AppendLine("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validazione_Data) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Detentore) & "'  ")
            If Allegati_Documenti_Ente_Des IsNot Nothing Then
                StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Allegati_Documenti_Ente_Des) & "'  ")
            End If
            If Pratica_Cod > 0 Then
                StrSQL.AppendLine("        , " & Agro_SQL_SaveNum(Pratica_Cod) & "  ")
            End If
            StrSQL.AppendLine(")")


            If SalvaAllegato AndAlso File_Allegato_DB IsNot Nothing Then
                '---------------------------------------------
                Dim CmdParameters As New Dictionary(Of String, Byte())
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    CmdParameters.Add("?", File_Allegato_DB)
                Else
                    CmdParameters.Add("@P1", File_Allegato_DB)
                End If

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, nomeRoutine, CmdParameters)
                '--------------------------------------------------------------------------            
            Else
                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------  

            End If


            Dim objleggi As New Allegati_Documenti_R
            OUTPUT_Allegati_Documenti_Cod = objleggi.UltimoID(objParametri,
                                                              username_creazione:=username_creazione,
                                                              username_modifica:=username_modifica,
                                                              data_upload:=_Data_Upload,
                                                              data_creazione:=_Data_Creazione,
                                                              data_modifica:=_Data_Modifica)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Modifica(ByVal Allegati_Documenti_Piva As String,
                             ByVal Allegati_Documenti_Des As String,
                             ByVal Allegati_Documenti_NomeFile As String,
                             ByVal Allegati_Documenti_Numero As String,
                             ByVal Sottocartella As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal Allegati_Documenti_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Validazione_Data As Date = AGRODATAFINE,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal strXml As String = "",
                             Optional ByVal Codice_Detentore As String = "",
                             Optional ByVal Allegati_Documenti_Ente_Des As String = Nothing,
                             Optional ByVal Validazione_Flag As Integer = 0,
                             Optional ByVal Username_Upload As String = "",
                             Optional ByVal Data_Upload As DateTime = #2/1/1900#,
                             Optional ByVal File_Allegato_DB As Byte() = Nothing,
                             Optional ByVal bAllegato_Modificato As Boolean = False,
                             Optional ByVal SalvaAllegato As Integer = 0,
                             Optional ByVal Allegati_Documenti_Estensione As String = "",
                             Optional ByVal CompressoDaGIAS As Boolean = False,
                             Optional ByVal strJSON As String = "",
                             Optional ByVal Allegati_Documenti_Data As Date = AGRODATAINIZIO
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            If Data_Upload = #2/1/1900# Then
                Data_Upload = Now
            End If

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("UPDATE Allegati_Documenti SET " & vbCrLf)
            If strJSON <> "" Then
                StrSQL.Append(" allegatiDocumentiText = '" & Agro_SQL_SaveText(strJSON) & "', " & vbCrLf)
            End If
            StrSQL.Append(" Allegati_Documenti_Numero = '" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "', " & vbCrLf)
            StrSQL.Append(" Validazione_Flag = " & Agro_SQL_SaveNum(Validazione_Flag) & ", " & vbCrLf)

            If Allegati_Documenti_Ente_Des IsNot Nothing Then
                StrSQL.Append(" Allegati_Documenti_Ente_Des = '" & Agro_SQL_SaveText(Allegati_Documenti_Ente_Des) & "', " & vbCrLf)
            End If
            If Validazione_Data <> AGRODATAFINE Then
                StrSQL.Append(" Validazione_Data = " & Agro_SQL_SaveDate(Validazione_Data) & ", " & vbCrLf)
            End If

            If Allegati_Documenti_Data <> AGRODATAINIZIO Then
                StrSQL.Append(" Allegati_Documenti_Data = " & Agro_SQL_SaveDate(Allegati_Documenti_Data) & ", " & vbCrLf)
            Else
                StrSQL.Append(" Allegati_Documenti_Data = NULL, " & vbCrLf)
            End If


            If bAllegato_Modificato Then

                StrSQL.Append(" Allegati_Documenti_Des = '" & Agro_SQL_SaveText(Allegati_Documenti_Des) & "', " & vbCrLf)
                StrSQL.Append(" Allegati_Documenti_NomeFile = '" & Agro_SQL_SaveText(Allegati_Documenti_NomeFile) & "', " & vbCrLf)
                StrSQL.Append(" Allegati_Documenti_Estensione = '" & Agro_SQL_SaveText(Allegati_Documenti_Estensione) & "', " & vbCrLf)
                StrSQL.Append(" Sottocartella = '" & Agro_SQL_SaveText(Sottocartella) & "', " & vbCrLf)
                StrSQL.Append(" Username_Upload = '" & Agro_SQL_SaveText(Username_Upload) & "', " & vbCrLf)
                StrSQL.Append(" Data_Upload = " & Agro_SQL_SaveDateTime(Data_Upload) & ", " & vbCrLf)

                If SalvaAllegato = 1 AndAlso File_Allegato_DB IsNot Nothing Then
                    If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                        'Salvataggio su DB
                        StrSQL.Append(" File_Allegato_DB = ?, " & vbCrLf)
                    Else
                        StrSQL.Append(" File_Allegato_DB = @P1, " & vbCrLf)
                    End If
                ElseIf SalvaAllegato = 0 Then
                    StrSQL.Append(" File_Allegato_DB = NULL, " & vbCrLf)
                End If

                If Not IsNothing(CompressoDaGIAS) Then
                    StrSQL.Append(" CompressoDaGIAS = " & Agro_SQL_SaveBoolStrToInt(CompressoDaGIAS) & ", " & vbCrLf)
                End If
            End If


            StrSQL.Append(" username_modifica = '" & Agro_SQL_SaveText(username_modifica) & "', " & vbCrLf)
            StrSQL.Append(" data_modifica = " & Agro_SQL_SaveDateTime(Now) & ", " & vbCrLf)
            StrSQL.Append(" validita_inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", " & vbCrLf)
            StrSQL.Append(" validita_fine = " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " " & vbCrLf)


            If bAllegato_Modificato AndAlso SalvaAllegato = 1 AndAlso File_Allegato_DB IsNot Nothing Then
                '---------------------------------------------
                Dim CmdParameters As New Dictionary(Of String, Byte())
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    CmdParameters.Add("?", File_Allegato_DB)
                Else
                    CmdParameters.Add("@P1", File_Allegato_DB)
                End If

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, nomeRoutine, CmdParameters)
                '--------------------------------------------------------------------------            
            Else
                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------  

            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    '#########################################################################
    Public Function Modifica_File_Allegato_DB(ByVal Allegati_Documenti_Piva As String,
                                              ByVal Allegati_Documenti_Cod As Integer,
                                              ByVal File_Allegato_DB As Byte(),
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_File_Allegato_DB()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Allegati_Documenti SET ")

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                'Salvataggio su DB
                StrSQL.Append(" File_Allegato_DB = ? ")
            Else
                StrSQL.Append(" File_Allegato_DB = @P1 ")
            End If

            StrSQL.Append(" Where Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")

            If Allegati_Documenti_Piva <> "" Then
                StrSQL.Append(" And Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "'")
            End If

            '---------------------------------------------
            Dim CmdParameters As New Dictionary(Of String, Byte())
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                CmdParameters.Add("?", File_Allegato_DB)
            Else
                CmdParameters.Add("@P1", File_Allegato_DB)
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, nomeRoutine, CmdParameters)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################################
    Public Function Modifica_NumeroDataValidazione_DatoPlanning(ByVal Programmazione_cod As Integer,
                                                                ByVal Allegati_Documenti_Numero As String,
                                                                ByVal Validazione_Data As Date,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_NumeroDataValidazione_DatoPlanning()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Programmazione_cod = 0 Then
                Throw New Exception("Programmazione_cod = 0 .. questo non è consentito ..")
            End If

            Dim Data_modifica As DateTime = Date.Now
            Dim username_modifica As String = objParametri.UsernameOperazione

            Dim leggiP As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim dtLeggiP As DataTable = leggiP.Leggi(Allegati_Documenti_Cod := 0,
                                                     Piva := "",
                                                     Sa_Cod := 0,
                                                     Campo_Cod := 0,
                                                     Appezza := 0,
                                                     Id_Imp := 0,
                                                     Fabbricato_Cod := 0,
                                                     Prov := "",
                                                     Com := "",
                                                     Sezione := "",
                                                     Foglio := 0,
                                                     Numero := 0,
                                                     Subalterno := "",
                                                     ID_Oggetto_Grafico := "",
                                                     Chiave_Albero_Imprese := "",
                                                     Analisi_Testata_Cod := 0,
                                                     Programmazione_Cod := Programmazione_cod,
                                                     Programmazione_Entita_Cod := 0,
                                                     xFiltroAggiuntivo := "",
                                                     xOrderBy := "",
                                                     objParametri := objParametri)

            If dtLeggiP.Rows.Count > 0 Then

                Dim lAllegatiDocumentiCod As Integer = dtLeggiP.Rows(0)("Allegati_Documenti_cod")

                '---------------------------------------------
                stb.Length = 0
                stb.Append(" update D set " & vbCrLf)
                stb.Append("   Allegati_Documenti_Numero = '" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "'" & vbCrLf)
                stb.Append(" , Validazione_Data =  " & Agro_SQL_SaveDate(Validazione_Data) & vbCrLf)
                stb.Append(" , Data_Modifica = " & Agro_SQL_SaveDate(Data_modifica) & " " & vbCrLf)
                stb.Append(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

                stb.Append(" from Allegati_Documenti D " & vbCrLf)
                stb.Append(" where Allegati_Documenti_cod =  " & lAllegatiDocumentiCod & vbCrLf)

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica_Programmazione_Des_Dato_NumeroValidazione(ByVal Allegati_Documenti_Numero As String,
                                                                       ByVal Allegati_Documenti_Piva As String,
                                                                       ByVal Programmazione_Des As String,
                                                                       ByRef objParametri As AgronicaCoreParametri
                                                                       ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_Programmazione_Des_Dato_NumeroValidazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim Data_modifica As DateTime = Date.Now
            Dim username_modifica As String = objParametri.UsernameOperazione

            '---------------------------------------------
            stb.Length = 0
            stb.Append(" update D set " & vbCrLf)
            stb.Append("   Allegati_Documenti_Des = '" & Agro_SQL_SaveText(Programmazione_Des) & "'" & vbCrLf)
            stb.Append(" , Data_Modifica = " & Agro_SQL_SaveDate(Data_modifica) & " " & vbCrLf)
            stb.Append(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            stb.Append(" FROM Allegati_Documenti D " & vbCrLf)
            stb.Append(" WHERE Allegati_Documenti_Piva = " & Agro_SQL_SaveText_NULL(Allegati_Documenti_Piva) & "")
            stb.Append(" AND Allegati_Documenti_Numero = " & Agro_SQL_SaveText_NULL(Allegati_Documenti_Numero) & "")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Validazione(ByVal Allegati_Documenti_Cod As Integer,
                                         ByVal Validazione_Flag As Integer,
                                         ByVal Validazione_Data As Date,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_Validazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim username_modifica As String = objParametri.UsernameOperazione

            stb.Length = 0
            stb.Append(" Update Allegati_Documenti set " & vbCrLf)
            stb.Append("   Validazione_Flag = " & Agro_SQL_SaveNum(Validazione_Flag) & " " & vbCrLf)
            stb.Append(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Validazione_Data) & " " & vbCrLf)
            stb.Append(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)
            '---------------------------------------------


            stb.Append(" FROM Allegati_Documenti " & vbCrLf)
            stb.Append(" WHERE Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & "")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica_ImportazioneApp(ByVal ID As String,
                                             ByVal Importato_Errore As String,
                                             ByVal Importato_Data As Date,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_ImportazioneApp()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim username_modifica As String = objParametri.UsernameOperazione

            stb.Length = 0
            stb.Append(" Update App_Documenti set " & vbCrLf)
            stb.Append(" Importato_Errore = '" & Agro_SQL_SaveText(Importato_Errore, True) & "' " & vbCrLf)

            'If Trim(Importato_Errore) = "" Then
            stb.Append(" , Importato_Data = " & Agro_SQL_SaveDateTime(Importato_Data) & " " & vbCrLf)
            'End If

            '----------------------------
            stb.Append(" WHERE ID = '" & Agro_SQL_SaveText(ID) & "'")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return 0

    End Function


    '#########################################################################
    Public Function Modifica_FascicoloXML(ByVal Allegati_Documenti_Cod As Integer,
                                          ByVal piva As String,
                                          ByVal Allegati_Documenti_Ente_Cod As Integer,
                                          ByVal strXml As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                          Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                          Optional ByVal username_creazione As String = "",
                                          Optional ByVal username_modifica As String = ""
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Modifica_FascicoloXML()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
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

            stb.Length = 0
            stb.AppendLine(" update D set ")
            stb.AppendLine("   allegatiDocumentiXML = ")
            stb.Append(Agro_SQL_SaveStringToXML(strXml))
            stb.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDate(Data_modifica) & " ")
            stb.AppendLine(" , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine(" , Allegati_Documenti_Ente_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Ente_Cod))


            stb.Append(" from Allegati_Documenti D ")
            stb.Append(" where Allegati_Documenti_cod =  " & Agro_SQL_SaveNum(Allegati_Documenti_Cod))
            stb.Append(" and Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(piva) & "'")
            stb.Append(" and Allegati_Documenti_SuperUser =  '" & objParametri.PivaSuperUser & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################################
    Public Function Cancella(ByVal Allegati_Documenti_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Allegati_Documenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Allegati_Documenti ")
                StrSQL.Append(" WHERE    Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato = 0 ")

            End If

            StrSQL.Append(" AND Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Pulisci_DocumentiAPP(ByRef NumRecord As Integer, ByVal MaxRecord As Integer, ByVal GgConservazione As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Pulisci_DocumentiAPP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Integer = 0

        Try

            StrSQL.AppendLine("SELECT COUNT(*) FROM APP_Documenti with(nolock) ")
            StrSQL.AppendLine("  INNER JOIN Alert_Elenco with(nolock) ON APP_Documenti.id=Alert_Elenco.id_app ")
            StrSQL.AppendLine("  WHERE Importato_Data IS NULL AND LEN(FileAllegato) > 0 ")
            StrSQL.AppendLine("  AND APP_Documenti.Data_Creazione < DATEADD(day, -" & GgConservazione & ", GetDate()) ")
            Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            If dt.Rows.Count > 0 AndAlso dt.Rows(0)(0) > 0 Then

                NumRecord = dt.Rows(0)(0)

                StrSQL.AppendLine("UPDATE APP_Documenti SET FileAllegato=NULL, Importato_Data=Data_Creazione ")
                StrSQL.AppendLine("WHERE id in (SELECT TOP " & MaxRecord & " id FROM APP_Documenti with(nolock) ")
                StrSQL.AppendLine("  INNER JOIN Alert_Elenco with(nolock) ON APP_Documenti.id=Alert_Elenco.id_app ")
                StrSQL.AppendLine("  WHERE Importato_Data IS NULL AND LEN(FileAllegato) > 0 ")
                StrSQL.AppendLine("  AND APP_Documenti.Data_Creazione < DATEADD(day, -" & GgConservazione & ", GetDate()) ")
                StrSQL.AppendLine("  ORDER BY APP_Documenti.Data_Creazione)")

                If EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine) Then
                    xRisp = If(NumRecord > MaxRecord, MaxRecord, NumRecord)
                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Cancella_DocumentoAPP(ByVal ID As String, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Cancella_DocumentoAPP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Append(" DELETE FROM APP_Documenti ")
            StrSQL.Append(" WHERE ID LIKE '%" & Agro_SQL_SaveText(ID) & "%' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Scrivi_DocumentoAPP(ByVal ID As String,
                                        ByVal Piva As String,
                                        ByVal Documento_Cod As Integer,
                                        ByVal ID_Tipologia As Integer,
                                        ByVal Descrizione As String,
                                        ByVal Data_Scadenza As Date,
                                        ByVal Note As String,
                                        ByVal NomeFile As String,
                                        ByVal APP_Ricette_Operazioni_ID As String,
                                        ByVal APP_Ricette_Destinazioni_ID As String,
                                        ByVal FileAllegato As Byte(),
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal Visita_Cod As Integer = 0,
                                        Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                        Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                        Optional ByVal username_creazione As String = "",
                                        Optional ByVal username_modifica As String = "",
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                        Optional ByVal Data_Documento As Date = AGRODATAINIZIO,
                                        Optional ByVal Numero_Documento As String = ""
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.Scrivi_DocumentoAPP()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
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

            StrSQL.AppendLine("INSERT INTO APP_Documenti( ")
            StrSQL.AppendLine("  ID, Piva_SuperUser, Piva, Documento_Cod, ID_Tipologia, Descrizione, Data_Scadenza, NomeFile, FileAllegato, Note, Visita_Cod, APP_Ricette_Operazioni_ID, APP_Ricette_Destinazioni_ID, ")
            StrSQL.AppendLine("  Inviato, DataInvio, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine, ")
            StrSQL.AppendLine("  Data_Documento, Numero_Documento ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(ID) & "' ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("  , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveNum(Documento_Cod) & "  ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveNum(ID_Tipologia) & "  ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(Descrizione) & "' ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveDateTime(Data_Scadenza) & "  ")

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.SqlDataProvider Then
                StrSQL.AppendLine("  , '" & Agro_SQL_SaveText_UNICODE(NomeFile, "N") & "' ")
            Else
                StrSQL.AppendLine("  ,N'" & Agro_SQL_SaveText(NomeFile) & "' ")
            End If

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.AppendLine(" , ?")
            Else
                StrSQL.AppendLine(" , @P1")
            End If

            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(Note) & "' ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveNum(Visita_Cod) & "  ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(APP_Ricette_Operazioni_ID) & "' ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(APP_Ricette_Destinazioni_ID) & "' ")
            StrSQL.AppendLine("  , 0 ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("	 , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("	 , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("	 ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("	 ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If Data_Documento <> AGRODATAINIZIO Then
                StrSQL.AppendLine("  , " & Agro_SQL_SaveDate(Data_Documento) & "  ")
            Else
                StrSQL.AppendLine("  , NULL ")
            End If
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(Numero_Documento) & "' ")

            StrSQL.AppendLine(")")

            '---------------------------------------------
            Dim CmdParameters As New Dictionary(Of String, Byte())
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                CmdParameters.Add("?", FileAllegato)
            Else
                CmdParameters.Add("@P1", FileAllegato)
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, nomeRoutine, CmdParameters)
            '--------------------------------------------------------------------------            

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ModificaAutomatica(ByVal Allegati_Documenti_Cod As Integer,
                                       ByVal Validazione_Data As Date,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_W.ModificaAutomatica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Append(" UPDATE Allegati_Documenti SET ")
            StrSQL.Append("     Validazione_Data = " & Agro_SQL_SaveDate(Validazione_Data) & " ")

            StrSQL.Append("   , Inviato =  0 ")
            StrSQL.Append("   , DataInvio =  NULL ")
            StrSQL.Append("   , Data_Modifica =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Cod) & " ")
            StrSQL.Append(" AND Allegati_Documenti_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Allegati_Documenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiListaCodDaListaRicettaOperazioni(ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                          ByVal xFiltroAggiuntivo As String,
                                                          ByVal xOrderBy As String,
                                                          ByRef objParametri As AgronicaCoreParametri,
                                                          Optional ByVal Allegati_Documenti_Piva As String = "",
                                                          Optional ByVal Allegati_Documenti_Numero As String = "",
                                                          Optional ByVal Fonte_Cod As Integer = 0
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM   Alert_Entita ")
                    StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Allegati_Documenti_Piva <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "' ") 'commento
                    End If

                    If Allegati_Documenti_Numero <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Numero = '" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "' ")
                    End If

                    If Fonte_Cod <> 0 Then
                        StrSQL.Append(" AND Allegati_Documenti_Ente_Cod = '" & Agro_SQL_SaveText(Fonte_Cod) & "' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    Throw New NotImplementedException

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function LeggiXML(ByVal Allegati_Documenti_Cod As Integer,
                             ByVal xSelezioneVariabile As enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             ByVal Allegati_Documenti_Piva As String
                             ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.LeggiXML()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As String

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT allegatiDocumentiXML ")
                    StrSQL.Append(" FROM   Allegati_Documenti ")
                    StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Allegati_Documenti_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Allegati_Documenti_Cod <> 0 Then
                        StrSQL.Append(" AND Allegati_Documenti_Cod = " & Allegati_Documenti_Cod & " ")
                    End If

                    If Allegati_Documenti_Piva <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "' ") 'commento
                    End If

                    'If Allegati_Documenti_Ente_Cod <> 0 Then
                    '    StrSQL.Append(" AND Allegati_Documenti_Ente_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Ente_Cod) & " ")
                    'End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    Throw New NotImplementedException

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura_XML(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################

    Public Function LeggiXML_JSON(ByVal Allegati_Documenti_Cod As Integer,
                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByVal Allegati_Documenti_Piva As String
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.LeggiXML()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As New DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Allegati_Documenti_Cod,allegatiDocumentiXML,allegatiDocumentiText ")
                    StrSQL.Append(" FROM   Allegati_Documenti ")
                    StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Allegati_Documenti_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Allegati_Documenti_Cod <> 0 Then
                        StrSQL.Append(" AND Allegati_Documenti_Cod = " & Allegati_Documenti_Cod & " ")
                    End If

                    If Allegati_Documenti_Piva <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "' ") 'commento
                    End If

                    'If Allegati_Documenti_Ente_Cod <> 0 Then
                    '    StrSQL.Append(" AND Allegati_Documenti_Ente_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Ente_Cod) & " ")
                    'End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    Throw New NotImplementedException

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Allegati_Documenti_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Allegati_Documenti_Piva As String = "",
                          Optional ByVal Allegati_Documenti_Numero As String = "",
                          Optional ByVal Fonte_Cod As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM   Allegati_Documenti ")
                    StrSQL.Append(" LEFT JOIN pratiche p on p.pratica_cod = Allegati_Documenti.pratica_cod ")
                    StrSQL.Append(" LEFT JOIN pratiche_Stati_Attuali psa on psa.pratica_cod = p.pratica_cod ")
                    StrSQL.Append(" WHERE  Allegati_Documenti.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Allegati_Documenti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Allegati_Documenti_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Allegati_Documenti_Cod <> 0 Then
                        StrSQL.Append(" AND Allegati_Documenti_Cod = " & Allegati_Documenti_Cod & " ")
                    End If

                    If Allegati_Documenti_Piva <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "' ") 'commento
                    End If

                    If Allegati_Documenti_Numero <> "" Then
                        StrSQL.Append(" AND Allegati_Documenti_Numero = '" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "' ")
                    End If

                    If Fonte_Cod <> 0 Then
                        StrSQL.Append(" AND Allegati_Documenti_Ente_Cod = '" & Agro_SQL_SaveText(Fonte_Cod) & "' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Allegati_Documenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Allegati_Documenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    Throw New NotImplementedException

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    Throw New NotImplementedException

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function LeggixSincroAnalisi(ByVal analisi_testata_cod As Integer,
                                        ByVal Allegati_documenti_des As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.LeggixSincroAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select * from Allegati_Documenti  ")
            StrSQL.Append(" inner join Allegati_EntitaxDocumenti on Allegati_EntitaxDocumenti.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod  ")

            StrSQL.Append(" where analisi_testata_cod = " & Agro_SQL_SaveNum(analisi_testata_cod) & " ")
            StrSQL.Append(" AND    Allegati_documenti_des = '" & Agro_SQL_SaveText(Allegati_documenti_des) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '##############################################################################################
    Public Function UltimoID(ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal username_creazione As String = "",
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal data_upload As String = "",
                             Optional ByVal data_creazione As String = "",
                             Optional ByVal data_modifica As String = ""
                             ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.UltimoID()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT isnull(max(Allegati_Documenti_Cod),0) as MAX from Allegati_Documenti   ")

            If username_creazione <> "" Then
                StrSQL.AppendLine(" WHERE username_creazione = '" & Agro_SQL_SaveText(username_creazione) & "'  ")
                StrSQL.AppendLine(" AND username_modifica = '" & Agro_SQL_SaveText(username_modifica) & "'  ")
                StrSQL.AppendLine(" AND data_upload =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_upload) & "',121)")
                StrSQL.AppendLine(" AND data_creazione =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_creazione) & "',121)")
                StrSQL.AppendLine(" AND data_modifica =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_modifica) & "',121)")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows(0).Item("MAX")

    End Function

    Public Function EsisteDocumento_Da_Numero(ByVal Allegati_Documenti_Numero As String,
                                              ByVal Allegati_Documenti_Piva_Opzionale As String,
                                              ByVal Allegati_Documenti_CatCod_Opzionale As Integer,
                                              ByRef Allegati_Documenti_Cod_RITORNO As Integer,
                                              ByVal ObjParametri_Server As AgronicaCoreParametri,
                                              ByVal Fonte_Cod As Integer,
                                              Optional ByVal Allegati_Documenti_Ente_Cod As Integer = 0)

        Dim filtro As String = "  Allegati_Documenti_Numero = '" & Agro_SQL_SaveText(Allegati_Documenti_Numero) & "' "
        filtro &= "  AND          Allegati_Documenti_SuperUser = '" & ObjParametri_Server.PivaSuperUser & "' "

        If Allegati_Documenti_Piva_Opzionale <> "" Then
            filtro &= "  AND          Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva_Opzionale) & "' "
        End If
        If Allegati_Documenti_CatCod_Opzionale <> 0 Then
            filtro &= "  AND          Allegati_Documenti_CatCod = " & Agro_SQL_SaveNum(Allegati_Documenti_CatCod_Opzionale) & " "
        End If
        If Allegati_Documenti_Ente_Cod <> 0 Then
            filtro &= "  AND          Allegati_Documenti_Ente_Cod = " & Agro_SQL_SaveNum(Allegati_Documenti_Ente_Cod) & " "
        End If

        Dim dt As DataTable = Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    filtro, "", ObjParametri_Server, Fonte_Cod:=Fonte_Cod)

        If dt.Rows.Count > 0 Then
            Allegati_Documenti_Cod_RITORNO = dt.Rows(0).Item("Allegati_Documenti_Cod")
            Return True
        End If

        Return False

    End Function

    Public Function EsisteDocumento_Da_piva(ByVal Allegati_Documenti_Piva As String,
                                            ByVal Allegati_Documenti_CatCod_Opzionale As Integer,
                                            ByRef Allegati_Documenti_Cod_RITORNO As Integer,
                                            ByRef Allegati_Documenti_Numero As String,
                                            ByRef Validazione_Data As Date,
                                            ByVal ObjParametri_Server As AgronicaCoreParametri)

        Dim filtro As String = "  Allegati_Documenti_Piva = '" & Agro_SQL_SaveText(Allegati_Documenti_Piva) & "' "
        filtro &= "  AND          Allegati_Documenti_SuperUser = '" & ObjParametri_Server.PivaSuperUser & "' "

        If Allegati_Documenti_CatCod_Opzionale <> 0 Then
            filtro &= "  AND          Allegati_Documenti_CatCod = " & Allegati_Documenti_CatCod_Opzionale & " "
        End If

        Dim dt As DataTable = Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    filtro, "", ObjParametri_Server)

        If dt.Rows.Count = 1 Then
            Allegati_Documenti_Cod_RITORNO = dt.Rows(0).Item("Allegati_Documenti_Cod")
            If Not IsDBNull(dt.Rows(0).Item("Allegati_Documenti_Numero")) Then
                Allegati_Documenti_Numero = dt.Rows(0).Item("Allegati_Documenti_Numero")
            End If
            If Not IsDBNull(dt.Rows(0).Item("Validazione_Data")) Then
                Validazione_Data = dt.Rows(0).Item("Validazione_Data")
            End If
            Return True
        End If

        Return False

    End Function



    '##############################################################################################
    Public Function LeggiApp_Documenti(ByVal Solo_Non_Sincronizzati As Boolean,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.LeggiApp_Documenti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   App_Documenti ")
            'StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            'StrSQL.Append(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" WHERE  1 = 1 ")

            If Solo_Non_Sincronizzati Then
                'StrSQL.Append(" And ID Not In (Select Id_App From Alert_Elenco) ")
                StrSQL.Append(" And Importato_Data Is Null ")
                StrSQL.Append(" And APP_Ricette_Operazioni_ID = '' ")
                StrSQL.Append(" And APP_Ricette_Destinazioni_ID = '' ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ID ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiAllegatiDaRicettaDestinazione(ByVal pivaSuperUser As String,
                                                       ByVal piva As String,
                                                       ByVal sa_Cod As Int32,
                                                       ByVal appezza As Int32,
                                                       ByVal id_Imp As Int32,
                                                       ByVal ricettaOperazione_cod As Int32,
                                                       ByVal allegati_Documenti_CatCod As Int32,
                                                       ByVal selezione_Tabella As enumSelezioneVariabile,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal bAddPayload As Boolean = False
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Allegati_Documenti_R.LeggiAllegatiDaRicettaDestinazione()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            If String.IsNullOrWhiteSpace(pivaSuperUser) Then
                Throw New Exception("Parametro non corretto (PivaSuperUser obbligatorio)")
            End If

            Select Case selezione_Tabella
                Case enumSelezioneVariabile.Selezione_TabellaCompleta,
                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT AD.Allegati_Documenti_Cod ")
                    StrSQL.Append(" 	   , AD.Allegati_Documenti_Des ")
                    If bAddPayload Then
                        StrSQL.Append(" 	   , AD.allegatiDocumentiXML ")
                        StrSQL.Append(" 	   , AD.allegatiDocumentiText ")
                    End If
                    StrSQL.Append(" FROM Alert_Entita E ")
                    StrSQL.Append(" INNER JOIN Allegati_Documenti AD ")
                    StrSQL.Append(" 	ON E.Allegati_Documenti_Cod = AD.Allegati_Documenti_Cod ")
                    StrSQL.Append(" 	AND E.PivaSuperUser = AD.Allegati_Documenti_SuperUser ")

                    StrSQL.Append(String.Format(" WHERE E.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(pivaSuperUser)))
                    StrSQL.Append(String.Format("       AND E.Piva = '{0}' ", Agro_SQL_SaveText(piva)))
                    StrSQL.Append(String.Format("       AND E.Sa_Cod = {0} ", Agro_SQL_SaveNum(sa_Cod)))
                    StrSQL.Append(String.Format("       AND E.Appezza = {0} ", Agro_SQL_SaveNum(appezza)))
                    StrSQL.Append(String.Format("       AND E.Id_Imp = {0} ", Agro_SQL_SaveNum(id_Imp)))
                    StrSQL.Append(String.Format("       AND E.Ricetta_Operazione_cod = {0} ", Agro_SQL_SaveNum(ricettaOperazione_cod)))
                    StrSQL.Append(String.Format("       AND AD.Allegati_Documenti_CatCod = {0} ", Agro_SQL_SaveNum(allegati_Documenti_CatCod)))

                    If xOrderBy <> "" Then
                        StrSQL.Append(String.Format(" ORDER BY {0}", Agro_SQL_Save_xOrderBy(xOrderBy, objParametri)))
                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
