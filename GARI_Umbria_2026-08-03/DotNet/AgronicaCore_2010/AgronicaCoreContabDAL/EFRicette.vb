Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFRicette

    Public Shared Function ReadRicetta(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByVal Piva_SuperUser As String,
                                   ByVal Ricetta_Cod As Integer) As Ricette

        Dim ricetta = (From r In GiasContext.Ricette
                       Where r.Ricetta_Cod = Ricetta_Cod And r.Ricetta_SuperUser = Piva_SuperUser
                                 ).FirstOrDefault()

        Return ricetta

    End Function
    Public Shared Function ReadRicettaOperazione(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByVal Piva_SuperUser As String,
                                   ByVal Ricetta_Operazione_Cod As Integer) As Ricette_Operazioni

        Dim ricetta_operazione = (From r In GiasContext.Ricette_Operazioni
                                  Where r.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod And r.Ricetta_SuperUser = Piva_SuperUser
                                 ).FirstOrDefault()

        Return ricetta_operazione

    End Function

    Public Shared Function ReadRicetteOperazioneByRaccoglitore(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                   ByVal Piva_SuperUser As String,
                                   ByVal Raccoglitore_Cod As Integer) As List(Of Ricette_Operazioni)

        Dim ricette_operazione = (From r In GiasContext.Ricette_Operazioni
                                  Where r.Raccoglitore_Cod = Raccoglitore_Cod And r.Ricetta_SuperUser = Piva_SuperUser
                                 ).ToList()

        Return ricette_operazione

    End Function

    Public Shared Function ReadRicettaDestinazioni(ByRef GiasContext As Gias_DeveloperServer_Entities,
                               ByVal Piva_SuperUser As String,
                               ByVal Ricetta_Cod As Integer,
                               ByVal Ricetta_Operazione_Cod As Integer,
                               ByVal Ricetta_Dettaglio_Cod As Integer) As List(Of Ricette_Destinazioni)

        Dim ricetta_destinazioni = (From r In GiasContext.Ricette_Destinazioni
                                    Where r.Ricetta_SuperUser = Piva_SuperUser And
                                          r.Ricetta_Cod = Ricetta_Cod And
                                          r.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod And
                                        (Ricetta_Dettaglio_Cod = 0 Or r.Ricetta_Dettaglio_Cod = Ricetta_Dettaglio_Cod)
                                       ).ToList()

        Return ricetta_destinazioni

    End Function

    Public Shared Function ReadRicettaDettagli(ByRef GiasContext As Gias_DeveloperServer_Entities,
                               ByVal Piva_SuperUser As String,
                               ByVal Ricetta_Cod As Integer,
                               ByVal Ricetta_Operazione_Cod As Integer) As List(Of Ricette_Dettagli)

        Dim ricetta_dettagli = (From r In GiasContext.Ricette_Dettagli
                                Where r.Ricetta_SuperUser = Piva_SuperUser And
                                          r.Ricetta_Cod = Ricetta_Cod And
                                          r.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod
                                       ).ToList()

        Return ricetta_dettagli

    End Function

    Public Shared Function ReadRicettaDettaglioTecnico(ByRef GiasContext As Gias_DeveloperServer_Entities,
                               ByVal Piva_SuperUser As String,
                               ByVal Ricetta_Cod As Integer,
                               ByVal Ricetta_Operazione_Cod As Integer,
                               ByVal Ricetta_Dettaglio_Cod As Integer) As List(Of Ricette_Dettaglio_Tecnico)

        Dim ricetta_dettagli_tecnici = (From r In GiasContext.Ricette_Dettaglio_Tecnico
                                        Where r.Ricetta_SuperUser = Piva_SuperUser And
                                          r.Ricetta_Cod = Ricetta_Cod And
                                          r.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod And
                                          r.Ricetta_Dettaglio_Cod = Ricetta_Dettaglio_Cod
                                       ).ToList()

        Return ricetta_dettagli_tecnici

    End Function

End Class
