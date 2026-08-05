Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ToolStandard
    Public cssColonna As String
    Public btn As List(Of btnAzioni)

    Public Sub New()
        cssColonna = ""
        btn = New List(Of btnAzioni)
    End Sub

    Public Sub New(ByVal _btn As List(Of btnAzioni))
        cssColonna = ""
        btn = _btn
    End Sub

    Public Overrides Function toString() As String
        Dim str As String = "<div class='text-center " & cssColonna & "'> "
        For Each b In btn
            str &= "<span class='fa " & b.cssDef & " " & b.btnClass & " fa-2x' chiave='{0}' onclick='" & b.btnOnClick & "' title='" & b.tooltip & "'></span> "
        Next
        str &= "</div> "
        Return str
    End Function

End Class

Public Class btnAzioni

    Public btnClass As String
    Public btnOnClick As String
    Public cssDef As String
    Public tooltip As String

    Public Sub New(ByVal _class As String, ByVal _onclick As String)
        btnClass = _class
        btnOnClick = _onclick
    End Sub

    Public Sub New(ByVal _class As String, ByVal _onclick As String, _tooltip As String)
        btnClass = _class
        btnOnClick = _onclick
        tooltip = _tooltip
    End Sub
End Class

Public Class funzioneDiValidazione
    Public nome As String
    Public messaggioErrore As String
    Public testJavascriptBooleano As String

    Public Sub New(ByVal _nome As String, ByVal _testJavascriptBooleano As String, _messaggioErrore As String)
        nome = _nome
        testJavascriptBooleano = _testJavascriptBooleano
        messaggioErrore = _messaggioErrore
    End Sub

End Class

