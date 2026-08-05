Imports System.Data.Entity
Imports System.Globalization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL

Public Class Conf_Servizi_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal pivaSuperUser As String, ByVal tipoServizio As enum_Tipi_Servizi_Background) As List(Of Configurazione_Servizio)

        Try

            Dim retVal = New List(Of Configurazione_Servizio)

            Dim csr = New Configurazione_Servizi_R()
            Dim servizi = csr.Leggi(pivaSuperUser, enum_Id_Servizio.GiasOnline, tipoServizio, 0, False, _objParametriServer.StringaConnessione)

            If servizi.Rows.Count = 0 Then
                Return Nothing
            End If

            For Each row As DataRow In servizi.Rows
                retVal.Add(New Configurazione_Servizio With
                           {
                                .DirectoryFileEsportazioni = row.Item("DirectoryFileEsportazioni").ToString(),
                                .DirectoryLOG = row.Item("DirectoryLOG").ToString(),
                                .Parametri_Extra = row.Item("Parametri_Extra").ToString(),
                                .Id_Cod_Cliente = CInt(row.Item("Id_Cod_Cliente"))
                            })

            Next row

            Return retVal

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Conf_Servizi_R.Leggi")
        End Try

    End Function

End Class

Public Class Mov_Det_Tecnico_Extra_R :: Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Function Leggi(ByVal PIVA As String, ByVal idAgenda As Integer) As Mov_Dettaglio_Tecnico_Extra

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From mdte In dal.Mov_Dettaglio_Tecnico_Extra
                      Where mdte.Piva.Equals(PIVA) AndAlso mdte.Id_Mov_Det = 0 AndAlso mdte.Id_Agenda.Equals(idAgenda)

            Return qry.FirstOrDefault()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Mov_Det_Tecnico_Extra_R.Leggi")
        End Try
    End Function


End Class

Public Class Province_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Function Leggi() As List(Of Lista_Province)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From provincia In dal.Lista_Province

            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Province_R.Leggi")
        End Try
    End Function

End Class

Public Class Comuni_R : Inherits EFatturaBaseDAL


    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As List(Of ISTAT)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Return (From comuni In dal.ISTAT).ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Comuni_R.Leggi")
        End Try
    End Function

End Class

Public Class TipologieDocumento_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As List(Of TipologiaDocumento)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From tipo In dal.TipologieDocumento
                      Select New TipologiaDocumento With {
                          .Codice = tipo.Codice,
                          .Descrizione = tipo.Descrizione,
                          .SiglaAE = tipo.SiglaAE
                      }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "TipologieDocumento_R.Leggi")
        End Try
    End Function
End Class

Public Class RegimiFiscali_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal PIVA As String) As List(Of RegimeFiscaleXSezionale)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From isz In dal.Imprese_Sezionali
                      Group Join rf In dal.RegimiFiscali
                      On isz.RegimeFiscale_Cod Equals rf.Codice
                      Into rf_group = Group
                      From _rf_group In rf_group.DefaultIfEmpty()
                      Where isz.Piva.Equals(PIVA)
                      Select New RegimeFiscaleXSezionale With
                        {
                            .PIVA = isz.Piva,
                            .CodiceSezionale = isz.Sezionale_Cod,
                            .EsigibilitaIva = isz.EsigibilitaIva,
                            .CodiceRegimeFiscale = _rf_group.Codice,
                            .SiglaAE = _rf_group.SiglaAE
                        }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "RegimiFiscali_R.Leggi")
        End Try
    End Function

End Class

