Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class StandardDettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_StandardDettagli( _
                                    ByVal Standard_Nome As String, _
                                    ByVal Standard_Revisione As String, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal PrmXMod_Cod As Integer?, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_R.Leggi_StandardTutti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, PrmXMod_Cod, String_LimiteInf, String_LimiteSup, DateTime_LimiteInf, DateTime_LimiteSup, Int_LimiteInf, Int_LimiteSup, Float_LimiteInf, Float_LimiteSup" + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardDettagli " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Standard_Nome) Then
                strSQL.Append(" AND Standard_Nome = " & Agro_SQL_SaveText_NULL(Standard_Nome))
            End If

            If Not IsNothing(Standard_Revisione) Then
                strSQL.Append(" AND Standard_Revisione = " & Agro_SQL_SaveText_NULL(Standard_Revisione))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(PrmXMod_Cod) Then
                strSQL.Append(" AND PrmXMod_Cod = " & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Function Verifica_StdUsaPrm( _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal PrmXMod_Cod As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_R.Verifica_StdUsaPrm()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardDettagli " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            strSQL.Append(" AND PrmXMod_Cod = " & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class StandardDettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Standard_Nome As String, _
                ByVal Standard_Revisione As String, _
                ByVal Modello_Codice As String, _
                ByVal Modello_Revisione As String, _
                ByVal PrmXMod_Cod As Integer, _
                ByVal String_LimiteInf As String, _
                ByVal String_LimiteSup As String, _
                ByVal DateTime_LimiteInf As DateTime?, _
                ByVal DateTime_LimiteSup As DateTime?, _
                ByVal Int_LimiteInf As Integer?, _
                ByVal Int_LimiteSup As Integer?, _
                ByVal Float_LimiteInf As Double?, _
                ByVal Float_LimiteSup As Double?, _
                Optional ByVal Data_creazione As Date = #2/1/1900#, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_creazione As String = "", _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardDettagli_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO  LCQ_StandardDettagli" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,             Standard_Nome, ")
            StrSQL.Append("              Standard_Revisione,        Modello_Codice, ")
            StrSQL.Append("              Modello_Revisione,         PrmXMod_Cod, ")
            StrSQL.Append("              String_LimiteInf,          String_LimiteSup, ")
            StrSQL.Append("              DateTime_LimiteInf,        DateTime_LimiteSup, ")
            StrSQL.Append("              Int_LimiteInf,             Int_LimiteSup, ")
            StrSQL.Append("              Float_LimiteInf,           Float_LimiteSup, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Standard_Nome))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Standard_Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Codice))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(String_LimiteInf)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(String_LimiteSup)))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DateTime_LimiteInf)))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DateTime_LimiteSup)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Int_LimiteInf)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Int_LimiteSup)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Float_LimiteInf)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Float_LimiteSup)))


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '  Marco Grilli, 21/10/2014 15:40:36: appositamente non ho messo il codice articolo in quanto non deve essere modificabile
    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Standard_Nome As String, _
                ByVal Old_Standard_Revisione As String, _
                ByVal Old_Modello_Codice As String, _
                ByVal Old_Modello_Revisione As String, _
                ByVal Old_PrmXMod_Cod As Integer, _
                ByVal New_String_LimiteInf As String, _
                ByVal New_String_LimiteSup As String, _
                ByVal New_DateTime_LimiteInf As DateTime?, _
                ByVal New_DateTime_LimiteSup As DateTime?, _
                ByVal New_Int_LimiteInf As Integer?, _
                ByVal New_Int_LimiteSup As Integer?, _
                ByVal New_Float_LimiteInf As Double?, _
                ByVal New_Float_LimiteSup As Double?, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardDettagli_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_StandardDettagli SET" + vbCrLf)

            StrSQL.Append(" String_LimiteInf = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_String_LimiteInf)) & vbCrLf)
            StrSQL.Append(", String_LimiteSup = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_String_LimiteSup)) & vbCrLf)
            StrSQL.Append(", DateTime_LimiteInf = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DateTime_LimiteInf)) & vbCrLf)
            StrSQL.Append(", DateTime_LimiteSup = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DateTime_LimiteSup)) & vbCrLf)
            StrSQL.Append(", Int_LimiteInf = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Int_LimiteInf)) & vbCrLf)
            StrSQL.Append(", Int_LimiteSup = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Int_LimiteSup)) & vbCrLf)
            StrSQL.Append(", Float_LimiteInf = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Float_LimiteInf)) & vbCrLf)
            StrSQL.Append(", Float_LimiteSup = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Float_LimiteSup)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Standard_Nome =		" & Agro_SQL_SaveText_NULL(Old_Standard_Nome))
            StrSQL.Append("	AND Standard_Revisione =		" & Agro_SQL_SaveText_NULL(Old_Standard_Revisione))
            StrSQL.Append("	AND Modello_Codice =		" & Agro_SQL_SaveText_NULL(Old_Modello_Codice))
            StrSQL.Append("	AND Modello_Revisione =		" & Agro_SQL_SaveText_NULL(Old_Modello_Revisione))
            StrSQL.Append("	AND PrmXMod_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_PrmXMod_Cod))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '#################################################################
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Standard_Nome As String, _
                            ByVal Standard_Revisione As String, _
                            ByVal Modello_Codice As String, _
                            ByVal Modello_Revisione As String, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardDettagli_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM LCQ_StandardDettagli ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Standard_Nome =		" & Agro_SQL_SaveText_NULL(Standard_Nome))
                StrSQL.Append("	AND Standard_Revisione =		" & Agro_SQL_SaveText_NULL(Standard_Revisione))
                StrSQL.Append("	AND Modello_Codice =		" & Agro_SQL_SaveText_NULL(Modello_Codice))
                StrSQL.Append("	AND Modello_Revisione =		" & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Standard_Nome As String, _
                            ByVal Standard_Revisione As String, _
                            ByVal Modello_Codice As String, _
                            ByVal Modello_Revisione As String, _
                            ByVal PrmXMod_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardDettagli_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM LCQ_StandardDettagli ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Standard_Nome =		" & Agro_SQL_SaveText_NULL(Standard_Nome))
                StrSQL.Append("	AND Standard_Revisione =		" & Agro_SQL_SaveText_NULL(Standard_Revisione))
                StrSQL.Append("	AND Modello_Codice =		" & Agro_SQL_SaveText_NULL(Modello_Codice))
                StrSQL.Append("	AND Modello_Revisione =		" & Agro_SQL_SaveText_NULL(Modello_Revisione))
                StrSQL.Append("	AND PrmXMod_Cod =		" & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
End Class
