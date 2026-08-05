Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi

Public Class STD_TrasformatiVegetali

    Public Function LeggiTrasformatiVegetali_Qdc(impianti As Impianto(),
                                             specie As utilizzi.Specie,
                                             objParametri_Super_Server As AgronicaCoreParametri,
                                             objParametri_Server As AgronicaCoreParametri,
                                             objParametri_Utenti As AgronicaCoreParametri
                                             ) As List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta)

        Dim trasformatiVegetaliList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta)

        Dim Piva = STD_Utility.getPivaDaImpianti(impianti)

        Dim Veg_Cod = specie.codice


        Dim DtRisultati As DataTable
        Dim Mat_Cod, Mat_Des,
            Cod_Articolo As String

        Dim Dr As DataRow
        Dim DtTrasformatiVegetaliOrdinati = New DataTable
        DtTrasformatiVegetaliOrdinati.Columns.Add(New DataColumn("Testo", GetType(String)))
        DtTrasformatiVegetaliOrdinati.Columns.Add(New DataColumn("DettaglioTrasformatoVegetale", GetType(AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta)))
        DtTrasformatiVegetaliOrdinati.Columns.Add(New DataColumn("Mat_Des", GetType(String)))


        'DT: sa_cod ininfluente, le materie prime non sono usate per centro aziendale
        Dim objTrasformatiVegetali As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        DtRisultati = objTrasformatiVegetali.Leggi(Piva, Sa_Cod:=0,
                                                   TRASFORMATI_VEGETALI,
                                                   0, "",
                                                   Veg_Cod, 0,
                                                   0, 0,
                                                   0, 0, 0,
                                                   "", 0,
                                                   "",
                                                   False,
                                                   Flag_MateriePrimeSoloPrivate:=False,
                                                   "",
                                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                   "", "",
                                                   objParametri_Server)


        If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

            For i = 0 To DtRisultati.Rows.Count - 1

                Mat_Cod = DtRisultati.Rows(i).Item("mat_cod")
                Mat_Des = DtRisultati.Rows(i).Item("mat_des").ToString
                Cod_Articolo = DtRisultati.Rows(i).Item("Cod_Articolo").ToString

                Dim dettaglioTrasformatoVegetale = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta

                Dim rilevamentoMagazzino As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino

                Dr = DtTrasformatiVegetaliOrdinati.NewRow
                Dr.Item("Testo") = Mat_Des + " " + "(Cod.Articolo: " + Cod_Articolo + ")"
                Dr.Item("Mat_Des") = Mat_Des

                dettaglioTrasformatoVegetale.prodotto = New Prodotto(Mat_Cod, TRASFORMATI_VEGETALI) With {
                    .descrizione = Mat_Des
                }
                dettaglioTrasformatoVegetale.varieta = New utilizzi.Varieta With {
                    .codice = DtRisultati.Rows(i).Item("Cul_cod"),
                    .descrizione = DtRisultati.Rows(i).Item("Cul_Des").ToString,
                    .specie = New utilizzi.Specie
                }
                dettaglioTrasformatoVegetale.varieta.specie.codice = DtRisultati.Rows(i).Item("Veg_cod")
                dettaglioTrasformatoVegetale.varieta.specie.descrizione = DtRisultati.Rows(i).Item("Veg_Des").ToString
                dettaglioTrasformatoVegetale.codArticolo = DtRisultati.Rows(i).Item("Cod_Articolo").ToString
                dettaglioTrasformatoVegetale.regolamento = DtRisultati.Rows(i).Item("Regolamento")

                Dr.Item("DettaglioTrasformatoVegetale") = dettaglioTrasformatoVegetale

                DtTrasformatiVegetaliOrdinati.Rows.Add(Dr)
            Next
        End If


        If DtTrasformatiVegetaliOrdinati IsNot Nothing Then

            'uso il dataview per ordinare 
            Dim Dv As New DataView()
            DtTrasformatiVegetaliOrdinati.TableName = "TrasformatiVegetali"
            Dv.Table = DtTrasformatiVegetaliOrdinati
            Dv.Sort = "Mat_Des ASC"

            For i = 0 To Dv.Count - 1
                Dim traformatoVegetale As AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta = Dv(i).Item("DettaglioTrasformatoVegetale")
                trasformatiVegetaliList.Add(traformatoVegetale)
            Next

        End If

        Return trasformatiVegetaliList

    End Function

    Public Function LeggiTrasformatiVegetali_Anagrafica(impresa As Impresa,
                                                        specie As utilizzi.Specie,
                                                        varieta As utilizzi.Varieta,
                                                        regolamento As Regolamenti,
                                                        finalita As GruppoFinalita,
                                                        objParametri_Super_Server As AgronicaCoreParametri,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                        objParametri_Utenti As AgronicaCoreParametri) As List(Of Prodotto)

        Dim trasformatiVegetaliList As New List(Of Prodotto)

        Dim Piva = impresa.partitaIva

        Dim Veg_Cod = 0
        If (specie IsNot Nothing AndAlso specie.codice > 0) Then
            Veg_Cod = specie.codice
        End If

        Dim Cul_Cod = 0
        If (varieta IsNot Nothing AndAlso varieta.codice > 0) Then
            Cul_Cod = varieta.codice
        End If

        Dim Grfi_Cod = 0
        If (finalita IsNot Nothing AndAlso finalita.codice > 0) Then
            Grfi_Cod = finalita.codice
        End If

        Dim DtRisultati As DataTable
        Dim Mat_Cod, Mat_Des,
            Cod_Articolo As String


        Dim xFiltroAggiuntivo As String = ""
        If Grfi_Cod > 0 Then
            xFiltroAggiuntivo = " Materie_Prime.Grfi_Cod = " & Grfi_Cod & " "
        End If

        'DT: sa_cod ininfluente, le materie prime non sono usate per centro aziendale
        Dim objTrasformatiVegetali As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        DtRisultati = objTrasformatiVegetali.Leggi(Piva, Sa_Cod:=0,
                                                   TRASFORMATI_VEGETALI,
                                                   0, "",
                                                   Veg_Cod, Cul_Cod,
                                                   0, 0,
                                                   0, 0, 0,
                                                   "", 0,
                                                   "",
                                                   False,
                                                   Flag_MateriePrimeSoloPrivate:=False,
                                                   "",
                                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                   xFiltroAggiuntivo, "",
                                                   objParametri_Server)


        If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

            For i = 0 To DtRisultati.Rows.Count - 1

                Mat_Cod = DtRisultati.Rows(i).Item("mat_cod")
                Mat_Des = DtRisultati.Rows(i).Item("mat_des").ToString
                Cod_Articolo = DtRisultati.Rows(i).Item("Cod_Articolo").ToString

                Dim testo = Mat_Des + " " + "(Cod.Articolo: " + Cod_Articolo + ")"

                Dim dettaglioTrasformatoVegetale = New Prodotto(Mat_Cod, TRASFORMATI_VEGETALI) With {
                    .descrizione = Mat_Des,
                    .varieta = New utilizzi.Varieta With {
                        .codice = DtRisultati.Rows(i).Item("Cul_cod"),
                        .descrizione = DtRisultati.Rows(i).Item("Cul_Des").ToString,
                        .specie = New utilizzi.Specie With {
                            .codice = DtRisultati.Rows(i).Item("Veg_cod"),
                            .descrizione = DtRisultati.Rows(i).Item("Veg_Des").ToString
                        }
                    },
                    .finalita = New utilizzi.GruppoFinalita With {
                        .codice = DtRisultati.Rows(i).Item("Grfi_Cod"),
                        .descrizione = DtRisultati.Rows(i).Item("Grfi_Des").ToString
                    },
                    .codice_alfanumerico = DtRisultati.Rows(i).Item("Cod_Articolo").ToString,
                    .regolamento = New Regolamenti(DtRisultati.Rows(i).Item("Regolamento"))
                }

                trasformatiVegetaliList.Add(dettaglioTrasformatoVegetale)

            Next
        End If

        Return trasformatiVegetaliList

    End Function
End Class
