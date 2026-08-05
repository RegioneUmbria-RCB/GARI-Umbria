
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Prodotti_Extra_Privata_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Piva As String,
                            ByVal Mat_Cod As Integer,
                            ByVal Elem_COd As Integer,
                            ByVal Pro_Cod As Integer,
                            ByVal Cod_Iva As Integer?,
                            ByVal Cod_Iva_Compensazione As Integer?,
                            ByVal Cod_Conto_Economico_Acquisto As Integer?,
                            ByVal Cod_Conto_Economico_Vendita As Integer?,
                            ByVal Cod_Conto_Patrimoniale_Acquisto As Integer?,
                            ByVal Cod_Conto_Patrimoniale_Vendita As Integer?,
                            ByVal EAN As String,
                            ByVal Barcode As String,
                            ByVal Ingredienti As String,
                            ByVal Produzione_Propria As Integer?,
                            ByVal Peso_Netto As Decimal?,
                            ByVal Peso_Sgocciolato As Decimal?,
                            ByVal Tara As Decimal?,
                            ByVal Peso_Egalizzato As Integer?,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Prodotti_Extra_Privata_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Prodotti_Extra_Privata  ")
                    StrSQL.AppendLine(" WHERE 1=1")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" SELECT Prodotti_Extra_Privata.* ")
                    StrSQL.AppendLine(" ,Gruppi_Merce.Codice AS Gruppo_Merce_Codice, Gruppi_Merce.Descrizione AS Gruppo_Merce_Descrizione ")
                    StrSQL.AppendLine(" FROM  Prodotti_Extra_Privata  ")
                    StrSQL.AppendLine(" LEFT JOIN Gruppi_Merce ")
                    StrSQL.AppendLine(" ON Gruppi_Merce.Id_Gruppo_Merce = Prodotti_Extra_Privata.Id_Gruppo_Merce ")
                    StrSQL.AppendLine(" WHERE 1=1")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

            End Select

            StrSQL.AppendLine(" AND   Prodotti_Extra_Privata.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Prodotti_Extra_Privata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   Prodotti_Extra_Privata.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  Prodotti_Extra_Privata.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Mat_COd = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Elem_COd <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Elem_Cod = " & Agro_SQL_SaveNum(Elem_COd) & "   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Not Cod_Iva Is Nothing AndAlso Cod_Iva.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Iva = " & Agro_SQL_SaveNum(Cod_Iva.Value) & "   ")
            End If

            If Not Cod_Iva_Compensazione Is Nothing AndAlso Cod_Iva_Compensazione.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Iva_Compensazione = " & Agro_SQL_SaveNum(Cod_Iva_Compensazione.Value) & "   ")
            End If

            If Not Cod_Conto_Economico_Acquisto Is Nothing AndAlso Cod_Conto_Economico_Acquisto.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Conto_Economico_Acquisto = " & Agro_SQL_SaveNum(Cod_Conto_Economico_Acquisto.Value) & "   ")
            End If

            If Not Cod_Conto_Economico_Vendita Is Nothing AndAlso Cod_Conto_Economico_Vendita.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Conto_Economico_Vendita = " & Agro_SQL_SaveNum(Cod_Conto_Economico_Vendita.Value) & "   ")
            End If

            If Not Cod_Conto_Patrimoniale_Acquisto Is Nothing AndAlso Cod_Conto_Patrimoniale_Acquisto.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Conto_Patrimoniale_Acquisto = " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Acquisto.Value) & "   ")
            End If

            If Not Cod_Conto_Patrimoniale_Vendita Is Nothing AndAlso Cod_Conto_Patrimoniale_Vendita.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Cod_Conto_Patrimoniale_Vendita = " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Vendita.Value) & "   ")
            End If

            If EAN <> "" Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.EAN = '" & Agro_SQL_SaveText(EAN) & "'   ")
            End If

            If Barcode <> "" Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Barcode = '" & Agro_SQL_SaveText(Barcode) & "'   ")
            End If

            If Ingredienti <> "" Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Ingredienti = '" & Agro_SQL_SaveText(Ingredienti) & "'   ")
            End If

            If Not Produzione_Propria Is Nothing AndAlso Produzione_Propria.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Produzione_Propria = " & Agro_SQL_SaveNum(Produzione_Propria.Value) & "   ")
            End If

            If Not Peso_Netto Is Nothing AndAlso Peso_Netto.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Peso_Netto = " & Agro_SQL_SaveNum(Peso_Netto.Value) & "   ")
            End If

            If Not Peso_Sgocciolato Is Nothing AndAlso Peso_Sgocciolato.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Peso_Sgocciolato = " & Agro_SQL_SaveNum(Peso_Sgocciolato.Value) & "   ")
            End If

            If Not Tara Is Nothing AndAlso Tara.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Tara = " & Agro_SQL_SaveNum(Tara.Value) & "   ")
            End If

            If Not Peso_Egalizzato Is Nothing AndAlso Peso_Egalizzato.HasValue Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Peso_Egalizzato = " & Agro_SQL_SaveNum(Peso_Egalizzato.Value) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Prodotti_Extra_Privata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Prodotti_Extra_Privata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Prodotti_Extra_Privata.Piva ASC, Prodotti_Extra_Privata.Mat_Cod ASC, Prodotti_Extra_Privata.Pro_Cod ASC")
            End If

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