Public Class ColonneNome
    Public _Nome_colonna_DT As String
    Public _Nome_colonna_Json As String
    Public _Tipo As String
    Public _Filtrabile As Boolean
    Public _OperatoreFiltro_Kendo As String = "contains"
    Public _FormatoParticolare As String
    ''' <summary>
    ''' Indicare l'Id del template che verrà inserito in kendo.template($('#idTemplate').html())
    ''' </summary>
    Public _TemplateHtmlID As String
    Public _Editor As String
    Public _RemoveHtmlEncode As Boolean = False
    Public _FiltrabileConCheck As Boolean
    Public _placeHolder As String
    Public _hidden As Boolean
    Public _Display As Boolean
    Public _nonPassare As Boolean
    Public _css As String
    Public _cssHeader As String
    Public _Editabile As Boolean = False
    Public _width As String
    Public _locked As Boolean
    Public _valueDefault As String
    Public _daDuplicare As Boolean = False
    Public _obbligatorio As Boolean = False
    Public _stringaTotale As String = "Totale: "
    Public _generaModelloDati As Boolean = True
    Public _gruppoColonne As String
    Public _listaFunzioniDiValidazione As New List(Of funzioneDiValidazione)

    Public _ColonnaDiSelezione As Boolean = False

    Public _min As Boolean
    Public _max As Boolean
    Public _average As Boolean
    Public _sum As Boolean
    Public _count As Boolean
    Public _formatNr As String
    Public _formatDataOra As String


    Public Sub New(nome_colonna_DT As String, ByVal nome_da_dare_alla_colonna As String, ByVal Tipo As String)
        _Nome_colonna_DT = nome_colonna_DT
        _Nome_colonna_Json = nome_da_dare_alla_colonna
        _Tipo = Tipo
        _css = ""
        _cssHeader = ""
        _Filtrabile = True
        _FiltrabileConCheck = False
        _hidden = False
        _nonPassare = False
        _Display = True
        _min = False
        _max = False
        _average = False
        _sum = False
        _count = False
        _formatNr = ""
        _formatDataOra = ""
        _width = ""
        _locked = False
        _valueDefault = ""
        _daDuplicare = False
        _obbligatorio = False
        _stringaTotale = "Totale: "
        _gruppoColonne = ""
        _listaFunzioniDiValidazione = New List(Of funzioneDiValidazione)
    End Sub

    Public Sub New(ByRef dc As DataColumn, ByVal nome_da_dare_alla_colonna As String)
        _Nome_colonna_DT = dc.ColumnName
        _Nome_colonna_Json = nome_da_dare_alla_colonna
        _Tipo = getJSONtypeFromVBtype(dc.DataType)
        _css = ""
        _cssHeader = ""
        _Filtrabile = True
        _hidden = False
        _Display = True
        _nonPassare = False
        _min = False
        _max = False
        _average = False
        _sum = False
        _count = False
        _formatNr = ""
        _formatDataOra = ""
        _width = ""
        _locked = False
        _valueDefault = ""
        _daDuplicare = False
        _obbligatorio = False
        _stringaTotale = "Totale: "
        _gruppoColonne = ""
        _listaFunzioniDiValidazione = New List(Of funzioneDiValidazione)
    End Sub

    Public Sub New(ByRef dc As DataColumn, ByVal nome_da_dare_alla_colonna As String, ByVal css As String, ByVal useCss As Boolean)
        _Nome_colonna_DT = dc.ColumnName
        _Nome_colonna_Json = nome_da_dare_alla_colonna
        _Tipo = getJSONtypeFromVBtype(dc.DataType)
        _nonPassare = False
        If useCss Then
            _css = css
        End If
        _cssHeader = ""
        _Filtrabile = True
        _hidden = False
        _min = False
        _max = False
        _average = False
        _sum = False
        _count = False
        _formatNr = ""
        _formatDataOra = ""
        _width = ""
        _locked = False
        _valueDefault = ""
        _daDuplicare = False
        _obbligatorio = False
        _stringaTotale = "Totale: "
        _gruppoColonne = ""
        _listaFunzioniDiValidazione = New List(Of funzioneDiValidazione)
    End Sub


    Private Function getJSONtypeFromVBtype(tipoVB As Type) As String
        Dim tipoJSON As String

        Select Case tipoVB
            Case Type.GetType("System.String")
                tipoJSON = "string"
            Case Type.GetType("System.Integer"), Type.GetType("System.Int32"),
                 Type.GetType("System.Int64"), Type.GetType("System.Int16"),
                 Type.GetType("System.Long"), Type.GetType("System.Short"),
                 Type.GetType("System.Single"), Type.GetType("System.Decimal"),
                 Type.GetType("System.Double")
                tipoJSON = "number"
                'Case System.Type.GetType("System.Single"), System.Type.GetType("System.Decimal")
                '    newL.Add(New ColonneNome(col.ColumnName, col.ColumnName, "string"))
            Case Type.GetType("System.Boolean")
                tipoJSON = "bool"
            Case Type.GetType("System.Date"), Type.GetType("System.DateTime")
                tipoJSON = "date"
            Case Else
                tipoJSON = "string"
        End Select

        Return tipoJSON
    End Function
End Class


Public Class JSON_DataTable

    'TODO
    Private jsonReplacements As String = ""

    Public Editabile_Deafault As Boolean = False

    Public Const COLUMNS_TESTED As Integer = 87
    Public Const ROWS_TESTED As Integer = 50000
    Public Const COLUMNS_X_ROWS_MAX_ALLOWANCE As Integer = COLUMNS_TESTED * ROWS_TESTED

    Public Shared Sub kendo_columns(ByVal l As List(Of ColonneNome), ByVal ImpostaOperatoreFiltroDaTipoDati As Boolean, ByVal Editabile_Default As Boolean, ByRef KendoCols As String, ByRef KendoModel As String, ByVal TipoFiltroKendo As TipiEnumerativi.TipoFiltroKendo_colonne, Optional ImpostaFiltroColonneStdGIAS As Boolean = False)

        Dim JsonString_Cols As New StringBuilder()
        Dim JsonString_Model As New StringBuilder()

        JsonString_Model.Append("{")
        JsonString_Cols.Append("[")


        'Tutti i primi che non sono visibili determinano un aumento dell'indice da dove si parte a mettere il primo carattere ","
        Dim indiceDiPartenzaColonne As Integer = 0
        For i As Integer = 0 To l.Count - 1
            If l(i)._hidden Then
                indiceDiPartenzaColonne += 1
            Else
                Exit For
            End If
        Next


        For i As Integer = 0 To l.Count - 1
            If Not l(i)._nonPassare Then

                'separatore, colonne
                If i > indiceDiPartenzaColonne Then
                    If Not l(i)._hidden Then
                        JsonString_Cols.Append(",")
                    End If
                End If

                'separatore, model
                If i > 0 AndAlso l(i)._generaModelloDati Then
                    JsonString_Model.Append(",")
                End If



                'If l(i)._ColonnaDiSelezione Then

                '    JsonString_Cols.Append("{  ")
                '    JsonString_Cols.Append("                ""field"": """ & l(i)._Nome_colonna_DT & """, ")
                '    JsonString_Cols.Append("                ""title"": """ & l(i)._Nome_colonna_Json & """, ") 
                '    JsonString_Cols.Append("                ""headerTemplate"": ""<input type='checkbox' id='header-chb' class='k-checkbox'> ") 
                '    JsonString_Cols.Append("                                          <label class='k-checkbox-label' for='header-chb'></label>"", ") 
                '    JsonString_Cols.Append("                ""template"": ""function(dataItem){ ") 
                '    JsonString_Cols.Append("                    return \""<input type='checkbox' id='${dataItem." & l(i)._Nome_colonna_DT & "}' class='k-checkbox'> ") 
                '    JsonString_Cols.Append("                                          <label class='k-checkbox-label' for='${dataItem." & l(i)._Nome_colonna_DT & "}'></label>\"" ") 
                '    JsonString_Cols.Append("              } "" ") 
                '    JsonString_Cols.Append(" }"  )

                'Else



                'End If

                Dim sFilter As String = ""
                Dim sOperatoreFiltro As String = ""

                If ImpostaFiltroColonneStdGIAS Then
                    If l(i)._Tipo = "string" Then
                        sFilter = ", ""filterable"": { ""multi"": true, ""search"": true } "
                    Else
                        sFilter = ", ""filterable"": { ""cell"": { ""operator"": ""__OP__"", ""suggestionOperator"": ""contains"" } }"

                        Select Case l(i)._Tipo
                            Case "number"

                                If ImpostaOperatoreFiltroDaTipoDati Then
                                    sOperatoreFiltro = "gte"
                                Else
                                    sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                End If

                                sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                            Case "date"

                                If ImpostaOperatoreFiltroDaTipoDati Then
                                    sOperatoreFiltro = "contains"
                                Else
                                    sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                End If

                                sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                            Case "boolean"

                                If ImpostaOperatoreFiltroDaTipoDati Then
                                    sOperatoreFiltro = "eq"
                                Else
                                    sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                End If

                                sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                            Case Else

                                sFilter = ", ""filterable"": { ""multi"": true, ""search"": true } "
                        End Select
                    End If

                Else
                    ' VAnni: 25/1/2017: impostazioni di filtro
                    If l(i)._Filtrabile Then

                        ' VAnni: 25/1/2017: standard
                        If Not l(i)._FiltrabileConCheck AndAlso
                        (TipoFiltroKendo = TipiEnumerativi.TipoFiltroKendo_colonne.CasellaTesto OrElse TipoFiltroKendo = TipiEnumerativi.TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa) Then

                            sFilter = ", ""filterable"": { ""cell"": { ""operator"": ""__OP__"", ""suggestionOperator"": ""contains"" } }"


                            Select Case l(i)._Tipo
                                Case "string"


                                    sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                    sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)


                                Case "number"

                                    If ImpostaOperatoreFiltroDaTipoDati Then
                                        sOperatoreFiltro = "gte"
                                    Else
                                        sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                    End If

                                    sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                                Case "date"

                                    If ImpostaOperatoreFiltroDaTipoDati Then
                                        sOperatoreFiltro = "contains"
                                    Else
                                        sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                    End If

                                    sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                                Case "boolean"

                                    If ImpostaOperatoreFiltroDaTipoDati Then
                                        sOperatoreFiltro = "eq"
                                    Else
                                        sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                    End If

                                    sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                                Case Else


                                    sOperatoreFiltro = l(i)._OperatoreFiltro_Kendo
                                    sFilter = sFilter.Replace("__OP__", sOperatoreFiltro)

                            End Select

                        Else
                            sFilter = ", ""filterable"": { ""multi"": true, ""search"": true } "
                        End If

                    Else
                        sFilter = ", ""filterable"": false "
                    End If

                End If

                ' VAnni: 25/1/2017: impostazioni di template
                Dim sTemplate As String = ""
                If l(i)._FormatoParticolare <> "" Then
                    sTemplate = ", ""template"": """ & l(i)._FormatoParticolare & """"
                ElseIf l(i)._TemplateHtmlID <> "" Then
                    'Giulia : 04/09/2020: In alternativa a FormatoParticolare è possibile indicare l'id dello script con type="text/x-kendo-template"
                    'che verrà sostituito sul creaKendoGrid costruendo la giusta funzione per il template (non è possibile memorizza funzioni come valori in json)
                    sTemplate = ", ""templateIdControllo"": """ & l(i)._TemplateHtmlID & """"
                End If

                ' Stefano: 9/3/2018: le colonne vengono associate ad un gruppo per poter poi essere mostrate o meno 
                Dim sGruppoColonne As String = ""
                If l(i)._gruppoColonne <> "" Then
                    sGruppoColonne = ", ""gruppoColonne"": """ & l(i)._gruppoColonne & """"
                End If

                Dim sEditor As String = ""
                ' VAnni: 15/2/2017: todo: verificare .. non è realizzabile lato server, poiché non siamo in grado al momento di passare un oggetto ma solo una stringa
                'If l(i)._Editor <> "" Then
                '    sEditor = ", ""editor"": " & l(i)._Editor
                'End If

                ' VAnni: 25/1/2017: gestisce codifica html sui campi
                Dim sHtmlEncode As String = ""
                If l(i)._RemoveHtmlEncode Then
                    sHtmlEncode = ", ""encoded"": false "
                End If

                Dim sCss As String = ""
                If l(i)._cssHeader <> "" Then
                    sCss = ", ""headerAttributes"": { ""class"": """ & l(i)._cssHeader & """ } "
                End If
                If l(i)._css <> "" Then
                    sCss &= ", ""attributes"": { ""class"": """ & l(i)._css & """ } "
                End If

                Dim sFormatType As String = ""
                If l(i)._Tipo = "date" Then
                    Dim formatDataOra As String = "dd/MM/yyyy"
                    If Not String.IsNullOrEmpty(l(i)._formatDataOra) Then
                        formatDataOra = l(i)._formatDataOra
                    End If
                    sFormatType = ", ""format"": ""{0:" & formatDataOra & "}"" "
                End If
                If l(i)._Tipo = "number" AndAlso Not String.IsNullOrEmpty(l(i)._formatNr) Then
                    sFormatType = ", ""format"": ""{0:" & l(i)._formatNr & "}"" "
                End If

                Dim sDisplay As String = ""
                If Not l(i)._Display Then
                    sDisplay = ", ""hidden"": true "
                End If

                Dim sFooterTemplate As String = ""

                If l(i)._sum OrElse l(i)._min OrElse l(i)._max OrElse l(i)._average OrElse l(i)._count Then
                    sFooterTemplate = " ,""footerTemplate"": """
                End If
                If l(i)._sum Then
                    sFooterTemplate = sFooterTemplate & l(i)._stringaTotale & "#: kendo.toString(sum, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._min Then
                    sFooterTemplate = sFooterTemplate & "Min: #: kendo.toString(min, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._max Then
                    sFooterTemplate = sFooterTemplate & "Max: #: kendo.toString(max, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._average Then
                    sFooterTemplate = sFooterTemplate & "Media: #: kendo.toString(average, \""" & l(i)._average & "\"") # "
                End If
                If l(i)._count Then
                    sFooterTemplate = sFooterTemplate & "Cont: #: count # "
                End If
                If l(i)._sum OrElse l(i)._min OrElse l(i)._max OrElse l(i)._average OrElse l(i)._count Then
                    sFooterTemplate = sFooterTemplate & """"
                End If

                Dim sGroupFooterTemplate As String = ""

                If l(i)._sum OrElse l(i)._min OrElse l(i)._max OrElse l(i)._average OrElse l(i)._count Then
                    sGroupFooterTemplate = " ,""groupFooterTemplate"": """
                End If
                If l(i)._sum Then
                    sGroupFooterTemplate = sGroupFooterTemplate & "#: kendo.toString(sum, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._min Then
                    sGroupFooterTemplate = sGroupFooterTemplate & "Min: #: kendo.toString(min, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._max Then
                    sGroupFooterTemplate = sGroupFooterTemplate & "Max: #: kendo.toString(max, \""" & l(i)._formatNr & "\"") # "
                End If
                If l(i)._average Then
                    sGroupFooterTemplate = sGroupFooterTemplate & "Media: #: kendo.toString(average, \""" & l(i)._average & "\"") # "
                End If
                If l(i)._count Then
                    sGroupFooterTemplate = sGroupFooterTemplate & "Cont: #: count # "
                End If
                If l(i)._sum OrElse l(i)._min OrElse l(i)._max OrElse l(i)._average OrElse l(i)._count Then
                    sGroupFooterTemplate = sGroupFooterTemplate & """"
                End If

                'Imposto il width
                Dim sWidth As String = ""
                If l(i)._width <> "" Then
                    If IsNumeric(l(i)._width) Then
                        sWidth = ", ""widthfisso"": true, ""width"": " & l(i)._width & " "
                    Else
                        sWidth = ", ""widthfisso"": true, ""width"": """ & l(i)._width & """ "
                    End If
                End If

                Dim sLocked As String = ""
                If l(i)._locked Then
                    sLocked = ", ""locked"": true "
                End If

                Dim sDaDuplicare As String = ""
                If l(i)._daDuplicare Then
                    sDaDuplicare = ", ""daDuplicare"": true "
                End If

                'kendo col: 
                '{ field: "kendoKey", title: "kendoKey" },
                If Not l(i)._hidden Then
                    JsonString_Cols.Append("{ ""field"": """ & Escape_Kendo(l(i)._Nome_colonna_DT) & """, ""title"": """ & Escape_Kendo(l(i)._Nome_colonna_Json) & """ " & sFilter & sDisplay & sWidth & sLocked & sDaDuplicare & sEditor & sTemplate & sGruppoColonne & sFormatType & sCss & sHtmlEncode & sFooterTemplate & " } ")
                End If

                'kendo Model:
                ' nomecampo: { editable: false, type: "string" },

                If (Editabile_Default) Then
                    l(i)._Editabile = True
                End If

                Dim sValueDefault As String = ""

                If (l(i)._Tipo = "number" OrElse (l(i)._Tipo = "date" AndAlso l(i)._valueDefault = "null")) Then
                    sValueDefault = If(l(i)._valueDefault = "", "", ", ""defaultValue"": " & l(i)._valueDefault & "")
                Else
                    sValueDefault = If(l(i)._valueDefault = "", "", ", ""defaultValue"": """ & l(i)._valueDefault & """")
                End If

                'VALIDAZIONE
                Dim sValidazione As String = ""
                If l(i)._obbligatorio = True OrElse l(i)._listaFunzioniDiValidazione.Count > 0 Then
                    Dim listaRegole As New List(Of String)

                    If l(i)._obbligatorio = True Then
                        listaRegole.Add("""required"": true")
                    End If

                    'For Each v As funzioneDiValidazione In l(i)._listaFunzioniDiValidazione
                    '    Dim regola As String = ""
                    '    regola &= v.nome + ": function (input) {"
                    '    regola &= "if (input.is(""[name='" + Escape_Kendo(l(i)._Nome_colonna_DT) + "']"") && input.val() != """") {"
                    '    regola &= "input.attr('data-" + v.nome + "-msg', '" + v.messaggioErrore + "');"
                    '    regola &= "return " + v.testJavascriptBooleano + ";"
                    '    regola &= "}"
                    '    regola &= "return true;"
                    '    regola &= "}"
                    '    listaRegole.Add(regola)
                    'Next

                    sValidazione = ", ""validation"": {" & String.Join(",", listaRegole) & "}"

                End If
                'Dim sValidazione As String = IIf(l(i)._obbligatorio = True, ", ""validation"": {""required"": true}", "")


                If l(i)._generaModelloDati Then
                    JsonString_Model.Append(" """ & Escape_Kendo(l(i)._Nome_colonna_DT) & """: { ""editable"": " & l(i)._Editabile.ToString.ToLower & ", ""type"": """ & l(i)._Tipo & """" & sValueDefault & sValidazione & "}")
                End If


            End If
        Next

        JsonString_Model.Append("}")
        JsonString_Cols.Append("]")

        KendoCols = JsonString_Cols.ToString
        KendoModel = JsonString_Model.ToString
    End Sub

    Public Shared Sub kendo_Rows(dt As DataTable, l As List(Of ColonneNome), ByRef JsonString As StringBuilder)

        ' VAnni: 28/8/2017: gestito come parametro per riferimento.
        'Dim JsonString As New StringBuilder()
        JsonString.Append(" [ ")

        '@Paolo: Controllo se nella lista delle colonne esiste 'checked'
        'Dim z As Integer
        Dim check_flag As Boolean = False
        'For z = 0 To l.Count - 1
        '    If (l(z)._Nome_colonna_DT = "checked") Then
        '        check_flag = True
        '    End If
        'Next

        Dim checked = l.Where(Function(elem) elem._Nome_colonna_DT = "checked").FirstOrDefault
        If checked IsNot Nothing Then
            check_flag = True
        End If

        For i = 0 To dt.Rows.Count - 1

            If i > 0 Then
                JsonString.Append(",")
            End If
            JsonString.Append("{")

            Dim j As Integer
            For j = 0 To l.Count - 1
                If l(j)._nonPassare = False Then
                    If j > 0 Then
                        JsonString.Append(",")

                        '''@Paolo: Aggiungo la gestione del check di riga watable tramite la lettura ddel volore di riga su colonna "checked"
                    Else

                        ' Controllo se esiste la colonna "checked" nella DT 
                        If (check_flag AndAlso IsDBNull(dt.Rows(i).Item("checked")) <> True) Then

                            ' Controllo se checked è 1
                            If CInt(dt.Rows(i).Item("checked")) = 1 Then
                                JsonString.Append("""row-checked"":true")
                                JsonString.Append(", ")
                            End If
                        End If

                    End If
                    If (IsDBNull(dt.Rows(i).Item(l(j)._Nome_colonna_DT)) = True) Then

                        If l(j)._Tipo = "string" Then
                            JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: """ & "" & """")
                        Else
                            JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: null")
                        End If


                    Else

                        Select Case l(j)._Tipo
                            Case "string"
                                JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: """ & Escape_Kendo(dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString) & """")

                            Case "number"
                                JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: " & Escape_Kendo(CDec(dt.Rows(i).Item(l(j)._Nome_colonna_DT)).ToString.Replace(",", ".")) & " ")

                            Case "datetime"
                                '2017-03-14T11:31:16.861Z
                                JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: """ & CType(dt.Rows(i).Item(l(j)._Nome_colonna_DT), DateTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ") & """")

                            Case "json"
                                JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: " & Escape_Kendo(dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString) & "")

                            Case Else
                                JsonString.Append(" """ & Escape_Kendo(l(j)._Nome_colonna_DT) & """: """ & Escape_Kendo(dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString) & """")

                        End Select


                    End If
                End If
            Next

            JsonString.Append("}")

        Next
        JsonString.Append(" ]")

        ' VAnni: 28/8/2017: gestito come parametro per riferimento.
        'Return JsonString.ToString
    End Sub

    Public Shared Sub kendo_RowsOpt(dt As DataTable, l As List(Of ColonneNome), ByRef JsonString As StringBuilder)
        Dim jArray As New JArray

        Dim check_flag As Boolean = False

        Dim checked = l.Where(Function(elem) elem._Nome_colonna_DT = "checked").FirstOrDefault
        If checked IsNot Nothing Then
            check_flag = True
        End If

        For i = 0 To dt.Rows.Count - 1
            Dim obj As New JObject

            Dim j As Integer
            For j = 0 To l.Count - 1
                If l(j)._nonPassare = False AndAlso dt.Columns.Contains(l(j)._Nome_colonna_DT) = True Then
                    If j = 0 Then
                        ' Controllo se esiste la colonna "checked" nella DT 
                        If (check_flag AndAlso IsDBNull(dt.Rows(i).Item("checked")) <> True) Then
                            ' Controllo se checked è 1
                            If CInt(dt.Rows(i).Item("checked")) = 1 Then
                                obj("row-checked") = True
                            End If
                        End If

                    End If
                    If (IsDBNull(dt.Rows(i).Item(l(j)._Nome_colonna_DT)) = True) Then

                        If l(j)._Tipo = "string" Then
                            obj(l(j)._Nome_colonna_DT) = ""
                        Else
                            obj(l(j)._Nome_colonna_DT) = Nothing
                        End If


                    Else

                        Select Case l(j)._Tipo
                            Case "string"

                                obj(l(j)._Nome_colonna_DT) = dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString

                            Case "number"

                                obj(l(j)._Nome_colonna_DT) = CDec(dt.Rows(i).Item(l(j)._Nome_colonna_DT)) '.ToString.Replace(",", ".")

                            Case "datetime"

                                obj(l(j)._Nome_colonna_DT) = CType(dt.Rows(i).Item(l(j)._Nome_colonna_DT), DateTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

                            Case "json"

                                obj(l(j)._Nome_colonna_DT) = dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString

                            Case Else

                                obj(l(j)._Nome_colonna_DT) = dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString

                        End Select


                    End If
                End If
            Next

            jArray.Add(obj)

        Next

        JsonString.Append(jArray.ToString)
    End Sub

    Public Function JSON_DataTable_Kendo(dt As DataTable, l As List(Of ColonneNome),
                                         Optional AssegnaAutomaticamenteTipiDato As Boolean = False,
                                         Optional ByVal AssegnaAutomaticamenteTipoFiltro_daTipoDato As Boolean = False,
                                         Optional ByVal tipoFiltroKendo As TipiEnumerativi.TipoFiltroKendo_colonne = TipiEnumerativi.TipoFiltroKendo_colonne.Menu,
                                         Optional ByVal stringaKendoRow As String = "",
                                         Optional ImpostaFiltroColonneStdGIAS As Boolean = False) As String

        'se si è scelto di assegnare i tipi di dato in modo dinamico, rifaccio la lista di colonne nome scegliendo il dato sulla base del tipo nel datatable
        If AssegnaAutomaticamenteTipiDato Then
            Dim newL As New List(Of ColonneNome)

            For Each l1 In l
                For Each col As DataColumn In dt.Columns
                    'For iColonneDT As Integer = 0 To dt.Columns.Count - 1
                    'Dim col As DataColumn = dt.Columns(iColonneDT)

                    If col.ColumnName.ToLower = l1._Nome_colonna_DT.ToLower Then

                        Select Case col.DataType
                            Case Type.GetType("System.String")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))

                            Case Type.GetType("System.Integer"), Type.GetType("System.Int32"),
                                 Type.GetType("System.Int64"), Type.GetType("System.Int16"),
                                 Type.GetType("System.Long"), Type.GetType("System.Short"),
                                 Type.GetType("System.Single"), Type.GetType("System.Decimal"),
                                 Type.GetType("System.Double")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "number"))


                            Case Type.GetType("System.Boolean")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "boolean"))

                            Case Type.GetType("System.Date"), Type.GetType("System.DateTime")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "date"))

                            Case Else

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))

                        End Select

                    End If


                Next
            Next
            'sostituisco la lista di colonneNome con quella nuova
            l = newL
        End If


        Dim JsonString As New StringBuilder()

        Dim kendo_cols As String = ""
        Dim kendo_model As String = ""

        kendo_columns(l, AssegnaAutomaticamenteTipoFiltro_daTipoDato, Editabile_Deafault, kendo_cols, kendo_model, tipoFiltroKendo, ImpostaFiltroColonneStdGIAS:=ImpostaFiltroColonneStdGIAS)

        JsonString.Append("{ ")
        JsonString.Append(" ""kendo_columns"": " & kendo_cols)
        JsonString.Append(" ,""kendo_model"": " & kendo_model)

        JsonString.Append(" ,""kendo_rows"": ")
        If stringaKendoRow = "" Then
            kendo_Rows(dt, l, JsonString)
        Else
            JsonString.Append(stringaKendoRow)
        End If

        JsonString.Append("} ")

        Return JsonString.ToString()
    End Function

    Public Function JSON_DataTable_KendoOpt(dt As DataTable, l As List(Of ColonneNome),
                                         Optional AssegnaAutomaticamenteTipiDato As Boolean = False,
                                         Optional ByVal AssegnaAutomaticamenteTipoFiltro_daTipoDato As Boolean = False,
                                         Optional ByVal tipoFiltroKendo As TipiEnumerativi.TipoFiltroKendo_colonne = TipiEnumerativi.TipoFiltroKendo_colonne.Menu,
                                         Optional ByVal stringaKendoRow As String = "",
                                         Optional ByVal inParallelo As Boolean = False,
                                            Optional ByRef errorString As String = "",
                                         Optional ByVal filtro_Ordinamento As String = "") As String

        'se si è scelto di assegnare i tipi di dato in modo dinamico, rifaccio la lista di colonne nome scegliendo il dato sulla base del tipo nel datatable
        If AssegnaAutomaticamenteTipiDato Then
            Dim newL As New List(Of ColonneNome)

            For Each l1 In l
                For Each col As DataColumn In dt.Columns
                    'For iColonneDT As Integer = 0 To dt.Columns.Count - 1
                    'Dim col As DataColumn = dt.Columns(iColonneDT)

                    If col.ColumnName.ToLower = l1._Nome_colonna_DT.ToLower Then

                        Select Case col.DataType
                            Case Type.GetType("System.String")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))

                            Case Type.GetType("System.Integer"), Type.GetType("System.Int32"),
                                 Type.GetType("System.Int64"), Type.GetType("System.Int16"),
                                 Type.GetType("System.Long"), Type.GetType("System.Short"),
                                 Type.GetType("System.Single"), Type.GetType("System.Decimal"),
                                 Type.GetType("System.Double")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "number"))


                            Case Type.GetType("System.Boolean")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "boolean"))

                            Case Type.GetType("System.Date"), Type.GetType("System.DateTime")

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "date"))

                            Case Else

                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))

                        End Select

                    End If


                Next
            Next
            'sostituisco la lista di colonneNome con quella nuova
            l = newL
        End If


        Dim JsonString As New StringBuilder()

        Dim kendo_cols As String = ""
        Dim kendo_model As String = ""

        kendo_columns(l, AssegnaAutomaticamenteTipoFiltro_daTipoDato, Editabile_Deafault, kendo_cols, kendo_model, tipoFiltroKendo)

        JsonString.Append("{ ")
        JsonString.Append(" ""kendo_columns"": " & kendo_cols)
        JsonString.Append(" ,""kendo_model"": " & kendo_model)

        JsonString.Append(" ,""kendo_rows"": ")
        'Aggiunto da Scatto il 26/9/2022 per errori sul format dei numeri
        inParallelo = False
        'FINE Aggiunto da Scatto il 26/9/2022 per errori sul format dei numeri
        If stringaKendoRow = "" Then
            If inParallelo Then
                kendo_RowsOpt_Parallel(dt, l, JsonString, errorString, filtro_Ordinamento)
            Else
                kendo_RowsOpt(dt, l, JsonString)
                'kendo_Rows(dt, l, JsonString)
            End If
        Else
            JsonString.Append(stringaKendoRow)
        End If

        JsonString.Append("} ")

        Try
            Return JsonString.ToString()
        Catch ex As OutOfMemoryException
            Return " "
        End Try

    End Function


    Public Shared Function Escape(ByVal s As String) As String
        Return s.Replace("""", "&#34;").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ").Replace("\", "&#92;").Replace(vbTab, " ")
    End Function

    Public Shared Function Escape_Kendo(ByVal s As String) As String
        Dim sReturn As String = JsonConvert.SerializeObject(s)
        sReturn = sReturn.ToString.Remove(0, 1)
        sReturn = sReturn.ToString.Remove(sReturn.ToString.Length - 1, 1)
        Return sReturn
        'Return s.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
    End Function

    Public Shared Function Create_cols(l As List(Of ColonneNome)) As String
        Dim JsonString As New StringBuilder()
        JsonString.Append(" { ")
        For i = 0 To l.Count - 1
            If l(i)._nonPassare = False Then
                If i > 0 Then
                    JsonString.Append(",")
                End If
                JsonString.Append(" """ & Escape(l(i)._Nome_colonna_Json) & """: { ""index"":" & (i + 1) & ", ""type"":""" & l(i)._Tipo & """, ""dt"":""" & Escape(l(i)._Nome_colonna_DT) & """")
                If l(i)._ColonnaDiSelezione Then
                    JsonString.Append(" ,""unique"":true")
                End If
                If l(i)._Filtrabile = False Then
                    JsonString.Append(" ,""filter"":false")
                End If
                If l(i)._FiltrabileConCheck = False Then
                    JsonString.Append(" ,""filtrabilecheck"":false")
                Else
                    JsonString.Append(" ,""filtrabilecheck"":true")
                End If

                If l(i)._placeHolder <> "" Then
                    JsonString.Append(" ,""placeHolder"":""" & l(i)._placeHolder & """")
                End If
                If l(i)._FormatoParticolare <> "" Then
                    JsonString.Append(" ,""format"":""" & l(i)._FormatoParticolare & """")
                ElseIf l(i)._TemplateHtmlID <> "" Then
                    'Giulia : 04/09/2020: In alternativa a FormatoParticolare è possibile indicare l'id dello script con type="text/x-kendo-template"
                    'che verrà sostituito sul creaKendoGrid costruendo la giusta funzione per il template (non è possibile memorizza funzioni come valori in json)
                    JsonString.Append(", ""templateIdControllo"": """ & l(i)._TemplateHtmlID & """")
                End If
                If l(i)._hidden = True Then
                    JsonString.Append(" ,""hidden"": true ")
                End If

                If l(i)._css <> "" Then
                    JsonString.Append(" ,""cls"":""" & l(i)._css & """")
                End If


                JsonString.Append("}")
            End If
        Next
        JsonString.Append(" }")
        Return JsonString.ToString
    End Function



    Public Shared Function Create_rows(dt As DataTable, l As List(Of ColonneNome)) As String
        Dim JsonString As New StringBuilder()
        JsonString.Append(" [ ")

        '@Paolo: Controllo se nella lista delle colonne esiste 'checked'
        Dim z As Integer
        Dim check_flag As Boolean = False
        For z = 0 To l.Count - 1
            If (l(z)._Nome_colonna_DT = "checked") Then
                check_flag = True
            End If
        Next

        For i = 0 To dt.Rows.Count - 1

            If i > 0 Then
                JsonString.Append(", ")
            End If
            JsonString.Append("{")

            Dim j As Integer
            For j = 0 To l.Count - 1
                If l(j)._nonPassare = False Then
                    If j > 0 Then
                        JsonString.Append(",")

                        '''@Paolo: Aggiungo la gestione del check di riga watable tramite la lettura ddel volore di riga su colonna "checked"
                    Else

                        ' Controllo se esiste la colonna "checked" nella DT 
                        If (check_flag AndAlso IsDBNull(dt.Rows(i).Item("checked")) <> True) Then

                            ' Controllo se checked è 1
                            If CInt(dt.Rows(i).Item("checked")) = 1 Then
                                JsonString.Append("""row-checked"":true")
                                JsonString.Append(", ")
                            End If
                        End If

                    End If
                    If (IsDBNull(dt.Rows(i).Item(l(j)._Nome_colonna_DT)) = True) Then
                        JsonString.Append(" """ & Escape(l(j)._Nome_colonna_Json) & """: """ & "" & """")
                    Else
                        JsonString.Append(" """ & Escape(l(j)._Nome_colonna_Json) & """: """ & Escape(dt.Rows(i).Item(l(j)._Nome_colonna_DT).ToString) & """")
                    End If
                End If
            Next

            JsonString.Append("}")

        Next
        JsonString.Append(" ]")
        Return JsonString.ToString
    End Function

    Public Function JSON_DataTable(dt As DataTable, l As List(Of ColonneNome), Optional AssegnaAutomaticamenteTipiDato As Boolean = False) As String

        'se si è scelto di assegnare i tipi di dato in modo dinamico, rifaccio la lista di colonne nome scegliendo il dato sulla base del tipo nel datatable
        If AssegnaAutomaticamenteTipiDato Then
            Dim newL As New List(Of ColonneNome)

            For Each l1 In l
                For Each col As DataColumn In dt.Columns
                    'For iColonneDT As Integer = 0 To dt.Columns.Count - 1
                    'Dim col As DataColumn = dt.Columns(iColonneDT)

                    If col.ColumnName.ToLower = l1._Nome_colonna_DT.ToLower Then

                        Select Case col.DataType
                            Case Type.GetType("System.String")
                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))
                            Case Type.GetType("System.Integer"), Type.GetType("System.Int32"),
                                 Type.GetType("System.Int64"), Type.GetType("System.Int16"),
                                 Type.GetType("System.Long"), Type.GetType("System.Short"),
                                 Type.GetType("System.Single"), Type.GetType("System.Decimal"),
                                 Type.GetType("System.Double")
                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "number"))
                                'Case Type.GetType("System.Single"), Type.GetType("System.Decimal")
                                '    newL.Add(New ColonneNome(col.ColumnName, col.ColumnName, "string"))
                            Case Type.GetType("System.Boolean")
                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "bool"))
                            Case Type.GetType("System.Date"), Type.GetType("System.DateTime")
                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "date"))
                            Case Else
                                newL.Add(New ColonneNome(col.ColumnName, l1._Nome_colonna_Json, "string"))
                        End Select

                    End If


                Next
            Next
            'sostituisco la lista di colonneNome con quella nuova
            l = newL
        End If


        Dim JsonString As New StringBuilder()

        JsonString.Append("{ ")
        JsonString.Append(" ""cols"": " & Create_cols(l))
        JsonString.Append(" ,""rows"": " & Create_rows(dt, l))
        JsonString.Append("} ")

        Return JsonString.ToString()
    End Function



    Public Function JSON_DataTable_Senza_Colonne_Gia_Inserite(dt As DataTable, l As List(Of ColonneNome), ByVal l_old As List(Of ColonneNome), Optional AssegnaAutomaticamenteTipiDato As Boolean = False, Optional nonPassare_al_js As Boolean = False) As String

        'se si è scelto di assegnare i tipi di dato in modo dinamico, rifaccio la lista di colonne nome scegliendo il dato sulla base del tipo nel datatable
        If AssegnaAutomaticamenteTipiDato Then


            For Each col As DataColumn In dt.Columns
                Dim aggiungi As Boolean
                aggiungi = True
                For Each kk In l_old
                    If kk._Nome_colonna_DT.ToLower = col.ColumnName.ToLower Then
                        aggiungi = False
                    End If
                Next
                'For iColonneDT As Integer = 0 To dt.Columns.Count - 1
                'Dim col As DataColumn = dt.Columns(iColonneDT)
                If aggiungi = True Then

                    Select Case col.DataType
                        Case Type.GetType("System.String")
                            Dim e = New ColonneNome(col.ColumnName, col.ColumnName, "string")
                            e._nonPassare = nonPassare_al_js
                            l.Add(e)
                        Case Type.GetType("System.Integer"), Type.GetType("System.Int32"),
                             Type.GetType("System.Int64"), Type.GetType("System.Int16"),
                             Type.GetType("System.Long"), Type.GetType("System.Short"),
                             Type.GetType("System.Single"), Type.GetType("System.Decimal")

                            Dim e = New ColonneNome(col.ColumnName, col.ColumnName, "number")
                            e._nonPassare = nonPassare_al_js
                            l.Add(e)

                        Case Type.GetType("System.Boolean")
                            Dim e = New ColonneNome(col.ColumnName, col.ColumnName, "bool")
                            e._nonPassare = nonPassare_al_js
                            l.Add(e)
                        Case Type.GetType("System.Date"), Type.GetType("System.DateTime")

                            Dim e = New ColonneNome(col.ColumnName, col.ColumnName, "date")
                            e._nonPassare = nonPassare_al_js
                            l.Add(e)
                        Case Else
                            Dim e = New ColonneNome(col.ColumnName, col.ColumnName, "string")
                            e._nonPassare = nonPassare_al_js
                            l.Add(e)
                    End Select
                End If
            Next
        End If


        Dim JsonString As New StringBuilder()

        JsonString.Append("{ ")
        JsonString.Append(" ""cols"": " & Create_cols(l))
        JsonString.Append(" ,""rows"": " & Create_rows(dt, l))
        JsonString.Append("} ")

        Return JsonString.ToString()
    End Function

    ' @Paolo
    ' Sviluppo lettura DA stringa JSON A Datatable
    ' Dati passati:
    ' 1) Datatable per nome e tipizzazione colonna
    ' 2) stringa JSON con dati
    Public Function GetDataTableFromJSON(ByVal Dt_colonne As DataTable, ByVal jS As String) As DataTable

        'Dim rigArr() As String
        'Dim rigArr2() As String
        'Dim rigArr3() As String

        '' Pulisco da parentesi graffe iniziali
        'jS = jS.Substring(1, jS.Length - 1)

        'rigArr = jS.Split(New String() {"},{"}, StringSplitOptions.None)
        '' ciclo le righe json
        'For count = 0 To rigArr.Length - 1

        '    rigArr2 = rigArr(count).Split(",")

        '    ' ciclo coppia campi json
        '    For count2 = 0 To rigArr2.Length - 1

        '        rigArr3 = jS.Split("",")

        '        ' ciclo campo json
        '        For count3 = 0 To rigArr2.Length - 1

        '            ' ciclo le colonne della tabella
        '            For i = 0 To Dt_colonne.Columns.Count - 1
        '                If rigArr2(count2) = Dt_colonne.Columns(i).ColumnName Then

        '                End If
        '            Next


        '        Next

        '    Next

        'Next


        Return Dt_colonne
    End Function

    Public Shared Function getListaColonneFromDT(ByVal dt As DataTable) As List(Of ColonneNome)
        Dim r As New List(Of ColonneNome)
        Dim i As Integer
        For i = 0 To dt.Columns.Count - 1
            Dim n As New ColonneNome(dt.Columns(i), dt.Columns(i).ColumnName)
            r.Add(n)
        Next
        Return r
    End Function


    Public Shared Function getListaColonneFromDT(ByVal dt As DataTable, ByVal ColonneEscluse As List(Of String)) As List(Of ColonneNome)

        Dim myList2 As List(Of String) = ColonneEscluse.ConvertAll(Function(d) d.ToLower())

        Dim r As New List(Of ColonneNome)
        Dim i As Integer
        For i = 0 To dt.Columns.Count - 1
            If Not myList2.Contains(dt.Columns(i).ColumnName.ToLower) Then
                Dim n As New ColonneNome(dt.Columns(i), dt.Columns(i).ColumnName)
                r.Add(n)
            End If
        Next
        Return r
    End Function

    Public Shared Sub kendo_RowsOpt_Parallel(dt As DataTable, l As List(Of ColonneNome), ByRef JsonString As StringBuilder, ByRef ErrorString As String, Optional ByVal filtro_Ordinamento As String = "")

        Dim altered As Boolean = False

        Dim jArray As New JArray
        Dim res As String = ""
        Dim check_flag As Boolean = False

        Dim checked = l.Where(Function(elem) elem._Nome_colonna_DT = "checked").FirstOrDefault
        If checked IsNot Nothing Then
            check_flag = True
        End If
        Dim processori = Environment.ProcessorCount

        If dt.Rows.Count * l.Count > COLUMNS_X_ROWS_MAX_ALLOWANCE Then

            dt = dt.AsEnumerable().Take(CInt(Math.Truncate((ROWS_TESTED * l.Count) / COLUMNS_TESTED))).CopyToDataTable
            altered = True

        End If

        Try

            Parallel.ForEach(dt.AsEnumerable().AsParallel().AsOrdered, New ParallelOptions With {.MaxDegreeOfParallelism = processori},
                                                         Sub(row As DataRow, state As ParallelLoopState)

                                                             Dim obj As New JObject

                                                             Dim j As Integer
                                                             Try
                                                                 For j = 0 To l.Count - 1
                                                                     If l(j)._nonPassare = False AndAlso dt.Columns.Contains(l(j)._Nome_colonna_DT) = True Then
                                                                         If j = 0 Then
                                                                             ' Controllo se esiste la colonna "checked" nella DT 
                                                                             If (check_flag AndAlso IsDBNull(row.Item("checked")) <> True) Then
                                                                                 ' Controllo se checked è 1
                                                                                 If CInt(row.Item("checked")) = 1 Then
                                                                                     obj("row-checked") = True
                                                                                 End If
                                                                             End If

                                                                         End If
                                                                         If (IsDBNull(row.Item(l(j)._Nome_colonna_DT)) = True) Then

                                                                             If l(j)._Tipo = "string" Then
                                                                                 obj(l(j)._Nome_colonna_DT) = ""
                                                                             Else
                                                                                 obj(l(j)._Nome_colonna_DT) = Nothing
                                                                             End If


                                                                         Else

                                                                             Select Case l(j)._Tipo
                                                                                 Case "string"

                                                                                     obj(l(j)._Nome_colonna_DT) = row.Item(l(j)._Nome_colonna_DT).ToString

                                                                                 Case "number"

                                                                                     obj(l(j)._Nome_colonna_DT) = CDec(row.Item(l(j)._Nome_colonna_DT)) '.ToString.Replace(",", ".")

                                                                                 Case "datetime"

                                                                                     obj(l(j)._Nome_colonna_DT) = CType(row.Item(l(j)._Nome_colonna_DT), DateTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

                                                                                 Case "json"

                                                                                     obj(l(j)._Nome_colonna_DT) = row.Item(l(j)._Nome_colonna_DT).ToString

                                                                                 Case Else

                                                                                     obj(l(j)._Nome_colonna_DT) = row.Item(l(j)._Nome_colonna_DT).ToString

                                                                             End Select


                                                                         End If
                                                                     End If
                                                                 Next

                                                                 SyncLock jArray
                                                                     jArray.Add(obj)
                                                                 End SyncLock

                                                             Catch ex As OutOfMemoryException
                                                                 state.Break()
                                                             End Try

                                                         End Sub)

            If (Not String.IsNullOrEmpty(filtro_Ordinamento)) Then
                Dim listaOrdinamento = filtro_Ordinamento.Split({","c})
                Dim resEnumerable = jArray.OrderBy(Function(obj) obj(listaOrdinamento.First.ToString))

                If listaOrdinamento.Count > 1 Then
                    For index = 1 To listaOrdinamento.Count - 1
                        Dim tmp = index
                        resEnumerable = resEnumerable.ThenBy(Function(obj) obj(listaOrdinamento.GetValue(tmp).ToString.Trim()))
                    Next
                End If

                jArray = JArray.FromObject(resEnumerable)
            End If

            res = jArray.ToString
            JsonString.Append(res)
            'jArray.Clear()
        Catch ex As OutOfMemoryException

            altered = False
            ErrorString = " Numero totale di righe troppo elevate per essere gestito, agire sui filtri e sul periodo temporale per un’estrazione dati mirata contenente tutte le righe interessate."
            JsonString.Length = 0

        End Try

        If altered Then

            ErrorString = "Sono state estratte solo " & dt.Rows.Count & " righe per il periodo " & jArray.First.Item("Data_Inserimento").ToString & " - " &
                jArray.Last.Item("Data_Inserimento").ToString & " in quanto il numero totale delle righe era troppo elevato per poter essere gestito. " &
                " Agire sui filtri e sul periodo temporale per un’estrazione dati mirata contenente tutte le righe interessate."

        End If

    End Sub

End Class
