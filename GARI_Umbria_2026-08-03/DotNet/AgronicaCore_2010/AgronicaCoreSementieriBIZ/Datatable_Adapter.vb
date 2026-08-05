Imports AgronicaCoreDataProvider

Public Class DataTable_Adapter


    '################################################################################
    Public Shared Sub SuperDT_Impianti_Integrazione_01(
                                ByRef DT As DataTable,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim i As Integer

        'Rendo modificabili le colonne 
        For i = 0 To DT.Columns.Count - 1
            DT.Columns(i).ReadOnly = False
        Next

        'Formatto alcuni campi ...
        For i = 0 To DT.Rows.Count - 1

            '--- Selezionato

            DT.Rows(i).Item("Selezionato") = 0

            '--- UNID_Impianto

            'DT.Rows(i).Item("UNID_Impianto") = i

            '--- Grva_Cod

            If IsDBNull(DT.Rows(i).Item("Grva_Cod")) Then
                DT.Rows(i).Item("Grva_Cod") = 0
                DT.Rows(i).Item("Grva_Des") = " ... "
            End If

            '--- Grva_Cod_Veg

            'Se il campo vale -1 allora lo pongo a zero 
            'perche' il valore -1 e' dovuto ad un errore 
            '(da luogo alla tipologia 'Precocissima')

            'If DT.Rows(i).Item("Grva_Cod_Veg") = -1 Then
            '    DT.Rows(i).Item("Grva_Cod_Veg") = 0
            '    DT.Rows(i).Item("Grva_Cod") = 0
            '    DT.Rows(i).Item("Grva_Des") = " ... "
            'End If

            '--- Flag_Hybrid

            'Un valore negativo indica che la coltura e' Hybrid

            'If DT.Rows(i).Item("Grva_Cod_Veg") < 0 Then
            '    DT.Rows(i).Item("Flag_Hybrid") = 1
            'Else
            '    DT.Rows(i).Item("Flag_Hybrid") = 0
            'End If

            '--- Tipologia_Des

            If IsDBNull(DT.Rows(i).Item("Grva_Des")) Then
                DT.Rows(i).Item("Tipologia_Des") = " ... "
            Else
                If DT.Rows(i).Item("Flag_Hybrid") = 1 Then
                    DT.Rows(i).Item("Tipologia_Des") = DT.Rows(i).Item("Grva_Des") & " {Hybrid}"
                Else
                    DT.Rows(i).Item("Tipologia_Des") = DT.Rows(i).Item("Grva_Des")
                End If
            End If

            '--- Descrizione Poligono

            DT.Rows(i).Item("Descrizione_Poligono") =
                        DT.Rows(i).Item("Veg_Des") & " - " & DT.Rows(i).Item("Tipologia_Des")

            '--- Indirizzo

        Next

    End Sub



    '################################################################################
    Public Shared Sub SuperDT_Impianti_Pulizia_Selezione(
                                ByRef DT As DataTable,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim i As Integer

        '--------------------------------------------
        'Elimino gli impianti non selezionati ...
        '--------------------------------------------
        For i = (DT.Rows.Count - 1) To 0 Step -1
            If DT.Rows(i).Item("Selezionato") = 0 Then
                DT.Rows(i).Delete()
            End If
        Next
        DT.AcceptChanges()

    End Sub



    '################################################################################
    Public Shared Sub SuperDT_Impianti_Pulizia_Poligoni(
                                ByRef DT As DataTable,
                                ByRef objServer As System.Web.HttpServerUtility,
                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                ByRef objPage As System.Web.UI.Page)

        Dim i As Integer

        '---------------------------------------------------
        'Elimino gli impianti senza poligono associato ...
        '---------------------------------------------------

        For i = (DT.Rows.Count - 1) To 0 Step -1
            If DT.Rows(i).Item("Flag_PoligonoPresente") = 0 Then
                DT.Rows(i).Delete()
            End If
        Next

        DT.AcceptChanges()

    End Sub



    '################################################################################
    Public Shared Sub SuperDT_Impianti_Integrazione_02(
                                ByRef DT As DataTable,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim i As Integer
        Dim ID_Grafica As String
        Dim Chiave_Grafica As String

        '------------------------------------------------------------
        'Integro alcuni campi ... "ID_Grafica" e "Chiave_Grafica"
        '------------------------------------------------------------
        For i = 0 To DT.Rows.Count - 1
            'ID_Grafica = Agro_Calcola_IDGrafica( _
            '                            DT.Rows(i).Item("Piva"), _
            '                            DT.Rows(i).Item("Sa_Cod"), _
            '                            DT.Rows(i).Item("Appezza"), _
            '                            DT.Rows(i).Item("Id_Reg"))

            'Chiave_Grafica = Agro_Calcola_ChiaveGrafica( _
            '                            DT.Rows(i).Item("Piva"), _
            '                            DT.Rows(i).Item("Sa_Cod"), _
            '                            ID_Grafica)

            DT.Rows(i).Item("ID_Grafica") = DT.Rows(i).Item("Entita_Cod")
            DT.Rows(i).Item("Chiave_Grafica") = DT.Rows(i).Item("Entita_Cod")
        Next
    End Sub



    '################################################################################
    Public Shared Sub SuperDT_Impianti_Integrazione_04(
                                ByRef DT_Impianti As DataTable,
                                ByRef DT_Poligoni As DataTable,
                                ByRef objServer As System.Web.HttpServerUtility,
                                ByRef objSession As System.Web.SessionState.HttpSessionState,
                                ByRef objPage As System.Web.UI.Page)

        Dim i As Integer
        Dim iR As Integer
        Dim Risultato As DataRow()
        Dim Chiave_Grafica As String

        Dim X As Integer
        Dim Y As Integer
        Dim SommaX As Double
        Dim SommaY As Double
        Dim MinX, MaxX, MinY, MaxY As Integer
        Dim NumeroVertici As Integer
        Dim BaricentroX As Integer
        Dim BaricentroY As Integer

        '------------------------------------------------------------
        'Integro le informazioni grafiche ...
        '------------------------------------------------------------

        For i = 0 To DT_Impianti.Rows.Count - 1

            Chiave_Grafica = DT_Impianti.Rows(i).Item("Chiave_Grafica")

            Risultato = DT_Poligoni.Select("Chiave_Grafica = '" & Chiave_Grafica & "' ")

            NumeroVertici = Risultato.GetLength(0)

            If NumeroVertici > 0 Then

                'Inizializzo
                SommaX = 0
                SommaY = 0
                MinX = 2000000000
                MaxX = 0
                MinY = 2000000000
                MaxY = 0

                'Esamino i vertici
                For iR = 0 To NumeroVertici - 1

                    X = Int(Risultato(iR)("X"))
                    Y = Int(Risultato(iR)("Y"))

                    SommaX = SommaX + X
                    SommaY = SommaY + Y

                    If X < MinX Then MinX = X
                    If Y < MinY Then MinY = Y
                    If X > MaxX Then MaxX = X
                    If Y > MaxY Then MaxY = Y

                Next

                BaricentroX = Int(SommaX / NumeroVertici)
                BaricentroY = Int(SommaY / NumeroVertici)

                'Poligono presente
                DT_Impianti.Rows(i).Item("Flag_PoligonoPresente") = 1
                DT_Impianti.Rows(i).Item("Baricentro_X") = BaricentroX
                DT_Impianti.Rows(i).Item("Baricentro_Y") = BaricentroY
                DT_Impianti.Rows(i).Item("Min_X") = MinX
                DT_Impianti.Rows(i).Item("Min_Y") = MinY
                DT_Impianti.Rows(i).Item("Max_X") = MaxX
                DT_Impianti.Rows(i).Item("Max_Y") = MaxY

            Else

                'Nessun poligono
                DT_Impianti.Rows(i).Item("Flag_PoligonoPresente") = 0

            End If

        Next

    End Sub




    '################################################################################
    Public Shared Sub SuperDT_Impianti_Integrazione_05(
                                ByRef DT As DataTable,
                                ByRef objParametri As AgronicaCoreParametri)

        Dim i As Integer
        Dim DTrv As New DataTable
        Dim DTms As New DataTable
        Dim DRrv() As DataRow
        Dim DRms() As DataRow

        Dim Veg_Cod As Integer
        Dim Grva_Cod As Integer
        Dim Raggruppamento As Integer
        Dim Flag_Hybrid As Integer
        Dim Flag_DatiSpecie_Completi As Integer

        Dim ID_Specie As Integer
        Dim ID_SottoSpecie As Integer
        Dim ID_Gruppo As Integer
        Dim ID_Genotipo As Integer

        '------------------------------------------------------------
        'Integro le informazioni relative alle specie di sementi ...
        '------------------------------------------------------------

        'Recupero il datatable con le informazioni sul RAGGRUPPAMENTO ...
        Dim leggiGruppoVarietale As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        DTrv = leggiGruppoVarietale.Leggi(-1, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        'ìDTrv = AD_RaggruppamentoVarietale_Leggi(-1, objServer, objSession, objPage)

        'Recupero il datatable con le informazioni sulle specie dei sementieri ...
        Dim leggiMappaturaSpecie As New AgronicaCoreSementieriDAL.Mappatura_Specie_R
        DTms = leggiMappaturaSpecie.Leggi("", "", objParametri)
        'DTms = AD_MappaturaSpecie_Leggi(objServer, objSession, objPage)

        For i = 0 To DT.Rows.Count - 1

            Veg_Cod = DT.Rows(i).Item("Veg_Cod")
            Grva_Cod = DT.Rows(i).Item("Grva_Cod")
            Flag_Hybrid = DT.Rows(i).Item("Flag_Hybrid")

            '--- RAGGRUPPAMENTO

            'Cerco il record con le informazioni ...
            DRrv = DTrv.Select("(Veg_Cod = " & Veg_Cod & ") AND (Grva_Cod = " & Grva_Cod & ")")

            If DRrv.GetLength(0) > 0 Then
                'Ho trovato il raggruppamento ... 
                Raggruppamento = DRrv(0).Item("Raggruppamento")
                Flag_DatiSpecie_Completi = 1
            Else
                'Non ho trovato nulla ... il grva_cod non era valorizzato correttamente
                Raggruppamento = 0
                Flag_DatiSpecie_Completi = 0
            End If

            'Aggiorno il datatable Impianti
            DT.Rows(i).Item("Raggruppamento") = Raggruppamento
            DT.Rows(i).Item("Flag_DatiSpecie_Completi") = Flag_DatiSpecie_Completi

            '--- MAPPATURA SPECIE

            'Se mi trovo nel caso BIETOLA non ha importanza il valore di HYBRID
            If (Veg_Cod = 6) Or (Veg_Cod = 5000021) Or (Veg_Cod = 70) Or (Veg_Cod = 69) Then
                Flag_Hybrid = -1
            End If

            'I valori 0 devono essere sostituiti da -1 per effettuare la ricerca ...
            If Grva_Cod = 0 Then Grva_Cod = -1
            If Raggruppamento = 0 Then Raggruppamento = -1

            'Cerco il record con le informazioni ...
            DRms = DTms.Select("(Veg_Cod = " & Veg_Cod & ") AND " &
                               "(Grva_Cod = " & Grva_Cod & ") AND " &
                               "(Raggruppamento = " & Raggruppamento & ") AND " &
                               "(Hybrid = " & Flag_Hybrid & ") ")

            If DRms.GetLength(0) > 0 Then

                'Ho trovato le informazioni
                ID_Specie = DRms(0).Item("ID_Specie")
                ID_SottoSpecie = DRms(0).Item("ID_SottoSpecie")
                ID_Gruppo = DRms(0).Item("ID_Gruppo")
                ID_Genotipo = DRms(0).Item("ID_Genotipo")

            Else

                Flag_DatiSpecie_Completi = 0

                ID_Specie = 0
                ID_SottoSpecie = 0
                ID_Gruppo = 0
                ID_Genotipo = 0

                'Non ho trovato nulla ... mi procuro almeno la specie ...
                DRms = DTms.Select("Veg_Cod = " & Veg_Cod)

                If DRms.GetLength(0) > 0 Then
                    ID_Specie = DRms(0).Item("ID_Specie")
                End If

            End If

            'Aggiorno il datatable Impianti
            DT.Rows(i).Item("ID_Specie") = ID_Specie
            DT.Rows(i).Item("ID_SottoSpecie") = ID_SottoSpecie
            DT.Rows(i).Item("ID_Gruppo") = ID_Gruppo
            DT.Rows(i).Item("ID_Genotipo") = ID_Genotipo
            DT.Rows(i).Item("Flag_DatiSpecie_Completi") = Flag_DatiSpecie_Completi

        Next

        'Garbage collection
        DTrv.Dispose()
        DTms.Dispose()

    End Sub






End Class
