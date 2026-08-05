Imports AgronicaCoreDataProvider

Public Class ProfilatoreUtenze

    Public Function ProfilaUtenteDaServizio(ByVal idServizio As Int32) As Int32

        Dim rval As Int32

        Select Case idServizio
            Case TipiEnumerativi.enum_Servizi.RBase
                rval = TipiEnumerativi.enum_TipologieUtenti.RBase
            Case TipiEnumerativi.enum_Servizi.RPlus
                rval = TipiEnumerativi.enum_TipologieUtenti.RPlus
            Case Else
                Throw New Exception("Codice servizio non riconosciuto")
        End Select

        Return rval

    End Function

    Public Sub impostaVisibilitaUtente_descr1descr2(ByVal listaPive As List(Of String), ByRef d1 As String, ByRef d2 As String)

        If listaPive.Count = 1 Then

            d1 = "<DatiFiltri><Filtro><Impresa piva='" & listaPive.First & "'><Struttura><Appezzamento><Impianto><Agenda><Contatto /></Agenda></Impianto></Appezzamento></Struttura></Impresa><DatiGerarchiaImprese /></Filtro></DatiFiltri>"
            d2 = "Imprese.piva='" & listaPive.First & "'"
        Else

            Dim xDocRval As XDocument = XDocument.Parse("<DatiFiltri><Filtro><DatiGerarchiaImprese/></Filtro></DatiFiltri>")
            Dim lRval As New List(Of String)

            For Each curPiva In listaPive

                Dim xN As XElement = <Impresa piva=<%= curPiva %>><Struttura><Appezzamento><Impianto><Agenda><Contatto/></Agenda></Impianto></Appezzamento></Struttura></Impresa>
                xDocRval.Element("DatiFiltri").Element("Filtro").Add(xN)

                lRval.Add("Imprese.piva = '" & curPiva & "'")

            Next

            d1 = xDocRval.ToString()
            d2 = " ( " & String.Join(" OR ", lRval.ToArray) & " )"

        End If


        'Descrizione_1	Descrizione_2
        '<DatiFiltri><Filtro><DatiGerarchiaImprese/><DatiPive><Piva piva="01511110221" /<> piva piva="01551420225" /><Piva piva="00894290220"/><Piva piva="0089429O220"/></DatiPive><Impresa><Struttura><Appezzamento><Impianto><Agenda><Contatto/></Agenda></Impianto></Appezzamento></Struttura></Impresa></Filtro></DatiFiltri> |||||	And (  Imprese.piva = '01511110221' OR  Imprese.piva = '01551420225' OR  Imprese.piva = '00894290220' OR  Imprese.piva = '0089429O220' )
    End Sub

End Class
