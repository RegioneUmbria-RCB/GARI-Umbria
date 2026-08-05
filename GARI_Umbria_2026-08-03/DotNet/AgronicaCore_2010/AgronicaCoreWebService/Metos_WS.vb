Imports System.ServiceModel
Imports AgronicaCoreWebService.WS_Metos

Public Class Metos_WS

    'http://www.fieldclimate.com/soap_manual/soap_manual.htm
    'http://www.fieldclimate.com/index_new.php
    'http://www.metos.at/pikernel/cidiwsdl.php



    'notee
    '<binding name="iMetosSOAPBinding" />
    '
    '       <endpoint address="http://www.metos.at/pikernel/cidiwsdl.php?debug=1"
    '    binding="basicHttpBinding" bindingConfiguration="iMetosSOAPBinding"
    '    contract="WS_Metos.iMetosSOAPPortType" name="iMetosSOAPPort" />

    ' xx()
    '        <endpoint address="http://www.metos.at/pikernel/cidiwsdl.php?debug=1"
    'binding="basicHttpBinding" bindingConfiguration="iMetosSOAPBinding"
    'contract="WS_Metos.iMetosSOAPPortType" name="iMetosSOAPPort" />

    'Dim ContractDescription As New System.ServiceModel.Description.ContractDescription("WS_Metos.iMetosSOAPPortType", "http://metos.at")
    'ContractDescription = System.ServiceModel.Description.ContractDescription.GetContract(GetType(WS_Metos.iMetosSOAPPortType), GetType(WS_Metos.iMetosSOAPPortType))

    'Dim basicHttpBinding As New System.ServiceModel.BasicHttpBinding()
    'basicHttpBinding.Name = "iMetosSOAPBinding"
    'Dim EndpointAddress As New System.ServiceModel.EndpointAddress("http://www.metos.at/pikernel/cidiwsdl.php?debug=1")
    ''Dim Endpoint As New System.ServiceModel.Description.ServiceEndpoint(ContractDescription, basicHttpBinding, EndpointAddress)
    'Endpoint.Name = "iMetosSOAPPort"

    'Dim iMetosSOAPPortType As New WS_Metos.iMetosSOAPPortTypeClient(basicHttpBinding, EndpointAddress)
    'Dim GetUserInfoData As WS_Metos.struct_cidiuser_GetUserInfoData = _
    '       iMetosSOAPPortType.cidiuser_GetUserInfo(cookie, debug)
    'Dim GetInfoData As WS_Metos.struct_cidiuser_GetInfoData = _
    '       iMetosSOAPPortType.cidiuser_GetInfo(user, passw, debug)
    'Dim StationNumberData As WS_Metos.struct_cidistationlist3_GetStationNumberData
    'StationNumberData = iMetosSOAPPortType.cidistationlist3_GetStationNumber(user, passw, debug)
    'Dim cidistationlist2_GetFirstData As WS_Metos.struct_cidistationlist2_GetFirstData
    'cidistationlist2_GetFirstData = iMetosSOAPPortType.cidistationlist2_GetFirst(user, passw, rowcount, 0, debug)
    'Dim raggrupamento As Integer = 0 '0=nessun raggruppamento
    'Dim cidistationdata_GetFirstData As WS_Metos.struct_cidistationdata_GetFirstData
    'cidistationdata_GetFirstData = _
    '    iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode, debug)
    'cidistationdata_GetFirstData = _
    '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode + 1, debug)
    'cidistationdata_GetFirstData = _
    '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount + 1, stationname, groupcode, debug)
    'Dim cidistationdata_GetLastData As WS_Metos.struct_cidistationdata_GetLastData
    'cidistationdata_GetLastData = _
    '    iMetosSOAPPortType.cidistationdata_GetLast(user, passw, righe, stazione, groupcode, debug)
    'Return 0 'StationNumberData.DataSetMain.ReturnDataSet(0)



