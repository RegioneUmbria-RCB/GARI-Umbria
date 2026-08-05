Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Web.UI.WebControls
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

Namespace AgronicaMeteoSuite

    Public Class Metos_R_W
        Inherits AgronicaCoreDataProvider.DataProvider

        Private Property LogDirectory As String = "C:\GIASLAN\LOG"
        Private Property LogErroriFileName As String = "Sincro_Metos.txt"


#Region "StazioniAppoggio"

        Function Metos_Stazioni_Appoggio_Aggiungi(nome As String, descrizione As String, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Stazioni_Appoggio_Aggiungi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder


            If nome.Trim = "" Or descrizione.Trim = "" Then
                Return False
            End If

            Try

                StrSQL.Length = 0

                StrSQL.Append(" INSERT  ")
                StrSQL.Append(" 	into Metos_Stazioni_Appoggio")

                StrSQL.Append("([Nome],[Descrizione]) values ( ")

                StrSQL.Append(" " & Agro_SQL_SaveText_NULL(nome.Trim) & ",  " & Agro_SQL_SaveText_NULL(descrizione.Trim) & " ")

                StrSQL.Append(" )")

                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False

        End Function


        Function Metos_Stazioni_Appoggio_Leggi(nome As String, StringaConnessione As String) As DataTable

            Dim NomeRoutine As String = "Metos_Stazioni_Appoggio_Leggi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try

                StrSQL.Length = 0

                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" FROM Metos_Stazioni_Appoggio ")


                StrSQL.Append(" WHERE   1=1  ")
                If nome <> "" Then
                    StrSQL.Append(" and nome  = " & Agro_SQL_SaveText_NULL(nome) & "  ")
                End If


                StrSQL.Append(" ORDER BY nome")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing

        End Function

        Function Metos_Stazioni_Appoggio_Leggi_Con_LatLong(nome As String, StringaConnessione As String) As DataTable
            Dim NomeRoutine As String = "Metos_Stazioni_Appoggio_Leggi_Con_LatLong"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try

                StrSQL.Length = 0

                StrSQL.Append(" SELECT Metos_Stazioni_Appoggio.*, f_latitude as lat,  f_longitude as long, '' as Distanza ")
                StrSQL.Append(" 	from Metos_Stazioni_Appoggio left join Metos_Stazioni ")
                StrSQL.Append(" on Metos_Stazioni.f_name=Metos_Stazioni_Appoggio.Nome ")

                StrSQL.Append(" WHERE   1=1  ")
                If nome <> "" Then
                    StrSQL.Append(" and nome  = " & Agro_SQL_SaveText_NULL(nome) & "  ")
                End If


                StrSQL.Append(" ORDER BY nome")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing

        End Function




        Function Metos_Stazioni_Appoggio_Leggi_SoloConSensorePioggia_Con_Distanza(nome As String, StringaConnessione As String, Long_Centro As Decimal, Lat_Centro As Decimal) As DataTable
            Dim NomeRoutine As String = "Metos_Stazioni_Appoggio_Leggi_Con_Distanza"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try

                StrSQL.Length = 0

                StrSQL.Append(" select Metos_Stazioni_Appoggio.* , a.distanza, a.[f_longitude],a.[f_latitude]  ")
                StrSQL.Append(" from ")
                StrSQL.Append(" ( ")
                StrSQL.Append(" select *,")
                StrSQL.Append("     geography::STGeomFromText( ")
                StrSQL.Append("     'POINT(' + ")
                StrSQL.Append("         replace([f_longitude], 'E', '') + ' ' +  ")
                StrSQL.Append("         replace([f_latitude], 'N', '')  ")
                StrSQL.Append("     + ')' , 4326 ).STDistance(  geography::STGeomFromText('POINT(" & Agro_SQL_SaveNum(Long_Centro) & " " & Agro_SQL_SaveNum(Lat_Centro) & ")', 4326 ) ) as distanza ")
                StrSQL.Append(" FROM [Metos_Stazioni]     ")
                StrSQL.Append(" inner join  Metos_Sensori on      ")
                StrSQL.Append("  where f_latitude <> ''    ")
                StrSQL.Append("  and f_longitude <> ''   ")
                StrSQL.Append("   ) a right join Metos_Stazioni_Appoggio   ")
                StrSQL.Append(" on a.f_name=Metos_Stazioni_Appoggio.nome     ")
                StrSQL.Append(" WHERE   1=1  ")
                If nome <> "" Then
                    StrSQL.Append(" and nome  = " & Agro_SQL_SaveText_NULL(nome) & "  ")
                End If
                StrSQL.Append(" order by distanza     ")
                StrSQL.Append("      ")




                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing

        End Function


        'Sub stazioniMeteo(
        '    ByRef Controllo As ListControl,
        '    ByVal piva_SuperUser As String,
        '    ByVal PrimaRiga_Flag As Boolean,
        '    ByVal PrimaRiga_Text As String,
        '    ByVal PrimaRiga_Value As String,
        '    ByVal xFiltroAggiuntivo As String,
        '    ByVal xOrderBy As String,
        '    ByRef StringaConnessione As String
        ' )
        '    ' Dim StringaConnessione As String = "Provider=SQLOLEDB;Server=*****;Initial Catalog=AgronicaMeteoSuite;User Id=*****;Password=*****;"
        '    Try

        '        Dim DT As DataTable
        '        Dim i As Integer

        '        Dim Metos_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

        '        'Pulisco il controllo
        '        Controllo.Items.Clear()

        '        If PrimaRiga_Flag = True Then
        '            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        '        End If


        '        DT = Metos_R_W.Agronica_Stazioni_Appoggio_Leggi(piva_SuperUser, "", StringaConnessione)

        '        If DT.Rows.Count > 0 Then
        '            For i = 0 To DT.Rows.Count - 1

        '                Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Descrizione"),
        '                                                 DT.Rows(i).Item("Nome")))

        '            Next
        '        End If

        '    Catch ex As Exception

        '    End Try



        'End Sub

        'Sub stazioniMeteoConDistanza(
        '    ByRef Controllo As ListControl,
        '    Long_Centro As Decimal, Lat_Centro As Decimal,
        '    ByVal piva_SuperUser As String,
        '    ByVal TipoSorgente As enum_Meteo_Tiposorgente,
        '    ByVal PrimaRiga_Flag As Boolean,
        '    ByVal PrimaRiga_Text As String,
        '    ByVal PrimaRiga_Value As String,
        '    ByVal xFiltroAggiuntivo As String,
        '    ByVal xOrderBy As String,
        '    ByRef StringaConnessione As String
        ')
        '    ' Dim StringaConnessione As String = "Provider=SQLOLEDB;Server=*****;Initial Catalog=AgronicaMeteoSuite;User Id=*****;Password=*****;"
        '    Try

        '        Dim DT As DataTable
        '        Dim i As Integer

        '        Dim Metos_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

        '        'Pulisco il controllo
        '        Controllo.Items.Clear()

        '        If PrimaRiga_Flag = True Then
        '            Controllo.Items.Add(New ListItem(PrimaRiga_Text, PrimaRiga_Value))
        '        End If

        '        DT = Metos_R_W.Agronica_Stazioni_Appoggio_Leggi_Con_Distanza(piva_SuperUser, "", StringaConnessione, Long_Centro, Lat_Centro, TipoSorgente)

        '        If DT.Rows.Count > 0 Then

        '            For i = 0 To DT.Rows.Count - 1

        '                If Not IsDBNull(DT.Rows(i).Item("Distanza")) Then
        '                    Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Descrizione") & " (" & CInt(CInt(DT.Rows(i).Item("Distanza")) / 1000) & "km)",
        '                                                 DT.Rows(i).Item("Nome")))
        '                End If

        '            Next



        '            For i = 0 To DT.Rows.Count - 1

        '                If IsDBNull(DT.Rows(i).Item("Distanza")) Then
        '                    Controllo.Items.Add(New ListItem(DT.Rows(i).Item("Descrizione") & "",
        '                                                 DT.Rows(i).Item("Nome")))
        '                End If

        '            Next


        '        End If

        '    Catch ex As Exception

        '    End Try



        'End Sub




