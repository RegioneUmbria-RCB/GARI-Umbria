Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports System.Data.OleDb
Imports System.Linq
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility

Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreBudgetDAL
Public Class Budget_Codici_R

End Class

Public Class Budget_Codici_W

    Private Function Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                   ByRef Impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                   ByRef Centri As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                   ByRef Contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                   ByRef Appezzamento As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento,
                                   ByRef Campi As AgronicaCoreEntityFramework_POCO.Budget_Campi,
                                   ByRef Reg_Impianti As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti,
                                   ByRef Imprese_Progetti As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti,
                                   Elemento_Anagrafico As enum_EntitaAlberoImprese,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                   Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Codici_W.Scrivi_Codici_Anagrafe_Valori()"
        Dim messaggioErrore As String = ""

        Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim id_cod = CodiciAnagrafeValori.codiceAnagrafe.codice
        Dim val_cod = CodiciAnagrafeValori.valore
        Dim validita = CodiciAnagrafeValori.validita

        Try


            If val_cod <> "" AndAlso val_cod <> "0" Then

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim PIVA = Impresa.PIVA
                        Dim ImpresexCodiciEF As AgronicaCoreEntityFramework_POCO.Imprese_Codici

                        ImpresexCodiciEF = (From ixc In GiasContext.Imprese_Codici Where
                                                            ixc.id_cod = id_cod AndAlso
                                                            PIVA = ixc.PIVA).FirstOrDefault()

                        If ImpresexCodiciEF Is Nothing Then
                            ImpresexCodiciEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri_Server, Impresa, id_cod, val_cod, username)
                        End If

                        ImpresexCodiciEF.val_cod = val_cod
                        ImpresexCodiciEF.Username_Modifica = username
                        ImpresexCodiciEF.Data_Modifica = DateTime.Now
                        ImpresexCodiciEF.Validita_Inizio = validita.inizio
                        ImpresexCodiciEF.Validita_Fine = validita.fine

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim CentrixCodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

                        CentrixCodiciEF = (From cxc In GiasContext.Centri_Aziendali_Codici Where
                                                            cxc.id_cod = id_cod AndAlso
                                                            PIVA = cxc.PIVA AndAlso
                                                            SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CentrixCodiciEF Is Nothing Then
                            CentrixCodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centri, id_cod, val_cod, username)
                        End If

                        CentrixCodiciEF.val_cod = val_cod
                        CentrixCodiciEF.Username_Modifica = username
                        CentrixCodiciEF.Data_Modifica = DateTime.Now
                        CentrixCodiciEF.Validita_Inizio = validita.inizio
                        CentrixCodiciEF.Validita_Fine = validita.fine

                    Case enum_EntitaAlberoImprese.Campo
                        Dim Id_Budget = Campi.Id_Budget
                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim CampixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Campi_Codici

                        Dim codici = From cxc In GiasContext.Budget_Campi_Codici
                                     Where cxc.id_cod = id_cod AndAlso
                                            Id_Budget = cxc.Id_Budget AndAlso
                                            PIVA = cxc.PIVA AndAlso
                                            campo_cod = cxc.campo_cod AndAlso
                                            SA_COD = cxc.sa_cod
                                     Select cxc

                        Dim codice = codici.FirstOrDefault()

                        Dim efCampi As New Budget_EFCampi()

                        If codice Is Nothing Then
                            codice = efCampi.Create_CampiCodici(GiasContext, objParametri_Server, Campi, id_cod, val_cod, username)
                        End If

                        codice.val_cod = val_cod
                        codice.Username_Modifica = username
                        codice.Data_Modifica = DateTime.Now
                        codice.Validita_Inizio = If(validita Is Nothing, AGRODATAINIZIO, validita.inizio)
                        codice.Validita_Fine = If(validita Is Nothing, AGRODATAFINE, validita.fine)

                        GiasContext.Budget_Campi_Codici.Attach(codice)
                        GiasContext.Entry(codice).State = EntityState.Added
                        GiasContext.SaveChanges()

                        'Scrittura tabella Agronica_Log_Anagrafe
                        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.CampiCodici,
                                                                                                CStr(codice.PIVA), CStr(SA_COD),
                                                                                                (codice.campo_cod), Nothing,
                                                                                                Nothing, Nothing,
                                                                                                enum_TipoOperazioneDB.Scrittura,
                                                                                                objParametri_Server, enum_Id_Servizio.GiasOnline, Id_Budget:=codice.Id_Budget)

                        GiasContext.Agronica_Log_Anagrafe.Add(log)
                        GiasContext.SaveChanges()

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim Id_Budget = Appezzamento.Id_Budget
                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim AppezzamentixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento_Codici

                        AppezzamentixCodiciEF = (From axc In GiasContext.Budget_Appezzamento_Codici Where
                                                            axc.id_cod = id_cod AndAlso
                                                            Id_Budget = axc.Id_Budget AndAlso
                                                            PIVA = axc.PIVA AndAlso
                                                            appezza = axc.appezza AndAlso
                                                            SA_COD = axc.sa_cod).FirstOrDefault()

                        If AppezzamentixCodiciEF Is Nothing Then
                            AppezzamentixCodiciEF = AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Create_AppezzamentoCodici(GiasContext, Appezzamento, id_cod, val_cod, username)
                        End If

                        AppezzamentixCodiciEF.val_cod = val_cod
                        AppezzamentixCodiciEF.Username_Modifica = username
                        AppezzamentixCodiciEF.Data_Modifica = DateTime.Now
                        AppezzamentixCodiciEF.Validita_Inizio = validita.inizio
                        AppezzamentixCodiciEF.Validita_Fine = validita.fine

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim Id_Budget = Reg_Impianti.Id_Budget
                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim ImpiantixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici

                        ImpiantixCodiciEF = (From ixc In GiasContext.Budget_Reg_Impianti_Codici Where
                                                            ixc.id_cod = id_cod AndAlso
                                                            Id_Budget = ixc.Id_Budget AndAlso
                                                            PIVA = ixc.PIVA AndAlso
                                                            appezza = ixc.appezza AndAlso
                                                            SA_COD = ixc.sa_cod AndAlso
                                                            id_Reg = ixc.Id_Reg AndAlso
                                                            ixc.Progetto_Cod = 0).FirstOrDefault()

                        If ImpiantixCodiciEF Is Nothing Then
                            ImpiantixCodiciEF = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)
                        End If

                        ImpiantixCodiciEF.val_cod = val_cod
                        ImpiantixCodiciEF.Username_Modifica = username
                        ImpiantixCodiciEF.Data_Modifica = DateTime.Now
                        ImpiantixCodiciEF.Validita_Inizio = validita.inizio
                        ImpiantixCodiciEF.Validita_Fine = validita.fine

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim Id_Budget = Imprese_Progetti.Id_Budget
                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg
                        Dim progetto_cod = Imprese_Progetti.Progetto_Cod

                        Dim DistintexCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici

                        DistintexCodiciEF = (From dxc In GiasContext.Budget_Reg_Impianti_Codici Where
                                                            dxc.id_cod = id_cod AndAlso
                                                            Id_Budget = dxc.Id_Budget AndAlso
                                                            PIVA = dxc.PIVA AndAlso
                                                            appezza = dxc.appezza AndAlso
                                                            SA_COD = dxc.sa_cod AndAlso
                                                            id_Reg = dxc.Id_Reg AndAlso
                                                            dxc.Progetto_Cod = progetto_cod).FirstOrDefault()

                        If DistintexCodiciEF Is Nothing Then
                            DistintexCodiciEF = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)
                        End If

                        DistintexCodiciEF.val_cod = val_cod
                        DistintexCodiciEF.Username_Modifica = username
                        DistintexCodiciEF.Data_Modifica = DateTime.Now
                        DistintexCodiciEF.Validita_Inizio = validita.inizio
                        DistintexCodiciEF.Validita_Fine = validita.fine


                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim cod_contatto = Contatto.Cod_Contatto


                        Dim ContattoxCodiciEF As AgronicaCoreEntityFramework_POCO.Contatti_Codici

                        ContattoxCodiciEF = (From cxc In GiasContext.Contatti_Codici Where
                                                            cxc.Id_cod = id_cod AndAlso
                                                            PIVA = cxc.PIVA AndAlso
                                                            cod_contatto = cxc.Cod_Contatto).FirstOrDefault()

                        If ContattoxCodiciEF Is Nothing Then
                            ContattoxCodiciEF = AgronicaCoreAnagrafeDAL.EFContatti.Create_ContattiCodici(GiasContext, objParametri_Server, Contatto, id_cod, val_cod, username)
                        End If

                        ContattoxCodiciEF.Val_cod = val_cod
                        ContattoxCodiciEF.Username_Modifica = username
                        ContattoxCodiciEF.Data_Modifica = DateTime.Now
                        ContattoxCodiciEF.Validita_Inizio = validita.inizio
                        ContattoxCodiciEF.Validita_Fine = validita.fine

                    Case Else
                        Throw New Exception("Scrittura Codice Anagrafe su elemento anagrafico " + Elemento_Anagrafico + " non riuscita. ")
                End Select

            Else

                Select Case Elemento_Anagrafico
                    Case enum_EntitaAlberoImprese.Impresa

                        Dim ImpresexCodiciEF As AgronicaCoreEntityFramework_POCO.Imprese_Codici
                        Dim PIVA = Impresa.PIVA

                        ImpresexCodiciEF = (From ixc In GiasContext.Imprese_Codici Where
                                                            ixc.id_cod = id_cod AndAlso
                                                            PIVA = ixc.PIVA).FirstOrDefault()

                        If ImpresexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Imprese_Codici.Attach(ImpresexCodiciEF)
                            GiasContext.Imprese_Codici.Remove(ImpresexCodiciEF)

                            'RW = re-write
                            Dim RW_ImpresexCodiciEF = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri_Server, Impresa, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Centro

                        Dim PIVA = Centri.PIVA
                        Dim SA_COD = Centri.sa_cod

                        Dim CentrixCodiciEF As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici

                        CentrixCodiciEF = (From cxc In GiasContext.Centri_Aziendali_Codici Where
                                                            cxc.id_cod = id_cod AndAlso
                                                            PIVA = cxc.PIVA AndAlso
                                                            SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CentrixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Centri_Aziendali_Codici.Attach(CentrixCodiciEF)
                            GiasContext.Centri_Aziendali_Codici.Remove(CentrixCodiciEF)

                            'RW = re-write
                            Dim RW_CentrixCodiciEF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliCodici(GiasContext, objParametri_Server, Centri, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Campo

                        Dim Id_Budget = Campi.Id_Budget
                        Dim PIVA = Campi.Piva
                        Dim SA_COD = Campi.Sa_Cod
                        Dim campo_cod = Campi.Campo_Cod

                        Dim CampixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Campi_Codici

                        CampixCodiciEF = (From cxc In GiasContext.Budget_Campi_Codici Where
                                                            cxc.id_cod = id_cod AndAlso
                                                            Id_Budget = cxc.Id_Budget AndAlso
                                                            PIVA = cxc.PIVA AndAlso
                                                            campo_cod = cxc.campo_cod AndAlso
                                                            SA_COD = cxc.sa_cod).FirstOrDefault()

                        If CampixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Budget_Campi_Codici.Attach(CampixCodiciEF)
                            GiasContext.Budget_Campi_Codici.Remove(CampixCodiciEF)

                            Dim efCampi As New Budget_EFCampi()

                            'RW = re-write
                            Dim RW_CampixCodiciEF = efCampi.Create_CampiCodici(GiasContext, objParametri_Server, Campi, id_cod, val_cod, objParametri_Utenti.UsernameOperazione)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Appezzamento

                        Dim Id_budget = Appezzamento.Id_Budget
                        Dim PIVA = Appezzamento.PIVA
                        Dim SA_COD = Appezzamento.SA_COD
                        Dim appezza = Appezzamento.APPEZZA

                        Dim AppezzamentixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento_Codici

                        AppezzamentixCodiciEF = (From axc In GiasContext.Budget_Appezzamento_Codici Where
                                                            axc.id_cod = id_cod AndAlso
                                                            Id_budget = axc.Id_Budget AndAlso
                                                            PIVA = axc.PIVA AndAlso
                                                            appezza = axc.appezza AndAlso
                                                            SA_COD = axc.sa_cod).FirstOrDefault()

                        If AppezzamentixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Budget_Appezzamento_Codici.Attach(AppezzamentixCodiciEF)
                            GiasContext.Budget_Appezzamento_Codici.Remove(AppezzamentixCodiciEF)

                            'RW = re-write
                            Dim RW_AppezzamentixCodiciEF = AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Create_AppezzamentoCodici(GiasContext, Appezzamento, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Impianto

                        Dim Id_Budget = Reg_Impianti.Id_Budget
                        Dim PIVA = Reg_Impianti.PIVA
                        Dim SA_COD = Reg_Impianti.SA_COD
                        Dim appezza = Reg_Impianti.APPEZZA
                        Dim id_Reg = Reg_Impianti.ID_REG

                        Dim ImpiantixCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici

                        ImpiantixCodiciEF = (From ixc In GiasContext.Budget_Reg_Impianti_Codici Where
                                                            ixc.id_cod = id_cod AndAlso
                                                            Id_Budget = ixc.Id_Budget AndAlso
                                                            PIVA = ixc.PIVA AndAlso
                                                            appezza = ixc.appezza AndAlso
                                                            SA_COD = ixc.sa_cod AndAlso
                                                            id_Reg = ixc.Id_Reg AndAlso
                                                            ixc.Progetto_Cod = 0).FirstOrDefault()

                        If ImpiantixCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Budget_Reg_Impianti_Codici.Attach(ImpiantixCodiciEF)
                            GiasContext.Budget_Reg_Impianti_Codici.Remove(ImpiantixCodiciEF)

                            'RW = re-write
                            Dim RW_ImpiantixCodiciEF = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Distinta

                        Dim Id_Budget = Imprese_Progetti.Id_Budget
                        Dim PIVA = Imprese_Progetti.Piva
                        Dim SA_COD = Imprese_Progetti.Sa_Cod
                        Dim appezza = Imprese_Progetti.Appezza
                        Dim id_Reg = Imprese_Progetti.Id_Reg
                        Dim progetto_cod = Imprese_Progetti.Progetto_Cod

                        Dim DistintexCodiciEF As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici

                        DistintexCodiciEF = (From dxc In GiasContext.Budget_Reg_Impianti_Codici Where
                                                            dxc.id_cod = id_cod AndAlso
                                                            Id_Budget = dxc.Id_Budget AndAlso
                                                            PIVA = dxc.PIVA AndAlso
                                                            appezza = dxc.appezza AndAlso
                                                            SA_COD = dxc.sa_cod AndAlso
                                                            id_Reg = dxc.Id_Reg AndAlso
                                                            dxc.Progetto_Cod = progetto_cod).FirstOrDefault()

                        If DistintexCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Budget_Reg_Impianti_Codici.Attach(DistintexCodiciEF)
                            GiasContext.Budget_Reg_Impianti_Codici.Remove(DistintexCodiciEF)

                            'RW = re-write
                            Dim RW_DistintexCodiciEF = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Create_RegImpiantiCodici(GiasContext, Reg_Impianti, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case enum_EntitaAlberoImprese.Contatto

                        Dim PIVA = Contatto.Piva
                        Dim cod_contatto = Contatto.Cod_Contatto
                        Dim SA_COD = Contatto.Sa_Cod

                        Dim ContattoxCodiciEF As AgronicaCoreEntityFramework_POCO.Contatti_Codici

                        ContattoxCodiciEF = (From cxc In GiasContext.Contatti_Codici Where
                                                            cxc.Id_cod = id_cod AndAlso
                                                            PIVA = cxc.PIVA AndAlso
                                                            cod_contatto = cxc.Cod_Contatto).FirstOrDefault()

                        If ContattoxCodiciEF IsNot Nothing Then
                            ' funzione per cancellare il record
                            GiasContext.Contatti_Codici.Attach(ContattoxCodiciEF)
                            GiasContext.Contatti_Codici.Remove(ContattoxCodiciEF)

                            'RW = re-write
                            Dim RW_ContattoxCodiciEF = AgronicaCoreAnagrafeDAL.EFContatti.Create_ContattiCodici(GiasContext, objParametri_Server, Contatto, id_cod, val_cod, username)

                        Else
                            'non faccio nulla
                        End If

                    Case Else
                        Throw New Exception("Modifica Codice Anagrafe su elemento anagrafico " + Elemento_Anagrafico + " non riuscita. ")
                End Select

                GiasContext.SaveChanges()

                'If NewTransaction Then
                '    scope.Complete()
                '    scope.Dispose()
                'End If

            End If


        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If NewTransaction Then
            scope.Complete()
            scope.Dispose()
        End If

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return id_cod

    End Function

    Public Function Scrivi_Codici_Campo(CodiciAnagrafeValori As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori,
                                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                                        ByRef Campo As AgronicaCoreEntityFramework_POCO.Budget_Campi,
                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal NewTransaction As Boolean = True)

        Return Scrivi_Codici_Anagrafe_Valori(CodiciAnagrafeValori, Nothing, Nothing, Nothing, Nothing, Campo, Nothing, Nothing, enum_EntitaAlberoImprese.Campo, objParametri_Server, objParametri_Utenti, GiasContext, NewTransaction)
    End Function
End Class