#Region "Stazioni"

    'Non funzia
    Public Shared Function Leggi_da_WS_Numero_Stazioni(ByVal user As String, ByVal passw As String) As Integer


        Try

            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()

            Dim debug As Integer = 0

            Dim key As String
            Dim cookie As String

            'Dim LoginData As WS_Metos.struct_cidiuser_LoginData = _
            '     iMetosSOAPPortType.cidiuser_Login(user, passw, key, cookie, debug)


            'Dim GetUserInfoData As WS_Metos.struct_cidiuser_GetUserInfoData = _
            '       iMetosSOAPPortType.cidiuser_GetUserInfo(cookie, debug)


            'Dim GetInfoData As WS_Metos.struct_cidiuser_GetInfoData = _
            '       iMetosSOAPPortType.cidiuser_GetInfo(user, passw, debug)


            Dim StationNumberData As WS_Metos.struct_cidistationlist3_GetStationNumberData
            StationNumberData = iMetosSOAPPortType.cidistationlist3_GetStationNumber(user, passw, debug)

            Dim rowcount As Integer = 0


            'Dim cidistationlist2_GetFirstData As WS_Metos.struct_cidistationlist2_GetFirstData
            'cidistationlist2_GetFirstData = iMetosSOAPPortType.cidistationlist2_GetFirst(user, passw, rowcount, 0, debug)

            'Dim stationname As String = "01101C60"
            'Dim groupcode As Integer = 0
            'Dim cidistationdata_GetFirstData As WS_Metos.struct_cidistationdata_GetFirstData
            'cidistationdata_GetFirstData = _
            '    iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode, debug)

            'cidistationdata_GetFirstData = _
            '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode + 1, debug)


            'cidistationdata_GetFirstData = _
            '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount + 1, stationname, groupcode, debug)


            'Dim cidistationdata_GetLastData As WS_Metos.struct_cidistationdata_GetLastData
            'cidistationdata_GetLastData = _
            '    iMetosSOAPPortType.cidistationdata_GetLast(user, passw, rowcount, stationname, groupcode, debug)

            Return StationNumberData.DataSetMain.ReturnDataSet(0)

        Catch ex As Exception
            Return 0
        End Try


    End Function

    Shared Function Importa_da_WS_TutteStazioni(username As String, password As String, StringaConnessione As String) As Boolean

        Try

            Dim Metor_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W
            Dim dt As DataTable = Metor_R_W.Metos_Stazioni_Appoggio_Leggi("", StringaConnessione)
            For i = 0 To dt.Rows.Count - 1
                AgronicaCoreWebService.Metos_WS.Importa_da_WS_Stazione(username, password, dt.Rows(i).Item("Nome"), StringaConnessione)
            Next
            Return True

        Catch ex As Exception
            Dim i = 0
            Return False
        End Try

    End Function

    Shared Function Importa_da_WS_Stazione(username As String, password As String, stazioneMeteo As String, StringaConnessione As String) As Boolean
        Try
            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()

            Dim Dt_Dati_Stazione As DataTable = Leggi_da_WS_Stazione(username, password, stazioneMeteo)

            If Dt_Dati_Stazione.Rows.Count = 1 Then

                Dim Metor_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

                If Metor_R_W.Metos_Stazioni_Leggi(0, stazioneMeteo, StringaConnessione).Rows.Count = 0 Then

                    Metor_R_W.Metos_Stazioni_Aggiungi(Dt_Dati_Stazione.Rows(0), StringaConnessione)

                    Return True

                End If

            End If

            Return False

        Catch ex As Exception
            Dim i = 0
            Return False
        End Try

    End Function

    Shared Function Leggi_da_WS_Stazione(username As String, password As String, stazioneMeteo As String) As DataTable
        Try
            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()


            Dim debug As Integer = 0



            Dim cidistationconfig2_GetData As WS_Metos.struct_cidistationconfig2_GetData
            cidistationconfig2_GetData = _
                iMetosSOAPPortType.cidistationconfig2_Get(username, password, stazioneMeteo, debug)




            Dim ReturnDataInfo As StructReturnDataInfo() = cidistationconfig2_GetData.DataSetMain.ReturnDataInfo
            Dim ReturnDataSet As String() = cidistationconfig2_GetData.DataSetMain.ReturnDataSet

            Dim Dt As DataTable = Genera_Datatable_From_Struct(ReturnDataInfo, ReturnDataSet, ".")

            Return Dt

        Catch ex As Exception
            Dim i = 0
            Return Nothing
        End Try
    End Function

#End Region
   

