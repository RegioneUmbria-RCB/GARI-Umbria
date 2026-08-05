Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO


Public Class Importa_Codifiche_Agea_2015_2020

    Const COL_codiceProdotto As String = "CODICE PRODOTTO"
    Const COL_varieta As String = "VARIETA'-ALLEGATO 2"
    Const COL_codiceUso As String = "CODICE USO"
    Const COL_destinazioneProduttiva As String = "DESTINAZIONE PRODUTTIVA"
    Const COL_uso As String = "USO "
    Const COL_codiceMacrouso As String = "Macrouso - codice"
    Const COL_Macrouso As String = "Macrouso - descrizione"
    Const COL_codiceOccupazione As String = "OCCUPAZIONE DEL SUOLO -CODICE"
    Const COL_occupazione As String = "OCCUPAZIONE DEL SUOLO -DESCRIZIONE"
    Const COL_codiceDestinazione As String = "DESTINAZIONE - codice"
    Const COL_destinazione As String = "DESTINAZIONE - descrizione"
    Const COL_codiceQualita As String = "QUALITA'-CODICE"
    Const COL_qualita As String = "QUALITA'-DESCRIZIONE"

    Public Sub Importa_Codifiche(ByVal StringaConnessione As String,
                                 ByVal Utente_Username As String,
                                 ByVal Utente_Password As String,
                                 ByVal LogDirectory As String,
                                 ByVal LogFileName As String,
                                 ByRef Messaggio As String,
                                 ByRef LogCodificheMancantiSpecie As String,
                                 ByRef LogCodificheMancantiVarieta As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Gestione_Codifiche_AGEA(StringaConnessione, Utente_Username, Utente_Password, LogDirectory, LogFileName, Messaggio, LogCodificheMancantiSpecie, LogCodificheMancantiVarieta, objParametri_Server, objParametri_Utenti)
        Gestione_Codifiche_ENTI(StringaConnessione, Utente_Username, Utente_Password, LogDirectory, LogFileName, Messaggio, LogCodificheMancantiSpecie, LogCodificheMancantiVarieta, objParametri_Server, objParametri_Utenti)

    End Sub

    Public Sub Gestione_Codifiche_AGEA(ByVal StringaConnessione As String,
                                       ByVal Utente_Username As String,
                                       ByVal Utente_Password As String,
                                       ByVal LogDirectory As String,
                                       ByVal LogFileName As String,
                                       ByRef Messaggio As String,
                                       ByRef LogCodificheMancantiSpecie As String,
                                       ByRef LogCodificheMancantiVarieta As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim DT_Prodotti As New DataTable
        Dim DT_Varieta As New DataTable
        Dim NomeRoutine As String = "Importa_Dati"

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        objLog.Scrivi_LOG(objParametri_Server,
                   NomeRoutine,
                   "Inizio importazione",
                   CustomLOGParams:=customLOGParams)

        'Leggo i File
        Crea_Dt_Prodotti(StringaConnessione, Messaggio, DT_Prodotti)
        Crea_Dt_Varieta(StringaConnessione, Messaggio, DT_Varieta)

        objLog.Scrivi_LOG(objParametri_Server,
                         NomeRoutine,
                         Messaggio,
                         CustomLOGParams:=customLOGParams)


        objLog.Scrivi_LOG(objParametri_Server,
                          NomeRoutine,
                          "Inizio lettura dei dati.",
                          CustomLOGParams:=customLOGParams)

        Dim codiceProdotto As String
        Dim codiceUso As String
        Dim destinazioneProduttiva As String
        Dim uso As String
        Dim codiceMacrouso As String
        Dim Macrouso As String
        Dim codiceOccupazione As String
        Dim occupazione As String
        Dim codiceDestinazione As String
        Dim destinazione As String
        Dim codiceQualita As String
        Dim qualita As String

        Dim varieta As String = ""
        Dim codiceVarieta As String
        Dim veg_cod As Integer
        Dim cul_cod As Integer

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim objCodificheScrivi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W
        Dim scope As New TransactionScope()
        Dim importati As Integer = 0
        Try
            DT_Prodotti = DT_Prodotti.Select(" [" & COL_codiceProdotto & "] <> '' ").CopyToDataTable

            For Each prodotto As DataRow In DT_Prodotti.Rows
                codiceProdotto = CInt(insertInteger(prodotto.Item(COL_codiceProdotto))).ToString("D3")
                codiceUso = CInt(insertInteger(prodotto.Item(COL_codiceUso))).ToString("D3")
                destinazioneProduttiva = prodotto.Item(COL_destinazioneProduttiva).ToString()
                uso = prodotto.Item(COL_uso).ToString()
                codiceMacrouso = CInt(insertInteger(prodotto.Item(COL_codiceMacrouso))).ToString("D3")
                Macrouso = prodotto.Item(COL_Macrouso).ToString()
                codiceOccupazione = CInt(insertInteger(prodotto.Item(COL_codiceOccupazione))).ToString("D3")
                occupazione = prodotto.Item(COL_occupazione).ToString()
                codiceDestinazione = CInt(insertInteger(prodotto.Item(COL_codiceDestinazione))).ToString("D3")
                destinazione = prodotto.Item(COL_destinazione).ToString()
                codiceQualita = CInt(insertInteger(prodotto.Item(COL_codiceQualita))).ToString("D3")
                qualita = prodotto.Item(COL_qualita).ToString()
                Dim codificheAgeaOld = (From codA As Codifica_SpecieVegetali_Agea In GiasContext.Codifica_SpecieVegetali_Agea Where codA.Veg_Cod_Agea = codiceProdotto)
                If codificheAgeaOld.Count > 0 Then
                    For Each codificaOld As Codifica_SpecieVegetali_Agea In codificheAgeaOld
                        varieta = codificaOld.Cul_Des_Agea
                        codiceVarieta = codificaOld.Cul_Cod_Agea
                        veg_cod = codificaOld.Veg_cod
                        cul_cod = codificaOld.Cul_cod

                        Dim codificaNew = (From codN As Codifica_SpecieVegetali_Agea_2015_2020 In
                                                       GiasContext.Codifica_SpecieVegetali_Agea_2015_2020 Where
                                                        codN.Veg_cod_Agea = codiceProdotto AndAlso
                                                        codN.Cul_Cod_Agea = codiceVarieta AndAlso
                                                        codN.Uso_Cod = codiceUso AndAlso
                                                        codN.Macrouso_Cod = codiceMacrouso AndAlso
                                                        codN.Occupazione_Cod = codiceOccupazione AndAlso
                                                        codN.Destinazione_Cod = codiceDestinazione AndAlso
                                                        codN.Qualita_Cod = codiceQualita)
                        If codificaNew.Count > 1 Then
                            'PROBLEMA
                            Dim msg As String = "CODIFICHE MULTIPLE"
                            objLog.Scrivi_LOG(objParametri_Server,
                                     NomeRoutine,
                                     msg,
                                     CustomLOGParams:=customLOGParams)
                        ElseIf codificaNew.Count = 1 Then

                        Else
                            If destinazioneProduttiva = "LUPOLINA - DA FORAGGIO" Then
                                Dim i = 0
                            End If
                            objCodificheScrivi.Scrivi(objParametri_Server, objParametri_Utenti, codiceProdotto, codiceVarieta, destinazioneProduttiva, varieta, codiceUso, uso, codiceMacrouso,
                                                      Macrouso, codiceOccupazione, occupazione, codiceDestinazione, destinazione, codiceQualita, qualita, veg_cod, cul_cod, codificaOld.Grfi_cod,
                                                      codificaOld.Grva_cod, codificaOld.Metodo_Produzione_cod, codificaOld.Reg_cod, codificaOld.Id_cod, codificaOld.Grsp_Cod,
                                                      0, DateTime.Now, DateTime.Now, DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
                            importati += 1
                        End If

                    Next
                Else
                    Dim codificaNew = (From codN As Codifica_SpecieVegetali_Agea_2015_2020 In
                                                       GiasContext.Codifica_SpecieVegetali_Agea_2015_2020 Where
                                                        codN.Veg_cod_Agea = codiceProdotto AndAlso
                                                        codN.Cul_Cod_Agea = "000" AndAlso
                                                        codN.Uso_Cod = codiceUso AndAlso
                                                        codN.Macrouso_Cod = codiceMacrouso AndAlso
                                                        codN.Occupazione_Cod = codiceOccupazione AndAlso
                                                        codN.Destinazione_Cod = codiceDestinazione AndAlso
                                                        codN.Qualita_Cod = codiceQualita)
                    If codificaNew.Count = 0 Then
                        If destinazioneProduttiva = "LUPOLINA - DA FORAGGIO" Then
                            Dim i = 0
                        End If
                        objCodificheScrivi.Scrivi(objParametri_Server, objParametri_Utenti, codiceProdotto, "000", destinazioneProduttiva, varieta, codiceUso, uso, codiceMacrouso,
                                                      Macrouso, codiceOccupazione, occupazione, codiceDestinazione, destinazione, codiceQualita, qualita, 0, 0, 0,
                                                      0, 0, 0, 0, 0,
                                                      0, DateTime.Now, DateTime.Now, DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
                        importati += 1
                    End If
                End If
            Next
            Dim codificheNewID = GiasContext.Codifica_SpecieVegetali_Agea_2015_2020.Select(Function(x) x.Veg_cod_Agea).ToArray
            Dim codificheOld = GiasContext.Codifica_SpecieVegetali_Agea.Where(Function(o) Not codificheNewID.Contains(o.Veg_Cod_Agea)).ToList
            For Each codifica In codificheOld
                objCodificheScrivi.Scrivi(objParametri_Server, objParametri_Utenti, codifica.Veg_Cod_Agea, codifica.Cul_Cod_Agea, codifica.Veg_Des_Agea, codifica.Cul_Des_Agea,
                                          "000", "", "000", "", "000", "", "000", "", "000", "", codifica.Veg_cod, codifica.Cul_cod, codifica.Grfi_cod, codifica.Grva_cod, codifica.Metodo_Produzione_cod,
                                          codifica.Reg_cod, codifica.Id_cod, codifica.Grsp_Cod, 0, DateTime.Now, DateTime.Now, DateTime.Now, "agronica", "agronica", AGRODATAINIZIO, AGRODATAFINE)
            Next

            Messaggio = "Importazione terminata " & importati
        Catch ex As Exception
            Messaggio = "Importazione terminata " & ex.Message
        Finally
            scope.Complete()
            scope.Dispose()
            GiasContext.Database.Connection.Close()
            GiasContext = Nothing
        End Try
    End Sub

    Public Sub Modifica_VegCod_Id_Cod(ByVal StringaConnessione As String,
                                      ByVal Utente_Username As String,
                                      ByVal Utente_Password As String,
                                      ByVal LogDirectory As String,
                                      ByVal LogFileName As String,
                                      ByRef Messaggio As String,
                                      ByRef LogCodificheMancantiSpecie As String,
                                      ByRef LogCodificheMancantiVarieta As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()

        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Grva_Cod As Integer
        Dim Metodo_Produzione_Cod As Integer
        Dim Reg_cod As Integer
        Dim Id_Cod As Integer
        Dim Grsp_Cod As Integer
        'Dim ObjAgea As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W
        Try

            Dim DT_Distinct_Des = GiasContext.Codifica_SpecieVegetali_Agea_2015_2020.Where(Function(x) x.Id_Cod = 0 And x.Veg_cod = 0 And x.Cul_Cod = 0).Select(Function(z) z.Veg_Des_Agea).Distinct.ToList
            For Each el In DT_Distinct_Des
                Veg_Cod = 0
                Cul_Cod = 0
                Grfi_Cod = 0
                Grva_Cod = 0
                Metodo_Produzione_Cod = 0
                Reg_cod = 0
                Id_Cod = 0
                Grsp_Cod = 0
                Dim CodificheCorrette = (From x In GiasContext.Codifica_SpecieVegetali_Agea_2015_2020 Where x.Veg_Des_Agea = el And (x.Id_Cod <> 0 Or x.Veg_cod <> 0))
                If CodificheCorrette.Count > 0 Then
                    Dim CodificaCorretta = CodificheCorrette.First
                    Veg_Cod = CodificaCorretta.Veg_cod
                    Cul_Cod = CodificaCorretta.Cul_Cod
                    Grfi_Cod = CodificaCorretta.Grfi_Cod
                    Grva_Cod = CodificaCorretta.Grva_Cod
                    Metodo_Produzione_Cod = CodificaCorretta.Metodo_Produzione_Cod
                    Reg_cod = CodificaCorretta.Reg_Cod
                    Id_Cod = CodificaCorretta.Id_Cod
                    Grsp_Cod = CodificaCorretta.Grsp_Cod
                    'ObjAgea.AggiornaNonCodificate_Des(objParametri_Server, el, Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Metodo_Produzione_Cod, Reg_cod, Id_Cod, Grsp_Cod, DateTime.Now)
                End If
            Next

            Messaggio = "Importazione terminata "

        Catch ex As Exception
            Messaggio = "Importazione terminata con errore:" & ex.Message
        Finally
            GiasContext.Database.Connection.Close()
            GiasContext = Nothing
        End Try

    End Sub

    Public Sub Gestione_Codifiche_ENTI(ByVal StringaConnessione As String,
                                       ByVal Utente_Username As String,
                                       ByVal Utente_Password As String,
                                       ByVal LogDirectory As String,
                                       ByVal LogFileName As String,
                                       ByRef Messaggio As String,
                                       ByRef LogCodificheMancantiSpecie As String,
                                       ByRef LogCodificheMancantiVarieta As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

    End Sub

    Private Function insertInteger(value As Object) As String
        Dim returnValue As String = "0"
        If Not IsDBNull(value) Then
            Return CStr(CInt(value))
        End If
        Return returnValue
    End Function

    Private Sub Crea_Dt_Prodotti(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = "Prodotti$" 'dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
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
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = "Varieta$" 'dtSheet.Rows(1)("TABLE_NAME").ToString()
            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)

        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try


    End Sub

End Class
