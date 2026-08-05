Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ParametriValoriGenerico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_ParametriXValori( _
                                        ByVal NomeTab As String, _
                                        ByVal Documento_Cod As Integer?, _
                                        ByVal PrmXMod_Cod As Integer?, _
                                        ByVal DataOraRilevazione As DateTime?, _
                                        ByVal Utente As String, _
                                        ByVal String_Valore As String, _
                                        ByVal DateTime_Valore As DateTime?, _
                                        ByVal Int_Valore As Integer?, _
                                        ByVal Float_Valore As Double?, _
                                        ByVal Provvisorio As Boolean?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Leggi_ParametriValori"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Documento_Cod, PrmXMod_Cod, DataOraRilevazione, Utente, String_Valore, DateTime_Valore, Int_Valore, Float_Valore, Provvisorio " + vbCrLf)
            strSQL.Append(" FROM " & NomeTab & " ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(PrmXMod_Cod) Then
                strSQL.Append(" AND PrmXMod_Cod = " & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))
            End If

            If Not IsNothing(DataOraRilevazione) Then
                strSQL.Append(" AND DataOraRilevazione = " & Agro_SQL_SaveDateTime_NULL(DataOraRilevazione))
            End If

            If Not IsNothing(String_Valore) Then
                strSQL.Append(" AND String_Valore = " & Agro_SQL_SaveText_NULL(String_Valore))
            End If

            If Not IsNothing(DateTime_Valore) Then
                strSQL.Append(" AND DateTime_Valore = " & Agro_SQL_SaveDateTime_NULL(DateTime_Valore))
            End If

            If Not IsNothing(Int_Valore) Then
                strSQL.Append(" AND Int_Valore = " & Agro_SQL_SaveNum_NULL(Int_Valore))
            End If

            If Not IsNothing(Float_Valore) Then
                strSQL.Append(" AND Float_Valore = " & Agro_SQL_SaveNum_NULL(Float_Valore))
            End If

            If Not IsNothing(Provvisorio) Then
                strSQL.Append(" AND Provvisorio = " & Agro_SQL_SaveText_NULL(Provvisorio))
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


    '##############################################################################################
    Public Function Leggi_ParametriXValori( _
                                        ByVal NomeTab As String, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal DaValutare As Boolean, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Leggi_ParametriValori"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT pv.PivaSuperUser, pv.Documento_Cod, pv.PrmXMod_Cod, pv.DataOraRilevazione, pv.Utente, pv.String_Valore, pv.DateTime_Valore, pv.Int_Valore, pv.Float_Valore, pv.Provvisorio " + vbCrLf)
            strSQL.Append(" FROM " & NomeTab & " as pv")

            strSQL.Append(" INNER JOIN LCQ_ParametriXModelli as pxm ")
            strSQL.Append(" ON pv.PivaSuperUser = pxm.PivaSuperUser ")
            strSQL.Append(" AND pv.PrmXMod_Cod = pxm.ParametriXModelli_Cod ")

            strSQL.Append(" INNER JOIN LCQ_Parametri as p ")
            strSQL.Append(" ON p.PivaSuperUser = pxm.PivaSuperUser ")
            strSQL.Append(" AND p.Parametri_Cod = pxm.Parametro_Cod ")

            strSQL.Append(" WHERE pv.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Documento_Cod) Then
                strSQL.Append(" AND pv.Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            End If

            If Not IsNothing(DaValutare) Then
                strSQL.Append(" AND p.DaValutare = " & Agro_SQL_SaveText_NULL(DaValutare.ToString()))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   pv.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   pv.Inviato =-1 ")
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

    Public Function Verifica_ValEsiste( _
                                    ByVal NomeTab As String, _
                                    ByVal Documento_Cod As Integer, _
                                    ByVal PrmXMod_Cod As Integer, _
                                    ByVal DataOraRilevazione As DateTime?, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_R.Verifica_ValEsiste"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM " & NomeTab & " " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Documento_Cod = " & Agro_SQL_SaveNum_NULL(Documento_Cod))
            strSQL.Append(" AND PrmXMod_Cod = " & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))

            If Not IsNothing(DataOraRilevazione) Then
                strSQL.Append(" AND DataOraRilevazione = " & Agro_SQL_SaveDateTime_NULL(DataOraRilevazione))
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


Public Class ParametriValoriGenerico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal NomeTab As String, _
                                        ByVal Documento_Cod As Integer, _
                                        ByVal PrmXMod_Cod As Integer, _
                                        ByVal DataOraRilevazione As DateTime?, _
                                        ByVal Utente As String, _
                                        ByVal String_Valore As String, _
                                        ByVal DateTime_Valore As DateTime?, _
                                        ByVal Int_Valore As Integer?, _
                                        ByVal Float_Valore As Double?, _
                                        ByVal Provvisorio As Boolean?, _
                                        Optional ByVal Data_creazione As Date = #2/1/1900#, _
                                        Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                        Optional ByVal username_creazione As String = "", _
                                        Optional ByVal username_modifica As String = "" _
                                        ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO " & NomeTab + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,             Documento_Cod, ")
            StrSQL.Append("              PrmXMod_Cod ,              DataOraRilevazione , ")
            StrSQL.Append("              Utente ,                   String_Valore , ")
            StrSQL.Append("              DateTime_Valore ,          Int_Valore , ")
            StrSQL.Append("              Float_Valore,              Provvisorio, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(Documento_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))

            'la data, se il prm è di testata, è inutile e mi arriva nothing, ci metto quindi la data di modifica
            If IsNothing(DataOraRilevazione) Then
                StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(DateTime.Now()))
            Else
                StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(DataOraRilevazione))
            End If

            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Utente)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(String_Valore)))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DateTime_Valore)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Int_Valore)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Float_Valore)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Provvisorio)))


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

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal NomeTab As String, _
                ByVal Old_Documento_Cod As Integer, _
                ByVal Old_PrmXMod_Cod As Integer, _
                ByVal Old_DataOraRilevazione As DateTime?, _
                ByVal New_Utente As String, _
                ByVal New_String_Valore As String, _
                ByVal New_DateTime_Valore As DateTime?, _
                ByVal New_Int_Valore As Integer?, _
                ByVal New_Float_Valore As Double?, _
                ByVal Provvisorio As Boolean?, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.Modifica()"

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
            StrSQL.Append(" UPDATE " & NomeTab & " SET" + vbCrLf)

            'se il parametro è di sola testata, non mi interessa la dataora di rilevazione e l'utente
            If Not IsNothing(Old_DataOraRilevazione) Then
                'StrSQL.Append(" DataOraRilevazione = " & Agro_SQL_SaveDateTime_NULL(New_DataOraRilevazione) & vbCrLf)
                StrSQL.Append(" Utente = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Utente)) & vbCrLf)
                StrSQL.Append(" , ")
            Else

            End If

            StrSQL.Append(" String_Valore = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_String_Valore)) & vbCrLf)
            StrSQL.Append(", DateTime_Valore = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DateTime_Valore)) & vbCrLf)
            StrSQL.Append(", Int_Valore = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Int_Valore)) & vbCrLf)
            StrSQL.Append(", Float_Valore = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Float_Valore)) & vbCrLf)
            StrSQL.Append(", Provvisorio = " & Agro_SQL_SaveText_NULL(NothingToDBNull(Provvisorio)))

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))
            StrSQL.Append("	AND PrmXMod_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_PrmXMod_Cod))

            'se il parametro è di sola testata, la chiave dataora di rilevazione viene ignorata
            If Not IsNothing(Old_DataOraRilevazione) Then
                StrSQL.Append("	AND DataOraRilevazione =		" & Agro_SQL_SaveDateTime_NULL(Old_DataOraRilevazione))
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

    '##############################################################################################
    Public Function ModificaOrario(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal NomeTab As String, _
                ByVal Old_Documento_Cod As Integer, _
                ByVal Old_DataOraRilevazione As DateTime, _
                ByVal New_DataOraRilevazione As DateTime, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ModificaOrario()"

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
            StrSQL.Append(" UPDATE " & NomeTab & " SET" + vbCrLf)

            StrSQL.Append(" DataOraRilevazione = " & Agro_SQL_SaveDateTime_NULL(New_DataOraRilevazione) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))
            StrSQL.Append("	AND DataOraRilevazione =		" & Agro_SQL_SaveDateTime_NULL(Old_DataOraRilevazione))

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

    '##############################################################################################
    Public Function ModificaUtente(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                    ByVal NomeTab As String, _
                                    ByVal Old_Documento_Cod As Integer, _
                                    ByVal Old_DataOraRilevazione As DateTime, _
                                    ByVal New_Utente As String, _
                                    Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                    Optional ByVal username_modifica As String = "" _
                                    ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.ModificaUtente()"

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
            StrSQL.Append(" UPDATE " & NomeTab & " SET" + vbCrLf)

            StrSQL.Append(" Utente = " & Agro_SQL_SaveText_NULL(New_Utente) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Documento_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_Documento_Cod))
            StrSQL.Append("	AND DataOraRilevazione =		" & Agro_SQL_SaveDateTime_NULL(Old_DataOraRilevazione))

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
                            ByVal NomeTab As String, _
                            ByVal Documento_Cod As Integer, _
                            ByVal PrmXMod_Cod As Integer, _
                            ByVal DataOraRilevazione As DateTime, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.Cancella()"

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
                StrSQL.Append(" DELETE FROM " & NomeTab & " " + vbCrLf)
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Documento_Cod))
                StrSQL.Append("	AND PrmXMod_Cod =		" & Agro_SQL_SaveNum_NULL(PrmXMod_Cod))
                StrSQL.Append("	AND DataOraRilevazione =		" & Agro_SQL_SaveDateTime_NULL(DataOraRilevazione))
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
    Public Function CancellaDocumento(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal NomeTab As String, _
                            ByVal Documento_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriValori_W.CancellaDocumento()"

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
                StrSQL.Append(" DELETE FROM " & NomeTab & " " + vbCrLf)
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Documento_Cod =		" & Agro_SQL_SaveNum_NULL(Documento_Cod))
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