End Class

Public Class Prodotti_Extra_Privata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                                ByVal Piva As String,
                                ByVal Mat_Cod As Integer,
                                ByVal Elem_COd As Integer,
                                ByVal Pro_Cod As Integer,
                                ByVal Cod_Iva As Integer?,
                                ByVal Cod_Iva_Compensazione As Integer?,
                                ByVal Cod_Conto_Economico_Acquisto As Integer?,
                                ByVal Cod_Conto_Economico_Vendita As Integer?,
                                ByVal Cod_Conto_Patrimoniale_Acquisto As Integer?,
                                ByVal Cod_Conto_Patrimoniale_Vendita As Integer?,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal EAN As String,
                                ByVal Barcode As String,
                                ByVal Ingredienti As String,
                                ByVal Produzione_Propria As Integer?,
                                ByVal SalvaAllegatoImmagine As Integer,
                                ByVal Immagine As Byte(),
                                ByVal Immagine_Estensione As String,
                                ByVal Immagine_NomeFile As String,
                                ByVal Peso_Netto As Decimal?,
                                ByVal Peso_Sgocciolato As Decimal?,
                                ByVal Tara As Decimal?,
                                ByVal Peso_Egalizzato As Decimal?,
                                ByVal Id_Gruppo_Merce As Integer?,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                Optional ByVal username_creazione As String = "",
                                Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Prodotti_Extra_Privata_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Prodotti_Extra_Privata (  Piva_SuperUser, Piva,  ")
            StrSQL.AppendLine("                    Mat_Cod, Elem_Cod, Pro_Cod, ")
            StrSQL.AppendLine("                    Cod_Iva, Cod_Iva_Compensazione, ")
            StrSQL.AppendLine("                    Cod_Conto_Economico_Acquisto, Cod_Conto_Economico_Vendita, ")
            StrSQL.AppendLine("                    Cod_Conto_Patrimoniale_Acquisto, Cod_Conto_Patrimoniale_Vendita, ")
            StrSQL.AppendLine("                    EAN, Barcode, Ingredienti, Produzione_Propria, ")
            StrSQL.AppendLine("                    Immagine, Immagine_Estensione, Immagine_NomeFile, ")
            StrSQL.AppendLine("                    Peso_Netto, Peso_Sgocciolato, Tara, Peso_Egalizzato, Id_Gruppo_Merce, ")
            StrSQL.AppendLine("                    Inviato, DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Elem_COd) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & " ")

            If Not Cod_Iva Is Nothing AndAlso Cod_Iva.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Iva) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Cod_Iva_Compensazione Is Nothing AndAlso Cod_Iva_Compensazione.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Iva_Compensazione) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Cod_Conto_Economico_Acquisto Is Nothing AndAlso Cod_Conto_Economico_Acquisto.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Economico_Acquisto) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Cod_Conto_Economico_Vendita Is Nothing AndAlso Cod_Conto_Economico_Vendita.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Economico_Vendita) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Cod_Conto_Patrimoniale_Acquisto Is Nothing AndAlso Cod_Conto_Patrimoniale_Acquisto.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Acquisto) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Cod_Conto_Patrimoniale_Vendita Is Nothing AndAlso Cod_Conto_Patrimoniale_Vendita.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Vendita) & " ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If EAN <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(EAN) & "' ")
            Else
                StrSQL.AppendLine("         , '' ")
            End If

            If Barcode <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Barcode) & "' ")
            Else
                StrSQL.AppendLine("         , '' ")
            End If

            If Ingredienti <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Ingredienti) & "' ")
            Else
                StrSQL.AppendLine("         , NULL ")
            End If

            If Not Produzione_Propria Is Nothing AndAlso Produzione_Propria.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Produzione_Propria) & " ")
            Else
                StrSQL.AppendLine("         , 0 ")
            End If

            'Se 1 salvo l'immagine altrimenti se 0 scrivo NULL
            If SalvaAllegatoImmagine = 1 Then
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    'Salvataggio su DB
                    StrSQL.AppendLine(" , ?")
                Else
                    StrSQL.AppendLine(" , @P1")
                End If

                StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Immagine_Estensione) & "'")
                StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Immagine_NomeFile) & "'")

            Else
                StrSQL.AppendLine("         , NULL")
                StrSQL.AppendLine("         , NULL")
                StrSQL.AppendLine("         , NULL")
            End If

            If Not Peso_Netto Is Nothing AndAlso Peso_Netto.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Peso_Netto) & " ")
            Else
                StrSQL.AppendLine("         , 0 ")
            End If

            If Not Peso_Sgocciolato Is Nothing AndAlso Peso_Sgocciolato.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Peso_Sgocciolato) & " ")
            Else
                StrSQL.AppendLine("         , 0 ")
            End If

            If Not Tara Is Nothing AndAlso Tara.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tara) & " ")
            Else
                StrSQL.AppendLine("         , 0 ")
            End If

            If Not Peso_Egalizzato Is Nothing AndAlso Peso_Egalizzato.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Peso_Egalizzato) & " ")
            Else
                StrSQL.AppendLine("         , -1 ")
            End If

            If Not Id_Gruppo_Merce Is Nothing AndAlso Id_Gruppo_Merce.HasValue Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Gruppo_Merce) & " ")
            Else
                StrSQL.AppendLine("         , 0 ")
            End If

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            If SalvaAllegatoImmagine Then
                '---------------------------------------------
                Dim CmdParameters As New Dictionary(Of String, Byte())
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    CmdParameters.Add("?", Immagine)
                Else
                    CmdParameters.Add("@P1", Immagine)
                End If

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, NomeRoutine, CmdParameters)
                '--------------------------------------------------------------------------            
            Else
                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------  

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(
                               ByVal Piva As String,
                               ByVal Mat_Cod As Integer,
                               ByVal Elem_COd As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Cod_Iva As Integer?,
                               ByVal Cod_Iva_Compensazione As Integer?,
                               ByVal Cod_Conto_Economico_Acquisto As Integer?,
                               ByVal Cod_Conto_Economico_Vendita As Integer?,
                               ByVal Cod_Conto_Patrimoniale_Acquisto As Integer?,
                               ByVal Cod_Conto_Patrimoniale_Vendita As Integer?,
                               ByVal Validita_Inizio As Date?,
                               ByVal Validita_Fine As Date?,
                               ByVal EAN As String,
                                ByVal Barcode As String,
                                ByVal Ingredienti As String,
                                ByVal Produzione_Propria As Integer?,
                                ByVal SalvaAllegatoImmagine As Integer,
                                ByVal Immagine As Byte(),
                                ByVal Immagine_Estensione As String,
                                ByVal Immagine_NomeFile As String,
                                ByVal Peso_Netto As Decimal?,
                                ByVal Peso_Sgocciolato As Decimal?,
                                ByVal Tara As Decimal?,
                                ByVal Peso_Egalizzato As Decimal?,
                                ByVal Id_Gruppo_Merce As Integer?,
                                ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                               Optional ByVal username_modifica As String = ""
                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Prodotti_Extra_Privata_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If


        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Prodotti_Extra_Privata SET  ")

            If Not Cod_Iva Is Nothing AndAlso Cod_Iva.HasValue Then
                StrSQL.AppendLine(" Cod_Iva = " & Agro_SQL_SaveNum(Cod_Iva) & "  ")
            Else
                StrSQL.AppendLine(" Cod_Iva = NULL ")
            End If

            If Not Cod_Iva_Compensazione Is Nothing AndAlso Cod_Iva_Compensazione.HasValue Then
                StrSQL.AppendLine(" ,Cod_Iva_Compensazione = " & Agro_SQL_SaveNum(Cod_Iva_Compensazione) & "  ")
            Else
                StrSQL.AppendLine(" ,Cod_Iva_Compensazione = NULL ")
            End If

            If Not Cod_Conto_Economico_Acquisto Is Nothing AndAlso Cod_Conto_Economico_Acquisto.HasValue Then
                StrSQL.AppendLine(" ,Cod_Conto_Economico_Acquisto = " & Agro_SQL_SaveNum(Cod_Conto_Economico_Acquisto) & "  ")
            Else
                StrSQL.AppendLine(" ,Cod_Conto_Economico_Acquisto = NULL ")
            End If

            If Not Cod_Conto_Economico_Vendita Is Nothing AndAlso Cod_Conto_Economico_Vendita.HasValue Then
                StrSQL.AppendLine(" ,Cod_Conto_Economico_Vendita = " & Agro_SQL_SaveNum(Cod_Conto_Economico_Vendita) & "  ")
            Else
                StrSQL.AppendLine(" ,Cod_Conto_Economico_Vendita = NULL ")
            End If

            If Not Cod_Conto_Patrimoniale_Acquisto Is Nothing AndAlso Cod_Conto_Patrimoniale_Acquisto.HasValue Then
                StrSQL.AppendLine(" ,Cod_Conto_Patrimoniale_Acquisto = " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Acquisto) & "  ")
            Else
                StrSQL.AppendLine(" ,Cod_Conto_Patrimoniale_Acquisto = NULL ")
            End If

            If Not Cod_Conto_Patrimoniale_Vendita Is Nothing AndAlso Cod_Conto_Patrimoniale_Vendita.HasValue Then
                StrSQL.AppendLine(" ,Cod_Conto_Patrimoniale_Vendita = " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Vendita) & "  ")
            Else
                StrSQL.AppendLine(" ,Cod_Conto_Patrimoniale_Vendita = NULL ")
            End If

            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Not Validita_Inizio Is Nothing AndAlso Validita_Inizio.HasValue Then
                StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            End If
            If Not Validita_Fine Is Nothing AndAlso Validita_Fine.HasValue Then
                StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            If EAN <> "" Then
                StrSQL.AppendLine(" ,EAN = '" & Agro_SQL_SaveText(EAN) & "'  ")
            Else
                StrSQL.AppendLine(" ,EAN = '' ")
            End If

            If Barcode <> "" Then
                StrSQL.AppendLine(" ,Barcode = '" & Agro_SQL_SaveText(Barcode) & "'  ")
            Else
                StrSQL.AppendLine(" ,Barcode = '' ")
            End If

            If Ingredienti <> "" Then
                StrSQL.AppendLine(" ,Ingredienti = '" & Agro_SQL_SaveText(Ingredienti) & "'  ")
            Else
                StrSQL.AppendLine(" ,Ingredienti = NULL ")
            End If

            If Not Produzione_Propria Is Nothing AndAlso Produzione_Propria.HasValue Then
                StrSQL.AppendLine(" ,Produzione_Propria = " & Agro_SQL_SaveNum(Produzione_Propria) & "  ")
            Else
                StrSQL.AppendLine(" ,Produzione_Propria = 0 ")
            End If

            'Se 1 salvo l'immagine altrimenti se 0 scrivo NULL
            If SalvaAllegatoImmagine = 1 Then
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    'Salvataggio su DB
                    StrSQL.AppendLine(" ,Immagine = ?")
                Else
                    StrSQL.AppendLine(" ,Immagine = @P1")
                End If

                StrSQL.AppendLine(" ,Immagine_Estensione = '" & Agro_SQL_SaveText(Immagine_Estensione) & "' ")
                StrSQL.AppendLine(" ,Immagine_NomeFile = '" & Agro_SQL_SaveText(Immagine_NomeFile) & "' ")

            Else
                StrSQL.AppendLine(" ,Immagine = NULL")
                StrSQL.AppendLine(" ,Immagine_Estensione = NULL")
                StrSQL.AppendLine(" ,Immagine_NomeFile = NULL")
            End If

            If Not Peso_Netto Is Nothing AndAlso Peso_Netto.HasValue Then
                StrSQL.AppendLine(" ,Peso_Netto = " & Agro_SQL_SaveNum(Peso_Netto) & "  ")
            Else
                StrSQL.AppendLine(" ,Peso_Netto = 0 ")
            End If

            If Not Peso_Sgocciolato Is Nothing AndAlso Peso_Sgocciolato.HasValue Then
                StrSQL.AppendLine(" ,Peso_Sgocciolato = " & Agro_SQL_SaveNum(Peso_Sgocciolato) & "  ")
            Else
                StrSQL.AppendLine(" ,Peso_Sgocciolato = 0 ")
            End If

            If Not Tara Is Nothing AndAlso Tara.HasValue Then
                StrSQL.AppendLine(" ,Tara = " & Agro_SQL_SaveNum(Tara) & "  ")
            Else
                StrSQL.AppendLine(" ,Tara = 0 ")
            End If

            If Not Peso_Egalizzato Is Nothing AndAlso Peso_Egalizzato.HasValue Then
                StrSQL.AppendLine(" ,Peso_Egalizzato = " & Agro_SQL_SaveNum(Peso_Egalizzato) & "  ")
            Else
                StrSQL.AppendLine(" ,Peso_Egalizzato = -1 ")
            End If

            If Not Id_Gruppo_Merce Is Nothing AndAlso Id_Gruppo_Merce.HasValue Then
                StrSQL.AppendLine(" ,Id_Gruppo_Merce = " & Agro_SQL_SaveNum(Id_Gruppo_Merce) & "  ")
            Else
                StrSQL.AppendLine(" ,Id_Gruppo_Merce = 0 ")
            End If

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine(" AND Mat_COd = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            StrSQL.AppendLine(" AND Elem_COd = " & Agro_SQL_SaveNum(Elem_COd) & " ")
            StrSQL.AppendLine(" AND Pro_COd = " & Agro_SQL_SaveNum(Pro_Cod) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If SalvaAllegatoImmagine Then
                '---------------------------------------------
                Dim CmdParameters As New Dictionary(Of String, Byte())
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    CmdParameters.Add("?", Immagine)
                Else
                    CmdParameters.Add("@P1", Immagine)
                End If

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura_ParamVarBinary(objParametri, StrSQL.ToString, NomeRoutine, CmdParameters)
                '--------------------------------------------------------------------------            
            Else
                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------  

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(
                               ByVal Piva_SuperUser As String,
                               ByVal Piva As String,
                               ByVal Mat_Cod As Integer,
                               ByVal Elem_COd As Integer,
                               ByVal Pro_Cod As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Prodotti_Extra_Privata_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM Prodotti_Extra_Privata ")
            StrSQL.AppendLine(" WHERE  1=1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Piva_SuperUser <> "" Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Elem_COd <> 0 Then
                StrSQL.AppendLine(" AND Prodotti_Extra_Privata.Elem_Cod = " & Agro_SQL_SaveNum(Elem_COd) & "   ")
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