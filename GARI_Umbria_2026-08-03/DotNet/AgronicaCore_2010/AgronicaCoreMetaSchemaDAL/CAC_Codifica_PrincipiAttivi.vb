Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CAC_Codifica_PrincipiAttivi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        byval Cod_Principio_Cliente As String, 
        byval pa_cod As integer, 
        byval Tipo_Codifica_PrincipioAttivo As enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica,
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
            Stb.Append(" FROM CAC_Codifica_PrincipiAttivi " + vbCrLf)
            Stb.Append(" WHERE " + vbCrLf)
            Stb.Append(" Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica_PrincipioAttivo ) & " " + vbCrLf)

            If pa_cod<>0 Then
                Stb.Append(" AND pa_cod = " & Agro_SQL_SaveNum(pa_cod ) & " " + vbCrLf)
            End If
            
            If Cod_Principio_Cliente <> "" Then
                Stb.Append(" AND Cod_Principio_Cliente = '" & Agro_SQL_SaveText(Cod_Principio_Cliente ) & "' " + vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            

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

Public Class CAC_Codifica_PrincipiAttivi_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(          
          byval Piva_SuperUser as string _ 
        , byval Cod_Principio_Cliente as string _ 
        , byval Desc_Principio_Cliente as string _ 
        , byval Categoria_Principio_Cliente as string _ 
        , byval Pa_Cod as integer _ 
        , byval Pa_Des as string _ 
        , byval Note as string _ 
        , byval Tipo_Codifica as integer _ 
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        , Optional ByVal Data_creazione As Date = #2/1/1900# _
        , Optional ByVal Data_modifica As Date = #2/1/1900# _
        , Optional ByVal username_creazione As String = "" _
        , Optional ByVal username_modifica As String = "" _
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
            Stb.Append(" INSERT INTO CAC_Codifica_PrincipiAttivi " + vbCrLf)

            Stb.Append("              (")

            Stb.append("   [Piva_SuperUser] "& vbcrlf )
            Stb.append("  ,[Cod_Principio_Cliente] "& vbcrlf )
            Stb.append("  ,[Desc_Principio_Cliente] "& vbcrlf )
            Stb.append("  ,[Categoria_Principio_Cliente] "& vbcrlf )
            Stb.append("  ,[Pa_Cod] "& vbcrlf )
            Stb.append("  ,[Pa_Des] "& vbcrlf )
            Stb.append("  ,[Note] "& vbcrlf )
            Stb.append("  ,[Tipo_Codifica] "& vbcrlf )


            Stb.Append("              , Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica ")            
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            stb.append(" '" & agro_sql_savetext(Piva_SuperUser) & "'" & vbcrlf ) 
            stb.append(",'" & agro_sql_savetext(Cod_Principio_Cliente) & "'" & vbcrlf ) 
            stb.append(",'" & agro_sql_savetext(Desc_Principio_Cliente) & "'" & vbcrlf ) 
            stb.append(",'" & agro_sql_savetext(Categoria_Principio_Cliente) & "'" & vbcrlf ) 
            stb.append(", " & agro_sql_savenum(Pa_Cod) & " " & vbcrlf ) 
            stb.append(",'" & agro_sql_savetext(Pa_Des) & "'" & vbcrlf ) 
            stb.append(",'" & agro_sql_savetext(Note) & "'" & vbcrlf ) 
            stb.append(", " & agro_sql_savenum(Tipo_Codifica) & " " & vbcrlf ) 


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







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
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
