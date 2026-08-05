


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class FF_TrackedData_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
            ByVal FF_TrackedData_Cod As Integer, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " + vbCrLf)
            Stb.Append(" FROM FF_TrackedData " + vbCrLf)

            Stb.Append(" WHERE FF_TrackedData_Cod =  " & Agro_SQL_SaveNum(FF_TrackedData_Cod) & vbCrLf)

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



    Public Function VerificaEsistenza(
            ByVal TrackedCodes As String,
            ByVal cCalCod As Integer,
            ByVal cIdMovDet As Integer,
            ByVal tipo As Integer,
            ByVal FromOutToIn As Boolean,
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


            Stb.Append("select * " & vbCrLf)
            Stb.Append(" from FF_TrackedData td " & vbCrLf)
            Stb.Append(" where td.FF_TrackedData_TrackedCodes =  '" & Agro_SQL_SaveText(TrackedCodes) & "'" & vbCrLf)
            Stb.Append(" and td.Cal_Cod = " & Agro_SQL_SaveNum(cCalCod) & vbCrLf)
            Stb.Append(" and td.IdMovDet = " & Agro_SQL_SaveNum(cIdMovDet) & vbCrLf)
            Stb.Append(" and td.Tipo = " & Agro_SQL_SaveNum(tipo) & vbCrLf)
            Stb.Append(" and td.FromOutToIn = " & Agro_SQL_SaveBoolStrToInt(FromOutToIn.ToString()) & vbCrLf)

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

    Public Function LeggiCache( _
            ByVal TrackedCodes As String, _
            ByVal tipo As Integer, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append("select * " & vbCrLf)
            Stb.Append(" from FF_TrackedData td " & vbCrLf)
            Stb.Append("    inner join FF_TrackedData_Agenda ta " & vbCrLf)
            Stb.Append("        on td.FF_TrackedData_Cod = ta.FF_TrackedData_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join Agenda a " & vbCrLf)
            Stb.Append("        on a.piva = ta.piva  " & vbCrLf)
            Stb.Append("        and a.sa_Cod = ta.sa_cod " & vbCrLf)
            Stb.Append("        and a.Id_Agenda = ta.id_Agenda " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where td.TrackedCodes = @c " & vbCrLf)
            Stb.Append(" and td.Tipo = @t " & vbCrLf)
            Stb.Append(" ")


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

Public Class FF_TrackedData_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                          ByVal FF_TrackedData_Cod As Integer _
                        , ByVal FF_TrackedData_Des As String _
                        , ByVal TrackedCodes As String _
                        , ByVal Cal_Cod As Integer _
                        , ByVal IdMovDet As Integer _
                        , ByVal Tipo As Integer _
                        , ByVal FromOutToIn As Boolean _
                        , ByVal Validita_Inizio As DateTime _
                        , ByVal Validita_Fine As DateTime,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
        Dim Stb As New System.Text.StringBuilder
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
            Stb.Length = 0
            Stb.Append(" INSERT FF_TrackedData " + vbCrLf)


            Stb.Append("              (")


            Stb.Append("   [FF_TrackedData_Cod] " & vbCrLf)
            Stb.Append("  ,[FF_TrackedData_Des] " & vbCrLf)
            Stb.Append("  ,[FF_TrackedData_TrackedCodes] " & vbCrLf)
            Stb.Append("  ,[Cal_Cod] " & vbCrLf)
            Stb.Append("  ,[IdMovDet] " & vbCrLf)
            Stb.Append("  ,[Tipo] " & vbCrLf)
            Stb.Append("  ,[FromOutToIn], " & vbCrLf)

            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append("  " & Agro_SQL_SaveNum(FF_TrackedData_Cod) & " " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(FF_TrackedData_Des) & "' " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(TrackedCodes) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Cal_Cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(IdMovDet) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Tipo) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveBoolStrToInt(FromOutToIn.ToString()) & " " & vbCrLf)

            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(") ")

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







    '#################################################################
    Public Function Cancella( _
                            ByVal FF_TrackedData_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
                Stb.Append(" UPDATE FF_TrackedData ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
                Stb.Append(" AND     FF_TrackedData_cod =  " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
            Else
                Stb.Append(" DELETE FROM FF_TrackedData ")
                Stb.Append(" WHERE FF_TrackedData_cod =  " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
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



