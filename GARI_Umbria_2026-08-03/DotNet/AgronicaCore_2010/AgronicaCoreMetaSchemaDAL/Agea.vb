

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

Public Interface ISpecieAgea
    Property Cul_Cod_Agea As String
    Property Uso_Cod_Agea As String
    Property Occupazione_Cod_Agea As String
    Property Destinazione_Cod_Agea As String
    Property Qualita_Cod_Agea As String
End Interface

Public Class SpecieAgea
    Implements ISpecieAgea
    Public Property Cul_Cod_Agea As String Implements ISpecieAgea.Cul_Cod_Agea
    Public Property Uso_Cod_Agea As String Implements ISpecieAgea.Uso_Cod_Agea
    Public Property Occupazione_Cod_Agea As String Implements ISpecieAgea.Occupazione_Cod_Agea
    Public Property Destinazione_Cod_Agea As String Implements ISpecieAgea.Destinazione_Cod_Agea
    Public Property Qualita_Cod_Agea As String Implements ISpecieAgea.Qualita_Cod_Agea

    Public Sub New()
        Cul_Cod_Agea = String.Empty
        Uso_Cod_Agea = String.Empty
        Occupazione_Cod_Agea = String.Empty
        Destinazione_Cod_Agea = String.Empty
        Qualita_Cod_Agea = String.Empty
    End Sub

    ''' <returns>A string that concatenates the codes ordered from the least important.</returns>
    Public Function GetConcatenatedKey(Optional separator As String = "|") As String
        Return Cul_Cod_Agea & separator & Qualita_Cod_Agea &
                separator & Uso_Cod_Agea & separator & Destinazione_Cod_Agea & separator & Occupazione_Cod_Agea
    End Function
End Class

Public Class Agea_Codifiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Reads the GIAS Veg_Cod associated to the specified Agea species.
    ''' </summary>
    ''' <param name="listaSpecieAgea">Enumerable of objects containing the Agea species identifier (quintuple)</param>
    ''' <param name="objParametri">objParametri_Server</param>
    Public Function LeggiVegCodDaCodificaAgea(
        listaSpecieAgea As IEnumerable(Of ISpecieAgea),
        ByRef objParametri As AgronicaCoreParametri
    ) As IEnumerable(Of String)
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R.LeggiVegCodDaCodificaAgea()"
        Try
            Return MapSpecieAgeaToVegCod(listaSpecieAgea, objParametri).
            Select.Select(Function(row) CStr(row("Veg_Cod"))).
            ToHashSet
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Reads the GIAS Veg_Cod associated to the specified Agea species.
    ''' </summary>
    ''' <param name="listaSpecieAgea">Enumerable of objects containing the Agea species identifier (quintuple)</param>
    ''' <param name="objParametri">objParametri_Server</param>
    Public Function MapSpecieAgeaToVegCod(
        listaSpecieAgea As IEnumerable(Of ISpecieAgea),
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R.MapSpecieAgeaToVegCod()"
        Try
            Dim stb As New StringBuilder With {.Length = 0}
            Dim createSpeciesFilter = Function(x As SpecieAgea) As String
                                          Return "( Cul_Cod_Agea = '" & Agro_SQL_SaveText(x.Cul_Cod_Agea) &
                                           "' AND Uso_Cod = '" & Agro_SQL_SaveText(x.Uso_Cod_Agea) &
                                           "' AND Occupazione_Cod = '" & Agro_SQL_SaveText(x.Occupazione_Cod_Agea) &
                                           "' AND Destinazione_Cod = '" & Agro_SQL_SaveText(x.Destinazione_Cod_Agea) &
                                           "' AND Qualita_Cod = '" & Agro_SQL_SaveText(x.Qualita_Cod_Agea) & "' )"
                                      End Function

            stb.AppendLine(" SELECT DISTINCT Veg_Cod, Cul_Cod_Agea, ")
            stb.AppendLine("   Uso_Cod as Uso_Cod_Agea, ")
            stb.AppendLine("   Occupazione_Cod as Occupazione_Cod_Agea, ")
            stb.AppendLine("   Destinazione_Cod as Destinazione_Cod_Agea, ")
            stb.AppendLine("   Qualita_Cod as Qualita_Cod_Agea ")
            stb.AppendLine(" FROM Codifica_SpecieVegetali_Agea_2015_2020 ")
            stb.AppendLine(" WHERE 1=2 ")

            If listaSpecieAgea IsNot Nothing AndAlso listaSpecieAgea.Any Then
                listaSpecieAgea.Select(Function(species) createSpeciesFilter(species)).ToList.
                    ForEach(Sub(filter) stb.AppendLine(" OR " & filter))
            End If

            Return EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function MapByOccupazione(
        listaSpecieAgea As IEnumerable(Of ISpecieAgea),
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R.MapSpecieAgeaToVegCod()"
        Try
            Dim stb As New StringBuilder With {.Length = 0}
            stb.AppendLine(" SELECT DISTINCT Veg_Cod, ")
            stb.AppendLine("   Occupazione_Cod as Occupazione_Cod_Agea, ")
            stb.AppendLine("   Destinazione_Cod as Destinazione_Cod_Agea, ")
            stb.AppendLine("   Uso_Cod as Uso_Cod_Agea, ")
            stb.AppendLine("   Qualita_Cod as Qualita_Cod_Agea, ")
            stb.AppendLine("   Cul_Cod_Agea, ")
            stb.AppendLine("   CONCAT(Cul_Cod_Agea, '|', Qualita_Cod, '|', Uso_Cod, '|', Destinazione_Cod, '|', Occupazione_Cod) as key_concat ")
            stb.AppendLine(" FROM Codifica_SpecieVegetali_Agea_2015_2020 ")
            stb.AppendLine(" WHERE 1=2 ")

            If listaSpecieAgea IsNot Nothing AndAlso listaSpecieAgea.Any Then
                listaSpecieAgea.Select(Function(species) " Occupazione_Cod = '" & species.Occupazione_Cod_Agea & "' ").
                    ToList.ForEach(Sub(filter) stb.AppendLine(" OR " & filter))
            End If

            Return EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function


    ''' <summary>
    ''' Legge le codifiche Agea 2015 / 2020 in sequenza (prima join su tutto, poi join su cultivar, poi join su id_cod
    ''' </summary>
    ''' <param name="stb">se passato viene restituito un DT che vale nothing</param>
    ''' <param name="piva"></param>
    ''' <param name="FiltroImpiantiConAND"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="Grfi_Cod"></param>
    ''' <param name="Grva_Cod"></param>
    ''' <param name="Metodo_Produzione_Cod"></param>
    ''' <param name="Reg_Cod"></param>
    ''' <param name="Id_Cod"></param>
    ''' <param name="Grsp_Cod"></param>
    ''' <param name="Cul_Cod_Agea"></param>
    ''' <param name="Uso_Cod_Agea"></param>
    ''' <param name="Occupazione_Cod_Agea"></param>
    ''' <param name="Destinazione_Cod_Agea"></param>
    ''' <param name="Qualita_Cod_Agea"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiCodificheSuImpiantiOppurePlanning(
        ByRef stb As StringBuilder,
        ByVal piva As String,
        ByVal FiltroImpiantiConAND As String,
        ByVal Veg_Cod As Integer,
        ByVal Cul_Cod As Integer,
        ByVal Grfi_Cod As Integer,
        ByVal Grva_Cod As Integer,
        ByVal Metodo_Produzione_Cod As Integer,
        ByVal Reg_Cod As Integer,
        ByVal Id_Cod As Integer,
        ByVal Grsp_Cod As Integer,
        ByVal Cul_Cod_Agea As String,
        ByVal Uso_Cod_Agea As String,
        ByVal Occupazione_Cod_Agea As String,
        ByVal Destinazione_Cod_Agea As String,
        ByVal Qualita_Cod_Agea As String,
        ByVal CodificaAgeaDaGias_livelloDettaglio As List(Of enum_CodificaAgeaDaGias_livelloDettaglio),
        ByVal SostiuisciSpecieSpecificaConSpecieGenerica As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try
            Dim standAlone As Boolean = True

            If stb Is Nothing Then
                stb = New StringBuilder
                stb.Length = 0
            Else
                standAlone = False
            End If

            If standAlone Then
                stb.AppendLine("        SELECT *")
                stb.AppendLine("        FROM (")
            End If

            stb.AppendLine("        --' VAnni: 7/3/2019: In ultimo, qualsiasi risultato sottante viene ugualmente raggruppato sulla chiave dell'impianto. Questo garantisce al 100% che non ci saranno righe duplicate.")
            stb.AppendLine("		SELECT")
            stb.AppendLine("			piva,")
            stb.AppendLine("			SA_COD,")
            stb.AppendLine("			APPEZZA,")
            stb.AppendLine("			ID_REG,")
            stb.AppendLine("			ag1.*")

            stb.AppendLine("		FROM (")

            stb.AppendLine("			SELECT")
            stb.AppendLine("				piva,")
            stb.AppendLine("				SA_COD,")
            stb.AppendLine("				APPEZZA,")
            stb.AppendLine("				ID_REG,")

            stb.AppendLine("				MIN(")
            stb.AppendLine("					Veg_cod_Agea + '-' +")
            stb.AppendLine("					Cul_Cod_Agea + '-' +")
            stb.AppendLine("					Uso_Cod + '-' +")
            stb.AppendLine("					Macrouso_Cod + '-' +")
            stb.AppendLine("					Occupazione_Cod + '-' +")
            stb.AppendLine("					Destinazione_Cod +'-' +")
            stb.AppendLine("					Qualita_Cod")
            stb.AppendLine("				) AS quintupla")

            stb.AppendLine("")
            stb.AppendLine("			FROM (")
            stb.AppendLine("				SELECT")

            qryGeneraSelectCaseAgea("Veg_Cod_Agea", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Cul_Cod_Agea", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Uso_Cod", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Macrouso_Cod", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Occupazione_Cod", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Destinazione_Cod", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")
            qryGeneraSelectCaseAgea("Qualita_Cod", CodificaAgeaDaGias_livelloDettaglio, stb)
            stb.Append(",")
            stb.AppendLine("")

            stb.AppendLine("					Reg_Impianti.piva,")
            stb.AppendLine("					Reg_Impianti.SA_COD,")
            stb.AppendLine("					Reg_Impianti.APPEZZA,")
            stb.AppendLine("					Reg_Impianti.ID_REG")
            stb.AppendLine("")
            stb.AppendLine("				FROM Reg_Impianti")
            stb.AppendLine("")

            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.CultivarPiCodici) Then


                stb.AppendLine("				--codifica di cultivar con codici gias ")
                stb.AppendLine("					LEFT JOIN (")
                stb.AppendLine("						SELECT")
                stb.AppendLine("							imp.PIVA,")
                stb.AppendLine("							imp.sa_cod,")
                stb.AppendLine("							imp.APPEZZA,")
                stb.AppendLine("							imp.id_reg,")
                stb.AppendLine("							ag1.*")
                If SostiuisciSpecieSpecificaConSpecieGenerica Then
                    SostiuisciSpecieSpecificaGetQuery(stb)
                Else
                    stb.AppendLine("					    FROM Reg_Impianti imp") 'qui
                End If

                stb.AppendLine("					        INNER JOIN Appezzamento_Codici a ON a.PIVA = imp.PIVA")
                stb.AppendLine("                                AND a.SA_COD = imp.SA_COD")
                stb.AppendLine("                                AND a.APPEZZA = imp.APPEZZA")
                stb.AppendLine("                                AND id_cod = 1018")

                stb.AppendLine("")
                stb.AppendLine("					        INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 ag ON imp.CUL_COD = ag.Cul_Cod")
                stb.AppendLine("                                AND imp.GRFI_COD = ag.Grfi_Cod")
                stb.AppendLine("                                AND imp.GRVA_Cod_VEG = ag.Grva_Cod")
                stb.AppendLine("                                AND imp.REGOLAMENTO = ag.Reg_Cod")
                stb.AppendLine("                                AND a.val_cod = ag.Metodo_Produzione_Cod")
                stb.AppendLine("					    WHERE ag.Id_Cod = 0")

                'filtri su codici Agronica
                If Veg_Cod <> 0 Then
                    stb.AppendLine("					        AND AG.veg_cod = " & Veg_Cod & vbCrLf)
                End If

                If Cul_Cod <> 0 Then
                    stb.AppendLine("					        AND AG.cul_cod = " & Cul_Cod & vbCrLf)
                End If


                If Grfi_Cod <> 0 Then
                    stb.AppendLine("					        AND AG.grfi_cod = " & Grfi_Cod & vbCrLf)
                End If

                If Grva_Cod <> 0 Then
                    stb.AppendLine("					        AND AG.grva_cod = " & Grva_Cod & vbCrLf)
                End If

                If Metodo_Produzione_Cod <> 0 Then
                    stb.AppendLine("					        AND AG.Metodo_Produzione_Cod = " & Metodo_Produzione_Cod & vbCrLf)
                End If

                If Reg_Cod <> 0 Then
                    stb.AppendLine("					        AND PE.Reg_Cod = " & Reg_Cod & vbCrLf)
                End If

                If Grsp_Cod <> 0 Then
                    stb.AppendLine("					        AND PE.grsp_cod = " & Grsp_Cod & vbCrLf)
                End If
                'fine filtri su codici Agronica


                'filtro AGEA
                If Cul_Cod_Agea <> "" Then
                    stb.AppendLine("					        AND AG1.Cul_Cod_Agea = " & Cul_Cod_Agea & vbCrLf)
                End If


                If Uso_Cod_Agea <> "" Then
                    stb.AppendLine("					        AND AG1.Uso_Cod_Agea = '" & Agro_SQL_SaveText(Uso_Cod_Agea) & "'" & vbCrLf)
                End If

                If Occupazione_Cod_Agea <> "" Then
                    stb.AppendLine("					        AND AG1.Occupazione_Cod_Agea = '" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Destinazione_Cod_Agea <> "" Then
                    stb.AppendLine("					        AND AG1.Destinazione_Cod_Agea = '" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Qualita_Cod_Agea <> "" Then
                    stb.AppendLine("					        AND AG1.Qualita_Cod_Agea = '" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "'" & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    stb.AppendLine("					        AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, standAlone, objParametri))
                End If

                stb.AppendLine("					) iCulPiuCod ON iCulPiuCod.PIVA = Reg_Impianti.PIVA")
                stb.AppendLine("					    And iCulPiuCod.SA_COD = Reg_Impianti.SA_COD  ")
                stb.AppendLine("					    And iCulPiuCod.APPEZZA = Reg_Impianti.APPEZZA ")
                stb.AppendLine("					    And iCulPiuCod.ID_REG = Reg_Impianti.ID_REG")


            End If
            '  --codifica di cultivar con codici gias 

            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.Cultivar) Then


                stb.AppendLine("				--codifica di solo cultivar")
                stb.AppendLine("					LEFT JOIN(")
                stb.AppendLine("						SELECT")
                stb.AppendLine("						    imp.PIVA,")
                stb.AppendLine("						    imp.sa_cod,")
                stb.AppendLine("						    imp.APPEZZA,")
                stb.AppendLine("						    imp.id_reg,")
                stb.AppendLine("						    ag1.*")
                If SostiuisciSpecieSpecificaConSpecieGenerica Then
                    SostiuisciSpecieSpecificaGetQuery(stb)
                Else
                    stb.AppendLine("					    FROM Reg_Impianti imp") 'qui
                End If
                stb.AppendLine("					        INNER JOIN(")
                stb.AppendLine("					            SELECT")
                stb.AppendLine("					                Veg_Cod ")
                stb.AppendLine("					                cul_cod ")
                stb.AppendLine("					                MIN(")
                stb.AppendLine("					                    Veg_cod_Agea + '-' +")
                stb.AppendLine("					                    Cul_Cod_Agea + '-' +")
                stb.AppendLine("					                    Uso_Cod + '-' +")
                stb.AppendLine("					                    Macrouso_Cod + '-' +")
                stb.AppendLine("					                    Occupazione_Cod + '-' +")
                stb.AppendLine("					                    Destinazione_Cod +'-' +")
                stb.AppendLine("					                    Qualita_Cod")
                stb.AppendLine("					                ) AS quintupla")
                stb.AppendLine("					            FROM Codifica_SpecieVegetali_Agea_2015_2020")
                stb.AppendLine("					            WHERE Veg_Cod <> 0 AND Cul_Cod <> 0")

                'filtro su cultivar Agronica
                If Cul_Cod <> 0 Then
                    stb.AppendLine("					                AND AG.cul_cod = " & Cul_Cod & vbCrLf)
                End If


                'filtro AGEA
                If Cul_Cod_Agea <> "" Then
                    stb.AppendLine("					                AND AG1.Cul_Cod_Agea = " & Cul_Cod_Agea & vbCrLf)
                End If


                If Uso_Cod_Agea <> "" Then
                    stb.AppendLine("					                AND AG1.Uso_Cod_Agea = '" & Agro_SQL_SaveText(Uso_Cod_Agea) & "'" & vbCrLf)
                End If

                If Occupazione_Cod_Agea <> "" Then
                    stb.AppendLine("					                AND AG1.Occupazione_Cod_Agea = '" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Destinazione_Cod_Agea <> "" Then
                    stb.AppendLine("					                AND AG1.Destinazione_Cod_Agea = '" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Qualita_Cod_Agea <> "" Then
                    stb.AppendLine("					                AND AG1.Qualita_Cod_Agea = '" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "'" & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    stb.AppendLine("					                AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, standAlone, objParametri))
                End If

                stb.AppendLine("							GROUP BY Veg_Cod,")
                stb.AppendLine("							    cul_cod")
                stb.AppendLine("						) ag ON imp.CUL_COD = ag.Cul_Cod")

                stb.AppendLine("")
                stb.AppendLine("						    INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 ag1")
                stb.AppendLine("						        ON SUBSTRING(ag.quintupla, 1,3) = ag1.Veg_cod_Agea")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 5,3) = ag1.cul_Cod_agea ")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 9,3) = ag1.Uso_Cod ")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 13,3) = ag1.Macrouso_Cod ")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 17,3) = ag1.Occupazione_Cod ")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 21,3) = ag1.Destinazione_Cod ")
                stb.AppendLine("						        AND SUBSTRING(ag.quintupla, 25,3) = ag1.Qualita_cod ")
                stb.AppendLine("					) iCul ON iCul.PIVA = Reg_Impianti.PIVA")
                stb.AppendLine("					    AND iCul.SA_COD = Reg_Impianti.SA_COD  ")
                stb.AppendLine("					    AND iCul.APPEZZA = Reg_Impianti.APPEZZA ")
                stb.AppendLine("					    AND iCul.ID_REG = Reg_Impianti.ID_REG ")
                stb.AppendLine("")
            End If
            ' codifica di solo cultivar

            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.SpecieVegetale) Then
                stb.AppendLine("				--codifica di sole specie vegetali (da noi esiste una cultivar specifica ma in AGEA NO) ")
                stb.AppendLine("					LEFT JOIN (")
                stb.AppendLine("						SELECT")
                stb.AppendLine("							imp.PIVA,")
                stb.AppendLine("							imp.sa_cod,")
                stb.AppendLine("							imp.APPEZZA,")
                stb.AppendLine("							imp.id_reg,")
                stb.AppendLine("							ag1.*")
                If SostiuisciSpecieSpecificaConSpecieGenerica Then
                    SostiuisciSpecieSpecificaGetQuery(stb)
                Else
                    stb.AppendLine("					FROM Reg_Impianti imp") 'qui
                End If
                stb.AppendLine("						INNER JOIN cultivar cul1 ON cul1.cul_cod = imp.cul_Cod")
                stb.AppendLine("")
                stb.AppendLine("						INNER JOIN (")
                stb.AppendLine("							SELECT")
                stb.AppendLine("								Veg_Cod,")
                stb.AppendLine("								grfi_Cod,")
                stb.AppendLine("					            MIN(")
                stb.AppendLine("					                Veg_cod_Agea + '-' +")
                stb.AppendLine("					                Cul_Cod_Agea + '-' +")
                stb.AppendLine("					                Uso_Cod + '-' +")
                stb.AppendLine("					                Macrouso_Cod + '-' +")
                stb.AppendLine("					                Occupazione_Cod + '-' +")
                stb.AppendLine("					                Destinazione_Cod +'-' +")
                stb.AppendLine("					                Qualita_Cod")
                stb.AppendLine("					            ) AS quintupla")
                stb.AppendLine("")
                stb.AppendLine("							FROM Codifica_SpecieVegetali_Agea_2015_2020")
                stb.AppendLine("")
                stb.AppendLine("							WHERE Veg_Cod <> 0")

                'filtro su cultivar Agronica
                If Cul_Cod <> 0 Then
                    stb.AppendLine("								AND AG.cul_cod = " & Cul_Cod & vbCrLf)
                End If


                'filtro AGEA
                If Cul_Cod_Agea <> "" Then
                    stb.AppendLine("								AND AG1.Cul_Cod_Agea = " & Cul_Cod_Agea & vbCrLf)
                End If


                If Uso_Cod_Agea <> "" Then
                    stb.AppendLine("								AND AG1.Uso_Cod_Agea = '" & Agro_SQL_SaveText(Uso_Cod_Agea) & "'" & vbCrLf)
                End If

                If Occupazione_Cod_Agea <> "" Then
                    stb.AppendLine("								AND AG1.Occupazione_Cod_Agea = '" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Destinazione_Cod_Agea <> "" Then
                    stb.AppendLine("								AND AG1.Destinazione_Cod_Agea = '" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Qualita_Cod_Agea <> "" Then
                    stb.AppendLine("								AND AG1.Qualita_Cod_Agea = '" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "'" & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    stb.AppendLine("								AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, standAlone, objParametri))
                End If

                stb.AppendLine("							GROUP BY Veg_Cod,")
                stb.AppendLine("								grfi_Cod")
                stb.AppendLine("						) ag ON cul1.veg_cod = ag.veg_Cod")
                stb.AppendLine("							AND imp.GRFI_COD = ag.Grfi_Cod")

                stb.AppendLine("")
                stb.AppendLine("						INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 ag1 ON SUBSTRING(ag.quintupla, 1,3) = ag1.Veg_cod_Agea")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 5,3) = ag1.cul_Cod_agea")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 9,3) = ag1.Uso_Cod")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 13,3) = ag1.Macrouso_Cod")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 17,3) = ag1.Occupazione_Cod")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 21,3) = ag1.Destinazione_Cod")
                stb.AppendLine("							AND SUBSTRING(ag.quintupla, 25,3) = ag1.Qualita_cod")
                stb.AppendLine("")
                stb.AppendLine("					) iVeg ON  iVeg.PIVA = Reg_Impianti.PIVA")
                stb.AppendLine("						AND iVeg.SA_COD = Reg_Impianti.SA_COD")
                stb.AppendLine("						AND iVeg.APPEZZA = Reg_Impianti.APPEZZA")
                stb.AppendLine("						AND iVeg.ID_REG = Reg_Impianti.ID_REG")
                stb.AppendLine("")


            End If
            '   --codifica di sole specie vegetali (da noi esiste una cultivar specifica ma in AGEA NO)

            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.CodiciAnagrafe) Then



                stb.AppendLine("					--codifica di soli id_cod ")
                stb.AppendLine("					LEFT JOIN (")
                stb.AppendLine("						SELECT")
                stb.AppendLine("							imp.PIVA,")
                stb.AppendLine("							imp.sa_cod,")
                stb.AppendLine("							imp.APPEZZA,")
                stb.AppendLine("							imp.id_reg,")
                stb.AppendLine("							ag1.*")
                stb.AppendLine("						FROM Reg_Impianti imp") 'qui

                stb.AppendLine("")
                stb.AppendLine("							INNER JOIN Reg_Impianti_Codici r ON  r.PIVA = imp.PIVA")
                stb.AppendLine("								AND r.SA_COD = imp.SA_COD")
                stb.AppendLine("								AND r.APPEZZA = imp.APPEZZA")
                stb.AppendLine("								AND r.Id_Reg = imp.ID_REG")
                stb.AppendLine("								AND r.Progetto_Cod = 0")

                stb.AppendLine("")
                stb.AppendLine("							INNER JOIN (")
                stb.AppendLine("								SELECT")
                stb.AppendLine("									Id_Cod,")
                stb.AppendLine("					                MIN(")
                stb.AppendLine("					                    Veg_cod_Agea + '-' +")
                stb.AppendLine("					                    Cul_Cod_Agea + '-' +")
                stb.AppendLine("					                    Uso_Cod + '-' +")
                stb.AppendLine("					                    Macrouso_Cod + '-' +")
                stb.AppendLine("					                    Occupazione_Cod + '-' +")
                stb.AppendLine("					                    Destinazione_Cod +'-' +")
                stb.AppendLine("					                    Qualita_Cod")
                stb.AppendLine("					                ) AS quintupla")
                stb.AppendLine("								FROM Codifica_SpecieVegetali_Agea_2015_2020")
                stb.AppendLine("")
                stb.AppendLine("								WHERE Id_Cod <> 0")

                'filtro su ID_Cod agronica
                If Id_Cod <> 0 Then
                    stb.AppendLine("								    AND id_cod = " & Id_Cod & vbCrLf)
                End If

                'filtro AGEA
                If Cul_Cod_Agea <> "" Then
                    stb.AppendLine("								    AND AG1.Cul_Cod_Agea = " & Cul_Cod_Agea & vbCrLf)
                End If


                If Uso_Cod_Agea <> "" Then
                    stb.AppendLine("								    AND AG1.Uso_Cod_Agea = '" & Agro_SQL_SaveText(Uso_Cod_Agea) & "'" & vbCrLf)
                End If

                If Occupazione_Cod_Agea <> "" Then
                    stb.AppendLine("								    AND AG1.Occupazione_Cod_Agea = '" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Destinazione_Cod_Agea <> "" Then
                    stb.AppendLine("								    AND AG1.Destinazione_Cod_Agea = '" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "'" & vbCrLf)
                End If

                If Qualita_Cod_Agea <> "" Then
                    stb.AppendLine("								    AND AG1.Qualita_Cod_Agea = '" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "'" & vbCrLf)
                End If

                If xFiltroAggiuntivo <> "" Then
                    stb.AppendLine("								    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, standAlone, objParametri))
                End If

                stb.AppendLine("								GROUP BY Id_Cod")
                stb.AppendLine("							) ag ON  r.id_cod = ag.Id_Cod")

                stb.AppendLine("")
                stb.AppendLine("							INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 ag1 ON SUBSTRING(ag.quintupla, 1,3) = ag1.Veg_cod_Agea")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 5,3) = ag1.cul_Cod_agea")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 9,3) = ag1.Uso_Cod")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 13,3) = ag1.Macrouso_Cod")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 17,3) = ag1.Occupazione_Cod")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 21,3) = ag1.Destinazione_Cod")
                stb.AppendLine("								AND SUBSTRING(ag.quintupla, 25,3) = ag1.Qualita_Cod")

                stb.AppendLine("")
                stb.AppendLine("					) idCod ON  idCod.PIVA = Reg_Impianti.PIVA")
                stb.AppendLine("						AND idCod.SA_COD = Reg_Impianti.SA_COD")
                stb.AppendLine("						AND idCod.APPEZZA = Reg_Impianti.APPEZZA")
                stb.AppendLine("						AND idCod.ID_REG = Reg_Impianti.ID_REG")
            End If
            ' codifica di soli id_cod 


            stb.AppendLine("			) a")
            stb.AppendLine("")
            stb.AppendLine("			GROUP BY piva,")
            stb.AppendLine("				SA_COD,")
            stb.AppendLine("				APPEZZA,")
            stb.AppendLine("				ID_REG")
            stb.AppendLine("")
            stb.AppendLine("		) ns")
            stb.AppendLine("")
            stb.AppendLine("			INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 ag1 ON SUBSTRING(ns.quintupla, 1,3) = ag1.Veg_cod_Agea")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 5,3) = ag1.cul_Cod_agea")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 9,3) = ag1.Uso_Cod")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 13,3) = ag1.Macrouso_Cod")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 17,3) = ag1.Occupazione_Cod")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 21,3) = ag1.Destinazione_Cod")
            stb.AppendLine("				AND SUBSTRING(ns.quintupla, 25,3) = ag1.Qualita_cod")


            If standAlone Then

                stb.AppendLine("    ) AG2015")

                If xOrderBy <> "" Then
                    stb.AppendLine("    ORDER BY " & xOrderBy & vbCrLf)
                End If



                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Else
                DT = Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Private Shared Sub SostiuisciSpecieSpecificaGetQuery(stb As StringBuilder)
        stb.AppendLine("						FROM (")
        stb.AppendLine("							SELECT")
        stb.AppendLine("								i.PIVA,")
        stb.AppendLine("								i.SA_COD,")
        stb.AppendLine("								i.APPEZZA,")
        stb.AppendLine("								i.ID_REG,")
        stb.AppendLine("								cAltre.Cul_Cod,")
        stb.AppendLine("								i.GRFI_COD,")
        stb.AppendLine("								i.GRVA_Cod_VEG,")
        stb.AppendLine("								i.REGOLAMENTO /*, veg.Veg_Cod, veg.veg_des, cAltre.Cul_Cod, cAltre.Cul_Des*/")

        stb.AppendLine("")
        stb.AppendLine("							FROM Reg_Impianti i")
        stb.AppendLine("								INNER JOIN Cultivar c ON c.Cul_Cod = i.CUL_COD")
        stb.AppendLine("")
        stb.AppendLine("								INNER JOIN SpecieVegetali veg ON veg.Veg_Cod = c.Veg_Cod")
        stb.AppendLine("")
        stb.AppendLine("								INNER JOIN(")
        stb.AppendLine("									SELECT")
        stb.AppendLine("										Veg_Cod,")
        stb.AppendLine("										Cul_Cod,")
        stb.AppendLine("										Cul_Des")
        stb.AppendLine("									FROM Cultivar")
        stb.AppendLine("									WHERE Cul_Des = 'altre'")
        stb.AppendLine("								) cAltre ON cAltre.Veg_Cod = c.Veg_Cod")
        stb.AppendLine("						) imp")
    End Sub

    Private Shared Sub qryGeneraSelectCaseAgea(CampoSelect As String, ByVal CodificaAgeaDaGias_livelloDettaglio As List(Of enum_CodificaAgeaDaGias_livelloDettaglio), stb As StringBuilder)

        Dim EndDaChiudere As Integer = 0
        If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.CultivarPiCodici) Then
            stb.AppendLine("         Case when Not iCulPiuCod. " & CampoSelect & " Is null then iCulPiuCod." & CampoSelect)
            EndDaChiudere += 1
            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse EndDaChiudere < CodificaAgeaDaGias_livelloDettaglio.Count Then
                stb.Append(" else ")
            End If
        End If

        If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.Cultivar) Then
            stb.AppendLine("           Case when Not iCul." & CampoSelect & " Is null then iCul." & CampoSelect)
            EndDaChiudere += 1
            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse EndDaChiudere < CodificaAgeaDaGias_livelloDettaglio.Count Then
                stb.Append(" else ")
            End If
        End If

        If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.SpecieVegetale) Then
            stb.AppendLine("					Case")
            stb.AppendLine("						when Not iVeg." & CampoSelect & " Is null")
            stb.AppendLine("						then iVeg." & CampoSelect)
            EndDaChiudere += 1
            If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse EndDaChiudere < CodificaAgeaDaGias_livelloDettaglio.Count Then
                stb.AppendLine("						else")
            End If
        End If

        If CodificaAgeaDaGias_livelloDettaglio.Count = 0 OrElse CodificaAgeaDaGias_livelloDettaglio.Contains(enum_CodificaAgeaDaGias_livelloDettaglio.CodiciAnagrafe) Then
            stb.AppendLine("							Case when Not idCod." & CampoSelect & " Is null then idCod." & CampoSelect)
            EndDaChiudere += 1
        End If

        For i = 1 To EndDaChiudere
            stb.AppendLine("						End")
        Next

        stb.AppendLine("						As " & CampoSelect)
    End Sub

End Class

