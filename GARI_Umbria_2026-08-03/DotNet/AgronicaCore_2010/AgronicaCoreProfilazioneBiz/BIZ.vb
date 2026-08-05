
Imports AgronicaCoreDataProvider
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.utente

Public Class Profilazione_R


    'legge le note inserite
    'legge e da la precedenza a:
    'note azienda per operazione
    'note azienda generiche
    'note globali per operazione
    'note globali generiche
    Public Function LeggiProfilazioneNote_In_Cascata(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Veg_cod As Integer,
                                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT_Elenco_Note As DataTable
        'Azienda - Lav_Cod - Veg_Cod
        DT_Elenco_Note = LeggiProfilazioneNote(PIVA_isK, Codice_Chiave_isK, Lav_Cod, Veg_cod, True, True, objParametri)

        'Azienda - Lav_Cod - 0
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote(PIVA_isK, Codice_Chiave_isK, Lav_Cod, 0, True, True, objParametri)
        End If

        'Azienda - 0 - Veg_Cod
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote(PIVA_isK, Codice_Chiave_isK, 0, Veg_cod, True, True, objParametri)
        End If

        'Azienda - 0 - 0
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote(PIVA_isK, Codice_Chiave_isK, 0, 0, True, True, objParametri)
        End If

        'Globale -  Lav_Cod - Veg_Cod
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote("", Codice_Chiave_isK, Lav_Cod, Veg_cod, True, True, objParametri)
        End If

        'Globale - Lav_Cod -0
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote("", Codice_Chiave_isK, Lav_Cod, 0, True, True, objParametri)
        End If

        'Globale - 0 - Veg_Cod
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote("", Codice_Chiave_isK, 0, Veg_cod, True, True, objParametri)
        End If

        'Globale - 0 - 0
        If IsNothing(DT_Elenco_Note) Or DT_Elenco_Note.Rows.Count = 0 Then
            DT_Elenco_Note = LeggiProfilazioneNote("", Codice_Chiave_isK, 0, 0, True, True, objParametri)
        End If


        Return DT_Elenco_Note

    End Function

    'legge le note inserite
    Public Function LeggiProfilazioneNote(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal FiltraVeg_CodAncheSeZero As Integer,
                                                ByVal LeggiSoloNoteConLavorazioneNullSeLAv_CodZero As Boolean,
                                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim DT As DataTable
        Dim objDal As New AgronicaCoreProfilazioneDAL.Profilazione_R

        'devo usare filtro aggiuntivo perche se passo 0 in vegcod comunque non lo considera
        Dim Filtro As String = ""
        If FiltraVeg_CodAncheSeZero Then
            Filtro = "  Veg_Cod= " & Agro_SQL_SaveNum(Veg_Cod) & " "
        End If
        DT = objDal.Leggi(PIVA_isK, 0, Codice_Chiave_isK, "note", Filtro, "", Lav_Cod, 0, LeggiSoloNoteConLavorazioneNullSeLAv_CodZero, objParametri)

        Dim dtNote As New DataTable("note")
        dtNote.Columns.Add("notautilizzo_cod", Type.GetType("System.Int32"))
        dtNote.Columns.Add("nota_cod", Type.GetType("System.Int32"))
        dtNote.Columns.Add("nota_des", Type.GetType("System.String"))
        dtNote.Columns.Add("Lav_cod", Type.GetType("System.Int32"))
        dtNote.Columns.Add("Veg_Cod", Type.GetType("System.Int32"))



        If (DT.Rows.Count > 0) Then

            'per ogni riga della tabella cerco di ottenere i codici
            For Each dr As DataRow In DT.Rows

                Dim chr() As Char = {","}

                'PATH DI SALVATAGGIO es: 
                'notautilizzo_cod=107|nota_cod={15,4}

                Dim dati() As String = dr("Valore_Salvato").ToString().Split("|")
                If IsNumeric(dati(0).Split("=")(1)) Then
                    Dim nota_util_cod As Integer = CInt(dati(0).Split("=")(1))
                    'cero ci valorizzare gli array
                    Dim c As Integer
                    'al primo posto ho sempre il codice utilizzo, che ho già preso
                    For c = 1 To dati.Length - 1
                        Dim codici() As String = dati(c).Substring(dati(c).IndexOf("{") + 1).Replace("}", "").Split(chr)
                        'elenco delle note
                        For Each codice As String In codici
                            If (IsNumeric(codice)) Then
                                Dim drNota As DataRow = dtNote.NewRow()
                                drNota("notautilizzo_cod") = nota_util_cod
                                drNota("nota_cod") = CInt(codice)
                                'ottengo la descrizione, ho le colonne nel datatable
                                Dim oNo As New Note_Intervento_R
                                drNota("nota_des") = oNo.NotaDesFromNotaCod(CInt(codice), objParametri)

                                If IsDBNull(dr.Item("Lav_cod")) Then
                                    drNota("Lav_cod") = 0
                                Else
                                    drNota("Lav_cod") = dr.Item("Lav_cod")
                                End If

                                drNota("Veg_cod") = dr.Item("Veg_cod")

                                'aggiungo la riga al dt
                                dtNote.Rows.Add(drNota)
                            End If
                        Next
                    Next
                End If
            Next
        End If

        Return dtNote


    End Function



    'legge le macchine inserite
    'legge e da la precedenza a:
    'macchine azienda per operazione
    'macchine azienda generiche
    'macchine globali per operazione
    'macchine globali generiche
    'Codice_Chiave_isK = lav_cod per le macchine
    Public Function LeggiProfilazioneMacchine_In_Cascata(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal Veg_Cod As Integer,
                                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataSet

        Dim DT_Elenco_Note As DataSet


        'Azienda - Lav_Cod - Veg_Cod
        DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione(PIVA_isK, Codice_Chiave_isK, Veg_Cod, True, objParametri)

        'Azienda - Lav_Cod - 0
        If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
            DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione(PIVA_isK, Codice_Chiave_isK, 0, True, objParametri)
        End If

        ' ''Azienda - 0 - Veg_Cod
        ''If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
        ''    DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione(PIVA_isK, 0, Veg_Cod, objParametri)
        ''End If

        ' ''Azienda - 0 - 0
        ''If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
        ''    DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione(PIVA_isK, 0, 0, objParametri)
        ''End If

        'Globale -  Lav_Cod - Veg_Cod
        If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
            DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione("", Codice_Chiave_isK, Veg_Cod, True, objParametri)
        End If

        'Globale - Lav_Cod -0
        If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
            DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione("", Codice_Chiave_isK, 0, True, objParametri)
        End If

        ' ''Globale - 0 - Veg_Cod
        ''If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
        ''    DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione("", 0, Veg_Cod, objParametri)
        ''End If

        ' ''Globale - 0 - 0
        ''If IsNothing(DT_Elenco_Note) Or (DT_Elenco_Note.Tables.Count < 2) Or (DT_Elenco_Note.Tables.Count = 2 AndAlso DT_Elenco_Note.Tables(0).Rows.Count = 0 AndAlso DT_Elenco_Note.Tables(1).Rows.Count = 0) Then
        ''    DT_Elenco_Note = LeggiProfilazioneMacchine_ContattiXCodiceLavorazione("", 0, 0, objParametri)
        ''End If

        Return DT_Elenco_Note
    End Function


    'legge i dati in base ai valori, nel campo codice chiave avrò il lav cod
    'il patter per ora gestito è lav_cod=<codice lavorazione>|mac_cod={<codice macchina 1>,<codice macchina 2>,<codice macchina n>}
    Public Function LeggiProfilazioneMacchine_ContattiXCodiceLavorazione(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal Veg_Cod As Integer,
                                                ByVal FiltraVeg_CodAncheSeZero As Integer,
                                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataSet


        Dim DT As DataTable
        Dim objDal As New AgronicaCoreProfilazioneDAL.Profilazione_R

        'devo usare filtro aggiuntivo perche se passo 0 in vegcod comunque non lo considera
        Dim Filtro As String = ""
        If FiltraVeg_CodAncheSeZero Then
            Filtro = "  Veg_Cod= " & Agro_SQL_SaveNum(Veg_Cod) & " "
        End If
        DT = objDal.Leggi(PIVA_isK, 0, Codice_Chiave_isK, "macXlav", Filtro, "", 0, Veg_Cod, False, objParametri)



        Dim dtMacchine As New DataTable("macXlav")
        dtMacchine.Columns.Add("Id_Profilo_Dati", Type.GetType("System.Int32"))

        dtMacchine.Columns.Add("lav_cod", Type.GetType("System.Int32"))
        dtMacchine.Columns.Add("Veg_cod", Type.GetType("System.Int32"))

        dtMacchine.Columns.Add("mac_cod", Type.GetType("System.Int32"))
        dtMacchine.Columns.Add("mac_des", Type.GetType("System.String"))
        'orario espresso in minuti
        dtMacchine.Columns.Add("minuti", Type.GetType("System.Int32"))
        dtMacchine.Columns.Add("ore", Type.GetType("System.Int32"))

        Dim dtContatti As New DataTable("contXlav")
        dtContatti.Columns.Add("Id_Profilo_Dati", Type.GetType("System.Int32"))

        dtContatti.Columns.Add("lav_cod", Type.GetType("System.Int32"))
        dtContatti.Columns.Add("veg_cod", Type.GetType("System.Int32"))

        dtContatti.Columns.Add("cod_risum", Type.GetType("System.Int32"))
        dtContatti.Columns.Add("rag_soc", Type.GetType("System.String"))
        dtContatti.Columns.Add("nome", Type.GetType("System.String"))
        dtContatti.Columns.Add("cognome", Type.GetType("System.String"))
        dtContatti.Columns.Add("rapporto_des", Type.GetType("System.String"))
        dtContatti.Columns.Add("minuti", Type.GetType("System.Int32"))
        dtContatti.Columns.Add("ore", Type.GetType("System.Int32"))


        'aggiungo le tabelle
        Dim ds As New DataSet
        ds.Tables.Add(dtMacchine)
        ds.Tables.Add(dtContatti)


        If (DT.Rows.Count > 0) Then


            'per ogni riga della tabella cerco di ottenere i codici
            For Each dr As DataRow In DT.Rows

                Dim chr() As Char = {","}

                'prendo l'elenco degli eventuali mac_cod o lav_cod...
                'l'array almeno sarà lungo 2 (per forza ci devono essere le macchine o i contatti)
                'PATH DI SALVATAGGIO es: 
                'lav_cod=107|mac_cod={15,4}|cod_cont={cvcdgi58l22c469x,brtpla67m02a965f}
                'non è detto che siano contemporaneamente presenti l'elenco delle macchine
                'e dei contatti

                Dim dati() As String = dr("Valore_Salvato").ToString().Split("|")
                Dim oremin() As String = dr("Valore_Salvato").ToString().Split("/")
                'preno il lav cod...
                Dim lav_cod As Integer = CInt(dati(0).Split("|")(0).Split("=")(1))

                Dim Id_Profilo_Dati As Integer = dr("Id_Profilo_Dati")

                'Dim drNewData As DataRow = dtCod.NewRow()
                'drNewData("lav_cod") = Int32.Parse(lav_cod)



                'cero ci valorizzare gli array
                Dim c As Integer
                'al primo posto ho sempre il lav_cod, che ho già preso
                For c = 1 To dati.Length - 1
                    Dim codici() As String = dati(c).Substring(dati(c).IndexOf("{") + 1).Replace("}", "").Split(chr)

                    If (dati(c).StartsWith("mac_cod")) Then
                        'elenco delle macchine
                        For Each codice As String In codici
                            If (Not codice.Equals("")) Then
                                Dim drM As DataRow = dtMacchine.NewRow()

                                drM("Id_Profilo_Dati") = Id_Profilo_Dati

                                drM("lav_cod") = lav_cod

                                ''controllo se ci sono i minuti
                                If (codice.IndexOf("/") > -1) Then
                                    Dim cod_min() As String = codice.Split("/")
                                    drM("mac_cod") = CInt(cod_min(0))
                                    'drM("minuti") = CInt(cod_min(1))
                                Else
                                    drM("mac_cod") = CInt(codice)
                                    'drM("minuti") = 0
                                End If
                                drM("Ore") = 0
                                drM("Minuti") = 0
                                If oremin.Length > 1 Then
                                    Dim orMin() As String
                                    orMin = oremin(1).Split(":")
                                    drM("Ore") = orMin(0)
                                    drM("Minuti") = orMin(1)
                                End If

                                drM("Veg_Cod") = dr("Veg_Cod")

                                'ottengo la descrizione, ho le colonne nel datatable
                                Dim oM As New Parco_Macchine_R
                                drM("mac_des") = oM.MacchinaDes_from_MacchinaCod(PIVA_isK, CInt(drM("mac_cod")), objParametri)

                                'aggiungo la riga al dt
                                dtMacchine.Rows.Add(drM)
                            End If
                        Next

                    ElseIf (dati(c).StartsWith("cod_cont")) Then
                        'elenco dei contatti
                        For Each codice As String In codici
                            If (Not codice.Equals("")) Then

                                Dim drC As DataRow = dtContatti.NewRow()

                                drC("Id_Profilo_Dati") = Id_Profilo_Dati

                                'ottengo la descrizione...
                                Dim oC As New Contatti_R

                                'controllo se ci sono i minuti
                                If (codice.IndexOf(":") > -1) Then
                                    Dim cod_min() As String = codice.Split(":")
                                    drC("cod_risum") = cod_min(0).Split("/")(0)
                                    ' drC("minuti") = CInt(cod_min(1))
                                Else
                                    If IsNumeric(codice) Then
                                        drC("cod_risum") = codice
                                    Else
                                        Exit For
                                    End If
                                    'drC("minuti") = 0
                                End If

                                drC("Ore") = 0
                                drC("Minuti") = 0
                                If oremin.Length > 1 Then
                                    Dim orMin() As String
                                    orMin = oremin(1).Split("/")
                                    drC("Ore") = orMin(0).Split(":")(0)

                                    drC("Minuti") = orMin(0).Split(":")(1)
                                End If

                                drC("Veg_Cod") = dr("Veg_Cod")

                                'dovrei ottenere sempre una riga sola
                                Dim dtDes As DataTable = oC.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(CStr(drC("cod_risum")), objParametri)
                                If (dtDes.Rows.Count > 0) Then

                                    drC("lav_cod") = lav_cod
                                    drC("nome") = dtDes.Rows(0)("nome")
                                    drC("cognome") = dtDes.Rows(0)("cognome")
                                    drC("rag_soc") = dtDes.Rows(0)("rag_soc")
                                    drC("rapporto_des") = dtDes.Rows(0)("rapporto_des")
                                    'aggiungo la riga al dt
                                    dtContatti.Rows.Add(drC)
                                End If


                            End If
                        Next

                    End If
                Next

            Next

        End If

        Return ds


    End Function

    'legge le note inserite
    Public Function LeggiProfilazioneXRilievi(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Personalizzati As Boolean,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim Filtro As String = ""
        If Veg_Cod <> 0 Then
            Filtro = "  Veg_Cod= " & Agro_SQL_SaveNum(Veg_Cod) & " "
        End If

        Dim objDal As New AgronicaCoreProfilazioneDAL.Profilazione_R
        Dim DT As DataTable = objDal.Leggi(PIVA_isK, 0, Codice_Chiave_isK, "presetXril", Filtro, "", Lav_Cod, 0, False, objParametri)

        Dim dtPreset As New DataTable()
        dtPreset.Columns.Add("ind_mat_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("av_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("av_gru", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("udm_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("Lav_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("Veg_Cod", Type.GetType("System.Int32"))
        'dtPreset.Columns.Add("cod_ss", Type.GetType("System.Int32"))
        'dtPreset.Columns.Add("id_bbch", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("ff_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("dr_cod", Type.GetType("System.Int32"))
        dtPreset.Columns.Add("Descrizione", Type.GetType("System.String"))

        'per ogni riga della tabella cerco di ottenere i codici
        For Each dr As DataRow In DT.Rows

            'PATH DI SALVATAGGIO es: 
            'lav_cod=109|cod={11|1,12|1004} RILIEVO_INDICI_MATURITA RILIEVO_INDICI_RESE_RACCOLTA
            'lav_cod=113|cod={107|0|84,138|0|80,32|0|13} RILIEVO_AVVERSITA_CAMPO RILIEVO_ERBE_INFESTANTI
            'lav_cod=79|cod={1150,1155} LAVCOD_FASI_FENOLOGICHE

            'lav_cod=5007|cod={109$11|1,109$12|1004,79$1150,79$1155,32|0|13} LAVCOD_VISITA

            Dim val As String = dr("Valore_Salvato")

            Dim codici() As String = val.Substring(val.IndexOf("{") + 1).Replace("}", "").Split(",")
            For Each codice As String In codici

                Dim drPreset As DataRow = dtPreset.NewRow()
                drPreset("Lav_cod") = dr("Lav_cod")
                drPreset("Veg_Cod") = dr("Veg_Cod")

                Select Case dr("Lav_cod")
                    Case CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA, CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                        drPreset("ind_mat_cod") = codice.Split("|")(0)
                        drPreset("udm_cod") = codice.Split("|")(1)

                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = LeggiIndiciMaturitaDaWS(Veg_Cod, Personalizzati, objParametri, objParametri_Super_Server)
                        Dim lista = objParametriUscita.ListaIndiciMaturita
                        drPreset("Descrizione") = (From a In lista Where a.ind_mat_cod = drPreset("ind_mat_cod") AndAlso a.udm_cod = drPreset("udm_cod") Select a.ind_mat_des & " (" & a.udm_des & ")").FirstOrDefault()

                    Case CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE
                        'drPreset("cod_ss") = codice.Split("|")(0)
                        'drPreset("id_bbch") = codice.Split("|")(1)
                        drPreset("ff_cod") = codice.Split("|")(0) 'cod_ss

                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = LeggiFasiFenologicheDaWS(Veg_Cod, False, Personalizzati, objParametri, objParametri_Super_Server)
                        Dim lista = objParametriUscita.ListaFasiFenologiche
                        'drPreset("Descrizione") = (From a In lista Where a.Cod_SS = drPreset("Cod_SS") AndAlso a.ID_BBCH = drPreset("ID_BBCH") AndAlso a.FF_Cod = drPreset("FF_Cod") Select a.Descrizione).FirstOrDefault()
                        drPreset("Descrizione") = (From a In lista Where a.Cod_SS = drPreset("ff_cod") Select a.Descrizione).FirstOrDefault()

                    Case CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO, CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI
                        drPreset("av_cod") = codice.Split("|")(0)
                        drPreset("av_gru") = codice.Split("|")(1)
                        drPreset("udm_cod") = codice.Split("|")(2)

                        Dim tipoTestata As Integer
                        Select Case dr("Lav_cod")
                            Case CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                tipoTestata = 0
                            Case CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI
                                tipoTestata = 1
                            Case Else
                                Throw New NotImplementedException
                        End Select

                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = LeggiMisureXAvversitaDaWS(Veg_Cod, 0, 0, 0, tipoTestata, Personalizzati, objParametri, objParametri_Super_Server)
                        Dim lista = objParametriUscita.ListaMisureXAvversita
                        drPreset("Descrizione") = (From a In lista Where a.Av_Cod = drPreset("av_cod") AndAlso a.Av_Gru = drPreset("av_gru") AndAlso a.Udm_Cod = drPreset("udm_cod") Select a.Av_Des_Vol & " (" & a.Udm_Des & ")").FirstOrDefault()

                    Case CostantiPersonalizzate.LAVCOD_DANNI_RACCOLTA
                        drPreset("dr_cod") = codice.Split("|")(0)
                        drPreset("udm_cod") = codice.Split("|")(1)

                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = LeggiDanniRaccoltaDaWS(Veg_Cod, Personalizzati, objParametri, objParametri_Super_Server)
                        Dim lista = objParametriUscita.ListaMisureXDanniRaccolta
                        drPreset("Descrizione") = (From a In lista Where a.Dr_Cod = drPreset("dr_cod") AndAlso a.Udm_Cod = drPreset("udm_cod") Select a.Dr_Des & " (" & a.Udm_Des & ")").FirstOrDefault()

                    Case CostantiPersonalizzate.LAVCOD_VISITA
                        drPreset("Lav_cod") = codice.Split("$")(0)
                        Dim cod As String = codice.Split("$")(1)

                        Select Case drPreset("Lav_cod")
                            Case CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA, CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                drPreset("ind_mat_cod") = cod.Split("|")(0)
                                drPreset("udm_cod") = cod.Split("|")(1)

                                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = LeggiIndiciMaturitaDaWS(Veg_Cod, Personalizzati, objParametri, objParametri_Super_Server)
                                Dim lista = objParametriUscita.ListaIndiciMaturita
                                drPreset("Descrizione") = (From a In lista Where a.ind_mat_cod = drPreset("ind_mat_cod") AndAlso a.udm_cod = drPreset("udm_cod") Select a.ind_mat_des & " (" & a.udm_des & ")").FirstOrDefault()

                            Case CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO, CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI
                                drPreset("av_cod") = cod.Split("|")(0)
                                drPreset("av_gru") = cod.Split("|")(1)
                                drPreset("udm_cod") = cod.Split("|")(2)

                                Dim tipoTestata As Integer
                                Select Case drPreset("Lav_cod")
                                    Case CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                        tipoTestata = 0
                                    Case CostantiPersonalizzate.LAVCOD_RILIEVO_ERBE_INFESTANTI
                                        tipoTestata = 1
                                    Case Else
                                        Throw New NotImplementedException
                                End Select

                                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = LeggiMisureXAvversitaDaWS(Veg_Cod, 0, 0, 0, tipoTestata, Personalizzati, objParametri, objParametri_Super_Server)
                                Dim lista = objParametriUscita.ListaMisureXAvversita
                                drPreset("Descrizione") = (From a In lista Where a.Av_Cod = drPreset("av_cod") AndAlso a.Av_Gru = drPreset("av_gru") AndAlso a.Udm_Cod = drPreset("udm_cod") Select a.Av_Des_Vol & " (" & a.Udm_Des & ")").FirstOrDefault()

                            Case CostantiPersonalizzate.LAVCOD_FASI_FENOLOGICHE
                                drPreset("ff_cod") = cod.Split("|")(0) 'cod_ss

                                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = LeggiFasiFenologicheDaWS(Veg_Cod, False, Personalizzati, objParametri, objParametri_Super_Server)
                                Dim lista = objParametriUscita.ListaFasiFenologiche
                                'drPreset("Descrizione") = (From a In lista Where a.Cod_SS = drPreset("Cod_SS") AndAlso a.ID_BBCH = drPreset("ID_BBCH") AndAlso a.FF_Cod = drPreset("FF_Cod") Select a.Descrizione).FirstOrDefault()
                                drPreset("Descrizione") = (From a In lista Where a.Cod_SS = drPreset("ff_cod") Select a.Descrizione).FirstOrDefault()


                        End Select

                End Select

                dtPreset.Rows.Add(drPreset)

            Next

        Next

        Return dtPreset

    End Function

    Public Function LeggiProfilazioneXRilievi_In_Cascata(ByVal PIVA_isK As String,
                                            ByVal Codice_Chiave_isK As String,
                                            ByVal Lav_Cod As Integer,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Personalizzati As Boolean,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        'Azienda - Lav_Cod - Veg_Cod
        Dim DT As DataTable = LeggiProfilazioneXRilievi(PIVA_isK, Codice_Chiave_isK, Lav_Cod, Veg_Cod, Personalizzati, objParametri, objParametri_Super_Server)

        'Globale - Lav_Cod - Veg_Cod
        If IsNothing(DT) OrElse DT.Rows.Count = 0 Then
            DT = LeggiProfilazioneXRilievi("", Codice_Chiave_isK, Lav_Cod, Veg_Cod, Personalizzati, objParametri, objParametri_Super_Server)
        End If

        Return DT

    End Function

    Private Function LeggiIndiciMaturitaDaWS(veg_cod As Integer, mostraPersonalizzati As Boolean,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.IndiciMaturita_input()
        objParametriIngresso.veg_cod = veg_cod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If mostraPersonalizzati Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/IndiciMaturita"
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim objWS As New AgronicaCoreWebService.IndiciMaturita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = objWS.IndiciMaturita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiMisureXAvversitaDaWS(vegCod As Integer, dpiCod As Integer, idRcdpi As Integer, pubblicoPrivato As Integer,
                                                    tipoTestata As Integer, mostraPersonalizzati As Boolean,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output


        'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_input
        objParametriIngresso.Veg_Cod = vegCod
        objParametriIngresso.Dpi_Cod = dpiCod
        objParametriIngresso.Id_Rcdpi = idRcdpi
        objParametriIngresso.Dpi_Pubblico_Privato = pubblicoPrivato
        objParametriIngresso.TipoTestata = tipoTestata

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If mostraPersonalizzati Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/MisureXAvversita"

        Dim objWS As New AgronicaCoreWebService.MisureXAvversita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = objWS.MisureXAvversita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiDanniRaccoltaDaWS(vegCod As Integer, mostraPersonalizzati As Boolean,
                                        objParametri_Server As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_input
        objParametriIngresso.Veg_Cod = vegCod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If mostraPersonalizzati Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/MisureXDanniRaccolta"

        Dim objWS As New AgronicaCoreWebService.MisureXDanniRaccolta_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = objWS.MisureXDanniRaccolta(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiFasiFenologicheDaWS(vegCod As Integer, fasiOld As Boolean, mostraPersonalizzati As Boolean,
                                          objParametri_Server As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        objParametriIngresso.Veg_Cod = vegCod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

        If mostraPersonalizzati Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        If fasiOld = False Then
            objParametriIngresso.Url = url & "/FasiFenologiche"
            objParametriUscita = objFasi_WS.FasiFenologiche(objParametriIngresso)
        Else
            objParametriIngresso.Url = url & "/FasiFenologiche_OLD"
            objParametriIngresso.strOrdinamento = "Progressivo ASC"
            objParametriUscita = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
        End If

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Public Function CaricaRisorseMacchine_QdC(ByVal Piva As String,
                                              ByVal Data As Date,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim i As Integer = 0
        Dim objDTableParcoMacchine As New DataTable
        Dim Dr As DataRow

        Dim dtClass_Code As New DataTable

        objDTableParcoMacchine.Columns.Add("Piva", GetType(String))

        objDTableParcoMacchine.Columns.Add("Modello", GetType(String))

        objDTableParcoMacchine.Columns.Add("Marca", GetType(AgronicaCoreModelsSTD.metaschema.DittaMacchina))

        objDTableParcoMacchine.Columns.Add("Data_Scadenza_Taratura", GetType(Date))

        objDTableParcoMacchine.Columns.Add("Taratura_Ugello", GetType(Double))

        objDTableParcoMacchine.Columns.Add("Mac_Cod", GetType(Integer))

        objDTableParcoMacchine.Columns.Add("Mac_Des", GetType(String))

        objDTableParcoMacchine.Columns.Add("Risorsa_Cod", GetType(String))

        objDTableParcoMacchine.Columns.Add("Risorsa_Des", GetType(String))

        objDTableParcoMacchine.Columns.Add("Tipo", GetType(AgronicaCoreModelsSTD.metaschema.Macchine))

        objDTableParcoMacchine.Columns.Add("Dettaglio_1", GetType(AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1))

        objDTableParcoMacchine.Columns.Add("Dettaglio_2", GetType(AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2))

        objDTableParcoMacchine.Columns.Add("Validita", GetType(AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale))

        Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim Dt_Macchine As DataTable = objPM.LeggiMacchine_xCostiAccessori(Piva,
                                                                           Data,
                                                                           "", "", objParametri_Server)
        objPM = Nothing



        For i = 0 To Dt_Macchine.Rows.Count - 1

            Dr = objDTableParcoMacchine.NewRow()

            Dim Validita_Taratura_Fine = AGRODATAFINE

            If Not IsDBNull(Dt_Macchine.Rows(i).Item("Validita_Taratura_Fine")) Then
                Validita_Taratura_Fine = Dt_Macchine.Rows(i).Item("Validita_Taratura_Fine")
            End If

            Dr.Item("Risorsa_Cod") = -1 & "*" &
                                                MACCHINE & "*" &
                                                0 & "*" &
                                                Dt_Macchine.Rows(i).Item("Col_8") & "*" &
                                                0

            Dr.Item("Risorsa_Des") = GetDescrizioneMacchina(Dt_Macchine.Rows(i).Item("Col_0"), Dt_Macchine.Rows(i).Item("Col_1"), Dt_Macchine.Rows(i).Item("Ditta_Des"),
                                                                        Dt_Macchine.Rows(i).Item("Modello"), Validita_Taratura_Fine)

            Dr.Item("Piva") = Dt_Macchine.Rows(i).Item("Piva")

            Dr.Item("Modello") = Dt_Macchine.Rows(i).Item("Modello")

            Dr.Item("Marca") = New AgronicaCoreModelsSTD.metaschema.DittaMacchina With {
                        .codice = Dt_Macchine.Rows(i).Item("Ditta_Cod"),
                        .descrizione = Dt_Macchine.Rows(i).Item("Ditta_Des")
                        }

            Dr.Item("Data_Scadenza_Taratura") = CDate(Validita_Taratura_Fine)

            Dr.Item("Mac_Cod") = Dt_Macchine.Rows(i).Item("Col_8")

            Dr.Item("Mac_Des") = Dt_Macchine.Rows(i).Item("Col_1")

            Dr.Item("Taratura_Ugello") = Dt_Macchine.Rows(i).Item("Taratura_Ugello")

            Dr.Item("Tipo") = New AgronicaCoreModelsSTD.metaschema.Macchine With {
                                .codice = "",
                                .descrizione = ""
                    }

            Dr.Item("Dettaglio_1") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1 With {
                                .codice = "",
                                .descrizione = ""
                    }

            Dr.Item("Dettaglio_2") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2 With {
                                .codice = "",
                                .descrizione = ""
                    }


            Dim Class_Code_Split = Dt_Macchine.Rows(i).Item("Class_Code").ToString().Split(".")

            If Not IsNothing(Class_Code_Split) AndAlso Class_Code_Split.Length > 0 Then

                If IsNothing(dtClass_Code) OrElse dtClass_Code.Rows.Count = 0 Then
                    Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R

                    dtClass_Code = coreM.Leggi("", objParametri_Server)
                End If


                If Class_Code_Split.Length >= 1 Then

                    Dim Tipo = Class_Code_Split(0)

                    Dr.Item("Tipo") = New AgronicaCoreModelsSTD.metaschema.Macchine With {
                                    .codice = Tipo,
                                    .descrizione = dtClass_Code.Select("class_code = '" & Tipo & "'")(0)("class_desc")
                                }
                End If


                If Class_Code_Split.Length >= 2 Then

                    Dim Dettaglio_1 = Class_Code_Split(0) & "." & Class_Code_Split(1)

                    Dr.Item("Dettaglio_1") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1 With {
                                .codice = Dettaglio_1,
                                .descrizione = dtClass_Code.Select("class_code = '" & Dettaglio_1 & "'")(0)("class_desc")
                            }
                End If

                If Class_Code_Split.Length >= 3 Then

                    Dim Dettaglio_2 = Class_Code_Split(0) & "." & Class_Code_Split(1) & "." & Class_Code_Split(2)

                    Dr.Item("Dettaglio_2") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2 With {
                                .codice = Dettaglio_2,
                                .descrizione = dtClass_Code.Select("class_code = '" & Dettaglio_2 & "'")(0)("class_desc")
                            }
                End If

            End If

            Dr.Item("Validita") = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale With {
                                .inizio = CDate(Dt_Macchine.Rows(i).Item("Validita_Inizio")),
                                .fine = CDate(Dt_Macchine.Rows(i).Item("Validita_Fine"))
                    }

            objDTableParcoMacchine.Rows.Add(Dr)

        Next

        Return objDTableParcoMacchine

    End Function


    Public Function CaricaRisorseOperatori_QdC(ByVal Piva As String,
                                              ByVal Data As Date,
                                              ByVal objParametri_Server As AgronicaCoreParametri)

        Dim i As Integer = 0
        Dim objDTableOperatori As New DataTable
        Dim Dr As DataRow

        objDTableOperatori.Columns.Add("Piva", GetType(String))

        objDTableOperatori.Columns.Add("Cod_Contatto", GetType(String))

        objDTableOperatori.Columns.Add("Cod_RisUm", GetType(Integer))

        objDTableOperatori.Columns.Add("Risorsa_Cod", GetType(String))

        objDTableOperatori.Columns.Add("Risorsa_Des", GetType(String))

        objDTableOperatori.Columns.Add("Cod_Rapporto", GetType(Integer))

        objDTableOperatori.Columns.Add("Rapporto_Des", GetType(String))

        objDTableOperatori.Columns.Add("Nome", GetType(String))

        objDTableOperatori.Columns.Add("Cognome", GetType(String))

        objDTableOperatori.Columns.Add("Data_Scadenza_Patentino", GetType(Date))

        Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R


        Dim Str As String = " (( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6, -5) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1 or Rapporti_Contabili.Terzista=1) " & vbCrLf &
                                " AND Risorse_Umane.Validita_Inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Data) & " AND Risorse_Umane.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Data) & " "

        Dim Dt_Manodopera As DataTable = objRapp_Contabili.RapportiContabilixCostiAccessori_Flag_Data_Scadenza_Patentino(Piva,
                                                                                                                         False,
                                                                                                                        False,
                                                                                                                         Data,
                                                                                                                         True,
                                                                                                                          Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)


        For i = 0 To Dt_Manodopera.Rows.Count - 1

            Dr = objDTableOperatori.NewRow()

            Dim Data_Scadenza_Patentino = AGRODATAFINE

            If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Data_Scadenza_Patentino")) Then
                Data_Scadenza_Patentino = Dt_Manodopera.Rows(i).Item("Data_Scadenza_Patentino")
            End If

            Dim Descr_Data_Scadenza_Patentino As String = ""

            'Mostro la data Scadenza solo se è diversa da AGRODATAFINE
            If Not IsDBNull(Data_Scadenza_Patentino) AndAlso CDate(Data_Scadenza_Patentino) <> AGRODATAFINE Then
                Descr_Data_Scadenza_Patentino = " - Data Scad. Patentino: " & CDate(Data_Scadenza_Patentino).ToShortDateString()
            End If

            Dim Descr As String = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des"))

            'Se c'è aggiungo anche il progressivo
            If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Settore_Des")) AndAlso CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() <> "" Then
                Descr &= " (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() & ")"
            End If

            Dr.Item("Risorsa_Cod") = -2 & "*" &
                                                ELEMCOD_MANODOPERA & "*" &
                                                0 & "*" &
                                                Dt_Manodopera.Rows(i).Item("Cod_RisUm") & "*" &
                                                CStr(Dt_Manodopera.Rows(i).Item("Cod_Contatto")) & "*" &
                                                CStr(Dt_Manodopera.Rows(i).Item("Piva"))

            Dr.Item("Risorsa_Des") = Descr & " - " & Dt_Manodopera.Rows(i).Item("Rag_Soc") & Descr_Data_Scadenza_Patentino

            Dr.Item("Piva") = Dt_Manodopera.Rows(i).Item("Piva")

            Dr.Item("Cod_RisUm") = Dt_Manodopera.Rows(i).Item("Cod_RisUm")

            Dr.Item("Cod_Contatto") = Dt_Manodopera.Rows(i).Item("Cod_Contatto")

            Dr.Item("Cod_Rapporto") = Dt_Manodopera.Rows(i).Item("Cod_Rapporto")

            Dr.Item("Rapporto_Des") = Dt_Manodopera.Rows(i).Item("Rapporto_Des")

            Dr.Item("Nome") = Dt_Manodopera.Rows(i).Item("Nome")

            Dr.Item("Cognome") = Dt_Manodopera.Rows(i).Item("Cognome")

            Dr.Item("Data_Scadenza_Patentino") = Data_Scadenza_Patentino

            objDTableOperatori.Rows.Add(Dr)

        Next

        Return objDTableOperatori

    End Function

    Public Function CaricaDefaultMacchine(ByVal Piva As String,
                                          ByVal Lav_Cod_List As List(Of Integer),
                                          ByVal Veg_cod As Integer,
                                          ByVal Data As Date,
                                          ByVal objParametri_Server As AgronicaCoreParametri) As DataTable


        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R


        'Creo le liste con i codici delle macchine evitando doppioni
        Dim listaMacCod As New List(Of String)

        For Each Lav_Cod In Lav_Cod_List
            Dim DS_Macchine As DataSet = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(Piva,
                                                                            Lav_Cod, Veg_cod,
                                                                            objParametri_Server)

            For Each dtDati As DataTable In DS_Macchine.Tables
                For Each drDati In dtDati.Rows
                    If (dtDati.TableName.Equals("macXlav")) Then
                        'cerco il check e lo seleziono
                        If Not listaMacCod.Contains(drDati("mac_cod")) Then
                            listaMacCod.Add(drDati("mac_cod"))
                        End If
                    End If
                Next
            Next

        Next


        Dim dtDefault As New DataTable

        dtDefault.Columns.Add("Piva", GetType(String))

        dtDefault.Columns.Add("Modello", GetType(String))

        dtDefault.Columns.Add("Marca", GetType(AgronicaCoreModelsSTD.metaschema.DittaMacchina))

        dtDefault.Columns.Add("Data_Scadenza_Taratura", GetType(Date))

        dtDefault.Columns.Add("Taratura_Ugello", GetType(Double))

        dtDefault.Columns.Add("Mac_Cod", GetType(Integer))

        dtDefault.Columns.Add("Mac_Des", GetType(String))

        dtDefault.Columns.Add("Risorsa_Cod", GetType(String))

        dtDefault.Columns.Add("Risorsa_Des", GetType(String))

        dtDefault.Columns.Add("Tipo", GetType(AgronicaCoreModelsSTD.metaschema.Macchine))

        dtDefault.Columns.Add("Dettaglio_1", GetType(AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1))

        dtDefault.Columns.Add("Dettaglio_2", GetType(AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2))



        'PARCO MACCHINE
        If Not IsNothing(listaMacCod) AndAlso listaMacCod.Count > 0 Then

            For i = 0 To listaMacCod.Count - 1


                Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                Dim DT = objContab.LeggiMacchine_xCostiAccessori(Piva,
                                                                Data,
                                                                " PM.Mac_Cod=" & listaMacCod(i), "",
                                                                objParametri_Server)



                Dim Piva_Macchina As String = ""
                Dim Categoria_Des As String = ""
                Dim Risorsa_Des As String = ""
                Dim Taratura_Ugello As Decimal = 0
                Dim Descr_Validita_Taratura_Fine As String = ""
                Dim dtClass_Code As New DataTable

                If DT.Rows.Count > 0 Then

                    Piva_Macchina = DT.Rows(0).Item("Piva")

                    Dim Validita_Taratura_Fine As Date = IIf(
                    IsDBNull(DT.Rows(0).Item("Validita_Taratura_Fine")),
                    AGRODATAFINE,
                    DT.Rows(0).Item("Validita_Taratura_Fine"))

                    Categoria_Des = DT.Rows(0).Item("Col_0")
                    Risorsa_Des = String.Join(" - ", {DT.Rows(0).Item("Ditta_Des"), DT.Rows(0).Item("Modello"), DT.Rows(0).Item("Col_1")}.Where(Function(s) (Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s))))


                    If Not IsDBNull(DT.Rows(0).Item("Taratura_Ugello")) Then
                        If IsNumeric(DT.Rows(0).Item("Taratura_Ugello")) Then
                            Taratura_Ugello = DT.Rows(0).Item("Taratura_Ugello")
                        End If
                    End If

                    'Mostro la data Scadenza solo se è diversa da AGRODATAFINE
                    If CDate(Validita_Taratura_Fine) <> AGRODATAFINE Then

                        Descr_Validita_Taratura_Fine = "- Data Scad. Taratura: " & CDate(Validita_Taratura_Fine).ToShortDateString()

                    End If

                    Risorsa_Des = Categoria_Des & " - " & Risorsa_Des & Descr_Validita_Taratura_Fine

                    Dim Dr As DataRow = dtDefault.NewRow

                    Dr.Item("Piva") = Piva_Macchina

                    Dr.Item("Modello") = DT.Rows(0).Item("Modello")

                    Dr.Item("Marca") = New AgronicaCoreModelsSTD.metaschema.DittaMacchina With {
                        .codice = DT.Rows(0).Item("Ditta_Cod"),
                        .descrizione = DT.Rows(0).Item("Ditta_Des")
                        }

                    Dr.Item("Data_Scadenza_Taratura") = CDate(Validita_Taratura_Fine)

                    Dr.Item("Taratura_Ugello") = Taratura_Ugello

                    Dr.Item("Mac_Cod") = listaMacCod(i)

                    Dr.Item("Mac_Des") = DT.Rows(0).Item("Col_1")

                    Dr.Item("Risorsa_Cod") = -1 & "*" &
                                                    MACCHINE & "*" &
                                                    0 & "*" &
                                                    listaMacCod(i) & "*" &
                                                    0

                    Dr.Item("Risorsa_Des") = Risorsa_Des

                    Dr.Item("Tipo") = New AgronicaCoreModelsSTD.metaschema.Macchine With {
                                .codice = "",
                                .descrizione = ""
                    }

                    Dr.Item("Dettaglio_1") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1 With {
                                .codice = "",
                                .descrizione = ""
                    }

                    Dr.Item("Dettaglio_2") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2 With {
                                .codice = "",
                                .descrizione = ""
                    }


                    Dim Class_Code_Split = DT.Rows(0).Item("Class_Code").ToString().Split(".")

                    If Not IsNothing(Class_Code_Split) AndAlso Class_Code_Split.Length > 0 Then

                        If IsNothing(dtClass_Code) OrElse dtClass_Code.Rows.Count = 0 Then
                            Dim coreM As New AgronicaCoreMetaSchemaDAL.Macchine_R

                            dtClass_Code = coreM.Leggi("", objParametri_Server)
                        End If


                        If Class_Code_Split.Length >= 1 Then

                            Dim Tipo = Class_Code_Split(0)

                            Dr.Item("Tipo") = New AgronicaCoreModelsSTD.metaschema.Macchine With {
                                    .codice = Tipo,
                                    .descrizione = dtClass_Code.Select("class_code = '" & Tipo & "'")(0)("class_desc")
                                }
                        End If


                        If Class_Code_Split.Length >= 2 Then

                            Dim Dettaglio_1 = Class_Code_Split(0) & "." & Class_Code_Split(1)

                            Dr.Item("Dettaglio_1") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio1 With {
                                .codice = Dettaglio_1,
                                .descrizione = dtClass_Code.Select("class_code = '" & Dettaglio_1 & "'")(0)("class_desc")
                            }
                        End If

                        If Class_Code_Split.Length >= 3 Then

                            Dim Dettaglio_2 = Class_Code_Split(0) & "." & Class_Code_Split(1) & "." & Class_Code_Split(2)

                            Dr.Item("Dettaglio_2") = New AgronicaCoreModelsSTD.metaschema.MacchineDettaglio2 With {
                                .codice = Dettaglio_2,
                                .descrizione = dtClass_Code.Select("class_code = '" & Dettaglio_2 & "'")(0)("class_desc")
                            }
                        End If

                    End If

                    dtDefault.Rows.Add(Dr)

                End If
            Next

        End If

        Return dtDefault

    End Function

    Public Function CaricaDefaultOperatori(ByVal Piva As String,
                                          ByVal Lav_cod_List As List(Of Integer),
                                          ByVal Veg_cod As Integer,
                                          ByVal Data As Date,
                                          ByVal objParametri_Server As AgronicaCoreParametri) As DataTable


        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R

        'Creo le liste con i codici delle risorse umane evitando doppioni
        Dim listaCodRisum As New List(Of String)

        For Each Lav_Cod In Lav_cod_List
            Dim DS_Macchine As DataSet = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(Piva,
                                                                                     Lav_Cod, Veg_cod,
                                                                                     objParametri_Server)

            For Each dtDati As DataTable In DS_Macchine.Tables
                For Each drDati In dtDati.Rows
                    If (dtDati.TableName.Equals("contXlav")) Then
                        If Not listaCodRisum.Contains(drDati("cod_risum")) Then
                            listaCodRisum.Add(drDati("cod_risum"))
                        End If
                    End If
                Next
            Next
        Next



        Dim dtDefault As New DataTable

        dtDefault.Columns.Add("Piva", GetType(String))

        dtDefault.Columns.Add("Cod_Contatto", GetType(String))

        dtDefault.Columns.Add("Cod_RisUm", GetType(Integer))

        dtDefault.Columns.Add("Risorsa_Cod", GetType(String))

        dtDefault.Columns.Add("Risorsa_Des", GetType(String))

        dtDefault.Columns.Add("Cod_Rapporto", GetType(Integer))

        dtDefault.Columns.Add("Rapporto_Des", GetType(String))

        dtDefault.Columns.Add("Nome", GetType(String))

        dtDefault.Columns.Add("Cognome", GetType(String))

        dtDefault.Columns.Add("Data_Scadenza_Patentino", GetType(Date))


        'RAPPORTI CONTABILI
        If Not IsNothing(listaCodRisum) AndAlso listaCodRisum.Count > 0 Then

            For i = 0 To listaCodRisum.Count - 1

                Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                Dim xFiltroAggiuntivo As String = " ( (Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6, -5) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1 or Rapporti_Contabili.Terzista=1 )  "

                Dim xOrderBy As String = " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  "

                Dim Dt_Manodopera As DataTable = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm_Flag_Data_Scadenza_Patentino(Piva,
                                                                                                                                            listaCodRisum(i),
                                                                                                                                            False,
                                                                                                                                            False,
                                                                                                                                            Data,
                                                                                                                                            True,
                                                                                                                                            xFiltroAggiuntivo,
                                                                                                                                            xOrderBy, objParametri_Server)

                If Dt_Manodopera.Rows.Count = 0 Then
                    'Exit For
                    Continue For
                End If

                Dim Cod_Contatto As String = Dt_Manodopera.Rows(0).Item("Cod_Contatto")
                Dim Piva_Operatore As String = Dt_Manodopera.Rows(0).Item("Piva")
                Dim Categoria_Des As String = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                Dim Risorsa_Des As String = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                Dim Cod_Rapporto As Integer = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

                Dim Data_Scadenza_Patentino = AGRODATAFINE

                If Not IsDBNull(Dt_Manodopera.Rows(0).Item("Data_Scadenza_Patentino")) Then
                    Data_Scadenza_Patentino = Dt_Manodopera.Rows(0).Item("Data_Scadenza_Patentino")
                End If

                Dim Descr_Data_Scadenza_Patentino As String = ""

                'Mostro la data Scadenza solo se è diversa da AGRODATAFINE
                If CDate(Data_Scadenza_Patentino) <> AGRODATAFINE Then
                    Descr_Data_Scadenza_Patentino = " - Data Scad. Patentino: " & CDate(Data_Scadenza_Patentino).ToShortDateString()
                End If

                Risorsa_Des = Categoria_Des & " - " & Risorsa_Des & Descr_Data_Scadenza_Patentino

                Dim Dr As DataRow = dtDefault.NewRow

                Dr.Item("Piva") = Piva_Operatore

                Dr.Item("Cod_Contatto") = Cod_Contatto

                Dr.Item("Cod_RisUm") = listaCodRisum(i)

                Dr.Item("Risorsa_Cod") = -2 & "*" &
                                                ELEMCOD_MANODOPERA & "*" &
                                                0 & "*" &
                                                 listaCodRisum(i) & "*" &
                                                Cod_Contatto & "*" &
                                                Piva_Operatore

                Dr.Item("Risorsa_Des") = Risorsa_Des

                Dr.Item("Cod_Rapporto") = Cod_Rapporto

                Dr.Item("Rapporto_Des") = Categoria_Des

                Dr.Item("Nome") = Dt_Manodopera.Rows(0).Item("Nome")

                Dr.Item("Cognome") = Dt_Manodopera.Rows(0).Item("Cognome")

                Dr.Item("Data_Scadenza_Patentino") = Data_Scadenza_Patentino

                dtDefault.Rows.Add(Dr)
            Next
        End If


        Return dtDefault

    End Function

    Public Function LeggiAziendexUtenti(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT As DataTable
        Dim objDal As New AgronicaCoreProfilazioneDAL.Profilazione_R


        Dim DT_AziendexUtenti = objDal.LeggiAziendexUtenti(objParametri)

        Return DT

    End Function

    Public Function GetDescrizioneMacchina(ByVal Class_Desc As String, ByVal Mac_Desc As String, ByVal Ditta_Des As String,
                                       ByVal Modello As String, ByVal Validita_Fine_Taratura As Date) As String

        Dim descr As String = String.Join(" - ", {Class_Desc, Ditta_Des, Modello, Mac_Desc}.Where(Function(s) (Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s))))

        'Mostro la data Scadenza solo se è diversa da AGRODATAFINE
        If CDate(Validita_Fine_Taratura) <> AGRODATAFINE Then
            descr += " " & String.Format(AgronicaCoreDataProvider.My.Resources.Gias.DataScadenzaTaraturaAbbr, CDate(Validita_Fine_Taratura).ToShortDateString())
        End If

        Return descr
    End Function

End Class






Public Class Profilazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'salva, aggiorna o inserisce...
    Public Function Scrivi_Inserisce_O_Aggiorna(ByVal PIVA_isK As String,
                                                ByVal Codice_Chiave_isK As String,
                                                ByVal IdGruppo_isK As String, ByVal Piva_SuperUser As String,
                                                ByVal Quesito As String, ByVal ValoreSalvato As String,
                                                ByVal validita_inizio As DateTime, ByVal validita_fine As DateTime,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim NomeRoutine As String = "Scrivi_Inserisce_O_Aggiorna"


        Dim DT As DataTable
        Dim objProfilazioneR As New AgronicaCoreProfilazioneDAL.Profilazione_R
        Try

            DT = objProfilazioneR.Leggi(PIVA_isK, 0, Codice_Chiave_isK, IdGruppo_isK, "", "", Lav_Cod, Veg_Cod, True, objParametri)
            Dim objW As New AgronicaCoreProfilazioneDAL.Profilazione_W
            Dim IdProfiloDati_isK As Integer
            If DT.Rows.Count - 1 Then
                'inserisco
                'trovo un nuovo id 
                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                IdProfiloDati_isK = objSeq.NuovoId_Tabella("Profilazione_Dati", 0, 2000000000, objParametri)
                objW.Scrivi(PIVA_isK, IdProfiloDati_isK, Codice_Chiave_isK, Quesito, IdGruppo_isK, ValoreSalvato, validita_inizio, validita_fine, Lav_Cod, Veg_Cod, objParametri)

            Else
                'modifico
                IdProfiloDati_isK = DT.Rows(0).Item("Id_Profilo_Dati")
                objW.Modifica(Quesito, ValoreSalvato, PIVA_isK, IdProfiloDati_isK, Codice_Chiave_isK, IdGruppo_isK, validita_inizio, validita_fine, "", Lav_Cod, Veg_Cod, objParametri)
            End If
            xRisp = True
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Inserisci_Default_CostiAccessori_Profilazione_Dati(ByVal Piva As String,
                                                                       ByVal List_Lav_Cod As List(Of Integer),
                                                                       ByVal List_Mac_Cod As List(Of Integer),
                                                                       ByVal List_Cod_Risum As List(Of Integer),
                                                                       ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xRisp As Boolean
        Dim NomeRoutine As String = "AgronicaCoreProfilazioneBIZ.Profilazione_W.Inserisci_Default_CostiAccessori_Profilazione_Dati()"

        Dim FlagTransazioneLocale_Server As Boolean = False
        Dim FlagConnessioneLocale_Server As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale_Server,
                                                                        FlagTransazioneLocale_Server,
                                                                        objParametri_Server)

            For Each Lav_Cod In List_Lav_Cod

                Dim val2save As String = "lav_cod=" & Lav_Cod

                If List_Mac_Cod.Count > 0 Then
                    val2save += "|mac_cod={" & String.Join(",", List_Mac_Cod) & "}"
                End If

                If List_Cod_Risum.Count > 0 Then
                    val2save += "|cod_cont={" & String.Join(",", List_Cod_Risum) & "}"
                End If


                Dim Risp As Boolean = Scrivi_Inserisce_O_Aggiorna(Piva,
                                                                Lav_Cod,
                                                                "macXlav",
                                                                objParametri_Server.PivaSuperUser,
                                                                "Macchine/Contatti per operazioni", val2save,
                                                                AGRODATAINIZIO, AGRODATAFINE,
                                                                Lav_Cod,
                                                                0,
                                                                objParametri_Server)

            Next

            xRisp = True

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale_Server, objParametri_Server)

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri_Server.objConnessione Is Nothing AndAlso
               Not objParametri_Server.objTransazione Is Nothing AndAlso
               FlagTransazioneLocale_Server = True Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If


            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale_Server, objParametri_Server)

        End Try

        Return xRisp

    End Function

End Class


