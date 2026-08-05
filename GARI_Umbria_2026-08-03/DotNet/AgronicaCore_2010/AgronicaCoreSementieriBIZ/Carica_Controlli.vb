
Imports System.Web.ui.webcontrols


Public Class Carica_Controlli



    '#####################################################################
    Public Shared Sub Carica_CBL_Regioni(
                                ByRef CBL As CheckBoxList,
                                ByVal Lista_Regione_Cod As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer

        'Pulizia
        CBL.Items.Clear()
        Dim objRegioni As New AgronicaCoreSementieriDAL.CaricaVarie_x_Sementi_R
        'Recupero i dati
        DT = objRegioni.Regioni_Leggi(Lista_Regione_Cod, objParametri)
        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1
            CBL.Items.Add(New ListItem(DT.Rows(i).Item("Regione_Des"),
                                       DT.Rows(i).Item("Regione_Cod")))
        Next
        'Garbage Collection
        DT.Dispose()

    End Sub



    '#####################################################################
    Public Shared Sub Carica_CBL_Provincie(
                                ByRef CBL As CheckBoxList,
                                ByVal Lista_Regione_Cod As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer
        Dim Testo As String

        'Pulizia
        CBL.Items.Clear()
        Dim objProv As New AgronicaCoreSementieriDAL.CaricaVarie_x_Sementi_R
        DT = objProv.Provincie_Leggi(Lista_Regione_Cod, "", "", objParametri)
        'Recupero i dati
        'DT = AD_Provincie_Leggi(Lista_Regione_Cod, "", "", objServer, objSession, objPage)

        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1

            Testo = "{" & DT.Rows(i).Item("Regione_Sigla") & "} {" & DT.Rows(i).Item("Provincia_Sigla") & "} " & DT.Rows(i).Item("Provincia_Des")

            CBL.Items.Add(New ListItem(Testo,
                                       DT.Rows(i).Item("Provincia_Cod")))
        Next

        'Garbage Collection
        DT.Dispose()

    End Sub



    '#####################################################################
    Public Shared Sub Carica_CBL_Comuni(
                                ByRef CBL As CheckBoxList,
                                ByVal Lista_Provincia_Cod As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer
        Dim Testo As String

        'Pulizia
        CBL.Items.Clear()
        Dim objProv As New AgronicaCoreSementieriDAL.CaricaVarie_x_Sementi_R
        DT = objProv.Comuni_Leggi(Lista_Provincia_Cod, "", "", "", objParametri)

        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1

            Testo = "{" & DT.Rows(i).Item("Provincia_Sigla") & "} " & DT.Rows(i).Item("Comune_Des")

            CBL.Items.Add(New ListItem(Testo,
                                       DT.Rows(i).Item("Provincia_Cod") & ":" & DT.Rows(i).Item("Comune_Cod")))
        Next

        'Garbage Collection
        DT.Dispose()

    End Sub





    '#####################################################################
    Public Shared Sub Carica_CBL_SpecieVegetali(
                                ByRef CBL As CheckBoxList,
                                ByVal Gru_Cod As Integer,
                                ByVal Veg_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer

        'Pulizia
        CBL.Items.Clear()

        'Recupero i dati
        Dim dtLeggiSpecieVegetali As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        DT = dtLeggiSpecieVegetali.Leggi(Veg_Cod, Gru_Cod, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        '        DT = AD_SpecieVegetali_Leggi(Gru_Cod, Veg_Cod, objServer, objSession, objPage)

        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1
            CBL.Items.Add(New ListItem(DT.Rows(i).Item("Veg_Des"),
                                       DT.Rows(i).Item("Veg_Cod")))
        Next

        'Garbage Collection
        DT.Dispose()

    End Sub

    Public Shared Sub Carica_RBL_Sementi(ByRef RBL As RadioButtonList, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        RBL.Items.Clear()

        Dim leggiSementi As New AgronicaCoreSementieriDAL.ClassiDiSpecie_R
        Dim DT As DataTable = leggiSementi.distinct_sementieri_classidispecievegetali_des(objParametri)

        For Each drow As DataRow In DT.Rows
            RBL.Items.Add(New ListItem(drow("sementieri_classidispecievegetali_des"), drow("id_specie")))
        Next

    End Sub

    '#####################################################################
    Public Shared Sub Carica_CBL_SpecieVegetali_Light(ByRef CBL As CheckBoxList, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        CBL.Items.Clear()

        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim DT As DataTable = objSpecie.Leggi_x_agronicaSementi(objParametri)

        'Carico il controllo
        For Each drow In DT.Rows
            CBL.Items.Add(New ListItem(drow.Item("Veg_Des"), drow.Item("Veg_Cod")))
        Next

        'Garbage Collection
        DT.Dispose()

    End Sub



    '#####################################################################
    Public Shared Sub Carica_CBL_TipologieVarietali(
                                ByRef CBL As CheckBoxList,
                                ByVal Veg_Cod As Integer,
                                ByVal Grva_Cod As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer

        'Pulizia
        CBL.Items.Clear()
        Dim objListe As New AgronicaCoreSementieriDAL.CaricaVarie_x_Sementi_R
        DT = objListe.TipologiaVarietale_Leggi2(Veg_Cod, Grva_Cod, objParametri)
        'Recupero i dati
        '        DT = AD_TipologiaVarietale_Leggi(Veg_Cod, Grva_Cod, objServer, objSession, objPage)

        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1

            CBL.Items.Add(New ListItem(DT.Rows(i).Item("Grva_Des"),
                                       DT.Rows(i).Item("Grva_Cod")))

            CBL.Items.Add(New ListItem(DT.Rows(i).Item("Grva_Des") & " --- Hybrid ",
                                       "-" & DT.Rows(i).Item("Grva_Cod")))

        Next

        'Garbage Collection
        DT.Dispose()

    End Sub




    '#####################################################################
    Public Shared Sub Carica_CBL_PadriGerarchia(
                                ByRef CBL As CheckBoxList,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT As DataTable
        Dim i As Integer

        'Pulizia
        CBL.Items.Clear()
        Dim objImprese As New AgronicaCoreSementieriDAL.CaricaVarie_x_Sementi_R
        DT = objImprese.Leggi_Gerarchia_x_sementieri(True, True, objParametri)

        'Recupero i dati
        'DT = AD_Gerarchia_Leggi(True, True, objServer, objSession, objPage)

        'Carico il controllo
        For i = 0 To DT.Rows.Count - 1

            CBL.Items.Add(New ListItem(DT.Rows(i).Item("Padre_RagSoc"),
                                       DT.Rows(i).Item("Padre_Piva")))

        Next

        'Garbage Collection
        DT.Dispose()

    End Sub





    '#####################################################################
    '#####################################################################
    '#####################################################################
    '#####################################################################
    '#####################################################################




    '####################################################################################
    Public Shared Sub Carica_Combo_Dimensioni(ByRef CMB As DropDownList)

        CMB.Items.Clear()
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("200   [m] ", "200"))
        CMB.Items.Add(New ListItem("300   [m] ", "300"))
        CMB.Items.Add(New ListItem("400   [m] ", "400"))
        CMB.Items.Add(New ListItem("500   [m] ", "500"))
        CMB.Items.Add(New ListItem("600   [m] ", "600"))
        CMB.Items.Add(New ListItem("700   [m] ", "700"))
        CMB.Items.Add(New ListItem("800   [m] ", "800"))
        CMB.Items.Add(New ListItem("900   [m] ", "900"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("  1,0 [km]", "1000"))
        CMB.Items.Add(New ListItem("  1,1 [km]", "1100"))
        CMB.Items.Add(New ListItem("  1,2 [km]", "1200"))
        CMB.Items.Add(New ListItem("  1,3 [km]", "1300"))
        CMB.Items.Add(New ListItem("  1,4 [km]", "1400"))
        CMB.Items.Add(New ListItem("  1,5 [km]", "1500"))
        CMB.Items.Add(New ListItem("  1,6 [km]", "1600"))
        CMB.Items.Add(New ListItem("  1,7 [km]", "1700"))
        CMB.Items.Add(New ListItem("  1,8 [km]", "1800"))
        CMB.Items.Add(New ListItem("  1,9 [km]", "1900"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("  2   [km]", "2000"))
        CMB.Items.Add(New ListItem("  3   [km]", "3000"))
        CMB.Items.Add(New ListItem("  4   [km]", "4000"))
        CMB.Items.Add(New ListItem("  5   [km]", "5000"))
        CMB.Items.Add(New ListItem("  6   [km]", "6000"))
        CMB.Items.Add(New ListItem("  7   [km]", "7000"))
        CMB.Items.Add(New ListItem("  8   [km]", "8000"))
        CMB.Items.Add(New ListItem("  9   [km]", "9000"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem(" 10   [km]", "10000"))
        CMB.Items.Add(New ListItem(" 11   [km]", "11000"))
        CMB.Items.Add(New ListItem(" 12   [km]", "12000"))
        CMB.Items.Add(New ListItem(" 15   [km]", "15000"))
        CMB.Items.Add(New ListItem(" 20   [km]", "20000"))
        CMB.Items.Add(New ListItem(" 25   [km]", "25000"))
        CMB.Items.Add(New ListItem(" 30   [km]", "30000"))
        CMB.Items.Add(New ListItem(" 40   [km]", "40000"))
        CMB.Items.Add(New ListItem(" 50   [km]", "50000"))

    End Sub



    '####################################################################################
    Public Shared Sub Carica_Combo_ImpostaZona(ByRef CMB As DropDownList)


        CMB.Items.Clear()
        CMB.Items.Add(New ListItem(" ", "#"))

        CMB.Items.Add(New ListItem("(BO) BOLOGNA", "686366|4930261|20000"))
        CMB.Items.Add(New ListItem("(BO)  Imola", "716729|4914447|20000"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("(FC) CESENA", "759612|4892279|20000"))
        CMB.Items.Add(New ListItem("(FC) FORLI'", "742871|4901202|20000"))
        CMB.Items.Add(New ListItem("(FC)  Gambettola", "767608|4890409|20000"))
        CMB.Items.Add(New ListItem("(FC)  Savignano SR", "772066|4887602|20000"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("(FE) FERRARA", "707319|4968343|20000"))
        CMB.Items.Add(New ListItem("(FE)  Argenta", "725438|4943980|20000"))
        CMB.Items.Add(New ListItem("(FE)  Comacchio", "752341|4954124|20000"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("(RA) RAVENNA", "754991|4923225|20000"))
        CMB.Items.Add(New ListItem("(RA)  Alfonsine", "741983|4932569|20000"))
        CMB.Items.Add(New ListItem("(RA)  Faenza", "730397|4907810|20000"))
        CMB.Items.Add(New ListItem("(RA)  Lugo", "732127|4922777|20000"))
        CMB.Items.Add(New ListItem("(RA)  Russi", "741701|4917725|20000"))
        CMB.Items.Add(New ListItem(" ", "#"))
        CMB.Items.Add(New ListItem("(RN) RIMINI", "785569|4884622|10000"))
        CMB.Items.Add(New ListItem("(RN)  Santarcangelo", "776366|4884697|20000"))

    End Sub








End Class
