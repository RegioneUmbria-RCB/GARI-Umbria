Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.note_intervento
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.metaschema.utilizzi

Public Class STD_Note

#Region "Lettura"

    Public Function LeggiNote(tipoAttivita As Attivita.Tipo_Attivita,
                              meno1tutti_0nonVisibili_1soloVisibili As Integer,
                              objParametri_Server As AgronicaCoreParametri) As List(Of NoteIntervento)

        Dim noteList As New List(Of NoteIntervento)

        Dim DTRisultati As DataTable

        Dim ObjContab As New AgronicaCoreContabDAL.Note_Intervento_R


        Dim NotaUtilizzo_Cod As enum_Note_Intervento_Utilizzo = enum_Note_Intervento_Utilizzo.QuadernoCampagna
        If tipoAttivita = Attivita.Tipo_Attivita.Ricetta Then
            NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
        End If

        DTRisultati = ObjContab.Leggi_con_Utilizzo(Nota_Cod:=0,
                             NotaGruppo_Cod:=0,
                             NotaUtilizzo_Cod:=NotaUtilizzo_Cod,
                             xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             xFiltroAggiuntivo:="",
                             xOrderBy:="",
                             objParametri:=objParametri_Server,
                             meno1tutti_0nonVisibili_1soloVisibili:=meno1tutti_0nonVisibili_1soloVisibili)

        For Each dr As DataRow In DTRisultati.Rows
            Dim nota = New NoteIntervento
            nota.codice = dr.Item("Nota_Cod")
            nota.descrizione = dr.Item("Nota_Des")
            'nota.visibile = dr.Item("Visible")
            nota.noteInterventoGruppi = New NoteInterventoGruppi
            nota.noteInterventoGruppi.codice = dr.Item("NotaGruppo_Cod")
            nota.noteInterventoGruppi.descrizione = dr.Item("NotaGruppo_Des")

            noteList.Add(nota)
        Next

        Return noteList
    End Function

    Public Function LeggiGruppi(objParametri_Server As AgronicaCoreParametri) As List(Of NoteInterventoGruppi)

        Dim gruppiList As New List(Of NoteInterventoGruppi)

        Dim objNoteGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R

        Dim DT_NoteGruppi As DataTable =
            objNoteGruppi_R.Leggi(0,
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    " NotaGruppo_Cod DESC",
                                     objParametri_Server)

        For Each dr As DataRow In DT_NoteGruppi.Rows
            If dr.Item("visibile") = 1 Then
                Dim gruppo = New NoteInterventoGruppi
                gruppo.codice = dr.Item("NotaGruppo_Cod")
                gruppo.descrizione = dr.Item("NotaGruppo_Des")
                gruppo.tipo = dr.Item("Tipo_Gruppo_Note")

                gruppiList.Add(gruppo)
            End If

        Next

        Return gruppiList
    End Function

    Public Function LeggiGruppiConUtilizzo(params As LeggiNote,
                          meno1tutti_0nonVisibili_1soloVisibili As Integer,
                          objParametri_Server As AgronicaCoreParametri) As List(Of NoteInterventoGruppi)

        Dim gruppiList As New List(Of NoteInterventoGruppi)

        Dim DTRisultati As DataTable

        Dim ObjContab As New AgronicaCoreContabDAL.Note_Intervento_R
        Dim objProf As New AgronicaCoreProfilazioneBIZ.Profilazione_R

        Dim NotaUtilizzo_Cod As enum_Note_Intervento_Utilizzo = enum_Note_Intervento_Utilizzo.QuadernoCampagna
        If params.tipoAttivita = Attivita.Tipo_Attivita.Ricetta Then
            NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
        End If

        '-- Leggo le note di default
        Dim DT_ As DataTable
        Dim presets As New HashSet(Of Integer)

        If Not IsNothing(params.lavorazioni) Then
            For Each lavorazione In params.lavorazioni
                DT_ = objProf.LeggiProfilazioneNote_In_Cascata(params.piva, NotaUtilizzo_Cod,
                                                                lavorazione.primaryKey.codice, params.specieVegetale.codice,
                                                                objParametri_Server)
                For Each row In DT_.Rows
                    presets.Add(row(1))
                Next
            Next
        End If


        '-- Leggo tutti i valori delle note
        DTRisultati = ObjContab.Leggi_con_Utilizzo(Nota_Cod:=0,
                             NotaGruppo_Cod:=0,
                             NotaUtilizzo_Cod:=NotaUtilizzo_Cod,
                             xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             xFiltroAggiuntivo:="",
                             xOrderBy:=" NotaGruppo_Cod DESC",
                             objParametri:=objParametri_Server,
                             meno1tutti_0nonVisibili_1soloVisibili:=meno1tutti_0nonVisibili_1soloVisibili)

        Dim gruppiDict As Dictionary(Of String, String) = New Dictionary(Of String, String)

        '-- Controllo le note esitenti e creo i gruppi
        For Each dr As DataRow In DTRisultati.Rows

            If Not gruppiDict.ContainsKey(dr.Item("NotaGruppo_Cod")) Then

                gruppiDict.Add(dr.Item("NotaGruppo_Cod"), "")

                Dim gruppo = New NoteInterventoGruppi
                gruppo.codice = dr.Item("NotaGruppo_Cod")
                gruppo.descrizione = dr.Item("NotaGruppo_Des")
                gruppo.tipo = dr.Item("Tipo_Gruppo_Note")
                gruppo.presets = Array.Empty(Of baseClass.BaseCodeDescr)

                gruppiList.Add(gruppo)
            End If

            '-- Aggiugo i default ai gruppi
            If presets.Contains(dr.Item("Nota_Cod")) Then
                Dim gr = gruppiList.Find(Function(g) (g.codice = dr.Item("NotaGruppo_Cod")))

                If IsNothing(gr) Then
                    Continue For
                End If
                Dim nota = New baseClass.BaseCodeDescr
                nota.codice = dr.Item("Nota_Cod")
                nota.descrizione = dr.Item("Nota_Des")

                gr.presets = gr.presets.Append(nota).ToArray()
            End If
        Next

        Return gruppiList

    End Function

    Public Function LeggiDaSpecieLavorazione(specie As IBaseCodeDescr, lav As Job, objParametri_Server As AgronicaCoreParametri,
                                             Optional piva As String = "0") As IEnumerable(Of NoteIntervento)
        Dim profilation As New AgronicaCoreProfilazioneBIZ.Profilazione_R
        Dim notesKey = "-2"
        Dim vegCod = If(specie Is Nothing, 0, specie.codice)
        Return profilation.LeggiProfilazioneNote_In_Cascata(
            piva, notesKey, lav.getCodice, vegCod, objParametri_Server
        ).Select.
        Select(Function(row) New NoteIntervento(CInt(row("nota_cod")), CStr(row("nota_des"))))
    End Function

