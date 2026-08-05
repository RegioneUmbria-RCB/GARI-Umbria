Imports System.Text
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PDC_Dettagli

    Sub New()
        CampioniAssociati = New List(Of PDC_Campioni)
        ProdottiUtilizzati = New List(Of ProdottiQuantita)
        LFO = New PDC_LFO
        Impianto = New Impianto
    End Sub

    Public Property Bool As Boolean
    Public Property InfoVarie As String
    Public Property LFO_Proposta_1 As String
    Public Property LFO_Proposta_2 As String
    Public Property LFO_Proposta_3 As String
    Public Property PivaSuperUser As String
    Public Property Id_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
    Public Property PDC_Dettagli_Des As String
    Public Property LFO As PDC_LFO
    Public Property Flag_PDC As Integer
    Public Property CampioniAssociati As List(Of PDC_Campioni)
    Public Property PrincipiAttivi As Hashtable
    Public Property ProdottiUtilizzati As List(Of ProdottiQuantita)
    Public Property Impianto As Impianto

    Public Property Piva_OP As String
    Public Property Rag_Soc_OP As String
    Public Property Grower_Number As String
    Public Property Kpin As String
    Public Property Block As String
    Public Property Ind_Des As String
    Public Property Frz_Des As String
    Public Property CAP As String
    Public Property Com_Des As String
    Public Property Pro_Cod As String
    Public Property Reg As String
    Public Property Regione As String
    Public Property Stato As String
    Public Property Pro_Cod_Istat As String
    Public Property Com_Cod_Istat As String
    Public Property Tecnico_Campionamento As String
    Public Property Tecnico_Campionamento_Tel As String
    Public Property Progetto_Nome As String
    Public Property App_Lat As Double
    Public Property App_Long As Double

    'Colonne ZOO
    Public Property Cod_Animale As Integer
    Public Property Cod_Progetto As Integer
    Public Property GEN_COD As Integer
    Public Property Genere As String
    Public Property SPE_COD As Integer
    Public Property Specie As String
    Public Property RAZ_COD As Integer
    Public Property Razza As String
    Public Property Data_Nascita As Date
    Public Property Giorni_Vita As Integer
    Public Property Sesso As String
    Public Property Matricola As String
    Public Property Lotto As String
    Public Property Raggruppamento_Cod As Integer
    Public Property Raggruppamento As String
    Public Property Sta_Num As Integer
    Public Property Sta_Des As String

End Class

'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################
Public Class PDC_Dettagli_Helper

