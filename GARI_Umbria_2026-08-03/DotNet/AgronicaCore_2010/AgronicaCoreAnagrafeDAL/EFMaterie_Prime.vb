Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class EFMaterie_Prime

    Public Shared Function CreateMaterie_Prime(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef piva As String,
                                               ByRef elemCod As Integer,
                                               ByRef username As String,
                                               Optional ByVal saCod As Integer = PUBBLICO,
                                               Optional ByVal codArticolo As String = ""
                                               ) As AgronicaCoreEntityFramework_POCO.Materie_Prime

        Dim nomeRoutine As String = "CreateMaterie_Prime"

        Dim mat As AgronicaCoreEntityFramework_POCO.Materie_Prime
        Dim idGen As New Agro_Sequenze

        Try
            mat = New AgronicaCoreEntityFramework_POCO.Materie_Prime

            mat.Piva = piva
            mat.Elem_Cod = elemCod
            mat.Mat_Cod = idGen.NuovoId_Tabella_EF(dal, "Materie_Prime", 0, 2000000000, objParametri)

            mat.Sa_Cod = saCod
            mat.Cod_Articolo = codArticolo
            mat.Mat_Des = ""
            mat.Sem_Cod = 0
            mat.Cul_Cod = 0
            mat.Veg_Cod = 0
            mat.Trap_Dur = 0
            mat.Uso = 0
            mat.ClToss_Cod = "0"
            mat.NewClToss_Cod = "0"
            mat.Ditta_Cod = 0
            mat.N = 0
            mat.P2O5 = 0
            mat.K2O = 0
            mat.MgO = 0
            mat.Note = ""
            mat.inviato = 0
            mat.Data_Creazione = DateTime.Now
            mat.Data_Modifica = DateTime.Now
            mat.Validita_Inizio = AGRODATAINIZIO
            mat.Validita_Fine = AGRODATAFINE
            mat.Username_Creazione = username
            mat.Username_Modifica = username
            mat.Regolamento = 0
            mat.Cal_Cod = 0
            mat.Prezzo_Unitario = 0
            mat.Extra_Str = ""
            mat.Extra_Int = 0
            mat.Extra_Date = AGRODATAFINE
            mat.GRVA_COD_VEG = 0
            mat.GEN_COD = 0
            mat.SPE_COD = 0
            mat.RAZ_COD = 0
            mat.IPRO_COD = 0
            mat.CAT_COD = 0
            mat.Flag_Biologico = 0
            mat.Flag_Convenzionale = 0
            mat.Flag_NonAgricolo = 0
            mat.Flag_AusiliareFabbricazione = 0
            mat.Udm_Cod_Extra = 0
            mat.Flag_Extra = 0
            mat.ChkImballaggio = 0
            mat.Qta_Extra = 0
            mat.Taglio = 0
            mat.Mat_Cod_Origine = 0
            mat.Piva_SuperUser_Origine = ""
            mat.ChkListino = 0
            mat.Grfi_Cod = 0
            mat.Codice_Prodotto = 0
            mat.Colore = 0
            mat.Codice_NC = ""
            mat.Manipolazioni = 0
            mat.Titolo_Alcol = 0
            mat.ChkContenitore = 0
            mat.Qta_Contenitore = 0
            mat.Tipo_Peso = 1
            mat.Tara = 0
            mat.Udm_Cod = 0
            mat.Peso_Set = 0
            mat.ChkEscludi_Magazzino = 0
            mat.ChkEscludi_Preparazione = 0
            mat.Id_Accisa_Cod = 0
            mat.Confezione_Cod = ""
            mat.Categoria_Vino_Cod = 0
            mat.Tipo_Reg_Alcoli = ""
            mat.ChkAlias = 0
            mat.Linea_Cod = 0
            mat.Tipo_Default = 0
            mat.ChkStampa_Dettagli = 0
            mat.ChkReferenza = 0
            mat.Mat_Cod_Referenza = 0
            mat.ID_DisciplinareAcquisti = -1
            mat.Lotto_Default = ""
            mat.OTabella_Cod_Base = 0
            mat.Provenienza_TR = "01"
            mat.eBacchus = ""
            mat.Flag_Variazione = 0
            mat.Codice_Esterno = ""
            mat.Flag_Importato = 0

            dal.Materie_Prime.Add(mat)
            dal.SaveChanges()

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return mat

    End Function

    Public Shared Function CreateProdotti_Costi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                ByRef piva As String,
                                                ByRef elemCod As Integer,
                                                ByRef proCod As Integer,
                                                ByRef matCod As Integer,
                                                ByRef username As String
                                                ) As AgronicaCoreEntityFramework_POCO.Prodotti_Costi

        Dim nomeRoutine As String = "CreateProdotti_Costi"
        Dim prod As AgronicaCoreEntityFramework_POCO.Prodotti_Costi

        Try
            prod = New AgronicaCoreEntityFramework_POCO.Prodotti_Costi

            'prod.ID è campo identity auto-assegnato

            prod.Piva = piva
            prod.Elem_Cod = elemCod
            prod.Pro_Cod = proCod
            prod.Mat_Cod = matCod

            prod.Riferimento = ""
            prod.Udm_Cod = 0
            prod.Mezzo = 0
            prod.Prezzo_Unitario = 0
            prod.Veg_Cod = 0
            prod.Cul_Cod = 0
            prod.inviato = 0
            prod.Data_Creazione = DateTime.Now
            prod.Data_Modifica = DateTime.Now
            prod.Validita_Inizio = AGRODATAINIZIO
            prod.Validita_Fine = AGRODATAFINE
            prod.Username_Creazione = username
            prod.Username_Modifica = username
            prod.Id_Budget = 0

            dal.Prodotti_Costi.Add(prod)
            dal.SaveChanges()

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return prod

    End Function

    Public Shared Function CreateCAC_Codifica_ProdottiAziendali(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                ByRef pivaSuperUser As String,
                                                                ByRef piva As String,
                                                                ByRef elemCod As Integer,
                                                                ByRef codGias As Integer,
                                                                ByRef codProdottoCliente As String,
                                                                ByRef codArticolo As String,
                                                                ByRef tipoCodifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                                                ByRef username As String
                                                                ) As AgronicaCoreEntityFramework_POCO.CAC_Codifica_prodottiAziendali

        Dim cac As New AgronicaCoreEntityFramework_POCO.CAC_Codifica_prodottiAziendali

        cac.Piva_SuperUser = pivaSuperUser              'PK
        cac.Elem_Cod = elemCod                          'PK
        cac.Codice_GIAS = codGias                       'PK
        cac.Cod_Prodotto_Cliente = codProdottoCliente   'PK
        cac.Piva = piva                                 'PK
        cac.Cod_Articolo = codArticolo                  'PK
        cac.Tipo_Codifica = CInt(tipoCodifica)          'PK

        cac.Desc_Prodotto_Cliente = ""
        cac.Categoria_Prodotto_Cliente = ""
        cac.Desc_GIAS = ""
        cac.Note = ""
        cac.Username_Creazione = username
        cac.Username_Modifica = username
        cac.Data_Creazione = DateTime.Now
        cac.Data_Modifica = DateTime.Now

        dal.CAC_Codifica_prodottiAziendali.Add(cac)
        dal.SaveChanges()

        Return cac

    End Function

End Class
