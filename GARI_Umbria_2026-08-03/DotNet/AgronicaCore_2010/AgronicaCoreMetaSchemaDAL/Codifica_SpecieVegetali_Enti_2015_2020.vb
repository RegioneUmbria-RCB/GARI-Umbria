Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Codifica_SpecieVegetali_Enti_2015_2020_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function leggiNonCodificati(ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       Ente_Cod As enum_Planning_Fonte) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case Ente_Cod

                Case enum_Planning_Fonte.Agrea
                    Stb.AppendLine("Select * From Codifica_SpecieVegetali_Agrea ao ")
                    Stb.AppendLine(" Left Join [Codifica_SpecieVegetali_Enti_2015_2020] e ON ao.Veg_Cod_Agrea=e.Veg_Cod_Ente And ao.Cul_Cod_Agrea = e. Cul_Cod_Ente ")
                    Stb.AppendLine(" WHERE e.Cul_Cod_Agea Is null")

                Case enum_Planning_Fonte.Avepa
                    Stb.AppendLine("Select * From Codifica_SpecieVegetali_Avepa ao ")
                    Stb.AppendLine(" Left Join [Codifica_SpecieVegetali_Enti_2015_2020] e ON ao.Veg_Cod_Avepa=e.Veg_Cod_Ente And ao.Cul_Cod_avepa = e. Cul_Cod_Ente ")
                    Stb.AppendLine(" WHERE e.Cul_Cod_Agea Is null")

                Case enum_Planning_Fonte.Artea
                    Stb.AppendLine("Select * From Codifica_SpecieVegetali_Artea ao ")
                    Stb.AppendLine(" Left Join [Codifica_SpecieVegetali_Enti_2015_2020] e ON ao.Veg_Cod_Artea=e.Veg_Cod_Ente And ao.Cul_Cod_Artea = e. Cul_Cod_Ente ")
                    Stb.AppendLine(" WHERE e.Cul_Cod_Agea Is null")

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function leggiNonCodificati_in_AGEA(ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               Ente_Cod As enum_Planning_Fonte) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case Ente_Cod

                Case enum_Planning_Fonte.Agrea
                    Stb.AppendLine("Select ")
                    Stb.AppendLine(" ao.*,  ")
                    Stb.AppendLine(" e.*,  ")
                    Stb.AppendLine(" e.Veg_Cod_Agea as Veg_Cod_Agea_New, ")
                    Stb.AppendLine(" e.Cul_Cod_Agea as Cul_Cod_Agea_New")
                    Stb.AppendLine("From Codifica_SpecieVegetali_Agrea ao ")
                    Stb.AppendLine("Left Join Codifica_SpecieVegetali_Enti_2015_2020 e ON ao.Veg_Cod_Agrea=e.Veg_Cod_Ente And ao.Cul_Cod_Agrea = e.Cul_Cod_Ente ")
                    Stb.AppendLine("Left Join Codifica_SpecieVegetali_Agea_2015_2020 ag ON e.Veg_Cod_Agea = ag.Veg_Cod_Agea ")
                    Stb.AppendLine("  And e.Cul_Cod_Agea = ag.Cul_Cod_Agea ")
                    Stb.AppendLine("  And e.Qualita_Cod = ag.Qualita_Cod ")
                    Stb.AppendLine("  And e.Macrouso_Cod = ag.Macrouso_Cod ")
                    Stb.AppendLine("  And e.Occupazione_Cod = ag.Occupazione_Cod ")
                    Stb.AppendLine("  And e.Destinazione_Cod = ag.Destinazione_Cod ")
                    Stb.AppendLine("  And e.Uso_Cod = ag.Uso_Cod  ")
                    Stb.AppendLine("  And e.Ente_cod = " & CStr(Ente_Cod) & " ")
                    Stb.AppendLine("WHERE ag.Veg_cod = 0 And ag.Id_cod = 0")


                Case enum_Planning_Fonte.Avepa
                    Stb.AppendLine("Select * From Codifica_SpecieVegetali_Avepa ao ")
                    Stb.AppendLine(" Left Join [Codifica_SpecieVegetali_Enti_2015_2020] e ON ao.Veg_Cod_Avepa=e.Veg_Cod_Ente And ao.Cul_Cod_avepa = e. Cul_Cod_Ente ")
                    Stb.AppendLine(" WHERE e.Cul_Cod_Agea Is null")

                Case enum_Planning_Fonte.Artea
                    Stb.AppendLine("Select * From Codifica_SpecieVegetali_Artea ao ")
                    Stb.AppendLine(" Left Join [Codifica_SpecieVegetali_Enti_2015_2020] e ON ao.Veg_Cod_Artea=e.Veg_Cod_Ente And ao.Cul_Cod_Artea = e. Cul_Cod_Ente ")
                    Stb.AppendLine(" WHERE e.Cul_Cod_Agea Is null")

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function leggi(ByRef objParametri_Server As AgronicaCoreParametri,
                           Ente_Cod As enum_Planning_Fonte,
                          Veg_Cod_Ente As String,
                          Cul_Cod_Ente As String,
                          Veg_cod_Agea As String,
                          Cul_Cod_Agea As String,
                          Uso_Cod As String,
                          Macrouso_Cod As String,
                          Occupazione_Cod As String,
                          Destinazione_Cod As String,
                          Qualita_Cod As String) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.AppendLine(" Select * ")
            Stb.AppendLine(" From [dbo].[Codifica_SpecieVegetali_Enti_2015_2020] ")
            Stb.AppendLine(" Where 1 = 1 And ")
            If Ente_Cod <> 0 Then
                Stb.AppendLine(" Ente_Cod = " & Agro_SQL_SaveNum(Ente_Cod) & " And ")
            End If
            If Veg_Cod_Ente <> "" Then
                Stb.AppendLine(" Veg_Cod_Ente = " & Agro_SQL_SaveText_NULL(Veg_Cod_Ente) & " AND ")
            End If
            If Cul_Cod_Ente <> "" Then
                Stb.AppendLine(" Cul_Cod_Ente = " & Agro_SQL_SaveText_NULL(Cul_Cod_Ente) & " AND ")
            End If
            If Veg_cod_Agea <> "" Then
                Stb.AppendLine(" Veg_cod_Agea = " & Agro_SQL_SaveText_NULL(Veg_cod_Agea) & " AND ")
            End If
            If Cul_Cod_Agea <> "" Then
                Stb.AppendLine(" Cul_Cod_Agea = " & Agro_SQL_SaveText_NULL(Cul_Cod_Agea) & " AND ")
            End If
            If Uso_Cod <> "" Then
                Stb.AppendLine(" Uso_Cod = " & Agro_SQL_SaveText_NULL(Uso_Cod) & " AND ")
            End If
            If Macrouso_Cod <> "" Then
                Stb.AppendLine(" Macrouso_Cod = " & Agro_SQL_SaveText_NULL(Macrouso_Cod) & " AND ")
            End If
            If Occupazione_Cod <> "" Then
                Stb.AppendLine(" Occupazione_Cod = " & Agro_SQL_SaveText_NULL(Occupazione_Cod) & " AND ")
            End If
            If Destinazione_Cod <> "" Then
                Stb.AppendLine(" Destinazione_Cod = " & Agro_SQL_SaveText_NULL(Destinazione_Cod) & " AND ")
            End If
            If Qualita_Cod <> "" Then
                Stb.AppendLine(" Qualita_Cod = " & Agro_SQL_SaveText_NULL(Qualita_Cod) & " AND ")
            End If
            Stb.AppendLine(" 1 = 1")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function Specie_e_Varieta_Gias_Da_Ente(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Ente_Cod As enum_Planning_Fonte,
                                                ByVal Veg_Cod_Ente As String,
                                                ByVal Cul_Cod_Ente As String,
                                                ByRef Veg_Cod_Agea As String,
                                                ByRef Cul_Cod_Agea As String,
                                                ByRef Uso_Cod_Agea As String,
                                                ByRef Occupazione_Cod_Agea As String,
                                                ByRef Destinazione_Cod_Agea As String,
                                                ByRef Qualita_Cod_Agea As String,
                                                ByRef Veg_Cod As Integer,
                                                ByRef Cul_Cod As Integer,
                                                ByRef Grfi_Cod As Integer,
                                                ByRef Grva_Cod As Integer,
                                                ByRef Id_Cod As Integer,
                                                ByRef Specie_Des_Agea As String,
                                                ByRef Varieta_Des_Agea As String,
                                                ByVal Specie_Des_AgeaInput As String,
                                                ByVal Varieta_Des_AgeaInput As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean

        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_Ente"

        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel_Completo As String = ""
        Dim DtCodifica As DataTable
        Dim DrVar() As DataRow
        Dim DtAgea As DataTable
        Dim Esito As Boolean = False
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Id_Cod = 0
        Grva_Cod = 0
        Specie_Des_Agea = ""
        Varieta_Des_Agea = ""

        'If Veg_Cod_Agea = "154" Then
        '    Dim i = 0
        'End If


        'LEGGO TUTTI I RECORD DELLA SPECIE  
        'filtro poi se è mappata anche la varietà
        'altrimenti metto 'altre'....

        Dim objEnteR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Enti_2015_2020_R
        Dim objAgeaR As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        Dim objMeta As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        DtCodifica = objEnteR.leggi(objParametri, Ente_Cod, Veg_Cod_Ente, "", "", "", "", "", "", "", "")

        Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti_Excel.csv"
                    }

        Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti.txt"
                    }

        Dim customLOGParamsCsvCompleto As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti_Excel_Completo.csv"
                    }

        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Ente='" & Cul_Cod_Ente.ToString & "'")

            If DrVar IsNot Nothing AndAlso DrVar.Length > 0 Then

                DtAgea = objAgeaR.leggi(objParametri, DrVar(0).Item("Veg_Cod_Agea"), DrVar(0).Item("Cul_Cod_Agea"), DrVar(0).Item("Uso_Cod"), "", DrVar(0).Item("Occupazione_Cod"), DrVar(0).Item("Destinazione_Cod"), DrVar(0).Item("Qualita_Cod"), 0, 0, 0, 0, 0, 0, 0, 0)
                If DtAgea IsNot Nothing AndAlso DtAgea.Rows.Count > 0 Then
                    Veg_Cod = DtAgea.Rows(0).Item("Veg_Cod")
                    Cul_Cod = DtAgea.Rows(0).Item("Cul_Cod")
                    Grfi_Cod = DtAgea.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtAgea.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtAgea.Rows(0).Item("Grva_Cod")

                    Veg_Cod_Agea = DtAgea.Rows(0).Item("Veg_Cod_Agea")
                    Cul_Cod_Agea = DtAgea.Rows(0).Item("Cul_Cod_Agea")
                    Uso_Cod_Agea = DtAgea.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod_Agea = DtAgea.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod_Agea = DtAgea.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod_Agea = DtAgea.Rows(0).Item("Qualita_Cod")

                    If Not IsDBNull(DtAgea.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtAgea.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If
                    Esito = True

                    If Veg_Cod <> 0 AndAlso Cul_Cod = 0 Then
                        Cul_Cod = objMeta.VarietaAltre(Veg_Cod, objParametri)
                    End If

                    If Veg_Cod = 0 AndAlso Cul_Cod = 0 AndAlso Id_Cod = 0 Then
                        Specie_Des_Agea = "Nessuna mappatura con Gias"
                        Varieta_Des_Agea = "Nessuna mappatura con Gias"
                        Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Agrea=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Agrea=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If

                Else
                    Specie_Des_Agea = "Nessuna mappatura con Gias"
                    Varieta_Des_Agea = "Nessuna mappatura con Gias"
                    Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                    'Manca collegamento Agrea-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agrea=" & CStr(Veg_Cod_Ente) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agrea=" & CStr(Cul_Cod_Ente) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If

            Else

                If Cul_Cod_Ente <> "" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agrea=" & CStr(Veg_Cod_Ente) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agrea=" & CStr(Cul_Cod_Ente) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    'Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If


                DtAgea = objAgeaR.leggi(objParametri, "", "", DtCodifica.Rows(0).Item("Uso_Cod"), "", DtCodifica.Rows(0).Item("Occupazione_Cod"), DtCodifica.Rows(0).Item("Destinazione_Cod"), DtCodifica.Rows(0).Item("Qualita_Cod"), 0, 0, 0, 0, 0, 0, 0, 0)

                If DtAgea IsNot Nothing AndAlso DtAgea.Rows.Count > 0 Then
                    Veg_Cod = DtAgea.Rows(0).Item("Veg_Cod")
                    If Veg_Cod = 0 Then
                        Cul_Cod = 0
                    Else
                        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri)
                    End If

                    Grfi_Cod = DtAgea.Rows(0).Item("Grfi_Cod")
                    Id_Cod = DtAgea.Rows(0).Item("Id_Cod")
                    Grva_Cod = DtAgea.Rows(0).Item("Grva_Cod")
                    Specie_Des_Agea = DtAgea.Rows(0).Item("Veg_Des_Agea")

                    Veg_Cod_Agea = DtAgea.Rows(0).Item("Veg_Cod_Agea")
                    Cul_Cod_Agea = DtAgea.Rows(0).Item("Cul_Cod_Agea")
                    Uso_Cod_Agea = DtAgea.Rows(0).Item("Uso_Cod")
                    Occupazione_Cod_Agea = DtAgea.Rows(0).Item("Occupazione_Cod")
                    Destinazione_Cod_Agea = DtAgea.Rows(0).Item("Destinazione_Cod")
                    Qualita_Cod_Agea = DtAgea.Rows(0).Item("Qualita_Cod")

                    If Not IsDBNull(DtAgea.Rows(0).Item("Cul_Des_Agea")) Then
                        Varieta_Des_Agea = DtAgea.Rows(0).Item("Cul_Des_Agea")
                    Else
                        Varieta_Des_Agea = ""
                    End If

                    Esito = True

                    If Veg_Cod <> 0 AndAlso Cul_Cod = 0 Then
                        Cul_Cod = objMeta.VarietaAltre(Veg_Cod, objParametri)
                    End If

                    If Veg_Cod = 0 AndAlso Cul_Cod = 0 AndAlso Id_Cod = 0 Then
                        Specie_Des_Agea = "Nessuna mappatura con Gias"
                        Varieta_Des_Agea = "Nessuna mappatura con Gias"
                        Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                        LogCodificheMancantiVarieta &= "Specie: " & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Agrea=" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") &
                        " - la varietà " & DtAgea.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Agrea=" & DtAgea.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                        LogCodificheMancantiVarieta_Excel &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & DtAgea.Rows(0).Item("Veg_Des_AGEA") & ";" & DtAgea.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtAgea.Rows(0).Item("Cul_Des_Agea") & ";" & DtAgea.Rows(0).Item("Cul_Cod_AGEA")
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                        Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                        Esito = False
                    End If
                Else
                    Specie_Des_Agea = "Nessuna mappatura con Gias"
                    Varieta_Des_Agea = "Nessuna mappatura con Gias"
                    Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
                    'Manca collegamento Agrea-AGEA
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agrea=" & CStr(Veg_Cod_Ente) &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agrea=" & CStr(Cul_Cod_Ente) & " non è mappata con AGEA." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If
            End If

        Else
            Veg_Cod = 0
            Cul_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Id_Cod = enum_CodiciAnagrafe.NessunaMappaturaConGias
            Specie_Des_Agea = "Nessuna mappatura con Gias"
            Varieta_Des_Agea = "Nessuna mappatura con Gias"
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Agrea=" & CStr(Veg_Cod_Ente) & "  " & Specie_Des_AgeaInput & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Ente)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Ente) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Ente)
            customLOGParamsTxt.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti.txt"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)
            customLOGParamsCsv.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)
            customLOGParamsCsvCompleto.LogFileName = "CodificaSpecieVegetaliAvepa_SPECIE_mancanti_Excel_Completo.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)


        End If

        Return Esito
    End Function

