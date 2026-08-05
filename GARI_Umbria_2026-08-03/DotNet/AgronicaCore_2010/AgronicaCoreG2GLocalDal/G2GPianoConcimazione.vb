Imports System.Transactions

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity

Public Class G2GPianoConcimazione_R

    Public Function LeggiPerGias2Gias(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal Piva_Destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_PianoConcimazione


        Dim NomeRoutine As String = "G2GlocalDal.G2GPianoConcimazione_R.LeggiPerGias2Gias()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_PianiConcimazione
        If objOpzioniImportImpresa.configurazione_piano_concimazione <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_PianiConcimazione)(objOpzioniImportImpresa.configurazione_piano_concimazione)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_PianiConcimazione
            oConfigurazione.listaPianoConcimazione_Tipo = New List(Of Integer)
        End If

        GiasContext.Database.CommandTimeout = 3600
        Dim rval As New G2G_PianoConcimazione

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_PianoConcimazione_Testata_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Testata)
            .G2G_PianoConcimazione_Testata_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Testata)
            .G2G_PianoConcimazione_Dettagli_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Dettagli)
            .G2G_PianoConcimazione_Dettagli_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Dettagli)
            .G2G_PianoConcimazione_Elaborazioni_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
            .G2G_PianoConcimazione_Elaborazioni_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
            .G2G_PianoConcimazione_EntitaxTestata_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)
            .pianoconcimazione_dettagli_delete = New List(Of PianoConcimazione_Dettagli)
            .pianoconcimazione_elaborazioni_delete = New List(Of PianoConcimazione_Elaborazioni)
            .pianoconcimazione_entitaxtestata_delete = New List(Of PianoConcimazione_EntitaxTestata)
            .pianoconcimazione_testata_delete = New List(Of PianoConcimazione_Testata)
            .To_Piva = Piva_Destinazione
        End With

        Dim pianoconcimazione_testata_cod_update As New List(Of Integer)

        'Elaborazioni
        rval.pianoconcimazione_elaborazioni_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Join b In GiasContext.PianoConcimazione_Elaborazioni On t.PC_Elaborazione_Cod Equals b.PC_Elaborazione_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Any(Function(g) g.From_PivaSuperUser = t.PC_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_PC_Elaborazione_Cod = b.PC_Elaborazione_Cod)
            Select b).Distinct.ToList()

        rval.pianoconcimazione_elaborazioni_update = (From p In GiasContext.PianoConcimazione_Elaborazioni
                                                      Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni
                                                          On p.PC_Elaborazione_Cod Equals g2g.From_PC_Elaborazione_Cod _
                                                          And p.PC_SuperUser Equals g2g.From_PivaSuperUser
                                                      Join pc_testata In GiasContext.PianoConcimazione_Testata
                                                          On pc_testata.PC_SuperUser Equals g2g.From_PivaSuperUser _
                                                          And pc_testata.PC_Elaborazione_Cod Equals g2g.From_PC_Elaborazione_Cod
                                                      Join pc_entita In GiasContext.PianoConcimazione_Dettagli
                                                          On pc_entita.PC_Testata_Cod Equals pc_testata.PC_Testata_Cod _
                                                          And pc_entita.PC_Dettagli_SuperUser Equals pc_testata.PC_SuperUser
                                                      Where pc_entita.PC_Dettagli_PIVA = piva _
                                                          AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
                                                      Select p).Distinct.ToList()

        rval.G2G_PianoConcimazione_Elaborazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni
            Where Not GiasContext.PianoConcimazione_Elaborazioni.Any(Function(p) p.PC_Elaborazione_Cod = r.From_PC_Elaborazione_Cod And p.PC_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Testate
        rval.pianoconcimazione_testata_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Testata.Any(Function(g) g.From_PivaSuperUser = t.PC_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_PC_Testata_Cod = t.PC_Testata_Cod)
            Select t).Distinct.ToList()

        'rval.pianoconcimazione_testata_update = New List(Of PianoConcimazione_Testata)
        rval.pianoconcimazione_testata_update = (
        From p In GiasContext.PianoConcimazione_Testata
        Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Testata
                On p.PC_Testata_Cod Equals g2g.From_PC_Testata_Cod _
                And p.PC_SuperUser Equals g2g.From_PivaSuperUser
        Join pc_entita In GiasContext.PianoConcimazione_Dettagli
                On pc_entita.PC_Testata_Cod Equals p.PC_Testata_Cod _
                And pc_entita.PC_Dettagli_SuperUser Equals p.PC_SuperUser
        Where pc_entita.PC_Dettagli_PIVA = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
        Select p
            ).Distinct.ToList()

        For Each testata_update In rval.pianoconcimazione_testata_update
            pianoconcimazione_testata_cod_update.Add(testata_update.PC_Testata_Cod)
        Next

        'rval.G2G_PianoConcimazione_Testata_Recode_delete = New List(Of G2G_Recode_PianoConcimazione_Testata)
        rval.G2G_PianoConcimazione_Testata_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Testata
            Where Not GiasContext.PianoConcimazione_Testata.Any(Function(p) p.PC_Testata_Cod = r.From_PC_Testata_Cod And p.PC_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Dettagli
        rval.pianoconcimazione_dettagli_insert = (
            From t In GiasContext.PianoConcimazione_Testata
            Join d In GiasContext.PianoConcimazione_Dettagli On t.PC_Testata_Cod Equals d.PC_Testata_Cod
            Where d.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Any(Function(g) g.From_PivaSuperUser = d.PC_Dettagli_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_PC_Testata_Cod = d.PC_Testata_Cod And g.From_PC_Dettagli_Cod = d.PC_Dettagli_Cod)
            Select d).Distinct.ToList()

        rval.pianoconcimazione_dettagli_update = (
            From p In GiasContext.PianoConcimazione_Dettagli
            Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Dettagli
                On p.PC_Testata_Cod Equals g2g.From_PC_Testata_Cod _
                And p.PC_Dettagli_Cod Equals g2g.From_PC_Dettagli_Cod _
                And p.PC_Dettagli_SuperUser Equals g2g.From_PivaSuperUser
            Join pc_entita In GiasContext.PianoConcimazione_EntitaxTestata
                On pc_entita.PC_Testata_Cod Equals p.PC_Testata_Cod _
                And pc_entita.PC_SuperUser Equals p.PC_Dettagli_SuperUser
            Where pc_entita.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_PianoConcimazione_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Dettagli
            Where Not GiasContext.PianoConcimazione_Dettagli.Any(Function(p) p.PC_Dettagli_Cod = r.From_PC_Dettagli_Cod And p.PC_Testata_Cod = r.From_PC_Testata_Cod And p.PC_Dettagli_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'EntitaxTestata
        rval.G2G_PianoConcimazione_EntitaxTestata_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata
            Where ((Not GiasContext.PianoConcimazione_EntitaxTestata.Any(Function(p) p.PC_Testata_Cod = r.From_PC_Testata_Cod And p.PC_SuperUser = r.From_PivaSuperUser And p.Piva = piva) And r.From_Piva = piva) _
                Or pianoconcimazione_testata_cod_update.Contains(r.From_PC_Testata_Cod)) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        rval.pianoconcimazione_entitaxtestata_insert = (
            From e In GiasContext.PianoConcimazione_EntitaxTestata
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Where e.Piva = piva _
                AndAlso (t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Any(Function(g) g.From_PivaSuperUser = e.PC_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_PC_Testata_Cod = e.PC_Testata_Cod And g.From_Piva = e.Piva)) _
                Or pianoconcimazione_testata_cod_update.Contains(e.PC_Testata_Cod)
            Select e).Distinct.ToList()


        'Fattori Correttivi
        rval.pianoconcimazione_fattoricorrettivi_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Dettagli_SuperUser Equals t.PC_SuperUser And e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Join f In GiasContext.PianoConcimazione_FattoriCorrettivi On t.PC_Testata_Cod Equals f.PC_Testata_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso (t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Testata.Any(Function(g) g.From_PivaSuperUser = e.PC_Dettagli_SuperUser And g.From_PC_Testata_Cod = e.PC_Testata_Cod))
            Select f).Distinct.ToList().Union(
            (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi
             Where pianoconcimazione_testata_cod_update.Contains(fc.PC_Testata_Cod)
             Select fc).Distinct.ToList
        ).ToList

        rval.pianoconcimazione_fattoricorrettivi_delete =
            (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi
             Join t In GiasContext.PianoConcimazione_Testata
                On fc.PC_Testata_Cod Equals t.PC_Testata_Cod _
                And fc.Piva_SuperUser Equals t.PC_SuperUser
             Where pianoconcimazione_testata_cod_update.Contains(t.PC_Testata_Cod)
             Select fc).Distinct.ToList

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal Piva_Destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_PianoConcimazione_Reverse


        Dim NomeRoutine As String = "G2GlocalDal.G2GPianoConcimazione_R.LeggiPerGias2GiasReverse()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_PianiConcimazione
        If objOpzioniImportImpresa.configurazione_piano_concimazione <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_PianiConcimazione)(objOpzioniImportImpresa.configurazione_piano_concimazione)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_PianiConcimazione
            oConfigurazione.listaPianoConcimazione_Tipo = New List(Of Integer)
        End If
        GiasContext.Database.CommandTimeout = 3600

        Dim rval As New G2G_PianoConcimazione_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_PianoConcimazione_Testata_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Testata)
            .G2G_PianoConcimazione_Testata_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Testata)
            .G2G_PianoConcimazione_Dettagli_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Dettagli)
            .G2G_PianoConcimazione_Dettagli_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Dettagli)
            .G2G_PianoConcimazione_Elaborazioni_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
            .G2G_PianoConcimazione_Elaborazioni_Recode_update = New List(Of G2G_Recode_PianoConcimazione_Elaborazioni)
            .G2G_PianoConcimazione_EntitaxTestata_Recode_insert = New List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)
            .pianoconcimazione_dettagli_delete = New List(Of PianoConcimazione_Dettagli)
            .pianoconcimazione_elaborazioni_delete = New List(Of PianoConcimazione_Elaborazioni)
            .pianoconcimazione_entitaxtestata_delete = New List(Of PianoConcimazione_EntitaxTestata)
            .pianoconcimazione_testata_delete = New List(Of PianoConcimazione_Testata)
            .To_Piva = Piva_Destinazione
        End With

        Dim pianoconcimazione_testata_cod_update As New List(Of Integer)

        'Elaborazioni
        rval.pianoconcimazione_elaborazioni_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Join b In GiasContext.PianoConcimazione_Elaborazioni On t.PC_Elaborazione_Cod Equals b.PC_Elaborazione_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Any(Function(g) g.To_PivaSuperUser = t.PC_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_PC_Elaborazione_Cod = b.PC_Elaborazione_Cod)
            Select b).Distinct.ToList()

        rval.pianoconcimazione_elaborazioni_update = (From p In GiasContext.PianoConcimazione_Elaborazioni
                                                      Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni
                                                          On p.PC_Elaborazione_Cod Equals g2g.To_PC_Elaborazione_Cod _
                                                          And p.PC_SuperUser Equals g2g.To_PivaSuperUser
                                                      Join pc_testata In GiasContext.PianoConcimazione_Testata
                                                          On pc_testata.PC_SuperUser Equals g2g.To_PivaSuperUser _
                                                          And pc_testata.PC_Elaborazione_Cod Equals g2g.To_PC_Elaborazione_Cod
                                                      Join pc_entita In GiasContext.PianoConcimazione_Dettagli
                                                          On pc_entita.PC_Testata_Cod Equals pc_testata.PC_Testata_Cod _
                                                          And pc_entita.PC_Dettagli_SuperUser Equals pc_testata.PC_SuperUser
                                                      Where pc_entita.PC_Dettagli_PIVA = piva _
                                                          AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                          AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
                                                      Select p).Distinct.ToList()

        rval.G2G_PianoConcimazione_Elaborazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni
            Where Not GiasContext.PianoConcimazione_Elaborazioni.Any(Function(p) p.PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod And p.PC_SuperUser = r.To_PivaSuperUser) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Testate
        rval.pianoconcimazione_testata_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Testata.Any(Function(g) g.To_PivaSuperUser = t.PC_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_PC_Testata_Cod = t.PC_Testata_Cod)
            Select t).Distinct.ToList()

        'rval.pianoconcimazione_testata_update = New List(Of PianoConcimazione_Testata)
        rval.pianoconcimazione_testata_update = (
        From p In GiasContext.PianoConcimazione_Testata
        Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Testata
                On p.PC_Testata_Cod Equals g2g.To_PC_Testata_Cod _
                And p.PC_SuperUser Equals g2g.To_PivaSuperUser
        Join pc_entita In GiasContext.PianoConcimazione_Dettagli
                On pc_entita.PC_Testata_Cod Equals p.PC_Testata_Cod _
                And pc_entita.PC_Dettagli_SuperUser Equals p.PC_SuperUser
        Where pc_entita.PC_Dettagli_PIVA = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
        Select p
            ).Distinct.ToList()

        For Each testata_update In rval.pianoconcimazione_testata_update
            pianoconcimazione_testata_cod_update.Add(testata_update.PC_Testata_Cod)
        Next

        'rval.G2G_PianoConcimazione_Testata_Recode_delete = New List(Of G2G_Recode_PianoConcimazione_Testata)
        rval.G2G_PianoConcimazione_Testata_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Testata
            Where Not GiasContext.PianoConcimazione_Testata.Any(Function(p) p.PC_Testata_Cod = r.To_PC_Testata_Cod And p.PC_SuperUser = r.To_PivaSuperUser) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Dettagli
        rval.pianoconcimazione_dettagli_insert = (
            From t In GiasContext.PianoConcimazione_Testata
            Join d In GiasContext.PianoConcimazione_Dettagli On t.PC_Testata_Cod Equals d.PC_Testata_Cod
            Where d.PC_Dettagli_PIVA = piva _
                AndAlso t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Any(Function(g) g.To_PivaSuperUser = d.PC_Dettagli_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_PC_Testata_Cod = d.PC_Testata_Cod And g.To_PC_Dettagli_Cod = d.PC_Dettagli_Cod)
            Select d).Distinct.ToList()

        rval.pianoconcimazione_dettagli_update = (
            From p In GiasContext.PianoConcimazione_Dettagli
            Join g2g In GiasContext.G2G_Recode_PianoConcimazione_Dettagli
                On p.PC_Testata_Cod Equals g2g.To_PC_Testata_Cod _
                And p.PC_Dettagli_Cod Equals g2g.To_PC_Dettagli_Cod _
                And p.PC_Dettagli_SuperUser Equals g2g.To_PivaSuperUser
            Join pc_entita In GiasContext.PianoConcimazione_EntitaxTestata
                On pc_entita.PC_Testata_Cod Equals p.PC_Testata_Cod _
                And pc_entita.PC_SuperUser Equals p.PC_Dettagli_SuperUser
            Where pc_entita.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_PianoConcimazione_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_Dettagli
            Where Not GiasContext.PianoConcimazione_Dettagli.Any(Function(p) p.PC_Dettagli_Cod = r.To_PC_Dettagli_Cod And p.PC_Testata_Cod = r.To_PC_Testata_Cod And p.PC_Dettagli_SuperUser = r.To_PivaSuperUser) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'EntitaxTestata
        rval.G2G_PianoConcimazione_EntitaxTestata_Recode_delete = (
            From r In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata
            Where ((Not GiasContext.PianoConcimazione_EntitaxTestata.Any(Function(p) p.PC_Testata_Cod = r.To_PC_Testata_Cod And p.PC_SuperUser = r.To_PivaSuperUser And p.Piva = piva) And r.To_Piva = piva) _
                Or pianoconcimazione_testata_cod_update.Contains(r.To_PC_Testata_Cod)) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        rval.pianoconcimazione_entitaxtestata_insert = (
            From e In GiasContext.PianoConcimazione_EntitaxTestata
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Where e.Piva = piva _
                AndAlso (t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Any(Function(g) g.To_PivaSuperUser = e.PC_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_PC_Testata_Cod = e.PC_Testata_Cod And g.To_Piva = e.Piva)) _
                Or pianoconcimazione_testata_cod_update.Contains(e.PC_Testata_Cod)
            Select e).Distinct.ToList()


        'Fattori Correttivi
        rval.pianoconcimazione_fattoricorrettivi_insert = (
            From e In GiasContext.PianoConcimazione_Dettagli
            Join t In GiasContext.PianoConcimazione_Testata On e.PC_Dettagli_SuperUser Equals t.PC_SuperUser And e.PC_Testata_Cod Equals t.PC_Testata_Cod
            Join f In GiasContext.PianoConcimazione_FattoriCorrettivi On t.PC_Testata_Cod Equals f.PC_Testata_Cod
            Where e.PC_Dettagli_PIVA = piva _
                AndAlso (t.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_piano_concimazione _
                AndAlso (oConfigurazione.listaPianoConcimazione_Tipo.Count = 0 Or oConfigurazione.listaPianoConcimazione_Tipo.Contains(t.PC_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_PianoConcimazione_Testata.Any(Function(g) g.To_PivaSuperUser = e.PC_Dettagli_SuperUser And g.To_PC_Testata_Cod = e.PC_Testata_Cod))
            Select f).Distinct.ToList().Union(
            (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi
             Where pianoconcimazione_testata_cod_update.Contains(fc.PC_Testata_Cod)
             Select fc).Distinct.ToList
        ).ToList

        rval.pianoconcimazione_fattoricorrettivi_delete =
            (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi
             Join t In GiasContext.PianoConcimazione_Testata
                On fc.PC_Testata_Cod Equals t.PC_Testata_Cod _
                And fc.Piva_SuperUser Equals t.PC_SuperUser
             Where pianoconcimazione_testata_cod_update.Contains(t.PC_Testata_Cod)
             Select fc).Distinct.ToList

        Return rval

    End Function

End Class

Public Class G2GPianoConcimazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_PianiConcimazione_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_PianoConcimazione, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GPianoConcimazione_W.Scrivi_PianiConcimazione_G2G()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        'Dim log As New LogProvider

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '
                    'DELETE
                    '

                    For Each s As PianoConcimazione_FattoriCorrettivi In g2g.pianoconcimazione_fattoricorrettivi_delete

                        Dim pianoconcimazione_fattoricorrettivi = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        pianoconcimazione_fattoricorrettivi.Piva_SuperUser = Destinazione_Piva_SuperUser
                        pianoconcimazione_fattoricorrettivi.PC_Testata_Cod = (From t In GiasContext.G2G_Recode_PianoConcimazione_Testata
                                                                              Where t.From_PivaSuperUser = s.Piva_SuperUser And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                      And t.From_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.To_PC_Testata_Cod

                        Dim fattore_correttivo = (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi Where fc.Piva_SuperUser = pianoconcimazione_fattoricorrettivi.Piva_SuperUser And
                                                                                                                 fc.PC_Testata_Cod = pianoconcimazione_fattoricorrettivi.PC_Testata_Cod And
                                                                                                                 fc.Regolamento_Cod = pianoconcimazione_fattoricorrettivi.Regolamento_Cod And
                                                                                                                 fc.Fattore_Cod = pianoconcimazione_fattoricorrettivi.Fattore_Cod).FirstOrDefault

                        If fattore_correttivo IsNot Nothing Then
                            GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                            GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_PianoConcimazione_Elaborazioni In g2g.G2G_PianoConcimazione_Elaborazioni_Recode_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Join t In GiasContext.PianoConcimazione_Testata On t.PC_Elaborazione_Cod Equals e.PC_Elaborazione_Cod
                                                              Join ext In GiasContext.PianoConcimazione_EntitaxTestata On ext.PC_Testata_Cod Equals t.PC_Testata_Cod
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod _
                                                                  AndAlso ext.Piva = pivaDest
                                                              Select e).FirstOrDefault()

                        If pianoconcimazione_elaborazioni Is Nothing Then
                            pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Join t In GiasContext.PianoConcimazione_Testata On t.PC_Elaborazione_Cod Equals e.PC_Elaborazione_Cod
                                                              Join ext In GiasContext.PianoConcimazione_Dettagli On ext.PC_Testata_Cod Equals t.PC_Testata_Cod
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod _
                                                                  AndAlso ext.PC_Dettagli_PIVA = pivaDest
                                                              Select e).FirstOrDefault()
                        End If

                        If pianoconcimazione_elaborazioni Is Nothing Then
                            pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod _
                                                                  AndAlso Not GiasContext.PianoConcimazione_Testata.Any(Function(t) t.PC_SuperUser = e.PC_SuperUser And t.PC_Elaborazione_Cod = e.PC_Elaborazione_Cod)
                                                              Select e).FirstOrDefault()
                        End If

                        If Not IsNothing(pianoconcimazione_elaborazioni) Then
                            GiasContext.PianoConcimazione_Elaborazioni.Attach(pianoconcimazione_elaborazioni)
                            GiasContext.PianoConcimazione_Elaborazioni.Remove(pianoconcimazione_elaborazioni)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Elaborazione_Cod = r.From_PC_Elaborazione_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For i As Integer = g2g.G2G_PianoConcimazione_Testata_Recode_delete.Count - 1 To 0 Step -1

                        Dim r As G2G_Recode_PianoConcimazione_Testata = g2g.G2G_PianoConcimazione_Testata_Recode_delete(i)

                        ''cancella entita
                        Dim pc_testata = (From m In GiasContext.PianoConcimazione_Testata Where m.PC_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.To_PC_Testata_Cod).FirstOrDefault()

                        If pc_testata IsNot Nothing Then

                            Dim contienePiva1 = (From m In GiasContext.PianoConcimazione_Dettagli Where m.PC_Testata_Cod = pc_testata.PC_Testata_Cod And m.PC_Dettagli_PIVA = piva).Count
                            Dim contienePiva2 = (From m In GiasContext.PianoConcimazione_EntitaxTestata Where m.PC_Testata_Cod = pc_testata.PC_Testata_Cod And m.Piva = piva).Count

                            If contienePiva1 > 0 Or contienePiva2 > 0 Then
                                Dim fattoricorrettivis = (From f In GiasContext.PianoConcimazione_FattoriCorrettivi Where f.PC_Testata_Cod = pc_testata.PC_Testata_Cod).ToList
                                For Each fattore_correttivo In fattoricorrettivis
                                    GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                                    GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                                Next

                                GiasContext.PianoConcimazione_Testata.Attach(pc_testata)
                                GiasContext.PianoConcimazione_Testata.Remove(pc_testata)

                                '' cancella recode entita
                                Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                            Else

                                Dim pianoconcimazione_testata = (From t In GiasContext.PianoConcimazione_Testata
                                                                 Where Not GiasContext.PianoConcimazione_Dettagli.Any(Function(d) t.PC_Testata_Cod = d.PC_Testata_Cod AndAlso t.PC_SuperUser = d.PC_Dettagli_SuperUser)
                                                                 Select t).FirstOrDefault

                                If pianoconcimazione_testata IsNot Nothing Then

                                    Dim fattoricorrettivis = (From f In GiasContext.PianoConcimazione_FattoriCorrettivi Where f.PC_Testata_Cod = pianoconcimazione_testata.PC_Testata_Cod).ToList
                                    For Each fattore_correttivo In fattoricorrettivis
                                        GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                                        GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                                    Next

                                    GiasContext.PianoConcimazione_Testata.Attach(pianoconcimazione_testata)
                                    GiasContext.PianoConcimazione_Testata.Remove(pianoconcimazione_testata)

                                    '' cancella recode entita
                                    Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()
                                    If recode IsNot Nothing Then
                                        GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                        GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                                    End If
                                    GiasContext.SaveChanges()

                                Else
                                    g2g.G2G_PianoConcimazione_Testata_Recode_delete.RemoveAt(i)
                                End If
                            End If

                        Else


                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                            Else
                                g2g.G2G_PianoConcimazione_Testata_Recode_delete.RemoveAt(i)
                            End If

                        End If

                    Next

                    GiasContext.SaveChanges()
                    'log.Scrivi_LOG(objParametri, "AgronicaCoreG2GLocalDal.G2GPianoConcimazione_W.Scrivi_PianiConcimazione_G2G", " Start g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_delete")
                    For Each r As G2G_Recode_PianoConcimazione_EntitaxTestata In g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_delete

                        ''cancella entita
                        Dim pc_entitas = (From m In GiasContext.PianoConcimazione_EntitaxTestata Where m.PC_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.To_PC_Testata_Cod).ToList()
                        For Each pc_entita In pc_entitas
                            GiasContext.PianoConcimazione_EntitaxTestata.Attach(pc_entita)
                            GiasContext.PianoConcimazione_EntitaxTestata.Remove(pc_entita)
                        Next


                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Attach(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Remove(recode)

                    Next

                    GiasContext.SaveChanges()


                    For i As Integer = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete.Count - 1 To 0 Step -1

                        Dim r As G2G_Recode_PianoConcimazione_Dettagli = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete(i)

                        ''cancella entita
                        Dim pc_dettagli = (From m In GiasContext.PianoConcimazione_Dettagli Where m.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.To_PC_Testata_Cod AndAlso m.PC_Dettagli_Cod = r.To_PC_Dettagli_Cod AndAlso m.PC_Dettagli_PIVA = piva).FirstOrDefault()
                        If pc_dettagli IsNot Nothing Then
                            GiasContext.PianoConcimazione_Dettagli.Attach(pc_dettagli)
                            GiasContext.PianoConcimazione_Dettagli.Remove(pc_dettagli)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod AndAlso rr.From_PC_Dettagli_Cod = r.From_PC_Dettagli_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Remove(recode)
                        Else
                            g2g.G2G_PianoConcimazione_Dettagli_Recode_delete.RemoveAt(i)
                        End If
                    Next

                    GiasContext.SaveChanges()


                    '
                    'INSERT
                    '
                    Dim jj = 0
                    If g2g.pianoconcimazione_elaborazioni_insert.Count > 0 Then
                        For Each s As PianoConcimazione_Elaborazioni In g2g.pianoconcimazione_elaborazioni_insert

                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("pianoconcimazione_elaborazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pianoconcimazione_elaborazioni = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            pianoconcimazione_elaborazioni.PC_Elaborazione_Cod = idSeq
                            pianoconcimazione_elaborazioni.PC_SuperUser = Destinazione_Piva_SuperUser

                            GiasContext.PianoConcimazione_Elaborazioni.Add(pianoconcimazione_elaborazioni)

                            Dim recode =
                                        New G2G_Recode_PianoConcimazione_Elaborazioni With {
                                            .From_PivaSuperUser = Origine_Piva_SuperUser,
                                            .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                            .From_PC_Elaborazione_Cod = s.PC_Elaborazione_Cod,
                                            .To_PC_Elaborazione_Cod = pianoconcimazione_elaborazioni.PC_Elaborazione_Cod,
                                            .Username_Creazione = username,
                                            .Username_Modifica = username,
                                            .Data_Creazione = Now(),
                                            .Data_Modifica = Now(),
                                            .Validita_Inizio = AGRODATAINIZIO,
                                            .Validita_Fine = AGRODATAFINE,
                                            .inviato = "0",
                                            .datainvio = Now()
                                        }

                            g2g.G2G_PianoConcimazione_Elaborazioni_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Add(recode)

                            jj += 1
                        Next

                        GiasContext.SaveChanges()

                    End If

                    jj = 0
                    If g2g.pianoconcimazione_testata_insert.Count > 0 Then
                        For Each s As PianoConcimazione_Testata In g2g.pianoconcimazione_testata_insert

                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PIANOCONCIMAZIONE_TESTATA", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            Dim pianoconcimazione_testata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            pianoconcimazione_testata.PC_Testata_Cod = idSeq
                            pianoconcimazione_testata.PC_SuperUser = Destinazione_Piva_SuperUser

                            If pianoconcimazione_testata.PC_Elaborazione_Cod IsNot Nothing AndAlso pianoconcimazione_testata.PC_Elaborazione_Cod <> 0 Then
                                Dim recodeElaborazione = (From c In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                  c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                  c.From_PC_Elaborazione_Cod = s.PC_Elaborazione_Cod).FirstOrDefault

                                If recodeElaborazione IsNot Nothing Then
                                    pianoconcimazione_testata.PC_Elaborazione_Cod = recodeElaborazione.To_PC_Elaborazione_Cod
                                End If

                            End If

                            GiasContext.PianoConcimazione_Testata.Add(pianoconcimazione_testata)

                            Dim recode =
                                New G2G_Recode_PianoConcimazione_Testata With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_PC_Testata_Cod = s.PC_Testata_Cod,
                                    .To_PC_Testata_Cod = pianoconcimazione_testata.PC_Testata_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.G2G_PianoConcimazione_Testata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Testata.Add(recode)
                            jj += 1
                        Next
                        GiasContext.SaveChanges()

                    End If

                    Dim hashPCTestata = New Hashtable()
                    Dim hashAnalisi = New Hashtable()

                    Dim listPianoConcimazione_Dettagli As New List(Of PianoConcimazione_Dettagli)
                    Dim listG2G_Recode_PianoConcimazione_Dettagli As New List(Of G2G_Recode_PianoConcimazione_Dettagli)
                    For Each s As PianoConcimazione_Dettagli In g2g.pianoconcimazione_dettagli_insert

                        Dim pianoconcimazione_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If pianoconcimazione_dettagli.PC_Dettagli_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PIANOCONCIMAZIONE_DETTAGLI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            pianoconcimazione_dettagli.PC_Dettagli_Cod = idSeq
                        End If

                        pianoconcimazione_dettagli.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser

                        If Not hashPCTestata.Contains(s.PC_Testata_Cod) Then
                            hashPCTestata.Add(s.PC_Testata_Cod, (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                         c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                         c.From_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.To_PC_Testata_Cod)
                        End If

                        pianoconcimazione_dettagli.PC_Testata_Cod = hashPCTestata(s.PC_Testata_Cod)

                        pianoconcimazione_dettagli.PC_Dettagli_PIVA = piva

                        If pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod IsNot Nothing AndAlso pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod <> 0 Then

                            If Not hashAnalisi.Contains(s.PC_Dettagli_Analisi_Testata_Cod) Then
                                hashAnalisi.Add(s.PC_Dettagli_Analisi_Testata_Cod, (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                c.From_Analisi_Testata_Cod = s.PC_Dettagli_Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod)
                            End If

                            pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod = hashAnalisi(s.PC_Dettagli_Analisi_Testata_Cod)

                        End If

                        listPianoConcimazione_Dettagli.Add(pianoconcimazione_dettagli)
                        'GiasContext.PianoConcimazione_Dettagli.Add(pianoconcimazione_dettagli)
                        'GiasContext.SaveChanges()

                        Dim recode =
                                    New G2G_Recode_PianoConcimazione_Dettagli With {
                                        .From_PivaSuperUser = Origine_Piva_SuperUser,
                                        .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .From_PC_Testata_Cod = s.PC_Testata_Cod,
                                        .To_PC_Testata_Cod = pianoconcimazione_dettagli.PC_Testata_Cod,
                                        .From_PC_Dettagli_Cod = s.PC_Dettagli_Cod,
                                        .To_PC_Dettagli_Cod = pianoconcimazione_dettagli.PC_Dettagli_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.G2G_PianoConcimazione_Dettagli_Recode_insert.Add(recode)
                        'GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Add(recode)
                        listG2G_Recode_PianoConcimazione_Dettagli.Add(recode)
                    Next
                    GiasContext.PianoConcimazione_Dettagli.AddRange(listPianoConcimazione_Dettagli)
                    GiasContext.G2G_Recode_PianoConcimazione_Dettagli.AddRange(listG2G_Recode_PianoConcimazione_Dettagli)

                    GiasContext.SaveChanges()

                    Dim listPianoConcimazione_EntitaxTestata As New List(Of PianoConcimazione_EntitaxTestata)
                    Dim listG2G_Recode_PianoConcimazione_EntitaxTestata As New List(Of G2G_Recode_PianoConcimazione_EntitaxTestata)

                    For Each s As PianoConcimazione_EntitaxTestata In g2g.pianoconcimazione_entitaxtestata_insert

                        'Richiedo un nuovo id sequenza
                        Dim pianoconcimazione_entitaxtestata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If Not hashPCTestata.Contains(s.PC_Testata_Cod) Then
                            hashPCTestata.Add(s.PC_Testata_Cod, (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                         c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                         c.From_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.To_PC_Testata_Cod)
                        End If

                        pianoconcimazione_entitaxtestata.PC_Testata_Cod = hashPCTestata(s.PC_Testata_Cod)


                        pianoconcimazione_entitaxtestata.PC_SuperUser = Destinazione_Piva_SuperUser
                        pianoconcimazione_entitaxtestata.Piva = piva

                        'GiasContext.PianoConcimazione_EntitaxTestata.Add(pianoconcimazione_entitaxtestata)
                        listPianoConcimazione_EntitaxTestata.Add(pianoconcimazione_entitaxtestata)

                        If (From rec In listG2G_Recode_PianoConcimazione_EntitaxTestata Where rec.From_PivaSuperUser = s.PC_SuperUser _
                                                                                                        And rec.From_PC_Testata_Cod = s.PC_Testata_Cod _
                                                                                                        And rec.From_Piva = s.Piva _
                                                                                                        And rec.To_PC_Testata_Cod = pianoconcimazione_entitaxtestata.PC_Testata_Cod _
                                                                                                        And rec.To_PivaSuperUser = pianoconcimazione_entitaxtestata.PC_SuperUser _
                                                                                                        And rec.To_Piva = pianoconcimazione_entitaxtestata.Piva).FirstOrDefault Is Nothing AndAlso
                            (From rec In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where rec.From_PivaSuperUser = s.PC_SuperUser _
                                                                                                        And rec.From_PC_Testata_Cod = s.PC_Testata_Cod _
                                                                                                        And rec.From_Piva = s.Piva _
                                                                                                        And rec.To_PC_Testata_Cod = pianoconcimazione_entitaxtestata.PC_Testata_Cod _
                                                                                                        And rec.To_PivaSuperUser = pianoconcimazione_entitaxtestata.PC_SuperUser _
                                                                                                        And rec.To_Piva = pianoconcimazione_entitaxtestata.Piva).FirstOrDefault Is Nothing Then

                            Dim recode =
                                   New G2G_Recode_PianoConcimazione_EntitaxTestata With {
                                       .From_PivaSuperUser = Origine_Piva_SuperUser,
                                       .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                       .From_PC_Testata_Cod = s.PC_Testata_Cod,
                                       .To_PC_Testata_Cod = pianoconcimazione_entitaxtestata.PC_Testata_Cod,
                                       .From_Piva = s.Piva,
                                       .To_Piva = piva,
                                       .Username_Creazione = username,
                                       .Username_Modifica = username,
                                       .Data_Creazione = Now(),
                                       .Data_Modifica = Now(),
                                       .Validita_Inizio = AGRODATAINIZIO,
                                       .Validita_Fine = AGRODATAFINE,
                                       .inviato = "0",
                                       .datainvio = Now()
                                   }
                            g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_insert.Add(recode)
                            'GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Add(recode)
                            listG2G_Recode_PianoConcimazione_EntitaxTestata.Add(recode)
                        End If


                    Next

                    If listPianoConcimazione_EntitaxTestata.Count > 0 Then
                        GiasContext.PianoConcimazione_EntitaxTestata.AddRange(listPianoConcimazione_EntitaxTestata)
                    End If
                    If listG2G_Recode_PianoConcimazione_EntitaxTestata.Count > 0 Then
                        GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.AddRange(listG2G_Recode_PianoConcimazione_EntitaxTestata)
                    End If

                    GiasContext.SaveChanges()

                    Dim listPianoConcimazione_FattoriCorrettivi As New List(Of PianoConcimazione_FattoriCorrettivi)
                    For Each s As PianoConcimazione_FattoriCorrettivi In g2g.pianoconcimazione_fattoricorrettivi_insert
                        'Richiedo un nuovo id sequenza
                        Dim pianoconcimazione_fattoricorrettivi = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If Not hashPCTestata.Contains(s.PC_Testata_Cod) Then
                            hashPCTestata.Add(s.PC_Testata_Cod, (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                     c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                     c.From_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.To_PC_Testata_Cod)
                        End If
                        pianoconcimazione_fattoricorrettivi.PC_Testata_Cod = hashPCTestata(s.PC_Testata_Cod)


                        pianoconcimazione_fattoricorrettivi.Piva_SuperUser = Destinazione_Piva_SuperUser

                        listPianoConcimazione_FattoriCorrettivi.Add(pianoconcimazione_fattoricorrettivi)


                    Next

                    GiasContext.PianoConcimazione_FattoriCorrettivi.AddRange(listPianoConcimazione_FattoriCorrettivi)
                    GiasContext.SaveChanges()

                    '
                    'UPDATE
                    '
                    For Each m As PianoConcimazione_Elaborazioni In g2g.pianoconcimazione_elaborazioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Elaborazione_Cod = m.PC_Elaborazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Elaborazioni_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim elaborazione = (From mm In GiasContext.PianoConcimazione_Elaborazioni Where mm.PC_Elaborazione_Cod = recode.To_PC_Elaborazione_Cod).FirstOrDefault()
                        elaborazione = Gias_EF_Utility.CopyEntity(GiasContext, m, elaborazione, username, data)
                        elaborazione.PC_SuperUser = Destinazione_Piva_SuperUser
                        elaborazione.PC_Elaborazione_Cod = recode.To_PC_Elaborazione_Cod
                        GiasContext.PianoConcimazione_Elaborazioni.Attach(elaborazione)
                        GiasContext.Entry(elaborazione).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()


                    For Each m As PianoConcimazione_Testata In g2g.pianoconcimazione_testata_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = m.PC_Testata_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Testata_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim testata = (From mm In GiasContext.PianoConcimazione_Testata Where mm.PC_Testata_Cod = recode.To_PC_Testata_Cod).FirstOrDefault()
                        testata = Gias_EF_Utility.CopyEntity(GiasContext, m, testata, username, data)
                        testata.PC_SuperUser = Destinazione_Piva_SuperUser
                        testata.PC_Testata_Cod = recode.To_PC_Testata_Cod
                        If testata.PC_Elaborazione_Cod <> 0 Then
                            testata.PC_Elaborazione_Cod = (From m1 In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where m1.To_PivaSuperUser = Destinazione_Piva_SuperUser And m1.From_PC_Elaborazione_Cod = testata.PC_Elaborazione_Cod).FirstOrDefault.To_PC_Elaborazione_Cod
                        End If
                        GiasContext.PianoConcimazione_Testata.Attach(testata)
                        GiasContext.Entry(testata).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As PianoConcimazione_Dettagli In g2g.pianoconcimazione_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = m.PC_Testata_Cod AndAlso rr.From_PC_Dettagli_Cod = m.PC_Dettagli_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Dettagli_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim dettaglio = (From mm In GiasContext.PianoConcimazione_Dettagli Where mm.PC_Testata_Cod = recode.To_PC_Testata_Cod AndAlso mm.PC_Dettagli_Cod = recode.To_PC_Dettagli_Cod).FirstOrDefault()
                        dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, dettaglio, username, data)
                        dettaglio.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser
                        dettaglio.PC_Testata_Cod = recode.To_PC_Testata_Cod
                        dettaglio.PC_Dettagli_Cod = recode.To_PC_Dettagli_Cod
                        dettaglio.PC_Dettagli_PIVA = piva

                        If dettaglio.PC_Dettagli_Analisi_Testata_Cod IsNot Nothing AndAlso dettaglio.PC_Dettagli_Analisi_Testata_Cod <> 0 Then

                            dettaglio.PC_Dettagli_Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                             c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                             c.From_Analisi_Testata_Cod = m.PC_Dettagli_Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod


                        End If

                        GiasContext.PianoConcimazione_Dettagli.Attach(dettaglio)
                        GiasContext.Entry(dettaglio).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    scope.Complete()

                    g2g.Recode.G2GRecodePianoConcimazione_TestataToInsert = g2g.G2G_PianoConcimazione_Testata_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_TestataToUpdate = g2g.G2G_PianoConcimazione_Testata_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_TestataToDelete = g2g.G2G_PianoConcimazione_Testata_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToInsert = g2g.G2G_PianoConcimazione_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToUpdate = g2g.G2G_PianoConcimazione_Dettagli_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToDelete = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToInsert = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToUpdate = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToDelete = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_EntitaxTestataToInsert = g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_insert
                    'g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToUpdate = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_EntitaxTestataToDelete = g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_delete

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            messaggioErrore &= If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message)
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_PianiConcimazione_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_PianoConcimazione_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GPianoConcimazione_W.Scrivi_PianiConcimazione_G2GReverse()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    '
                    'DELETE
                    '
                    'cancellazioni, da gestire ... 

                    For Each s As PianoConcimazione_FattoriCorrettivi In g2g.pianoconcimazione_fattoricorrettivi_delete

                        Dim pianoconcimazione_fattoricorrettivi = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        pianoconcimazione_fattoricorrettivi.Piva_SuperUser = Destinazione_Piva_SuperUser
                        pianoconcimazione_fattoricorrettivi.PC_Testata_Cod = (From t In GiasContext.G2G_Recode_PianoConcimazione_Testata
                                                                              Where t.To_PivaSuperUser = s.Piva_SuperUser And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                      And t.To_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.From_PC_Testata_Cod

                        Dim fattore_correttivo = (From fc In GiasContext.PianoConcimazione_FattoriCorrettivi Where fc.Piva_SuperUser = pianoconcimazione_fattoricorrettivi.Piva_SuperUser And
                                                                                                                 fc.PC_Testata_Cod = pianoconcimazione_fattoricorrettivi.PC_Testata_Cod And
                                                                                                                 fc.Regolamento_Cod = pianoconcimazione_fattoricorrettivi.Regolamento_Cod And
                                                                                                                 fc.Fattore_Cod = pianoconcimazione_fattoricorrettivi.Fattore_Cod).FirstOrDefault

                        If fattore_correttivo IsNot Nothing Then
                            GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                            GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_PianoConcimazione_Elaborazioni In g2g.G2G_PianoConcimazione_Elaborazioni_Recode_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Join t In GiasContext.PianoConcimazione_Testata On t.PC_Elaborazione_Cod Equals e.PC_Elaborazione_Cod
                                                              Join ext In GiasContext.PianoConcimazione_EntitaxTestata On ext.PC_Testata_Cod Equals t.PC_Testata_Cod
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.From_PC_Elaborazione_Cod _
                                                                  AndAlso ext.Piva = pivaDest
                                                              Select e).FirstOrDefault()

                        If pianoconcimazione_elaborazioni Is Nothing Then
                            pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Join t In GiasContext.PianoConcimazione_Testata On t.PC_Elaborazione_Cod Equals e.PC_Elaborazione_Cod
                                                              Join ext In GiasContext.PianoConcimazione_Dettagli On ext.PC_Testata_Cod Equals t.PC_Testata_Cod
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.From_PC_Elaborazione_Cod _
                                                                  AndAlso ext.PC_Dettagli_PIVA = pivaDest
                                                              Select e).FirstOrDefault()
                        End If

                        If pianoconcimazione_elaborazioni Is Nothing Then
                            pianoconcimazione_elaborazioni = (From e In GiasContext.PianoConcimazione_Elaborazioni
                                                              Where e.PC_SuperUser = Destinazione_Piva_SuperUser _
                                                                  AndAlso e.PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod _
                                                                  AndAlso Not GiasContext.PianoConcimazione_Testata.Any(Function(t) t.PC_SuperUser = e.PC_SuperUser And t.PC_Elaborazione_Cod = e.PC_Elaborazione_Cod)
                                                              Select e).FirstOrDefault()
                        End If

                        If Not IsNothing(pianoconcimazione_elaborazioni) Then
                            GiasContext.PianoConcimazione_Elaborazioni.Attach(pianoconcimazione_elaborazioni)
                            GiasContext.PianoConcimazione_Elaborazioni.Remove(pianoconcimazione_elaborazioni)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Elaborazione_Cod = r.To_PC_Elaborazione_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For i As Integer = g2g.G2G_PianoConcimazione_Testata_Recode_delete.Count - 1 To 0 Step -1

                        Dim r As G2G_Recode_PianoConcimazione_Testata = g2g.G2G_PianoConcimazione_Testata_Recode_delete(i)

                        ''cancella entita
                        Dim pc_testata = (From m In GiasContext.PianoConcimazione_Testata Where m.PC_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()

                        If pc_testata IsNot Nothing Then

                            Dim contienePiva1 = (From m In GiasContext.PianoConcimazione_Dettagli Where m.PC_Testata_Cod = pc_testata.PC_Testata_Cod And m.PC_Dettagli_PIVA = piva).Count
                            Dim contienePiva2 = (From m In GiasContext.PianoConcimazione_EntitaxTestata Where m.PC_Testata_Cod = pc_testata.PC_Testata_Cod And m.Piva = piva).Count

                            If contienePiva1 > 0 Or contienePiva2 > 0 Then
                                Dim fattoricorrettivis = (From f In GiasContext.PianoConcimazione_FattoriCorrettivi Where f.PC_Testata_Cod = pc_testata.PC_Testata_Cod).ToList
                                For Each fattore_correttivo In fattoricorrettivis
                                    GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                                    GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                                Next

                                GiasContext.PianoConcimazione_Testata.Attach(pc_testata)
                                GiasContext.PianoConcimazione_Testata.Remove(pc_testata)

                                '' cancella recode entita
                                Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = r.To_PC_Testata_Cod).FirstOrDefault()
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                            Else

                                Dim pianoconcimazione_testata = (From t In GiasContext.PianoConcimazione_Testata
                                                                 Where Not GiasContext.PianoConcimazione_Dettagli.Any(Function(d) t.PC_Testata_Cod = d.PC_Testata_Cod AndAlso t.PC_SuperUser = d.PC_Dettagli_SuperUser)
                                                                 Select t).FirstOrDefault

                                If pianoconcimazione_testata IsNot Nothing Then

                                    Dim fattoricorrettivis = (From f In GiasContext.PianoConcimazione_FattoriCorrettivi Where f.PC_Testata_Cod = pianoconcimazione_testata.PC_Testata_Cod).ToList
                                    For Each fattore_correttivo In fattoricorrettivis
                                        GiasContext.PianoConcimazione_FattoriCorrettivi.Attach(fattore_correttivo)
                                        GiasContext.PianoConcimazione_FattoriCorrettivi.Remove(fattore_correttivo)
                                    Next

                                    GiasContext.PianoConcimazione_Testata.Attach(pianoconcimazione_testata)
                                    GiasContext.PianoConcimazione_Testata.Remove(pianoconcimazione_testata)

                                    '' cancella recode entita
                                    Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = r.To_PC_Testata_Cod).FirstOrDefault()
                                    GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                    GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                                    GiasContext.SaveChanges()

                                Else
                                    g2g.G2G_PianoConcimazione_Testata_Recode_delete.RemoveAt(i)
                                End If
                            End If

                        Else


                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = r.To_PC_Testata_Cod).FirstOrDefault()
                            If recode IsNot Nothing Then
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                                GiasContext.G2G_Recode_PianoConcimazione_Testata.Remove(recode)
                            Else
                                g2g.G2G_PianoConcimazione_Testata_Recode_delete.RemoveAt(i)
                            End If

                        End If

                    Next

                    GiasContext.SaveChanges()
                    Dim jj = 0
                    For Each r As G2G_Recode_PianoConcimazione_EntitaxTestata In g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_delete

                        ''cancella entita
                        Dim pc_entitas = (From m In GiasContext.PianoConcimazione_EntitaxTestata Where m.PC_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.From_PC_Testata_Cod).ToList()
                        For Each pc_entita In pc_entitas
                            GiasContext.PianoConcimazione_EntitaxTestata.Attach(pc_entita)
                            GiasContext.PianoConcimazione_EntitaxTestata.Remove(pc_entita)
                            jj += 1
                        Next


                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_PC_Testata_Cod = r.From_PC_Testata_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Attach(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Remove(recode)

                    Next

                    GiasContext.SaveChanges()


                    For i As Integer = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete.Count - 1 To 0 Step -1

                        Dim r As G2G_Recode_PianoConcimazione_Dettagli = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete(i)

                        ''cancella entita
                        Dim pc_dettagli = (From m In GiasContext.PianoConcimazione_Dettagli Where m.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser AndAlso m.PC_Testata_Cod = r.To_PC_Testata_Cod AndAlso m.PC_Dettagli_Cod = r.To_PC_Dettagli_Cod AndAlso m.PC_Dettagli_PIVA = piva).FirstOrDefault()
                        If pc_dettagli IsNot Nothing Then
                            GiasContext.PianoConcimazione_Dettagli.Attach(pc_dettagli)
                            GiasContext.PianoConcimazione_Dettagli.Remove(pc_dettagli)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = r.To_PC_Testata_Cod AndAlso rr.To_PC_Dettagli_Cod = r.To_PC_Dettagli_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Remove(recode)
                        Else
                            g2g.G2G_PianoConcimazione_Dettagli_Recode_delete.RemoveAt(i)
                        End If
                    Next

                    GiasContext.SaveChanges()


                    '
                    'INSERT
                    '

                    'Dim idSeq As Integer = 0
                    If g2g.pianoconcimazione_elaborazioni_insert.Count > 0 Then
                        For Each s As PianoConcimazione_Elaborazioni In g2g.pianoconcimazione_elaborazioni_insert

                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("pianoconcimazione_elaborazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            Dim pianoconcimazione_elaborazioni = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            pianoconcimazione_elaborazioni.PC_Elaborazione_Cod = idSeq
                            pianoconcimazione_elaborazioni.PC_SuperUser = Destinazione_Piva_SuperUser

                            GiasContext.PianoConcimazione_Elaborazioni.Add(pianoconcimazione_elaborazioni)

                            Dim recode =
                                        New G2G_Recode_PianoConcimazione_Elaborazioni With {
                                            .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                            .To_PivaSuperUser = Origine_Piva_SuperUser,
                                            .From_PC_Elaborazione_Cod = pianoconcimazione_elaborazioni.PC_Elaborazione_Cod,
                                            .To_PC_Elaborazione_Cod = s.PC_Elaborazione_Cod,
                                            .Username_Creazione = username,
                                            .Username_Modifica = username,
                                            .Data_Creazione = Now(),
                                            .Data_Modifica = Now(),
                                            .Validita_Inizio = AGRODATAINIZIO,
                                            .Validita_Fine = AGRODATAFINE,
                                            .inviato = "0",
                                            .datainvio = Now()
                                        }
                            g2g.G2G_PianoConcimazione_Elaborazioni_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    End If


                    If g2g.pianoconcimazione_testata_insert.Count > 0 Then
                        For Each s As PianoConcimazione_Testata In g2g.pianoconcimazione_testata_insert
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PIANOCONCIMAZIONE_TESTATA", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            Dim pianoconcimazione_testata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            pianoconcimazione_testata.PC_Testata_Cod = idSeq
                            pianoconcimazione_testata.PC_SuperUser = Destinazione_Piva_SuperUser

                            If pianoconcimazione_testata.PC_Elaborazione_Cod IsNot Nothing AndAlso pianoconcimazione_testata.PC_Elaborazione_Cod <> 0 Then
                                Dim recodeElaborazione = (From c In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                  c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                  c.To_PC_Elaborazione_Cod = s.PC_Elaborazione_Cod).FirstOrDefault

                                If recodeElaborazione IsNot Nothing Then
                                    pianoconcimazione_testata.PC_Elaborazione_Cod = recodeElaborazione.From_PC_Elaborazione_Cod
                                End If

                            End If

                            GiasContext.PianoConcimazione_Testata.Add(pianoconcimazione_testata)

                            Dim recode =
                                New G2G_Recode_PianoConcimazione_Testata With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_PC_Testata_Cod = pianoconcimazione_testata.PC_Testata_Cod,
                                    .To_PC_Testata_Cod = s.PC_Testata_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.G2G_PianoConcimazione_Testata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_Testata.Add(recode)

                        Next
                        GiasContext.SaveChanges()

                    End If


                    For Each s As PianoConcimazione_Dettagli In g2g.pianoconcimazione_dettagli_insert

                        Dim pianoconcimazione_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If pianoconcimazione_dettagli.PC_Dettagli_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("PIANOCONCIMAZIONE_DETTAGLI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            pianoconcimazione_dettagli.PC_Dettagli_Cod = idSeq
                        End If

                        pianoconcimazione_dettagli.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser

                        pianoconcimazione_dettagli.PC_Testata_Cod = (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                         c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                         c.To_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.From_PC_Testata_Cod

                        pianoconcimazione_dettagli.PC_Dettagli_PIVA = piva

                        If pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod IsNot Nothing AndAlso pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod <> 0 Then

                            pianoconcimazione_dettagli.PC_Dettagli_Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                         c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                         c.To_Analisi_Testata_Cod = s.PC_Dettagli_Analisi_Testata_Cod).FirstOrDefault.From_Analisi_Testata_Cod


                        End If

                        GiasContext.PianoConcimazione_Dettagli.Add(pianoconcimazione_dettagli)
                        GiasContext.SaveChanges()

                        Dim recode =
                                    New G2G_Recode_PianoConcimazione_Dettagli With {
                                        .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .To_PivaSuperUser = Origine_Piva_SuperUser,
                                        .From_PC_Testata_Cod = pianoconcimazione_dettagli.PC_Testata_Cod,
                                        .To_PC_Testata_Cod = s.PC_Testata_Cod,
                                        .From_PC_Dettagli_Cod = pianoconcimazione_dettagli.PC_Dettagli_Cod,
                                        .To_PC_Dettagli_Cod = s.PC_Dettagli_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.G2G_PianoConcimazione_Dettagli_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Add(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As PianoConcimazione_EntitaxTestata In g2g.pianoconcimazione_entitaxtestata_insert

                        'Richiedo un nuovo id sequenza
                        Dim pianoconcimazione_entitaxtestata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        pianoconcimazione_entitaxtestata.PC_Testata_Cod = (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                         c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                         c.To_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.From_PC_Testata_Cod


                        pianoconcimazione_entitaxtestata.PC_SuperUser = Destinazione_Piva_SuperUser
                        pianoconcimazione_entitaxtestata.Piva = piva

                        GiasContext.PianoConcimazione_EntitaxTestata.Add(pianoconcimazione_entitaxtestata)
                        GiasContext.SaveChanges()

                        If (From rec In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where rec.To_PivaSuperUser = s.PC_SuperUser _
                                                                                                        And rec.To_PC_Testata_Cod = s.PC_Testata_Cod _
                                                                                                        And rec.To_Piva = s.Piva _
                                                                                                        And rec.From_PC_Testata_Cod = pianoconcimazione_entitaxtestata.PC_Testata_Cod _
                                                                                                        And rec.From_PivaSuperUser = pianoconcimazione_entitaxtestata.PC_SuperUser _
                                                                                                        And rec.From_Piva = pianoconcimazione_entitaxtestata.Piva).FirstOrDefault Is Nothing Then

                            Dim recode =
                                   New G2G_Recode_PianoConcimazione_EntitaxTestata With {
                                       .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                       .To_PivaSuperUser = Origine_Piva_SuperUser,
                                       .From_PC_Testata_Cod = pianoconcimazione_entitaxtestata.PC_Testata_Cod,
                                       .To_PC_Testata_Cod = s.PC_Testata_Cod,
                                       .From_Piva = s.Piva,
                                       .To_Piva = piva,
                                       .Username_Creazione = username,
                                       .Username_Modifica = username,
                                       .Data_Creazione = Now(),
                                       .Data_Modifica = Now(),
                                       .Validita_Inizio = AGRODATAINIZIO,
                                       .Validita_Fine = AGRODATAFINE,
                                       .inviato = "0",
                                       .datainvio = Now()
                                   }
                            g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata.Add(recode)

                        End If


                    Next
                    GiasContext.SaveChanges()

                    For Each s As PianoConcimazione_FattoriCorrettivi In g2g.pianoconcimazione_fattoricorrettivi_insert
                        'Richiedo un nuovo id sequenza
                        Dim pianoconcimazione_fattoricorrettivi = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        pianoconcimazione_fattoricorrettivi.PC_Testata_Cod = (From c In GiasContext.G2G_Recode_PianoConcimazione_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                     c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                     c.To_PC_Testata_Cod = s.PC_Testata_Cod).FirstOrDefault.From_PC_Testata_Cod


                        pianoconcimazione_fattoricorrettivi.Piva_SuperUser = Destinazione_Piva_SuperUser

                        GiasContext.PianoConcimazione_FattoriCorrettivi.Add(pianoconcimazione_fattoricorrettivi)



                    Next
                    GiasContext.SaveChanges()

                    '
                    'UPDATE
                    '


                    For Each m As PianoConcimazione_Elaborazioni In g2g.pianoconcimazione_elaborazioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Elaborazione_Cod = m.PC_Elaborazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Elaborazioni_Recode_update.Add(recode)
                        'GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim elaborazione = (From mm In GiasContext.PianoConcimazione_Elaborazioni Where mm.PC_Elaborazione_Cod = recode.From_PC_Elaborazione_Cod).FirstOrDefault()
                        elaborazione = Gias_EF_Utility.CopyEntity(GiasContext, m, elaborazione, username, data)
                        elaborazione.PC_SuperUser = Destinazione_Piva_SuperUser
                        elaborazione.PC_Elaborazione_Cod = recode.From_PC_Elaborazione_Cod
                        'GiasContext.PianoConcimazione_Elaborazioni.Attach(elaborazione)
                        GiasContext.Entry(elaborazione).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    For Each m As PianoConcimazione_Testata In g2g.pianoconcimazione_testata_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = m.PC_Testata_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Testata_Recode_update.Add(recode)
                        'GiasContext.G2G_Recode_PianoConcimazione_Testata.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim testata = (From mm In GiasContext.PianoConcimazione_Testata Where mm.PC_Testata_Cod = recode.From_PC_Testata_Cod).FirstOrDefault()
                        testata = Gias_EF_Utility.CopyEntity(GiasContext, m, testata, username, data)
                        testata.PC_SuperUser = Destinazione_Piva_SuperUser
                        testata.PC_Testata_Cod = recode.From_PC_Testata_Cod
                        If testata.PC_Elaborazione_Cod <> 0 Then
                            testata.PC_Elaborazione_Cod = (From m1 In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where m1.From_PivaSuperUser = Destinazione_Piva_SuperUser And m1.To_PC_Elaborazione_Cod = testata.PC_Elaborazione_Cod).FirstOrDefault.From_PC_Elaborazione_Cod
                        End If
                        'GiasContext.PianoConcimazione_Testata.Attach(testata)
                        GiasContext.Entry(testata).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As PianoConcimazione_Dettagli In g2g.pianoconcimazione_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_PC_Testata_Cod = m.PC_Testata_Cod AndAlso rr.To_PC_Dettagli_Cod = m.PC_Dettagli_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_PianoConcimazione_Dettagli_Recode_update.Add(recode)
                        'GiasContext.G2G_Recode_PianoConcimazione_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim dettaglio = (From mm In GiasContext.PianoConcimazione_Dettagli Where mm.PC_Testata_Cod = recode.From_PC_Testata_Cod AndAlso mm.PC_Dettagli_Cod = recode.From_PC_Dettagli_Cod).FirstOrDefault()
                        dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, dettaglio, username, data)
                        dettaglio.PC_Dettagli_SuperUser = Destinazione_Piva_SuperUser
                        dettaglio.PC_Testata_Cod = recode.From_PC_Testata_Cod
                        dettaglio.PC_Dettagli_Cod = recode.From_PC_Dettagli_Cod
                        dettaglio.PC_Dettagli_PIVA = piva

                        If dettaglio.PC_Dettagli_Analisi_Testata_Cod IsNot Nothing AndAlso dettaglio.PC_Dettagli_Analisi_Testata_Cod <> 0 Then

                            dettaglio.PC_Dettagli_Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                             c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                             c.To_Analisi_Testata_Cod = m.PC_Dettagli_Analisi_Testata_Cod).FirstOrDefault.From_Analisi_Testata_Cod


                        End If

                        'GiasContext.PianoConcimazione_Dettagli.Attach(dettaglio)
                        GiasContext.Entry(dettaglio).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()

                    scope.Complete()

                    g2g.Recode.G2GRecodePianoConcimazione_TestataToInsert = g2g.G2G_PianoConcimazione_Testata_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_TestataToUpdate = g2g.G2G_PianoConcimazione_Testata_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_TestataToDelete = g2g.G2G_PianoConcimazione_Testata_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToInsert = g2g.G2G_PianoConcimazione_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToUpdate = g2g.G2G_PianoConcimazione_Dettagli_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_DettagliToDelete = g2g.G2G_PianoConcimazione_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToInsert = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToUpdate = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_update
                    g2g.Recode.G2GRecodePianoConcimazione_ElaborazioniToDelete = g2g.G2G_PianoConcimazione_Elaborazioni_Recode_delete

                    g2g.Recode.G2GRecodePianoConcimazione_EntitaxTestataToInsert = g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_insert
                    'g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToUpdate = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    g2g.Recode.G2GRecodePianoConcimazione_EntitaxTestataToDelete = g2g.G2G_PianoConcimazione_EntitaxTestata_Recode_delete

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class
