
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PianoConcimazione_FattoriCorrettivi

    Public Sub New()

    End Sub

    Public Function FattoriCorrettivi(ByVal FattoriCorrettiviInput As PianoConcimazione_FattoriCorrettivi_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_FattoriCorrettivi_output

        Dim FattoriCorrettiviOutput As New PianoConcimazione_FattoriCorrettivi_output
        Dim FattoreCorrettivo As FattoreCorrettivo

        Dim Dt As DataTable
        Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R

        Dim Filtro As String = ""
        If FattoriCorrettiviInput.SoloVisibili = True Then
            Filtro = " FC.Visibile = 1 "
        End If
        If FattoriCorrettiviInput.SoloValorizzati = True Then
            If Filtro <> "" Then
                Filtro &= " AND "
            End If
            '(28/02/2019 fede) aggiunta lettura forzata dose elevata fosforo e potassio che può essere = 0
            'Filtro &= " FCS.Valore > 0 "
            Filtro &= " (FCS.Valore > 0 or fcs.Fattore_Cod = " & enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Elevata & " or fcs.Fattore_Cod = " & enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Elevata & " )"
        End If

        Dt = objFattoriCorrettivi.Leggi2(FattoriCorrettiviInput.Regolamento_Cod,
                                                        FattoriCorrettiviInput.Fattore_Cod, FattoriCorrettiviInput.Tipo, FattoriCorrettiviInput.Variazione,
                                                        FattoriCorrettiviInput.Veg_Cod,
                                                        FattoriCorrettiviInput.Grfi_Cod,
                                                        Filtro, "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            FattoreCorrettivo = New FattoreCorrettivo
            FattoreCorrettivo.Descrizione = Dt.Rows(i).Item("Fattore_Des")
            FattoreCorrettivo.Codice = Dt.Rows(i).Item("Fattore_Cod")
            FattoreCorrettivo.Tipo = Dt.Rows(i).Item("Tipo")
            FattoreCorrettivo.Variazione = Dt.Rows(i).Item("Variazione")
            FattoreCorrettivo.Valore = CDec(Format(Dt.Rows(i).Item("Valore"), "0.####"))
            FattoreCorrettivo.Visibile = Dt.Rows(i).Item("Visibile")
            FattoreCorrettivo.Regolamento_Cod = Dt.Rows(i).Item("Regolamento_cod")
            FattoreCorrettivo.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")
            FattoreCorrettivo.Grfi_Cod = Dt.Rows(i).Item("Grfi_Cod")
            FattoreCorrettivo.Controllo_Valore = CDec(Format(Dt.Rows(i).Item("Controllo_Valore"), "0.####"))
            FattoreCorrettivo.Controllo_Funzione = Dt.Rows(i).Item("Controllo_Funzione")
            FattoriCorrettiviOutput.ListaFattoriCorrettivi.Add(FattoreCorrettivo)

        Next

        Return FattoriCorrettiviOutput

    End Function


    Public Function FattoriCorrettivi_ConFinalitaGias(ByVal FattoriCorrettiviInput As PianoConcimazione_FattoriCorrettivi_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_FattoriCorrettivi_output

        Dim FattoriCorrettiviOutput As New PianoConcimazione_FattoriCorrettivi_output
        Dim FattoreCorrettivo As FattoreCorrettivo

        Dim Dt As DataTable
        Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R

        Dim Filtro As String = ""
        If FattoriCorrettiviInput.SoloVisibili = True Then
            Filtro = " FC.Visibile = 1 "
        End If
        If FattoriCorrettiviInput.SoloValorizzati = True Then
            If Filtro <> "" Then
                Filtro &= " AND "
            End If
            '(28/02/2019 fede) aggiunta lettura forzata dose elevata fosforo e potassio che può essere = 0
            'Filtro &= " FCS.Valore > 0 "
            Filtro &= " (FCS.Valore > 0 or fcs.Fattore_Cod = " & enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Elevata & " or fcs.Fattore_Cod = " & enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Elevata & " )"
        End If

        Dt = objFattoriCorrettivi.Leggi_conFinalitaGias(FattoriCorrettiviInput.Regolamento_Cod,
                                                        FattoriCorrettiviInput.Fattore_Cod, FattoriCorrettiviInput.Tipo, FattoriCorrettiviInput.Variazione,
                                                        FattoriCorrettiviInput.Veg_Cod,
                                                        FattoriCorrettiviInput.Grfi_Cod, FattoriCorrettiviInput.Grfi_Cod_Gias,
                                                        Filtro, "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            FattoreCorrettivo = New FattoreCorrettivo
            FattoreCorrettivo.Descrizione = Dt.Rows(i).Item("Fattore_Des")
            FattoreCorrettivo.Codice = Dt.Rows(i).Item("Fattore_Cod")
            FattoreCorrettivo.Tipo = Dt.Rows(i).Item("Tipo")
            FattoreCorrettivo.Variazione = Dt.Rows(i).Item("Variazione")
            FattoreCorrettivo.Valore = CDec(Format(Dt.Rows(i).Item("Valore"), "0.####"))
            FattoreCorrettivo.Visibile = Dt.Rows(i).Item("Visibile")
            FattoreCorrettivo.Regolamento_Cod = Dt.Rows(i).Item("Regolamento_cod")
            FattoreCorrettivo.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")
            FattoreCorrettivo.Grfi_Cod = Dt.Rows(i).Item("Grfi_Cod")
            FattoreCorrettivo.Grfi_Cod_Gias = Dt.Rows(i).Item("Grfi_Cod_Gias")
            FattoreCorrettivo.Controllo_Valore = CDec(Format(Dt.Rows(i).Item("Controllo_Valore"), "0.####"))
            FattoreCorrettivo.Controllo_Funzione = Dt.Rows(i).Item("Controllo_Funzione")
            FattoriCorrettiviOutput.ListaFattoriCorrettivi.Add(FattoreCorrettivo)

        Next

        Return FattoriCorrettiviOutput

    End Function

End Class

Public Class PianoConcimazione_FattoriCorrettivi_input

    Public Regolamento_Cod As Integer

    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Grfi_Cod_Gias As Integer

    Public Fattore_Cod As Integer
    Public Tipo As String
    Public Variazione As String

    Public SoloVisibili As Boolean
    Public SoloValorizzati As Boolean

    Public url As String

    Sub New()

        Fattore_Cod = 0
        Tipo = ""
        Variazione = ""

        SoloVisibili = False
        SoloValorizzati = False

    End Sub

End Class

Public Class PianoConcimazione_FattoriCorrettivi_output

    Public ListaFattoriCorrettivi As List(Of FattoreCorrettivo)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFattoriCorrettivi = New List(Of FattoreCorrettivo)
        MessaggioErrore = ""

    End Sub

End Class

Public Class FattoreCorrettivo

    Public Descrizione As String
    Public Codice As Integer
    Public Tipo As String
    Public Variazione As String
    Public Valore As Decimal
    Public Visibile As Integer
    Public Regolamento_Cod As Integer

    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Grfi_Cod_Gias As Integer

    Public Controllo_Valore As Decimal
    Public Controllo_Funzione As String

    Sub New()

        Descrizione = ""
        Codice = 0
        Tipo = ""
        Variazione = ""
        Valore = 0
        Visibile = 0
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Grfi_Cod_Gias = 0
        Controllo_Valore = 0
        Controllo_Funzione = ""

    End Sub

End Class