#Region "Sensori"

    Shared Function Importa_da_WS_Sensori_TutteStazioni(username As String, password As String, StringaConnessione As String) As DataTable

        Try

            Dim Metos_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

            Dim stazioniMeteo As String() = Metos_R_W.Metos_Stazioni_Leggi_Nomi(StringaConnessione)


            For i = 0 To stazioniMeteo.Count - 1
                Importa_da_WS_Sensori_Stazione(username, password, stazioniMeteo(i), StringaConnessione)
            Next


        Catch ex As Exception
            Dim i = 0
            Return Nothing
        End Try
    End Function

    Shared Function Importa_da_WS_Sensori_Stazione(username As String, password As String, stazioneMeteo As String, StringaConnessione As String) As Boolean
        Try
            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()

            Dim dts As DataTable = AgronicaCoreWebService.Metos_WS.Leggi_da_WS_Sensori_Stazione(username, password, stazioneMeteo)

            Dim Metor_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

            For i = 0 To dts.Rows.Count - 1
                Metor_R_W.Metos_Sensore_AggiungiAggiorna(dts.Rows(i), stazioneMeteo, StringaConnessione)
            Next

            Return True


        Catch ex As Exception
            Dim i = 0
            Return False
        End Try
    End Function

    Shared Function Leggi_da_WS_Sensori_Stazione(username As String, password As String, stazioneMeteo As String) As DataTable
        Try

            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()
            Dim debug As Integer = 0

            Dim cidistationsensors3_GetData As WS_Metos.struct_cidistationsensors3_GetData
            cidistationsensors3_GetData = _
                iMetosSOAPPortType.cidistationsensors3_Get(username, password, stazioneMeteo, False, False)


            Dim ReturnDataInfo As StructReturnDataInfo() = cidistationsensors3_GetData.DataSetMain.ReturnDataInfo
            Dim ReturnDataSet As String() = cidistationsensors3_GetData.DataSetMain.ReturnDataSet

            Dim Dt_Dati_Meteo As DataTable = Genera_Datatable_From_Struct(ReturnDataInfo, ReturnDataSet, ".")

            Return Dt_Dati_Meteo

        Catch ex As Exception
            Dim i = 0
            Return Nothing
        End Try
    End Function

#End Region


