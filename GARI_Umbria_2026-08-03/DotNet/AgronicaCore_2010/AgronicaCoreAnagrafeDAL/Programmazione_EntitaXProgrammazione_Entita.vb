Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Programmazione_EntitaXProgrammazione_Entita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Lettura libera basata su filtro aggiuntivo
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo">impostare il filtro senza clausola "where" che viene aggiunta dalla funzione</param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi(
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
            Stb.Append(" FROM Programmazione_EntitaXProgrammazione_Entita " + vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            Else
                Throw New Exception("impostare il filtro")
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


    ''' <summary>
    ''' Leggi i planning di destinazione dato il planning di partenza
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo">impostare il filtro senza clausola "where" che viene aggiunta dalla funzione</param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi(
        ByVal Programmazione_cod_From As Integer,
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

            Stb.AppendLine(" SELECT e.programmazione_cod as programmazione_cod_to ")
            Stb.AppendLine(" , pepe.programmazione_entita_cod_to ")
            Stb.AppendLine(" , pt.Programmazione_Cod_Padre ")
            Stb.AppendLine(" FROM Programmazione_EntitaXProgrammazione_Entita pepe ")
            Stb.AppendLine(" inner join  Programmazione_Entita  e on pepe.Programmazione_Entita_cod_to = e.programmazione_entita_cod ")
            Stb.AppendLine(" inner join  Programmazione_Testata  pt on pepe.Programmazione_cod_From = pt.Programmazione_Cod ")
            Stb.AppendLine(" WHERE pepe.Programmazione_cod_From = " & Programmazione_cod_From)

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   pepe.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   pepe.Inviato =-1 ")
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

Public Class Programmazione_EntitaXProgrammazione_Entita_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
          ByVal Piva_SuperUser_From As String _
        , ByVal Piva_SuperUser_To As String _
        , ByVal Programmazione_Entita_Cod_From As Integer _
        , ByVal Programmazione_Entita_Cod_To As Integer _
        , ByVal Programmazione_Cod_From As Integer _
        , ByVal Programmazione_Cod_To As Integer _
        , ByVal TipoRelazione As Integer _
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
            Stb.Append(" INSERT Programmazione_EntitaXProgrammazione_Entita " + vbCrLf)

            Stb.Append("              (")

            Stb.Append("   [Piva_SuperUser_From] " & vbCrLf)
            Stb.Append("  ,[Piva_SuperUser_To] " & vbCrLf)
            Stb.Append("  ,[Programmazione_Entita_Cod_From] " & vbCrLf)
            Stb.Append("  ,[Programmazione_Entita_Cod_To] " & vbCrLf)
            Stb.Append("  ,[Programmazione_Cod_From] " & vbCrLf)
            Stb.Append("  ,[Programmazione_Cod_To] " & vbCrLf)
            Stb.Append("  ,[TipoRelazione] " & vbCrLf)

            Stb.Append("              , Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append(" '" & Agro_SQL_SaveText(Piva_SuperUser_From) & "'" & vbCrLf)
            Stb.Append(",'" & Agro_SQL_SaveText(Piva_SuperUser_To) & "'" & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Programmazione_Entita_Cod_From) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Programmazione_Entita_Cod_To) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Programmazione_Cod_From) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Programmazione_Cod_To) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(TipoRelazione) & " " & vbCrLf)


            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")

            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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







    ''' <summary>
    ''' Rimuove tutte le relazione di un tipo legate alla riga di planning "origine" (Programmazione_Entita_Cod_From)
    ''' </summary>
    ''' <param name="Piva_SuperUser_From"></param>
    ''' <param name="Programmazione_Entita_Cod_From"></param>
    ''' <param name="TipoRelazione"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function CancellaPerEntitaFrom(
          ByVal Piva_SuperUser_From As String _
        , ByVal Programmazione_Entita_Cod_From As Integer _
        , ByVal TipoRelazione As Integer _
        , ByVal xFiltroAggiuntivo As String,
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


            Stb.Append(" DELETE FROM Programmazione_EntitaXProgrammazione_Entita ")
            Stb.Append(" WHERE ")
            Stb.Append("       [Piva_SuperUser_From] = '" & Agro_SQL_SaveText(Piva_SuperUser_From) & "' " & vbCrLf)
            Stb.Append("   AND [TipoRelazione] = " & Agro_SQL_SaveNum(TipoRelazione) & " " & vbCrLf)
            Stb.Append("   AND [Programmazione_Entita_Cod_From] =  " & Agro_SQL_SaveNum(Programmazione_Entita_Cod_From) & vbCrLf)
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
