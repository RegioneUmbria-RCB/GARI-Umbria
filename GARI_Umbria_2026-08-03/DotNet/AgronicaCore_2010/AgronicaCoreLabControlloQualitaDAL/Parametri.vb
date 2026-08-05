Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Parametri_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_Parametri( _
                                        ByVal Parametri_Cod As Integer?, _
                                        ByVal Nome As String, _
                                        ByVal DaValutare As Boolean?, _
                                        ByVal CampoCalcolato As Boolean?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_R.Leggi_Parametri"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Parametri_Cod, Nome, Descrizione, DaValutare, CampoCalcolato, PrmTipi_CodTipo, UnitaMisura, Param1, Param2 " + vbCrLf)
            strSQL.Append(" FROM LCQ_Parametri ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Parametri_Cod) Then
                strSQL.Append(" AND Parametri_Cod = " & Agro_SQL_SaveNum_NULL(Parametri_Cod))
            End If

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))
            End If

            If Not IsNothing(DaValutare) Then
                strSQL.Append(" AND DaValutare = " & Agro_SQL_SaveText_NULL(DaValutare.ToString()))
            End If

            If Not IsNothing(CampoCalcolato) Then
                strSQL.Append(" AND CampoCalcolato = " & Agro_SQL_SaveText_NULL(CampoCalcolato.ToString()))
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
    Public Function Leggi_Parametri( _
                                        ByVal Parametri_Cod As Integer?, _
                                        ByVal Nome As String, _
                                        ByVal DaValutare As Boolean?, _
                                        ByVal CampoCalcolato As Boolean?, _
                                        ByVal Modello_Codice As String, _
                                        ByVal Modello_Revisione As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_R.Leggi_Parametri"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT p.PivaSuperUser, p.Parametri_Cod, p.Nome, p.Descrizione, p.DaValutare, p.CampoCalcolato, p.PrmTipi_CodTipo, p.UnitaMisura, p.Param1, p.Param2 " + vbCrLf)
            strSQL.Append(" FROM LCQ_Parametri AS p ")
            strSQL.Append("    INNER JOIN LCQ_ParametriXModelli AS pm " & vbCrLf)
            strSQL.Append("    ON pm.PivaSuperUser=p.PivaSuperUser " & vbCrLf)
            strSQL.Append("    AND pm.Parametro_Cod=p.Parametri_Cod " & vbCrLf)

            strSQL.Append(" WHERE p.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Parametri_Cod) Then
                strSQL.Append(" AND p.Parametri_Cod = " & Agro_SQL_SaveNum_NULL(Parametri_Cod))
            End If

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND p.Nome = " & Agro_SQL_SaveText_NULL(Nome))
            End If

            If Not IsNothing(DaValutare) Then
                strSQL.Append(" AND p.DaValutare = " & Agro_SQL_SaveText_NULL(DaValutare.ToString()))
            End If

            If Not IsNothing(CampoCalcolato) Then
                strSQL.Append(" AND p.CampoCalcolato = " & Agro_SQL_SaveText_NULL(CampoCalcolato.ToString()))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND pm.Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND pm.Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   p.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   p.Inviato =-1 ")
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

    Function Verifica_PrmEsiste( _
                                    ByVal Nome As String, _
                                    ByVal DaValutare As Boolean?, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_R.Verifica_PrmEsiste()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_Parametri " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))

            If Not IsNothing(DaValutare) Then
                strSQL.Append(" AND DaValutare = " & Agro_SQL_SaveText_NULL(DaValutare.ToString()))
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

    Function Verifica_PrmInUso( _
                                   ByVal Parametro_Cod As Integer, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriRev_R.Verifica_PrmInUso()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_ParametriXModelli " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Parametro_Cod = " & Agro_SQL_SaveText_NULL(Parametro_Cod))

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


'#################################################################
'#################################################################
'#################################################################

Public Class Parametri_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ' non metto il campoCalcolato in quanto viene gestito solo da DB
    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Parametri_Cod As Integer, _
                            ByVal New_Parametri_Cod As Integer, _
                            ByVal New_Nome As String, _
                            ByVal New_Descrizione As String, _
                            ByVal New_DaValutare As Boolean, _
                            ByVal New_PrmTipi_CodTipo As Integer, _
                            ByVal New_UnitaMisura As String, _
                            ByVal New_Param1 As String, _
                            ByVal New_Param2 As String, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_R.Modifica()"

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
            StrSQL.Append(" UPDATE LCQ_Parametri SET" + vbCrLf)

            StrSQL.Append(" Parametri_Cod = " & Agro_SQL_SaveText_NULL(New_Parametri_Cod) & vbCrLf)
            StrSQL.Append(", Nome = " & Agro_SQL_SaveText_NULL(New_Nome) & vbCrLf)
            StrSQL.Append(", DaValutare = " & Agro_SQL_SaveText_NULL(New_DaValutare) & vbCrLf)
            'StrSQL.Append(", CampoCalcolato = " & Agro_SQL_SaveText_NULL(New_CampoCalcolato) & vbCrLf)
            StrSQL.Append(", Descrizione = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Descrizione)) & vbCrLf)
            StrSQL.Append(", PrmTipi_CodTipo = " & Agro_SQL_SaveText_NULL(New_PrmTipi_CodTipo) & vbCrLf)
            StrSQL.Append(", UnitaMisura = " & Agro_SQL_SaveText_NULL(New_UnitaMisura) & vbCrLf)
            StrSQL.Append(", Param1 = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Param1)) & vbCrLf)
            StrSQL.Append(", Param2 = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Param2)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Parametri_Cod =		" & Agro_SQL_SaveText_NULL(Old_Parametri_Cod))


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
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Parametri_Cod As Integer, _
                            ByVal Nome As String, _
                            ByVal Descrizione As String, _
                            ByVal DaValutare As Boolean, _
                            ByVal CampoCalcolato As Boolean, _
                            ByVal PrmTipi_CodTipo As Integer, _
                            ByVal UnitaMisura As String, _
                            ByVal Param1 As String, _
                            ByVal Param2 As String, _
                            Optional ByVal Data_creazione As Date = #2/1/1900#, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_R.Scrivi()"

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
            StrSQL.Append(" INSERT INTO  LCQ_Parametri" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,                 Parametri_Cod, ")
            StrSQL.Append("              Nome,                          Descrizione, ")
            StrSQL.Append("              DaValutare,                    CampoCalcolato, ")
            StrSQL.Append("              PrmTipi_CodTipo,               UnitaMisura,  ")
            StrSQL.Append("              Param1,                        Param2, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(Parametri_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Nome))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Descrizione)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(DaValutare))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(CampoCalcolato))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(PrmTipi_CodTipo))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(UnitaMisura))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Param1)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Param2)))


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


    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                             ByVal Parametri_Cod As Integer, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_W.Cancella()"

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
                StrSQL.Append(" DELETE FROM LCQ_Parametri ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Parametri_Cod =		" & Agro_SQL_SaveNum_NULL(Parametri_Cod))
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




End Class