#Region "DatiMeteo"

    <Obsolete("2. non utilizzare!", True)>
    Shared Function Importa_da_WS_DatiMeteo_TutteStazioni(username As String, password As String,
                                                          StringaConnessione As String, ByVal dataInizio As Date, dataFine As Date,
                                                          ByVal eseguiVerificheSensori As Boolean) As String

        Dim elencoErrori As String = ""

        Try

            Dim Metos_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W

            Dim stazioniMeteo As String() = Metos_R_W.Metos_Stazioni_Leggi_Nomi(StringaConnessione)


            For i = 0 To stazioniMeteo.Count - 1
                Try
                    Importa_da_WS_DatiMeteo_Stazione(username, password, StringaConnessione, stazioniMeteo(i), dataInizio, dataFine, eseguiVerificheSensori)
                Catch ex As Exception
                    Dim Metos_ As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W
                    Dim Errore As String = "Errore di importazione della stazione " & stazioniMeteo(i) & ": " & ex.Message
                    elencoErrori &= Errore
                    Metos_.LogErr(Errore)
                End Try

            Next


        Catch ex As Exception
            Dim i = 0
            Return ex.Message
        End Try

        Return elencoErrori

    End Function

    Shared Sub Importa_da_WS_DatiMeteo_Stazione(ByVal username As String, _
                                                ByVal password As String, _
                                                ByVal StringaConnessione As String, _
                                                ByVal stazioneMeteo As String, _
                                                ByVal dataInizio As Date, _
                                                dataFine As Date, _
                                                ByVal eseguiVerificheSensori As Boolean)
        Dim Metos_R_W As New AgronicaCoreVarieDAL.AgronicaMeteoSuite.Metos_R_W
        Dim station_code As Integer = Metos_R_W.Metos_Stazioni_Station_Code_Fom_Nome(stazioneMeteo, StringaConnessione)

        If eseguiVerificheSensori Then
            'leggo i sensori della stazione (sensor_code, group_code = tipo,  name=nome gruppo, unit= udm, tipologie (media max min..)
            'station_code 1 - f_sensor_ch 2 - f_sensor_code 3 - f_chain_code 4
            '- f_group_code 5 - f_unit_code 6 - f_name 7 - f_unit 8 - f_div 9 -
            'f_mul 10 - f_val_neg 11 - f_val_log 12 - f_val_last 13 - f_val_sum 14 - f_val_aver 15 -
            'f_val_min 16 - f_val_max 17 - f_val_time 18 - f_val_user 19 - f_create_time 20 -
            'f_val_axilary 21 - f_user_app 22 - f_color 23 - f_sensor_user_name 24 - f_opacity 25 -
            'f_user_unit_code 26 - f_unit_html
            Dim Sensori As DataTable = Leggi_da_WS_Sensori_Stazione(username, password, stazioneMeteo)


            'verifico se ho il gruppo in locale e se la descrizione e udm corrisponde
            'es: f_group_code=Air temperature in °C
            For j = 0 To Sensori.Rows.Count - 1

                Dim f_station_code As Integer = Sensori.Rows(j).Item("f_station_code")
                If station_code = f_station_code Then
                    Metos_R_W.Metos_Tipo_Sensore_VerificaAggiungiAggiorna(stazioneMeteo, StringaConnessione, Sensori.Rows(j))

                End If

            Next
        End If


        'genero il nome colonna e verifico se ho in locale
        'f_val_aver=1 -> ho la media quindi mappo il codice
        'f_group_code = 1 + f_val_aver=1 come descrizione 'Temperatura Media'

        'importo i dati meteo
        'dal nome colonna ricavo il codice sensore (ultimo numero dopo _)
        'dal nome di colona ricavo sens_min etc per capire
        Dim data As Date = dataInizio
        Dim righe As Integer = 24 '24 righe per raggruppamento orario
        Dim raggruppamento = 1 'orario
        While data <= dataFine
            Dim Dati As DataTable = Leggi_da_WS_DatiMeteo_Stazione(username, password, stazioneMeteo, data, righe, raggruppamento)
            Dim r5 As Boolean = Metos_R_W.Metos_Dati_Rilevati_RinominaColonne_E_AggiungiAggiorna(station_code, stazioneMeteo, Dati, StringaConnessione)
            data = data.AddDays(1)
        End While

    End Sub

    Shared Function Leggi_da_WS_DatiMeteo_Stazione(username As String, password As String, stazioneMeteo As String, dataInizio As String, numeroRighe As String, raggrupamento As Integer) As DataTable


        ' Dim raggrupamento As Integer = 0 
        '0:        no group (default)
        '1:        Hour
        '2:        Day
        '3:        Month

        Try

            Dim iMetosSOAPPortType As WS_Metos.iMetosSOAPPortTypeClient = GetIMetosSOAPPortType()

            Dim debug As Integer = 0
            Dim key As String
            Dim cookie As String

            'Dim LoginData As WS_Metos.struct_cidiuser_LoginData = _
            '     iMetosSOAPPortType.cidiuser_Login(username, password, key, cookie, debug)


            'Dim GetUserInfoData As WS_Metos.struct_cidiuser_GetUserInfoData = _
            '       iMetosSOAPPortType.cidiuser_GetUserInfo(cookie, debug)


            'Dim GetInfoData As WS_Metos.struct_cidiuser_GetInfoData = _
            '       iMetosSOAPPortType.cidiuser_GetInfo(user, passw, debug)


            'Dim StationNumberData As WS_Metos.struct_cidistationlist3_GetStationNumberData
            'StationNumberData = iMetosSOAPPortType.cidistationlist3_GetStationNumber(user, passw, debug)



            'Dim cidistationlist2_GetFirstData As WS_Metos.struct_cidistationlist2_GetFirstData
            'cidistationlist2_GetFirstData = iMetosSOAPPortType.cidistationlist2_GetFirst(user, passw, rowcount, 0, debug)




            'Dim cidistationdata_GetFirstData As WS_Metos.struct_cidistationdata_GetFirstData
            'cidistationdata_GetFirstData = _
            '    iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode, debug)

            'cidistationdata_GetFirstData = _
            '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount, stationname, groupcode + 1, debug)


            'cidistationdata_GetFirstData = _
            '        iMetosSOAPPortType.cidistationdata_GetFirst(user, passw, rowcount + 1, stationname, groupcode, debug)


            'Dim cidistationdata_GetLastData As WS_Metos.struct_cidistationdata_GetLastData
            'cidistationdata_GetLastData = _
            '    iMetosSOAPPortType.cidistationdata_GetLast(user, passw, righe, stazione, groupcode, debug)

            'Return 0 'StationNumberData.DataSetMain.ReturnDataSet(0)


            Dim struct_cidistationdata_GetFromDateData As WS_Metos.struct_cidistationdata_GetFromDateData
            struct_cidistationdata_GetFromDateData = _
                iMetosSOAPPortType.cidistationdata_GetFromDate(username, password, numeroRighe, stazioneMeteo, raggrupamento, dataInizio, debug)




            Dim ReturnDataInfo As StructReturnDataInfo() = struct_cidistationdata_GetFromDateData.DataSetMain.ReturnDataInfo
            Dim ReturnDataSet As String() = struct_cidistationdata_GetFromDateData.DataSetMain.ReturnDataSet

            Dim Dt_Dati_Meteo As DataTable = Genera_Datatable_From_Struct(ReturnDataInfo, ReturnDataSet, ".")

            Return Dt_Dati_Meteo

        Catch ex As Exception
            Dim i = 0
            Return Nothing
        End Try

    End Function

