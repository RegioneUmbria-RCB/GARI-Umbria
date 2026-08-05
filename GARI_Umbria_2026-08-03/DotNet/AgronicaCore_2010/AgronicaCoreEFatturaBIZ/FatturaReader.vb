Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL

Public Class DataReader : Implements IDataReader

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri

    Private ReadOnly _debug As Boolean
    Public Sub New(
                  ByVal objParametriServer As AgronicaCoreParametri,
                  ByVal objParametriUtente As AgronicaCoreParametri,
                  ByVal objParametriSuperServer As AgronicaCoreParametri,
                  ByVal debug As Boolean)
        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _debug = debug
    End Sub

    Public Function LeggiAgendeXLavCod(filtro As AgendaFiltroLettura) As GenerazioneFatture Implements IDataReader.LeggiAgendeXLavCod

        Dim dal = New Agenda_R(_objParametriServer)
        Return dal.LeggiAgendeXLavCod(filtro)

    End Function

    Public Function LeggiAgenda(ByVal agenda As AgendaXLavCod, ByVal tipoImpresa As enum_TipoImpresaGerarchia) As FatturaGiasFromDB Implements IDataReader.LeggiAgenda

        Dim dal = New Agenda_R(_objParametriServer)
        Return dal.LeggiAgenda(agenda, tipoImpresa, _debug)

    End Function

    Public Function CaricaProvince() As List(Of Lista_Province) Implements IDataReader.CaricaProvince

        Dim provinceDal = New Province_R(_objParametriServer)
        Return provinceDal.Leggi()

    End Function

    Public Function CaricaTipologieDocumento() As List(Of TipologiaDocumento) Implements IDataReader.CaricaTipologieDocumento

        Dim tipoDocDal As New TipologieDocumento_R(_objParametriServer)
        Return tipoDocDal.Leggi()

    End Function

    Public Function CaricaRegimiFiscali(ByVal PIVA As String) As List(Of RegimeFiscaleXSezionale) Implements IDataReader.CaricaRegimiFiscali

        Dim rfDal = New RegimiFiscali_R(_objParametriServer)
        Return rfDal.Leggi(PIVA)

    End Function

    Public Function CaricaContattiCodici(ByVal PIVA As String) As List(Of Contatto_Codice) Implements IDataReader.CaricaContattiCodici

        Dim ccDal = New Contatti_Codici_R(_objParametriServer)
        Return ccDal.Leggi(PIVA)

    End Function

    Public Function CaricaUnitaMisura() As List(Of UnitaMisura) Implements IDataReader.CaricaUnitaMisura

        Dim umDal = New UnitaMisura_R(_objParametriServer)
        Return umDal.Leggi()

    End Function

    Public Function CaricaAliquteIVA() As List(Of IVA) Implements IDataReader.CaricaAliquteIVA

        Dim ivaDal = New AliquotaIVA_R(_objParametriServer)
        Return ivaDal.Leggi()

    End Function

    Public Function CaricaRappresentantiFiscali(PIVA As String) As List(Of RappresentanteFiscale) Implements IDataReader.CaricaRappresentantiFiscali

        Dim rfDal = New RappresentantiFiscali_R(_objParametriServer)
        Return rfDal.Leggi(PIVA)

    End Function

    Public Function CaricaStabiliOrganizzazioni(PIVA As String, risorseUmane As List(Of Integer)) As Object Implements IDataReader.CaricaStabiliOrganizzazioni

        Dim soDal = New StabileOrganizzazione_R(_objParametriServer)
        Return soDal.Leggi(PIVA, risorseUmane)

    End Function

    Public Function CaricaComuni() As List(Of ISTAT) Implements IDataReader.CaricaComuni

        Dim comDal = New Comuni_R(_objParametriServer)
        Return comDal.Leggi()

    End Function

    Public Function LeggiModuloGenerazione() As enum_Omni_Modulo_Generazione Implements IDataReader.LeggiModuloGenerazione
        Dim dal = New ModuloGenerazione_R(_objParametriServer)
        Return dal.Leggi()
    End Function

    Public Function LeggiDescrizioneBeneServizio(ByVal movimento As Movimenti_dettagli,
                                                 ByRef flag_extra As Boolean,
                                                 ByVal modulo_generazione As enum_Omni_Modulo_Generazione) As String Implements IDataReader.LeggiDescrizioneBeneServizio

        Dim dal = New DescrizioneBeniServizi_R(_objParametriServer, _objParametriUtente)
        Return dal.Leggi(movimento, flag_extra, modulo_generazione)

    End Function

    Public Function LeggiDataAttivazione(ByVal PIVA As String) As DateTime Implements IDataReader.LeggiDataAttivazione

        Dim dal = New Imprese_Codici_R(_objParametriServer)
        Return dal.LeggiDataAttivazione(PIVA)

    End Function

    Public Function LeggiConfigurazioneServizi(ByVal pivaSuperUser As String, tipoServizio As enum_Tipi_Servizi_Background) As List(Of Configurazione_Servizio) Implements IDataReader.LeggiConfigurazioneServizi

        Dim dal = New Conf_Servizi_R(_objParametriSuperServer)
        Return dal.Leggi(pivaSuperUser, tipoServizio)

    End Function

    Public Function LeggiTipoImpresaGerarchia(ByVal PIVA As String) As enum_TipoImpresaGerarchia Implements IDataReader.LeggiTipoImpresaGerarchia
        Dim dal = New Imprese_R(_objParametriServer)
        Return dal.Leggi(PIVA)
    End Function

    Public Function CaricaSezionali(PIVA As String) As List(Of Imprese_Sezionali) Implements IDataReader.CaricaSezionali
        Dim sezDal = New Imprese_Sezionali_R(_objParametriServer)
        Return sezDal.Leggi(PIVA)
    End Function

    Function LeggiAgendeErrateDopoXGiorni(ByVal PIVA As String, ByVal periodoControlloGG As Integer) As ControlloPreliminare Implements IDataReader.LeggiAgendeErrateDopoXGiorni

        Dim logDal = New SDI_Log_R(_objParametriServer)
        Return logDal.LeggiAgendeErrateDopoXGiorni(PIVA, periodoControlloGG)


    End Function

End Class

