Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreModelsSTD.Zoo
Imports System.Reflection
Imports System.Text
Imports System.Security.Cryptography
Imports AgronicaCoreDTOStd.InData

Public Class LineeProduttive_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Piano_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Caratter_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT [PIVA],[sa_cod] ,[Piano_Cod],[Piano_Des] " + vbCrLf)
                    StrSQL.Append(",[DimX] ,[DimY],[Colore_Interno] ,[Colore_Esterno]" + vbCrLf)
                    StrSQL.Append(",[Spessore],[Riempimento],[Zoom] " + vbCrLf)
                    StrSQL.Append(" FROM  Cantina_Caratteristiche " + vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) + vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Piano_Cod <> 0 Then
                        StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY  Piano_Des ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " &
                                    " FROM  Cantina_Caratteristiche " &
                                    " WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Piano_Cod <> 0 Then
                        StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva, Sa_Cod, Piano_Des ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    Public Function Leggi_LineeProduttive(ByVal Piva As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.Leggi_LineeProduttive()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT prod.Piva, ")
            StrSQL.AppendLine("  prod.Linea_Cod, ")
            StrSQL.AppendLine("  prod.Linea_Cod_Des, ")
            StrSQL.AppendLine("  prod.Linea_Des, ")
            StrSQL.AppendLine("  prod.Linea_Classe_Cod, ")
            StrSQL.AppendLine("  ISNULL(lpp.Preparazione_Cod, 0) Preparazione_Cod, ")
            StrSQL.AppendLine("  ISNULL(prep.Preparazione_Sigla, '') Preparazione_Sigla, ")
            StrSQL.AppendLine("  ISNULL(prep.Preparazione_Des, '') Preparazione_Des, ")
            StrSQL.AppendLine("  CASE WHEN prod.Veg_Cod > 0 THEN prod.Veg_Cod ELSE mix.Veg_Cod END Veg_Cod, ")
            StrSQL.AppendLine("  CASE WHEN prod.Cul_Cod > 0 THEN prod.Cul_Cod ELSE mix.Cul_Cod END Cul_Cod, ")
            StrSQL.AppendLine("  prod.Reg_Cod, ")
            StrSQL.AppendLine("  ISNULL(prep.Modulo_Generazione, 2) Modulo_Generazione, ")
            StrSQL.AppendLine("  ISNULL(statiProdotto.Tipo_Generazione, 4) Tipo_Generazione, ")
            StrSQL.AppendLine("  ISNULL(statiProdotto.Codice_Generazione, '') Codice_Generazione, ")
            StrSQL.AppendLine("  ISNULL(statiProdotto.Elem_Cod, 210) Elem_Cod,")
            StrSQL.AppendLine("  ISNULL(statiProdotto.Descrizione, '') StatoProdotto, ")
            StrSQL.AppendLine("  prod.Data_Creazione, ")
            StrSQL.AppendLine("  prod.Username_Creazione, ")
            StrSQL.AppendLine("  prod.Validita_Inizio, ")
            StrSQL.AppendLine("  prod.Validita_Fine ")
            StrSQL.AppendLine("FROM  Linee_Produzioni prod ")
            StrSQL.AppendLine("LEFT JOIN Linee_Produzioni_Mix mix ")
            StrSQL.AppendLine("ON prod.Piva = mix.Piva ")
            StrSQL.AppendLine("AND prod.Linea_Cod = mix.Linea_Cod ")
            StrSQL.AppendLine("LEFT JOIN Linee_ProduzionixPreparazioni lpp ")
            StrSQL.AppendLine("ON prod.Piva = lpp.Piva ")
            StrSQL.AppendLine("AND prod.Linea_Cod = lpp.Linea_Cod ")
            StrSQL.AppendLine("LEFT JOIN Linee_Preparazioni prep ")
            StrSQL.AppendLine("ON lpp.Piva = prep.Piva ")
            StrSQL.AppendLine("AND lpp.Preparazione_Cod = prep.Preparazione_Cod ")
            StrSQL.AppendLine("LEFT JOIN (")
            StrSQL.AppendLine("  SELECT gal.Piva, gal.Linea_Cod,")
            StrSQL.AppendLine("    gal.Tipo_Generazione,")
            StrSQL.AppendLine("    gal.Modulo_Generazione,")
            StrSQL.AppendLine("    gal.Elem_Cod,")
            StrSQL.AppendLine("    ga.Codice_Generazione,")
            StrSQL.AppendLine("    ga.Descrizione ")
            StrSQL.AppendLine("  FROM OGenerazioni_Anagrafe ga")
            StrSQL.AppendLine("  JOIN OGenerazioni_Anagrafe_Log gal")
            StrSQL.AppendLine("  ON gal.Codice_Generazione = ga.Codice_Generazione")
            StrSQL.AppendLine("  AND gal.Elem_Cod = ga.Elem_Cod ")
            StrSQL.AppendLine("  AND ga.Tipo_Generazione = 4")
            StrSQL.AppendLine("  WHERE gal.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.AppendLine("  GROUP BY gal.Piva, gal.Linea_Cod,")
            StrSQL.AppendLine("    gal.Tipo_Generazione,")
            StrSQL.AppendLine("    gal.Modulo_Generazione,")
            StrSQL.AppendLine("    gal.Elem_Cod,")
            StrSQL.AppendLine("    ga.Codice_Generazione,")
            StrSQL.AppendLine("    ga.Descrizione ")
            StrSQL.AppendLine(") statiProdotto")
            StrSQL.AppendLine("ON prod.Piva = statiProdotto.Piva")
            StrSQL.AppendLine("AND prod.Linea_Cod = statiProdotto.Linea_Cod")
            StrSQL.AppendLine("WHERE prod.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    Public Function Leggi_LineaProduzione(piva As String, lineaCod As Integer, objParametriServer As AgronicaCoreParametri) As Linee_Produzioni
        Dim result As Linee_Produzioni
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.Leggi_LineaProduzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Linee_Produzioni ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New Linee_Produzioni
                Dim row As DataRow = DT.AsEnumerable().FirstOrDefault()
                For Each prop As PropertyInfo In result.GetType().GetProperties()
                    If DT.Columns.Contains(prop.Name) Then
                        Dim value As Object = Nothing
                        If Not IsDBNull(row(prop.Name)) Then
                            value = row(prop.Name)
                        End If
                        prop.SetValue(result, value)
                    End If
                Next
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

    Public Function VerificaCancellazioneLineaProduzione(piva As String, lineaCod As Integer, lineaDes As String,
                                                         deleteAll As Boolean, codiciLineePreparazioni As Integer(),
                                                         objParametriServer As AgronicaCoreParametri) As String
        Dim result As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.VerificaCancellazioneLineaProduzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT tot.Piva, tot.Linea_Cod, ")
            StrSQL.AppendLine("  MAX(tot.Referenziata) Referenziata, ")
            StrSQL.AppendLine("  MAX(tot.Utilizzata) Utilizzata, ")
            StrSQL.AppendLine("  MAX(tot.Movimentata) Movimentata ")
            StrSQL.AppendLine("FROM ( ")
            StrSQL.AppendLine("	SELECT linee.*, ")
            StrSQL.AppendLine("		CASE WHEN a.Id_Agenda IS NULL THEN 0 ELSE 1 END Utilizzata, ")
            StrSQL.AppendLine("		CASE WHEN md.Id_Mov IS NULL THEN 0 ELSE 1 END Movimentata ")
            StrSQL.AppendLine("	FROM ( ")
            StrSQL.AppendLine("		SELECT lpp.Piva, lpp.Linea_Cod, lpp.Preparazione_Cod, ")
            StrSQL.AppendLine("			gal.Elem_Cod, mp.Mat_Cod, mp.Mat_Des, ")
            StrSQL.AppendLine("			  CASE WHEN mp.Mat_Cod_Referenza IS NULL THEN 0 ELSE 1 END Referenziata ")
            StrSQL.AppendLine("		FROM Linee_ProduzionixPreparazioni lpp WITH(NOLOCK) ")
            StrSQL.AppendLine("		JOIN OGenerazioni_Anagrafe_Log gal WITH(NOLOCK) ")
            StrSQL.AppendLine("		ON lpp.Piva = gal.Piva  ")
            StrSQL.AppendLine("		AND lpp.Linea_Cod = gal.Linea_Cod ")
            StrSQL.AppendLine("		JOIN ( ")
            StrSQL.AppendLine("		  SELECT orig.Piva, orig.Sa_Cod, orig.Elem_Cod, ")
            StrSQL.AppendLine("			orig.Mat_Cod, orig.Mat_Des, ref.Mat_Cod_Referenza ")
            StrSQL.AppendLine("		  FROM Materie_Prime orig WITH(NOLOCK) ")
            StrSQL.AppendLine("		  LEFT JOIN ( ")
            StrSQL.AppendLine("			SELECT * ")
            StrSQL.AppendLine("			FROM Materie_Prime WITH(NOLOCK) ")
            StrSQL.AppendLine("			WHERE piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("			AND Mat_Cod_Referenza > 0 ")
            StrSQL.AppendLine("		  ) ref ")
            StrSQL.AppendLine("		  ON orig.Piva = ref.Piva ")
            StrSQL.AppendLine("		  AND orig.Elem_Cod = ref.Elem_Cod  ")
            StrSQL.AppendLine("		  AND orig.Mat_Cod = ref.Mat_Cod_Referenza ")
            StrSQL.AppendLine("		) mp ")
            StrSQL.AppendLine("		ON gal.Piva = mp.Piva ")
            StrSQL.AppendLine("		AND gal.Sa_Cod = mp.Sa_Cod  ")
            StrSQL.AppendLine("		AND gal.Elem_Cod = mp.Elem_Cod  ")
            StrSQL.AppendLine("		AND gal.Mat_Cod = mp.Mat_Cod ")
            StrSQL.AppendLine("	) linee ")
            StrSQL.AppendLine("	LEFT JOIN Agenda a WITH(NOLOCK) ")
            StrSQL.AppendLine("	ON linee.Piva = a.Piva ")
            StrSQL.AppendLine("	AND linee.Linea_Cod = a.LINEA_COD ")
            StrSQL.AppendLine("	AND linee.Preparazione_Cod = a.PREPARAZIONE_COD  ")
            StrSQL.AppendLine("	LEFT JOIN Movimenti_dettagli md WITH(NOLOCK)  ")
            StrSQL.AppendLine("	ON linee.Piva = md.Piva ")
            StrSQL.AppendLine("	AND linee.Mat_Cod = md.Mat_Cod ")
            StrSQL.AppendLine("	AND linee.Elem_Cod = md.Elem_Cod ")
            StrSQL.AppendLine("	WHERE linee.Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("	AND linee.Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))
            StrSQL.AppendLine(" AND linee.Preparazione_Cod IN (" & String.Join(", ", codiciLineePreparazioni) & ") ")
            StrSQL.AppendLine(") tot ")
            StrSQL.AppendLine("WHERE tot.Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND tot.Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))
            StrSQL.AppendLine("AND tot.Preparazione_Cod IN (" & String.Join(", ", codiciLineePreparazioni) & ") ")
            StrSQL.AppendLine("GROUP BY tot.Piva, tot.Linea_Cod ")
            StrSQL.AppendLine("ORDER BY tot.Piva, tot.Linea_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso (DT.Rows.Count > 0) Then
                Dim row As DataRow = DT.AsEnumerable.FirstOrDefault()
                If IsNumeric(row("Referenziata")) AndAlso CInt(row("Referenziata")) > 0 Then
                    If Not String.IsNullOrEmpty(result) Then
                        result += "<br/>"
                    End If
                    result += $"La linea '{lineaDes}' ha dei prodotti collegati che non consentono la cancellazione"
                End If

                If IsNumeric(row("Utilizzata")) AndAlso CInt(row("Utilizzata")) > 0 Then
                    If Not String.IsNullOrEmpty(result) Then
                        result += "<br/>"
                    End If
                    result += $"La linea '{lineaDes}' non può essere cancellata perchè in uso"
                End If

                If IsNumeric(row("Movimentata")) AndAlso CInt(row("Movimentata")) > 0 Then
                    If Not String.IsNullOrEmpty(result) Then
                        result += "<br/>"
                    End If
                    result += $"La linea '{lineaDes}' non può essere cancellata perchè già movimentata"
                End If
            End If


            'result = Not IsNothing(DT) AndAlso (DT.Rows.Count > 0)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function

    Public Function VerificaDescrizioneLineaProduzione(piva As String, lineaCod As Integer, descrizione As String, objParametriServer As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.VerificaDescrizioneLineaProduzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Linee_Produzioni ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND NOT Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))
            StrSQL.AppendLine("AND Linea_Cod_Des = '" & Agro_SQL_SaveText(Trim(descrizione)) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            result = Not IsNothing(DT) AndAlso (DT.Rows.Count > 0)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

    Public Function Leggi_LineeProduttiveMix(piva As String, lineaCod As Integer, objParametriServer As AgronicaCoreParametri) As Linee_Produzioni_Mix
        Dim result As Linee_Produzioni_Mix
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.Leggi_LineeProduttiveMix()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Linee_Produzioni_Mix ")
            StrSQL.AppendLine("WHERE Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND Linea_Cod = " & Agro_SQL_SaveNum(lineaCod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                result = New Linee_Produzioni_Mix
                Dim row As DataRow = DT.AsEnumerable().FirstOrDefault()
                For Each prop As PropertyInfo In result.GetType().GetProperties()
                    If DT.Columns.Contains(prop.Name) Then
                        Dim value As Object = Nothing
                        If Not IsDBNull(row(prop.Name)) Then
                            value = row(prop.Name)
                        End If
                        prop.SetValue(result, value)
                    End If
                Next
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function

    Public Function VerificaConfigurazioneLineaProduzione(piva As String, regCod As Integer, vegCod As Integer,
                                                          culCod As Integer, objParametriServer As AgronicaCoreParametri) As Boolean
        Dim result As Boolean = False
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.LineeProduttive_R.VerificaDescrizioneLineaProduzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT 1")
            StrSQL.AppendLine("FROM (")
            StrSQL.AppendLine("  SELECT prod.Piva,")
            StrSQL.AppendLine("    prod.Linea_Cod,")
            StrSQL.AppendLine("    prod.Linea_Cod_Des,")
            StrSQL.AppendLine("    prod.Reg_Cod,")
            StrSQL.AppendLine("    CASE WHEN prod.Veg_Cod > 0 THEN prod.Veg_Cod ELSE mix.Veg_Cod END Veg_Cod,")
            StrSQL.AppendLine("    CASE WHEN prod.Cul_Cod > 0 THEN prod.Cul_Cod ELSE mix.Cul_Cod END Cul_Cod")
            StrSQL.AppendLine("  FROM Linee_Produzioni prod")
            StrSQL.AppendLine("  LEFT JOIN Linee_Produzioni_Mix mix")
            StrSQL.AppendLine("  ON prod.Piva = mix.Piva")
            StrSQL.AppendLine("  AND prod.Linea_Cod = mix.Linea_Cod")
            StrSQL.AppendLine("  WHERE prod.Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine(") tot")
            StrSQL.AppendLine("WHERE tot.Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "'")
            StrSQL.AppendLine("AND tot.Reg_Cod = " & Agro_SQL_SaveNum(regCod))
            StrSQL.AppendLine("AND tot.Veg_Cod = " & Agro_SQL_SaveNum(vegCod))
            StrSQL.AppendLine("AND tot.Cul_Cod = " & Agro_SQL_SaveNum(culCod))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            result = Not IsNothing(DT) AndAlso (DT.Rows.Count > 0)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result
    End Function
End Class

Public Class LineeProduttive_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviLineeProduzioni(ByVal piva As String,
                           ByVal EFArrayToInsert As List(Of LineaProduttiva),
                           ByVal EFArrayToUpdate As List(Of LineaProduttiva),
                           ByVal EFArrayToDelete As List(Of LineaProduttiva),
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.LineeProduttive_W.ScriviLinee_Produzioni()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)
                Dim transaction As DbContextTransaction = Nothing
                ' Contiene anche i dettagli
                Try
                    transaction = GiasContext.Database.BeginTransaction()
                    ' Contiene anche i dettagli
                    For Each listProdotti As LineaProduttiva In EFArrayToInsert

                        If Not listProdotti.LineaPresente Then
                            Dim lineaCod As Integer = 0
                            Do
                                lineaCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                    "lineaproduzione",
                                                                    0,
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                    objParametri)
                            Loop While (lineaCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                                (GiasContext.Linee_Produzioni.Any(Function(x) x.Linea_Cod = lineaCod))

                            listProdotti.Linee_Produzioni.Linea_Cod = lineaCod
                            GiasContext.Linee_Produzioni.Add(listProdotti.Linee_Produzioni)

                            listProdotti.Linee_Produzioni_Mix.Linea_Cod = lineaCod
                            GiasContext.Linee_Produzioni_Mix.Add(listProdotti.Linee_Produzioni_Mix)

                        End If

                        If Not IsNothing(listProdotti.RelazioneLineePreparazioni) Then
                            For Each item As RelazioneLineePreparazioni In listProdotti.RelazioneLineePreparazioni
                                Dim preparazioneCod As Integer = 0
                                Do
                                    preparazioneCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "preparazione",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                                Loop While (preparazioneCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Linee_Preparazioni.Any(Function(x) x.Preparazione_Cod = preparazioneCod))

                                item.Linee_Preparazioni.Preparazione_Cod = preparazioneCod
                                GiasContext.Linee_Preparazioni.Add(item.Linee_Preparazioni)

                                item.Linee_ProduzionixPreparazioni.Linea_Cod = listProdotti.Linee_Produzioni.Linea_Cod
                                item.Linee_ProduzionixPreparazioni.Preparazione_Cod = preparazioneCod
                                GiasContext.Linee_ProduzionixPreparazioni.Add(item.Linee_ProduzionixPreparazioni)
                            Next
                        End If

                        For Each item In listProdotti.RelazioneMateriePrime
                            item.OGenerazioni_Anagrafe_Log.Linea_Cod = listProdotti.Linee_Produzioni.Linea_Cod

                            Dim matCod As Integer = 0
                            Do
                                matCod = sequenza_tabelle.NuovoId_Tabella_EF(GiasContext,
                                                                "Materie_Prime",
                                                                0,
                                                                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                                                                objParametri)
                            Loop While (matCod < AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode) AndAlso
                            (GiasContext.Materie_Prime.Any(Function(x) x.Mat_Cod = matCod))

                            item.Materie_Prime.Mat_Cod = matCod
                            item.Materie_Prime.Linea_Cod = listProdotti.Linee_Produzioni.Linea_Cod
                            GiasContext.Materie_Prime.Add(item.Materie_Prime)

                            item.OGenerazioni_Anagrafe_Log.Linea_Cod = listProdotti.Linee_Produzioni.Linea_Cod
                            item.OGenerazioni_Anagrafe_Log.Mat_Cod = matCod
                            GiasContext.OGenerazioni_Anagrafe_Log.Add(item.OGenerazioni_Anagrafe_Log)
                        Next

                    Next

                    For Each listProdotti As LineaProduttiva In EFArrayToUpdate
                        If Not IsNothing(listProdotti.RelazioneMateriePrime) Then
                            For Each item In listProdotti.RelazioneMateriePrime
                                If Not IsNothing(item.Materie_Prime) Then
                                    GiasContext.Materie_Prime.Attach(item.Materie_Prime)
                                    GiasContext.Entry(item.Materie_Prime).State = EntityState.Modified
                                End If
                            Next
                        End If

                        GiasContext.Linee_Produzioni.Attach(listProdotti.Linee_Produzioni)
                        GiasContext.Entry(listProdotti.Linee_Produzioni).State = EntityState.Modified
                    Next

                    For Each listProdotti As LineaProduttiva In EFArrayToDelete
                        If Not IsNothing(listProdotti.RelazioneMateriePrime) Then
                            For Each item In listProdotti.RelazioneMateriePrime
                                If Not IsNothing(item.OGenerazioni_Anagrafe_Log) Then
                                    GiasContext.OGenerazioni_Anagrafe_Log.Attach(item.OGenerazioni_Anagrafe_Log)
                                    GiasContext.OGenerazioni_Anagrafe_Log.Remove(item.OGenerazioni_Anagrafe_Log)
                                End If

                                If Not IsNothing(item.Materie_Prime) Then
                                    GiasContext.Materie_Prime.Attach(item.Materie_Prime)
                                    GiasContext.Materie_Prime.Remove(item.Materie_Prime)
                                End If
                            Next
                        End If

                        If Not IsNothing(listProdotti.RelazioneLineePreparazioni) Then
                            For Each item In listProdotti.RelazioneLineePreparazioni
                                If Not IsNothing(item.Linee_ProduzionixPreparazioni) Then
                                    GiasContext.Linee_ProduzionixPreparazioni.Attach(item.Linee_ProduzionixPreparazioni)
                                    GiasContext.Linee_ProduzionixPreparazioni.Remove(item.Linee_ProduzionixPreparazioni)
                                End If
                                If Not IsNothing(item.Linee_Preparazioni) Then
                                    GiasContext.Linee_Preparazioni.Attach(item.Linee_Preparazioni)
                                    GiasContext.Linee_Preparazioni.Remove(item.Linee_Preparazioni)
                                End If
                            Next
                        End If

                        If Not IsNothing(listProdotti.Linee_Produzioni_Mix) Then
                            GiasContext.Linee_Produzioni_Mix.Attach(listProdotti.Linee_Produzioni_Mix)
                            GiasContext.Linee_Produzioni_Mix.Remove(listProdotti.Linee_Produzioni_Mix)
                        End If

                        If Not IsNothing(listProdotti.Linee_Produzioni) Then
                            GiasContext.Linee_Produzioni.Attach(listProdotti.Linee_Produzioni)
                            GiasContext.Linee_Produzioni.Remove(listProdotti.Linee_Produzioni)
                        End If
                    Next

                    GiasContext.SaveChanges()
                    'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
                    'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
                    transaction.Commit()
                Catch ex As Exception
                    If IsNothing(transaction) Then
                        transaction.Rollback()
                    End If
                End Try
            End Using

            '---------------------------------------------

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


End Class
