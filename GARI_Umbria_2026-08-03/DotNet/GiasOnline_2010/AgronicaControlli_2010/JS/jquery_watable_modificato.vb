Imports AgronicaCoreVarieDAL
Imports System.Data

Public Class jquery_watable_modificato
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"

    'Public Property VersionWaTable As String

    Protected Overrides Sub OnPreRender(e As EventArgs)
        'Page.ClientScript.RegisterClientScriptInclude("jquery.watable_modificato" & _VersionWaTable, _
        '            Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jquery.watable_modificato" & _VersionWaTable & ".js"))

        Page.ClientScript.RegisterClientScriptInclude("jquery.watable", _
                    Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jquery.watable.js"))
    End Sub

#End Region

#Region "Gestione del filtro su Datatable rispetto a filtri WaTable"

    Public Shared Function FiltraDTconFiltriWatable(ByRef dt As DataTable, ByVal listaFiltriStr As String) As DataTable

        Dim dtFiltrato As DataTable = dt.Copy()
        Dim nomeFiltro, valFiltro As String
        listaFiltriStr = System.Web.HttpUtility.HtmlDecode(listaFiltriStr) 'depuro i filtri dai caratteri #34; che mi lascerebbero dei ;
        Dim listaFiltri As String() = listaFiltriStr.Split(";")

        For Each filtro As String In listaFiltri
            ' faccio il filtro solo se c'è scritto qualcosa
            valFiltro = filtro.Split(":")(1)

            If valFiltro <> "" Then
                nomeFiltro = filtro.Split(":")(0)

                'controllo se il filtro o positivo o una negazione "!"
                Dim filtroPositivo As Boolean = True
                If valFiltro.StartsWith("!") Then
                    valFiltro = valFiltro.Substring(1) 'tolgo il segno
                    filtroPositivo = False 'impsoto la stringa di filtro
                End If

                Select Case dt.Columns(nomeFiltro).DataType.Name

                    Case "String"
                        'Trova Mario Rossi:				    Mario Rossi
                        'Trova Mario e Luca Rossi(Regex):	?Mario Rossi|Luca Rossi
                        'Trova tutti tranne Mario Rossi:	!Mario Rossi

                        If valFiltro.StartsWith("?") Then 'è una regular expression
                            valFiltro = valFiltro.Substring(1) 'tolgo il '?'
                            Dim espr As New System.Text.RegularExpressions.Regex(valFiltro, System.Text.RegularExpressions.RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))

                            dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso espr.IsMatch(x(nomeFiltro))).CopyToDataTable()
                        Else
                            'metto la condizione tra parentesi e la prendo per buona se è l'opposto di 'non' in questo modo gestisco tutto in una sola riga
                            dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso (x(nomeFiltro).ToString().ToLower().Contains(valFiltro.ToString().ToLower())) = filtroPositivo).CopyToDataTable()
                        End If

                    Case "Int16", "Int32", "Int64", "Single", "Double"
                        'Valori da 10 a 20:				            10..20
                        'Valori fino a 30:				            ..30
                        'Tutti i valori tranne quelli tra 10 e 20:	!10..20
                        'Tutti i valori tranne quelli > 30:	        !30..
                        'Esattamente 50:					        =50

                        If valFiltro.StartsWith("=") Then 'se devo cercare un numero singolo...

                            'ripulisco del segno iniziale e converto il punto in verigola per gestire gli eventuali numeri decimali
                            valFiltro = valFiltro.Substring(1).Replace(".", ",")

                            'se è un numero e non una stringa a caso...
                            If IsNumeric(valFiltro) Then
                                'metto la condizione tra parentesi e la prendo per buona se è l'opposto di 'non' in questo modo gestisco tutto in una sola riga
                                dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                    Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso (x(nomeFiltro) = valFiltro) = filtroPositivo).CopyToDataTable()
                            End If

                        ElseIf valFiltro.Contains("..") Then 'se è un intervallo..

                            Dim estremi As String() = valFiltro.Split({".."}, StringSplitOptions.None) ' separo gli estremi del range
                            If estremi.Count = 2 Then ' controllo che ci sia un .. nella stringa

                                'se ho un solo estremo al posto di quello che manca metto il MinValue o il MaxValue
                                'il replace è per sistemare eventuali numeri decimali
                                Dim min As Object = If(IsNumeric(estremi(0).Replace(".", ",")), estremi(0).Replace(".", ","), Integer.MinValue)
                                Dim max As Object = If(IsNumeric(estremi(1).Replace(".", ",")), estremi(1).Replace(".", ","), Integer.MaxValue)

                                'metto la condizione tra parentesi e la prendo per buona se è l'opposto di 'non' in questo modo gestisco tutto in una sola riga
                                dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                        Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso (x(nomeFiltro) >= min AndAlso x(nomeFiltro) <= max) = filtroPositivo).CopyToDataTable()
                            End If

                        End If

                    Case "Boolean"
                        'Passa tra:indeterminato,presente,assente
                        Select Case valFiltro
                            Case "true"
                                dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                        Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso x(nomeFiltro) = 1).CopyToDataTable()
                            Case "false"
                                dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                        Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso x(nomeFiltro) = 0).CopyToDataTable()
                        End Select

                    Case "Date", "DateTime"
                        'Oggi:						        0..1
                        'Tutti fino a oggi:			        ..1
                        'Tutti eccetto oggi:				!0..1
                        'Tutti eccetto gli ultimi 7 giorni:	!-7..
                        'Ultima settimana oggi escluso:		-7..0

                        If valFiltro.Contains("..") Then 'se è un intervallo...

                            Dim estremi As String() = valFiltro.Split({".."}, StringSplitOptions.None) ' separo gli estremi del range
                            If estremi.Count = 2 Then ' controllo che ci sia un .. nella stringa

                                'se sono numero intero e non una stringa a caso...
                                Dim dataInizio As Date = If(IsNumeric(estremi(0)) AndAlso Not estremi(0).Contains(".") AndAlso Not estremi(0).Contains(","), Date.Today.AddDays(estremi(0)), Date.MinValue)
                                Dim dataFine As Date = If(IsNumeric(estremi(1)) AndAlso Not estremi(1).Contains(".") AndAlso Not estremi(1).Contains(","), Date.Today.AddDays(estremi(1)), Date.MaxValue)

                                'metto la condizione tra parentesi e la prendo per buona se è l'opposto di 'non' in questo modo gestisco tutto in una sola riga
                                dtFiltrato = dtFiltrato.AsEnumerable().Where(
                                        Function(x) Not IsDBNull(x(nomeFiltro)) AndAlso (x(nomeFiltro) >= dataInizio AndAlso x(nomeFiltro) <= dataFine) = filtroPositivo).CopyToDataTable()
                            End If
                        End If

                End Select

            End If
        Next

        Return dtFiltrato
    End Function

    Public Shared Function FiltraDTconSoloColonneOrdinateVisibili(ByRef dt As DataTable, ByVal listaColonneVisibili As String) As DataTable

        Dim dtFiltrato As DataTable = dt.Copy()
        listaColonneVisibili = System.Web.HttpUtility.HtmlDecode(listaColonneVisibili) 'depuro i filtri dai caratteri #34; che mi lascerebbero dei ;

        'Estraggio i nomi delle colonne del dt (con l'orinamento) ed il nome visualizzato
        Dim listaNomiInDt As New List(Of String)
        Dim listaNomiVisualiz As New List(Of String)
        For Each col As String In listaColonneVisibili.Split(";")
            listaNomiInDt.Add(col.Split(":")(0))
            listaNomiVisualiz.Add(col.Split(":")(1))
        Next

        'Creo una lista con le colonne da eliminare
        Dim listaColonneDaCanc As New List(Of String)
        For Each dc As DataColumn In dtFiltrato.Columns
            If Not listaNomiInDt.Contains(dc.ColumnName) Then
                listaColonneDaCanc.Add(dc.ColumnName)
            End If
        Next

        'Elimino le colonne
        For Each col As String In listaColonneDaCanc
            dtFiltrato.Columns.Remove(col)
        Next

        'ordino le colonne ed imposto il nome da visualizzare
        For i As Integer = 0 To listaNomiInDt.Count - 1
            dtFiltrato.Columns(listaNomiInDt(i)).SetOrdinal(i)
            dtFiltrato.Columns(listaNomiInDt(i)).Caption = listaNomiVisualiz(i)
        Next

        Return dtFiltrato
    End Function

#End Region

End Class
