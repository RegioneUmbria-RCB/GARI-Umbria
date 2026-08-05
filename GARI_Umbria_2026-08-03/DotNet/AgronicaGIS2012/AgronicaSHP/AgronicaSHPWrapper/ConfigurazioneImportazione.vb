
Imports AgronicaSHPWrapper.InterpretaDatiDBF

Public Class ConfigurazioneImportazione
    Public Sub New(
            shapeFile_fullFileName As String,
            PivaSuperUSer As String,
            piva As String,
            sa_cod As Integer,
            campo_cod As Integer,
            appezza As Integer,
            reg_impianto As Integer,
            programmazione_cod As Integer,
            lav_cod As Integer,
            lav_des As String,
            cod_risum As Integer,
            cod_mac As Integer,
            Layer_cod As Integer,
            tipo_importazione_agronica As Tipo_Importazione.Tipo_Importazione_ShapeFile,
            anno As Integer,
            ASG_Utente_Username As String,
            ASG_Utente_Password As String,
            ProgressivoGias As String,
            trasformaSistemaRiferimento As Boolean,
            GEORiferimento_COD As Integer,
            lTipoEntita_Cod As String,
            Codice_Fiscale_Tecnico As String,
            Optional ByVal GestioneRiportoDatiInGias As Tipo_Importazione.Tipo_GestioneRiportoDatiInGias = Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoAutomaticoDeiDati)

        ShapeFileFullFileName = shapeFile_fullFileName
        Me.PivaSuperUSer = PivaSuperUSer
        Me.Piva = piva
        SaCod = sa_cod
        CampoCod = campo_cod
        Me.Appezza = appezza
        RegImpianto = reg_impianto
        ProgrammazioneCod = programmazione_cod
        LavCod = lav_cod
        LavDes = lav_des
        CodRisum = cod_risum
        CodMac = cod_mac
        LayerCod = Layer_cod
        TipoImportazioneAgronica = tipo_importazione_agronica
        Me.Anno = anno
        AsgUtenteUsername = ASG_Utente_Username
        AsgUtentePassword = ASG_Utente_Password
        Me.ProgressivoGias = ProgressivoGias
        Me.TrasformaSistemaRiferimento = trasformaSistemaRiferimento
        GeoRiferimentoCod = GEORiferimento_COD
        LTipoEntitaCod = lTipoEntita_Cod
        CodiceFiscaleTecnico = Codice_Fiscale_Tecnico
        Me.GestioneRiportoDatiInGias = GestioneRiportoDatiInGias
        Me.Allegati_Documenti_Cod = 0
        Me.Ricetta_Operazione_Cod = 0
        Me.Allegati_Documenti_Cod_collegamento = 0
    End Sub


    Public Property CategoriaDocumento As AgronicaCoreDataProvider.TipiEnumerativi.enum_CategorieDocumenti
    Public Property GestioneRiportoDatiInGias As Tipo_Importazione.Tipo_GestioneRiportoDatiInGias
    Public Property ShapeFileFullFileName As String
    Public Property PivaSuperUSer As String
    Public Property Piva As String
    Public Property SaCod As Integer
    Public Property CampoCod As Integer
    Public Property Appezza As Integer
    Public Property RegImpianto As Integer
    Public Property ProgrammazioneCod As Integer
    Public Property LavCod As Integer
    Public Property LavDes As String
    Public Property CodRisum As Integer
    Public Property CodMac As Integer
    Public Property LayerCod As Integer
    Public Property TipoImportazioneAgronica As Tipo_Importazione.Tipo_Importazione_ShapeFile
    Public Property Anno As Integer
    Public Property AsgUtenteUsername As String
    Public Property AsgUtentePassword As String
    Public Property ProgressivoGias As String
    Public Property TrasformaSistemaRiferimento As Boolean
    Public Property GeoRiferimentoCod As Integer
    Public Property LTipoEntitaCod As String
    Public Property CodiceFiscaleTecnico As String
    Public Property ConfigurazioneImportazione_Catasto As ConfigurazioneImportazione_Catasto
    Public Property Allegati_Documenti_Cod As Integer
    Public Property OUTPUT_Allegati_Documenti_Cod As Integer
    Public Property Ricetta_Operazione_Cod As Integer

    Public Property Allegati_Documenti_Cod_collegamento As Integer

    Public Property DataInizioValidita As Date
    Public Property DataFineValidita As Date
    Public Property DescrizioneFile As String
    Public Property PixelSize As Int32
End Class

Public Class ConfigurazioneImportazione_Catasto

    Public Property CodBelfiore As String
    Public Property Prov As String

    Public Property Com As String
    Public Property Sezione As String
    Public Property Foglio As String
    Public Property Particella As String
    Public Property Subalterno As String
    Public Property IdentificativoEsterno As String
    Public Property AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge As Int16
    Public Property CreaLayerTestuale As Boolean
    Public Property FiltroParticelleCatastali As String
    Public Property ListaLayersDXFAgenziaEntrate As String

End Class