#Region "Funzioni per il DATATABLE"

    Public Shared Function GetDataTableAll(ByVal Lista_Oggetti As List(Of PDC_Dettagli)) As DataTable

        Dim dt As New DataTable
        GeneraStrutturaDT(dt)

        Dim dr As DataRow
        For i As Integer = 0 To Lista_Oggetti.Count - 1
            dr = dt.NewRow
            dr.Item("ID_PDC_Testata") = Lista_Oggetti(i).Id_PDC_Testata
            dr.Item("ID_PDC_Dettagli") = Lista_Oggetti(i).ID_PDC_Dettagli
            dr.Item("Codice_Cliente") = Lista_Oggetti(i).Impianto.Codice_Cliente
            dr.Item("Certificato") = Lista_Oggetti(i).Impianto.Certificato

            dr.Item("Codice_Cliente_2") = Lista_Oggetti(i).Impianto.Codice_Cliente_2
            dr.Item("Data_Inizio_Impianto") = Lista_Oggetti(i).Impianto.Data_Inizio_Impianto
            dr.Item("Sigla") = Lista_Oggetti(i).Impianto.Sigla
            dr.Item("Magazzino_di_conferimento") = Lista_Oggetti(i).Impianto.Magazzino_di_conferimento
            dr.Item("Dettaglio_Specie_Personalizzato") = Lista_Oggetti(i).Impianto.Dettaglio_Specie_Personalizzato

            dr.Item("Tecnico_di_riferimento") = Lista_Oggetti(i).Impianto.Tecnico_di_riferimento

            dr.Item("Capitolato_Privato") = Lista_Oggetti(i).Impianto.Capitolato_Privato
            dr.Item("Pdc_Dettagli_Des") = Lista_Oggetti(i).PDC_Dettagli_Des
            dr.Item("Piva") = Lista_Oggetti(i).Impianto.Piva
            dr.Item("Sa_Cod") = Lista_Oggetti(i).Impianto.Sa_Cod
            dr.Item("Appezza") = Lista_Oggetti(i).Impianto.Appezza
            dr.Item("Id_Reg") = Lista_Oggetti(i).Impianto.Id_Reg

            dr.Item("NomeArticolo") = Lista_Oggetti(i).Impianto.NomeArticolo
            dr.Item("LottoFornitore") = Lista_Oggetti(i).Impianto.LottoFornitore
            dr.Item("Note_Impianto") = Lista_Oggetti(i).Impianto.Note_Impianto
            dr.Item("Note2") = Lista_Oggetti(i).Impianto.note2

            dr.Item("Tipologia_Varietale") = Lista_Oggetti(i).Impianto.Tipologia_Varietale

            dr.Item("Descrizione_Disciplinare") = Lista_Oggetti(i).Impianto.Descrizione_Disciplinare
            dr.Item("Tipo_Lotta_Acquisti") = Lista_Oggetti(i).Impianto.Tipo_Lotta_Acquisti
            dr.Item("Tipo_Lotta_Acquisti_des") = Lista_Oggetti(i).Impianto.Tipo_Lotta_Acquisti_des
            dr.Item("Descrizione_PuntoDiPrelievo") = Lista_Oggetti(i).Impianto.Descrizione_PuntoDiPrelievo

            dr.Item("Rag_Soc") = Lista_Oggetti(i).Impianto.Rag_Soc
            dr.Item("Sa_Nome") = Lista_Oggetti(i).Impianto.Sa_Nome
            dr.Item("App_Nome") = Lista_Oggetti(i).Impianto.App_Nome
            dr.Item("Veg_Des") = Lista_Oggetti(i).Impianto.Veg_Des
            dr.Item("Cul_Des") = Lista_Oggetti(i).Impianto.Cul_Des
            dr.Item("Sup_Imp") = Lista_Oggetti(i).Impianto.Sup_Imp

            dr.Item("Veg_Cod") = Lista_Oggetti(i).Impianto.Veg_Cod
            dr.Item("Cul_Cod") = Lista_Oggetti(i).Impianto.Cul_Cod

            If Not IsNothing(Lista_Oggetti(i).LFO) Then
                dr.Item("Id_LFO") = Lista_Oggetti(i).LFO.ID_LFO
                dr.Item("LFO_Des") = Lista_Oggetti(i).LFO.LFO_Des
            Else
                dr.Item("Id_LFO") = 0
                dr.Item("LFO_Des") = ""
            End If

            dr.Item("Flag_PDC") = Lista_Oggetti(i).Flag_PDC

            dr.Item("Data_Fornitura") = Lista_Oggetti(i).Impianto.Data_Fornitura
            Dim str_data_app As String = Lista_Oggetti(i).Impianto.Data_Fornitura.ToShortDateString()
            If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                dr.Item("Data_Fornitura_STR") = ""
            Else
                If IsDBNull(str_data_app) Then
                    dr.Item("Data_Fornitura_STR") = ""
                Else
                    dr.Item("Data_Fornitura_STR") = str_data_app
                End If
            End If

            dr.Item("Data_Raccolta") = Lista_Oggetti(i).Impianto.Data_Raccolta
            str_data_app = Lista_Oggetti(i).Impianto.Data_Raccolta.ToShortDateString()
            If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                dr.Item("Data_Raccolta_STR") = ""
            Else
                If IsDBNull(str_data_app) Then
                    dr.Item("Data_Raccolta_STR") = ""
                Else
                    dr.Item("Data_Raccolta_STR") = str_data_app
                End If
            End If

            dr.Item("Data_Semina") = Lista_Oggetti(i).Impianto.Data_Semina
            str_data_app = Lista_Oggetti(i).Impianto.Data_Semina.ToShortDateString()
            If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                dr.Item("Data_Semina_STR") = ""
            Else
                If IsDBNull(str_data_app) Then
                    dr.Item("Data_Semina_STR") = ""
                Else
                    dr.Item("Data_Semina_STR") = str_data_app
                End If
            End If

            'Colore
            If Lista_Oggetti(i).CampioniAssociati.Count > 0 Then
                dr.Item("Colore") = 1
                If Lista_Oggetti(i).CampioniAssociati(0).AnalisiAssociate.Count > 0 Then
                    Dim analizzate As Integer = 0
                    Dim da_inviare As Integer = 0
                    Dim finalizzate As Integer = 0
                    Dim num_analisi As Integer = 0
                    For Each campione In Lista_Oggetti(i).CampioniAssociati
                        num_analisi += campione.AnalisiAssociate.Count
                        For Each analisi In campione.AnalisiAssociate
                            If analisi.PDC_Stato_Analisi = enum_PDC_Stato_Analisi.Analizzata Then
                                analizzate += 1
                                If analisi.PDC_Stato_Validazione = 1 Then
                                    finalizzate += 1
                                End If
                            ElseIf analisi.PDC_Stato_Analisi = enum_PDC_Stato_Analisi.Da_Inviare Then
                                da_inviare += 1
                            End If
                        Next
                    Next
                    If finalizzate = num_analisi Then
                        dr.Item("Colore") = 5
                    ElseIf analizzate = num_analisi Then
                        dr.Item("Colore") = 3
                    ElseIf da_inviare > 0 Then
                        dr.Item("Colore") = 4
                    Else
                        dr.Item("Colore") = 2
                    End If
                End If
            End If

            dr.Item("Regolamento") = Lista_Oggetti(i).Impianto.Regolamento

            dr.Item("note2") = Lista_Oggetti(i).Impianto.note2

            dr.Item("CapitolatiEsclusi") = Lista_Oggetti(i).Impianto.CapitolatiEsclusi

            ' nuovi campi PDC Dettagli
            dr.Item("Piva_OP") = Lista_Oggetti(i).Piva_OP
            dr.Item("Rag_Soc_OP") = Lista_Oggetti(i).Rag_Soc_OP
            dr.Item("Grower_Number") = Lista_Oggetti(i).Grower_Number
            dr.Item("Kpin") = Lista_Oggetti(i).Kpin
            dr.Item("Block") = Lista_Oggetti(i).Block
            dr.Item("Ind_Des") = Lista_Oggetti(i).Ind_Des
            dr.Item("Frz_Des") = Lista_Oggetti(i).Frz_Des
            dr.Item("CAP") = Lista_Oggetti(i).CAP
            dr.Item("Com_Des") = Lista_Oggetti(i).Com_Des
            dr.Item("Pro_Cod") = Lista_Oggetti(i).Pro_Cod
            dr.Item("Reg") = Lista_Oggetti(i).Reg
            dr.Item("Regione") = Lista_Oggetti(i).Regione
            dr.Item("Stato") = Lista_Oggetti(i).Stato
            dr.Item("Pro_Cod_Istat") = Lista_Oggetti(i).Pro_Cod_Istat
            dr.Item("Com_Cod_Istat") = Lista_Oggetti(i).Com_Cod_Istat
            dr.Item("Tecnico_Campionamento") = Lista_Oggetti(i).Tecnico_Campionamento
            dr.Item("Tecnico_Campionamento_Tel") = Lista_Oggetti(i).Tecnico_Campionamento_Tel
            dr.Item("Progetto_Nome") = Lista_Oggetti(i).Progetto_Nome
            dr.Item("App_Lat") = Lista_Oggetti(i).App_Lat
            dr.Item("App_Long") = Lista_Oggetti(i).App_Long

            'Colonne ZOO
            dr.Item("Cod_Animale") = Lista_Oggetti(i).Cod_Animale
            dr.Item("Cod_Progetto") = Lista_Oggetti(i).Cod_Progetto
            dr.Item("GEN_COD") = Lista_Oggetti(i).GEN_COD
            dr.Item("Genere") = Lista_Oggetti(i).Genere
            dr.Item("SPE_COD") = Lista_Oggetti(i).SPE_COD
            dr.Item("Specie") = Lista_Oggetti(i).Specie
            dr.Item("RAZ_COD") = Lista_Oggetti(i).RAZ_COD
            dr.Item("Razza") = Lista_Oggetti(i).Razza
            dr.Item("Data_Nascita") = Lista_Oggetti(i).Data_Nascita
            dr.Item("Giorni_Vita") = Lista_Oggetti(i).Giorni_Vita
            dr.Item("Sesso") = Lista_Oggetti(i).Sesso
            dr.Item("Matricola") = Lista_Oggetti(i).Matricola
            dr.Item("Lotto") = Lista_Oggetti(i).Lotto
            dr.Item("Raggruppamento_Cod") = Lista_Oggetti(i).Raggruppamento_Cod
            dr.Item("Raggruppamento") = Lista_Oggetti(i).Raggruppamento
            dr.Item("Sta_Num") = Lista_Oggetti(i).Sta_Num
            dr.Item("Sta_Des") = Lista_Oggetti(i).Sta_Des

            dt.Rows.Add(dr)
        Next

        Return dt

    End Function

    Public Shared Function GetDataTableLFO(ByVal Lista_Oggetti As List(Of PDC_Dettagli), ByRef HashAz As Hashtable, ByRef HashPa_Tot As Hashtable) As DataTable

        Dim DT As New DataTable
        GeneraStrutturaDT(DT)
        AggiungiColonnexLFO(DT)


        'Dim HashAz As New Hashtable
        'Dim HashPa_Tot As New Hashtable

        Dim KeyAz As String

        Dim i, j As Integer
        Dim dr As DataRow
        For i = 0 To Lista_Oggetti.Count - 1
            dr = DT.NewRow

            KeyAz = Lista_Oggetti(i).Impianto.Piva & "|" &
                Lista_Oggetti(i).Impianto.Sa_Cod & "|" &
                Lista_Oggetti(i).Impianto.Appezza & "|" &
                Lista_Oggetti(i).Impianto.Id_Reg

            dr.Item("ID_PDC_Dettagli") = Lista_Oggetti(i).ID_PDC_Dettagli
            dr.Item("Codice_Cliente") = Lista_Oggetti(i).Impianto.Codice_Cliente
            dr.Item("Certificato") = Lista_Oggetti(i).Impianto.Certificato
            dr.Item("Note_Impianto") = Lista_Oggetti(i).Impianto.Note_Impianto
            dr.Item("Note2") = Lista_Oggetti(i).Impianto.note2

            dr.Item("Capitolato_Privato") = Lista_Oggetti(i).Impianto.Capitolato_Privato
            dr.Item("Regolamento") = Lista_Oggetti(i).Impianto.Regolamento

            dr.Item("Piva") = Lista_Oggetti(i).Impianto.Piva
            dr.Item("Sa_Cod") = Lista_Oggetti(i).Impianto.Sa_Cod
            dr.Item("Appezza") = Lista_Oggetti(i).Impianto.Appezza
            dr.Item("Id_Reg") = Lista_Oggetti(i).Impianto.Id_Reg

            dr.Item("Rag_Soc") = Lista_Oggetti(i).Impianto.Rag_Soc
            dr.Item("Sa_Nome") = Lista_Oggetti(i).Impianto.Sa_Nome
            dr.Item("App_Nome") = Lista_Oggetti(i).Impianto.App_Nome
            dr.Item("Veg_Des") = Lista_Oggetti(i).Impianto.Veg_Des
            dr.Item("Cul_Des") = Lista_Oggetti(i).Impianto.Cul_Des
            dr.Item("Sup_Imp") = Lista_Oggetti(i).Impianto.Sup_Imp

            dr.Item("Veg_Cod") = Lista_Oggetti(i).Impianto.Veg_Cod
            dr.Item("Cul_Cod") = Lista_Oggetti(i).Impianto.Cul_Cod

            dr.Item("Progetto_Nome") = Lista_Oggetti(i).Progetto_Nome

            dr.Item("App_Lat") = Lista_Oggetti(i).App_Lat
            dr.Item("App_Long") = Lista_Oggetti(i).App_Long

            dr.Item("LFO_Proposta_1") = Lista_Oggetti(i).LFO_Proposta_1
            dr.Item("LFO_Proposta_2") = Lista_Oggetti(i).LFO_Proposta_2
            dr.Item("LFO_Proposta_3") = Lista_Oggetti(i).LFO_Proposta_3

            If Not IsNothing(Lista_Oggetti(i).LFO) Then
                dr.Item("Id_LFO") = Lista_Oggetti(i).LFO.ID_LFO
                dr.Item("LFO_Des") = Lista_Oggetti(i).LFO.LFO_Des
            Else
                dr.Item("Id_LFO") = 0
                dr.Item("LFO_Des") = ""
            End If


            Dim desc As String
            desc = Lista_Oggetti(i).Impianto.Rag_Soc & " " & Lista_Oggetti(i).Impianto.Sa_Nome & " " & Lista_Oggetti(i).Impianto.App_Nome
            desc = desc.Replace("'", " ")
            'info trattamenti
            Dim strTrattamenti As New StringBuilder
            Dim HashPA As New Hashtable
            Dim ListaOrdinataPa As New List(Of Integer)

            If Lista_Oggetti(i).ProdottiUtilizzati.Count > 0 Then
                strTrattamenti.Append("<div id='btn_" & i & "' class='ui-state-default ui-corner-all ui-divquadrato piumeno' style='float:left;' >")
                strTrattamenti.Append("     <span class='ui-icon ui-icon-carat-1-se'></span></div>")

                'con + e meno
                strTrattamenti.Append("<div class='espandi' title='" & desc & "' id='esp_" & i & "'><p>")

                For j = 0 To Lista_Oggetti(i).ProdottiUtilizzati.Count - 1
                    strTrattamenti.Append("<b>" & Lista_Oggetti(i).ProdottiUtilizzati(j).FR_Des & "</b>")
                    strTrattamenti.Append(" data:" & Lista_Oggetti(i).ProdottiUtilizzati(j).Data)

                    Dim k As Integer
                    For k = 0 To Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA.Count - 1
                        strTrattamenti.Append(" <br>")
                        strTrattamenti.Append(" PA:<i>" & Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Des & "</i>")
                        strTrattamenti.Append(" titolo:" & Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Titolo)


                        If HashPA.ContainsKey(Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Cod) = False Then
                            'se non lo contengo
                            Dim ListaDate As New List(Of Date)
                            ListaDate.Add(Lista_Oggetti(i).ProdottiUtilizzati(j).Data)
                            HashPA.Add(Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Cod, ListaDate)
                            ListaOrdinataPa.Add(Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Cod)
                        Else

                            If CType(HashPA.Item(Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Cod), List(Of Date)).Find(
                                                                                                                   Function(c) c = Lista_Oggetti(i).ProdottiUtilizzati(j).Data) <> Lista_Oggetti(i).ProdottiUtilizzati(j).Data Then
                                'Function f () AddressOf CercaListaData)  .Equals(Lista_Oggetti(i).ProdottiUtilizzati(j).Data)) = False Then
                                CType(HashPA.Item(Lista_Oggetti(i).ProdottiUtilizzati(j).ListaPA(k).Pa_Cod), List(Of Date)).Add(
                                    Lista_Oggetti(i).ProdottiUtilizzati(j).Data)
                            End If
                        End If
                    Next
                    strTrattamenti.Append(" <br>")
                Next
                strTrattamenti.Append("</p></div>")

                Dim KeyPA As String = ""
                Dim jj As Integer
                ListaOrdinataPa.Sort()
                For jj = 0 To ListaOrdinataPa.Count - 1
                    If KeyPA.Length > 0 Then
                        KeyPA += "$" & ListaOrdinataPa(jj)
                    Else
                        KeyPA = ListaOrdinataPa(jj)
                    End If
                Next

                If HashPa_Tot.Contains(KeyPA) = True Then
                    CType(HashPa_Tot.Item(KeyPA), List(Of String)).Add(KeyAz)
                    'HashPa_Tot.Item(KeyPA) = CType(HashPa_Tot.Item(KeyPA), List(Of Date)).Add(KeyAz)
                Else
                    Dim list As New List(Of String) From {KeyAz}
                    HashPa_Tot.Add(KeyPA, list)
                End If
            End If


            If strTrattamenti.Length > 0 Then
                dr.Item("InfoTrattamenti") = strTrattamenti.ToString
            End If

            'aggiungo alla HashAz
            HashAz.Add(KeyAz, HashPA)


            dr.Item("Flag_PDC") = Lista_Oggetti(i).Flag_PDC

            dr.Item("Data_Raccolta") = Lista_Oggetti(i).Impianto.Data_Raccolta


            Dim str_data_app As String = Lista_Oggetti(i).Impianto.Data_Raccolta.ToShortDateString()


            If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                dr.Item("Data_Raccolta_STR") = ""
            Else
                If IsDBNull(str_data_app) Then
                    dr.Item("Data_Raccolta_STR") = ""
                Else
                    dr.Item("Data_Raccolta_STR") = str_data_app
                End If
            End If

            dr.Item("Data_Semina") = Lista_Oggetti(i).Impianto.Data_Semina
            str_data_app = Lista_Oggetti(i).Impianto.Data_Semina.ToShortDateString()
            If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                dr.Item("Data_Semina_STR") = ""
            Else
                If IsDBNull(str_data_app) Then
                    dr.Item("Data_Semina_STR") = ""
                Else
                    dr.Item("Data_Semina_STR") = str_data_app
                End If
            End If

            DT.Rows.Add(dr)
        Next

        Return DT
    End Function



    Public Shared Function GetDataTablePDC(ByVal Lista_Oggetti As List(Of PDC_Dettagli), ByVal soloFlagPDC As Boolean) As DataTable

        Dim DT As New DataTable
        GeneraStrutturaDT(DT)
        AggiungiColonnexPDC(DT)
        AggiungiColonnexAnalisi(DT)

        'Dim DtKey(1) As DataColumn
        'DtKey(0) = DT.Columns("Analisi_Testata_Cod")
        'DT.PrimaryKey = DtKey

        Dim dr As DataRow

        For i As Integer = 0 To Lista_Oggetti.Count - 1

            If soloFlagPDC = False Or Lista_Oggetti(i).Flag_PDC = -1 Then
                dr = DT.NewRow
                dr.Item("Piva") = Lista_Oggetti(i).Impianto.Piva
                dr.Item("Sa_Cod") = Lista_Oggetti(i).Impianto.Sa_Cod
                dr.Item("Appezza") = Lista_Oggetti(i).Impianto.Appezza
                dr.Item("Id_Reg") = Lista_Oggetti(i).Impianto.Id_Reg
                dr.Item("Codice_Cliente") = Lista_Oggetti(i).Impianto.Codice_Cliente
                dr.Item("Certificato") = Lista_Oggetti(i).Impianto.Certificato
                dr.Item("Note_Impianto") = Lista_Oggetti(i).Impianto.Note_Impianto
                dr.Item("Note2") = Lista_Oggetti(i).Impianto.note2

                dr.Item("Capitolato_Privato") = Lista_Oggetti(i).Impianto.Capitolato_Privato
                dr.Item("Tipologia_Varietale") = Lista_Oggetti(i).Impianto.Tipologia_Varietale
                dr.Item("Descrizione_Disciplinare") = Lista_Oggetti(i).Impianto.Descrizione_Disciplinare
                dr.Item("Tipo_Lotta_Acquisti") = Lista_Oggetti(i).Impianto.Tipo_Lotta_Acquisti
                dr.Item("Descrizione_PuntoDiPrelievo") = Lista_Oggetti(i).Impianto.Descrizione_PuntoDiPrelievo
                dr.Item("Regolamento") = Lista_Oggetti(i).Impianto.Regolamento


                dr.Item("Rag_Soc") = Lista_Oggetti(i).Impianto.Rag_Soc
                dr.Item("Sa_Nome") = Lista_Oggetti(i).Impianto.Sa_Nome
                dr.Item("App_Nome") = Lista_Oggetti(i).Impianto.App_Nome
                dr.Item("Veg_Des") = Lista_Oggetti(i).Impianto.Veg_Des
                dr.Item("Cul_Des") = Lista_Oggetti(i).Impianto.Cul_Des
                dr.Item("Sup_Imp") = Lista_Oggetti(i).Impianto.Sup_Imp

                dr.Item("Veg_Cod") = Lista_Oggetti(i).Impianto.Veg_Cod
                dr.Item("Cul_Cod") = Lista_Oggetti(i).Impianto.Cul_Cod

                If Not IsNothing(Lista_Oggetti(i).LFO) Then
                    dr.Item("Id_LFO") = Lista_Oggetti(i).LFO.ID_LFO
                    dr.Item("LFO_Des") = Lista_Oggetti(i).LFO.LFO_Des
                Else
                    dr.Item("Id_LFO") = 0
                    dr.Item("LFO_Des") = ""
                End If

                dr.Item("ID_PDC_Testata") = Lista_Oggetti(i).Id_PDC_Testata
                dr.Item("ID_PDC_Dettagli") = Lista_Oggetti(i).ID_PDC_Dettagli

                dr.Item("NomeArticolo") = Lista_Oggetti(i).Impianto.NomeArticolo
                dr.Item("Pdc_Dettagli_Des") = Lista_Oggetti(i).PDC_Dettagli_Des
                dr.Item("LottoFornitore") = Lista_Oggetti(i).Impianto.LottoFornitore

                dr.Item("Data_Fornitura") = Lista_Oggetti(i).Impianto.Data_Fornitura
                Dim str_data_app As String = Lista_Oggetti(i).Impianto.Data_Fornitura.ToShortDateString()
                If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                    dr.Item("Data_Fornitura_str") = ""
                Else
                    If IsDBNull(str_data_app) Then
                        dr.Item("Data_Semina_STR") = ""
                    Else
                        dr.Item("Data_Fornitura_str") = str_data_app
                    End If
                End If


                dr.Item("Flag_PDC") = Lista_Oggetti(i).Flag_PDC

                dr.Item("Data_Raccolta") = Lista_Oggetti(i).Impianto.Data_Raccolta
                str_data_app = Lista_Oggetti(i).Impianto.Data_Raccolta.ToShortDateString()
                If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                    dr.Item("Data_Raccolta_STR") = ""
                Else
                    If IsDBNull(str_data_app) Then
                        dr.Item("Data_Raccolta_STR") = ""
                    Else
                        dr.Item("Data_Raccolta_STR") = str_data_app
                    End If
                End If




                dr.Item("Data_Semina") = Lista_Oggetti(i).Impianto.Data_Semina

                str_data_app = Lista_Oggetti(i).Impianto.Data_Semina.ToShortDateString()
                If str_data_app = "31/12/2100" Or str_data_app = "01/01/1900" Then
                    dr.Item("Data_Semina_STR") = ""
                Else
                    If IsDBNull(str_data_app) Then
                        dr.Item("Data_Semina_STR") = ""
                    Else
                        dr.Item("Data_Semina_STR") = str_data_app
                    End If
                End If


                ' nuovi campi PDC Dettagli
                dr.Item("Piva_OP") = Lista_Oggetti(i).Piva_OP
                dr.Item("Rag_Soc_OP") = Lista_Oggetti(i).Rag_Soc_OP
                dr.Item("Grower_Number") = Lista_Oggetti(i).Grower_Number
                dr.Item("Kpin") = Lista_Oggetti(i).Kpin
                dr.Item("Block") = Lista_Oggetti(i).Block
                dr.Item("Ind_Des") = Lista_Oggetti(i).Ind_Des
                dr.Item("Frz_Des") = Lista_Oggetti(i).Frz_Des
                dr.Item("CAP") = Lista_Oggetti(i).CAP
                dr.Item("Com_Des") = Lista_Oggetti(i).Com_Des
                dr.Item("Pro_Cod") = Lista_Oggetti(i).Pro_Cod
                dr.Item("Reg") = Lista_Oggetti(i).Reg
                dr.Item("Regione") = Lista_Oggetti(i).Regione
                dr.Item("Stato") = Lista_Oggetti(i).Stato
                dr.Item("Pro_Cod_Istat") = Lista_Oggetti(i).Pro_Cod_Istat
                dr.Item("Com_Cod_Istat") = Lista_Oggetti(i).Com_Cod_Istat
                dr.Item("Tecnico_Campionamento") = Lista_Oggetti(i).Tecnico_Campionamento
                dr.Item("Tecnico_Campionamento_Tel") = Lista_Oggetti(i).Tecnico_Campionamento_Tel
                dr.Item("Progetto_Nome") = Lista_Oggetti(i).Progetto_Nome
                dr.Item("App_Lat") = Lista_Oggetti(i).App_Lat
                dr.Item("App_Long") = Lista_Oggetti(i).App_Long

                dr.Item("Bool") = Lista_Oggetti(i).Bool

                'Colonne ZOO
                dr.Item("Cod_Animale") = Lista_Oggetti(i).Cod_Animale
                dr.Item("Cod_Progetto") = Lista_Oggetti(i).Cod_Progetto
                dr.Item("GEN_COD") = Lista_Oggetti(i).GEN_COD
                dr.Item("Genere") = Lista_Oggetti(i).Genere
                dr.Item("SPE_COD") = Lista_Oggetti(i).SPE_COD
                dr.Item("Specie") = Lista_Oggetti(i).Specie
                dr.Item("RAZ_COD") = Lista_Oggetti(i).RAZ_COD
                dr.Item("Razza") = Lista_Oggetti(i).Razza
                dr.Item("Data_Nascita") = Lista_Oggetti(i).Data_Nascita
                dr.Item("Giorni_Vita") = Lista_Oggetti(i).Giorni_Vita
                dr.Item("Sesso") = Lista_Oggetti(i).Sesso
                dr.Item("Matricola") = Lista_Oggetti(i).Matricola
                dr.Item("Lotto") = Lista_Oggetti(i).Lotto
                dr.Item("Raggruppamento_Cod") = Lista_Oggetti(i).Raggruppamento_Cod
                dr.Item("Raggruppamento") = Lista_Oggetti(i).Raggruppamento

                'Carico i dati del campione
                If Not IsNothing(Lista_Oggetti(i).CampioniAssociati) AndAlso
                   Lista_Oggetti(i).CampioniAssociati.Count > 0 Then

                    Dim r_Campioni As DataRow

                    For iCampioni As Integer = 0 To Lista_Oggetti(i).CampioniAssociati.Count - 1
                        'campioni
                        r_Campioni = DT.NewRow
                        r_Campioni.ItemArray = dr.ItemArray

                        r_Campioni.Item("ID_PDC_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).ID_PDC_Campione
                        r_Campioni.Item("ID_PDC_Stato_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).ID_PDC_Stato_Campione
                        r_Campioni.Item("ID_PDC_Stato_Campione_Des") = AgronicaCorePianidiCampionamentoDAL.PDC_R.GetStatoCampione(
                                                                    Lista_Oggetti(i).CampioniAssociati(iCampioni).ID_PDC_Stato_Campione)
                        r_Campioni.Item("Codice_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Codice_Campione
                        r_Campioni.Item("Data_Campionamento") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Data_Campionamento.ToShortDateString
                        r_Campioni.Item("ID_PuntoDiPrelievo") = Lista_Oggetti(i).CampioniAssociati(iCampioni).ID_PuntoDiPrelievo
                        r_Campioni.Item("Descrizione_PuntoDiPrelievo") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Descrizione_PuntoDiPrelievo

                        r_Campioni.Item("Note_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Note_Campione
                        r_Campioni.Item("Tecnico_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Tecnico_Campione

                        r_Campioni.Item("Tipo_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Tipo_Campione
                        r_Campioni.Item("ID_Motivo_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).ID_Motivo_Campione
                        r_Campioni.Item("Motivo_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Motivo_Campione
                        r_Campioni.Item("Num_Prodotti") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Num_Prodotti

                        r_Campioni.Item("Lat_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Lat_Campione
                        r_Campioni.Item("Long_Campione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).Long_Campione

                        'controllo le analisi 
                        If Not IsNothing(Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate) AndAlso
                           Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate.Count > 0 Then

                            Dim r_Analisi As DataRow
                            For iAnalisi As Integer = 0 To Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate.Count - 1

                                r_Analisi = DT.NewRow
                                r_Analisi.ItemArray = r_Campioni.ItemArray

                                r_Analisi.Item("Analisi_Testata_cod") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Testata_Cod
                                r_Analisi.Item("Analisi_Testata_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Testata_Des

                                r_Analisi.Item("FornitoreFatturazione_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).FornitoreFatturazione_Des
                                r_Analisi.Item("Piva_FornitoreFatturazione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Piva_FornitoreFatturazione


                                r_Analisi.Item("PDC_Stato_Analisi") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).PDC_Stato_Analisi
                                r_Analisi.Item("PDC_Stato_Analisi_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).PDC_Stato_Analisi_Des
                                r_Analisi.Item("PDC_Stato_Validazione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).PDC_Stato_Validazione
                                r_Analisi.Item("PDC_Stato_Pubblicazione") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).PDC_Stato_Pubblicazione

                                r_Analisi.Item("Cod_Contatto") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Cod_Contatto
                                r_Analisi.Item("Cod_Risum") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Cod_Risum
                                r_Analisi.Item("Cod_Risum_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Cod_Risum_Des

                                r_Analisi.Item("Analisi_Tipologia_cod") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Tipologia_Cod
                                r_Analisi.Item("Analisi_Tipologia_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Tipologia_Des
                                r_Analisi.Item("Analisi_Tipologia_Tipo") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Tipologia_Tipo

                                r_Analisi.Item("Tipo_Campione_Cod") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Tipo_Campione_Cod
                                r_Analisi.Item("Tipo_Campione_Des") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Tipo_Campione_Des

                                r_Analisi.Item("Altre_Molecole") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Altre_Molecole
                                r_Analisi.Item("Altre_Molecole_Cod") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Altre_Molecole_Cod
                                r_Analisi.Item("Data_Richiesta_Analisi") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Data_Richiesta_Analisi.ToShortDateString
                                r_Analisi.Item("Note_Richiesta_Analisi") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Note_Richiesta_Analisi

                                If CInt(r_Analisi.Item("PDC_Stato_Analisi")) = enum_PDC_Stato_Analisi.Analisi_In_Corso Then
                                    r_Analisi.Item("Colore") = 2
                                ElseIf CInt(r_Analisi.Item("PDC_Stato_Analisi")) = enum_PDC_Stato_Analisi.Analizzata Then
                                    r_Analisi.Item("Colore") = 3
                                ElseIf CInt(r_Analisi.Item("PDC_Stato_Analisi")) = enum_PDC_Stato_Analisi.Da_Inviare Then
                                    r_Analisi.Item("Colore") = 4
                                ElseIf CInt(r_Analisi.Item("PDC_Stato_Analisi")) = enum_PDC_Stato_Analisi.In_Lavorazione Then
                                    r_Analisi.Item("Colore") = 2
                                End If

                                r_Analisi.Item("InfoVarie") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).InfoVarie

                                If IsDBNull(r_Analisi.Item("InfoVarie")) = True Then
                                    r_Analisi.Item("InfoVarie") = Lista_Oggetti(i).InfoVarie
                                Else
                                    If r_Analisi.Item("InfoVarie").trim() = "" Then
                                        r_Analisi.Item("InfoVarie") = Lista_Oggetti(i).InfoVarie
                                    End If
                                End If

                                r_Analisi.Item("ID_NC") = Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).ID_NC

                                r_Analisi.Item("Chiave_Lab") = dr.Item("ID_PDC_Testata") & "_" &
                                                               dr.Item("ID_PDC_Dettagli") & "_" &
                                                               r_Campioni.Item("ID_PDC_Campione") & "_" &
                                                               Lista_Oggetti(i).CampioniAssociati(iCampioni).AnalisiAssociate(iAnalisi).Analisi_Testata_Cod

                                DT.Rows.Add(r_Analisi)

                            Next

                        Else

                            r_Campioni.Item("Analisi_Testata_cod") = ""
                            r_Campioni.Item("Analisi_Testata_Des") = ""
                            r_Campioni.Item("FornitoreFatturazione_Des") = ""
                            r_Campioni.Item("Piva_FornitoreFatturazione") = ""

                            r_Campioni.Item("PDC_Stato_Analisi") = ""
                            r_Campioni.Item("PDC_Stato_Analisi_Des") = ""
                            r_Campioni.Item("Cod_Risum") = ""
                            r_Campioni.Item("Cod_Risum_Des") = ""
                            r_Campioni.Item("Analisi_Tipologia_cod") = ""
                            r_Campioni.Item("Analisi_Tipologia_Des") = ""
                            r_Campioni.Item("Analisi_Tipologia_Tipo") = ""

                            r_Campioni.Item("Tipo_Campione_Cod") = ""
                            r_Campioni.Item("Tipo_Campione_Des") = ""

                            r_Campioni.Item("Altre_Molecole") = ""
                            r_Campioni.Item("Altre_Molecole_Cod") = ""
                            r_Campioni.Item("Data_Richiesta_Analisi") = ""
                            r_Campioni.Item("Note_Richiesta_Analisi") = ""

                            'controllo se ci sono delle  info sull impianto
                            If Lista_Oggetti(i).InfoVarie <> "" Then
                                r_Campioni.Item("InfoVarie") = Lista_Oggetti(i).InfoVarie
                            Else
                                r_Campioni.Item("InfoVarie") = ""
                            End If

                            r_Campioni.Item("Chiave_Lab") = ""

                            r_Campioni.Item("Colore") = 1
                            DT.Rows.Add(r_Campioni)

                        End If

                    Next
                Else
                    dr.Item("Analisi_Testata_cod") = ""
                    dr.Item("Analisi_Testata_Des") = ""
                    dr.Item("FornitoreFatturazione_Des") = ""
                    dr.Item("Piva_FornitoreFatturazione") = ""

                    dr.Item("PDC_Stato_Analisi") = ""
                    dr.Item("PDC_Stato_Analisi_Des") = ""
                    dr.Item("Cod_Risum") = ""
                    dr.Item("Cod_Risum_Des") = ""
                    dr.Item("Analisi_Tipologia_cod") = ""
                    dr.Item("Analisi_Tipologia_Tipo") = ""
                    dr.Item("Tipo_Campione_Cod") = ""
                    dr.Item("Tipo_Campione_Des") = ""

                    dr.Item("Altre_Molecole") = ""
                    dr.Item("Altre_Molecole_Cod") = ""
                    dr.Item("Data_Richiesta_Analisi") = ""
                    dr.Item("Note_Richiesta_Analisi") = ""

                    dr.Item("Analisi_Tipologia_Des") = ""


                    'controllo se ci sono delle  info sull impianto
                    If Lista_Oggetti(i).InfoVarie <> "" Then
                        dr.Item("InfoVarie") = Lista_Oggetti(i).InfoVarie
                    Else
                        dr.Item("InfoVarie") = ""
                    End If
                    dr.Item("Chiave_Lab") = ""
                    dr.Item("Colore") = -1
                    dr.Item("ID_PDC_Stato_Campione") = 0
                    dr.Item("ID_PDC_Stato_Campione_Des") = ""
                    dr.Item("Codice_Campione") = ""
                    dr.Item("Data_Campionamento") = ""
                    dr.Item("ID_PuntoDiPrelievo") = ""
                    dr.Item("Descrizione_PuntoDiPrelievo") = ""
                    dr.Item("Note_Campione") = ""
                    dr.Item("Tecnico_Campione") = ""
                    dr.Item("Tipo_Campione") = ""
                    dr.Item("ID_Motivo_Campione") = ""
                    dr.Item("Motivo_Campione") = ""
                    dr.Item("Num_Prodotti") = ""
                    dr.Item("Lat_Campione") = ""
                    dr.Item("Long_Campione") = ""

                    DT.Rows.Add(dr)
                End If

            End If
        Next

        Return DT

    End Function

    Private Shared Sub AggiungiColonnexLFO(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("LFO_Proposta_1", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("LFO_Proposta_2", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("LFO_Proposta_3", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("InfoTrattamenti", Type.GetType("System.String")))

    End Sub

    Private Shared Sub AggiungiColonnexPDC(ByRef DT As DataTable)

        ''Dati Campione
        Dim ID_PDC_Stato_Campione As New DataColumn("ID_PDC_Stato_Campione") With {
            .DataType = Type.GetType("System.Int32")
        }
        DT.Columns.Add(ID_PDC_Stato_Campione)

        Dim Note_Campione As New DataColumn("Note_Campione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Note_Campione)

        Dim Tecnico_Campione As New DataColumn("Tecnico_Campione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Tecnico_Campione)

        Dim Codice_Campione As New DataColumn("Codice_Campione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Codice_Campione)

        Dim Data_Campionamento As New DataColumn("Data_Campionamento") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Data_Campionamento)

        'Dim Descrizione_PuntoDiPrelievo As New DataColumn("Descrizione_PuntoDiPrelievo")
        'Descrizione_PuntoDiPrelievo.DataType = Type.GetType("System.String")
        'DT.Columns.Add(Descrizione_PuntoDiPrelievo)

        Dim ID_PuntoDiPrelievo As New DataColumn("ID_PuntoDiPrelievo") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(ID_PuntoDiPrelievo)

        Dim PDC_Campione_Des As New DataColumn("PDC_Campione_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(PDC_Campione_Des)

        Dim ID_PDC_Stato_Campione_Des As New DataColumn("ID_PDC_Stato_Campione_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(ID_PDC_Stato_Campione_Des)

        Dim ID_PDC_Campione As New DataColumn("ID_PDC_Campione") With {
            .DataType = Type.GetType("System.Int32")
        }
        DT.Columns.Add(ID_PDC_Campione)

        DT.Columns.Add(New DataColumn("Tipo_Campione", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("ID_Motivo_Campione", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Motivo_Campione", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Num_Prodotti", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Lat_Campione", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Long_Campione", Type.GetType("System.String")))

    End Sub


    Private Shared Sub AggiungiColonnexAnalisi(ByRef DT As DataTable)

        ''Dati Analisi
        Dim Analisi_Testata_Cod As New DataColumn("Analisi_Testata_Cod") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Testata_Cod)

        Dim Analisi_Testata_Des As New DataColumn("Analisi_Testata_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Testata_Des)

        Dim FornitoreFatturazione_Des As New DataColumn("FornitoreFatturazione_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(FornitoreFatturazione_Des)

        Dim Piva_FornitoreFatturazione As New DataColumn("Piva_FornitoreFatturazione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Piva_FornitoreFatturazione)

        Dim PDC_Stato_Analisi As New DataColumn("PDC_Stato_Analisi") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(PDC_Stato_Analisi)

        Dim PDC_Stato_Analisi_Des As New DataColumn("PDC_Stato_Analisi_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(PDC_Stato_Analisi_Des)

        Dim PDC_Stato_Validazione As New DataColumn("PDC_Stato_Validazione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(PDC_Stato_Validazione)

        Dim PDC_Stato_Pubblicazione As New DataColumn("PDC_Stato_Pubblicazione") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(PDC_Stato_Pubblicazione)

        Dim Cod_Contatto As New DataColumn("Cod_Contatto") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Cod_Contatto)

        Dim Cod_Risum As New DataColumn("Cod_Risum") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Cod_Risum)

        Dim Cod_Risum_Des As New DataColumn("Cod_Risum_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Cod_Risum_Des)

        Dim Analisi_Tipologia_Cod As New DataColumn("Analisi_Tipologia_Cod") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Tipologia_Cod)

        Dim Analisi_Tipologia_Des As New DataColumn("Analisi_Tipologia_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Tipologia_Des)

        Dim Analisi_Tipologia_Tipo As New DataColumn("Analisi_Tipologia_Tipo") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Tipologia_Tipo)

        Dim Tipo_Campione_Cod As New DataColumn("Tipo_Campione_Cod") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Tipo_Campione_Cod)

        Dim Tipo_Campione_Des As New DataColumn("Tipo_Campione_Des") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Tipo_Campione_Des)



        Dim Altre_Molecole As New DataColumn("Altre_Molecole") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Altre_Molecole)

        Dim Altre_Molecole_Cod As New DataColumn("Altre_Molecole_Cod") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Altre_Molecole_Cod)

        Dim Note_Richiesta_Analisi As New DataColumn("Note_Richiesta_Analisi") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Note_Richiesta_Analisi)


        Dim Data_Richiesta_Analisi As New DataColumn("Data_Richiesta_Analisi") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(Data_Richiesta_Analisi)

        Dim InfoVarie As New DataColumn("InfoVarie") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(InfoVarie)

        Dim ChiaveLab As New DataColumn("Chiave_Lab") With {
            .DataType = Type.GetType("System.String")
        }
        DT.Columns.Add(ChiaveLab)

        DT.Columns.Add(New DataColumn("ID_NC", Type.GetType("System.Int32")))

    End Sub


    Private Shared Sub GeneraStrutturaDT(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("ID_PDC_Testata", Type.GetType("System.Int32")))
        DT.Columns.Add(New DataColumn("ID_PDC_Dettagli", Type.GetType("System.Int32")))
        DT.Columns.Add(New DataColumn("Pdc_Dettagli_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Bool", Type.GetType("System.Boolean")))
        DT.Columns.Add(New DataColumn("Codice_Cliente", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Codice_Cliente_2", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sigla", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Data_Inizio_Impianto", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Magazzino_di_conferimento", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Dettaglio_Specie_Personalizzato", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Tecnico_di_riferimento", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("LottoFornitore", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Descrizione_Disciplinare", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Descrizione_PuntoDiPrelievo", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Tipologia_Varietale", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("NomeArticolo", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Certificato", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Note_Impianto", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Capitolato_Privato", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Colore", Type.GetType("System.Int32")))
        DT.Columns.Add(New DataColumn("Piva", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sa_Cod", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Appezza", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Id_Reg", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Rag_Soc", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sa_Nome", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("App_Nome", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Veg_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Cul_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sup_Imp", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Veg_Cod", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Cul_Cod", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("ID_LFO", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("LFO_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Flag_PDC", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Data_Fornitura", Type.GetType("System.DateTime")))
        DT.Columns.Add(New DataColumn("Data_Fornitura_STR", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Data_Raccolta", Type.GetType("System.DateTime")))
        DT.Columns.Add(New DataColumn("Data_Raccolta_STR", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Data_Semina", Type.GetType("System.DateTime")))
        DT.Columns.Add(New DataColumn("Data_Semina_STR", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Oggetti", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Regolamento", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("note2", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("Tipo_Lotta_Acquisti", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Tipo_Lotta_Acquisti_des", Type.GetType("System.String")))

        DT.Columns.Add(New DataColumn("CapitolatiEsclusi", Type.GetType("System.String")))

        ' nuovi campi PDC Dettagli
        DT.Columns.Add(New DataColumn("Piva_OP", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Rag_Soc_OP", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Grower_Number", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Kpin", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Block", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Ind_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Frz_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("CAP", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Com_Des", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Pro_Cod", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Reg", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Regione", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Stato", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Pro_Cod_Istat", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Com_Cod_Istat", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Tecnico_Campionamento", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Tecnico_Campionamento_Tel", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Progetto_Nome", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("App_Lat", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("App_Long", Type.GetType("System.String")))

        'Colonne ZOO
        DT.Columns.Add(New DataColumn("Cod_Animale", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Cod_Progetto", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("GEN_COD", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Genere", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("SPE_COD", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Specie", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("RAZ_COD", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Razza", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Data_Nascita", Type.GetType("System.DateTime")))
        DT.Columns.Add(New DataColumn("Giorni_Vita", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sesso", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Matricola", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Lotto", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Raggruppamento_Cod", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Raggruppamento", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sta_Num", Type.GetType("System.String")))
        DT.Columns.Add(New DataColumn("Sta_Des", Type.GetType("System.String")))

    End Sub

#End Region

    Public Shared Sub CaricaLista(ByVal Modalita_Nero As Boolean, ByVal ID_Testata As Integer, ByVal Data_Distinta_Attiva As Date,
                                  ByRef Lista_Oggetti As List(Of PDC_Dettagli), ByVal objParametri As AgronicaCoreParametri)

        Dim objPDCDettagli As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        Dim order As String = ""
        Dim OrdinaLFO As Boolean = True
        If OrdinaLFO = True Then
            order = " LFO_DES"
        End If

        Dim dt As DataTable = objPDCDettagli.Leggi(ID_Testata, 0, "", 0, 0, 0, 0, 0, 0, 0, "", order, objParametri)

        Lista_Oggetti.Clear()

        Dim i As Integer

        For i = 0 To dt.Rows.Count - 1
            Dim obj As New PDC_Dettagli

            obj.Id_PDC_Testata = ID_Testata
            obj.ID_PDC_Dettagli = dt.Rows(i).Item("ID_PDC_Dettagli")
            'inutilizzato
            obj.PDC_Dettagli_Des = dt.Rows(i).Item("PDC_Dettagli_Des")

            obj.Impianto.Data_Fornitura = CDate(dt.Rows(i).Item("Data_Fornitura")).ToShortDateString

            obj.Impianto.Piva = dt.Rows(i).Item("Piva")
            obj.Impianto.Sa_Cod = dt.Rows(i).Item("Sa_Cod")
            obj.Impianto.Appezza = dt.Rows(i).Item("Appezza")
            obj.Impianto.Id_Reg = dt.Rows(i).Item("Id_Reg")
            obj.Impianto.Veg_Cod = dt.Rows(i).Item("Veg_Cod")
            obj.Impianto.Cul_Cod = dt.Rows(i).Item("Cul_Cod")
            obj.Impianto.Codice_Cliente = dt.Rows(i).Item("CodiceFornitore")
            obj.Impianto.Certificato = dt.Rows(i).Item("Certificato")
            obj.Impianto.Note_Impianto = dt.Rows(i).Item("Note_Impianto")
            obj.Impianto.note2 = dt.Rows(i).Item("Note2")

            obj.Impianto.Codice_Cliente_2 = dt.Rows(i).Item("Codice_Cliente_2")
            obj.Impianto.Data_Inizio_Impianto = dt.Rows(i).Item("Data_Inizio_Impianto")
            obj.Impianto.Magazzino_di_conferimento = dt.Rows(i).Item("Magazzino_di_conferimento")
            obj.Impianto.Dettaglio_Specie_Personalizzato = dt.Rows(i).Item("Dettaglio_Specie_Personalizzato")


            obj.Impianto.Sigla = dt.Rows(i).Item("Sigla")
            obj.Impianto.Tecnico_di_riferimento = dt.Rows(i).Item("Tecnico_di_riferimento")


            obj.Impianto.LottoFornitore = dt.Rows(i).Item("LottoFornitore")
            obj.Impianto.NomeArticolo = dt.Rows(i).Item("NomeArticolo")
            obj.Impianto.Descrizione_Disciplinare = dt.Rows(i).Item("TipoLotta")
            obj.Impianto.Tipo_Lotta_Acquisti = dt.Rows(i).Item("Tipo_Lotta_Acquisti")
            obj.Impianto.Tipo_Lotta_Acquisti_des = dt.Rows(i).Item("Tipo_Lotta_Acquisti_des")

            obj.Impianto.Descrizione_PuntoDiPrelievo = dt.Rows(i).Item("PuntoPrelievo")


            obj.Impianto.Tipologia_Varietale = dt.Rows(i).Item("Tipologia_Varietale")

            obj.Impianto.Capitolato_Privato = dt.Rows(i).Item("CapitolatoPrivato")
            obj.Impianto.Regolamento = dt.Rows(i).Item("Regolamento")

            obj.Impianto.Rag_Soc = dt.Rows(i).Item("Rag_Soc")
            obj.Impianto.Sa_Nome = dt.Rows(i).Item("Sa_Nome")
            obj.Impianto.App_Nome = dt.Rows(i).Item("App_Nome")
            obj.Impianto.Veg_Des = dt.Rows(i).Item("Veg_Des")
            obj.Impianto.Cul_Des = dt.Rows(i).Item("Cul_Des")
            obj.Impianto.Sup_Imp = dt.Rows(i).Item("Sup_Imp")

            obj.Impianto.Data_Raccolta = dt.Rows(i).Item("Data_Raccolta")

            obj.Impianto.Data_Semina = dt.Rows(i).Item("Data_Semina")

            obj.Impianto.CapitolatiEsclusi = dt.Rows(i).Item("CapitolatiEsclusi")

            ' nuovi campi PDC Dettagli
            obj.Piva_OP = dt.Rows(i).Item("Piva_OP")
            obj.Rag_Soc_OP = dt.Rows(i).Item("Rag_Soc_OP")
            obj.Grower_Number = dt.Rows(i).Item("Grower_Number")
            obj.Kpin = dt.Rows(i).Item("Kpin")
            obj.Block = dt.Rows(i).Item("Block")
            obj.Ind_Des = dt.Rows(i).Item("Ind_Des")
            obj.Frz_Des = dt.Rows(i).Item("Frz_Des")
            obj.CAP = dt.Rows(i).Item("CAP")
            obj.Com_Des = dt.Rows(i).Item("Com_Des")
            obj.Pro_Cod = dt.Rows(i).Item("Pro_Cod")
            obj.Reg = dt.Rows(i).Item("Reg")
            obj.Regione = dt.Rows(i).Item("Regione")
            obj.Stato = dt.Rows(i).Item("Stato")
            obj.Pro_Cod_Istat = dt.Rows(i).Item("Pro_Cod_Istat")
            obj.Com_Cod_Istat = dt.Rows(i).Item("Com_Cod_Istat")
            obj.Tecnico_Campionamento = dt.Rows(i).Item("Tecnico_Campionamento")
            obj.Tecnico_Campionamento_Tel = dt.Rows(i).Item("Tecnico_Campionamento_Tel")
            obj.Progetto_Nome = dt.Rows(i).Item("Progetto_Nome")
            obj.App_Lat = dt.Rows(i).Item("App_Lat")
            obj.App_Long = dt.Rows(i).Item("App_Long")
            'obj.Lat_Campione = dt.Rows(i).Item("Lat_Campione")
            'obj.Long_Campione = dt.Rows(i).Item("Long_Campione")

            'Colonne ZOO
            obj.Cod_Animale = dt.Rows(i).Item("Cod_Animale")
            obj.Cod_Progetto = dt.Rows(i).Item("Cod_Progetto")
            obj.GEN_COD = dt.Rows(i).Item("GEN_COD")
            obj.Genere = dt.Rows(i).Item("Genere")
            obj.SPE_COD = dt.Rows(i).Item("SPE_COD")
            obj.Specie = dt.Rows(i).Item("Specie")
            obj.RAZ_COD = dt.Rows(i).Item("RAZ_COD")
            obj.Razza = dt.Rows(i).Item("Razza")
            obj.Data_Nascita = dt.Rows(i).Item("Data_Nascita")
            obj.Giorni_Vita = dt.Rows(i).Item("Giorni_Vita")
            obj.Sesso = dt.Rows(i).Item("Sesso")
            obj.Matricola = dt.Rows(i).Item("Matricola")
            obj.Lotto = dt.Rows(i).Item("Lotto")
            obj.Raggruppamento_Cod = dt.Rows(i).Item("Raggruppamento_Cod")
            obj.Raggruppamento = dt.Rows(i).Item("Raggruppamento")

            obj.Flag_PDC = dt.Rows(i).Item("Flag_PDC")
            If obj.Flag_PDC = -1 Then
                'Carico la lista dei campioni associati
                Dim objlistaCampioni As New List(Of PDC_Campioni)
                PDC_Campioni_Helper.CaricaLista(Modalita_Nero, ID_Testata, dt.Rows(i).Item("ID_PDC_Dettagli"),
                                                0, objlistaCampioni, objParametri)
                obj.CampioniAssociati = objlistaCampioni
            End If

            If dt.Rows(i).Item("ID_LFO") <> 0 Then
                'Carico l'LFO associato
                Dim objLFO As New PDC_LFO
                objLFO.LFO_Des = dt.Rows(i).Item("LFO_Des")
                objLFO.ID_LFO = dt.Rows(i).Item("ID_LFO")
                objLFO.Id_PDC_Testata = ID_Testata
                'PDC_LFO_Helper.Carica(ID_Testata, DT.Rows(i).Item("ID_LFO"), objLFO, objParametri)
                obj.LFO = objLFO
            End If
            Lista_Oggetti.Add(obj)
        Next

    End Sub

    Public Shared Sub Carica(ByVal Modalita_Nero As Boolean, ID_Testata As Integer, ByVal ID_PDC_Dettagli As Integer,
                             ByVal Data_Distinta_Attiva As Date, ByRef obj As PDC_Dettagli,
                             ByVal objParametri As AgronicaCoreParametri)

        Dim objPDCDettagli As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        Dim dt = objPDCDettagli.Leggi(ID_Testata, ID_PDC_Dettagli, "", 0, 0, 0, 0, 0, 0, 0, "", "", objParametri)

        If dt.Rows.Count > 0 Then
            obj.Id_PDC_Testata = ID_Testata
            obj.ID_PDC_Dettagli = dt.Rows(0).Item("ID_PDC_Dettagli")
            'inutilizzato
            obj.PDC_Dettagli_Des = dt.Rows(0).Item("PDC_Dettagli_Des")

            obj.Impianto.Piva = dt.Rows(0).Item("Piva")
            obj.Impianto.Sa_Cod = dt.Rows(0).Item("Sa_Cod")
            obj.Impianto.Appezza = dt.Rows(0).Item("Appezza")
            obj.Impianto.Id_Reg = dt.Rows(0).Item("Id_Reg")
            obj.Impianto.Veg_Cod = dt.Rows(0).Item("Veg_Cod")
            obj.Impianto.Cul_Cod = dt.Rows(0).Item("Cul_Cod")
            obj.Impianto.Codice_Cliente = dt.Rows(0).Item("CodiceFornitore")
            obj.Impianto.Capitolato_Privato = dt.Rows(0).Item("Capitolato_Privato")

            obj.Impianto.Rag_Soc = dt.Rows(0).Item("Rag_Soc")
            obj.Impianto.Sa_Nome = dt.Rows(0).Item("Sa_Nome")
            obj.Impianto.App_Nome = dt.Rows(0).Item("App_Nome")
            obj.Impianto.Veg_Des = dt.Rows(0).Item("Veg_Des")
            obj.Impianto.Cul_Des = dt.Rows(0).Item("Cul_Des")
            obj.Impianto.Sup_Imp = dt.Rows(0).Item("Sup_Imp")

            obj.Impianto.Data_Raccolta = dt.Rows(0).Item("Data_Raccolta")

            obj.Flag_PDC = dt.Rows(0).Item("Flag_PDC")
            If obj.Flag_PDC = True Then
                'Carico la lista dei campioni associati
                Dim objlistaCampioni As New List(Of PDC_Campioni)
                PDC_Campioni_Helper.CaricaLista(Modalita_Nero, ID_Testata, dt.Rows(0).Item("ID_PDC_Dettagli"),
                                                0, objlistaCampioni, objParametri)
                obj.CampioniAssociati = objlistaCampioni
            End If

            If dt.Rows(0).Item("ID_LFO") <> 0 Then
                'Carico l'LFO associato
                Dim objLFO As New PDC_LFO
                PDC_LFO_Helper.Carica(ID_Testata, dt.Rows(0).Item("ID_LFO"), objLFO, objParametri)
                'objLFO.LFO_Des = DT.Rows(0).Item("LFO_Des")
                'objLFO.ID_LFO = DT.Rows(0).Item("ID_LFO")
                'objLFO.Id_PDC_Testata = ID_Testata
                'PDC_LFO_Helper.Carica(ID_Testata, DT.Rows(i).Item("ID_LFO"), objLFO, objParametri)
                obj.LFO = objLFO
            End If
        End If
    End Sub


    ''' <summary>
    ''' Passato un oggetto LFO ritorna la lista modificata cambiando la LFO_Des
    ''' </summary>
    ''' <param name="Lista_Oggetti"></param>
    ''' <param name="LFO"></param>
    ''' <remarks></remarks>
    Public Shared Sub AggiornaLista_LFO(ByRef Lista_Oggetti As List(Of PDC_Dettagli), ByVal LFO As PDC_LFO)
        For i As Integer = 0 To Lista_Oggetti.Count - 1
            If Lista_Oggetti(i).LFO.ID_LFO = LFO.ID_LFO Then
                Lista_Oggetti(i).LFO.LFO_Des = LFO.LFO_Des
            End If
        Next
    End Sub




    Public Shared Function ScriviSingolo(ByRef oggetto As PDC_Dettagli, ByVal ID_PDC_Testata As String, ByVal objParametri As AgronicaCoreParametri)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'sono in fase di aggiunta 
            Dim objPdcDetta As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            'TODO: REFACTOR = Togli uso Session!!!
            oggetto.ID_PDC_Dettagli = objSeq.NuovoId_Tabella("PDC_Dettagli", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
            oggetto.Id_PDC_Testata = ID_PDC_Testata

            objPdcDetta.Scrivi(ID_PDC_Testata,
                               oggetto.ID_PDC_Dettagli,
                               oggetto.PDC_Dettagli_Des,
                               oggetto.Impianto.Piva,
                               oggetto.Impianto.Sa_Cod,
                               oggetto.Impianto.Appezza,
                               oggetto.Impianto.Id_Reg,
                               oggetto.Impianto.Veg_Cod,
                               oggetto.Impianto.Cul_Cod,
                               oggetto.LFO.ID_LFO,
                               oggetto.Flag_PDC,
                               objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function ScriviSingoloCapoAnimale(ByRef oggetto As PDC_Dettagli, ByVal ID_PDC_Testata As String, ByVal objParametri As AgronicaCoreParametri)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'sono in fase di aggiunta 
            Dim objPdcDetta As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            'TODO: REFACTOR = Togli uso Session!!!
            oggetto.ID_PDC_Dettagli = objSeq.NuovoId_Tabella("PDC_Dettagli", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
            oggetto.Id_PDC_Testata = ID_PDC_Testata

            objPdcDetta.Scrivi(ID_PDC_Testata,
                               oggetto.ID_PDC_Dettagli,
                               oggetto.PDC_Dettagli_Des,
                               "", 0, 0, 0, 0, 0, 0, -1,
                               objParametri,
                               oggetto.Cod_Animale,
                               oggetto.Cod_Progetto,
                               oggetto.GEN_COD,
                               oggetto.SPE_COD, oggetto.RAZ_COD,
                               oggetto.Data_Nascita,
                               oggetto.Sesso,
                               oggetto.Matricola,
                               oggetto.Lotto,
                               oggetto.Raggruppamento_Cod)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function Scrivi(ByVal oggetto As List(Of PDC_Dettagli), ByVal ID_PDC_Testata As String, ByVal objParametri As AgronicaCoreParametri, Optional ByVal baseCode As Integer = 0, Optional ByVal topCode As Integer = 0, Optional ByVal daZoo As Boolean = False)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            ' uso valori in session se definiti
            If HttpContext.Current.Session IsNot Nothing Then
                baseCode = HttpContext.Current.Session("BaseCode")
                topCode = HttpContext.Current.Session("TopCode")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'sono in fase di aggiunta 
            Dim objPdcDetta As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
            For i As Integer = 0 To oggetto.Count - 1

                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                oggetto(i).ID_PDC_Dettagli = objSeq.NuovoId_Tabella("PDC_Dettagli", baseCode, topCode, objParametri)
                oggetto(i).Id_PDC_Testata = ID_PDC_Testata

                objPdcDetta.Scrivi(ID_PDC_Testata,
                                   oggetto(i).ID_PDC_Dettagli,
                                   oggetto(i).PDC_Dettagli_Des,
                                   oggetto(i).Impianto.Piva,
                                   oggetto(i).Impianto.Sa_Cod,
                                   oggetto(i).Impianto.Appezza,
                                   oggetto(i).Impianto.Id_Reg,
                                   oggetto(i).Impianto.Veg_Cod,
                                   oggetto(i).Impianto.Cul_Cod,
                                   oggetto(i).LFO.ID_LFO,
                                   oggetto(i).Flag_PDC,
                                   objParametri,
                                   oggetto(i).Cod_Animale,
                                   oggetto(i).Cod_Progetto,
                                   oggetto(i).GEN_COD,
                                   oggetto(i).SPE_COD,
                                   oggetto(i).RAZ_COD,
                                   oggetto(i).Data_Nascita,
                                   oggetto(i).Sesso,
                                   oggetto(i).Matricola,
                                   oggetto(i).Lotto,
                                   oggetto(i).Raggruppamento_Cod)

                ' scrive campioni associati al dettaglio per pdc zoo
                If daZoo AndAlso oggetto(i).CampioniAssociati IsNot Nothing AndAlso oggetto(i).CampioniAssociati.Count > 0 Then
                    PDC_Campioni_Helper.Scrivi(oggetto(i).CampioniAssociati, ID_PDC_Testata, oggetto(i).ID_PDC_Dettagli, objParametri, baseCode, topCode)
                End If

            Next

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function


    Public Shared Function Modifica(ByVal oggetto As List(Of PDC_Dettagli), ByVal ID_PDC_Testata As String, ByVal objParametri As AgronicaCoreParametri, Optional ByVal baseCode As Integer = 0, Optional ByVal topCode As Integer = 0, Optional ByVal daZoo As Boolean = False)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try
            ' uso valori in session se definiti
            If HttpContext.Current.Session IsNot Nothing Then
                baseCode = HttpContext.Current.Session("BaseCode")
                topCode = HttpContext.Current.Session("TopCode")
            End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'sono in fase di modifica
            Dim objPdcDetta As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
            For i As Integer = 0 To oggetto.Count - 1

                If oggetto(i).ID_PDC_Dettagli = 0 Then

                    Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                    oggetto(i).ID_PDC_Dettagli = objSeq.NuovoId_Tabella("PDC_Dettagli", baseCode, topCode, objParametri)
                    oggetto(i).Id_PDC_Testata = ID_PDC_Testata

                    objPdcDetta.Scrivi(ID_PDC_Testata,
                                       oggetto(i).ID_PDC_Dettagli,
                                       oggetto(i).PDC_Dettagli_Des,
                                       oggetto(i).Impianto.Piva,
                                       oggetto(i).Impianto.Sa_Cod,
                                       oggetto(i).Impianto.Appezza,
                                       oggetto(i).Impianto.Id_Reg,
                                       oggetto(i).Impianto.Veg_Cod,
                                       oggetto(i).Impianto.Cul_Cod,
                                       oggetto(i).LFO.ID_LFO,
                                       oggetto(i).Flag_PDC,
                                       objParametri,
                                       oggetto(i).Cod_Animale,
                                       oggetto(i).Cod_Progetto,
                                       oggetto(i).GEN_COD,
                                       oggetto(i).SPE_COD,
                                       oggetto(i).RAZ_COD,
                                       oggetto(i).Data_Nascita,
                                       oggetto(i).Sesso,
                                       oggetto(i).Matricola,
                                       oggetto(i).Lotto,
                                       oggetto(i).Raggruppamento_Cod)

                    ' scrive campioni associati al dettaglio per pdc zoo
                    If daZoo AndAlso oggetto(i).CampioniAssociati IsNot Nothing AndAlso oggetto(i).CampioniAssociati.Count > 0 Then
                        PDC_Campioni_Helper.Scrivi(oggetto(i).CampioniAssociati, ID_PDC_Testata, oggetto(i).ID_PDC_Dettagli, objParametri, baseCode, topCode)
                    End If

                Else
                    'modifico 

                    objPdcDetta.Modifica(ID_PDC_Testata,
                                         oggetto(i).ID_PDC_Dettagli,
                                         oggetto(i).PDC_Dettagli_Des,
                                         oggetto(i).Impianto.Piva,
                                         oggetto(i).Impianto.Sa_Cod,
                                         oggetto(i).Impianto.Appezza,
                                         oggetto(i).Impianto.Id_Reg,
                                         oggetto(i).Impianto.Veg_Cod,
                                         oggetto(i).Impianto.Cul_Cod,
                                         oggetto(i).LFO.ID_LFO,
                                         oggetto(i).Flag_PDC,
                                         objParametri)

                End If


            Next

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Cancellazione dettaglio zoo
    ''' </summary>
    ''' <param name="Id_Testata"></param>
    ''' <param name="Cod_Animale"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks></remarks>
    Public Shared Sub CancellaDettagliZoo(ByVal Id_Testata As Integer, ByVal Cod_Animale As Integer, ByRef objParametri As AgronicaCoreParametri)

        ' cancella i dettagli relativi al capo animale passato
        Dim objPDCDettagli As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        Dim dt As DataTable = objPDCDettagli.Leggi_solo_dettagli(Id_Testata, "Cod_Animale=" & Cod_Animale, "", objParametri)

        For Each row In dt.Rows
            Cancella(Id_Testata, CInt(row.Item("ID_PDC_Dettagli")), objParametri)
        Next

    End Sub

    ''' <summary>
    ''' Cancellazione
    ''' </summary>
    ''' <param name="Id_Testata"></param>
    ''' <param name="Id_Dettagli"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="CancellaAncheBlocchi">indica se cancellare anche i relativi blocchi/sblocchi</param>
    ''' <param name="CancellaAncheMarketAccess">indica se cancellare anche il market access a valle</param>
    ''' <remarks></remarks>
    Public Shared Function Cancella(ByVal Id_Testata As Integer,
                                    ByVal Id_Dettagli As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal CancellaAncheBlocchi As Boolean = False,
                                    Optional ByVal CancellaAncheMarketAccess As Boolean = False
                                    ) As String

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'Elimino tutti i campioni associati a questo ID_Testata
            xRisp = PDC_Campioni_Helper.Cancella(Id_Testata, Id_Dettagli, 0,
                                                 objParametri, CancellaAncheBlocchi, CancellaAncheMarketAccess)
            If xRisp <> "" Then
                Throw New Exception(xRisp)
            End If

            'Elimino il Dettaglio
            Dim objPdc_DettagliW As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_W
            objPdc_DettagliW.Cancella(Id_Testata, Id_Dettagli, objParametri)

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function


    Public Shared Function Cancella(ByVal OggettoPDC As PDC_Testata,
                                    ByVal Id_Testata As Integer,
                                    ByVal Id_Dettagli As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal CancellaAncheBlocchi As Boolean = False,
                                    Optional ByVal CancellaAncheMarketAccess As Boolean = False)

        Dim flagConnessione, flagTransazione As Boolean
        Dim xRisp As String = ""

        Try

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'Eliminazione fisica dei record
            Cancella(Id_Testata, Id_Dettagli, objParametri, CancellaAncheBlocchi, CancellaAncheMarketAccess)

            'scorro fino all'elemento desiderato
            For i As Integer = 0 To OggettoPDC.PDC_Dettagli.Count - 1
                If OggettoPDC.PDC_Dettagli(i).ID_PDC_Dettagli = Id_Dettagli Then
                    OggettoPDC.PDC_Dettagli.RemoveAt(i)
                    Exit For
                End If
            Next

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Sub CaricaListaMassivo(ByVal Modalita_Nero As Boolean, ByVal ID_Testata As Integer, ByVal Data_Distinta_Attiva As Date,
                                         ByRef Lista_Oggetti As List(Of PDC_Dettagli), ByVal objParametri As AgronicaCoreParametri)


        Dim dtDettagli As New DataTable
        Dim dtCampioni As New DataTable
        Dim dtAnalisi As New DataTable

        estraiDT(ID_Testata, Modalita_Nero, dtDettagli, dtCampioni, dtAnalisi, objParametri)
        Lista_Oggetti.Clear()

        Dim i As Integer
        For i = 0 To dtDettagli.Rows.Count - 1
            Dim obj As New PDC_Dettagli

            obj.Id_PDC_Testata = ID_Testata
            obj.ID_PDC_Dettagli = dtDettagli.Rows(i).Item("ID_PDC_Dettagli")
            'inutilizzato
            obj.PDC_Dettagli_Des = dtDettagli.Rows(i).Item("PDC_Dettagli_Des")

            obj.Impianto.Data_Fornitura = CDate(dtDettagli.Rows(i).Item("Data_Fornitura")).ToShortDateString

            obj.Impianto.Piva = dtDettagli.Rows(i).Item("Piva")
            obj.Impianto.Sa_Cod = dtDettagli.Rows(i).Item("Sa_Cod")
            obj.Impianto.Appezza = dtDettagli.Rows(i).Item("Appezza")
            obj.Impianto.Id_Reg = dtDettagli.Rows(i).Item("Id_Reg")
            obj.Impianto.Veg_Cod = dtDettagli.Rows(i).Item("Veg_Cod")
            obj.Impianto.Cul_Cod = dtDettagli.Rows(i).Item("Cul_Cod")
            obj.Impianto.Codice_Cliente = dtDettagli.Rows(i).Item("CodiceFornitore")
            obj.Impianto.Certificato = dtDettagli.Rows(i).Item("Certificato")
            obj.Impianto.Note_Impianto = dtDettagli.Rows(i).Item("Note_Impianto")
            obj.Impianto.note2 = dtDettagli.Rows(i).Item("Note2")

            obj.Impianto.Codice_Cliente_2 = dtDettagli.Rows(i).Item("Codice_Cliente_2")
            obj.Impianto.Data_Inizio_Impianto = dtDettagli.Rows(i).Item("Data_Inizio_Impianto")
            obj.Impianto.Magazzino_di_conferimento = dtDettagli.Rows(i).Item("Magazzino_di_conferimento")
            obj.Impianto.Dettaglio_Specie_Personalizzato = dtDettagli.Rows(i).Item("Dettaglio_Specie_Personalizzato")


            obj.Impianto.Sigla = dtDettagli.Rows(i).Item("Sigla")
            obj.Impianto.Tecnico_di_riferimento = dtDettagli.Rows(i).Item("Tecnico_di_riferimento")


            obj.Impianto.LottoFornitore = dtDettagli.Rows(i).Item("LottoFornitore")
            obj.Impianto.NomeArticolo = dtDettagli.Rows(i).Item("NomeArticolo")
            obj.Impianto.Descrizione_Disciplinare = dtDettagli.Rows(i).Item("TipoLotta")
            obj.Impianto.Tipo_Lotta_Acquisti = dtDettagli.Rows(i).Item("Tipo_Lotta_Acquisti")
            obj.Impianto.Tipo_Lotta_Acquisti_des = dtDettagli.Rows(i).Item("Tipo_Lotta_Acquisti_des")

            obj.Impianto.Descrizione_PuntoDiPrelievo = dtDettagli.Rows(i).Item("PuntoPrelievo")


            obj.Impianto.Tipologia_Varietale = dtDettagli.Rows(i).Item("Tipologia_Varietale")

            obj.Impianto.Capitolato_Privato = dtDettagli.Rows(i).Item("CapitolatoPrivato")
            obj.Impianto.Regolamento = dtDettagli.Rows(i).Item("Regolamento")

            obj.Impianto.Rag_Soc = dtDettagli.Rows(i).Item("Rag_Soc")
            obj.Impianto.Sa_Nome = dtDettagli.Rows(i).Item("Sa_Nome")
            obj.Impianto.App_Nome = dtDettagli.Rows(i).Item("App_Nome")
            obj.Impianto.Veg_Des = dtDettagli.Rows(i).Item("Veg_Des")
            obj.Impianto.Cul_Des = dtDettagli.Rows(i).Item("Cul_Des")
            obj.Impianto.Sup_Imp = dtDettagli.Rows(i).Item("Sup_Imp")

            obj.Impianto.Data_Raccolta = dtDettagli.Rows(i).Item("Data_Raccolta")

            obj.Impianto.Data_Semina = dtDettagli.Rows(i).Item("Data_Semina")

            obj.Impianto.CapitolatiEsclusi = dtDettagli.Rows(i).Item("CapitolatiEsclusi")

            ' nuovi campi PDC Dettagli
            obj.Piva_OP = dtDettagli.Rows(i).Item("Piva_OP")
            obj.Rag_Soc_OP = dtDettagli.Rows(i).Item("Rag_Soc_OP")
            obj.Grower_Number = dtDettagli.Rows(i).Item("Grower_Number")
            obj.Kpin = dtDettagli.Rows(i).Item("Kpin")
            obj.Block = dtDettagli.Rows(i).Item("Block")
            obj.Ind_Des = dtDettagli.Rows(i).Item("Ind_Des")
            obj.Frz_Des = dtDettagli.Rows(i).Item("Frz_Des")
            obj.CAP = dtDettagli.Rows(i).Item("CAP")
            obj.Com_Des = dtDettagli.Rows(i).Item("Com_Des")
            obj.Pro_Cod = dtDettagli.Rows(i).Item("Pro_Cod")
            obj.Reg = dtDettagli.Rows(i).Item("Reg")
            obj.Regione = dtDettagli.Rows(i).Item("Regione")
            obj.Stato = dtDettagli.Rows(i).Item("Stato")
            obj.Pro_Cod_Istat = dtDettagli.Rows(i).Item("Pro_Cod_Istat")
            obj.Com_Cod_Istat = dtDettagli.Rows(i).Item("Com_Cod_Istat")
            obj.Tecnico_Campionamento = dtDettagli.Rows(i).Item("Tecnico_Campionamento")
            obj.Tecnico_Campionamento_Tel = dtDettagli.Rows(i).Item("Tecnico_Campionamento_Tel")
            obj.Progetto_Nome = dtDettagli.Rows(i).Item("Progetto_Nome")
            obj.App_Lat = dtDettagli.Rows(i).Item("App_Lat")
            obj.App_Long = dtDettagli.Rows(i).Item("App_Long")

            'Colonne ZOO
            obj.Cod_Animale = dtDettagli.Rows(i).Item("Cod_Animale")
            obj.Cod_Progetto = dtDettagli.Rows(i).Item("Cod_Progetto")
            obj.GEN_COD = dtDettagli.Rows(i).Item("GEN_COD")
            obj.Genere = dtDettagli.Rows(i).Item("Genere")
            obj.SPE_COD = dtDettagli.Rows(i).Item("SPE_COD")
            obj.Specie = dtDettagli.Rows(i).Item("Specie")
            obj.RAZ_COD = dtDettagli.Rows(i).Item("RAZ_COD")
            obj.Razza = dtDettagli.Rows(i).Item("Razza")
            obj.Data_Nascita = dtDettagli.Rows(i).Item("Data_Nascita")
            obj.Giorni_Vita = dtDettagli.Rows(i).Item("Giorni_Vita")
            obj.Sesso = dtDettagli.Rows(i).Item("Sesso")
            obj.Matricola = dtDettagli.Rows(i).Item("Matricola")
            obj.Lotto = dtDettagli.Rows(i).Item("Lotto")
            obj.Raggruppamento_Cod = dtDettagli.Rows(i).Item("Raggruppamento_Cod")
            obj.Raggruppamento = dtDettagli.Rows(i).Item("Raggruppamento")

            obj.Sta_Num = dtDettagli.Rows(i).Item("Sta_Num")
            obj.Sta_Des = dtDettagli.Rows(i).Item("Sta_Des")

            obj.Flag_PDC = dtDettagli.Rows(i).Item("Flag_PDC")

            If obj.Flag_PDC = -1 AndAlso dtCampioni.Rows.Count > 0 Then
                'Carico la lista dei campioni associati
                Dim objlistaCampioni As New List(Of PDC_Campioni)
                obj.CampioniAssociati = PDC_Campioni_Helper.CaricaCampioni(ID_Testata, dtDettagli.Rows(i).Item("ID_PDC_Dettagli"), dtCampioni, dtAnalisi)
            End If

            If dtDettagli.Rows(i).Item("ID_LFO") <> 0 Then
                'Carico l'LFO associato
                Dim objLFO As New PDC_LFO
                objLFO.LFO_Des = dtDettagli.Rows(i).Item("LFO_Des")
                objLFO.ID_LFO = dtDettagli.Rows(i).Item("ID_LFO")
                objLFO.Id_PDC_Testata = ID_Testata

                obj.LFO = objLFO
            End If
            Lista_Oggetti.Add(obj)
        Next

    End Sub

    Private Shared Sub estraiDT(ID_Testata As Integer, Modalita_Nero As Boolean, ByRef dt_dettagli As DataTable, ByRef dt_campioni As DataTable, ByRef dt_analisi As DataTable, objParametri As AgronicaCoreParametri)

        Dim objPDCDettagli As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        Dim objPDCCampioni As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_R
        Dim objPDCAnalisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R

        Dim xFiltroAggiuntivo As String = ""
        If Modalita_Nero = True Then
            xFiltroAggiuntivo = " PDC_Campioni.ID_PDC_Campione NOT IN ( " &
                         " Select PDC_Analisi.ID_PDC_Campione " &
                         " FROM         Analisi_Conformita_Capitolato_Cliente INNER JOIN " &
                         " PDC_Analisi ON Analisi_Conformita_Capitolato_Cliente.Analisi_Testata_Cod = PDC_Analisi.Analisi_Testata_Cod " &
                         " WHERE     (Analisi_Conformita_Capitolato_Cliente.CapitolatoCliente_Cod = 0) AND (Analisi_Conformita_Capitolato_Cliente.Esito <> - 1) " &
                         " AND ID_PDC_Testata=" & ID_Testata &
                         ")"
            xFiltroAggiuntivo &= " AND PDC_Campioni.ID_PDC_Campione NOT IN (  " &
                         " Select PDC_Analisi.ID_PDC_Campione " &
                         " FROM         PDC_Analisi " &
                         " WHERE     PDC_Analisi.mostra_in_stampe <>-1 " &
                         " AND ID_PDC_Testata=" & ID_Testata &
                         ")"
        End If

        dt_dettagli = objPDCDettagli.Leggi(ID_Testata, 0, "", 0, 0, 0, 0, 0, 0, 0, "", " LFO_DES", objParametri)
        dt_campioni = objPDCCampioni.Leggi(ID_Testata, 0, 0, xFiltroAggiuntivo, "", objParametri)
        dt_analisi = objPDCAnalisi.Leggi(ID_Testata, 0, 0, 0, "", "", objParametri)

    End Sub

End Class
