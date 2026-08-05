Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Vasche_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Vas_Cod As Int32, _
                            ByVal Piano_Cod As Int32, _
                            ByVal Identificativo As String, _
                            ByVal Numero_Serie As String, _
                            ByVal Modello As String, _
                            ByVal Materiale_Cod As Int32, _
                            ByVal Appoggio_Cod As Int32, _
                            ByVal Inclinato As Integer, _
                            ByVal Refrigerata As Integer, _
                            ByVal Tipo_Tasca As Int32, _
                            ByVal Coibentata As Integer, _
                            ByVal Udm_Cod_Capacita As Int32, _
                            ByVal Capacita_Nominale As Decimal, _
                            ByVal Capacita_Effettiva As Decimal, _
                            ByVal Udm_Cod_Altezza As Int32, _
                            ByVal Altezza_Cilindro As Decimal, _
                            ByVal Altezza_Totale As Decimal, _
                            ByVal Udm_Cod_Peso As Int32, _
                            ByVal Peso As Decimal, _
                            ByVal DimX As Int32, _
                            ByVal DimY As Int32, _
                            ByVal Rotazione As Decimal, _
                            ByVal Costo_Acquisto As Decimal, _
                            ByVal Ammortamento As Decimal, _
                            ByVal Ultima_Revisione As Date, _
                            ByVal Note As String, _
                            ByVal Tipo As String, _
                            ByVal Spessore As Int32, _
                            ByVal Colore_Esterno As Int32, _
                            ByVal PosX As Int32, _
                            ByVal PosY As Int32, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Vasche_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Cantina_Vasche ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,             Sa_Cod,           Vas_Cod,           Piano_Cod,          Identificativo,     Numero_Serie,    ")
            StrSQL.Append("          Modello,          Materiale_Cod,    Appoggio_Cod,      Inclinato,          Refrigerata,        Tipo_Tasca,      ")
            StrSQL.Append("          Coibentata,       Udm_Cod_Capacita, Capacita_Nominale, Capacita_Effettiva, Udm_Cod_Altezza, ")
            StrSQL.Append("          Altezza_Cilindro, Altezza_Totale,   Udm_Cod_Peso,      Peso,               DimX,  DimY,     ")
            StrSQL.Append("          Rotazione,        Costo_Acquisto,   Ammortamento,      Ultima_Revisione,   Note,            ")
            StrSQL.Append("          Tipo,             Spessore,         Colore_Esterno,    PosX,               PosY,                                ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Vas_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Identificativo) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Numero_Serie) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Modello) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Materiale_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appoggio_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Inclinato) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Refrigerata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Tasca) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Coibentata) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Capacita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Capacita_Nominale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Capacita_Effettiva))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Altezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Altezza_Cilindro) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Altezza_Totale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Peso) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DimX) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DimY) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Rotazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Costo_Acquisto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ammortamento) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Ultima_Revisione) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.Append("         ,'" & UCase(Agro_SQL_SaveText(Tipo)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Spessore) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Colore_Esterno) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PosX) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PosY) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Modifica(ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Vas_Cod As Int32, _
                            ByVal Piano_Cod As Int32, _
                            ByVal Identificativo As String, _
                            ByVal Numero_Serie As String, _
                            ByVal Modello As String, _
                            ByVal Materiale_Cod As Long, _
                            ByVal Appoggio_Cod As Int32, _
                            ByVal Inclinato As Integer, _
                            ByVal Refrigerata As Integer, _
                            ByVal Tipo_Tasca As Int32, _
                            ByVal Coibentata As Int16, _
                            ByVal Udm_Cod_Capacita As Int32, _
                            ByVal Capacita_Nominale As Decimal, _
                            ByVal Capacita_Effettiva As Decimal, _
                            ByVal Udm_Cod_Altezza As Long, _
                            ByVal Altezza_Cilindro As Decimal, _
                            ByVal Altezza_Totale As Decimal, _
                            ByVal Udm_Cod_Peso As Int32, _
                            ByVal Peso As Decimal, _
                            ByVal DimX As Int32, _
                            ByVal DimY As Int32, _
                            ByVal Rotazione As Decimal, _
                            ByVal Costo_Acquisto As Decimal, _
                            ByVal Ammortamento As Decimal, _
                            ByVal Ultima_Revisione As Date, _
                            ByVal Note As String, _
                            ByVal Tipo As String, _
                            ByVal Spessore As Int32, _
                            ByVal Colore_Esterno As Int32, _
                            ByVal PosX As Int32, _
                            ByVal PosY As Int32, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Vasche_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Cantina_Vasche SET ")
            StrSQL.Append("    Piano_Cod                    =  " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            StrSQL.Append("   ,Identificativo               = '" & Agro_SQL_SaveText(Identificativo) & "'  ")
            StrSQL.Append("   ,Numero_Serie                 = '" & Agro_SQL_SaveText(Numero_Serie) & "'  ")
            StrSQL.Append("   ,Modello                      = '" & Agro_SQL_SaveText(Modello) & "'  ")
            StrSQL.Append("   ,Materiale_Cod                =  " & Agro_SQL_SaveNum(Materiale_Cod) & "  ")
            StrSQL.Append("   ,Appoggio_Cod                 =  " & Agro_SQL_SaveNum(Appoggio_Cod) & "  ")
            StrSQL.Append("   ,Inclinato                    =  " & Agro_SQL_SaveNum(Inclinato) & "  ")
            StrSQL.Append("   ,Refrigerata                  =  " & Agro_SQL_SaveNum(Refrigerata) & "  ")
            StrSQL.Append("   ,Tipo_Tasca                   =  " & Agro_SQL_SaveNum(Tipo_Tasca) & "  ")
            StrSQL.Append("   ,Coibentata                   =  " & Agro_SQL_SaveNum(Coibentata) & "  ")
            StrSQL.Append("   ,Udm_Cod_Capacita             =  " & Agro_SQL_SaveNum(Udm_Cod_Capacita) & "  ")
            StrSQL.Append("   ,Capacita_Nominale            =  " & Agro_SQL_SaveNum(Capacita_Nominale) & "  ")
            StrSQL.Append("   ,Capacita_Effettiva           =  " & Agro_SQL_SaveNum(Capacita_Effettiva) & "  ")
            StrSQL.Append("   ,Udm_Cod_Altezza              =  " & Agro_SQL_SaveNum(Udm_Cod_Altezza) & "  ")
            StrSQL.Append("   ,Altezza_Cilindro             =  " & Agro_SQL_SaveNum(Altezza_Cilindro) & "  ")
            StrSQL.Append("   ,Altezza_Totale               =  " & Agro_SQL_SaveNum(Altezza_Totale) & "  ")
            StrSQL.Append("   ,Udm_Cod_Peso                 =  " & Agro_SQL_SaveNum(Udm_Cod_Peso) & "  ")
            StrSQL.Append("   ,Peso                         =  " & Agro_SQL_SaveNum(Peso) & "  ")
            StrSQL.Append("   ,DimX                         =  " & Agro_SQL_SaveNum(DimX) & "  ")
            StrSQL.Append("   ,DimY                         =  " & Agro_SQL_SaveNum(DimY) & "  ")
            StrSQL.Append("   ,Rotazione                    =  " & Agro_SQL_SaveNum(Rotazione) & "  ")
            StrSQL.Append("   ,Costo_Acquisto               =  " & Agro_SQL_SaveNum(Costo_Acquisto) & "  ")
            StrSQL.Append("   ,Ammortamento                 =  " & Agro_SQL_SaveNum(Ammortamento) & "  ")
            StrSQL.Append("   ,Ultima_Revisione             =  " & Agro_SQL_SaveDate(Ultima_Revisione) & "  ")
            StrSQL.Append("   ,Note                         = '" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.Append("   ,Tipo                         = '" & UCase(Agro_SQL_SaveText(Tipo)) & "'  ")
            StrSQL.Append("   ,Spessore                     =  " & Agro_SQL_SaveNum(Spessore) & "  ")
            StrSQL.Append("   ,Colore_Esterno               =  " & Agro_SQL_SaveNum(Colore_Esterno) & "  ")
            StrSQL.Append("   ,PosX                         =  " & Agro_SQL_SaveNum(PosX) & "  ")
            StrSQL.Append("   ,PosY                         =  " & Agro_SQL_SaveNum(PosY) & "  ")


            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))


            StrSQL.Append(" WHERE Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & "   ")
            End If


            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Vas_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Vasche_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Vas_Cod = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Cantina_Vasche ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Cantina_Vasche ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & "   ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
