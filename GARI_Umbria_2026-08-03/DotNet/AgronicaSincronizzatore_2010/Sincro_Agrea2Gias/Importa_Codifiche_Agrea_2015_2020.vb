Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL

Public Class Importa_Codifiche_Agrea_2015_2020

    Const COL_coltura_cod_AGREA As String = "ID_COLTURA_AGREA"

    Const COL_Varieta_cod_AGREA As String = "COD_VARIETA_AGREA"
    Const COL_Varieta_des_AGREA As String = "DESC_VARIETA"
    Const COL_varieta_cod_AGEA As String = "COD_VARIETA_AGEA"

    Const COL_codiceUso As String = "COD_USO"
    Const COL_uso As String = "DESC_USO"

    Const COL_codiceOccupazione As String = "COD_SUOLO"
    Const COL_occupazione As String = "DESC_SUOLO"

    Const COL_codiceDestinazione As String = "COD_DESTINAZIONE"
    Const COL_destinazione As String = "DESC_DESTINAZIONE"

    Const COL_codiceQualita As String = "COD_QUALITA"
    Const COL_qualita As String = "DESC_QUALITA"

    'Const COL_destinazioneProduttiva As String = "DESTINAZIONE PRODUTTIVA"
    'Const COL_codiceMacrouso As String = "Macrouso - codice"
    'Const COL_Macrouso As String = "Macrouso - descrizione"
    Public Function Importa_Codifiche(ByVal StringaConnessione As String,
                                   ByVal Utente_Username As String,
                                   ByVal Utente_Password As String,
                                                ByVal LogDirectory As String,
                                                ByVal LogFileName As String,
                                                  ByRef Messaggio As String,
                                                  ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Gestione_Codifiche_AGREA(StringaConnessione, Utente_Username, Utente_Password, LogDirectory, LogFileName, Messaggio, LogCodificheMancantiSpecie, LogCodificheMancantiVarieta, objParametri_Server, objParametri_Utenti)

    End Function

    Public Function Gestione_Codifiche_AGREA(ByVal StringaConnessione As String,
                                   ByVal Utente_Username As String,
                                   ByVal Utente_Password As String,
                                                ByVal LogDirectory As String,
                                                ByVal LogFileName As String,
                                                  ByRef Messaggio As String,
                                                  ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim DT_Prodotti As New DataTable
        Dim DT_Varieta As New DataTable
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        objLog.Scrivi_LOG(
            objParametri_Server,
            NomeRoutine,
            "Inizio importazione",
            CustomLOGParams:=customLOGParams)

        'Leggo i File
        Crea_Dt_Prodotti(StringaConnessione, Messaggio, DT_Prodotti)

        objLog.Scrivi_LOG(
            objParametri_Server,
            NomeRoutine,
            Messaggio,
            CustomLOGParams:=customLOGParams)


        objLog.Scrivi_LOG(
            objParametri_Server,
            NomeRoutine,
            "Inizio lettura dei dati.",
            CustomLOGParams:=customLOGParams)

        Dim Coltura_Agrea As String
        Dim codiceUso As String
        Dim uso As String
        Dim codiceOccupazione As String
        Dim occupazione As String
        Dim codiceDestinazione As String
        Dim destinazione As String
        Dim codiceQualita As String
        Dim qualita As String
        Dim macrousoCod As String
        Dim varieta_Agrea As String
        Dim codiceVarieta_Agrea As String
        Dim codiceVarieta_Agea As String


        Dim codiceProdottoAgea As String = "000"
        Dim DescrizioneProdottoAgea As String = occupazione
        Dim veg_Cod = 0
        Dim Cul_cod = 0
        Dim id_cod = 0
        Dim grfi_cod = 0
        Dim grva_cod = 0
        Dim metodoProduzione_Cod = 0
        Dim reg_cod = 0
        Dim Grsp_Cod = 0

        Dim macrouso As String = ""

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objCodificheScrivi 'As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W
        'Dim scope As New TransactionScope()
        Dim objAgeaImport As New Codifica_SpecieVegetali_Agea_2015_2020_W
        Dim objEntiImport As New Codifica_SpecieVegetali_Enti_2015_2020_W
        Dim objEntiReader As New Codifica_SpecieVegetali_Enti_2015_2020_R
        Dim importati As Integer = 0
        Try
            For Each prodotto As DataRow In DT_Prodotti.Rows

                Coltura_Agrea = CInt(insertInteger(prodotto.Item(COL_coltura_cod_AGREA))).ToString("D4")
                codiceUso = CInt(insertInteger(prodotto.Item(COL_codiceUso))).ToString("D3")
                uso = prodotto.Item(COL_uso).ToString()
                codiceOccupazione = CInt(insertInteger(prodotto.Item(COL_codiceOccupazione))).ToString("D3")
                occupazione = prodotto.Item(COL_occupazione).ToString()
                codiceDestinazione = CInt(insertInteger(prodotto.Item(COL_codiceDestinazione))).ToString("D3")
                destinazione = prodotto.Item(COL_destinazione).ToString()
                codiceQualita = CInt(insertInteger(prodotto.Item(COL_codiceQualita))).ToString("D3")
                qualita = prodotto.Item(COL_qualita).ToString()
                codiceVarieta_Agrea = CInt(insertInteger(prodotto.Item(COL_Varieta_cod_AGREA))).ToString("D3")
                varieta_Agrea = prodotto.Item(COL_Varieta_des_AGREA).ToString()
                codiceVarieta_Agea = CInt(insertInteger(prodotto.Item(COL_varieta_cod_AGEA))).ToString("D3")
                macrousoCod = "000"
                macrouso = ""
                codiceProdottoAgea = "000"
                DescrizioneProdottoAgea = occupazione
                veg_Cod = 0
                Cul_cod = 0
                id_cod = 0
                grfi_cod = 0
                grva_cod = 0
                metodoProduzione_Cod = 0
                reg_cod = 0
                Grsp_Cod = 0

                Dim codificheAgea = (From cod As Codifica_SpecieVegetali_Agea_2015_2020
                                         In GiasContext.Codifica_SpecieVegetali_Agea_2015_2020
                                     Where cod.Uso_Cod = codiceUso _
                                         And cod.Occupazione_Cod = codiceOccupazione _
                                         And cod.Destinazione_Cod = codiceDestinazione _
                                         And cod.Qualita_Cod = codiceQualita _
                                         And cod.Cul_Cod_Agea = codiceVarieta_Agea Select cod)

                Dim codA = 0
                If codificheAgea.Count = 0 Then
                    Dim CodificaSpecieAgea = (From cod As Codifica_SpecieVegetali_Agea_2015_2020
                                              In GiasContext.Codifica_SpecieVegetali_Agea_2015_2020
                                              Where cod.Uso_Cod = codiceUso _
                                              And cod.Occupazione_Cod = codiceOccupazione _
                                              And cod.Destinazione_Cod = codiceDestinazione _
                                              And cod.Qualita_Cod = codiceQualita _
                                              And cod.Cul_Cod_Agea = "000" Select cod)
                    If CodificaSpecieAgea.Count <> 0 Then
                        Dim codifica = CodificaSpecieAgea.FirstOrDefault
                        codiceProdottoAgea = codifica.Veg_cod_Agea
                        DescrizioneProdottoAgea = codifica.Veg_Des_Agea
                        veg_Cod = codifica.Veg_cod
                        id_cod = codifica.Id_Cod
                        grfi_cod = codifica.Grfi_Cod
                        grva_cod = codifica.Grva_Cod
                        metodoProduzione_Cod = codifica.Metodo_Produzione_Cod
                        reg_cod = codifica.Reg_Cod
                        Grsp_Cod = codifica.Grsp_Cod
                        macrousoCod = codifica.Macrouso_Cod
                        macrouso = codifica.Macrouso_Des
                    End If
                    objAgeaImport.Scrivi(objParametri_Server, objParametri_Utenti,
                                         codiceProdottoAgea,
                                         codiceVarieta_Agea,
                                         DescrizioneProdottoAgea,
                                         varieta_Agrea,
                                         codiceUso,
                                         uso,
                                         macrousoCod,
                                         macrouso,
                                         codiceOccupazione,
                                         occupazione,
                                         codiceDestinazione,
                                         destinazione,
                                         codiceQualita,
                                         qualita, veg_Cod, Cul_cod, grfi_cod, grva_cod, metodoProduzione_Cod, reg_cod, id_cod, Grsp_Cod, 0,
                                         DateTime.Now, DateTime.Now, DateTime.Now,
                                         "agronica", "agronica",
                                         AGRODATAINIZIO, AGRODATAFINE)
                Else
                    Dim codifica = codificheAgea.First
                    codiceProdottoAgea = codifica.Veg_cod_Agea
                    DescrizioneProdottoAgea = codifica.Veg_Des_Agea
                    veg_Cod = codifica.Veg_cod
                    id_cod = codifica.Id_Cod
                    grfi_cod = codifica.Grfi_Cod
                    grva_cod = codifica.Grva_Cod
                    metodoProduzione_Cod = codifica.Metodo_Produzione_Cod
                    reg_cod = codifica.Reg_Cod
                    Grsp_Cod = codifica.Grsp_Cod
                    macrousoCod = codifica.Macrouso_Cod
                    macrouso = codifica.Macrouso_Des
                End If

                Dim codificheEnti = (From cod In GiasContext.Codifica_SpecieVegetali_Enti_2015_2020 Where cod.Ente_cod = enum_Planning_Fonte.Agrea And
                                                                                                        cod.Veg_Cod_Ente = Coltura_Agrea And
                                                                                                        cod.Cul_Cod_Ente = codiceVarieta_Agrea)

                If codificheEnti.Count = 0 Then
                    objEntiImport.Scrivi(objParametri_Server, objParametri_Utenti, enum_Planning_Fonte.Agrea,
                                         Coltura_Agrea, codiceVarieta_Agrea, occupazione, varieta_Agrea, codiceProdottoAgea, codiceVarieta_Agea,
                                         occupazione, varieta_Agrea, codiceUso, uso, macrousoCod, macrouso, codiceOccupazione, occupazione,
                                         codiceDestinazione, destinazione, codiceQualita, qualita, 0, DateTime.Now, DateTime.Now,
                                         DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
                End If

            Next

            Dim nonCodificateOld As DataTable = objEntiReader.leggiNonCodificati(objParametri_Server, objParametri_Utenti, enum_Planning_Fonte.Agrea)

            Dim Veg_cod_Agea As String
            Dim Cul_cod_Agea As String
            Dim importataOld As Boolean
            For Each codOld As DataRow In nonCodificateOld.Rows
                importataOld = False
                veg_Cod = insertInteger(codOld.Item("Veg_cod"))
                Cul_cod = insertInteger(codOld.Item("Cul_cod"))
                id_cod = insertInteger(codOld.Item("Id_cod"))
                grfi_cod = insertInteger(codOld.Item("Grfi_cod"))
                grva_cod = insertInteger(codOld.Item("Grva_cod"))
                reg_cod = insertInteger(codOld.Item("Reg_cod"))
                Grsp_Cod = insertInteger(codOld.Item("Grsp_Cod"))
                Veg_cod_Agea = codOld.Item("Veg_Cod_Agea")
                Cul_cod_Agea = codOld.Item("Cul_Cod_Agea")
                If Veg_cod_Agea <> "" AndAlso Cul_cod_Agea <> "" Then
                    Dim codificheAgea = (From codA In GiasContext.Codifica_SpecieVegetali_Agea_2015_2020
                                         Where codA.Veg_cod_Agea = Veg_cod_Agea And
                                            codA.Cul_Cod_Agea = Cul_cod And
                                            codA.Veg_cod <> 0 And
                                            codA.Cul_Cod <> 0)
                    If codificheAgea.Count <> 0 Then
                        Dim codificaAgea = codificheAgea.First
                        objEntiImport.Scrivi(objParametri_Server, objParametri_Utenti, enum_Planning_Fonte.Agrea,
                                         codOld.Item("Veg_Cod_Agrea"), codOld.Item("Cul_Cod_Agrea"), codOld.Item("Veg_Des_Agrea"),
                                         codOld.Item("Cul_Des_Agrea"), codificaAgea.Veg_cod_Agea, codificaAgea.Cul_Cod_Agea, codificaAgea.Veg_Des_Agea,
                                         codificaAgea.Cul_Des_Agea, codificaAgea.Uso_Cod, codificaAgea.Uso_Des, codificaAgea.Macrouso_Cod,
                                         codificaAgea.Macrouso_Des, codificaAgea.Occupazione_Cod, codificaAgea.Occupazione_Des, codificaAgea.Destinazione_Cod,
                                         codificaAgea.Destinazione_Des, codificaAgea.Qualita_Cod, codificaAgea.Qualita_Des, 0, DateTime.Now, DateTime.Now,
                                         DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
                        importataOld = True
                    End If
                End If
                If Not importataOld Then
                    Dim codificheAgea = (From codA In GiasContext.Codifica_SpecieVegetali_Agea_2015_2020
                                         Where codA.Veg_cod = veg_Cod And
                                             codA.Cul_Cod = Cul_cod And
                                             codA.Grfi_Cod = grfi_cod And
                                             codA.Id_Cod = id_cod And
                                             codA.Grva_Cod = grva_cod And
                                             codA.Grsp_Cod = Grsp_Cod And
                                             codA.Reg_Cod = reg_cod)
                    If codificheAgea.Count > 0 Then
                        Dim codificaAgea = codificheAgea.First
                        objEntiImport.Scrivi(objParametri_Server, objParametri_Utenti, enum_Planning_Fonte.Agrea,
                                         codOld.Item("Veg_Cod_Agrea"), codOld.Item("Cul_Cod_Agrea"), codOld.Item("Veg_Des_Agrea"), codOld.Item("Cul_Des_Agrea"), codificaAgea.Veg_cod_Agea,
                                         codificaAgea.Cul_Cod_Agea, codificaAgea.Veg_Des_Agea, codificaAgea.Cul_Des_Agea, codificaAgea.Uso_Cod, codificaAgea.Uso_Des,
                                         codificaAgea.Macrouso_Cod, codificaAgea.Macrouso_Des, codificaAgea.Occupazione_Cod, codificaAgea.Occupazione_Des,
                                         codificaAgea.Destinazione_Cod, codificaAgea.Destinazione_Des, codificaAgea.Qualita_Cod, codificaAgea.Qualita_Des, 0, DateTime.Now, DateTime.Now,
                                         DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
                    Else
                        Dim a = 0
                    End If
                End If
            Next

            Dim nonCodificateAgea = objEntiReader.leggiNonCodificati_in_AGEA(objParametri_Server, objParametri_Utenti, enum_Planning_Fonte.Agrea)
            For Each codOld As DataRow In nonCodificateAgea.Rows
                veg_Cod = insertInteger(codOld.Item("Veg_cod"))
                Cul_cod = insertInteger(codOld.Item("Cul_cod"))
                id_cod = insertInteger(codOld.Item("Id_cod"))
                grfi_cod = insertInteger(codOld.Item("Grfi_cod"))
                grva_cod = insertInteger(codOld.Item("Grva_cod"))
                reg_cod = insertInteger(codOld.Item("Reg_cod"))
                Grsp_Cod = insertInteger(codOld.Item("Grsp_Cod"))
                metodoProduzione_Cod = insertInteger(codOld.Item("Metodo_Produzione_Cod"))
                objAgeaImport.AggiornaParametriGias(objParametri_Server, codOld.Item("Veg_cod_Agea_New"), codOld.Item("Cul_Cod_Agea_New"), codOld.Item("Uso_Cod"),
                                                    codOld.Item("Macrouso_Cod"), codOld.Item("Occupazione_Cod"), codOld.Item("Destinazione_Cod"), codOld.Item("Qualita_Cod"), veg_Cod,
                                                    Cul_cod, grfi_cod, grva_cod, metodoProduzione_Cod, reg_cod, id_cod, Grsp_Cod, DateTime.Now, "agronica", AGRODATAINIZIO, AGRODATAFINE)
            Next

            Messaggio = "Importazione terminata " & importati
        Catch ex As Exception
            Messaggio = "Importazione terminata " & ex.Message
        Finally
            'scope.Complete()
            'scope.Dispose()
            GiasContext.Database.Connection.Close()
            GiasContext = Nothing
        End Try
    End Function


    Public Function Gestione_Codifiche_ENTI(ByVal StringaConnessione As String,
                                   ByVal Utente_Username As String,
                                   ByVal Utente_Password As String,
                                                ByVal LogDirectory As String,
                                                ByVal LogFileName As String,
                                                  ByRef Messaggio As String,
                                                  ByRef LogCodificheMancantiSpecie As String,
                                                  ByRef LogCodificheMancantiVarieta As String,
                                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

    End Function
    Private Function insertInteger(value As Object) As String
        Dim returnValue As String = "0"
        If Not IsDBNull(value) Then
            If CStr(value) <> "" Then
                Return CStr(CInt(value))
            End If
        End If
        Return returnValue
    End Function

    Private Sub Crea_Dt_Prodotti(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            'Dim MyConnection As New System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; data source='" & File_Impianti & "'; Extended Properties=Excel 8.0;")
            'Dim MyConnection As New System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; data source='" & File_Impianti & "'; Extended Properties=Excel 8.0;HDR=Yes;IMEX=1 ")
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New System.Data.OleDb.OleDbDataAdapter("select * from [" + firstSheet + "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()
            Dt = ds.Tables(0)
        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try
    End Sub

    Private Sub Crea_Dt_Varieta(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            'Dim MyConnection As New System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; data source='" & File_Impianti & "'; Extended Properties=Excel 8.0;")
            'Dim MyConnection As New System.Data.OleDb.OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0; data source='" & File_Impianti & "'; Extended Properties=Excel 8.0;HDR=Yes;IMEX=1 ")
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = "Varieta$" 'dtSheet.Rows(1)("TABLE_NAME").ToString()
            Dim da As New System.Data.OleDb.OleDbDataAdapter("select * from [" + firstSheet + "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)

        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try


    End Sub
End Class