End Class

Public Class Codifica_SpecieVegetali_Enti_2015_2020_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                           Ente_Cod As Integer,
                           Veg_Cod_Ente As String,
                           Cul_Cod_Ente As String,
                           Veg_des_Ente As String,
                           Cul_Des_Ente As String,
                           Veg_Cod_Agea As String,
                           Cul_Cod_Agea As String,
                           Veg_des_Agea As String,
                           Cul_Des_Agea As String,
                           Uso_Cod As String,
                           Uso_Des As String,
                           Macrouso_Cod As String,
                           Macrouso_Des As String,
                           Occupazione_Cod As String,
                           Occupazione_Des As String,
                           Destinazione_Cod As String,
                           Destinazione_Des As String,
                           Qualita_Cod As String,
                           Qualita_Des As String,
                           inviato As Integer,
                           datainvio As DateTime,
                           Data_Creazione As DateTime,
                           Data_Modifica As DateTime,
                           Username_Creazione As String,
                           Username_Modifica As String,
                           Validita_Inizio As Date,
                           Validita_Fine As Date
        ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim result As Boolean

        Try

            Stb.AppendLine("INSERT INTO Codifica_SpecieVegetali_Enti_2015_2020 ")
            Stb.AppendLine("            ([Ente_cod] ")
            Stb.AppendLine("            ,[Veg_Cod_Ente] ")
            Stb.AppendLine("            ,[Cul_Cod_Ente] ")
            Stb.AppendLine("            ,[Veg_Des_Ente] ")
            Stb.AppendLine("            ,[Cul_Des_Ente] ")
            Stb.AppendLine("            ,[Veg_Cod_Agea] ")
            Stb.AppendLine("            ,[Veg_Des_Agea] ")
            Stb.AppendLine("            ,[Cul_Cod_Agea] ")
            Stb.AppendLine("            ,[Cul_Des_Agea] ")
            Stb.AppendLine("            ,[Uso_Cod] ")
            Stb.AppendLine("            ,[Uso_Des] ")
            Stb.AppendLine("            ,[Macrouso_Cod] ")
            Stb.AppendLine("            ,[Macrouso_Des] ")
            Stb.AppendLine("            ,[Occupazione_Cod] ")
            Stb.AppendLine("            ,[Occupazione_Des] ")
            Stb.AppendLine("            ,[Destinazione_Cod] ")
            Stb.AppendLine("            ,[Destinazione_Des] ")
            Stb.AppendLine("            ,[Qualita_Cod] ")
            Stb.AppendLine("            ,[Qualita_Des] ")
            Stb.AppendLine("            ,[inviato] ")
            Stb.AppendLine("            ,[datainvio] ")
            Stb.AppendLine("            ,[Data_Creazione] ")
            Stb.AppendLine("            ,[Data_Modifica] ")
            Stb.AppendLine("            ,[Username_Creazione] ")
            Stb.AppendLine("            ,[Username_Modifica] ")
            Stb.AppendLine("            ,[Validita_Inizio] ")
            Stb.AppendLine("            ,[Validita_Fine]) ")
            Stb.AppendLine("      VALUES ")
            Stb.AppendLine("            (" & Agro_SQL_SaveNum(Ente_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Veg_Cod_Ente) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Cul_Cod_Ente) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Veg_des_Ente) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Cul_Des_Ente) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Veg_Cod_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Veg_des_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Cul_Cod_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Cul_Des_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Uso_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Uso_Des) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Macrouso_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Macrouso_Des) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Occupazione_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Occupazione_Des) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Destinazione_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Destinazione_Des) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Qualita_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Qualita_Des) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum(inviato) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(datainvio) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Data_Creazione) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Username_Creazione) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Username_Modifica) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime(Validita_Fine) & " )")


            result = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            result = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result
    End Function

    Public Function Aggiorna_Da_Specie_Varieta_Occupazione(ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                           Ente_Cod As Integer,
                           Veg_Cod_Ente As String,
                           Cul_Cod_Ente As String,
                           Uso_Cod As String,
                           Uso_Des As String,
                           Occupazione_Cod As String,
                           Occupazione_Des As String,
                           Destinazione_Cod As String,
                           Destinazione_Des As String,
                           Qualita_Cod As String,
                           Qualita_Des As String) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim result As Boolean

        Try


            Stb.AppendLine(" UPDATE Codifica_SpecieVegetali_Enti_2015_2020 ")
            Stb.AppendLine(" SET ")
            Stb.AppendLine("     Uso_Cod = " & Agro_SQL_SaveText_NULL(Uso_Cod) & " , ")
            Stb.AppendLine("     Uso_Des = " & Agro_SQL_SaveText_NULL(Uso_Des) & ", ")
            Stb.AppendLine("     Destinazione_Cod = " & Agro_SQL_SaveText_NULL(Destinazione_Cod) & ", ")
            Stb.AppendLine("     Destinazione_Des = " & Agro_SQL_SaveText_NULL(Destinazione_Des) & ", ")
            Stb.AppendLine("     Qualita_Cod = " & Agro_SQL_SaveText_NULL(Qualita_Cod) & ", ")
            Stb.AppendLine("     Qualita_Des = " & Agro_SQL_SaveText_NULL(Qualita_Des) & ", ")
            Stb.AppendLine("     Data_Modifica = " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            Stb.AppendLine(" WHERE ")
            Stb.AppendLine("             Ente_Cod = " & Agro_SQL_SaveNum(Ente_Cod) & " And ")
            Stb.AppendLine("     Veg_Cod_Ente = " & Agro_SQL_SaveText_NULL(Veg_Cod_Ente) & " AND  ")
            Stb.AppendLine("     Cul_Cod_Ente = " & Agro_SQL_SaveText_NULL(Cul_Cod_Ente) & " AND  ")
            Stb.AppendLine("     Occupazione_Cod = " & Agro_SQL_SaveText_NULL(Occupazione_Cod) & " ")


            result = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            result = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return result
    End Function

End Class