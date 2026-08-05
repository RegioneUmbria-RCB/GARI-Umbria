
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ModelliPrevisionali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Mod_Cod As Integer,
                          ByVal PivaSuperuser As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.AppendLine("SELECT *, op_aut.DescrizioneAggiuntiva AS DescrizioneModello ")
            Stb.AppendLine("FROM ModelliPrevisionali ")
            Stb.AppendLine("LEFT JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate op_aut ")
            Stb.AppendLine("ON op_aut.Mod_Cod = ModelliPrevisionali.Mod_Cod ")
            Stb.AppendLine("WHERE 1 = 1 ")
            If Mod_Cod <> 0 Then
                Stb.AppendLine("AND ModelliPrevisionali.Mod_Cod = " & Mod_Cod)
            End If
            If Not String.IsNullOrEmpty(PivaSuperuser) Then
                Stb.AppendLine("AND (op_aut.Piva_Superuser IS NULL OR op_aut.Piva_Superuser = '" & Agro_SQL_SaveText(PivaSuperuser) & "')")
            End If
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            If Not String.IsNullOrEmpty(xOrderBy) Then
                Stb.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiPerTitolo(ByVal Mod_Cod As Integer, ByVal Tipo_Sorgente As Integer, ByVal Stazione_Cod As Integer, ByVal PivaSuperuser As String, ByVal Piva As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataRow

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.LeggiPerTitolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            sql.Clear()

            sql.AppendLine("DECLARE @STAZIONE AS VARCHAR(MAX) ")
            sql.AppendLine("SET @STAZIONE = ")

            Select Case Tipo_Sorgente

                Case enum_Meteo_Tiposorgente.Gias_RER

                    sql.AppendLine("(SELECT Stazione_Des FROM TB_Stazioni WHERE ID_Stazione = " & Stazione_Cod & ") ")

                Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                    sql.AppendLine("(SELECT Quadrante_Des FROM TB_Quadranti WHERE ID_Quadrante = " & Stazione_Cod & ") ")

                Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali

                    sql.AppendLine("(SELECT TOP 1 '[' + COALESCE(vis.DescrizioneAggiuntiva, forn.Descrizione, '') + '] ' + Nome ")
                    sql.AppendLine("FROM MeteoNT_Stazioni staz ")
                    sql.AppendLine("INNER JOIN MeteoNT_Fornitori forn ON forn.Id = staz.Id_Fornitore ")
                    sql.AppendLine("LEFT JOIN ( ")
                    sql.AppendLine("		SELECT 0 AS Ordine, Id_Stazione, DescrizioneAggiuntiva, Proprietario ")
                    sql.AppendLine("		FROM MeteoNT_VisibilitaStazioni ")
                    sql.AppendLine("		WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(PivaSuperuser) & "' AND PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    sql.AppendLine()
                    sql.AppendLine("		UNION ")
                    sql.AppendLine()
                    sql.AppendLine("		SELECT 1 AS Ordine, Id_Stazione, DescrizioneAggiuntiva, Proprietario ")
                    sql.AppendLine("		FROM MeteoNT_VisibilitaStazioni ")
                    sql.AppendLine("		WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(PivaSuperuser) & "' AND PIVA = '*' ")
                    sql.AppendLine(") vis ON vis.Id_Stazione = staz.Id ")
                    If Tipo_Sorgente = enum_Meteo_Tiposorgente.Aziendali Then
                        sql.AppendLine("AND vis.Proprietario = 1")
                    End If
                    sql.AppendLine("WHERE staz.Id = " & Stazione_Cod)
                    sql.AppendLine("ORDER BY vis.Ordine) ")

            End Select

            sql.AppendLine()
            sql.AppendLine()

            sql.AppendLine("SELECT ")
            sql.AppendLine("	ModelliPrevisionali.Mod_Des AS Modello ")
            sql.AppendLine("	, OpAut.DescrizioneAggiuntiva AS ModelloAux ")
            sql.AppendLine("	, Specie.Veg_Des AS Specie ")
            sql.AppendLine(" 	, COALESCE(@STAZIONE, '') AS Stazione ")
            sql.AppendLine("FROM ModelliPrevisionali ")
            sql.AppendLine("INNER JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate     AS OpAut    ON OpAut.Mod_Cod = ModelliPrevisionali.Mod_Cod ")
            sql.AppendLine("INNER JOIN ModelliXSpeciexAvversita										AS msa		ON msa.Mod_Cod = ModelliPrevisionali.Mod_Cod ")
            sql.AppendLine("INNER JOIN SpecieVegetali												AS Specie	ON Specie.Veg_Cod = msa.Veg_Cod ")
            sql.AppendLine("WHERE ModelliPrevisionali.Mod_Cod = " & Mod_Cod)
            sql.AppendLine("    AND (OpAut.Piva_Superuser IS NULL OR OpAut.Piva_Superuser= '" & Agro_SQL_SaveText(PivaSuperuser) & "') ")

            DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

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

    Public Function LeggiSpecieXModelli(ByVal elencoModelli As Integer(), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.LeggiSpecieXModelli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable = Nothing

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT DISTINCT msa.Veg_Cod ")
            Stb.AppendLine("FROM ModellixSpecieXAvversita msa ")
            Stb.AppendLine("INNER JOIN SpecieVegetali                                           veg     ON veg.Veg_Cod = msa.Veg_Cod ")
            Stb.AppendLine("INNER JOIN ModelliPrevisionali                                      modelli ON modelli.Mod_cod = msa.Mod_cod ")
            'Stb.AppendLine("LEFT JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate  opaut   ON opaut.Mod_Cod = modelli.Mod_Cod")
            Stb.AppendLine("WHERE msa.attivo = 1 ")
            'Stb.AppendLine("AND (opaut.Piva_Superuser IS NULL OR opaut.Piva_Superuser = '" & Piva_Superuser & "') ")
            If elencoModelli.Length > 0 Then
                Dim strLista As String = String.Join(", ", elencoModelli)
                Stb.AppendLine("AND modelli.Mod_Cod IN (" & Agro_SQL_Save_Clausola_IN(strLista) & ")")
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

    Public Function LeggiAvversitaXModelloSpecie(Mod_cod As Integer, Veg_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.LeggiAvversitaXModelloSpecie()"

        '----- Variabili
        Dim Stb As New Text.StringBuilder

        Dim Avv_Cod As Integer = 0

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT msa.Av_Cod ")
            Stb.AppendLine("FROM ModellixSpecieXAvversita msa ")
            Stb.AppendLine("WHERE msa.attivo = 1 ")
            Stb.AppendLine("AND Mod_Cod = " & Mod_cod.ToString())
            Stb.AppendLine("AND Veg_Cod = " & Veg_Cod.ToString())

            '--------------------------------------------------------------------------
            Dim DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then

                Avv_Cod = DT.Rows(0)("Av_Cod")

            End If

        Catch ex As Exception

            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Avv_Cod
    End Function

    Public Function LeggiAvversitaXModello(ByVal ModCod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.LeggiAvversitaXModello()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Av_Cod As Integer = 0

        Try

            Stb.Length = 0

            Stb.AppendLine("SELECT Av_Cod FROM ModellixSpecieXAvversita ")
            Stb.AppendLine("WHERE Attivo = 1 AND Mod_Cod = " & ModCod.ToString)

            '--------------------------------------------------------------------------
            Dim DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Av_Cod = DT.Rows(0)("Av_Cod")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Av_Cod = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Av_Cod
    End Function

    Public Function LeggiElenco(ByVal arr_codes() As Integer, ByVal PivaSuperuser As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionali_R.LeggiElenco()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim sql As New System.Text.StringBuilder
        Dim DT As DataTable = Nothing

        Try
            If arr_codes.Length > 0 Then

                sql.Length = 0

                sql.AppendLine("SELECT ")
                sql.AppendLine("	mod_cod = mp.Mod_Cod ")
                sql.AppendLine("	, mod_name = mp.Mod_Des ")
                sql.AppendLine(" 	, mod_name_agg = COALESCE(OpAut.DescrizioneAggiuntiva, '') ")
                sql.AppendLine("FROM ModelliPrevisionali mp ")
                sql.AppendLine("INNER JOIN ModelliPrevisionaliXpiva_superUser_OperazioniAutorizzate AS OpAut ON OpAut.Mod_Cod = mp.Mod_Cod ")
                sql.AppendLine("WHERE mp.Mod_Cod in (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arr_codes)) & ") ")
                sql.AppendLine("AND (OpAut.Piva_Superuser IS NULL OR OpAut.Piva_Superuser= '" & Agro_SQL_SaveText(PivaSuperuser) & "') ")

                DT = EseguiQuery_Lettura(objParametri, sql.ToString, NomeRoutine)

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

End Class
