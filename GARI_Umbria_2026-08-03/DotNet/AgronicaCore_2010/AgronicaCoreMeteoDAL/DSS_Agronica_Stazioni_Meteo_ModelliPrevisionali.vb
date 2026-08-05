
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiPerCentroAziendale(
        ByVal piva As String,
        ByVal sa_cod As Integer,
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
            Stb.AppendLine("   dss.piva + '-' + cast(dss.sa_cod as varchar(100)) + '-' + cast(tipo_sorgente as varchar(10)) + '-' + cast(Stazione_Cod as varchar(10)) as chiave ")
            Stb.AppendLine(" , sor.TipoSorgente_Des ")
            Stb.AppendLine(" , Stazione_Des as  Origine ")
            Stb.AppendLine(" , sa.sa_nome as Centro ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" , REPLACE( ISNULL((  ")
            Stb.AppendLine("  Select ModelliParametri = stuff(CAST(( ")
            Stb.AppendLine("         select ' §§ ' + mo.Descrizione  as [text()]  ")
            Stb.AppendLine("         From DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali mo ")
            Stb.AppendLine("      Where mo.tipo_sorgente = dss.tipo_sorgente ")
            Stb.AppendLine("      And mo.Stazione_Cod = dss.Stazione_Cod ")
            Stb.AppendLine("      For Xml PATH('') ")
            Stb.AppendLine("  ) ")
            Stb.AppendLine("  as varchar(max)) ")
            Stb.AppendLine("  , 1, 4, '') ")
            Stb.AppendLine(" ), '') ")
            Stb.AppendLine(" , '§§', '<br>') as modelli")


            Stb.AppendLine(" FROM DSS_Centri_Aziendali_Agronica_Stazioni_Meteo dss  ")
            Stb.AppendLine(" inner join centri_Aziendali sa  ")
            Stb.AppendLine(" on sa.piva = dss.piva  ")
            Stb.AppendLine(" and sa.sa_cod = dss.sa_cod ")
            Stb.AppendLine(" inner join (")
            Stb.AppendLine(" Select 0 As TipoSorgente_Cod, 'Gias (Stazioni Regione E.R.)' as TipoSorgente_Des ")
            Stb.AppendLine(" union ")
            Stb.AppendLine(" Select 3 As TipoSorgente_Cod, 'Gias (Quadranti Regione E.R.)' as TipoSorgente_Des ")
            Stb.AppendLine(" union ")
            Stb.AppendLine(" Select 1 As TipoSorgente_Cod, 'Tutte le stazioni in visibilità' as TipoSorgente_Des ")
            Stb.AppendLine(" union ")
            Stb.AppendLine(" Select 2 As TipoSorgente_Cod, 'Solo le stazioni aziendali' as TipoSorgente_Des ")
            Stb.AppendLine(")  sor  ")
            Stb.AppendLine("   on sor.TipoSorgente_Cod = dss.Tipo_Sorgente")

            Stb.AppendLine(" WHERE dss.piva = '" & Agro_SQL_SaveText(piva) & "'")
            If sa_cod <> 0 Then
                Stb.AppendLine(" AND dss.sa_cod = " & sa_cod)
            End If



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   dss.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   dss.Inviato =-1 ")
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
    Public Function LeggiPerStazione(
            ByVal SorgenteDati_Cod As Integer,
            ByVal Stazione_Cod As Integer,
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

            Stb.AppendLine(" SELECT  dss.* " & vbCrLf)
            Stb.AppendLine(" FROM DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali dss  ")
            Stb.AppendLine(" WHERE tipo_sorgente = " & SorgenteDati_Cod & " ")
            Stb.AppendLine(" AND Stazione_Cod = " & Stazione_Cod & " ")


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

    Public Function CentriXStazioniXModelli(ByVal PIVA As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreMeteoDAL.CentriXStazioniXModelli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            sql.Length = 0

            sql.AppendLine("DECLARE @PIVA AS VARCHAR(50) = '" & Agro_SQL_SaveText(PIVA) & "' ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	sa_cod = sa.sa_cod ")
            sql.AppendLine("	, sa_nome = sa.sa_nome ")
            sql.AppendLine("	, tipo_sorgente = meteo.tipo_sorgente ")
            sql.AppendLine("	, stazione_cod = meteo.Stazione_Cod ")
            sql.AppendLine("	, mod_cod = modelli.Mod_Cod ")
            sql.AppendLine("FROM Centri_Aziendali sa ")
            sql.AppendLine("LEFT JOIN DSS_Centri_Aziendali_Agronica_Stazioni_Meteo meteo ON meteo.Piva = sa.PIVA AND meteo.sa_cod = sa.sa_cod ")
            sql.AppendLine("LEFT JOIN DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali modelli ON modelli.tipo_sorgente = meteo.tipo_sorgente AND modelli.Stazione_Cod = meteo.Stazione_Cod ")
            sql.AppendLine("WHERE sa.PIVA = @PIVA ")
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        sql.Append(" AND   dss.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        sql.Append(" AND   dss.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            sql.AppendLine("ORDER BY sa.sa_cod, meteo.tipo_sorgente, meteo.stazione_cod, modelli.mod_cod ")
            sql.AppendLine("")

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

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

Public Class DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
          ByVal tipo_sorgente As Integer _
        , ByVal Stazione_Cod As Integer _
        , ByVal Mod_Cod As Integer _
        , ByVal Algoritmo_Cod As Integer _
        , ByVal ParametriElaborazione As String _
        , ByVal GG_InizioValidita As Integer _
        , ByVal GG_FineValidita As Integer _
        , ByVal descrizione As String _
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
            strSQL.Append(" INSERT DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali " & vbCrLf)


            strSQL.Append("              (")

            strSQL.Append("   [tipo_sorgente] " & vbCrLf)
            strSQL.Append("  ,[Stazione_Cod] " & vbCrLf)
            strSQL.Append("  ,[Mod_Cod] " & vbCrLf)
            strSQL.Append("  ,[Algoritmo_Cod] " & vbCrLf)
            strSQL.Append("  ,[ParametriElaborazione] " & vbCrLf)
            strSQL.Append("  ,[GG_InizioValidita] " & vbCrLf)
            strSQL.Append("  ,[GG_FineValidita] " & vbCrLf)
            strSQL.Append("  ,[Descrizione] " & vbCrLf)


            strSQL.Append("              ,Inviato,            datainvio, ")
            strSQL.Append("              Data_Creazione,     Data_Modifica, ")
            strSQL.Append("              UserName_Creazione, UserName_Modifica ")
            strSQL.Append("              ) ")

            strSQL.Append(" VALUES ( ")

            strSQL.Append("  " & Agro_SQL_SaveNum(tipo_sorgente) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Stazione_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Mod_Cod) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(Algoritmo_Cod) & " " & vbCrLf)
            strSQL.Append(",'" & Agro_SQL_SaveText(ParametriElaborazione) & "'" & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(GG_InizioValidita) & " " & vbCrLf)
            strSQL.Append(", " & Agro_SQL_SaveNum(GG_FineValidita) & " " & vbCrLf)
            strSQL.Append(", '" & Agro_SQL_SaveText(descrizione) & "' " & vbCrLf)


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
    Public Function CancellaStazione(
        ByVal Tipo_Sorgente As Integer,
        ByVal Stazione_Cod As Integer,
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
                Stb.Append(" UPDATE DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali ")
                Stb.Append(" WHERE  Tipo_Sorgente = " & Tipo_Sorgente)
                Stb.Append(" and Stazione_Cod = " & Stazione_Cod)
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



    Public Class ObjModello
        Public Tipo_Sorgente As Integer
        Public Stazione_Cod As Integer
        Public Mod_Cod As Integer
        Public Algoritmo_Cod As Integer
        Public ParametriElaborazione As String
        Public GG_InizioValidita As Integer
        Public GG_FineValidita As Integer
        Public Function AsStringSql() As String
            Dim result = "("
            result &= Tipo_Sorgente.ToString & ", "
            result &= Stazione_Cod.ToString & ", "
            result &= Mod_Cod.ToString & ", "
            result &= Algoritmo_Cod.ToString & ", "
            result &= "'" & ParametriElaborazione & "', "
            result &= GG_InizioValidita.ToString & ", "
            result &= GG_FineValidita.ToString
            result &= ")"
            Return result
        End Function
    End Class

    Public Function PopolaDaImpianti(ByVal modelli As List(Of ObjModello), ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "PopolaDaImpianti()"

        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim nInsert As Integer = 0

        Try

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TMP_TBL TABLE (tipo_sorgente INT, Stazione_Cod INT, Mod_Cod INT, Algoritmo_Cod INT, ParametriElaborazione VARCHAR(300), GG_InizioValidita INT, GG_FineValidita INT) ")
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TMP_TBL VALUES ")
            sql.AppendLine(String.Join(", " & vbCrLf, modelli.Select(Function(c) c.AsStringSql())))
            sql.AppendLine()
            sql.AppendLine("DECLARE @ElencoOperazioni TABLE(Operazione VARCHAR(20)); ")
            sql.AppendLine()
            sql.AppendLine("MERGE INTO DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali AS t ")
            sql.AppendLine("	USING (SELECT * FROM @TMP_TBL) AS s ")
            sql.AppendLine("	ON t.tipo_sorgente = s.tipo_sorgente ")
            sql.AppendLine("		AND t.Stazione_Cod = s.Stazione_Cod ")
            sql.AppendLine("		AND t.Mod_Cod = s.Mod_Cod ")
            sql.AppendLine("		AND t.Algoritmo_Cod = s.Algoritmo_Cod ")
            sql.AppendLine("		AND t.ParametriElaborazione = s.ParametriElaborazione ")
            sql.AppendLine("	--WHEN MATCHED THEN ")
            sql.AppendLine("	--	UPDATE SET piva = s.piva ")
            sql.AppendLine("WHEN NOT MATCHED THEN ")
            sql.AppendLine("	INSERT (tipo_sorgente, Stazione_Cod, Mod_Cod, Algoritmo_Cod, ParametriElaborazione, GG_InizioValidita, GG_FineValidita) ")
            sql.AppendLine("	VALUES (s.tipo_sorgente, s.Stazione_Cod, s.Mod_Cod, s.Algoritmo_Cod, s.ParametriElaborazione, s.GG_InizioValidita, s.GG_FineValidita) ")
            sql.AppendLine("OUTPUT $action INTO @ElencoOperazioni; ")
            sql.AppendLine()
            sql.AppendLine("SELECT Operazione, COUNT(*) AS ContaOperazioni FROM @ElencoOperazioni GROUP BY Operazione ")

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