#End Region


#Region "Stazioni"

        Function Metos_Stazioni_Leggi_Nomi(StringaConnessione As String) As String()
            Dim NomeRoutine As String = "Metos_Stazioni_Leggi_Nomi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try


                StrSQL.Length = 0

                StrSQL.Append(" SELECT f_station_code , f_name ")
                StrSQL.Append(" 	from Metos_Stazioni ")

                StrSQL.Append(" WHERE   1=1  ")



                StrSQL.Append(" ORDER BY f_name")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------
                If DT.Rows.Count > 0 Then
                    Dim nomi(DT.Rows.Count - 1) As String
                    For i = 0 To DT.Rows.Count - 1
                        nomi(i) = DT.Rows(i).Item("f_name")
                    Next
                    Return nomi
                End If

                Return Nothing
            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing

        End Function


        Function Metos_Stazioni_Station_Code_Fom_Nome(f_name As String, StringaConnessione As String) As Integer
            Dim f_station_code As Integer
            Dim dt As DataTable = Metos_Stazioni_Leggi(f_station_code, f_name, StringaConnessione)
            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("f_station_code")
            End If
            Return Nothing
        End Function


        Function Metos_Stazioni_Leggi(f_station_code As Integer, f_name As String, StringaConnessione As String) As DataTable
            Dim NomeRoutine As String = "Metos_Stazioni_Leggi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try

                StrSQL.Length = 0

                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" 	from Metos_Stazioni ")

                StrSQL.Append(" WHERE   1=1  ")
                If f_station_code <> 0 Then
                    StrSQL.Append(" and   f_station_code = " & Agro_SQL_SaveNum_NULL(f_station_code) & "  ")
                End If
                If f_name <> "" Then
                    StrSQL.Append(" and f_name  = " & Agro_SQL_SaveText_NULL(f_name) & "  ")
                End If


                StrSQL.Append(" ORDER BY f_station_code")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing

        End Function


        Function Metos_Stazioni_Aggiungi(dataRow As DataRow, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Stazioni_Aggiungi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder

            Try

                StrSQL.Length = 0

                StrSQL.Append(" INSERT INTO [dbo].[Metos_Stazioni]")
                StrSQL.Append(" ( ")

                StrSQL.Append("   [f_station_code] ")
                StrSQL.Append(" , [f_date] ")
                StrSQL.Append(" , [f_dev_id] ")
                StrSQL.Append(" , [f_name] ")
                StrSQL.Append(" , [f_descr] ")
                StrSQL.Append(" , [f_info] ")
                StrSQL.Append(" , [f_uid] ")
                StrSQL.Append(" , [f_status] ")
                StrSQL.Append(" , [f_create_time] ")
                StrSQL.Append(" , [f_master_name] ")
                StrSQL.Append(" , [f_date_min] ")
                StrSQL.Append(" , [f_date_max] ")
                StrSQL.Append(" , [f_date_last_down] ")
                StrSQL.Append(" , [f_date_sens] ")
                StrSQL.Append(" , [f_date_data] ")
                StrSQL.Append(" , [f_date_conf] ")
                StrSQL.Append(" , [f_prec_reduction] ")
                StrSQL.Append(" , [f_measure_int] ")
                StrSQL.Append(" , [f_data_int] ")
                StrSQL.Append(" , [f_timezone] ")
                StrSQL.Append(" , [f_latitude] ")
                StrSQL.Append(" , [f_longitude] ")
                StrSQL.Append(" , [f_altitude] ")
                StrSQL.Append(" , [f_hw_ver_major] ")
                StrSQL.Append(" , [f_hw_ver_minor] ")
                StrSQL.Append(" , [f_sms_warn_numbers] ")
                StrSQL.Append(" , [f_sms_warn_values] ")
                StrSQL.Append(" , [f_gsm_mcc] ")
                StrSQL.Append(" , [f_gsm_mnc] ")
                StrSQL.Append(" , [f_gprs_apn] ")
                StrSQL.Append(" , [f_gprs_user_id] ")
                StrSQL.Append(" , [f_gprs_passw] ")
                StrSQL.Append("  ")

                StrSQL.Append(" ) VALUES ( ")

                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_station_code")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_dev_id")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_name")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_descr")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_info")) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_uid")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_status")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_create_time")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_master_name")) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_min")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_max")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_last_down")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_sens")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_data")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date_conf")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_prec_reduction")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_measure_int")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_data_int")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_timezone")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_latitude")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_longitude")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_altitude")) & "  ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_hw_ver_major")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_hw_ver_minor")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sms_warn_numbers")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sms_warn_values")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_gsm_mcc")) & " ")
                StrSQL.Append(" , " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_gsm_mnc")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_gprs_apn")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_gprs_user_id")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_gprs_passw")) & "  ")

                StrSQL.Append(" ) ")


                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False
        End Function