#End Region


#Region "Varie"

    Private Shared Function GetIMetosSOAPPortType() As WS_Metos.iMetosSOAPPortTypeClient
        Dim basicHttpBinding As New System.ServiceModel.BasicHttpBinding()
        basicHttpBinding.Name = "iMetosSOAPBinding"
        basicHttpBinding.MaxReceivedMessageSize = 65536 * 4
        Dim EndpointAddress As New System.ServiceModel.EndpointAddress("http://www.metos.at/pikernel/cidiwsdl.php?debug=1")
        Dim iMetosSOAPPortType As New WS_Metos.iMetosSOAPPortTypeClient(basicHttpBinding, EndpointAddress)
        Return iMetosSOAPPortType
    End Function

    Private Shared Function Genera_Datatable_From_Struct(ByVal ReturnDataInfo As StructReturnDataInfo(), ByVal ReturnDataSet As String(), ByVal carattereDecimale As String) As DataTable
        Dim NumeroRecodTotali As Integer = ReturnDataSet.Length
        Dim NumeroColonne As Integer = ReturnDataInfo.Length
        Dim numeroRighe2 As Integer = NumeroRecodTotali / NumeroColonne

        Dim Dt_Dati_Meteo As New DataTable

        For i = 0 To NumeroColonne - 1
            Dt_Dati_Meteo.Columns.Add(New DataColumn(ReturnDataInfo(i).s_name, GetType(String)))
        Next

        Dim dr As DataRow
        Dim val As String
        For ir = 0 To numeroRighe2 - 1
            dr = Dt_Dati_Meteo.NewRow
            For ic = 0 To NumeroColonne - 1
                val = ReturnDataSet(ir * NumeroColonne + ic)
                If IsNumeric(val) Then
                    val = val.Replace(".", ",")
                End If
                dr.Item(ic) = val
            Next
            Dt_Dati_Meteo.Rows.Add(dr)
        Next
        Return Dt_Dati_Meteo
    End Function

#End Region




    'Shared Function LeggiSalvaSensoriStazioni(username As String, password As String, stazioniMeteo As String()) As DataTable

    '    Try


    '        For i = 0 To stazioniMeteo.Count - 1

    '            'leggo i sensori della stazione (sensor_code, group_code = tipo,  name=nome gruppo, unit= udm, tipologie (media max min..)
    '            'station_code 1 - f_sensor_ch 2 - f_sensor_code 3 - f_chain_code 4 
    '            '- f_group_code 5 - f_unit_code 6 - f_name 7 - f_unit 8 - f_div 9 - 
    '            'f_mul 10 - f_val_neg 11 - f_val_log 12 - f_val_last 13 - f_val_sum 14 - f_val_aver 15 - 
    '            'f_val_min 16 - f_val_max 17 - f_val_time 18 - f_val_user 19 - f_create_time 20 - 
    '            'f_val_axilary 21 - f_user_app 22 - f_color 23 - f_sensor_user_name 24 - f_opacity 25 - 
    '            'f_user_unit_code 26 - f_unit_html 
    '            Dim Sensori As DataTable = Leggi_da_WS_Sensori_Stazione(username, password, stazioniMeteo(i))


    '            'verifico se ho il gruppo in locale e se la descrizione e udm corrisponde
    '            'es: f_group_code=Air temperature in °C

    '            'genero il nome colonna e verifico se ho in locale 
    '            'f_val_aver=1 -> ho la media quindi mappo il codice
    '            'f_group_code = 1 + f_val_aver=1 come descrizione 'Temperatura Media'

    '            'importo i dati meteo
    '            'dal nome colonna ricavo il codice sensore (ultimo numero dopo _)
    '            'dal nome di colona ricavo sens_min etc per capire 
    '            Dim Dati As DataTable = LeggiValori(username, password, stazioniMeteo(i), Date.Now, 50)


    '        Next


    '    Catch ex As Exception
    '        Dim i = 0
    '        Return Nothing
    '    End Try
    'End Function

End Class