#End Region

#Region "Scrittura"

    Public Sub ImpostaPresets(pNote As LeggiNote, params As ObjParams)
        Dim profDataW As New AgronicaCoreProfilazioneBIZ.Profilazione_W
        Dim vegCod = If(pNote.specieVegetale.codice < 0, 0, pNote.specieVegetale.codice)
        Dim noteStr = "note"
        Dim codKey = If(pNote.tipoAttivita = Attivita.Tipo_Attivita.QuadernoDiCampagna, enum_Note_Intervento_Utilizzo.QuadernoCampagna, enum_Note_Intervento_Utilizzo.Ricetta)
        Dim newPresets = pNote.parametriAggiuntivi.Select(Function(t) t.Item2.ToString).
             Aggregate(Function(acc, n) acc & "," & n)
        Dim valStr = "notautilizzo_cod=" & codKey & "|nota_cod={" & newPresets & "}"

        If pNote.lavorazioni.Any Then
            For Each lav In pNote.lavorazioni
                profDataW.Scrivi_Inserisce_O_Aggiorna(
                    pNote.piva, codKey, noteStr,
                    params.ObjParametri_Server.PivaSuperUser, noteStr,
                    valStr,
                    AGRODATAINIZIO, AGRODATAFINE,
                    lav.getCodice, vegCod, params.ObjParametri_Server
                )
            Next
        Else
            profDataW.Scrivi_Inserisce_O_Aggiorna(
               pNote.piva, codKey, noteStr,
               params.ObjParametri_Server.PivaSuperUser, noteStr,
               valStr,
               AGRODATAINIZIO, AGRODATAFINE,
               0, vegCod, params.ObjParametri_Server
           )
        End If
    End Sub

#End Region

End Class
