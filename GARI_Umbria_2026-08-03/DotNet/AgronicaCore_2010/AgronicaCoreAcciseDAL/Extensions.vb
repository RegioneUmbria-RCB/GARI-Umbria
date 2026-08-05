Imports System.Runtime.CompilerServices
Imports AgronicaCoreAcciseCommon
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Module Extensions

    <Extension()>
    Public Function ToList(ByVal table As DataTable, ByVal tipoStampa As enum_TipoStampa_DAA, dataDa As Date, dataA As Date) As List(Of IListableDataTable)

        Select Case tipoStampa
            Case enum_TipoStampa_DAA.GaranziaCircolazione
                Return table.ToGaCirList()
            Case enum_TipoStampa_DAA.PartitaSospensione
                Return table.ToParSosList(dataDa, dataA)
            Case Else
                Return Enumerable.Empty(Of IListableDataTable).ToList()
        End Select

    End Function

    <Extension()>
    Public Function ToGaCirList(ByVal table As DataTable) As List(Of IListableDataTable)

        Dim garanzie As New List(Of GaranziaCircolazione)

        For Each row As DataRow In table.AsEnumerable()

            Dim g As New GaranziaCircolazione()
            g.Importo_Impegnato = row.Item("Importo_Impegnato")
            g.Importo_Svincolato = row.Item("Importo_Svincolato")
            g.Tipo = row.Item("Tipo")
            g.RiportoFinePeriodo = row.Item("RiportoFinePeriodo")
            g.RiportoPeriodoPrecedente = row.Item("RiportoPeriodoPrecedente")
            g.ImportoIntegrato = row.Item("ImportoIntegrato")
            g.ImportoScaduto = row.Item("ImportoScaduto")
            g.data_emis_documento_AAAAMMGG = row.Item("data_emis_documento_AAAAMMGG")
            g.CodFisGar = String.Empty
            garanzie.Add(g)
        Next

        If garanzie.Count = 1 AndAlso garanzie.FirstOrDefault().Tipo = "E" Then
            garanzie.Clear()
        End If

        Return garanzie.Select(Function(g) DirectCast(g, IListableDataTable)).ToList()

    End Function

    <Extension()>
    Public Function ToParSosList(ByVal table As DataTable, dataDa As Date, dataA As Date) As List(Of IListableDataTable)

        Dim mapper = PopolaPSMapper()

        Dim partiteSospensione As New List(Of PartitaSospensione)

        If (table.Rows.Count > 0) Then

            Dim data As String = Convert.ToDateTime(table.AsEnumerable().First().Item("data_origine")).ToString("ddMMyyyy")
            Dim progressivoRiga As Integer = 0

            For Each row As DataRow In table.AsEnumerable()

                Dim txt_Messaggio_C As String = row.Item("Messaggio").ToString()
                Dim codiceMessaggio = If(String.IsNullOrEmpty(txt_Messaggio_C), "", txt_Messaggio_C.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Skip(1).First().Substring(0, 5).ToUpper())
                Dim tipoMessaggio = CInt(row.Item("Tipo_Messaggio"))
                Dim dataOrigine = Convert.ToDateTime(row.Item("data_origine"))
                Dim dataTestata = Convert.ToDateTime(row.Item("data_emis_documento_AAAAMMGG"))
                Dim dataRientro3c = If(row.IsNull("DataRientroTerzaCopia"), dataTestata, Convert.ToDateTime(row.Item("DataRientroTerzaCopia")))

                Dim dataRecord = Convert.ToDateTime(row.Item("data_origine")).ToString("ddMMyyyy")
                If dataRecord <> data Then
                    data = dataRecord
                    progressivoRiga = 0
                End If
                progressivoRiga += 1

                Dim ps As New PartitaSospensione With
                {
                    .ProgressivoRiga = progressivoRiga,
                    .TipoMessaggio = tipoMessaggio,
                    .CodiceProdotto = String.Format("{0}{1}{2}{3}", row.Item("CPA"), row.Item("NC"), row.Item("TARIC"), row.Item("CADD")),
                    .DocumentoAccompagnamentoDocumento = New DocumentoAccompagnamentoMovimento With {
                        .TipoMovimento = OttieniCodiceTipoDocumento(mapper, codiceMessaggio, row.Item("tipo").ToString()),
                        .DataEmissione = CDate(dataTestata),
                        .NumeroIdentificativo = row.Item("arc"),
                        .DataRientro3C = CDate(dataRientro3c),
                        .DataGruppoStampa = dataOrigine
                    },
                    .Movimentazione = New Movimentazione With
                    {
                        .Provenzienza = row.Item("Cod_identificativo_destinatario").ToString().Substring(0, 2),
                        .CS = mapper(codiceMessaggio).CS,
                        .MittenteDestinatario = row.Item("Cod_identificativo_destinatario")
                    },
                    .Stoccaggio = New Stoccaggio With
                    {
                        .TipoStoccaggio = "N",
                        .NumeroConfezioni = CDec(row.Item("num_colli")),
                        .VNC = CDec(row.Item("volume_nominale_confezioni"))
                    },
                    .Quantita = New Quantita With
                    {
                        .GradoAlconalico = CDec(row.Item("Grado_Alcool")),
                        .LitriIdrati = CDec(row.Item("qta"))
                    },
                    .Contabilita = New Contabilita With
                    {
                        .CauMov = mapper(codiceMessaggio).CausaleMov,
                        .PosizioneFiscale = If(tipoMessaggio = 1, "002", "004"),
                        .AccisaAssolta = 0,
                        .AccisaSospesa = 0
                    }
                }
                partiteSospensione.Add(ps)

            Next

            Return partiteSospensione.Select(Function(g) DirectCast(g, IListableDataTable)).ToList()

        Else

            Return Enumerable.Empty(Of IListableDataTable).ToList()

        End If

    End Function





    Private Function OttieniCodiceTipoDocumento(ByVal mapper As Dictionary(Of String, PSMapper), ByVal codiceMessaggio As String, ByVal tipoMessaggioIE815 As String) As String

        If codiceMessaggio = "IE815" Then
            Return tipoMessaggioIE815
        Else
            Return mapper(codiceMessaggio).TipoMovimento
        End If

    End Function

    Private Function PopolaPSMapper() As Dictionary(Of String, PSMapper)

        Dim result = New Dictionary(Of String, PSMapper)

        ' messaggio vuoto
        result.Add("", New PSMapper With {.CS = "", .CausaleMov = "", .TipoMessaggio = "", .TipoMovimento = ""})

        ' Conferma rientro deposito
        result.Add("IE818", New PSMapper With {.CS = "C", .CausaleMov = "107", .TipoMessaggio = "IE818", .TipoMovimento = "ROR"})

        ' cambio destinazione
        result.Add("IE813", New PSMapper With {.CS = "C", .CausaleMov = "107", .TipoMessaggio = "IE813", .TipoMovimento = "COD"})

        ' daa
        result.Add("IE815", New PSMapper With {.CS = "S", .CausaleMov = "016", .TipoMessaggio = "IE815", .TipoMovimento = "EAD"})

        ' annullamento
        result.Add("IE810", New PSMapper With {.CS = "C", .CausaleMov = "107", .TipoMessaggio = "IE810", .TipoMovimento = "ANN"})

        Return result


    End Function


    Public Enum enum_TipoStampa_DAA
        GaranziaCircolazione = 0
        PartitaSospensione = 1
    End Enum

End Module


