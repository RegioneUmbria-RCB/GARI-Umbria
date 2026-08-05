Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

<CachedDataProviderAttribute("Codifica_SpecieVegetali_Agea_2015_2020_R")>
Public Class Codifica_SpecieVegetali_Agea_2015_2020_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider

    <Cacheable(True)>
    Public Function leggi(ByRef objParametri_Server As AgronicaCoreParametri,
                          Veg_cod_Agea As String,
                          Cul_Cod_Agea As String,
                          Uso_Cod As String,
                          Macrouso_Cod As String,
                          Occupazione_Cod As String,
                          Destinazione_Cod As String,
                          Qualita_Cod As String,
                          Veg_Cod As Integer,
                          Cul_Cod As Integer,
                          Grfi_Cod As Integer,
                          Grva_Cod As Integer,
                          Metodo_Produzione_Cod As Integer,
                          Reg_Cod As Integer,
                          Id_Cod As Integer,
                          Grsp_Cod As Integer) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.AppendLine("Select * ")
            Stb.AppendLine(" From [dbo].[Codifica_SpecieVegetali_Agea_2015_2020] ")
            Stb.AppendLine(" Where 1 = 1 And ")
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
            If Veg_Cod <> 0 Then
                Stb.AppendLine(" Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " AND ")
            End If
            If Cul_Cod <> 0 Then
                Stb.AppendLine(" Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " AND ")
            End If
            If Grfi_Cod <> 0 Then
                Stb.AppendLine(" Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " AND ")
            End If
            If Grva_Cod <> 0 Then
                Stb.AppendLine(" Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & " AND ")
            End If
            If Metodo_Produzione_Cod <> 0 Then
                Stb.AppendLine(" Metodo_Produzione_Cod = " & Agro_SQL_SaveNum(Metodo_Produzione_Cod) & " AND ")
            End If
            If Reg_Cod <> 0 Then
                Stb.AppendLine(" Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & " AND ")
            End If
            If Id_Cod <> 0 Then
                Stb.AppendLine(" Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " AND ")
            End If
            If Grsp_Cod <> 0 Then
                Stb.AppendLine(" Grsp_Cod = " & Agro_SQL_SaveNum(Grsp_Cod) & " AND ")
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

    Public Function Specie_e_Varieta_Gias_Da_AGEA(ByRef LogCodificheMancantiSpecie As String,
                                              ByRef LogCodificheMancantiVarieta As String,
                                                ByVal Veg_Cod_Agea As String,
                                                ByVal Cul_Cod_Agea As String,
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
                                                ByRef objParametri As AgronicaCoreParametri,
                                                ByRef Specie_Des_AgeaInput As String,
                                                ByRef Varieta_Des_AgeaInput As String,
                                                Optional ByVal FilePrefix As String = ""
                                                  ) As Boolean



        Dim NomeRoutine As String = "Specie_e_Varieta_Gias_Da_AGEA"

        Dim LogCodificheMancantiSpecie_Excel As String = ""
        Dim LogCodificheMancantiVarieta_Excel As String = ""
        Dim LogCodificheMancantiSpecie_Excel_Completo As String = ""
        Dim DtCodifica As DataTable
        Dim DrVar As DataRow()
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
        Dim objMeta As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        DtCodifica = leggi(objParametri, Veg_Cod_Agea, "", Uso_Cod_Agea, "", Occupazione_Cod_Agea, Destinazione_Cod_Agea, Qualita_Cod_Agea, 0, 0, 0, 0, 0, 0, 0, 0)

        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count = 0 Then
            DtCodifica = leggi(objParametri, Veg_Cod_Agea, "", "", "", Occupazione_Cod_Agea, Destinazione_Cod_Agea, Qualita_Cod_Agea, 0, 0, 0, 0, 0, 0, 0, 0)

            If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count = 0 Then
                DtCodifica = leggi(objParametri, Veg_Cod_Agea, "", "", "", Occupazione_Cod_Agea, Destinazione_Cod_Agea, "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count = 0 Then
                DtCodifica = leggi(objParametri, Veg_Cod_Agea, "", "", "", Occupazione_Cod_Agea, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If Veg_Cod_Agea <> "" Then
                DtCodifica = leggi(objParametri, Veg_Cod_Agea, "", "", "", "", "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

            If Occupazione_Cod_Agea <> "" Then
                DtCodifica = leggi(objParametri, "", "", "", "", Occupazione_Cod_Agea, "", "", 0, 0, 0, 0, 0, 0, 0, 0)
            End If

        End If

        Dim prefixFile As String = ""
        If Not String.IsNullOrEmpty(FilePrefix) Then
            prefixFile = $"{FilePrefix}_"
        End If

        Dim customLOGParamsCsv As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"{prefixFile}CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti_Excel.csv"
                    }

        Dim customLOGParamsTxt As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"{prefixFile}CodificaSpecieVegetaliAGEA_GIAS_VARIETA_mancanti.txt"
                    }

        Dim customLOGParamsCsvCompleto As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri.UtenteUsername,
                        .LogDirectory = objParametri.LogDirectory,
                        .LogFileName = $"{prefixFile}CodificaSpecieVegetaliAGEA_GIAS_SPECIE_mancanti_Excel_Completo.csv"
                    }


        If Not IsNothing(DtCodifica) AndAlso DtCodifica.Rows.Count > 0 Then

            DrVar = DtCodifica.Select("Cul_Cod_Agea='" & Cul_Cod_Agea.ToString & "'")

            If DrVar IsNot Nothing AndAlso DrVar.Length > 0 Then

                Veg_Cod = DrVar(0).Item("Veg_Cod")
                Cul_Cod = DrVar(0).Item("Cul_Cod")
                Grfi_Cod = DrVar(0).Item("Grfi_Cod")
                Id_Cod = DrVar(0).Item("Id_Cod")
                Grva_Cod = DrVar(0).Item("Grva_Cod")

                Veg_Cod_Agea = DrVar(0).Item("Veg_Cod_Agea")
                Cul_Cod_Agea = DrVar(0).Item("Cul_Cod_Agea")
                Uso_Cod_Agea = DrVar(0).Item("Uso_Cod")
                Occupazione_Cod_Agea = DrVar(0).Item("Occupazione_Cod")
                Destinazione_Cod_Agea = DrVar(0).Item("Destinazione_Cod")
                Qualita_Cod_Agea = DrVar(0).Item("Qualita_Cod")

                If Not IsDBNull(DrVar(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DrVar(0).Item("Cul_Des_Agea")
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
                    LogCodificheMancantiVarieta &= "Specie: " & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Agrea=" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & " Occupazione_Cod=" & Occupazione_Cod_Agea &
                    " - la varietà " & DtCodifica.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Agrea=" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & Occupazione_Cod_Agea & ";" & ";" & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ";" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtCodifica.Rows(0).Item("Cul_Des_Agea") & ";" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA")
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & Occupazione_Cod_Agea & ";" & ";" & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ";" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtCodifica.Rows(0).Item("Cul_Des_Agea") & ";" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA")


                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)

                    Esito = False
                End If

                Dim cul_des_DB = objMeta.CulDes_from_CulCod(Cul_Cod, objParametri)

                If Cul_Cod <> 0 AndAlso Cul_Cod_Agea <> "" AndAlso Cul_Cod_Agea <> "000" AndAlso Cul_Cod_Agea <> "999" AndAlso (cul_des_DB = "Altre" OrElse cul_des_DB = "altre") Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agea=" & CStr(Veg_Cod_Agea) & " Occupazione_Cod=" & Occupazione_Cod_Agea &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agea=" & CStr(Cul_Cod_Agea) & " è mappata con la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & Occupazione_Cod_Agea & ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Agea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Agea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & Occupazione_Cod_Agea & ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Agea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Agea)

                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)

                End If

            Else

                Veg_Cod = DtCodifica.Rows(0).Item("Veg_Cod")
                If Veg_Cod = 0 Then
                    Cul_Cod = 0
                Else
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod,
                                                       objParametri)
                End If

                If Cul_Cod_Agea <> "" AndAlso Cul_Cod_Agea <> "000" AndAlso Cul_Cod_Agea <> "999" Then
                    LogCodificheMancantiVarieta &= "Specie: " & Specie_Des_AgeaInput & ", Veg_Cod_Agrea=" & CStr(Veg_Cod_Agea) & " Occupazione_Cod=" & Occupazione_Cod_Agea &
                        " - la varietà " & Varieta_Des_AgeaInput & " con Cul_Cod_Agrea=" & CStr(Cul_Cod_Agea) & " non è mappata -> è stata selezionata la varietà Altre." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & Occupazione_Cod_Agea & ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Agea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Agea)
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & Occupazione_Cod_Agea & ";" & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Agea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Agea)

                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    'Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)
                End If

                Grfi_Cod = DtCodifica.Rows(0).Item("Grfi_Cod")
                Id_Cod = DtCodifica.Rows(0).Item("Id_Cod")
                Grva_Cod = DtCodifica.Rows(0).Item("Grva_Cod")
                Specie_Des_Agea = DtCodifica.Rows(0).Item("Veg_Des_Agea")

                Veg_Cod_Agea = DtCodifica.Rows(0).Item("Veg_Cod_Agea")
                Cul_Cod_Agea = DtCodifica.Rows(0).Item("Cul_Cod_Agea")
                Uso_Cod_Agea = DtCodifica.Rows(0).Item("Uso_Cod")
                Occupazione_Cod_Agea = DtCodifica.Rows(0).Item("Occupazione_Cod")
                Destinazione_Cod_Agea = DtCodifica.Rows(0).Item("Destinazione_Cod")
                Qualita_Cod_Agea = DtCodifica.Rows(0).Item("Qualita_Cod")

                If Not IsDBNull(DtCodifica.Rows(0).Item("Cul_Des_Agea")) Then
                    Varieta_Des_Agea = DtCodifica.Rows(0).Item("Cul_Des_Agea")
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
                    LogCodificheMancantiVarieta &= "Specie: " & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ", Veg_Cod_Agrea=" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & " Occupazione_Cod=" & Occupazione_Cod_Agea &
                        " - la varietà " & DtCodifica.Rows(0).Item("Cul_Des_Agea") & " con Cul_Cod_Agrea=" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA") & " non è mappata con GIAS." & vbCrLf & vbCrLf
                    LogCodificheMancantiVarieta_Excel &= ";" & Occupazione_Cod_Agea & ";" & ";" & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ";" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtCodifica.Rows(0).Item("Cul_Des_Agea") & ";" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA")
                    LogCodificheMancantiSpecie_Excel_Completo &= ";" & Occupazione_Cod_Agea & ";" & ";" & DtCodifica.Rows(0).Item("Veg_Des_AGEA") & ";" & DtCodifica.Rows(0).Item("Veg_Cod_AGEA") & ";" & DtCodifica.Rows(0).Item("Cul_Des_Agea") & ";" & DtCodifica.Rows(0).Item("Cul_Cod_AGEA")

                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta_Excel, CustomLOGParams:=customLOGParamsCsv)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiVarieta, CustomLOGParams:=customLOGParamsTxt)
                    Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)

                    Esito = False
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
            LogCodificheMancantiSpecie &= "Nessun record presente per il Veg_Cod_Agrea=" & CStr(Veg_Cod_Agea) & "  " & Specie_Des_AgeaInput & " Occupazione_Cod=" & Occupazione_Cod_Agea & vbCrLf & vbCrLf
            LogCodificheMancantiSpecie_Excel &= vbTab & vbTab & Specie_Des_AgeaInput & vbTab & CStr(Veg_Cod_Agea)
            LogCodificheMancantiSpecie_Excel_Completo &= ";" & Occupazione_Cod_Agea & ";" & Specie_Des_AgeaInput & ";" & CStr(Veg_Cod_Agea) & ";" & Varieta_Des_AgeaInput & ";" & CStr(Cul_Cod_Agea)

            customLOGParamsTxt.LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti.txt"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie, CustomLOGParams:=customLOGParamsTxt)

            customLOGParamsCsv.LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti_Excel.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel, CustomLOGParams:=customLOGParamsCsv)

            customLOGParamsCsvCompleto.LogFileName = "CodificaSpecieVegetaliAgea_SPECIE_mancanti_Excel_Completo.csv"
            Scrivi_LOG(objParametri, NomeRoutine, LogCodificheMancantiSpecie_Excel_Completo, CustomLOGParams:=customLOGParamsCsvCompleto)

        End If

        Return Esito
    End Function

    Public Function DecodificaSpecieVegetaleTramiteDescrizione(ByVal SpecieVegatale_Des As String,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByVal xOrderBy As String,
                                                               ByRef objParametri_Server As AgronicaCoreParametri
                                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R.DecodificaSpecieVegetaleTramiteDescrizione()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.AppendLine(" select  ")
            Stb.AppendLine("         distinct Occupazione_Cod,  ")
            Stb.AppendLine("         Occupazione_Des, ")
            Stb.AppendLine("         veg.veg_cod,  ")
            Stb.AppendLine("         veg.Veg_Des, ")
            Stb.AppendLine("         c.cul_cod, ")
            Stb.AppendLine("         c.cul_des ")
            Stb.AppendLine("  from [dbo].[Codifica_SpecieVegetali_Agea_2015_2020] agea ")
            Stb.AppendLine("       inner join SpecieVegetali veg ")
            Stb.AppendLine("         on veg.Veg_Cod = agea.Veg_cod ")
            Stb.AppendLine("       inner join Cultivar c ")
            Stb.AppendLine("         on c.Veg_Cod = veg.Veg_Cod ")
            Stb.AppendLine("         and c.Cul_Des = 'altre' ")
            Stb.AppendLine(" where 1=1 ")
            Stb.AppendLine("  and Occupazione_Cod<>'000' ")

            If SpecieVegatale_Des <> "" Then
                Stb.AppendLine(" and veg.Veg_Des like '" & Agro_SQL_SaveText(SpecieVegatale_Des) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

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

End Class

Public Class Codifica_SpecieVegetali_Agea_2015_2020_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
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
                           Veg_cod As Integer?,
                           Cul_Cod As Integer?,
                           Grfi_cod As Integer?,
                           Grva_Cod As Integer?,
                           Metodo_Produzione_Cod As Integer?,
                           Reg_cod As Integer?,
                           Id_Cod As Integer?,
                           Grsp_Cod As Integer?,
                           inviato As Integer?,
                           datainvio As DateTime,
                           Data_Creazione As DateTime,
                           Data_Modifica As DateTime,
                           Username_Creazione As String,
                           Username_Modifica As String,
                           Validita_Inizio As Date,
                           Validita_Fine As Date
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim result As Boolean

        Try
            Stb.AppendLine("INSERT INTO [dbo].[Codifica_SpecieVegetali_Agea_2015_2020] ")
            Stb.AppendLine("            ([Veg_cod_Agea] ")
            Stb.AppendLine("            ,[Cul_Cod_Agea] ")
            Stb.AppendLine("            ,[Veg_Des_Agea] ")
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
            Stb.AppendLine("            ,[Veg_cod] ")
            Stb.AppendLine("            ,[Cul_Cod] ")
            Stb.AppendLine("            ,[Grfi_Cod] ")
            Stb.AppendLine("            ,[Grva_Cod] ")
            Stb.AppendLine("            ,[Metodo_Produzione_Cod] ")
            Stb.AppendLine("            ,[Reg_Cod] ")
            Stb.AppendLine("            ,[Id_Cod] ")
            Stb.AppendLine("            ,[Grsp_Cod] ")
            Stb.AppendLine("            ,[inviato] ")
            Stb.AppendLine("            ,[datainvio] ")
            Stb.AppendLine("            ,[Data_Creazione] ")
            Stb.AppendLine("            ,[Data_Modifica] ")
            Stb.AppendLine("            ,[Username_Creazione] ")
            Stb.AppendLine("            ,[Username_Modifica] ")
            Stb.AppendLine("            ,[Validita_Inizio] ")
            Stb.AppendLine("            ,[Validita_Fine]) ")
            Stb.AppendLine("      VALUES ")
            Stb.AppendLine("            (" & Agro_SQL_SaveText_NULL(Veg_Cod_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Cul_Cod_Agea) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Veg_des_Agea) & " ")
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
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Veg_cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Cul_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Grfi_cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Grva_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Metodo_Produzione_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Reg_cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Id_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(Grsp_Cod) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveNum_NULL(inviato) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime_NULL(datainvio) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime_NULL(Data_Creazione) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime_NULL(Data_Modifica) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Username_Creazione) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveText_NULL(Username_Modifica) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime_NULL(Validita_Inizio) & " ")
            Stb.AppendLine("            ," & Agro_SQL_SaveDateTime_NULL(Validita_Fine) & " )")


            result = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            result = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result

    End Function

    Public Function AggiornaParametriGias(ByRef objParametri_Server As AgronicaCoreParametri,
                                          Veg_Cod_Agea As String,
                                          Cul_Cod_Agea As String,
                                           Uso_Cod As String,
                                           Macrouso_Cod As String,
                                           Occupazione_Cod As String,
                                           Destinazione_Cod As String,
                                           Qualita_Cod As String,
                                           Veg_cod As Integer,
                                           Cul_Cod As Integer,
                                           Grfi_cod As Integer,
                                           Grva_Cod As Integer,
                                           Metodo_Produzione_Cod As Integer,
                                           Reg_cod As Integer,
                                           Id_Cod As Integer,
                                           Grsp_Cod As Integer,
                                           Data_Modifica As DateTime,
                                           Username_Modifica As String,
                                           Validita_Inizio As Date,
                                           Validita_Fine As Date
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim result As Boolean

        Try

            Stb.AppendLine("UPDATE [dbo].[Codifica_SpecieVegetali_Agea_2015_2020] ")
            Stb.AppendLine(" SET ")
            Stb.AppendLine("    Veg_cod = " & Agro_SQL_SaveNum(Veg_cod) & ", ")
            Stb.AppendLine("    Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & ", ")
            Stb.AppendLine("    Grfi_cod = " & Agro_SQL_SaveNum(Grfi_cod) & ", ")
            Stb.AppendLine("    Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & ", ")
            Stb.AppendLine("    Metodo_Produzione_Cod = " & Agro_SQL_SaveNum(Metodo_Produzione_Cod) & ", ")
            Stb.AppendLine("    Reg_cod = " & Agro_SQL_SaveNum(Reg_cod) & ", ")
            Stb.AppendLine("    Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & ", ")
            Stb.AppendLine("    Grsp_Cod = " & Agro_SQL_SaveNum(Grsp_Cod) & ", ")
            Stb.AppendLine("    Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & ", ")
            Stb.AppendLine("    Username_Modifica = " & Agro_SQL_SaveText_NULL(Username_Modifica) & ", ")
            Stb.AppendLine("    Validita_Inizio = " & Agro_SQL_SaveDateTime(Validita_Inizio) & ", ")
            Stb.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDateTime(Validita_Fine) & " ")
            Stb.AppendLine("WHERE ")
            Stb.AppendLine("    1=1 And ")

            If Veg_Cod_Agea <> "" Then
                Stb.AppendLine("    Veg_Cod_Agea = " & Agro_SQL_SaveText(Veg_Cod_Agea) & " AND ")
            End If

            If Cul_Cod_Agea <> "" Then
                Stb.AppendLine("    Cul_Cod_Agea = " & Agro_SQL_SaveText(Cul_Cod_Agea) & " AND ")
            End If

            If Uso_Cod <> "" Then
                Stb.AppendLine("    Uso_Cod = " & Agro_SQL_SaveText(Uso_Cod) & " AND ")
            End If

            If Macrouso_Cod <> "" Then
                Stb.AppendLine("    Macrouso_Cod = " & Agro_SQL_SaveText(Macrouso_Cod) & " AND ")
            End If

            If Occupazione_Cod <> "" Then
                Stb.AppendLine("    Occupazione_Cod = " & Agro_SQL_SaveText(Occupazione_Cod) & " AND ")
            End If

            If Destinazione_Cod <> "" Then
                Stb.AppendLine("    Destinazione_Cod = " & Agro_SQL_SaveText(Destinazione_Cod) & " AND ")
            End If

            If Qualita_Cod <> "" Then
                Stb.AppendLine("    Qualita_Cod = " & Agro_SQL_SaveText(Qualita_Cod) & " AND ")
            End If

            Stb.AppendLine("    1 = 1")


            result = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, nomeRoutine)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            result = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result

    End Function

    Public Function AggiornaMacrousiUMA(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Cul_Cod_Agea As String,
                                        Uso_Cod As String,
                                        Macrouso_Cod As String,
                                        Occupazione_Cod As String,
                                        Destinazione_Cod As String,
                                        Qualita_Cod As String,
                                        Macrouso_UMA_Cod As String,
                                        Macrouso_UMA_Des As String,
                                        Optional DataModifica As DateTime? = Nothing
        ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W.AggiornaMacrousiUMA()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim result As Boolean

        Try

            Stb.AppendLine("UPDATE [dbo].[Codifica_SpecieVegetali_Agea_2015_2020] ")
            Stb.AppendLine("SET Macrouso_UMA_Cod = '" + Agro_SQL_SaveText(Macrouso_UMA_Cod) + "', ")
            Stb.AppendLine("    Macrouso_UMA_Des = '" + Agro_SQL_SaveText(Macrouso_UMA_Des) + IIf(DataModifica.HasValue, "', ", "'"))
            If (DataModifica.HasValue) Then
                Stb.AppendLine("    Data_Modifica = " + Agro_SQL_SaveDateTime(DataModifica.Value))
            End If
            Stb.AppendLine("WHERE 1 = 1 ")

            If Cul_Cod_Agea <> "" Then
                Stb.AppendLine("AND Cul_Cod_Agea = '" + Agro_SQL_SaveText(Cul_Cod_Agea) + "'")
            End If

            If Uso_Cod <> "" Then
                Stb.AppendLine("AND Uso_Cod = '" + Agro_SQL_SaveText(Uso_Cod) + "'")
            End If

            If Macrouso_Cod <> "" Then
                Stb.AppendLine("AND Macrouso_Cod = '" + Agro_SQL_SaveText(Macrouso_Cod) + "'")
            End If

            If Occupazione_Cod <> "" Then
                Stb.AppendLine("AND Occupazione_Cod = '" + Agro_SQL_SaveText(Occupazione_Cod) + "'")
            End If

            If Destinazione_Cod <> "" Then
                Stb.AppendLine("AND Destinazione_Cod = '" + Agro_SQL_SaveText(Destinazione_Cod) + "'")
            End If

            If Qualita_Cod <> "" Then
                Stb.AppendLine("AND Qualita_Cod = '" + Agro_SQL_SaveText(Qualita_Cod) + "'")
            End If

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