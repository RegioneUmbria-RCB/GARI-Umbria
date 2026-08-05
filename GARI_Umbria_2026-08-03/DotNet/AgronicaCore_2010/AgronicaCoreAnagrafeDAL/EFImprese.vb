Imports System.Globalization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

Public Class EFImprese
    Public Shared Function CreateImpreseEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef piva As String,
                                           ByRef rag_soc As String,
                                           ByRef username As String,
                                           Optional ByVal partitaIvaReale As String = "") As AgronicaCoreEntityFramework_POCO.Imprese
        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(piva) Then
            Throw New GiasException("Rilevato carattere non valido nella PIVA:" & piva)
        End If
        Dim impresa As New AgronicaCoreEntityFramework_POCO.Imprese
        impresa.PIVA = piva
        impresa.rag_soc = rag_soc
        impresa.Delega = ""
        impresa.AT_Prevalente = ""
        impresa.Forma_Giuridica = ""
        impresa.Forma_Conduzione = ""
        impresa.Sup_Totale = 0
        impresa.inviato = 0
        impresa.Data_Creazione = DateTime.Now
        impresa.Data_Modifica = DateTime.Now
        impresa.Validita_Inizio = AGRODATAINIZIO
        impresa.Validita_Fine = AGRODATAFINE
        impresa.Username_Creazione = username
        impresa.Username_Modifica = username
        impresa.Validazione = 0
        impresa.Data_Validazione = DateTime.Now
        impresa.UserName_Validazione = ""
        impresa.Blk_Flag = 0
        impresa.Blk_Inizio_Data = DateTime.Now
        impresa.Blk_Inizio_Username = ""
        impresa.Blk_Inizio_Note = ""
        impresa.Blk_Fine_Data = DateTime.Now
        impresa.Blk_Fine_Username = ""
        impresa.Blk_Fine_Note = ""
        impresa.TipoImpresaGerarchia = 1
        impresa.GruppoRaccolta_Cod = 0
        impresa.partitaIvaReale = If(String.IsNullOrEmpty(partitaIvaReale), piva, partitaIvaReale)
        dal.Imprese.Add(impresa)
        dal.SaveChanges()
        Return impresa
    End Function

    Private Shared Function CreateImprese_CodiciEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef id_cod As String,
                                                   ByRef val_cod As String,
                                                   ByRef username As String,
                                                   Optional ByRef validita_inizio As Date = AGRODATAINIZIO,
                                                   Optional ByRef validita_fine As Date = AGRODATAFINE) As AgronicaCoreEntityFramework_POCO.Imprese_Codici
        Dim impresa As New AgronicaCoreEntityFramework_POCO.Imprese_Codici

        impresa.PIVA = piva

        impresa.id_cod = id_cod
        impresa.val_cod = val_cod

        impresa.inviato = 0
        impresa.datainvio = DateTime.Now

        impresa.Data_Creazione = DateTime.Now
        impresa.Data_Modifica = DateTime.Now

        impresa.Validita_Inizio = validita_inizio
        impresa.Validita_Fine = validita_fine

        impresa.Username_Creazione = username
        impresa.Username_Modifica = username

        impresa.Validazione = 0
        impresa.Data_Validazione = DateTime.Now
        impresa.UserName_Validazione = ""

        dal.Imprese_Codici.Add(impresa)

        dal.SaveChanges()
        Return impresa
    End Function

    Public Shared Function CreateImprese_CodiciEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                                   ByRef id_cod As String,
                                                   ByRef val_cod As String,
                                                   ByRef username As String,
                                                   Optional ByRef validita_inizio As Date = AGRODATAINIZIO,
                                                   Optional ByRef validita_fine As Date = AGRODATAFINE) As AgronicaCoreEntityFramework_POCO.Imprese_Codici
        Dim impresa_Codice = CreateImprese_CodiciEF(dal, objParametri, impresa.PIVA, id_cod, val_cod, username, validita_inizio, validita_fine)
        dal.SaveChanges()
        Return impresa_Codice
    End Function

    Public Shared Function CreateGerarchiaImprese(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                   ByRef pivaPadre As String,
                                                   ByRef pivaFiglio As String,
                                                   ByRef Foglia As Integer,
                                                   ByRef Livello As Integer,
                                                   ByRef username As String,
                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                   Optional ByRef Codice_Iscrizione As String = "",
                                                   Optional ByRef Data_Iscrizione As Date = AGRODATAINIZIO) As AgronicaCoreEntityFramework_POCO.GerarchiaImprese
        Dim gerarchiaImpresa As New AgronicaCoreEntityFramework_POCO.GerarchiaImprese
        gerarchiaImpresa.Padre = pivaPadre
        gerarchiaImpresa.Figlio = pivaFiglio
        gerarchiaImpresa.Foglia = Foglia
        gerarchiaImpresa.Livello = Livello
        gerarchiaImpresa.LibroSoci_Codice = Codice_Iscrizione
        gerarchiaImpresa.LibroSoci_DataIscrizione = Data_Iscrizione
        gerarchiaImpresa.inviato = 0
        gerarchiaImpresa.Datainvio = DateTime.Now
        gerarchiaImpresa.Data_Creazione = DateTime.Now
        gerarchiaImpresa.Data_Modifica = DateTime.Now
        gerarchiaImpresa.Validita_Inizio = AGRODATAINIZIO
        gerarchiaImpresa.Validita_Fine = AGRODATAFINE
        gerarchiaImpresa.Username_Creazione = username
        gerarchiaImpresa.Username_Modifica = username
        dal.GerarchiaImprese.Add(gerarchiaImpresa)
        dal.SaveChanges()

        Dim handleGerarchiaImpr As New GerarchiaImprese_W
        handleGerarchiaImpr.AggiornaUtentiProfili(objParametri_Server, objParametri_Utenti, pivaPadre, pivaFiglio)

        Return gerarchiaImpresa
    End Function

    Public Shared Function CreateGerarchiaImprese(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef padre As AgronicaCoreEntityFramework_POCO.Imprese,
                                                   ByRef figlio As AgronicaCoreEntityFramework_POCO.Imprese,
                                                   ByRef Foglia As Integer,
                                                   ByRef Livello As Integer,
                                                   ByRef Codice_Iscrizione As String,
                                                   ByRef Data_Iscrizione As Date,
                                                   ByRef username As String,
                                                   ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.GerarchiaImprese
        Dim ger = CreateGerarchiaImprese(dal, objParametri, padre.PIVA, figlio.PIVA, Foglia, Livello, username, objParametri_Utenti, Codice_Iscrizione, Data_Iscrizione)
        Return ger
    End Function

    Public Shared Function CreateUtentixImprese(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef user As String,
                                                   ByRef piva As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.UtentiXImprese
        Dim uxi As New AgronicaCoreEntityFramework_POCO.UtentiXImprese
        uxi.USER = user
        uxi.PIVA = piva
        uxi.inviato = 0
        uxi.Data_Creazione = DateTime.Now
        uxi.Data_Modifica = DateTime.Now
        uxi.Validita_Inizio = AGRODATAINIZIO
        uxi.Validita_Fine = AGRODATAFINE
        uxi.Username_Creazione = username
        uxi.Username_Modifica = username
        uxi.Validazione = 0
        uxi.Data_Validazione = DateTime.Now
        uxi.UserName_Validazione = ""
        dal.UtentiXImprese.Add(uxi)
        dal.SaveChanges()
        Return uxi
    End Function

    Public Shared Function CreaImpresaCompleta(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef piva As String,
                                           ByRef rag_soc As String,
                                           ByRef Codice_Iscrizione As String,
                                           ByRef Data_Iscrizione As Date,
                                           ByRef username As String,
                                           ByRef PivaPadre As String,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.Imprese
        Dim impresa = CreateImpreseEF(dal, objParametri, piva, rag_soc, username)
        CreateGerarchiaImprese(dal, objParametri, PivaPadre, piva, 1, 2, username, objParametri_Utenti, Codice_Iscrizione, Data_Iscrizione)
        CreateUtentixImprese(dal, objParametri, PivaPadre, piva, PivaPadre)
        Return impresa
    End Function

    Public Shared Function CreaContattoImpresa(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                           ByRef cod_contatto As String,
                                           ByRef Sa_Cod As Integer,
                                           ByRef id_CF As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Contatti
        Return EFContatti.CreateContattiEF(dal, objParametri, impresa.PIVA, Sa_Cod, cod_contatto, id_CF, username)
    End Function

    Public Shared Function CreateIndirizzoImpresa(ByRef dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                 ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                           ByRef tipoIndirizzo As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Indirizzi
        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)
        Dim imxin = CreateImpresexIndirizzi(dal, objParametri, impresa.PIVA, indirizzo.cod_indirizzo, tipoIndirizzo, username)

        indirizzo.ind_des = dati_indirizzo.via
        indirizzo.frz_des = dati_indirizzo.frazione
        indirizzo.CAP = dati_indirizzo.cap

        If dati_indirizzo.istatComune.localita IsNot Nothing AndAlso dati_indirizzo.istatComune.localita <> "" Then
            indirizzo.com_des = dati_indirizzo.istatComune.localita
        End If

        If dati_indirizzo.istatComune.comuni_prov IsNot Nothing AndAlso dati_indirizzo.istatComune.comuni_prov <> "" Then
            indirizzo.pro_cod = dati_indirizzo.istatComune.comuni_prov
        End If

        indirizzo.stato = dati_indirizzo.stato.codice
        indirizzo.note = dati_indirizzo.note
        indirizzo.pro_cod_istat = dati_indirizzo.istatComune.prov
        indirizzo.com_cod_istat = dati_indirizzo.istatComune.com

        dal.SaveChanges()

        Return indirizzo
    End Function

    Public Shared Function CreateIndirizzoImpresa(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                           ByRef tipoIndirizzo As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Indirizzi

        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)
        Dim imxin = CreateImpresexIndirizzi(dal, objParametri, impresa.PIVA, indirizzo.cod_indirizzo, tipoIndirizzo, username)
        Return indirizzo
    End Function

    Private Shared Function CreateImpresexIndirizzi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef piva As String,
                                           ByRef codIndirizzo As Integer,
                                           ByRef tipoIndirizzo As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.ImpresexIndirizzi
        Dim imxin = New AgronicaCoreEntityFramework_POCO.ImpresexIndirizzi
        imxin.PIVA = piva
        imxin.cod_indirizzo = codIndirizzo
        imxin.Tipo_Indirizzo = tipoIndirizzo
        imxin.inviato = 0
        imxin.Data_Creazione = DateTime.Now
        imxin.Data_Modifica = DateTime.Now
        imxin.Validita_Inizio = AGRODATAINIZIO
        imxin.Validita_Fine = AGRODATAFINE
        imxin.Username_Creazione = username
        imxin.Username_Modifica = username
        imxin.Validazione = 0
        imxin.Data_Validazione = DateTime.Now
        imxin.UserName_Validazione = ""
        dal.ImpresexIndirizzi.Add(imxin)
        dal.SaveChanges()
        Return imxin
    End Function

    Public Shared Function CreateLiquiditaImprese(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                           ByRef riferimento As String,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Liquidita
        Dim liq = EFLiquidita.CreateLiquidita(dal, objParametri, impresa.PIVA, 0, riferimento, username)
        Return liq
    End Function

    Public Shared Function CreaRisorseUmaneImpresa(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                       ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                       ByRef username As String) As AgronicaCoreEntityFramework_POCO.Risorse_Umane

        Return EFContatti.CreateRisorse_Umane(dal, objParametri, contatto, username)
    End Function

    Public Shared Function ImpresaExist(ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                        ByVal PartitaIva As String) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFImprese.ImpresaExist()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            'Lavez - 29/05/2025 - refactoring
            'Dim impresaList = From impresa In GiasContext.Imprese
            '                  Where impresa.PIVA = PartitaIva
            '                  Select impresa

            'Dim impres = impresaList.FirstOrDefault
            'If (impres IsNot Nothing) Then
            '    ret = True
            'End If
            Dim Exists = (From impresa In GiasContext.Imprese
                          Where impresa.PIVA = PartitaIva
                          Select 1).Any()
            If Exists Then
                ret = True
            End If
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret

    End Function

End Class