#End Region


#Region "TipoSensore"

        Function Metos_Tipo_Sensore_VerificaAggiungiAggiorna(f_station_name As String, ByVal StringaConnessione As String, ByVal SensoriRow As DataRow) As Boolean
            Dim f_sensor_ch As Integer = SensoriRow.Item("f_sensor_ch")
            Dim f_sensor_code As Integer = SensoriRow.Item("f_sensor_code")
            Dim f_group_code As Integer = SensoriRow.Item("f_group_code")
            Dim f_name As String = SensoriRow.Item("f_name")
            Dim f_unit As String = SensoriRow.Item("f_unit")
            Dim f_unit_html As String = SensoriRow.Item("f_unit_html")

            If Metos_Tipo_Sensore_Esiste(f_group_code, StringaConnessione) Then
                'verifica
                Dim r1 As Boolean = Metos_Tipo_Sensore_VerificaCongruenza(f_group_code, f_name, f_unit, f_unit_html, StringaConnessione)
                Return r1
            Else
                'aggiungo
                Dim r2 As Boolean = Metos_Tipo_Sensore_Aggiungi(f_group_code, f_name, f_unit, f_unit_html, StringaConnessione)
                Return r2
            End If

        End Function

        Function Metos_Tipo_Sensore_Esiste(f_group_code As Integer, StringaConnessione As String) As Boolean
            ' Dim f_group_code As Integer
            Dim f_name As String
            Dim f_unit As String
            Dim dt As DataTable = Metos_Tipo_Sensore_Leggi(f_group_code, f_name, f_unit, "", StringaConnessione)
            If dt.Rows.Count = 0 Then
                Return False
            ElseIf dt.Rows.Count = 1 Then
                Return True
            End If
            Throw New Exception()
        End Function

        Function Metos_Tipo_Sensore_Leggi(f_group_code As Integer, f_name As String, f_unit As String, f_unit_html As String, StringaConnessione As String) As DataTable
            Dim NomeRoutine As String = "Metos_Tipo_Sensore_Leggi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try


                StrSQL.Length = 0

                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" 	from Metos_Tipo_Sensore ")

                StrSQL.Append(" WHERE   1=1  ")

                If f_group_code <> 0 Then
                    StrSQL.Append(" and   f_group_code = " & Agro_SQL_SaveNum_NULL(f_group_code) & "  ")
                End If


                If f_name <> "" Then
                    StrSQL.Append(" and f_name  = " & Agro_SQL_SaveText_NULL(f_name) & "  ")
                End If
                If f_unit <> "" Then
                    StrSQL.Append(" and f_unit  = " & Agro_SQL_SaveText_NULL(f_unit) & "  ")
                End If
                If f_unit_html <> "" Then
                    StrSQL.Append(" and f_unit_html  = " & Agro_SQL_SaveText_NULL(f_unit_html) & "  ")
                End If

                StrSQL.Append(" ORDER BY f_group_code, f_name ")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing
        End Function

        Function Metos_Tipo_Sensore_VerificaCongruenza(f_group_code As Integer, f_name As String, f_unit As String, f_unit_html As String, StringaConnessione As String) As Boolean
            Dim dt As DataTable = Metos_Tipo_Sensore_Leggi(f_group_code, "", "", "", StringaConnessione)
            If dt.Rows.Count = 0 Then
                Return False
            ElseIf dt.Rows.Count = 1 Then

                If f_name.Contains(dt.Rows(0).Item("f_name")) Then
                    Return True
                End If

                Return False
            End If
            Return False
        End Function

        Function Metos_Tipo_Sensore_Aggiungi(f_group_code As Integer, f_name As String, f_unit As String, f_unit_html As String, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Tipo_Sensore_Aggiungi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder

            Try

                StrSQL.Length = 0

                StrSQL.Append(" INSERT INTO [dbo].[Metos_Tipo_Sensore]")
                StrSQL.Append(" ([f_group_code]	 ")
                StrSQL.Append(" ,[f_name]	 ")
                StrSQL.Append(" ,[f_unit]	 ")
                StrSQL.Append(" ,[f_unit_html])	 ")

                StrSQL.Append(" VALUES ( ")

                StrSQL.Append("  " & Agro_SQL_SaveNum_NULL(f_group_code) & "   ")
                StrSQL.Append(" , " & Agro_SQL_SaveText_NULL(f_name) & "   ")
                StrSQL.Append(" , " & Agro_SQL_SaveText_NULL(f_unit) & "   ")
                StrSQL.Append(" , " & Agro_SQL_SaveText_NULL(f_unit_html) & "   ")

                StrSQL.Append(" ) ")


                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False

        End Function


#End Region


#Region "Sensori"

        Function Metos_Sensori_Leggi(f_station_code As Integer, f_station_name As String, f_sensor_ch As Integer, f_sensor_code As Integer, f_group_code As Integer, f_name As String, f_unit As String, StringaConnessione As String) As DataTable
            Dim NomeRoutine As String = "Metos_Sensori_Leggi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try


                StrSQL.Length = 0

                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" 	from Metos_Sensori ")

                StrSQL.Append(" WHERE   1=1  ")

                If f_station_code <> 0 Then
                    StrSQL.Append(" and   f_station_code = " & Agro_SQL_SaveNum_NULL(f_station_code) & "  ")
                End If
                If f_station_name <> "" Then
                    StrSQL.Append(" and   f_station_name = " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")
                End If
                If f_sensor_ch <> 0 Then
                    StrSQL.Append(" and   f_sensor_ch = " & Agro_SQL_SaveNum_NULL(f_sensor_ch) & "  ")
                End If
                If f_sensor_code <> 0 Then
                    StrSQL.Append(" and   f_sensor_code = " & Agro_SQL_SaveNum_NULL(f_sensor_code) & "  ")
                End If
                If f_group_code <> 0 Then
                    StrSQL.Append(" and   f_group_code = " & Agro_SQL_SaveNum_NULL(f_group_code) & "  ")
                End If


                If f_name <> "" Then
                    StrSQL.Append(" and f_name  = " & Agro_SQL_SaveText_NULL(f_name) & "  ")
                End If
                If f_unit <> "" Then
                    StrSQL.Append(" and f_unit  = " & Agro_SQL_SaveText_NULL(f_unit) & "  ")
                End If

                StrSQL.Append(" ORDER BY f_station_code,f_sensor_code ")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing
        End Function

        Function Metos_Sensore_AggiungiAggiorna(dataRow As DataRow, f_station_name As String, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Sensore_AggiungiAggiorna"
            Dim MessaggioErrore As String = ""

            Try

                Dim dts As DataTable = Metos_Sensori_Leggi(dataRow.Item("f_station_code"), f_station_name, dataRow.Item("f_sensor_ch"), 0, 0, "", "", StringaConnessione)

                If dts.Rows.Count > 0 Then
                    Return Metos_Sensore_Aggiorna(dataRow, f_station_name, StringaConnessione)
                Else
                    Return Metos_Sensore_Aggiungi(dataRow, f_station_name, StringaConnessione)
                End If

            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False
        End Function

        Function Metos_Sensore_Aggiungi(dataRow As DataRow, f_station_name As String, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Sensore_Aggiungi"
            Dim MessaggioErrore As String = ""


            Dim StrSQL As New System.Text.StringBuilder

            Try

                StrSQL.Length = 0

                StrSQL.Append(" INSERT INTO [dbo].[Metos_Sensori]")
                StrSQL.Append(" ( ")

                StrSQL.Append("   [f_station_code] ")
                StrSQL.Append(" , [f_station_name] ")
                StrSQL.Append(" , [f_sensor_ch] ")
                StrSQL.Append(" , [f_sensor_code] ")
                StrSQL.Append(" , [f_chain_code] ")
                StrSQL.Append(" , [f_group_code] ")
                StrSQL.Append(" , [f_unit_code] ")
                StrSQL.Append(" , [f_name] ")
                StrSQL.Append(" , [f_unit] ")
                StrSQL.Append(" , [f_div] ")
                StrSQL.Append(" , [f_mul] ")
                StrSQL.Append(" , [f_val_neg] ")
                StrSQL.Append(" , [f_val_log] ")
                StrSQL.Append(" , [f_val_last] ")
                StrSQL.Append(" , [f_val_sum] ")
                StrSQL.Append(" , [f_val_aver] ")
                StrSQL.Append(" , [f_val_min] ")
                StrSQL.Append(" , [f_val_max] ")
                StrSQL.Append(" , [f_val_time] ")
                StrSQL.Append(" , [f_val_user] ")
                StrSQL.Append(" , [f_create_time] ")
                StrSQL.Append(" , [f_val_axilary] ")
                StrSQL.Append(" , [f_user_app] ")
                StrSQL.Append(" , [f_color] ")
                StrSQL.Append(" , [f_sensor_user_name] ")
                StrSQL.Append(" , [f_opacity] ")
                StrSQL.Append(" , [f_user_unit_code] ")
                StrSQL.Append(" , [f_unit_html] ")

                StrSQL.Append("  ")

                StrSQL.Append(" ) VALUES ( ")

                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_station_code")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sensor_ch")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sensor_code")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_chain_code")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_group_code")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_unit_code")) & " ")

                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_name")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_unit")) & "  ")

                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_div")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_mul")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_neg")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_log")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_last")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_sum")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_aver")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_min")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_max")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_time")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_user")) & " ")


                StrSQL.Append(" , " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_create_time")) & " ")

                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_val_axilary")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_user_app")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_color")) & "  ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_sensor_user_name")) & "  ")

                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_opacity")) & " ")
                StrSQL.Append(" ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_user_unit_code")) & " ")

                StrSQL.Append(" ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_unit_html")) & "  ")

                StrSQL.Append(" ) ")


                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False
        End Function

        Function Metos_Sensore_Aggiorna(dataRow As DataRow, f_station_name As String, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Sensore_Aggiungi"
            Dim MessaggioErrore As String = ""


            Dim StrSQL As New System.Text.StringBuilder

            Try

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE [dbo].[Metos_Sensori]")
                StrSQL.Append(" SET ")

                StrSQL.Append("   [f_station_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_station_code")) & " ")

                StrSQL.Append("  , [f_station_name] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")

                StrSQL.Append("  , [f_sensor_ch] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sensor_ch")) & " ")

                StrSQL.Append("  , [f_sensor_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sensor_code")) & " ")

                StrSQL.Append("  , [f_chain_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_chain_code")) & " ")

                StrSQL.Append("  , [f_group_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_group_code")) & " ")

                StrSQL.Append("  , [f_unit_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_unit_code")) & " ")

                StrSQL.Append("  , [f_name] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_name")) & "  ")

                StrSQL.Append("  , [f_unit] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_unit")) & "  ")

                StrSQL.Append("  , [f_div] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_div")) & " ")

                StrSQL.Append("  , [f_mul] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_mul")) & " ")

                StrSQL.Append("  , [f_val_neg] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_neg")) & " ")

                StrSQL.Append("  , [f_val_log] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_log")) & " ")

                StrSQL.Append("  , [f_val_last] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_last")) & " ")

                StrSQL.Append("  , [f_val_sum] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_sum")) & " ")

                StrSQL.Append("  , [f_val_aver] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_aver")) & " ")

                StrSQL.Append("  , [f_val_min] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_min")) & " ")

                StrSQL.Append("  , [f_val_max] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_max")) & " ")

                StrSQL.Append("  , [f_val_time] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_time")) & " ")

                StrSQL.Append("  , [f_val_user] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_val_user")) & " ")

                StrSQL.Append("  , [f_create_time] = ")
                StrSQL.Append("  " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_create_time")) & " ")

                StrSQL.Append("  , [f_val_axilary] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_val_axilary")) & "  ")

                StrSQL.Append("  , [f_user_app] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_user_app")) & "  ")

                StrSQL.Append("  , [f_color] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_color")) & "  ")

                StrSQL.Append("  , [f_sensor_user_name] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_sensor_user_name")) & "  ")

                StrSQL.Append("  , [f_opacity] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_opacity")) & " ")

                StrSQL.Append("  , [f_user_unit_code] = ")
                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_user_unit_code")) & " ")

                StrSQL.Append("  , [f_unit_html] = ")
                StrSQL.Append("   " & Agro_SQL_SaveText_NULL(dataRow.Item("f_unit_html")) & "  ")

                StrSQL.Append(" WHERE   1=1  ")


                StrSQL.Append(" and   f_station_code = " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_station_code")) & "  ")
                StrSQL.Append(" and   f_station_name = " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")
                StrSQL.Append(" and   f_sensor_ch = " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_sensor_ch")) & "  ")



                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False
        End Function

#End Region


#Region "Dati"

        Function Metos_Dati_Rilevati_RinominaColonne_E_AggiungiAggiorna(station_code As Integer, f_station_name As String, Dati As DataTable, StringaConnessione As String) As Boolean

            Dim Dati_Rinominati As DataTable = Metos_Dati_Rilevati_RinominaColonne(station_code, f_station_name, Dati, StringaConnessione)

            For i = 0 To Dati_Rinominati.Rows.Count - 1
                Dim r4 As Boolean = Metos_Dati_Rilevati_AggiungiAggiorna(Dati_Rinominati.Rows(i), StringaConnessione)
            Next
            Return True

        End Function

        Function Metos_Dati_Rilevati_RinominaColonne(station_code As Integer, f_station_name As String, Dati As DataTable, StringaConnessione As String) As DataTable

            If Dati.Rows.Count > 0 Then

                Dim nomecolonna As String
                For i = 0 To Dati.Columns.Count - 1
                    nomecolonna = Dati.Columns(i).ColumnName
                    Dim nuovoNumeColonna As String = ""
                    Dim nomicolonna As String() = nomecolonna.Split("_")
                    If nomicolonna(0) = "sens" Then
                        Dim f_sensor_ch As String = nomicolonna(2)
                        Dim f_sensor_code As String = nomicolonna(3)


                        Dim dtSensori As DataTable = Metos_Sensori_Leggi(0, f_station_name, f_sensor_ch, f_sensor_code, 0, "", "", StringaConnessione)

                        'ci sono dei sensori con codice stazione diversa
                        If dtSensori.Rows.Count > 0 Then


                            Dim dtTipoSensori As DataTable = Metos_Tipo_Sensore_Leggi(dtSensori.Rows(0).Item("f_group_code"), "", "", "", StringaConnessione)

                            Dim f_group_code As Integer = dtSensori.Rows(0).Item("f_group_code")
                            Dim f_name As String = dtSensori.Rows(0).Item("f_name")

                            If dtTipoSensori.Rows.Count > 0 Then
                                Dim Mappatura As String = dtTipoSensori.Rows(0).Item("Mappatura")
                                If Mappatura <> "nessuna" Then
                                    nuovoNumeColonna = Mappatura & "_" & nomicolonna(1)
                                    If IsNothing(Dati.Columns.Item(nuovoNumeColonna)) Then
                                        Dati.Columns(i).ColumnName = nuovoNumeColonna
                                    Else
                                        'LogErr("Saltata colonna  " & nuovoNumeColonna & " perchè esistente")
                                    End If
                                Else
                                    ' LogErr("Il sensore  f_sensor_ch " & f_sensor_ch & "  f_sensor_code " & f_sensor_code & " di gruppo " & f_group_code & " nella la stazione " & f_station_name & "  non è volutamente mappato")
                                End If


                            Else

                                LogErr("Il sensore  f_sensor_ch " & f_sensor_ch & "  f_sensor_code " & f_sensor_code & " di gruppo " & f_group_code & " nella la stazione " & f_station_name & "  non è mappato in Metos_Tipo_Sensore")

                            End If



                        Else

                            LogErr("Il sensore  f_sensor_ch " & f_sensor_ch & "  f_sensor_code " & f_sensor_code & "  non è regitrato per la stazione " & f_station_name & "  ")

                        End If

                        ''Try
                        ''    Dati.Columns(i).ColumnName = nuovoNumeColonna
                        ''Catch ex As Exception
                        ''    Dim a As String = "ff"
                        ''End Try
                        'End If



                    End If


                Next

                Dim Dt_DatiRilevati As DataTable

                Dt_DatiRilevati = New DataTable
                Dt_DatiRilevati.Columns.Add(New DataColumn("f_station_code", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("f_station_Name", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("f_date", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("f_log_int", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Solar_Radiation_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Relative_Humidity_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Air_Temperature_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Air_Temperature_Min", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Air_Temperature_Max", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Precipitation_Sum", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Wind_Speed_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Wind_Speed_Max", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Battery_Voltage_Last", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Leaf_Wetness_Time", GetType(String)))
                'Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_1", GetType(String)))
                'Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_2", GetType(String)))
                'Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_3", GetType(String)))
                'Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_4", GetType(String)))
                'Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_5", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Water_Mark_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Dew_Point_Aver", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Dew_Point_Min", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Soil_temperature", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Wetbulb_Temperature", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Soil_Moisture", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("MPS_1", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Solar_Panel", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("ETo", GetType(String)))
                Dt_DatiRilevati.Columns.Add(New DataColumn("Evaporation_Pan", GetType(String)))
                Dim Dr As DataRow

                For j = 0 To Dati.Rows.Count - 1
                    Dr = Dt_DatiRilevati.NewRow()

                    Dr.Item("f_station_code") = station_code
                    Dr.Item("f_station_Name") = f_station_name

                    For i = 0 To Dati.Columns.Count - 1


                        If Not IsNothing(Dt_DatiRilevati.Columns.Item(Dati.Columns(i).ColumnName)) Then

                            Dr.Item(Dati.Columns(i).ColumnName) =
                                  Dati.Rows(j).Item(Dati.Columns(i).ColumnName)

                        Else
                            'LogErr("Saltata colonna  " & Dati.Columns(i).ColumnName & " con il dato " & Dati.Rows(j).Item(Dati.Columns(i).ColumnName) & " perchè non mappata")
                        End If



                    Next

                    Dt_DatiRilevati.Rows.Add(Dr)

                Next

                Return Dt_DatiRilevati
            End If

            Return New DataTable

        End Function

        Function Metos_Dati_Rilevati_AggiungiAggiorna(DataRow As DataRow, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Dati_Rilevati_AggiungiAggiorna"
            Dim MessaggioErrore As String = ""



            Try

                Dim dts As DataTable = Metos_Dati_Rilevati_Leggi(DataRow.Item("f_station_code"), DataRow.Item("f_station_name"), DataRow.Item("f_date"), StringaConnessione)

                If Not IsNothing(dts) AndAlso dts.Rows.Count > 0 Then
                    Return Metos_Dati_Rilevati_Aggiorna(DataRow, StringaConnessione)
                Else
                    Return Metos_Dati_Rilevati_Aggiungi(DataRow, StringaConnessione)
                End If

            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False
        End Function

        Function Metos_Dati_Rilevati_Leggi(f_station_code As Integer, f_station_name As String, f_date As String, StringaConnessione As String) As DataTable
            Dim NomeRoutine As String = "Metos_Dati_Rilevati_Leggi"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable

            Try


                StrSQL.Length = 0

                StrSQL.Append(" SELECT * ")
                StrSQL.Append(" 	from Metos_Dati_Rilevati ")

                StrSQL.Append(" WHERE   1=1  ")

                If f_station_code <> 0 Then
                    StrSQL.Append(" and   f_station_code = " & Agro_SQL_SaveNum_NULL(f_station_code) & "  ")
                End If
                If f_station_name <> "" Then
                    StrSQL.Append(" and   f_station_name = " & Agro_SQL_SaveText_NULL(f_station_name) & "  ")
                End If
                If f_date <> "" AndAlso IsDate(f_date) Then
                    StrSQL.Append(" and   f_date = " & Agro_SQL_SaveDateTime_NULL(CDate(f_date)) & "  ")
                End If

                StrSQL.Append(" ORDER BY f_date ")

                '-----------------------------------------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------

                Return DT

            Catch ex As Exception

                DT = Nothing
                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing
        End Function

        Function Metos_Dati_Rilevati_Aggiungi(dataRow As DataRow, StringaConnessione As String) As Boolean

            Dim NomeRoutine As String = "Metos_Dati_Rilevati_Aggiungi"
            Dim MessaggioErrore As String = ""


            Dim StrSQL As New System.Text.StringBuilder

            Try

                StrSQL.Length = 0

                StrSQL.Append(" INSERT INTO [dbo].[Metos_Dati_Rilevati]")
                StrSQL.Append(" ( ")

                StrSQL.Append("   [f_station_code] ")
                StrSQL.Append(" , [f_station_name] ")
                StrSQL.Append(" , [f_date] ")
                StrSQL.Append(" , [f_log_int] ")
                StrSQL.Append(" , [Solar_Radiation_Aver] ")
                StrSQL.Append(" , [Relative_Humidity_Aver] ")
                StrSQL.Append(" , [Air_Temperature_Aver] ")
                StrSQL.Append(" , [Air_Temperature_Min] ")
                StrSQL.Append(" , [Air_Temperature_Max] ")
                StrSQL.Append(" , [Precipitation_Sum] ")
                StrSQL.Append(" , [Wind_Speed_Aver] ")
                StrSQL.Append(" , [Wind_Speed_Max] ")
                StrSQL.Append(" , [Battery_Voltage_Last] ")
                StrSQL.Append(" , [Leaf_Wetness_Time] ")
                'StrSQL.Append(" , [Water_Mark_1] ")
                'StrSQL.Append(" , [Water_Mark_2] ")
                'StrSQL.Append(" , [Water_Mark_3] ")
                'StrSQL.Append(" , [Water_Mark_4] ")
                'StrSQL.Append(" , [Water_Mark_5] ")
                StrSQL.Append(" , [Water_Mark_Aver] ")
                StrSQL.Append(" , [Dew_Point_Aver] ")
                StrSQL.Append(" , [Dew_Point_Min] ")
                StrSQL.Append(" , [Soil_temperature] ")
                StrSQL.Append(" , [Wetbulb_Temperature] ")
                StrSQL.Append(" , [Soil_Moisture] ")
                StrSQL.Append(" , [MPS_1] ")
                StrSQL.Append(" , [Solar_Panel] ")
                StrSQL.Append(" , [ETo] ")
                StrSQL.Append(" , [Evaporation_Pan] ")

                StrSQL.Append("  ")

                StrSQL.Append(" ) VALUES ( ")

                StrSQL.Append("   " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_station_code")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveText_NULL(dataRow.Item("f_station_name")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("f_log_int")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Solar_Radiation_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Relative_Humidity_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Air_Temperature_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Air_Temperature_Min")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Air_Temperature_Max")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Precipitation_Sum")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Wind_Speed_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Wind_Speed_Max")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Battery_Voltage_Last")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Leaf_Wetness_Time")) & " ")
                'StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_1")) & " ")
                'StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_2")) & " ")
                'StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_3")) & " ")
                'StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_4")) & " ")
                'StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_5")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Water_Mark_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Dew_Point_Aver")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Dew_Point_Min")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Soil_temperature")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Wetbulb_Temperature")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Soil_Moisture")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("MPS_1")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Solar_Panel")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("ETo")) & " ")
                StrSQL.Append("  ,  " & Agro_SQL_SaveNum_NULL(dataRow.Item("Evaporation_Pan")) & " ")

                StrSQL.Append(" ) ")


                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return False

        End Function

        Function Metos_Dati_Rilevati_Elimina(dataRow As DataRow, StringaConnessione As String) As Boolean
            Dim NomeRoutine As String = "Metos_Dati_Rilevati_Elimina"
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder

            Try


                StrSQL.Length = 0

                StrSQL.Append(" DELETE  ")
                StrSQL.Append(" 	from Metos_Dati_Rilevati ")

                StrSQL.Append(" WHERE   1=1  ")

                StrSQL.Append("  AND f_station_name= " & Agro_SQL_SaveText_NULL(dataRow.Item("f_station_name")) & " ")
                StrSQL.Append("  AND f_date= " & Agro_SQL_SaveDateTime_NULL(dataRow.Item("f_date")) & " ")



                '-----------------------------------------------------------------------------------------------------------
                Return EseguiQuery_Scrittura(StringaConnessione, StrSQL.ToString, NomeRoutine)
                '-----------------------------------------------------------------------------------------------------------



            Catch ex As Exception

                MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
                Throw New Exception(MessaggioErrore)

            End Try

            Return Nothing
        End Function


        Function Metos_Dati_Rilevati_Aggiorna(DataRow As DataRow, StringaConnessione As String) As Boolean

            If Metos_Dati_Rilevati_Elimina(DataRow, StringaConnessione) Then
                Return Metos_Dati_Rilevati_Aggiungi(DataRow, StringaConnessione)
            Else
                Return False
            End If

        End Function


#End Region





        Public Sub LogErr(Msg As String)

            'If LogDirectory <> "" AndAlso LogErroriFileName <> "" Then

            '    Dim objLog As New AgronicaCoreDataProvider.LogProvider

            '    objLog.Scrivi_LOG(LogDirectory,
            '               LogErroriFileName,
            '                "",
            '               "",
            '              Msg)

            'End If
        End Sub






    End Class

End Namespace


