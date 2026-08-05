Public Class Fertilizzanti

    Public Sub New()

    End Sub

    Public Function Fertilizzanti(ByVal Input As Fertilizzanti_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                   As Fertilizzanti_output

        Dim Output As New Fertilizzanti_output
        Dim Elemento As Fertilizzante
        Dim Tipologia As Tipologia
        Dim Ditta As Ditta
        Dim HashF As New Hashtable


        If Not IsNothing(Input.Lingua_Cod) AndAlso Input.Lingua_Cod > 0 Then
            objParametri.Lingua_Cod = Input.Lingua_Cod
        End If

        Dim objCore As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
        Dim Dt As DataTable = objCore.Leggi_WS(Input.Codice,
                                               Input.Descrizione,
                                               Input.Tipo,
                                               Input.Regolamento,
                                               Input.DataInizio,
                                               Input.DataFine,
                                               Input.strFiltro,
                                               Input.strOrdinamento,
                                               Input.IncludiTipologia,
                                               Input.Stato_Cod,
                                                objParametri)

        'estraggo, se richieste, le tipologie
        If Input.IncludiTipologia Then
            For i = 0 To Dt.Rows.Count - 1
                If HashF.ContainsKey(Dt.Rows(i).Item("Fer_Cod")) Then
                    HashF(Dt.Rows(i).Item("Fer_Cod")) = HashF(Dt.Rows(i).Item("Fer_Cod")) & "," & Dt.Rows(i).Item("tp_cod") & "|" & Dt.Rows(i).Item("tp_des")
                Else
                    HashF.Add(Dt.Rows(i).Item("Fer_Cod"), Dt.Rows(i).Item("tp_cod") & "|" & Dt.Rows(i).Item("tp_des"))
                End If
            Next
        End If

        'leggo, se richieste, le ditte
        Dim DtDitte As DataTable
        If Input.IncludiDitte Then
            Dim objDitte As New AgronicaCoreMetaSchemaDAL.FertilizzantixDitte_R
            DtDitte = objDitte.Leggi(0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        End If

        'leggo le Formulazioni
        Dim DtFormulazioni As DataTable
        Dim objFormulazioni As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
        DtFormulazioni = objFormulazioni.FertilizzantixFormulazioni_Leggi("", "", "", objParametri)

        Dim HashInseriti As New Hashtable
        Dim Tipologie() As String

        For i = 0 To Dt.Rows.Count - 1

            If Not HashInseriti.ContainsKey(Dt.Rows(i).Item("Fer_Cod")) Then

                HashInseriti.Add(Dt.Rows(i).Item("Fer_Cod"), "")

                Elemento = New Fertilizzante

                Elemento.Codice = Dt.Rows(i).Item("Fer_Cod")
                Elemento.Descrizione = Dt.Rows(i).Item("Fer_Des")

                Select Case Dt.Rows(i).Item("Bio")
                    Case 1
                        Elemento.Biologico = True
                    Case Else
                        Elemento.Biologico = False
                End Select

                If Input.IncludiApporti = True Then
                    Elemento.N = Dt.Rows(i).Item("N")
                    Elemento.P2O5 = Dt.Rows(i).Item("P2O5")
                    Elemento.K2O = Dt.Rows(i).Item("K2O")
                    Elemento.MgO = Dt.Rows(i).Item("MgO")
                    Elemento.Cu = Dt.Rows(i).Item("Cu")
                End If

                If Input.IncludiTipologia And Input.Tipo < 6 Then
                    Tipologie = Split(HashF(Dt.Rows(i).Item("Fer_Cod")), ",")
                    If Not Tipologie Is Nothing AndAlso Tipologie.Length > 0 Then
                        For t = 0 To Tipologie.Length - 1
                            Tipologia = New Tipologia
                            Tipologia.Codice = Tipologie(t).Split("|")(0)
                            Tipologia.Descrizione = Tipologie(t).Split("|")(1)
                            Elemento.ListaTipologie.Add(Tipologia)
                        Next
                    End If
                End If

                If Input.IncludiDitte Then
                    Dim DrDitte() As DataRow
                    DrDitte = DtDitte.Select("Fer_Cod=" & Dt.Rows(i).Item("Fer_Cod"))
                    If Not DrDitte Is Nothing Then
                        For Each dr As DataRow In DrDitte
                            Ditta = New Ditta
                            Ditta.Codice = dr("ditta_cod")
                            Ditta.Descrizione = dr("Ditta_Des")
                            Elemento.ListaDitte.Add(Ditta)
                        Next
                    End If
                End If

                If Input.Tipo >= 6 Then
                    Elemento.TipoFertilizzanteCodice = Dt.Rows(i).Item("id_tp_fer")
                    Elemento.TipoFertilizzanteDescrizione = Dt.Rows(i).Item("descrizione")
                    Elemento.Udm_Cod = Dt.Rows(i).Item("Udm_Cod")
                    Elemento.Eff_Cod = Dt.Rows(i).Item("Eff_Cod")
                Else

                    '(14/09/2020 fede) aggiunta lettura unita misura (se presente)
                    Dim DrFormulazioni() As DataRow
                    DrFormulazioni = DtFormulazioni.Select("Fer_Cod=" & Dt.Rows(i).Item("Fer_Cod"))
                    If Not DrFormulazioni Is Nothing AndAlso DrFormulazioni.Length > 0 Then
                        If Not IsDBNull(DrFormulazioni(0).Item("Udm_Cod")) AndAlso CInt(DrFormulazioni(0).Item("Udm_Cod")) <> 0 Then
                            Elemento.Udm_Cod = DrFormulazioni(0).Item("Udm_Cod")
                        Else
                            If Not IsDBNull(DrFormulazioni(0).Item("FORM_FER_COD")) AndAlso CInt(DrFormulazioni(0).Item("FORM_FER_COD")) Then
                                Select Case CInt(DrFormulazioni(0).Item("FORM_FER_COD"))
                                    Case 1 'solido
                                        Elemento.Udm_Cod = AgronicaCoreDataProvider.TipiEnumerativi.enum_UnitaMisura.KG
                                    Case 2, 3 'liquido
                                        Elemento.Udm_Cod = AgronicaCoreDataProvider.TipiEnumerativi.enum_UnitaMisura.Litri
                                End Select
                            End If
                        End If
                    End If
                End If

                Output.ListaFertilizzanti.Add(Elemento)

            End If

        Next

        Return Output

    End Function


End Class


Public Class Fertilizzanti_input

    Public Codice As Integer
    Public Descrizione As String
    Public Regolamento As Integer 'x bio (reg=-2) e dpi (reg<>0)
    Public Tipo As Integer
    Public DataInizio As Date
    Public DataFine As Date
    Public strFiltro As String
    Public strOrdinamento As String

    Public IncludiApporti As Boolean
    Public IncludiTipologia As Boolean
    Public IncludiDitte As Boolean

    Public Stato_Cod As String
    Public Lingua_Cod As Integer

    Public Url As String

    Sub New()

        Codice = 0
        Descrizione = ""
        Regolamento = 0
        Tipo = 0
        DataInizio = #1/1/1900#
        DataFine = #12/31/2100#
        strFiltro = ""
        strOrdinamento = ""
        IncludiApporti = False
        IncludiTipologia = False
        IncludiDitte = False
        Stato_Cod = ""
        Lingua_Cod = 0
        Url = ""

    End Sub

End Class

Public Class Fertilizzanti_output

    Public ListaFertilizzanti As List(Of Fertilizzante)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFertilizzanti = New List(Of Fertilizzante)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Fertilizzante

    Public Descrizione As String
    Public Codice As Integer
    Public N As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public MgO As Decimal
    Public Cu As Decimal

    Public Udm_Cod As Integer
    Public Eff_Cod As Integer

    Public Biologico As Boolean

    Public ListaTipologie As List(Of Tipologia)

    Public ListaDitte As List(Of Ditta)

    Public TipoFertilizzanteCodice As Integer   'per PAN
    Public TipoFertilizzanteDescrizione As String
    Sub New()

        Descrizione = ""
        Codice = 0
        N = 0
        P2O5 = 0
        K2O = 0
        MgO = 0
        Cu = 0
        Udm_Cod = 0
        Eff_Cod = 0

        ListaTipologie = New List(Of Tipologia)

        ListaDitte = New List(Of Ditta)

        TipoFertilizzanteCodice = 0
        TipoFertilizzanteDescrizione = ""

        Biologico = False

    End Sub

End Class

Public Class Tipologia

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class

Public Class Ditta

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class


