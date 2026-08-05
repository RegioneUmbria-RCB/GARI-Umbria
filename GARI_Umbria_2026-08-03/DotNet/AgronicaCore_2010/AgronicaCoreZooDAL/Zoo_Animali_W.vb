

Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Zoo_Animali_W
    Inherits AgronicaCoreDataProvider.DataProvider




    ''============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Cod_Progetto As Int32,
    '                        ByVal Matricola As String,
    '                        ByVal Gen_Cod As Int32,
    '                        ByVal Spe_Cod As Int32,
    '                        ByVal Raz_Cod As Int32,
    '                        ByVal Ipro_Cod As Int32,
    '                        ByVal Nome As String,
    '                        ByVal Collare As String,
    '                        ByVal Nome_AIA As String,
    '                        ByVal Matricola_AIA As String,
    '                        ByVal Dat_Nascita As Date,
    '                        ByVal Prov_Nascita As String,
    '                        ByVal Stato_Nascita As String,
    '                        ByVal AUA_AZI_Nascita As String,
    '                        ByVal AUSL_AZI_Nascita As String,
    '                        ByVal Sesso As String,
    '                        ByVal Mat_Padre As String,
    '                        ByVal Mat_Madre As String,
    '                        ByVal CF_Proprietario As String,
    '                        ByVal CF_Detentore As String,
    '                        ByVal Presente As Int16,
    '                        ByVal Cat_Cod As Int32,
    '                        ByVal Peso As Decimal,
    '                        ByVal Data_Pesa As Date,
    '                        ByVal Metodo_Produzione As Int16,
    '                        ByVal Regolamento_Cod As Int16,
    '                        ByVal Conversione_Inizio As Date,
    '                        ByVal Conversione_Fine As Date,
    '                        ByVal Chk_Batteria As Int16,
    '                            ByVal UserName_Creazione As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" INSERT INTO Zoo_Animali ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          PIVA, Sa_Cod, Cod_Progetto, Matricola, ")
    '        StrSQL.Append("          GEN_COD, SPE_COD, RAZ_COD, IPRO_COD, Nome, ")
    '        StrSQL.Append("          Collare, NOME_AIA, MATRICOLA_AIA, DAT_NASCITA, ")
    '        StrSQL.Append("          PROV_NASCITA, STATO_NASCITA, AUA_AZI_NASCITA, AUSL_AZI_NASCITA, ")
    '        StrSQL.Append("          Sesso, MAT_PADRE, MAT_MADRE, CF_PROPRIETARIO, CF_DETENTORE, PRESENTE, ")
    '        StrSQL.Append("          Cat_Cod, Peso, Data_Pesa, ")
    '        StrSQL.Append("          Metodo_Produzione, Regolamento_Cod, Conversione_Inizio, Conversione_Fine, ")
    '        StrSQL.Append("          Chk_Batteria, ")

    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Progetto) & " ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Matricola) & " '  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Collare) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome_AIA) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Matricola_AIA) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Nascita) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Prov_Nascita)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Stato_Nascita) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(AUA_AZI_Nascita) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(AUSL_AZI_Nascita) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Mat_Padre)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Mat_Madre)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Presente) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Pesa) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Metodo_Produzione) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Fine) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Chk_Batteria) & "  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function





    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Cod_Progetto As Int32, _
                            ByVal Matricola As String, _
                            ByVal Gen_Cod As Int32, _
                            ByVal Spe_Cod As Int32, _
                            ByVal Raz_Cod As Int32, _
                            ByVal Ipro_Cod As Int32, _
                            ByVal Nome As String, _
                            ByVal Collare As String, _
                            ByVal Nome_AIA As String, _
                            ByVal Matricola_AIA As String, _
                            ByVal Dat_Nascita As Date, _
                            ByVal Prov_Nascita As String, _
                            ByVal Stato_Nascita As String, _
                            ByVal AUA_AZI_Nascita As String, _
                            ByVal AUSL_AZI_Nascita As String, _
                            ByVal Sesso As String, _
                            ByVal Mat_Padre As String, _
                            ByVal Mat_Madre As String, _
                            ByVal CF_Proprietario As String, _
                            ByVal CF_Detentore As String, _
                            ByVal Presente As Int16, _
                            ByVal Cat_Cod As Int32, _
                            ByVal Peso As Decimal, _
                            ByVal Data_Pesa As Date, _
                            ByVal Metodo_Produzione As Int16, _
                            ByVal Regolamento_Cod As Int16, _
                            ByVal Conversione_Inizio As Date, _
                            ByVal Conversione_Fine As Date, _
                            ByVal Chk_Batteria As Int16, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Zoo_Animali ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          PIVA, Sa_Cod, Cod_Progetto, Matricola, ")
            StrSQL.Append("          GEN_COD, SPE_COD, RAZ_COD, IPRO_COD, Nome, ")
            StrSQL.Append("          Collare, NOME_AIA, MATRICOLA_AIA, DAT_NASCITA, ")
            StrSQL.Append("          PROV_NASCITA, STATO_NASCITA, AUA_AZI_NASCITA, AUSL_AZI_NASCITA, ")
            StrSQL.Append("          Sesso, MAT_PADRE, MAT_MADRE, CF_PROPRIETARIO, CF_DETENTORE, PRESENTE, ")
            StrSQL.Append("          Cat_Cod, Peso, Data_Pesa, ")
            StrSQL.Append("          Metodo_Produzione, Regolamento_Cod, Conversione_Inizio, Conversione_Fine, ")
            StrSQL.Append("          Chk_Batteria, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Progetto) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Matricola) & " '  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Collare) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nome_AIA) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Matricola_AIA) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Nascita) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Prov_Nascita)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Stato_Nascita) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(AUA_AZI_Nascita) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(AUSL_AZI_Nascita) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Mat_Padre)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Mat_Madre)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Presente) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Pesa) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Metodo_Produzione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Conversione_Fine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Chk_Batteria) & "  ")

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
    'Public Function Modifica(
    '                    ByVal Piva As String,
    '                    ByVal Sa_Cod As Int32,
    '                    ByVal Cod_Progetto As Int32,
    '                    ByVal Matricola As String,
    '                    ByVal Gen_Cod As Int32,
    '                    ByVal Spe_Cod As Int32,
    '                    ByVal Raz_Cod As Int32,
    '                    ByVal Ipro_Cod As Int32,
    '                    ByVal Nome As String,
    '                    ByVal Collare As String,
    '                    ByVal Nome_AIA As String,
    '                    ByVal Matricola_AIA As String,
    '                    ByVal Dat_Nascita As Date,
    '                    ByVal Prov_Nascita As String,
    '                    ByVal Stato_Nascita As String,
    '                    ByVal AUA_AZI_Nascita As String,
    '                    ByVal AUSL_AZI_Nascita As String,
    '                    ByVal Sesso As String,
    '                    ByVal Mat_Padre As String,
    '                    ByVal Mat_Madre As String,
    '                    ByVal CF_Proprietario As String,
    '                    ByVal CF_Detentore As String,
    '                    ByVal Presente As Integer,
    '                    ByVal Cat_Cod As Int32,
    '                    ByVal Peso As Decimal,
    '                    ByVal Data_Pesa As Date,
    '                    ByVal Metodo_Produzione As Integer,
    '                    ByVal Regolamento_Cod As Integer,
    '                    ByVal Conversione_Inizio As Date,
    '                    ByVal Conversione_Fine As Date,
    '                    ByVal Chk_Batteria As Int16,
    '                            ByVal UserName_Modifica As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        If Cod_Progetto = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Progetto obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE Zoo_Animali SET ")
    '        StrSQL.Append("    MATRICOLA         =  '" & Agro_SQL_SaveText(Trim(Matricola)) & "'  ")
    '        StrSQL.Append("   ,GEN_COD           =  " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
    '        StrSQL.Append("   ,SPE_COD           =  " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
    '        StrSQL.Append("   ,RAZ_COD           =  " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
    '        StrSQL.Append("   ,IPRO_COD          =  " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
    '        StrSQL.Append("   ,Nome              = '" & Agro_SQL_SaveText(Nome) & "'  ")
    '        StrSQL.Append("   ,Collare           = '" & Agro_SQL_SaveText(Collare) & "'  ")
    '        StrSQL.Append("   ,NOME_AIA          = '" & Agro_SQL_SaveText(Nome_AIA) & "'  ")
    '        StrSQL.Append("   ,MATRICOLA_AIA     = '" & Agro_SQL_SaveText(Trim(Matricola_AIA)) & "'  ")
    '        StrSQL.Append("   ,DAT_NASCITA       =  " & Agro_SQL_SaveDate(Dat_Nascita))
    '        StrSQL.Append("   ,PROV_NASCITA      = '" & Agro_SQL_SaveText(Prov_Nascita) & "'  ")
    '        StrSQL.Append("   ,STATO_NASCITA     = '" & Agro_SQL_SaveText(Stato_Nascita) & "'  ")
    '        StrSQL.Append("   ,AUA_AZI_NASCITA   = '" & Agro_SQL_SaveText(Trim(AUA_AZI_Nascita)) & "'  ")
    '        StrSQL.Append("   ,AUSL_AZI_NASCITA  = '" & Agro_SQL_SaveText(Trim(AUSL_AZI_Nascita)) & "'  ")
    '        StrSQL.Append("   ,Sesso             = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
    '        StrSQL.Append("   ,MAT_PADRE         = '" & Agro_SQL_SaveText(Trim(Mat_Padre)) & "' ")
    '        StrSQL.Append("   ,MAT_MADRE         = '" & Agro_SQL_SaveText(Trim(Mat_Madre)) & "' ")
    '        StrSQL.Append("   ,CF_PROPRIETARIO   = '" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "' ")
    '        StrSQL.Append("   ,CF_DETENTORE      = '" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "' ")
    '        StrSQL.Append("   ,PRESENTE          =  " & Agro_SQL_SaveNum(Presente) & " ")
    '        StrSQL.Append("   ,Cat_Cod           =  " & Agro_SQL_SaveNum(Cat_Cod) & " ")
    '        StrSQL.Append("   ,Peso              =  " & Agro_SQL_SaveNum(Peso) & " ")
    '        StrSQL.Append("   ,Data_Pesa         =  " & Agro_SQL_SaveDate(Data_Pesa))
    '        StrSQL.Append("   ,Metodo_Produzione =  " & Agro_SQL_SaveNum(Metodo_Produzione) & " ")
    '        StrSQL.Append("   ,Regolamento_cod   =  " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
    '        StrSQL.Append("   ,Conversione_Inizio   =  " & Agro_SQL_SaveDate(Conversione_Inizio))
    '        StrSQL.Append("   ,Conversione_Fine     =  " & Agro_SQL_SaveDate(Conversione_Fine))
    '        StrSQL.Append("   ,Chk_Batteria     =  " & Agro_SQL_SaveNum(Chk_Batteria))

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        StrSQL.Append(" WHERE Cod_Progetto   =  " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")


    '        'Nota: la bestia potrebbe avere cambiato di proprietario

    '        'If Piva <> "" Then
    '        '   StrSQL.Append(" AND Piva = " & Agro_SQL_SaveText(Piva) & "'  ")
    '        'End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function




    Public Function Modifica( _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal Cod_Progetto As Int32, _
                        ByVal Matricola As String, _
                        ByVal Gen_Cod As Int32, _
                        ByVal Spe_Cod As Int32, _
                        ByVal Raz_Cod As Int32, _
                        ByVal Ipro_Cod As Int32, _
                        ByVal Nome As String, _
                        ByVal Collare As String, _
                        ByVal Nome_AIA As String, _
                        ByVal Matricola_AIA As String, _
                        ByVal Dat_Nascita As Date, _
                        ByVal Prov_Nascita As String, _
                        ByVal Stato_Nascita As String, _
                        ByVal AUA_AZI_Nascita As String, _
                        ByVal AUSL_AZI_Nascita As String, _
                        ByVal Sesso As String, _
                        ByVal Mat_Padre As String, _
                        ByVal Mat_Madre As String, _
                        ByVal CF_Proprietario As String, _
                        ByVal CF_Detentore As String, _
                        ByVal Presente As Integer, _
                        ByVal Cat_Cod As Int32, _
                        ByVal Peso As Decimal, _
                        ByVal Data_Pesa As Date, _
                        ByVal Metodo_Produzione As Integer, _
                        ByVal Regolamento_Cod As Integer, _
                        ByVal Conversione_Inizio As Date, _
                        ByVal Conversione_Fine As Date, _
                        ByVal Chk_Batteria As Int16, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Cod_Progetto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Progetto obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Zoo_Animali SET ")
            StrSQL.Append("    MATRICOLA         =  '" & Agro_SQL_SaveText(Trim(Matricola)) & "'  ")
            StrSQL.Append("   ,GEN_COD           =  " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("   ,SPE_COD           =  " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("   ,RAZ_COD           =  " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            StrSQL.Append("   ,IPRO_COD          =  " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("   ,Nome              = '" & Agro_SQL_SaveText(Nome) & "'  ")
            StrSQL.Append("   ,Collare           = '" & Agro_SQL_SaveText(Collare) & "'  ")
            StrSQL.Append("   ,NOME_AIA          = '" & Agro_SQL_SaveText(Nome_AIA) & "'  ")
            StrSQL.Append("   ,MATRICOLA_AIA     = '" & Agro_SQL_SaveText(Trim(Matricola_AIA)) & "'  ")
            StrSQL.Append("   ,DAT_NASCITA       =  " & Agro_SQL_SaveDate(Dat_Nascita))
            StrSQL.Append("   ,PROV_NASCITA      = '" & Agro_SQL_SaveText(Prov_Nascita) & "'  ")
            StrSQL.Append("   ,STATO_NASCITA     = '" & Agro_SQL_SaveText(Stato_Nascita) & "'  ")
            StrSQL.Append("   ,AUA_AZI_NASCITA   = '" & Agro_SQL_SaveText(Trim(AUA_AZI_Nascita)) & "'  ")
            StrSQL.Append("   ,AUSL_AZI_NASCITA  = '" & Agro_SQL_SaveText(Trim(AUSL_AZI_Nascita)) & "'  ")
            StrSQL.Append("   ,Sesso             = '" & Agro_SQL_SaveText(Trim(Sesso)) & "'  ")
            StrSQL.Append("   ,MAT_PADRE         = '" & Agro_SQL_SaveText(Trim(Mat_Padre)) & "' ")
            StrSQL.Append("   ,MAT_MADRE         = '" & Agro_SQL_SaveText(Trim(Mat_Madre)) & "' ")
            StrSQL.Append("   ,CF_PROPRIETARIO   = '" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "' ")
            StrSQL.Append("   ,CF_DETENTORE      = '" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "' ")
            StrSQL.Append("   ,PRESENTE          =  " & Agro_SQL_SaveNum(Presente) & " ")
            StrSQL.Append("   ,Cat_Cod           =  " & Agro_SQL_SaveNum(Cat_Cod) & " ")
            StrSQL.Append("   ,Peso              =  " & Agro_SQL_SaveNum(Peso) & " ")
            StrSQL.Append("   ,Data_Pesa         =  " & Agro_SQL_SaveDate(Data_Pesa))
            StrSQL.Append("   ,Metodo_Produzione =  " & Agro_SQL_SaveNum(Metodo_Produzione) & " ")
            StrSQL.Append("   ,Regolamento_cod   =  " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.Append("   ,Conversione_Inizio   =  " & Agro_SQL_SaveDate(Conversione_Inizio))
            StrSQL.Append("   ,Conversione_Fine     =  " & Agro_SQL_SaveDate(Conversione_Fine))
            StrSQL.Append("   ,Chk_Batteria     =  " & Agro_SQL_SaveNum(Chk_Batteria))

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Cod_Progetto   =  " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")


            'Nota: la bestia potrebbe avere cambiato di proprietario

            'If Piva <> "" Then
            '   StrSQL.Append(" AND Piva = " & Agro_SQL_SaveText(Piva) & "'  ")
            'End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If


            '----------------------------------------------------------------------
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
    'Public Function Cancella(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Cod_Progetto As Int32,
    '                            ByVal UserName_Modifica As String,
    '                            ByVal FlagCancellazioneLogica As Int32,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0
    '    '   Cod_Progetto = 0

    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Zoo_Animali ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")

    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Zoo_Animali ")
    '            StrSQL.Append(" WHERE  1=1 ")

    '        End If

    '        'Parametro obbligatorio
    '        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
    '        End If

    '        If Cod_Progetto <> 0 Then
    '            StrSQL.Append(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
    '        End If

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function




    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Cod_Progetto As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Cod_Progetto = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Zoo_Animali ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Zoo_Animali ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            'Parametro obbligatorio
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Cod_Progetto <> 0 Then
                StrSQL.Append(" AND Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            End If


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
    'Public Function Modifica_Validita_Fine(
    '                        ByVal Piva As String,
    '                        ByVal Cod_Progetto As Int32,
    '                        ByVal Validita_Fine As Date,
    '                            ByVal UserName_Modifica As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Modifica_Validita_Fine()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        'If Piva = "" Then
    '        '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        'End If

    '        If Cod_Progetto = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Progetto obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE Zoo_Animali SET ")
    '        StrSQL.Append("      Validita_Fine       =  " & Agro_SQL_SaveDate(Validita_Fine))
    '        StrSQL.Append("     ,Data_Modifica       =  " & Agro_SQL_SaveDate(Now.Today))
    '        StrSQL.Append("     ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'  ")

    '        StrSQL.Append(" WHERE Cod_Progetto   =  " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")


    '        'Nota: la bestia potrebbe avere cambiato di proprietario

    '        'If Trim(Piva) <> "" Then
    '        '       StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  "
    '        'End If

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function


    Public Function Modifica_Validita_Fine( _
                            ByVal Piva As String, _
                            ByVal Cod_Progetto As Int32, _
                            ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Zoo_Animali_W.Modifica_Validita_Fine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            If Cod_Progetto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Progetto obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Zoo_Animali SET ")
            StrSQL.Append("      Validita_Fine       =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("     ,Data_Modifica       =  " & Agro_SQL_SaveDate(Now.Today))
            StrSQL.Append("     ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")

            StrSQL.Append(" WHERE Cod_Progetto   =  " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")


            'Nota: la bestia potrebbe avere cambiato di proprietario

            'If Trim(Piva) <> "" Then
            '       StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  "
            'End If

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
