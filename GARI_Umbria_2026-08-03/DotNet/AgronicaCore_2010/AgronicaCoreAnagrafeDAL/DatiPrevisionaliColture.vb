Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports InData.DatiPrevisionaliColture
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports Newtonsoft.Json
Imports System.Data.Entity
Imports AgronicaCoreModelsSTD.exceptions

Public Class DatiPrevisionaliColture_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function readDatiPrevisionali(
                                        ByVal params As DatiPrevisionaliColtureRequest,
                                        ByVal objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_R.readDatiPrevisionali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine("IF (SELECT COUNT(*) FROM SpecieVegetali_Default WHERE Veg_Cod = " & Agro_SQL_SaveNum(params.vegCod.codice) & ") > 0")
            StrSQL.AppendLine("    SELECT s.*, um.UDM_DES, um.UDM_SIM")
            StrSQL.AppendLine("    FROM SpecieVegetali_Default s")
            StrSQL.AppendLine("        LEFT JOIN UnitaMisura um ON um.UDM_COD = s.udm_cod")
            StrSQL.AppendLine("    WHERE s.Veg_Cod = " & Agro_SQL_SaveNum(params.vegCod.codice))
            StrSQL.AppendLine("        AND s.Codice = " & Agro_SQL_SaveNum(params.parametroCod.codice))
            StrSQL.AppendLine("ELSE")
            StrSQL.AppendLine("    SELECT s.*, um.UDM_DES, um.UDM_SIM")
            StrSQL.AppendLine("    FROM SpecieVegetali_DefaultGlobali S")
            StrSQL.AppendLine("        LEFT JOIN UnitaMisura um ON um.UDM_COD = s.udm_cod")
            StrSQL.AppendLine("    WHERE s.veg_cod = " & Agro_SQL_SaveNum(params.vegCod.codice))
            StrSQL.AppendLine("        AND s.parametro = " & Agro_SQL_SaveNum(params.parametroCod.codice))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readDatiPrevisionaliAll(
                                           ByVal params As DatiPrevisionaliColtureRequest,
                                           ByVal objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_R.readDatiPrevisionali()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine("SELECT s.*")
            StrSQL.AppendLine("    , CONCAT(um.UDM_DES, ' (' + um.UDM_SIM + ')') AS udm_des")
            StrSQL.AppendLine("    , sv.veg_des")
            StrSQL.AppendLine("    , c.cul_des")
            StrSQL.AppendLine("    , gv.grva_des")
            StrSQL.AppendLine("    , gf.grfi_des")
            StrSQL.AppendLine("    , p.port_des")
            StrSQL.AppendLine("    , fa.foral_des")
            StrSQL.AppendLine("    , lr.Regione_Des")
            StrSQL.AppendLine("    , lp.PROVINCIA")
            StrSQL.AppendLine("    , pt.Descrizione AS codice_stato_des")
            StrSQL.AppendLine("    , (CASE WHEN reg_cod = 1 THEN 'Nessuno' ELSE (CASE WHEN reg_cod = 4 THEN 'Bio' ELSE '' END) END) AS regolamento_des")
            StrSQL.AppendLine("    , (CASE WHEN s.Codice = 4 THEN 'Resa Prevista' ELSE '' END) AS parametro_des")
            StrSQL.AppendLine("    , CASE")
            StrSQL.AppendLine("        WHEN s.stato_cod = 102 THEN 'In Produzione'")
            StrSQL.AppendLine("        ELSE fc.Fase_Des")
            StrSQL.AppendLine("    END AS stato_des")
            StrSQL.AppendLine("    , cac.InfoAgg_Des AS dettSpeciePersonalizzatoDes")
            StrSQL.AppendLine("FROM SpecieVegetali_Default s")
            StrSQL.AppendLine("    LEFT JOIN SpecieVegetali sv ON sv.veg_cod = s.veg_cod")
            StrSQL.AppendLine("    LEFT JOIN Cultivar c ON c.cul_cod = s.cul_cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoVarietale gv ON gv.grva_cod = s.grva_cod")
            StrSQL.AppendLine("    LEFT JOIN GruppoFinalita gf ON gf.grfi_cod = s.grfi_cod")
            StrSQL.AppendLine("    LEFT JOIN Portinnesti p ON p.port_cod = s.port_cod")
            StrSQL.AppendLine("    LEFT JOIN FormeAllevamento fa ON fa.foral_cod = s.foral_cod")
            StrSQL.AppendLine("    LEFT JOIN Lista_Regioni lr ON lr.reg = s.reg")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp ON lp.SIGLA = (SELECT TOP 1 ISTAT.COMUNI_PROV FROM ISTAT WHERE ISTAT.PROV = s.prov)")
            StrSQL.AppendLine("    LEFT JOIN UnitaMisura um ON um.UDM_COD = s.udm_cod")
            StrSQL.AppendLine("    LEFT JOIN FasiCicloColturale_Anagrafiche fc ON fc.Fase_Cod = s.stato_cod")
            StrSQL.AppendLine("    LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 pt ON pt.Codice = s.codice_stato")
            StrSQL.AppendLine("    LEFT JOIN CAC_Codifica_InfoAggiuntive cac ON cac.InfoAgg_Cod = s.dettSpeciePersonalizzatoCod")
            StrSQL.AppendLine("        AND cac.Argomento_Cod = " & enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.DettaglioSpeciePersonalizzato)
            StrSQL.AppendLine("WHERE 1 = 1")

            If params.vegCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.Veg_Cod = " & Agro_SQL_SaveNum(params.vegCod.codice))
            End If

            If params.culCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.Cul_Cod = " & Agro_SQL_SaveNum(params.culCod.codice))
            End If

            If params.parametroCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.Codice = " & Agro_SQL_SaveNum(params.parametroCod.codice))
            End If

            If params.grvaCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.grva_cod = " & Agro_SQL_SaveNum(params.grvaCod.codice))
            End If

            If params.grfiCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.grfi_cod = " & Agro_SQL_SaveNum(params.grfiCod.codice))
            End If

            If params.statoCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.stato_cod = " & Agro_SQL_SaveNum(params.statoCod.codice))
            End If

            If params.portCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.port_cod = " & Agro_SQL_SaveNum(params.portCod.codice))
            End If

            If params.foralCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.foral_cod = " & Agro_SQL_SaveNum(params.foralCod.codice))
            End If

            If params.dettSpeciePersonalizzatoCod.codice <> "" Then
                StrSQL.AppendLine("    AND s.dettSpeciePersonalizzatoCod = '" & Agro_SQL_SaveText(params.dettSpeciePersonalizzatoCod.codice) & "'")
            End If

            If params.regCod.codice <> 0 Then
                StrSQL.AppendLine("    AND s.reg_cod = " & Agro_SQL_SaveNum(params.regCod.codice))
            End If

            If params.reg.codice <> "" Then
                StrSQL.AppendLine("    AND s.reg = '" & Agro_SQL_SaveText(params.reg.codice) + "'")
            End If

            If params.prov.codice <> "" Then
                StrSQL.AppendLine("    AND s.prov = '" & Agro_SQL_SaveText(params.prov.codice) + "'")
            End If

            If IsNothing(params.validitaInizio) Then
                StrSQL.AppendLine("    AND s.Validita_Inizio = " & Agro_SQL_SaveDate(params.validitaInizio))
            End If

            If IsNothing(params.validitaFine) Then
                StrSQL.AppendLine("    AND s.Validita_Fine = " & Agro_SQL_SaveNum(params.validitaFine))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function readAddressForImpianto(
                                          ByVal piva As String,
                                          ByVal sa_cod As Integer,
                                          ByVal appezza As Integer,
                                          ByVal objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_R.readAddressForImpianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.AppendLine("")
            StrSQL.AppendLine("WITH axi AS (")
            StrSQL.AppendLine("    SELECT TOP 1 *")
            StrSQL.AppendLine("    FROM AppezzamentixIndirizzi")
            StrSQL.AppendLine("    WHERE appezza = " & Agro_SQL_SaveNum(appezza))
            StrSQL.AppendLine("        AND sa_cod = " & Agro_SQL_SaveNum(sa_cod))
            StrSQL.AppendLine("        AND PIVA= '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine(", cxi AS (")
            StrSQL.AppendLine("    SELECT TOP 1 *")
            StrSQL.AppendLine("    FROM CentrixIndirizzi")
            StrSQL.AppendLine("    WHERE sa_cod = " & Agro_SQL_SaveNum(sa_cod))
            StrSQL.AppendLine("        AND PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT lp.REG, Indirizzi.*")
            StrSQL.AppendLine("FROM Indirizzi")
            StrSQL.AppendLine("    INNER JOIN axi ON Indirizzi.cod_indirizzo = axi.cod_indirizzo")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp ON lp.PROV = Indirizzi.pro_cod_istat")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("UNION")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("SELECT lp.REG, Indirizzi.*")
            StrSQL.AppendLine("FROM Indirizzi")
            StrSQL.AppendLine("    INNER JOIN cxi ON cxi.cod_indirizzo = Indirizzi.cod_indirizzo")
            StrSQL.AppendLine("    LEFT JOIN Lista_Province lp ON lp.PROV = Indirizzi.pro_cod_istat")
            StrSQL.AppendLine("")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class DatiPrevisionaliColture_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Shared Function generateNewId(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Int32

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim MessaggioErrore As String = String.Empty
        Dim idSeq As Integer = Nothing

        Try
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                idSeq = ObjSequenze.NuovoId_Tabella_EF(
                    GiasContext,
                    "SpecieVegetali_Default",
                    0,
                    2000000000,
                    objParametri
                    )

            End Using
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return idSeq

    End Function

    Private Function createSpecieVegetaliDefault(
                                                ByRef piva As String,
                                                ByVal id As Integer,
                                                ByRef username As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As SpecieVegetali_Default

        Dim record As New AgronicaCoreEntityFramework_POCO.SpecieVegetali_Default

        Dim idTabella = 0
        If id = 0 OrElse id = Nothing Then
            idTabella = generateNewId(objParametri)
        Else
            idTabella = id
        End If

        record.Id = idTabella
        record.Piva = piva

        record.Codice = 0
        record.Cul_Cod = 0
        record.Data_Creazione = DateTime.Now
        record.Data_Modifica = DateTime.Now
        record.inviato = 0
        record.Numero_Ciclo = 0
        record.PivaSuperUser = objParametri.PivaSuperUser
        record.Username_Creazione = objParametri.UtenteUsername
        record.Username_Modifica = objParametri.UtenteUsername
        record.Validita_Fine = AGRODATAFINE
        record.Validita_Inizio = AGRODATAINIZIO
        record.Valore = 0
        record.Veg_Cod = 0

        record.foral_cod = 0
        record.grfi_cod = 0
        record.grva_cod = 0
        record.port_cod = 0
        record.dettSpeciePersonalizzatoCod = ""
        record.codice_stato = ""
        record.prov = ""
        record.reg = ""
        record.reg_cod = 0
        record.stato_cod = 0


        Return record
    End Function

    Public Function editDatiPrevisionaliColtureEF(
                                                 ByVal datiPrevisionaliColture As DatiPrevisionaliColtureComplete,
                                                 ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal NewTransaction As Boolean = True
                                                 ) As SpecieVegetali_Default

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_W.editDatiPrevisionaliColtureEF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim record = (
            From specie In GiasContext.SpecieVegetali_Default
            Where specie.Id = datiPrevisionaliColture.id
                ).FirstOrDefault()

        Dim dal As New DatiPrevisionaliColture_W

        Try
            If IsNothing(record) Then
                dal.createDatiPrevisionaliColtureEF(
                    datiPrevisionaliColture,
                    objParametriServer,
                    objParametriUtenti,
                    GiasContext,
                    NewTransaction
                    )
            Else
                record.Codice = datiPrevisionaliColture.parametroCod
                record.Cul_Cod = datiPrevisionaliColture.culCod
                record.Validita_Fine = datiPrevisionaliColture.validitaFine
                record.Validita_Inizio = datiPrevisionaliColture.validitaInizio
                record.Valore = datiPrevisionaliColture.valore
                record.Veg_Cod = datiPrevisionaliColture.vegCod
                record.Data_Modifica = DateTime.Now
                record.foral_cod = datiPrevisionaliColture.foralCod
                record.grfi_cod = datiPrevisionaliColture.grfiCod
                record.grva_cod = datiPrevisionaliColture.grvaCod
                record.port_cod = datiPrevisionaliColture.portCod
                record.dettSpeciePersonalizzatoCod = datiPrevisionaliColture.dettSpeciePersonalizzatoCod
                record.codice_stato = datiPrevisionaliColture.codiceStato
                record.prov = datiPrevisionaliColture.prov
                record.reg = datiPrevisionaliColture.reg
                record.reg_cod = datiPrevisionaliColture.regCod
                record.stato_cod = datiPrevisionaliColture.statoCod
                record.Username_Modifica = objParametriServer.UsernameOperazione
                record.udm_cod = datiPrevisionaliColture.udm

                GiasContext.SpecieVegetali_Default.Attach(record)
                GiasContext.Entry(record).State = EntityState.Modified
                GiasContext.SaveChanges()

                Dim datiPrevisionaliColtureStr = ""
                If datiPrevisionaliColture IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiPrevisionaliColtureStr = JsonConvert.SerializeObject(datiPrevisionaliColture, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "SpecieVegetali_Default",
                    CStr(record.Piva),
                    CStr(record.Id),
                    Nothing, Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Modifica,
                    objParametriServer,
                    enum_Id_Servizio.GiasOnline,
                    "",
                    datiPrevisionaliColtureStr
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()

                If NewTransaction Then
                    scope.Complete()
                    scope.Dispose()
                End If
            End If

        Catch ex As GiasException
            record = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            record = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return record

    End Function

    Public Function createDatiPrevisionaliColtureEF(
                                                   ByVal datiPrevisionaliColture As DatiPrevisionaliColtureComplete,
                                                   ByRef objParametriServer As AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreParametri,
                                                   Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                   Optional ByVal NewTransaction As Boolean = True
                                                   ) As SpecieVegetali_Default

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_W.createDatiPrevisionaliColtureEF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim dal As New DatiPrevisionaliColture_W

        Dim record = createSpecieVegetaliDefault(
            datiPrevisionaliColture.piva,
            datiPrevisionaliColture.id,
            objParametriServer.UsernameOperazione,
            objParametriServer
            )

        Try
            record.Piva = datiPrevisionaliColture.piva
            record.Codice = datiPrevisionaliColture.parametroCod
            record.Cul_Cod = datiPrevisionaliColture.culCod
            record.Validita_Fine = datiPrevisionaliColture.validitaFine
            record.Validita_Inizio = datiPrevisionaliColture.validitaInizio
            record.Valore = datiPrevisionaliColture.valore
            record.Veg_Cod = datiPrevisionaliColture.vegCod
            record.Data_Modifica = DateTime.Now
            record.foral_cod = datiPrevisionaliColture.foralCod
            record.grfi_cod = datiPrevisionaliColture.grfiCod
            record.grva_cod = datiPrevisionaliColture.grvaCod
            record.port_cod = datiPrevisionaliColture.portCod
            record.dettSpeciePersonalizzatoCod = datiPrevisionaliColture.dettSpeciePersonalizzatoCod
            record.codice_stato = datiPrevisionaliColture.codiceStato
            record.prov = datiPrevisionaliColture.prov
            record.reg = datiPrevisionaliColture.reg
            record.reg_cod = datiPrevisionaliColture.regCod
            record.stato_cod = datiPrevisionaliColture.statoCod
            record.Username_Modifica = objParametriServer.UsernameOperazione
            record.udm_cod = datiPrevisionaliColture.udm

            GiasContext.SpecieVegetali_Default.Add(record)
            GiasContext.SaveChanges()

            Dim datiPrevisionaliColtureStr = ""
            If datiPrevisionaliColture IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                datiPrevisionaliColtureStr = JsonConvert.SerializeObject(datiPrevisionaliColture, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                "SpecieVegetali_Default",
                CStr(record.Piva),
                CStr(record.Id),
                Nothing, Nothing,
                Nothing, Nothing,
                enum_TipoOperazioneDB.Scrittura,
                objParametriServer,
                enum_Id_Servizio.GiasOnline,
                "",
                datiPrevisionaliColtureStr
                )

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            record = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            record = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return record

    End Function

    Public Function deleteDatiPrevisionaliColtureEF(
                                                   ByVal datiPrevisionaliColture As DatiPrevisionaliColtureComplete,
                                                   ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                   Optional ByVal NewTransaction As Boolean = True
                                                   ) As SpecieVegetali_Default

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.DatiPrevisionaliColture_W.deleteDatiPrevisionaliColtureEF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim record = (
            From specie In GiasContext.SpecieVegetali_Default
            Where specie.Id = datiPrevisionaliColture.id
                ).FirstOrDefault()

        Dim dal As New DatiPrevisionaliColture_W

        Try
            GiasContext.SpecieVegetali_Default.Attach(record)
            GiasContext.SpecieVegetali_Default.Remove(record)
            GiasContext.SaveChanges()

            Dim datiPrevisionaliColtureStr = ""
            If datiPrevisionaliColture IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                datiPrevisionaliColtureStr = JsonConvert.SerializeObject(datiPrevisionaliColture, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                "SpecieVegetali_Default",
                CStr(datiPrevisionaliColture.piva),
                CStr(record.Id),
                Nothing, Nothing,
                Nothing, Nothing,
                enum_TipoOperazioneDB.Cancellazione,
                objParametriServer, enum_Id_Servizio.GiasOnline,
                "",
                datiPrevisionaliColtureStr
                )

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return record

    End Function

End Class