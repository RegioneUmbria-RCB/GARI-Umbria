Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Agro_Sequenze

    Public SequenceTableExceptions As New List(Of String) From {
        "jDeereDataModel_EntitaGIAS",
        "materie_prime_campionature",
        "ws_vivaiPassaporti_operazioni",
        "regolamentiRMA",
        "PrincipiAttiviRMA",
        "Gis_entita",
        "Gis_elementiGrafici",
        "GIS_LayerTilesDescrizione",
        "particellecatastali",
        "lineaproduzione",
        "linea_classe_produzione",
        "preparazione",
        "pagamenti_causali",
        "imprese_sezionali",
        "liquidita",
        "ist_credito",
        "causaletrasporto",
        "TabellaRMA",
        "DerrateCodifica"
    }

    'Private Function gestioneSequencexTabella(ByVal Nome_Tabella As String,
    '                                          ByRef ObjParametri As AgronicaCoreParametri) As Boolean
    '    'Default gestione con sequence
    '    Dim gestita As Boolean = True

    '    'check appSettings se è specificato false
    '    If Not CheckAllowAppSettingsFlagUseSequence() Then
    '        gestita = False
    '        Return gestita
    '    End If

    '    'sql Versione 2008 non gestisce le sequence
    '    Dim sqlVersion As Integer = VersioneSqlServer_anno(ObjParametri)
    '    If sqlVersion <= 2008 Then
    '        gestita = False
    '        Return gestita
    '    End If

    '    'Tabelle speciali che non devono essere MAI gestite con sequence
    '    If SequenceTableExceptions.Select(Function(x) x.ToLower).ToList.Contains(Nome_Tabella.ToLower) Then
    '        gestita = False
    '        Return gestita
    '    End If

    '    'Caso speciale di sequence utilizzata per progressivo annuale, pdc_campioni_2024, pdc_campioni_2025 etc etc
    '    If Nome_Tabella.ToLower().Contains("pdc_campioni_") Then
    '        gestita = False
    '        Return gestita
    '    End If
    '    Return gestita
    'End Function



    Public Function NuovoId_Tabella_EF(
        dbContext As GiasDbContext,
        ByVal NomeTabella As String,
        ByVal Base As Int32,
        ByVal Fine As Int32
    ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreDataProvider.Agro_Sequenze.NuovoId_Tabella_EF()"

        Dim MessaggioErrore As String = ""

        Dim NuovoValore As Int32

        Try

            'dbContext.Database.


            Dim seq = (From sequenza_Tabella In dbContext.APP_Sequenza_Tabelle
                       Where sequenza_Tabella.Nome_Tabella = NomeTabella).FirstOrDefault()

            If seq IsNot Nothing Then

                NuovoValore = seq.Ultimo_Valore + 1

                'Se il nuovo valore supera l'ultimo valore valido ho un errore ...
                If NuovoValore > seq.End Then

                    'Imposto un valore dummy
                    NuovoValore = -1

                    'Genero un errore
                    Throw New Exception("L'indice ha raggiunto il limite superiore")
                Else

                    seq.Ultimo_Valore = seq.Ultimo_Valore + 1

                End If

                dbContext.Update(Of APP_Sequenza_Tabelle)(seq)

            Else 'Non esiste il record in Sequenza_Tabelle

                Select Case NomeTabella.ToLower
                    Case "contatti"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case "impresa"
                        Base = -2000000000
                        Fine = 0
                        NuovoValore = -2000000000
                    Case Else
                        NuovoValore = Base + 1
                End Select

                Dim newSeq As New APP_Sequenza_Tabelle

                newSeq.Nome_Tabella = NomeTabella
                newSeq.Ultimo_Valore = NuovoValore
                newSeq.Data_Creazione = Date.Now
                newSeq.Data_Modifica = Date.Now
                newSeq.Username_Creazione = ""
                newSeq.Username_Modifica = ""
                newSeq.Base = Base
                newSeq.End = Fine

                dbContext.Add(newSeq)

            End If

        Catch ex As Exception

            MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return NuovoValore


    End Function



End Class
