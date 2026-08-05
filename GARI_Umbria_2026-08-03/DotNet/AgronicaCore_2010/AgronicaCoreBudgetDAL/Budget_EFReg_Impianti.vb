Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data.Entity
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

Public Class Budget_EFReg_Impianti
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Shared Function Impianto_Scrivi_EF(ByVal Id_Budget As Integer,
                                              ByVal dati_impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByVal username As String,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True,
                                              Optional ByVal NoteLog As String = ""
                                             ) As Budget_Reg_Impianti

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Impianto_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        If EFImprese.ImpresaExist(GiasContext, dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva) = False Then
            Throw New GiasException("Partita Iva (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + ") non anagrafica imprese. Operazione annullata")
        End If


        If EFCentri_Aziendali.CentroExist(GiasContext,
                                          dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                          dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice) = False Then
            Throw New GiasException("Centro aziendale (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                       dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice.ToString() + ") non anagrafica. Operazione annullata")
        End If


        If Budget_EFAppezzamento.AppezzamentoExist(GiasContext,
                                            Id_Budget,
                                            dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                            dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                            dati_impianto.primaryKey.appezzamentoPK.codice) = False Then
            Throw New GiasException("Budget_Appezzamento (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If



        Dim impianto = Create_RegImpianti(GiasContext,
                                              objParametriServer,
                                              objParametriUtenti,
                                              Id_Budget,
                                              dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                              dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                              dati_impianto.primaryKey.appezzamentoPK.codice,
                                              username
                                              )

        Try
            impianto.DATA_AGG = Now.Date

            Select Case dati_impianto.utilizzoTerreno.GetType()
                Case GetType(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                    impianto.CUL_COD = CType(dati_impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).codice
                Case Else
                    impianto.CUL_COD = 0
            End Select

            impianto.GRFI_COD = If(dati_impianto.gruppoFinalita Is Nothing, 0, dati_impianto.gruppoFinalita.codice)
            impianto.COP_DI = If(dati_impianto.cop_Data_Inizio < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.cop_Data_Inizio > AGRODATAFINE, AGRODATAFINE, dati_impianto.cop_Data_Inizio))
            impianto.COP_DF = If(dati_impianto.cop_Data_Fine < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.cop_Data_Fine > AGRODATAFINE, AGRODATAFINE, dati_impianto.cop_Data_Fine))
            impianto.Piante_Maschi_InSesto = dati_impianto.maschi_in_Sesto
            impianto.Data_Inizio_Innesto = If(dati_impianto.data_Innesto_Varieta < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Innesto_Varieta > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Innesto_Varieta))
            impianto.Data_Inizio_Produzione = If(dati_impianto.data_Inizio_Produzione < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Inizio_Produzione > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Inizio_Produzione))
            impianto.Data_Inizio_Portinnesto = If(dati_impianto.data_Inizio_Portinnesto < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Inizio_Portinnesto > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Inizio_Portinnesto))
            impianto.FORAL_COD = If(dati_impianto.formaAllevamento Is Nothing, 0, dati_impianto.formaAllevamento.codice)
            impianto.SETUP_COD = If(dati_impianto.seminaTrapianto Is Nothing, "", dati_impianto.seminaTrapianto.codice)
            impianto.PORT_COD = If(dati_impianto.portinnesto Is Nothing, -1, dati_impianto.portinnesto.codice)
            impianto.IMP_COD = If(dati_impianto.irrigazione Is Nothing, 0, dati_impianto.irrigazione.codice)
            If dati_impianto.macchineIrrigazione IsNot Nothing Then
                impianto.flagImpiantoIsMacchina = If(dati_impianto.macchineIrrigazione.Count > 0, 1, 0)
            End If
            impianto.TECN_COD = If(dati_impianto.tecnicaConduzioneTraFila Is Nothing, 0, dati_impianto.tecnicaConduzioneTraFila.codice)
            impianto.SU_COD = If(dati_impianto.tecnicaConduzioneSuFila Is Nothing, 0, dati_impianto.tecnicaConduzioneSuFila.codice)
            impianto.COP_COD = If(dati_impianto.copertura Is Nothing, 0, dati_impianto.copertura.codice)
            impianto.COVER = dati_impianto.cover_Crops
            impianto.MONITORATO = dati_impianto.monitorato
            impianto.ProvenienzaSeme = If(dati_impianto.provenienzaSeme Is Nothing, 0, dati_impianto.provenienzaSeme.codice)
            impianto.Sup_Imp = dati_impianto.superficie
            impianto.GRVA_Cod_VEG = If(dati_impianto.gruppoVarietale Is Nothing, 0, dati_impianto.gruppoVarietale.codice)
            impianto.Id_Consociazione = If(dati_impianto.consociazionePK Is Nothing, 0, dati_impianto.consociazionePK.codice)
            impianto.Unita_Vitata = dati_impianto.unita_Vitata
            If dati_impianto.unitaMisuraAlternativa IsNot Nothing Then
                impianto.UDM_COD_ALT = dati_impianto.unitaMisuraAlternativa.codice
                impianto.SUP_ALT = dati_impianto.superficieAlternativa
            End If
            impianto.SUP_ALT = dati_impianto.superficieAlternativa

            If dati_impianto.validita.inizio >= AGRODATAINIZIO And dati_impianto.validita.inizio <= AGRODATAFINE Then
                impianto.Validita_Inizio = dati_impianto.validita.inizio
            End If
            If dati_impianto.validita.fine >= AGRODATAINIZIO And dati_impianto.validita.fine <= AGRODATAFINE Then
                impianto.Validita_Fine = dati_impianto.validita.fine
            End If

            impianto.Data_Modifica = DateTime.Now()

            'lavez - 21/03/2024 - chiavi nuovo tracciato agea
            If dati_impianto.Agea_idColt IsNot Nothing Then
                impianto.Agea_idColt = dati_impianto.Agea_idColt
            End If

            'lavez - 15/04/2024 - nuovo campo data inizio impianto
            If dati_impianto.data_Inizio_Impianto >= AGRODATAINIZIO And dati_impianto.data_Inizio_Impianto <= AGRODATAFINE Then
                impianto.Data_Inizio_Impianto = dati_impianto.data_Inizio_Impianto
            End If

            GiasContext.Budget_Reg_Impianti.Add(impianto)
            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiImpiantoStr = JsonConvert.SerializeObject(dati_impianto, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Budget_Impianti,
                                                                                 CStr(Id_Budget),
                                                                                 impianto.PIVA, CStr(impianto.SA_COD),
                                                                                 CStr(impianto.APPEZZA), CStr(impianto.ID_REG),
                                                                                 Nothing,
                                                                                 enum_TipoOperazioneDB.Scrittura,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 NoteLog, DatiImpiantoStr,
                                                                                 Id_Budget:=impianto.Id_Budget)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

        Catch ex As GiasException
            impianto = Nothing
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            impianto = Nothing
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return impianto

    End Function
    Public Shared Function Impianto_Modifica_EF(ByVal Id_Budget As Integer,
                                       ByVal dati_impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal username As String,
                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                       Optional ByVal NewTransaction As Boolean = True,
                                        Optional NoteLog As String = ""
                                       ) As Budget_Reg_Impianti

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Impianto_Modifica_EF"
        Dim messaggioErrore As String = ""


        If EFImprese.ImpresaExist(GiasContext, dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva) = False Then
            Throw New Exception("Partita Iva (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + ") non anagrafica imprese. Operazione annullata")
        End If


        If EFCentri_Aziendali.CentroExist(GiasContext,
                                          dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                          dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice) = False Then
            Throw New Exception("Centro aziendale (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                       dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice.ToString() + ") non anagrafica. Operazione annullata")
        End If


        If Budget_EFAppezzamento.AppezzamentoExist(GiasContext,
                                            Id_Budget,
                                            dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                            dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                            dati_impianto.primaryKey.appezzamentoPK.codice) = False Then
            Throw New Exception("Budget_Appezzamento (" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Dim reg_impianti = From impianto In GiasContext.Budget_Reg_Impianti
                           Where impianto.Id_Budget = Id_Budget AndAlso
                                     impianto.PIVA = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                     impianto.SA_COD = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice AndAlso
                                     impianto.APPEZZA = dati_impianto.primaryKey.appezzamentoPK.codice AndAlso
                                     impianto.ID_REG = dati_impianto.primaryKey.codice
                           Select impianto

        Dim imp = reg_impianti.FirstOrDefault
        If imp Is Nothing Then
            Throw New Exception("Budget_Impianto (" + Id_Budget.ToString() + "/" + dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   dati_impianto.primaryKey.appezzamentoPK.codice.ToString() + "/" +
                                                   dati_impianto.primaryKey.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If
        Try

            imp.Validita_Inizio = dati_impianto.validita.inizio
            imp.Validita_Fine = dati_impianto.validita.fine
            If dati_impianto.utilizzoTerreno IsNot Nothing Then
                Select Case dati_impianto.utilizzoTerreno.GetType()
                    Case GetType(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                        imp.CUL_COD = CType(dati_impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).codice
                    Case Else
                        imp.CUL_COD = 0
                End Select
            End If
            If (dati_impianto.portinnesto IsNot Nothing) Then
                imp.PORT_COD = dati_impianto.portinnesto.codice
            End If
            If (dati_impianto.formaAllevamento IsNot Nothing) Then
                imp.FORAL_COD = dati_impianto.formaAllevamento.codice
            End If
            If (dati_impianto.irrigazione IsNot Nothing) Then
                imp.IMP_COD = dati_impianto.irrigazione.codice
            End If
            If dati_impianto.macchineIrrigazione IsNot Nothing Then
                imp.flagImpiantoIsMacchina = If(dati_impianto.macchineIrrigazione.Count > 0, 1, 0)
            End If
            If (dati_impianto.provenienzaSeme IsNot Nothing) Then
                imp.ProvenienzaSeme = dati_impianto.provenienzaSeme.codice
            End If
            If (dati_impianto.copertura IsNot Nothing) Then
                imp.COP_COD = dati_impianto.copertura.codice
            End If
            imp.Sup_Imp = dati_impianto.superficie
            If (dati_impianto.gruppoFinalita IsNot Nothing) Then
                imp.GRFI_COD = dati_impianto.gruppoFinalita.codice
            End If
            If (dati_impianto.gruppoVarietale IsNot Nothing) Then
                imp.GRVA_Cod_VEG = dati_impianto.gruppoVarietale.codice
            End If
            If (dati_impianto.seminaTrapianto IsNot Nothing) Then
                imp.SETUP_COD = dati_impianto.seminaTrapianto.codice
            End If
            imp.COVER = dati_impianto.cover_Crops
            imp.MONITORATO = dati_impianto.monitorato
            imp.COP_DI = If(dati_impianto.cop_Data_Inizio < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.cop_Data_Inizio > AGRODATAFINE, AGRODATAFINE, dati_impianto.cop_Data_Inizio))
            imp.COP_DF = If(dati_impianto.cop_Data_Fine < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.cop_Data_Fine > AGRODATAFINE, AGRODATAFINE, dati_impianto.cop_Data_Fine))
            If (dati_impianto.tecnicaConduzioneTraFila IsNot Nothing) Then
                imp.TECN_COD = dati_impianto.tecnicaConduzioneTraFila.codice
            End If
            If (dati_impianto.tecnicaConduzioneSuFila IsNot Nothing) Then
                imp.SU_COD = dati_impianto.tecnicaConduzioneSuFila.codice
            End If

            If (dati_impianto.unitaMisuraAlternativa IsNot Nothing) Then
                imp.UDM_COD_ALT = dati_impianto.unitaMisuraAlternativa.codice
            End If
            imp.SUP_ALT = dati_impianto.superficieAlternativa

            imp.DATA_AGG = Now.Date
            imp.Data_Modifica = DateTime.Now
            imp.Username_Modifica = username
            If (dati_impianto.consociazionePK IsNot Nothing) Then
                imp.Id_Consociazione = dati_impianto.consociazionePK.codice
            End If
            imp.Unita_Vitata = dati_impianto.unita_Vitata
            imp.Data_Inizio_Innesto = If(dati_impianto.data_Innesto_Varieta < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Innesto_Varieta > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Innesto_Varieta))
            imp.Data_Inizio_Produzione = If(dati_impianto.data_Inizio_Produzione < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Inizio_Produzione > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Inizio_Produzione))
            imp.Data_Inizio_Portinnesto = If(dati_impianto.data_Inizio_Portinnesto < AGRODATAINIZIO, AGRODATAINIZIO, If(dati_impianto.data_Inizio_Portinnesto > AGRODATAFINE, AGRODATAFINE, dati_impianto.data_Inizio_Portinnesto))
            imp.Piante_Maschi_InSesto = dati_impianto.maschi_in_Sesto

            'lavez - 21/03/2024 - chiavi nuovo tracciato agea
            If dati_impianto.Agea_idColt IsNot Nothing Then
                imp.Agea_idColt = dati_impianto.Agea_idColt
            End If

            'lavez - 15/04/2024 - nuovo campo data inizio impianto
            If dati_impianto.data_Inizio_Impianto >= AGRODATAINIZIO And dati_impianto.data_Inizio_Impianto <= AGRODATAFINE Then
                imp.Data_Inizio_Impianto = dati_impianto.data_Inizio_Impianto
            End If

            GiasContext.Budget_Reg_Impianti.Attach(imp)
            GiasContext.Entry(imp).State = EntityState.Modified

            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiImpiantoStr = JsonConvert.SerializeObject(dati_impianto, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Budget_Impianti,
                                                                                 Id_Budget, imp.PIVA, CStr(imp.SA_COD),
                                                                                 CStr(imp.APPEZZA), CStr(imp.ID_REG),
                                                                                 Nothing,
                                                                                 enum_TipoOperazioneDB.Modifica,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 NoteLog, DatiImpiantoStr,
                                                                                 Id_Budget:=imp.Id_Budget)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()


        Catch ex As Exception
            imp = Nothing
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return imp

    End Function
    Public Shared Sub ScriviModificaEliminaDestinazioneUsoxImpianto(ByVal Id_Budget As Integer,
                                                     ByVal piva As String,
                                                     ByVal saCod As Integer,
                                                     ByVal appezza As Integer,
                                                     ByVal idReg As Integer,
                                                     ByVal idCod As Integer,
                                                     ByVal valCod As String,
                                                     ByVal delete As Boolean,
                                                     ByVal username As String,
                                                     ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                     Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W.ScriviModificaEliminaDestinazioneUsoxImpianto()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If idCod <> 0 Then
                'pulizia destinazione uso precedente
                Dim imp_codl = From ic In GiasContext.Budget_Reg_Impianti_Codici
                               Where ic.Id_Budget = Id_Budget AndAlso
                                     ic.PIVA = piva AndAlso
                                     ic.sa_cod = saCod AndAlso
                                     ic.appezza = appezza AndAlso
                                     ic.Id_Reg = idReg AndAlso
                                     ic.Progetto_Cod = 0 AndAlso
                                     ic.id_cod >= 3000 AndAlso ic.id_cod <= 3999
                               Select ic

                For Each imp_codOri In imp_codl
                    GiasContext.Budget_Reg_Impianti_Codici.Attach(imp_codOri)
                    GiasContext.Budget_Reg_Impianti_Codici.Remove(imp_codOri)
                Next

                If delete = False Then

                    Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici With {
                            .Id_Budget = Id_Budget,
                            .PIVA = piva,
                            .sa_cod = saCod,
                            .appezza = appezza,
                            .Id_Reg = idReg,
                            .Progetto_Cod = 0,
                            .id_cod = idCod,
                            .val_cod = valCod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = username,
                            .Username_Modifica = username
                        }

                    GiasContext.Budget_Reg_Impianti_Codici.Add(imp_cod)
                End If

                GiasContext.SaveChanges()

            End If

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

    End Sub
    Public Shared Sub ScriviModificaEliminaxImpianto(ByVal Id_Budget As Integer,
                                                     ByVal piva As String,
                                                     ByVal saCod As Integer,
                                                     ByVal appezza As Integer,
                                                     ByVal idReg As Integer,
                                                     ByVal idCod As Integer,
                                                     ByVal valCod As String,
                                                     ByVal delete As Boolean,
                                                     ByVal username As String,
                                                     ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                     Optional ByVal NewTransaction As Boolean = True,
                                                     Optional writeNullValCod As Boolean = False)

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.ScriviModificaEliminaxImpianto()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If idCod <> 0 Then

                Dim imp_codl = (From ic In GiasContext.Budget_Reg_Impianti_Codici
                                Where ic.Id_Budget = Id_Budget AndAlso
                                     ic.PIVA = piva AndAlso
                                     ic.sa_cod = saCod AndAlso
                                     ic.appezza = appezza AndAlso
                                     ic.Id_Reg = idReg AndAlso
                                     ic.Progetto_Cod = 0 AndAlso
                                     ic.id_cod = idCod
                                Select ic).ToList()

                Dim operazione As enum_TipoOperazioneDB

                If imp_codl.Count > 0 AndAlso (valCod <> "0" AndAlso valCod <> "") Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf imp_codl.Count > 0 AndAlso (valCod = "0" OrElse valCod = "") Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf imp_codl.Count = 0 AndAlso (valCod = "" OrElse valCod = "0") Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf imp_codl.Count = 0 AndAlso (valCod <> "" AndAlso valCod <> "0") Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                If valCod Is Nothing Then
                    valCod = ""
                End If

                If writeNullValCod Then
                    If imp_codl.Count > 0 Then
                        operazione = enum_TipoOperazioneDB.Modifica
                    ElseIf imp_codl.Count = 0 Then
                        operazione = enum_TipoOperazioneDB.Scrittura
                    End If
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici With {
                            .Id_Budget = Id_Budget,
                            .PIVA = piva,
                            .sa_cod = saCod,
                            .appezza = appezza,
                            .Id_Reg = idReg,
                            .Progetto_Cod = 0,
                            .id_cod = idCod,
                            .val_cod = valCod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = username,
                            .Username_Modifica = username
                        }

                        GiasContext.Budget_Reg_Impianti_Codici.Add(imp_cod)

                    Case enum_TipoOperazioneDB.Modifica
                        Dim imp_cod = imp_codl.FirstOrDefault
                        imp_cod.val_cod = valCod
                        imp_cod.Data_Modifica = DateTime.Now
                        imp_cod.Username_Modifica = username

                        GiasContext.Budget_Reg_Impianti_Codici.Attach(imp_cod)
                        GiasContext.Entry(imp_cod).State = EntityState.Modified

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim imp_cod = imp_codl.FirstOrDefault
                        GiasContext.Budget_Reg_Impianti_Codici.Attach(imp_cod)
                        GiasContext.Budget_Reg_Impianti_Codici.Remove(imp_cod)

                End Select

                GiasContext.SaveChanges()

            End If

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

    End Sub
    Public Shared Sub ScriviModificaEliminaxProgetto(ByVal Id_Budget As Integer,
                                              ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByVal appezza As Integer,
                                              ByVal idReg As Integer,
                                              ByVal progettoCod As Integer,
                                              ByVal idCod As Integer,
                                              ByVal valCod As String,
                                              ByVal delete As Boolean,
                                              ByVal username As String,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True,
                                              Optional ByVal operazione As Integer = -1,
                                              Optional ByVal imp_codl As List(Of Budget_Reg_Impianti_Codici) = Nothing)

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.ScriviModificaEliminaxProgetto()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If operazione = -1 AndAlso IsNothing(imp_codl) Then

                imp_codl = Leggi_Reg_Impianti_Codici(GiasContext, Id_Budget, piva, saCod, appezza, idReg, progettoCod, idCod)

                If valCod Is Nothing Then
                    valCod = ""
                Else
                    valCod = CStr(valCod)
                End If

                If imp_codl.Count > 0 AndAlso (valCod <> "0" AndAlso valCod <> "") Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf imp_codl.Count > 0 AndAlso (valCod = "0" OrElse valCod = "") Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf imp_codl.Count = 0 AndAlso (valCod = "" OrElse valCod = "0") Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf imp_codl.Count = 0 AndAlso (valCod <> "" AndAlso valCod <> "0") Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If
            End If

            Select Case operazione
                Case enum_TipoOperazioneDB.Scrittura

                    Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici With {
                        .Id_Budget = Id_Budget,
                        .PIVA = piva,
                        .sa_cod = saCod,
                        .appezza = appezza,
                        .Id_Reg = idReg,
                        .Progetto_Cod = progettoCod,
                        .id_cod = idCod,
                        .val_cod = valCod,
                        .inviato = 0,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE,
                        .Username_Creazione = username,
                        .Username_Modifica = username
                    }

                    GiasContext.Budget_Reg_Impianti_Codici.Add(imp_cod)

                Case enum_TipoOperazioneDB.Modifica
                    Dim imp_cod = imp_codl.FirstOrDefault
                    imp_cod.val_cod = valCod
                    imp_cod.Data_Modifica = DateTime.Now
                    imp_cod.Username_Modifica = username

                    GiasContext.Budget_Reg_Impianti_Codici.Attach(imp_cod)
                    GiasContext.Entry(imp_cod).State = EntityState.Modified

                Case enum_TipoOperazioneDB.Cancellazione

                    Dim imp_cod = imp_codl.FirstOrDefault
                    GiasContext.Budget_Reg_Impianti_Codici.Attach(imp_cod)
                    GiasContext.Budget_Reg_Impianti_Codici.Remove(imp_cod)

            End Select

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    End Sub

    Public Shared Sub ScrivixProgetto(ByVal Id_Budget As Integer,
                                              ByVal piva As String,
                                              ByVal saCod As Integer,
                                              ByVal appezza As Integer,
                                              ByVal idReg As Integer,
                                              ByVal progettoCod As Integer,
                                              ByVal idCod As Integer,
                                              ByVal valCod As String,
                                              ByVal delete As Boolean,
                                              ByVal username As String,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True,
                                              Optional ByVal validita_inizio As Date = AGRODATAINIZIO,
                                              Optional ByVal validita_fine As Date = AGRODATAFINE)

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_W.ScriviModificaEliminaxProgetto()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim imp_cod As New AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici With {
                .Id_Budget = Id_Budget,
                .PIVA = piva,
                .sa_cod = saCod,
                .appezza = appezza,
                .Id_Reg = idReg,
                .Progetto_Cod = progettoCod,
                .id_cod = idCod,
                .val_cod = valCod,
                .inviato = 0,
                .Data_Creazione = DateTime.Now,
                .Data_Modifica = DateTime.Now,
                .Validita_Inizio = validita_inizio,
                .Validita_Fine = validita_fine,
                .Username_Creazione = username,
                .Username_Modifica = username
            }

            GiasContext.Budget_Reg_Impianti_Codici.Add(imp_cod)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    End Sub

    Public Shared Function Create_RegImpiantiCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                   ByRef impianto As Budget_Reg_Impianti,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As Budget_Reg_Impianti_Codici

        Dim reg = Create_RegImpiantiCodici(dal, impianto.Id_Budget, impianto.PIVA, impianto.SA_COD, impianto.APPEZZA, impianto.ID_REG, id_cod, val_cod, username)

        Return reg
    End Function

    Private Shared Function Create_RegImpiantiCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                   ByRef Id_budget As Integer,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef appezza As Integer,
                                                   ByRef id_reg As Integer,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As Budget_Reg_Impianti_Codici

        Dim impianto As New Budget_Reg_Impianti_Codici

        impianto.Id_Budget = Id_budget
        impianto.PIVA = piva
        impianto.sa_cod = sa_cod
        impianto.appezza = appezza
        impianto.Id_Reg = id_reg
        impianto.Progetto_Cod = 0

        impianto.id_cod = id_cod
        impianto.val_cod = val_cod

        impianto.inviato = 0
        impianto.datainvio = DateTime.Now

        impianto.Data_Creazione = DateTime.Now
        impianto.Data_Modifica = DateTime.Now

        impianto.Validita_Inizio = AGRODATAINIZIO
        impianto.Validita_Fine = AGRODATAFINE

        impianto.Username_Creazione = username
        impianto.Username_Modifica = username

        impianto.Validazione = 0
        impianto.Data_Validazione = DateTime.Now
        impianto.UserName_Validazione = ""

        dal.Budget_Reg_Impianti_Codici.Add(impianto)
        dal.SaveChanges()

        Return impianto
    End Function

    Private Shared Function NuovoImpianto_Cod(piva As String,
                                              sa_cod As Integer,
                                              appezza As Integer,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              ByRef objParametriUtenti As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objParametriUtenti)
        Dim progressivogias As Long = dt.Rows(0).Item("ProgressivoGIAS")

        Dim basecode As Long = 0
        Dim topcode As Long = 20000000
        idGen.Calcola_BaseCode(progressivogias, topcode, basecode, objParametri)
        Dim impianto = idGen.NuovoId_Reg_Impianti(piva, sa_cod, appezza, basecode, topcode, objParametri)

        Return impianto
    End Function

    Private Shared Function Create_RegImpianti(ByRef dal As Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri,
                                               ByRef Id_Budget As Integer,
                                               ByRef piva As String,
                                               ByRef sa_cod As Integer,
                                               ByRef appezza As Integer,
                                               ByRef username As String
                                              ) As Budget_Reg_Impianti

        Dim impianto As New Budget_Reg_Impianti

        impianto.Id_Budget = Id_Budget
        impianto.PIVA = piva
        impianto.SA_COD = sa_cod
        impianto.APPEZZA = appezza
        impianto.ID_REG = NuovoImpianto_Cod(piva, sa_cod, appezza, objParametri, objParametriUtenti)
        impianto.DATA_AGG = Now.Date
        impianto.COD_RESP = 0
        impianto.COD_ENTE = 0
        impianto.CAMPO_SPIA = 0
        impianto.DATA = Now.Date
        impianto.CUL_COD = 0
        impianto.GRFI_COD = 0
        impianto.DATA_RACCOLTA = AGRODATAINIZIO
        impianto.PRODUZIONE = 0
        impianto.Resa_Prevista = 0
        impianto.Resa_Effettiva = 0
        impianto.SCARTO = 0
        impianto.IND_MAT_COD = 0
        impianto.IND_MAT_RIL = 0
        impianto.STA_TER = ""
        impianto.TRA_FILA = 0
        impianto.SU_FILA = 0
        impianto.P_HA = 0
        impianto.COP_DI = AGRODATAINIZIO
        impianto.COP_DF = AGRODATAINIZIO
        impianto.FORAL_COD = 0
        impianto.SETUP_COD = ""
        impianto.PORT_COD = 0
        impianto.IMP_COD = 0
        impianto.STRU_PROT = 0
        impianto.PRO_PAG = 0
        impianto.SEME_Q = 0
        impianto.SEME_T = 0
        impianto.SEME_P = 0
        impianto.SEME_D = 0
        impianto.STATO_RESIDUI = ""
        impianto.TECN_COD = 0
        impianto.DENITRIFICAZIONE = 0
        impianto.VOLATILIZZAZIONE = 0
        impianto.PROFONDITALAV = 0
        impianto.ID_CAMPO = 0
        impianto.SU_COD = 0
        impianto.COP_COD = 0
        impianto.COVER = 0
        impianto.MONITORATO = 0
        impianto.CODICE_FISCALE_TECNICO = ""
        impianto.USER = objParametri.PivaSuperUser
        impianto.REGOLAMENTO = 1
        impianto.FINANZIAMENTO = 0
        impianto.Data_Conversione = AGRODATAINIZIO
        impianto.ProvenienzaSeme = 0
        impianto.inviato = 0
        impianto.Data_Creazione = DateTime.Now
        impianto.Data_Modifica = DateTime.Now
        impianto.Username_Creazione = username
        impianto.Username_Modifica = username
        impianto.Validita_Inizio = AGRODATAINIZIO
        impianto.Validita_Fine = AGRODATAFINE
        impianto.Validazione = 0
        impianto.Data_Validazione = DateTime.Now
        impianto.UserName_Validazione = ""
        impianto.Sup_Imp = 0
        impianto.GRVA_Cod_VEG = 0
        impianto.Id_Consociazione = 0
        impianto.Unita_Vitata = 0
        impianto.Sovrainnesto_Cod = 0
        impianto.ancoraggiTestata = 0
        impianto.annoRiferimento = 0
        impianto.codFiliStostegno = ""
        impianto.codPaliTessitura = ""
        impianto.codPaliTestata = ""
        impianto.codStatoColt = ""
        impianto.codTipoVari = ""
        impianto.DataProtocollo = AGRODATAINIZIO
        impianto.DataRilievo = AGRODATAINIZIO
        impianto.destProduttiva = ""
        impianto.destProduttivaDescr = ""
        impianto.distanzaPali = 0
        impianto.dtFine = AGRODATAINIZIO
        impianto.dtFineGestione = AGRODATAINIZIO
        impianto.dtInizio = AGRODATAINIZIO
        impianto.dtInizioGestione = AGRODATAINIZIO
        impianto.dtIns = AGRODATAINIZIO
        impianto.dtVar = AGRODATAINIZIO
        impianto.fallanzePerc = 0
        impianto.flagAnomalia = 0
        impianto.flagAttuale = 0
        impianto.flagCessata = 0
        impianto.flagContributo = 0
        impianto.flagRegolarizz2009 = 0
        impianto.flagRicalcoloGis = 0
        impianto.GiacituraTerreno = ""
        impianto.idUnitaVitata = 0
        impianto.idUtenteIns = ""
        impianto.idUtenteVar = ""
        impianto.numeroProtocollo = ""
        impianto.progPoligono = ""
        impianto.supVitataDich = 0
        impianto.supVitataDichPRCalcolo = 0
        impianto.SuperficieServizioMq = 0
        impianto.Terrazzamenti = 0
        impianto.TipoColtura = ""
        impianto.tipoProcedimento = ""
        impianto.tipoUnar = ""
        impianto.tipoVariazione = ""
        impianto.unar = ""
        impianto.Data_Inizio_Portinnesto = AGRODATAINIZIO
        impianto.Data_Inizio_Innesto = AGRODATAINIZIO
        impianto.Piante_Maschi_InSesto = 0
        impianto.Data_Inizio_Produzione = AGRODATAINIZIO
        impianto.UDM_COD_ALT = 0
        impianto.SUP_ALT = 0

        Return impianto
    End Function

    Public Shared Sub ScriviModificaEliminaMacchinaxImpianto(tipoOperazioneImpianto As enum_TipoOperazioneDB,
                                                     ByVal Id_Budget As Integer,
                                                     ByVal piva As String,
                                                     ByVal saCod As Integer,
                                                     ByVal appezza As Integer,
                                                     ByVal idReg As Integer,
                                                     ByVal macchine As List(Of ParcoMacchine),
                                                     ByVal username As String,
                                                     ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                     ByVal NewTransaction As Boolean,
                                                     ByVal SaveChanges As Boolean)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Budget_Reg_Impianti_Codici_W.ScriviModificaEliminaMacchinaxImpianto()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim macCod As Integer = 0
            If macchine IsNot Nothing AndAlso macchine.Count > 0 Then
                macCod = macchine.First.codice
            End If

            Dim imp_mac As New List(Of AgronicaCoreEntityFramework_POCO.Budget_Reg_ImpiantiXParcoMacchine)
            If tipoOperazioneImpianto = enum_TipoOperazioneDB.Scrittura Then
                imp_mac = New List(Of Budget_Reg_ImpiantiXParcoMacchine)
            Else
                imp_mac = (From ic In GiasContext.Budget_Reg_ImpiantiXParcoMacchine
                           Where ic.Id_Budget = Id_Budget AndAlso
                            ic.Piva = piva AndAlso
                            ic.Sa_Cod = saCod AndAlso
                            ic.Appezza = appezza AndAlso
                            ic.ID_Reg = idReg
                           Select ic).ToList()
            End If

            For Each imp_macOri In imp_mac
                GiasContext.Budget_Reg_ImpiantiXParcoMacchine.Attach(imp_macOri)
                GiasContext.Budget_Reg_ImpiantiXParcoMacchine.Remove(imp_macOri)
            Next

            If tipoOperazioneImpianto <> enum_TipoOperazioneDB.Cancellazione AndAlso macCod <> 0 Then
                Dim impxmac As New AgronicaCoreEntityFramework_POCO.Budget_Reg_ImpiantiXParcoMacchine With {
                        .Id_Budget = Id_Budget,
                        .Piva = piva,
                        .Sa_Cod = saCod,
                        .Appezza = appezza,
                        .ID_Reg = idReg,
                        .Mac_Cod = macCod,
                        .inviato = 0,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE,
                        .Username_Creazione = username,
                        .Username_Modifica = username
                    }

                GiasContext.Budget_Reg_ImpiantiXParcoMacchine.Add(impxmac)
            End If

            If SaveChanges Then
                GiasContext.SaveChanges()
            End If

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

    End Sub

    Public Shared Function ImpiantoExist(ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                         ByVal Id_Budget As Integer,
                                         ByVal PartitaIva As String,
                                         ByVal CentroAziendale As Integer,
                                         ByVal CodiceAppezzamento As Integer,
                                         ByVal id_reg As Integer) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ImpiantoExist()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            Dim regimpList = From regimp In GiasContext.Budget_Reg_Impianti
                             Where regimp.Id_Budget = Id_Budget AndAlso
                                     regimp.PIVA = PartitaIva AndAlso
                                     regimp.SA_COD = CentroAziendale AndAlso
                                     regimp.APPEZZA = CodiceAppezzamento AndAlso
                                     regimp.ID_REG = id_reg
                             Select regimp

            Dim reg = regimpList.FirstOrDefault()
            If (reg IsNot Nothing) Then
                ret = True
            End If
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret

    End Function


    Public Shared Function Leggi_Reg_Impianti_Codici(ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                     ByVal Id_Budget As Integer,
                                                 ByVal piva As String,
                                                 ByVal saCod As Integer,
                                                 ByVal appezza As Integer,
                                                 ByVal idReg As Integer,
                                                ByVal progettoCod As Integer,
                                                ByVal idCod As Integer) As List(Of Budget_Reg_Impianti_Codici)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Budget_EFReg_Impianti.Leggi_Reg_Impianti_Codici()"
        Dim messaggioErrore As String = ""
        Dim lst_reg_impianti_codici As New List(Of Budget_Reg_Impianti_Codici)
        Try
            lst_reg_impianti_codici = (From ic In GiasContext.Budget_Reg_Impianti_Codici
                                       Where ic.Id_Budget = Id_Budget AndAlso
                                 ic.PIVA = piva AndAlso
                                 ic.sa_cod = saCod AndAlso
                                 ic.appezza = appezza AndAlso
                                 ic.Id_Reg = idReg AndAlso
                                 ic.Progetto_Cod = progettoCod AndAlso
                                 ic.id_cod = idCod
                                       Select ic).ToList()
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return lst_reg_impianti_codici

    End Function
End Class
