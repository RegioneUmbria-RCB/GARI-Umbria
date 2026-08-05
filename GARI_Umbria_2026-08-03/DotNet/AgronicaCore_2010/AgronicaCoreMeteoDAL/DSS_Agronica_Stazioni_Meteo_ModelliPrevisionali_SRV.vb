

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiRisultatoModello(ByVal tipo_sorgente As Integer,
                                          ByVal Stazione_Cod As Integer,
                                          ByVal Mod_Cod As Integer,
                                          ByVal Algoritmo_Cod As Integer,
                                          ByVal ParametriElaborazione As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataRow

        Dim NomeRoutine As String = "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable = Nothing

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    COALESCE(GSB_DataOraElaborazione, CAST('1900-01-01' AS DATE)) AS GSB_DataOraElaborazione ")
            Stb.AppendLine("    , COALESCE(RisultatoElaborazione, '') AS RisultatoElaborazione ")
            Stb.AppendLine("    , COALESCE(GSB_Flag, 0) AS GSB_Flag ")
            Stb.AppendLine("FROM DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV ")
            Stb.AppendLine("WHERE tipo_sorgente = " & tipo_sorgente)
            Stb.AppendLine("AND Stazione_Cod = " & Stazione_Cod)
            Stb.AppendLine("AND Mod_Cod = " & Mod_Cod)
            Stb.AppendLine("AND Algoritmo_Cod = " & Algoritmo_Cod)
            Stb.AppendLine("AND ParametriElaborazione = '" & Agro_SQL_SaveText(ParametriElaborazione) & "'")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine("AND Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine("AND Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return Nothing
        End If

        Return DT.Rows(0)

    End Function


    '##############################################################################################
    Public Function LeggiModelliDaElaborare(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0



            Stb.AppendLine(" Select  ")
            Stb.AppendLine("    dss.* ")
            Stb.AppendLine("  , mVeg.Av_Cod ")
            Stb.AppendLine("  , mVeg.Veg_Cod   ")
            Stb.AppendLine("  , mVeg.Attivo ")
            Stb.AppendLine(" From DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV dss ")
            Stb.AppendLine("  inner Join  ModelliXSpeciexAvversita mVeg  ")
            Stb.AppendLine("         On dss.Mod_Cod = mVeg.Mod_Cod ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" where dss.GSB_Flag = 1 ")
            Stb.AppendLine(" And mVeg.Attivo<>0 ")

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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

Public Class DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Class TableKey
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Mod_Cod As Integer
        Public Algoritmo_Cod As Integer
        Public ParametriElaborazione As String
    End Class

    '##############################################################################################
    Public Function Scrivi(
          ByVal tipo_sorgente As Integer _
        , ByVal Stazione_Cod As Integer _
        , ByVal Mod_Cod As Integer _
        , ByVal Algoritmo_Cod As Integer _
        , ByVal ParametriElaborazione As String _
        , ByVal GSB_Flag As Integer _
        , ByVal GSB_DataOraElaborazione As Date _
        , ByVal RisultatoElaborazione As String _
        , ByVal Qta_Ril As Decimal _
        , ByVal udm_cod As Integer _
        , ByVal Data_Riferimento As Date _
        , ByVal av_cod As Integer _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
    ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
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
            strSQL.Length = 0
            strSQL.AppendLine("INSERT DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV ")
            strSQL.AppendLine(" ( [tipo_sorgente] ")
            strSQL.AppendLine(" , [Stazione_Cod] ")
            strSQL.AppendLine(" , [Mod_Cod] ")
            strSQL.AppendLine(" , [Algoritmo_Cod] ")
            strSQL.AppendLine(" , [ParametriElaborazione] ")
            strSQL.AppendLine(" , [GSB_Flag] ")
            strSQL.AppendLine(" , [GSB_DataOraElaborazione] ")
            strSQL.AppendLine(" , [RisultatoElaborazione] ")

            strSQL.AppendLine(" , Inviato, datainvio ")
            strSQL.AppendLine(" , Data_Creazione, Data_Modifica ")
            strSQL.AppendLine(" , UserName_Creazione, UserName_Modifica ")
            'strSQL.AppendLine(" , Validita_Inizio, Validita_Fine ")

            strSQL.AppendLine(" , [Qta_Ril] ")
            strSQL.AppendLine(" , [udm_cod] ")
            strSQL.AppendLine(" , [Data_Riferimento] ")
            strSQL.AppendLine(" , [av_cod] ")
            strSQL.AppendLine(" ) ")

            strSQL.AppendLine("VALUES ( ")

            strSQL.AppendLine("   " & Agro_SQL_SaveNum(tipo_sorgente) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(Stazione_Cod) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(Mod_Cod) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(Algoritmo_Cod) & " ")
            strSQL.AppendLine(" , '" & Agro_SQL_SaveText(ParametriElaborazione) & "' ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(GSB_Flag) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveDate(GSB_DataOraElaborazione) & " ")
            strSQL.AppendLine(" , '" & Agro_SQL_SaveText(RisultatoElaborazione) & "' ")

            strSQL.AppendLine(" , 0, NULL ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveDate(Data_creazione) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveDate(Data_modifica) & " ")
            strSQL.AppendLine(" , '" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSQL.AppendLine(" , '" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(Qta_Ril) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(udm_cod) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
            strSQL.AppendLine(" , " & Agro_SQL_SaveNum(av_cod) & " ")

            strSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function RichiediElaborazioneModelli(ByVal listaChiavi As List(Of TableKey), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        If listaChiavi Is Nothing OrElse listaChiavi.Count = 0 Then
            Return False
        End If

        Dim NomeRoutine As String = "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_W.RichiediElaborazioneModelli()"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder

        Dim xRisp As Boolean = False

        Try

            strSQL.Length = 0
            strSQL.AppendLine("UPDATE DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV ")
            strSQL.AppendLine("SET GSB_Flag = 1 ")
            strSQL.AppendLine("WHERE ")

            Dim first_el As Boolean = True
            For Each tk In listaChiavi

                If Not first_el Then
                    strSQL.AppendLine("OR ")
                End If
                first_el = False

                strSQL.AppendLine("( ")
                strSQL.AppendLine("tipo_Sorgente = " & tk.Tipo_Sorgente & " ")
                strSQL.AppendLine("AND stazione_cod = " & tk.Stazione_Cod & " ")
                strSQL.AppendLine("AND Mod_cod = " & tk.Mod_Cod & " ")
                strSQL.AppendLine("AND Algoritmo_Cod = " & tk.Algoritmo_Cod & " ")
                strSQL.AppendLine("AND ParametriElaborazione = '" & Agro_SQL_SaveText(tk.ParametriElaborazione) & "'")
                strSQL.AppendLine(") ")
            Next

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function InserisciElaborazioneModelli(ByVal listaChiavi As List(Of TableKey), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        If listaChiavi Is Nothing OrElse listaChiavi.Count = 0 Then
            Return False
        End If

        Dim NomeRoutine As String = "DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_W.RichiediElaborazioneModelli()"

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder

        Dim xRisp As Boolean = False

        'Dim data As DateTime = Date.Now
        'Dim username As String = objParametri.UsernameOperazione

        Try

            strSQL.Length = 0
            strSQL.AppendLine("INSERT INTO DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV ")
            strSQL.AppendLine("(tipo_sorgente, Stazione_Cod, Mod_Cod, Algoritmo_Cod, ParametriElaborazione, GSB_Flag, GSB_DataOraElaborazione, RisultatoElaborazione) ")
            'strSQL.AppendLine(", Inviato, datainvio, Data_Creazione, Data_Modifica, UserName_Creazione, UserName_Modifica, Validita_Inizio, Validita_Fine) ")
            strSQL.AppendLine("VALUES ")

            Dim first_el As Boolean = True
            For Each tk In listaChiavi

                If Not first_el Then
                    strSQL.AppendLine(", ")
                End If
                first_el = False

                strSQL.AppendLine("(" & Agro_SQL_SaveNum(tk.Tipo_Sorgente) & ", " & Agro_SQL_SaveNum(tk.Stazione_Cod) & ", " &
                                  Agro_SQL_SaveNum(tk.Mod_Cod) & ", " & Agro_SQL_SaveNum(tk.Algoritmo_Cod) & ", '" & Agro_SQL_SaveText(tk.ParametriElaborazione) & "', 1, NULL, NULL) ")
                'strSQL.AppendLine(", 0, NULL, " & Agro_SQL_SaveDate(data) & ", " & Agro_SQL_SaveDate(data) &
                '                  ", '" & Agro_SQL_SaveText(username) & "', '" & Agro_SQL_SaveText(username) & "' ")
            Next

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function ImpostaRisultatoSuModello(
          ByVal tipo_sorgente As Integer _
        , ByVal Stazione_Cod As Integer _
        , ByVal Mod_Cod As Integer _
        , ByVal Algoritmo_Cod As Integer _
        , ByVal ParametriElaborazione As String _
        , ByVal RisultatoElaborazione As String _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = ""
    ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
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
            strSQL.Length = 0
            strSQL.Append(" UPDATE DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV " & vbCrLf)


            strSQL.Append("  SET")

            strSQL.Append("   [GSB_Flag] = 0 " & vbCrLf)
            strSQL.Append("  ,[GSB_DataOraElaborazione] = " & Agro_SQL_SaveDateTime(Now) & " " & vbCrLf)
            strSQL.Append("  ,[RisultatoElaborazione] = '" & Agro_SQL_SaveText(RisultatoElaborazione) & "' " & vbCrLf)



            strSQL.Append("  , [Data_Modifica] = " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSQL.Append("  , [UserName_Modifica] = 'agronica' ")
            strSQL.Append("              ")

            strSQL.Append(" WHERE  ")

            strSQL.Append("     tipo_Sorgente = " & Agro_SQL_SaveNum(tipo_sorgente) & " " & vbCrLf)
            strSQL.Append(" AND  stazione_cod = " & Agro_SQL_SaveNum(Stazione_Cod) & " " & vbCrLf)
            strSQL.Append(" AND  Mod_cod = " & Agro_SQL_SaveNum(Mod_Cod) & " " & vbCrLf)
            strSQL.Append(" AND  Algoritmo_Cod = " & Agro_SQL_SaveNum(Algoritmo_Cod) & " " & vbCrLf)
            strSQL.Append(" AND ParametriElaborazione = '" & Agro_SQL_SaveText(ParametriElaborazione) & "'" & vbCrLf)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSQL.ToString, NomeRoutine)
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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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