Public Class Contatti_Codici_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal PIVA As String) As List(Of Contatto_Codice)

        Dim codiciValidi As Int32() =
            {
                 enum_CodiciAnagrafe.TipoContattoFattura,
                 enum_CodiciAnagrafe.PecContatto,
                 enum_CodiciAnagrafe.CodiceSDI,
                 enum_CodiciAnagrafe.RappresentanteFiscale,
                 enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo,
                 enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione
            }

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From cc In dal.Contatti_Codici
                      Where codiciValidi.Contains(cc.Id_cod) _
                      AndAlso ((cc.PIVA.Equals(PIVA) AndAlso cc.Sa_Cod = 0) OrElse cc.Sa_Cod = -1)

            Return qry.Select(Function(s) New Contatto_Codice With
                                  {
                                    .Id_cod = s.Id_cod,
                                    .Sa_Cod = s.Sa_Cod,
                                    .PIVA = s.PIVA,
                                    .Val_cod = s.Val_cod,
                                    .Cod_Contatto = s.Cod_Contatto
                                  }).ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Contatti_Codici_R.Leggi")
        End Try

    End Function

End Class

Public Class UnitaMisura_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As List(Of UnitaMisura)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From um In dal.UnitaMisura Select um

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "UnitaMisura_R.Leggi")
        End Try

    End Function

End Class

Public Class AliquotaIVA_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As List(Of IVA)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From iva In dal.IVA_Aliquote
                      Select New IVA With
                          {
                            .Codice = iva.Codice,
                            .Aliquota = iva.Aliquota,
                            .Descrizione = iva.Descrizione,
                            .NaturaEsclusione = iva.NaturaEsclusione_2
                          }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "AliquotaIVA_R.Leggi")
        End Try

    End Function

End Class

Public Class RappresentantiFiscali_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal PIVA As String) As List(Of RappresentanteFiscale)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From cc In dal.Contatti_Codici
                      Group Join rm In dal.Risorse_Umane
                      On cc.Val_cod Equals rm.Cod_RisUm
                      Into rm_grup = Group
                      From _rm_group In rm_grup.DefaultIfEmpty
                      Group Join c In dal.Contatti
                      On c.Cod_Contatto Equals _rm_group.Cod_Contatto
                      Into c_group = Group
                      From _c_group In c_group.DefaultIfEmpty
                      Where cc.Id_cod = enum_CodiciAnagrafe.RappresentanteFiscale AndAlso
                      cc.Val_cod <> "" AndAlso cc.Val_cod <> "0" AndAlso
                      cc.PIVA.Equals(PIVA)
                      Select New RappresentanteFiscale With
                          {
                              .Cod_Contatto = _c_group.Cod_Contatto,
                              .Cod_RisUm = _rm_group.Cod_RisUm,
                              .Rag_Soc = _c_group.Rag_Soc
                          }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "RappresentantiFiscali_R.Leggi")
        End Try

    End Function

End Class

Public Class StabileOrganizzazione_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal PIVA As String, ByVal risorseUmane As List(Of Integer)) As List(Of StabileOrganizzazione)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From ci In dal.ContattiXIndirizzi
                      Group Join c In dal.Contatti
                      On ci.Cod_Contatto Equals c.Cod_Contatto
                      Into C_group = Group
                      From _c_group In C_group.DefaultIfEmpty
                      Group Join rm In dal.Risorse_Umane
                      On rm.Cod_Contatto Equals _c_group.Cod_Contatto
                      Into rm_group = Group
                      From _rm_group In rm_group.DefaultIfEmpty
                      Group Join ind In dal.Indirizzi
                      On ind.cod_indirizzo Equals ci.Cod_Indirizzo
                      Into ind_group = Group
                      From _ind_group In ind_group.DefaultIfEmpty
                      Where ci.Piva.Equals(PIVA) AndAlso _c_group.Id_CF = enum_Contatti_IdCf.ContattoEstero AndAlso
                      ci.Tipo_Indirizzo = enum_TipiIndirizzi.StabileOrganizzazione AndAlso
                      risorseUmane.Contains(_rm_group.Cod_RisUm)
                      Select New StabileOrganizzazione With
                          {
                                .Cod_Contatto = ci.Cod_Contatto,
                                .Cod_RisUm = _rm_group.Cod_RisUm,
                                .Indirizzo = New Indirizzo With
                                {
                                    .CAP = _ind_group.CAP,
                                    .com_cod_istat = _ind_group.com_cod_istat,
                                    .com_des = _ind_group.com_des,
                                    .frz_des = _ind_group.frz_des,
                                    .ind_des = _ind_group.ind_des,
                                    .pro_cod = _ind_group.pro_cod,
                                    .stato = _ind_group.stato,
                                    .pro_cod_istat = _ind_group.pro_cod_istat
                                }
                          }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "StabileOrganizzazione_R.Leggi")
        End Try
    End Function


