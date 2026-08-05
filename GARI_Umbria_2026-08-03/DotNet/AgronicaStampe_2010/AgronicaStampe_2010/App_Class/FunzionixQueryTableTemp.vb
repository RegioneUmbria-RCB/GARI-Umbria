Public Module FunzionixQueryTableTemp


    '###############################################################################################
       'legge la chiave del web.config -> nell'agronicastampe_2010 il webconfig non c'è e mette allora il default
    Public Function Recupera_TabelleTemp_Mode() As Integer

        '<add key="TabelleTemp_Mode" value="1"/> <!-- 1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO | 2: TRAMITE CREATE TABLE -->

        '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT - INSERT INTO
        '2: CREAZIONE TABELLA TEMPORANEA TRAMITE CREATE TABLE

        Dim TabelleTemp_Mode As Integer

        If Not IsNothing(ConfigurationSettings.AppSettings("TabelleTemp_Mode")) And _
                ConfigurationSettings.AppSettings("TabelleTemp_Mode") <> "" Then

            TabelleTemp_Mode = ConfigurationSettings.AppSettings("TabelleTemp_Mode")

            If TabelleTemp_Mode <> 1 And TabelleTemp_Mode <> 2 Then
                TabelleTemp_Mode = 1
            End If

        Else
            TabelleTemp_Mode = 1

        End If


        Return TabelleTemp_Mode


    End Function

    '###############################################################################################
       'legge la chiave del web.config -> nell'agronicastampe_2010 il webconfig non c'è e mette allora il default
    Public Function Recupera_Str_TabelleTemp_RegolaConfronto() As String

        '<add key="Str_TabelleTemp_RegolaConfronto" value=""/> <!-- Esempio: COLLATE SQL_Latin1_General_CP850_CI_AS -->

        'Recupera la regola di confronto

        Dim Str_TabelleTemp_RegolaConfronto As String

        If Not IsNothing(ConfigurationSettings.AppSettings("Str_TabelleTemp_RegolaConfronto")) And _
                ConfigurationSettings.AppSettings("Str_TabelleTemp_RegolaConfronto") <> "" Then

            Str_TabelleTemp_RegolaConfronto = ConfigurationSettings.AppSettings("Str_TabelleTemp_RegolaConfronto")

        Else
            Str_TabelleTemp_RegolaConfronto = " COLLATE SQL_Latin1_General_CP850_CI_AS "

        End If


        Return Str_TabelleTemp_RegolaConfronto


    End Function




End Module
