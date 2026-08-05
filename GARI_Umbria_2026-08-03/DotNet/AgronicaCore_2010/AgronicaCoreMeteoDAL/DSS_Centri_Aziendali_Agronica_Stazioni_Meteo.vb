
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMeteoDAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM DSS_Centri_Aziendali_Agronica_Stazioni_Meteo " & vbCrLf)
            Stb.Append(" WHERE 1=1" & vbCrLf)

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


    Public Function Leggi_ElencoModelli(ByVal piva As String, ByVal DataCalcolo As DateTime, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMeteoDAL.Leggi_ElencoModelli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xml As String = ""

        Try

            Stb.Length = 0

            Stb.AppendLine("DECLARE @DATACALCOLO DATETIME = " & Agro_SQL_SaveDate(DataCalcolo))
            Stb.AppendLine("DECLARE @GG AS INTEGER = DATEPART(DY, @DATACALCOLO) --Giorno giuliano")
            Stb.AppendLine("")
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    Modelli.tipo_sorgente ")
            Stb.AppendLine("    , Modelli.Stazione_Cod ")
            Stb.AppendLine("    , Modelli.Mod_Cod ")
            Stb.AppendLine("    , Modelli.Algoritmo_Cod ")
            Stb.AppendLine("    , Modelli.ParametriElaborazione ")
            Stb.AppendLine("	, CASE WHEN Modelli.GG_InizioValidita <= @GG AND @GG <= Modelli.GG_FineValidita THEN 1 ELSE 0 END AS GG_Validita ")
            Stb.AppendLine("    , Centri.sa_nome as CentroAziendale ")
            Stb.AppendLine("FROM DSS_Centri_Aziendali_Agronica_Stazioni_Meteo AS Stazioni ")
            Stb.AppendLine("INNER JOIN DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali AS Modelli ON Modelli.tipo_sorgente = Stazioni.tipo_sorgente AND Modelli.Stazione_Cod = Stazioni.Stazione_Cod ")
            Stb.AppendLine("INNER JOIN Centri_Aziendali AS Centri ON Centri.PIVA = Stazioni.piva AND Centri.sa_cod = Stazioni.sa_cod ")
            Stb.AppendLine("WHERE Stazioni.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            Stb.AppendLine("AND Stazioni.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine("ORDER BY CentroAziendale ")
            Stb.AppendLine("for xml path('Modello'), root('ElencoModelli'), elements ")

            '--------------------------------------------------------------------------
            xml = EseguiQuery_Lettura_XML(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xml = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xml

    End Function

    Public Function Leggi_ElencoStazioni(ByVal piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMeteoDAL.Leggi_ElencoStazioni()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xml As String = ""

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("    Stazioni.tipo_sorgente ")
            Stb.AppendLine("    , Stazioni.Stazione_Cod ")
            Stb.AppendLine("    , Centri.sa_nome as CentroAziendale ")
            Stb.AppendLine("FROM DSS_Centri_Aziendali_Agronica_Stazioni_Meteo AS Stazioni ")
            Stb.AppendLine("INNER JOIN Centri_Aziendali AS Centri ON Centri.PIVA = Stazioni.piva AND Centri.sa_cod = Stazioni.sa_cod ")
            Stb.AppendLine("WHERE Stazioni.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            Stb.AppendLine("AND Stazioni.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine("ORDER BY CentroAziendale ")
            Stb.AppendLine("for xml path('Stazione'), root('ElencoStazioni'), elements ")

            '--------------------------------------------------------------------------
            xml = EseguiQuery_Lettura_XML(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xml = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xml

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
              ByVal piva As String _
            , ByVal sa_cod As Integer _
            , ByVal tipo_sorgente As Integer _
            , ByVal Stazione_Cod As Integer _
            , ByVal Stazione_Des As String _
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
            strSQL.Append(" INSERT DSS_Centri_Aziendali_Agronica_Stazioni_Meteo ( " + vbCrLf)

            strSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            strSQL.Append("  ,[piva] " & vbCrLf)
            strSQL.Append("  ,[sa_cod] " & vbCrLf)
            strSQL.Append("  ,[tipo_sorgente] " & vbCrLf)
            strSQL.Append("  ,[Stazione_Cod] " & vbCrLf)
            strSQL.Append("  ,[Stazione_Des] " & vbCrLf)


            strSQL.Append("              ")
            strSQL.Append("              , Inviato,            datainvio, ")
            strSQL.Append("              Data_Creazione,     Data_Modifica, ")
            strSQL.Append("              UserName_Creazione, UserName_Modifica ")
            strSQL.Append("              ) ")

            strSQL.Append(" VALUES ( ")

            strSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(piva) & "'" & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(sa_cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(tipo_sorgente) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Stazione_Cod) & " " & vbCrLf)
            strSQL.Append(", '" & Agro_SQL_SaveText(Stazione_Des) & "' " & vbCrLf)


            strSQL.Append("         , 0  " + vbCrLf)
            strSQL.Append("         , Null  " + vbCrLf)

            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            strSQL.Append(") ")

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
    Public Function Cancella(
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal origine_cod As Integer,
        ByVal sorgentedati_cod As Integer,
        ByVal xFiltroAggiuntivo As String,
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
                Stb.Append(" UPDATE DSS_Centri_Aziendali_Agronica_Stazioni_Meteo ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM DSS_Centri_Aziendali_Agronica_Stazioni_Meteo ")
                Stb.Append(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                Stb.Append(" and sa_cod = " & sa_cod & " ")
                Stb.Append(" and tipo_sorgente = " & origine_cod & "  ")
                Stb.Append(" and stazione_cod = " & sorgentedati_cod & "  ")
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


    Public Function PopolaDaImpianti(ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "PopolaDaImpianti()"

        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim nInsert As Integer = 0

        Try

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO DSS_Centri_Aziendali_Agronica_Stazioni_Meteo AS t ")
            sql.AppendLine("	USING ")
            sql.AppendLine("		(SELECT DISTINCT Piva_SuperUser, piva, sa_cod, tipo_sorgente, Stazione_Cod ")
            sql.AppendLine("		FROM DSS_Impianti_Aziendali_Agronica_Stazioni_Meteo) AS s ")
            sql.AppendLine("	ON t.Piva_SuperUser = s.Piva_SuperUser ")
            sql.AppendLine("		AND t.piva = s.piva ")
            sql.AppendLine("		AND t.sa_cod = s.sa_cod ")
            sql.AppendLine("		--AND t.tipo_sorgente = s.tipo_sorgente ")
            sql.AppendLine("		--AND t.stazione_cod = s.stazione_cod ")
            sql.AppendLine("	--WHEN MATCHED THEN ")
            sql.AppendLine("	--	UPDATE SET piva = s.piva ")
            sql.AppendLine("	WHEN NOT MATCHED THEN ")
            sql.AppendLine("		INSERT (Piva_SuperUser, piva, sa_cod, tipo_sorgente, Stazione_Cod) ")
            sql.AppendLine("		VALUES (s.Piva_SuperUser, s.piva, s.sa_cod, s.tipo_sorgente, s.Stazione_Cod) ")
            sql.AppendLine("	OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione")

            Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            For Each r In DT.Rows
                If r("Operazione").ToString.ToUpper = "INSERT" Then
                    nInsert = r("ContaOperazioni")
                End If
            Next

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return nInsert

    End Function

End Class