End Class

Public Class ModuloGenerazione_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi() As enum_Omni_Modulo_Generazione

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim modulo = (From mg In dal.OGenerazioni_Anagrafe_Moduli_Log).FirstOrDefault()

            If modulo IsNot Nothing Then
                Return modulo.Modulo_Generazione
            Else
                Return enum_Omni_Modulo_Generazione.Nessuno
            End If
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ModuloGenerazione_R.Leggi")
        End Try

    End Function

End Class

Public Class Imprese_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function Leggi(ByVal PIVA As String) As enum_TipoImpresaGerarchia

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim impresa = (From imp In dal.Imprese Where imp.PIVA.Equals(PIVA)).FirstOrDefault()

            If impresa IsNot Nothing Then
                Return impresa.TipoImpresaGerarchia
            Else
                Return enum_TipoImpresaGerarchia.Impresa
            End If

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Imprese_R.Leggi")
        End Try

    End Function

End Class

Public Class Imprese_Codici_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function LeggiDataAttivazione(ByVal PIVA As String) As DateTime

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim recordAttivazione = (From ic In dal.Imprese_Codici
                                     Where ic.id_cod = enum_CodiciAnagrafe.DataAttivazioneEFattura _
                      AndAlso ic.PIVA = PIVA).FirstOrDefault()

            If recordAttivazione Is Nothing Then
                Return DateTime.MinValue
            Else
                Dim data = DateTime.ParseExact(recordAttivazione.val_cod, "yyyyMMdd", CultureInfo.InvariantCulture)
                Return New DateTime(data.Year, data.Month, data.Day, 0, 0, 0)
            End If
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Imprese_Codici_R.LeggiDataAttivazione")
        End Try

    End Function

    Public Function Leggi(ByVal PIVA As String, ByVal codice As Integer) As String

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim result = (From ic In dal.Imprese_Codici Where ic.id_cod = codice AndAlso ic.PIVA = PIVA).FirstOrDefault()

            If IsNothing(result) Then
                Return ""
            End If

            Return result.val_cod
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Imprese_Codici_R.Leggi")
        End Try
    End Function

End Class

Public Class Imprese_Codici_W : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub Scrivi(ByVal PIVA As String, ByVal codice As Integer, ByVal valore As String)

        Try

            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim imprese_codici = (From a In dal.Imprese_Codici Where a.PIVA = PIVA AndAlso a.id_cod = codice).FirstOrDefault

            If imprese_codici Is Nothing Then
                imprese_codici = New Imprese_Codici With {
                    .PIVA = PIVA,
                    .id_cod = codice,
                    .val_cod = valore,
                    .inviato = 0,
                    .Data_Creazione = DateTime.Now,
                    .Data_Modifica = DateTime.Now,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE,
                    .Username_Creazione = _objParametriServer.UsernameOperazione,
                    .Username_Modifica = _objParametriServer.UsernameOperazione,
                    .Validazione = 0,
                    .Data_Validazione = DateTime.Now,
                    .UserName_Validazione = ""
                }
                dal.Imprese_Codici.Add(imprese_codici)
            Else
                imprese_codici.val_cod = valore
                imprese_codici.Data_Modifica = DateTime.Now
                imprese_codici.Username_Creazione = _objParametriServer.UsernameOperazione
                dal.Entry(imprese_codici).State = EntityState.Modified
            End If

            dal.SaveChanges()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Imprese_Codici_W.Scrivi")
        End Try

    End Sub

End Class

Public Class Imprese_Sezionali_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Function Leggi(ByVal PIVA As String) As List(Of Imprese_Sezionali)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim qry = From sez In dal.Imprese_Sezionali
                      Where sez.Piva.Equals(PIVA)

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Imprese_Sezionali_R.Leggi")
        End Try
    End Function

End Class
