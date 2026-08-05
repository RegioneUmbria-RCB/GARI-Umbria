
Imports AgronicaCoreAnagrafeStdBIZ
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Ricette_Operazioni_R

    Public Function LeggiPerEsclusioneBozze(dbContext As GiasDbContext, piva As String) As List(Of Integer)

        Dim listaBozze As List(Of APP_Ricette_Operazioni) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_Ricette_Operazioni
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where op.Ricetta_Operazione_Cod <= 0 AndAlso r.piva = piva AndAlso op.Bozza = 1
            Select op
        ).ToList()

        Dim rval As List(Of Integer) = (From r In listaBozze Select r.Ricetta_Cod).ToList

        Return rval
    End Function

    Public Function LeggiPerCancellazione(dbContext As GiasDbContext, piva As String) As List(Of APP_Ricette_Operazioni)

        Dim rval As List(Of APP_Ricette_Operazioni) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_Ricette_Operazioni
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where (piva = "" OrElse r.piva = piva) AndAlso op.Bozza = 0 AndAlso r.ricetta_cod < 0
            Select op
        ).ToList()

        Return rval
    End Function

    Public Function LeggiPerVerificaPresenzaDatiLocali(dbContext As GiasDbContext) As List(Of APP_Ricette_Operazioni)

        Dim rval As List(Of APP_Ricette_Operazioni) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_Ricette_Operazioni
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where op.Ricetta_Operazione_Cod <= 0
            Select op
        ).ToList()

        Return rval
    End Function

    Public Function LeggiPerRicaricoDati(dbContext As GiasDbContext, piva As String) As List(Of APP_Ricette_Operazioni)

        Dim rval As List(Of APP_Ricette_Operazioni) = (
            From r In dbContext.APP_Ricette
            Join op In dbContext.APP_Ricette_Operazioni
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Where op.Ricetta_Operazione_Cod <= 0 AndAlso r.piva = piva
            Select op
        ).ToList()

        Return rval
    End Function

    Public Function LeggiPerLista(dbContext As GiasDbContext, tipoRicetta As enum_TipoRicetta_APP, stato As enum_WWorflow_WAnagraficaStati, piva As String) As List(Of APP_Ricette_Operazioni)

        Dim rval As List(Of APP_Ricette_Operazioni)
        Dim listaNonFiltrata As List(Of APP_Ricette_Operazioni) = (
            From op In dbContext.APP_Ricette_Operazioni
            Join r In dbContext.APP_Ricette
                    On op.Ricetta_Cod Equals r.ricetta_cod
            Join lav In dbContext.APP_Operazioni
                    On lav.lav_cod Equals op.Lav_Cod
            Where (piva = "" OrElse r.piva = piva) AndAlso
                (
                    (tipoRicetta = enum_TipoRicetta_APP.RilieviNatiSuAPP AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.InterventiNatiSuAPP AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.InterventiScaricatiDaServer AndAlso op.Ricetta_Cod > 0 AndAlso op.Ricetta_Operazione_Cod > 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.InterventiScaricatiDaServerEdApplicati AndAlso op.Ricetta_Cod > 0 AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.NatiSuAPP AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.InterventiScaricatiDaServerEdApplicati_o_NatiSuAPP AndAlso op.Ricetta_Operazione_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita) OrElse
                    (tipoRicetta = enum_TipoRicetta_APP.Ricette AndAlso op.Ricetta_Cod < 0 AndAlso op.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire)
                )
            Order By op.Validita_Inizio Descending
            Select op
        ).ToList()

        Select Case tipoRicetta
            Case enum_TipoRicetta_APP.NatiSuAPP
                rval = listaNonFiltrata
            Case enum_TipoRicetta_APP.RilieviNatiSuAPP
                rval = listaNonFiltrata.Where(Function(Q) Operazioni.OperazioniRilieviDisponibiliDaTipoRicetta(enum_TipoRicetta_DB.Standard_Destinazioni).Contains(Q.Lav_Cod)).ToList()
            Case Else
                rval = listaNonFiltrata.Where(Function(Q) Operazioni.OperazioniDisponibiliDaTipoRicetta(enum_TipoRicetta_DB.Standard_Destinazioni).Contains(Q.Lav_Cod)).ToList()
        End Select

        Return rval

    End Function

    Public Function Leggi(dbContext As GiasDbContext, ricetta_cod As Integer, ricetta_Operazione_cod As Integer) As List(Of APP_Ricette_Operazioni)

        Dim rval As List(Of APP_Ricette_Operazioni) = (
            From r In dbContext.APP_Ricette_Operazioni
            Where (ricetta_cod = 0 OrElse r.Ricetta_Cod = ricetta_cod) AndAlso
                  (ricetta_Operazione_cod = 0 OrElse r.Ricetta_Operazione_Cod = ricetta_Operazione_cod)
            Select r
        ).ToList()

        Return rval
    End Function


End Class

Public Class Ricette_Operazioni_W


    Public Sub ImpostaStatoSuOperazione(dbContext As GiasDbContext, ByVal Ricetta_Operazione_Cod As Integer, ByVal W_Anagrafica_Stati_Cod As Integer)
        'dbContext.Database.ExecuteSqlCommand("UPDATE [APP_Ricette_Operazioni] SET W_Anagrafica_Stati_Cod = {1} WHERE Ricetta_Operazione_Cod = {0}", Ricetta_Operazione_Cod, W_Anagrafica_Stati_Cod)
        Dim letturaOperazione As New Ricette_Operazioni_R
        Dim ricette_operazioni = letturaOperazione.Leggi(dbContext, 0, Ricetta_Operazione_Cod)
        For Each ricetta_operazione In ricette_operazioni
            ricetta_operazione.W_Anagrafica_Stati_Cod = W_Anagrafica_Stati_Cod
            dbContext.SaveChanges()
        Next
    End Sub

    Public Sub ImpostaBozzaSuOperazione(dbContext As GiasDbContext, ByVal Ricetta_Operazione_Cod As Integer, ByVal Bozza As Integer)
        Dim letturaOperazione As New Ricette_Operazioni_R
        Dim ricette_operazioni = letturaOperazione.Leggi(dbContext, 0, Ricetta_Operazione_Cod)
        For Each ricetta_operazione In ricette_operazioni
            ricetta_operazione.Bozza = Bozza
            dbContext.APP_Ricette_Operazioni.Update(ricetta_operazione)
            dbContext.SaveChanges()
        Next
    End Sub

    Public Sub CancellaDataImpresa(dbContext As GiasDbContext, ByVal piva As String)

        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette_Operazioni] WHERE Ricetta_COD in (select Ricetta_Cod from APP_Ricette WHERE Piva={0})", piva)

    End Sub

    Public Sub CancellaDaRicettaOperazioneCod(dbContext As GiasDbContext, ricettaOperazioneCod As Integer)

        dbContext.Database.ExecuteSqlCommand("DELETE FROM [APP_Ricette_Operazioni] WHERE Ricetta_Operazione_COD = {0}", ricettaOperazioneCod)

    End Sub
End Class
