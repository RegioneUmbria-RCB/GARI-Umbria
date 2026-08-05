Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class Lotto_AssegnaxRisumSpeVarQualCert_DAL_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Tipo_Lotto As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_R.Leggi()"
        Dim risposta As String = ""

        Dim pivaSuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

            Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
            Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
            Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri
            Dim specieVegetali As DbSet(Of SpecieVegetali) = GiasContext.SpecieVegetali
            Dim cultivar As DbSet(Of Cultivar) = GiasContext.Cultivar
            Dim lotto_assegna As DbSet(Of Lotto_AssegnaxRisumSpeVarQualCert) = GiasContext.Lotto_AssegnaxRisumSpeVarQualCert
            Dim linee_preprazioni As DbSet(Of Linee_Preparazioni) = GiasContext.Linee_Preparazioni

            Dim lotto =
            From l In lotto_assegna
            Group Join r_u In risorse_umane
                    On
                     r_u.Piva Equals l.Piva And
                     r_u.Cod_RisUm Equals l.Cod_RisUm
                     Into r_u = Group
            From _ru In r_u.DefaultIfEmpty()
            Group Join cont In contatti
                    On
                     cont.Piva Equals _ru.Piva And
                     cont.Cod_Contatto Equals _ru.Cod_Contatto
                   Into cont = Group
            From _cont In cont.DefaultIfEmpty()
            Group Join specie_veg In specieVegetali
                    On
                       specie_veg.Veg_Cod Equals l.Veg_Cod
                    Into specie_veg = Group
            From _specie_veg In specie_veg.DefaultIfEmpty()
            Group Join var In cultivar
                    On
                         var.Cul_Cod Equals l.Cul_Cod
                   Into var = Group
            From _var In var.DefaultIfEmpty()
            Group Join otab_param_cert In tabelleParametri.Where(Function(x) x.Tabella_Cod = 12 AndAlso (x.Piva = Piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On otab_param_cert.Tabella_Par_Cod Equals l.Cert_Cod
            Into otab_param_cert_group = Group
            From _otp_cert In otab_param_cert_group.DefaultIfEmpty()
            Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = 3 AndAlso (x.Piva = Piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = 2)
                    On
                     otab_param_qual.Tabella_Par_Cod Equals l.Qual_Cod
                    Into otab_param_qual_group = Group
            From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
            Group Join linee_prepa In linee_preprazioni
                    On
                    linee_prepa.Preparazione_Cod Equals l.Preparazione_Cod
                    Into linee_prepa = Group
            From _linee_prepa In linee_prepa.DefaultIfEmpty()
            Where
               (l.Piva_SuperUser.Equals(pivaSuperUser)) AndAlso
               (l.Piva.Equals(Piva)) AndAlso
               (Tipo_Lotto = "" OrElse l.Tipo_Lotto.Equals(Tipo_Lotto))
            Order By _cont.Rag_Soc, _specie_veg.Veg_Des, _var.Cul_Des, _otp_qual.Descrizione, _otp_cert.Descrizione
            Select New With {
                .key_lotto_assegna = "",
                .Cod_RisUm = l.Cod_RisUm,
                .Rag_Soc = If(_cont Is Nothing, "", _cont.Rag_Soc),
                .Veg_Cod = If(_specie_veg Is Nothing, -1, _specie_veg.Veg_Cod),
                .Veg_Des = If(_specie_veg Is Nothing, "", _specie_veg.Veg_Des),
                .Cul_Cod = If(_var Is Nothing, 0, _var.Cul_Cod),
                .Cul_Des = If(_var Is Nothing, "", _var.Cul_Des),
                .qualita_cod = If(_otp_qual Is Nothing, 0, _otp_qual.Tabella_Par_Cod),
                .qualita_des = If(_otp_qual Is Nothing, "", If(_otp_qual.Descrizione, "")),
                .qualita_sigla = CStr(If(_otp_qual Is Nothing, "", _otp_qual.Sigla)),
                .certif_cod = If(_otp_cert Is Nothing, 0, _otp_cert.Tabella_Par_Cod),
                .certif_des = If(_otp_cert Is Nothing, "", If(_otp_cert.Descrizione, "")),
                .certif_sigla = If(_otp_cert Is Nothing, "", If(_otp_cert.Sigla, "")),
                .lotto = l.Lotto,
                .validita_inizio = l.Validita_Inizio,
                .validita_fine = l.Validita_Fine,
                .Preparazione_Cod = l.Preparazione_Cod,
                .Preparazione_Des = If(_linee_prepa Is Nothing, "", _linee_prepa.Preparazione_Des),
                .Reg_Cod = l.Reg_Cod,
                .Reg_Des = "",
                .Tipo_Lotto = l.Tipo_Lotto,
                .Algoritmo_Config_String = l.Algoritmo_Config,
                .Algoritmo_Config_Des_String = "",
                .Separatore_Config = 0,
                .Separatore_Config_Des = l.Separatore_Config
            }

            Dim myList = lotto.ToList()
            For Each obj In myList

                obj.key_lotto_assegna = obj.Cod_RisUm & "_" & obj.Veg_Cod & "_" & obj.Cul_Cod & "_" & obj.qualita_cod & "_" & obj.certif_cod & "_" & CStr(obj.validita_inizio).Replace("/", "") & "_" & CStr(obj.validita_fine).Replace("/", "") & "_" &
                    obj.Preparazione_Cod & "_" & obj.Reg_Cod

                'Ottengo la Descrizione del Regolamento
                If obj.Reg_Cod = enum_Cod_Regolamento.Regolamento_bio Then
                    obj.Reg_Des = "Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))"
                ElseIf obj.Reg_Cod = enum_Cod_Regolamento.Regolamento_Nessuno Then
                    obj.Reg_Des = "Convenzionale (Reg. Nessuno)"
                End If


                'Ottengo la Descrizione dei Parametri Scelti
                If Not String.IsNullOrEmpty(obj.Algoritmo_Config_String) Then

                    Dim arrayAlgoritmoConfig = obj.Algoritmo_Config_String.Split("|")

                    Dim listAlgoritmoConfigDes As New List(Of String)

                    For Each Algoritmo_Config In arrayAlgoritmoConfig

                        Select Case CInt(Algoritmo_Config)

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_FORNITORE
                                listAlgoritmoConfigDes.Add("Codice Fornitore")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_DATA
                                listAlgoritmoConfigDes.Add("Data Ingresso Merce")

                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_DESTINAZIONE
                            '    listAlgoritmoConfigDes.Add("Codice Magazzino/Cella Stoccaggio")

                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_SIGLA_SPECIE_VARIETA
                            '    listAlgoritmoConfigDes.Add("Sigla Specie e Varietà")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO
                                listAlgoritmoConfigDes.Add("Contatore Univoco di Carico (6 Cifre)")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_ANNO_AA
                                listAlgoritmoConfigDes.Add("Anno - AA")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_LINEA
                                listAlgoritmoConfigDes.Add("Codice Linea")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_ARTICOLO
                                listAlgoritmoConfigDes.Add("Codice Articolo")

                            'Case enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO_PARAMETRI
                            '    listAlgoritmoConfigDes.Add("Contatore Univoco Prodotto/Calibro/Qualità")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_NUMERO_SETTIMANA
                                listAlgoritmoConfigDes.Add("Numero Settimana")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_GIORNO_GIULIANO
                                listAlgoritmoConfigDes.Add("Giorno Giuliano")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_ENTRATA
                                listAlgoritmoConfigDes.Add("Lotto Entrata In Lavorazione")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_CALIBRO
                                listAlgoritmoConfigDes.Add("Calibro")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_QUALITA
                                listAlgoritmoConfigDes.Add("Qualità")

                            Case enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_TESTATA
                                listAlgoritmoConfigDes.Add("Lotto Testata Lavorazione")

                        End Select

                    Next

                    If listAlgoritmoConfigDes.Count > 0 Then
                        obj.Algoritmo_Config_Des_String = String.Join(", ", listAlgoritmoConfigDes)
                    End If

                Else


                End If

                'Ottengo il valore di Separatore_Config
                If Not String.IsNullOrEmpty(obj.Separatore_Config_Des) Then

                    If obj.Separatore_Config_Des = "." Then
                        obj.Separatore_Config = 1
                    ElseIf obj.Separatore_Config_Des = "-" Then
                        obj.Separatore_Config = 2
                    End If

                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    '##############################################################################################
    Public Function Leggi_Elem_Lotto_Assegna(ByVal piva As String,
                                             ByVal cod_risum As Integer,
                                             ByVal veg_cod As Integer,
                                             ByVal cul_cod As Integer,
                                             ByVal qualita_cod As Integer,
                                             ByVal certif_cod As Integer,
                                             ByVal validita_inizio As Date,
                                             ByVal validita_fine As Date,
                                             ByVal preparazione_cod As Integer,
                                             ByVal reg_cod As Integer,
                                             ByVal tipo_lotto As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Lotto_AssegnaxRisumSpeVarQualCert

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_R.Leggi_Elem_Lotto_Assegna()"

        Dim pivaSuperUser = objParametri.PivaSuperUser
        Dim Elem As Lotto_AssegnaxRisumSpeVarQualCert = Nothing

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

            Elem =
           (From l_a In GiasContext.Lotto_AssegnaxRisumSpeVarQualCert
            Where
              l_a.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                l_a.Piva.Equals(piva) AndAlso
                l_a.Cod_RisUm = cod_risum AndAlso
                l_a.Veg_Cod = veg_cod AndAlso
                l_a.Cul_Cod = cul_cod AndAlso
                l_a.Qual_Cod = qualita_cod AndAlso
                l_a.Cert_Cod = certif_cod AndAlso
                l_a.Validita_Inizio = validita_inizio AndAlso
                l_a.Validita_Fine = validita_fine AndAlso
                l_a.Preparazione_Cod = preparazione_cod AndAlso
                l_a.Reg_Cod = reg_cod AndAlso
                l_a.Tipo_Lotto.Equals(tipo_lotto)
            Select l_a).FirstOrDefault()

        End Using

        Return Elem

    End Function


    Public Function Leggi_Lotto_AssegnaxRisumSpeVarQualCert(ByVal piva As String,
                                                            ByVal filtro_aggiuntivo As String,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.CDG_DAL_R.Leggi_Lotto_AssegnaxRisumSpeVarQualCert()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Lotto_AssegnaxRisumSpeVarQualCert.*  ")
            strSql.Append(" From Lotto_AssegnaxRisumSpeVarQualCert")
            strSql.Append(" Where Lotto_AssegnaxRisumSpeVarQualCert.Piva = '" & Agro_SQL_SaveText(piva) & "'")

            If filtro_aggiuntivo <> "" Then
                strSql.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(filtro_aggiuntivo) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Controlla_Date_LottoAssegna(ByVal lottoAssegnaInsert As Lotto_AssegnaxRisumSpeVarQualCert,
                                                ByVal lottoAssegnaOld As Lotto_AssegnaxRisumSpeVarQualCert,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_R.Controlla_Date_LottoAssegna()"

        Dim countTrovati As Integer = 0

        Dim pivaSuperUser = objParametri.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

            If lottoAssegnaOld Is Nothing Then
                Dim listLottoAssegna = From l_a In GiasContext.Lotto_AssegnaxRisumSpeVarQualCert
                                       Where
                                                l_a.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                                l_a.Piva.Equals(lottoAssegnaInsert.Piva) AndAlso
                                                l_a.Cod_RisUm = lottoAssegnaInsert.Cod_RisUm AndAlso
                                                l_a.Veg_Cod = lottoAssegnaInsert.Veg_Cod AndAlso
                                                l_a.Cul_Cod = lottoAssegnaInsert.Cul_Cod AndAlso
                                                l_a.Qual_Cod = lottoAssegnaInsert.Qual_Cod AndAlso
                                                l_a.Cert_Cod = lottoAssegnaInsert.Cert_Cod AndAlso
                                                ((lottoAssegnaInsert.Validita_Inizio <= l_a.Validita_Inizio AndAlso
                                                lottoAssegnaInsert.Validita_Fine >= l_a.Validita_Inizio) OrElse
                                                (lottoAssegnaInsert.Validita_Inizio <= l_a.Validita_Fine AndAlso
                                                lottoAssegnaInsert.Validita_Fine >= l_a.Validita_Fine) OrElse
                                                (lottoAssegnaInsert.Validita_Inizio >= l_a.Validita_Inizio AndAlso
                                                lottoAssegnaInsert.Validita_Fine <= l_a.Validita_Fine))
                                       Select l_a

                countTrovati = listLottoAssegna.Count()

            Else

                Dim listLottoAssegna = From l_a In GiasContext.Lotto_AssegnaxRisumSpeVarQualCert
                                       Where (l_a.Piva_SuperUser <> pivaSuperUser OrElse
                                                l_a.Piva <> lottoAssegnaOld.Piva OrElse
                                                l_a.Cod_RisUm <> lottoAssegnaOld.Cod_RisUm OrElse
                                                l_a.Veg_Cod <> lottoAssegnaOld.Veg_Cod OrElse
                                                l_a.Cul_Cod <> lottoAssegnaOld.Cul_Cod OrElse
                                                l_a.Qual_Cod <> lottoAssegnaOld.Qual_Cod OrElse
                                                l_a.Cert_Cod <> lottoAssegnaOld.Cert_Cod OrElse
                                                l_a.Validita_Inizio <> lottoAssegnaOld.Validita_Inizio OrElse
                                                l_a.Validita_Fine <> lottoAssegnaOld.Validita_Fine) AndAlso
                                                (l_a.Piva_SuperUser.Equals(pivaSuperUser) AndAlso
                                                l_a.Piva.Equals(lottoAssegnaInsert.Piva) AndAlso
                                                l_a.Cod_RisUm = lottoAssegnaInsert.Cod_RisUm AndAlso
                                                l_a.Veg_Cod = lottoAssegnaInsert.Veg_Cod AndAlso
                                                l_a.Cul_Cod = lottoAssegnaInsert.Cul_Cod AndAlso
                                                l_a.Qual_Cod = lottoAssegnaInsert.Qual_Cod AndAlso
                                                l_a.Cert_Cod = lottoAssegnaInsert.Cert_Cod AndAlso
                                                ((lottoAssegnaInsert.Validita_Inizio <= l_a.Validita_Inizio AndAlso
                                                lottoAssegnaInsert.Validita_Fine >= l_a.Validita_Inizio) OrElse
                                                (lottoAssegnaInsert.Validita_Inizio <= l_a.Validita_Fine AndAlso
                                                lottoAssegnaInsert.Validita_Fine >= l_a.Validita_Fine) OrElse
                                                (lottoAssegnaInsert.Validita_Inizio >= l_a.Validita_Inizio AndAlso
                                                lottoAssegnaInsert.Validita_Fine <= l_a.Validita_Fine)))
                                       Select l_a

                countTrovati = listLottoAssegna.Count()

            End If

        End Using

        Return countTrovati

    End Function

End Class



Public Class Lotto_AssegnaxRisumSpeVarQualCert_DAL_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Lotto_Assegna(ByVal piva As String,
                                           ByVal EFArrayToInsert As ArrayList,
                                           ByVal EFArrayToDelete As ArrayList,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        Const nomeRoutine = "AgronicaCorAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_W.Aggiorna_Lotto_Assegna()"

        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)

                For Each lottoAssegna As Lotto_AssegnaxRisumSpeVarQualCert In EFArrayToInsert
                    GiasContext.Lotto_AssegnaxRisumSpeVarQualCert.Add(lottoAssegna)
                Next

                For Each lottoAssegna As Lotto_AssegnaxRisumSpeVarQualCert In EFArrayToDelete
                    GiasContext.Lotto_AssegnaxRisumSpeVarQualCert.Attach(lottoAssegna)
                    GiasContext.Lotto_AssegnaxRisumSpeVarQualCert.Remove(lottoAssegna)
                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function


    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Elem_Cod As Integer,
                           ByVal Lotto_Cod As Integer,
                           ByVal Lotto_Des As String,
                           ByVal Cifra_Start As String,
                           ByVal Cifra_End As String,
                           ByVal Lotto_Des_Estesa As String,
                           ByVal Tipo As String,
                           ByVal validita_inizio As Date,
                           ByVal validita_fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCorAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT Lotto_Configurazione " & vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            StrSQL.Append("  ,[Piva] " & vbCrLf)
            StrSQL.Append("  ,[Elem_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Lotto_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Lotto_Des] " & vbCrLf)
            StrSQL.Append("  ,[Cifra_Start] " & vbCrLf)
            StrSQL.Append("  ,[Cifra_End] " & vbCrLf)
            StrSQL.Append("  ,[Lotto_Des_Estesa] " & vbCrLf)
            StrSQL.Append("  ,[Tipo], " & vbCrLf)


            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Elem_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Lotto_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Lotto_Des) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cifra_Start) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cifra_End) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Lotto_Des_Estesa) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Tipo) & "'" & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_fine) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Const nomeRoutine = "AgronicaCorAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
