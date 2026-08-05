Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class ParamEntrataXSpecieVarieta_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Leggi(ByVal Piva As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer?,
                          ByVal Reg_Cod As Integer?,
                          ByVal Percentuale_Degrado As Decimal?,
                          ByVal Riferimento_Prezzi As String,
                          ByVal Data_Riferimento As DateTime,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          ByVal Optional Id_Param As Integer? = Nothing
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        If String.IsNullOrEmpty(Piva) Then
            Throw New Exception(" Parametro Piva obbligatorio")
        End If

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM ParamEntrataXSpecieVarieta p ")
                    StrSQL.AppendLine(" WHERE p.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND p.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND p.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                    End If

                    If Cul_Cod IsNot Nothing AndAlso Cul_Cod.HasValue Then
                        StrSQL.AppendLine(" AND p.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
                    End If

                    If Reg_Cod IsNot Nothing AndAlso Reg_Cod.HasValue Then
                        StrSQL.AppendLine(" AND p.Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod))
                    End If

                    If Percentuale_Degrado IsNot Nothing AndAlso Percentuale_Degrado.HasValue Then
                        StrSQL.AppendLine(" AND p.Percentuale_Degrado = " & Agro_SQL_SaveNum(Percentuale_Degrado))
                    End If

                    If Not String.IsNullOrEmpty(Riferimento_Prezzi) Then
                        StrSQL.AppendLine(" AND p.Riferimento_Prezzi = '" & Agro_SQL_SaveText(Riferimento_Prezzi) & "' ")
                    End If

                    If Data_Riferimento <> AGRODATAINIZIO Then
                        StrSQL.AppendLine(" AND p.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                        StrSQL.AppendLine(" AND p.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                    End If

                    If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    Throw New NotImplementedException("Leggi Selezione_TabellaCompleta Non implementata")

                'Da fare:
                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    'Togliere la chiave composta e usare come chiave Id_param
                    StrSQL.AppendLine(" SELECT psv.Id_Param,psv.PivaSuperUser, psv.Piva, psv.Veg_Cod, psv.Cul_Cod, psv.Percentuale_Degrado,")
                    StrSQL.AppendLine(" psv.Riferimento_Prezzi as Riferimento_Prezzi_Cod, psv.Validita_Inizio, psv.Validita_Fine,")
                    StrSQL.AppendLine(" coalesce(c.Cul_Des,'') as Cul_Des,coalesce(spec.Veg_Des,'') as Veg_Des, ")
                    StrSQL.AppendLine(" psv.Data_Creazione,psv.inviato,psv.Username_Creazione, ")
                    StrSQL.AppendLine(" CASE ")
                    StrSQL.AppendLine("      WHEN psv.Riferimento_Prezzi = ' ' THEN '' ")
                    StrSQL.AppendLine("      WHEN psv.Riferimento_Prezzi = 'E' THEN 'Data Entrata' ")
                    StrSQL.AppendLine(" 	 WHEN psv.Riferimento_Prezzi = 'S' THEN 'Data Semina'  ")
                    StrSQL.AppendLine(" END ")
                    StrSQL.AppendLine(" as Riferimento_Prezzi_Des,  ")
                    StrSQL.AppendLine(" CASE ")
                    StrSQL.AppendLine("      WHEN psv.FormulaFissaLiquidazione = " & Agro_SQL_SaveNum(enum_FormuleFisseLiquidazioneFF.FFL_Nessuna) & " THEN 'Nessuna' ")
                    StrSQL.AppendLine(" 	 WHEN psv.FormulaFissaLiquidazione = " & Agro_SQL_SaveNum(enum_FormuleFisseLiquidazioneFF.FFL_ResiduoSeccoBorlotto_45_50) & " THEN 'Residuo Secco Borlotto 45 / 50' ")
                    StrSQL.AppendLine(" END ")
                    StrSQL.AppendLine(" as FormulaFissaLiquidazione_Des,  ")
                    StrSQL.AppendLine(" psv.FormulaFissaLiquidazione AS  FormulaFissaLiquidazione_Cod, ")
                    StrSQL.AppendLine(" CASE ")
                    StrSQL.AppendLine("      WHEN psv.Reg_Cod = 0 THEN '' ")
                    StrSQL.AppendLine(" 	 WHEN psv.Reg_Cod = 1 THEN 'Integrato' ")
                    StrSQL.AppendLine(" 	 WHEN psv.Reg_Cod = 4 THEN 'Biologico' ")
                    StrSQL.AppendLine(" END ")
                    StrSQL.AppendLine(" as Regolamento_Des,  ")
                    StrSQL.AppendLine(" psv.Reg_Cod AS Regolamento_Cod ")
                    StrSQL.AppendLine(" FROM ParamEntrataXSpecieVarieta psv ")
                    StrSQL.AppendLine(" LEFT JOIN  specievegetali spec")
                    StrSQL.AppendLine(" ON psv.Veg_Cod = spec.Veg_Cod")
                    StrSQL.AppendLine(" LEFT JOIN  cultivar c")
                    StrSQL.AppendLine(" ON psv.cul_Cod = c.Cul_Cod")
                    StrSQL.AppendLine(" AND psv.Veg_Cod = c.Veg_Cod")
                    StrSQL.AppendLine(" WHERE psv.piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                    If Id_Param IsNot Nothing Then
                        StrSQL.AppendLine(" AND psv.Id_Param = " & Agro_SQL_SaveNum(Id_Param))
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND psv.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                    End If

                    If Cul_Cod IsNot Nothing AndAlso Cul_Cod.HasValue AndAlso Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND psv.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
                    End If

                    If Percentuale_Degrado IsNot Nothing AndAlso Percentuale_Degrado.HasValue Then
                        StrSQL.AppendLine(" AND psv.Percentuale_Degrado = " & Agro_SQL_SaveNum(Percentuale_Degrado))
                    End If

                    If Not String.IsNullOrEmpty(Riferimento_Prezzi) Then
                        StrSQL.AppendLine(" AND psv.Riferimento_Prezzi = '" & Agro_SQL_SaveText(Riferimento_Prezzi) & "' ")
                    End If

                    If Data_Riferimento <> AGRODATAINIZIO Then
                        StrSQL.AppendLine(" AND psv.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                        StrSQL.AppendLine(" AND psv.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Riferimento) & " ")
                    End If

                    If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0
                    Throw New NotImplementedException("Leggi Selezione_JoinCompleta Non implementata")

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class ParamEntrataXSpecieVarieta_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '#########################################################################
    Public Function Scrivi(ByVal piva As String,
                           ByVal EFArrayToInsert As ArrayList,
                           ByVal EFArrayToUpdate As ArrayList,
                           ByVal EFArrayToDelete As ArrayList,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            'Prova di scrittura

            'Scrittura in Entity Framework 
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

                ' Contiene anche i dettagli

                For Each listProdotti As ParamEntrataXSpecieVarieta In EFArrayToInsert
                    GiasContext.ParamEntrataXSpecieVarieta.Add(listProdotti)
                Next

                For Each listProdotti As ParamEntrataXSpecieVarieta In EFArrayToUpdate
                    GiasContext.ParamEntrataXSpecieVarieta.Attach(listProdotti)
                    GiasContext.Entry(listProdotti).State = EntityState.Modified
                Next

                For Each listProdotti As ParamEntrataXSpecieVarieta In EFArrayToDelete
                    GiasContext.ParamEntrataXSpecieVarieta.Attach(listProdotti)
                    GiasContext.ParamEntrataXSpecieVarieta.Remove(listProdotti)
                Next

                GiasContext.SaveChanges()
                'Gias Context.SaveChanges() è come se fosse una transazione se c'è un errore,
                'nelle righe inserite,cancellate o modificate viene annullata tutta la scrittura
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


    '#########################################################################
    Public Function Modifica(ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    '#########################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParamEntrataXSpecieVarieta_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '--------------------------------------------------------------------------
            'xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
