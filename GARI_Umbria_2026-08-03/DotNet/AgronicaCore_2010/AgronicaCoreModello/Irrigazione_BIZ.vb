Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility
Imports AgronicaCoreContabBIZ
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports Newtonsoft.Json.Linq
Imports System.Web
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.UI.WebControls
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreXML
Imports System.Net.WebRequestMethods
Imports System.Text
Imports AgronicaCoreUtility.RestSharpHelper
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreContabDAL

Public Class Irrigazione_BIZ

#Region "Gestione Tab Div Note"
    Public Function Carica_NoteIrrigazione(ByVal Data As String,
                                           ByVal piva As String,
                                           ByVal Veg_Cod As Integer,
                                           ByVal Lav_Cod As Integer,
                                           ByVal Tipo_Operazione_Agenda As Integer,
                                           ByVal objParametriAgendaNote As String,
                                           ByVal Tipo_Operazione As Integer,
                                           ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard
        Try
            'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(DateTime.ParseExact(Data.ToString, "yyyyMMdd", Nothing),
                                                                        DateTime.ParseExact(Data.ToString, "yyyyMMdd", Nothing))

            Dim NotaUtilizzo_Cod As enum_Note_Intervento_Utilizzo = enum_Note_Intervento_Utilizzo.QuadernoCampagna

            If Not Tipo_Operazione_Agenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
            End If


            Dim CBL_Consigli As New CheckBoxList
            Dim CBL_Meteo As New CheckBoxList
            Dim CBL_VentoIntensita As New CheckBoxList
            Dim CBL_VentoDirezione As New CheckBoxList
            Dim CBL_Temperatura As New CheckBoxList
            Dim CBL_Motivazione As New CheckBoxList
            Dim CBL_Orario As New CheckBoxList


            Dim strFiltro As String = " Note_Intervento.Nota_Cod > 0 "
            Dim GruppoDes As String = ""

            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                                False,
                                                                    "", "",
                                                                    0,
                                                                    NotaUtilizzo_Cod,
                                                                    strFiltro, "",
                                                                    objParametri_Server,
                                                                    1,
                                                                    GruppoDes)

            objParametri_Server.ResettaFinestra()


            Dim objNoteGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R

            Dim DT_NoteGruppi As DataTable =
            objNoteGruppi_R.Leggi(0,
                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    " NotaGruppo_Cod < 0",
                                    "",
                                     objParametri_Server)

            If Not DT_NoteGruppi Is Nothing Then
                For n = 0 To DT_NoteGruppi.Rows.Count - 1
                    Select Case DT_NoteGruppi.Rows(n).Item("NotaGruppo_Cod")

                        Case enum_Note_Intervento_Gruppi.Meteo
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Meteo,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Meteo,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If

                        Case enum_Note_Intervento_Gruppi.Vento_Intensita
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoIntensita,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Vento_Intensita,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If

                        Case enum_Note_Intervento_Gruppi.Vento_Direzione
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoDirezione,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Vento_Direzione,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If
                        Case enum_Note_Intervento_Gruppi.Temperatura
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Temperatura,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Temperatura,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If
                        Case enum_Note_Intervento_Gruppi.Orario
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Orario,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Orario,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If
                        Case enum_Note_Intervento_Gruppi.Motivazione
                            If DT_NoteGruppi.Rows(n).Item("visibile") = 1 Then
                                AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Motivazione,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Motivazione,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                            End If
                    End Select
                Next
            End If

            'Imposto le checkbox selezionate in base all'objParametriAgenda.Note
            Dim objParametriAgendaNoteArray As JArray = JArray.Parse(objParametriAgendaNote)

            If Not objParametriAgendaNoteArray Is Nothing Then

                '-----------------------------------------------------
                'se ho note selezionate le checkko!!!
                '-----------------------------------------------------
                Dim Nota_Cod As Integer
                Dim TrovataNota As Boolean = False

                For i = 0 To objParametriAgendaNoteArray.Count - 1

                    TrovataNota = False

                    Nota_Cod = objParametriAgendaNoteArray(i)("Nota_Cod")

                    Select Case Nota_Cod

                        Case Is > 0 'utente

                            For j = 0 To CBL_Consigli.Items.Count - 1
                                If Nota_Cod = CBL_Consigli.Items(j).Value Then
                                    CBL_Consigli.Items(j).Selected = True
                                    TrovataNota = True
                                    Continue For
                                End If
                            Next

                        Case Is < 0 'standard

                            If CBL_Meteo.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_Meteo.Items.Count - 1
                                    If Nota_Cod = CBL_Meteo.Items(j).Value Then
                                        CBL_Meteo.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If
                            If CBL_VentoIntensita.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_VentoIntensita.Items.Count - 1
                                    If Nota_Cod = CBL_VentoIntensita.Items(j).Value Then
                                        CBL_VentoIntensita.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If
                            If CBL_VentoDirezione.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_VentoDirezione.Items.Count - 1
                                    If Nota_Cod = CBL_VentoDirezione.Items(j).Value Then
                                        CBL_VentoDirezione.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If
                            If CBL_Temperatura.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_Temperatura.Items.Count - 1
                                    If Nota_Cod = CBL_Temperatura.Items(j).Value Then
                                        CBL_Temperatura.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If
                            If CBL_Orario.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_Orario.Items.Count - 1
                                    If Nota_Cod = CBL_Orario.Items(j).Value Then
                                        CBL_Orario.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If
                            If CBL_Motivazione.Visible = True And TrovataNota = False Then
                                For j = 0 To CBL_Motivazione.Items.Count - 1
                                    If Nota_Cod = CBL_Motivazione.Items(j).Value Then
                                        CBL_Motivazione.Items(j).Selected = True
                                        TrovataNota = True
                                        Continue For
                                    End If
                                Next
                            End If

                    End Select

                    'nota salvata in agenda ma resa ora non visibile
                    'se il SUO gruppo è visibile la aggiungo al suo gruppo
                    'se il SUO gruppo è NON visibile la aggiungo alla lista generica consigli
                    'la chekko in ogni caso
                    If TrovataNota = False Then
                        Dim objNota As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R
                        Dim Dt_Nota As DataTable
                        Dim NotaDes As String = ""
                        Dim FiltroNota As String = " NI.Nota_Cod = " & Nota_Cod.ToString
                        Dt_Nota = objNota.LeggiNote_X_Utilizzo_Visibile(0, 0, False, FiltroNota, objParametri_Server)
                        If Not Dt_Nota Is Nothing AndAlso Dt_Nota.Rows.Count > 0 Then
                            NotaDes = Dt_Nota.Rows(0).Item("Nota_Des")
                            Select Case Dt_Nota.Rows(0).Item("NotaGruppo_Cod")
                                Case enum_Note_Intervento_Gruppi.Meteo
                                    CBL_Meteo.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Meteo.Items(CBL_Meteo.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Vento_Intensita
                                    CBL_VentoIntensita.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_VentoIntensita.Items(CBL_VentoIntensita.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Vento_Direzione
                                    CBL_VentoDirezione.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_VentoDirezione.Items(CBL_VentoDirezione.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Temperatura
                                    CBL_Temperatura.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Temperatura.Items(CBL_Temperatura.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Orario
                                    CBL_Orario.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Orario.Items(CBL_Orario.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Motivazione
                                    CBL_Motivazione.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Motivazione.Items(CBL_Motivazione.Items.Count - 1).Selected = True
                                Case Else
                                    CBL_Consigli.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Consigli.Items(CBL_Consigli.Items.Count - 1).Selected = True
                            End Select

                        End If
                    End If

                Next
            End If


            Dim risp As New List(Of Object)

            If GruppoDes <> "" Then
                risp.Add(New With {
                        .Titolo_Tab_Giustificazioni = GruppoDes
                        })
            End If


            DefaultAziendaliIrrigazione(Tipo_Operazione,
                                        Tipo_Operazione_Agenda,
                                        piva,
                                        Lav_Cod,
                                        Veg_Cod,
                                        CBL_Consigli,
                                        CBL_Meteo,
                                        CBL_VentoIntensita,
                                        CBL_VentoDirezione,
                                        CBL_Temperatura,
                                        CBL_Orario,
                                        CBL_Motivazione,
                                        objParametri_Server)

            For j = 0 To CBL_Consigli.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_Consigli.Items(j).Text,
                .Nota_Cod = CBL_Consigli.Items(j).Value,
                .NotaGruppo_Cod = "Giustificazioni",
                .Checkato = If(CBL_Consigli.Items(j).Selected, "true", "false")
             })
            Next

            For j = 0 To CBL_Meteo.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_Meteo.Items(j).Text,
                .Nota_Cod = CBL_Meteo.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Meteo,
                .Checkato = If(CBL_Meteo.Items(j).Selected, "true", "false")
             })
            Next

            For j = 0 To CBL_VentoIntensita.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_VentoIntensita.Items(j).Text,
                .Nota_Cod = CBL_VentoIntensita.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Vento_Intensita,
                .Checkato = If(CBL_VentoIntensita.Items(j).Selected, "true", "false")
             })
            Next

            For j = 0 To CBL_VentoDirezione.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_VentoDirezione.Items(j).Text,
                .Nota_Cod = CBL_VentoDirezione.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Vento_Direzione,
                .Checkato = If(CBL_VentoDirezione.Items(j).Selected, "true", "false")
             })
            Next

            For j = 0 To CBL_Temperatura.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_Temperatura.Items(j).Text,
                .Nota_Cod = CBL_Temperatura.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Temperatura,
                .Checkato = If(CBL_Temperatura.Items(j).Selected, "true", "false")
             })
            Next

            For j = 0 To CBL_Orario.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_Orario.Items(j).Text,
                .Nota_Cod = CBL_Orario.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Orario,
                .Checkato = If(CBL_Orario.Items(j).Selected, "true", "false")
             })
            Next


            For j = 0 To CBL_Motivazione.Items.Count - 1
                risp.Add(New With {
                .Nota_Des = CBL_Motivazione.Items(j).Text,
                .Nota_Cod = CBL_Motivazione.Items(j).Value,
                .NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Motivazione,
                .Checkato = If(CBL_Motivazione.Items(j).Selected, "true", "false")
             })
            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(risp, Newtonsoft.Json.Formatting.None)


            If Not DT_NoteGruppi Is Nothing Then
                r.ParametroDue = True

                'Se la prima tab "Giustificazioni" non contiene checkbox allora la nascondo
                Dim Elenco_Tab As New List(Of Object)

                Elenco_Tab.Add(New With {
                        .NotaGruppo_Cod = "Giustificazioni",
                        .NotaGruppo_Des = "Giustificazioni",
                        .visibile = 0
                        })

                For Each dr As DataRow In DT_NoteGruppi.Rows
                    Elenco_Tab.Add(New With {
                        .NotaGruppo_Cod = dr("NotaGruppo_Cod"),
                        .NotaGruppo_Des = dr("NotaGruppo_Des"),
                        .visibile = dr("visibile")
                        })
                Next

                If CBL_Consigli.Items.Count > 0 Then
                    Elenco_Tab(0).visibile = 1
                End If

                'Nascondo anche le altre tab se non contengono checkbox ma sono visibili
                For x = 0 To Elenco_Tab.Count - 1
                    If CBL_Meteo.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Meteo AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If

                    If CBL_Motivazione.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Motivazione AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If

                    If CBL_Orario.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Orario AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If

                    If CBL_Temperatura.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Temperatura AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If

                    If CBL_VentoDirezione.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Vento_Direzione AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If

                    If CBL_VentoIntensita.Items.Count = 0 AndAlso Elenco_Tab(x).NotaGruppo_Cod.ToString <> "Giustificazioni" AndAlso
                        Elenco_Tab(x).NotaGruppo_Cod = enum_Note_Intervento_Gruppi.Vento_Intensita AndAlso
                        Elenco_Tab(x).visibile = 1 Then

                        Elenco_Tab(x).visibile = 0

                    End If
                Next

                r.ParametroDue_stringa = JsonConvert.SerializeObject(Elenco_Tab, Newtonsoft.Json.Formatting.None)
            Else
                r.ParametroDue = False
            End If

        Catch ex As Exception
            r.RispostaOK = False

            r.ParametroDue = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Sub DefaultAziendaliIrrigazione(ByVal Tipo_OperazioneDB As Integer,
                                            ByVal Tipo_OperazioneAgenda As Integer,
                                            ByVal piva As String,
                                            ByVal Lav_Cod As Integer,
                                            ByVal Veg_Cod As Integer,
                                            ByRef CBL_Consigli As CheckBoxList,
                                            ByRef CBL_Meteo As CheckBoxList,
                                            ByRef CBL_VentoIntensita As CheckBoxList,
                                            ByRef CBL_VentoDirezione As CheckBoxList,
                                            ByRef CBL_Temperatura As CheckBoxList,
                                            ByRef CBL_Orario As CheckBoxList,
                                            ByRef CBL_Motivazione As CheckBoxList,
                                            ByVal objParametri_Server As AgronicaCoreParametri)

        If Tipo_OperazioneDB <> enum_TipoOperazioneDB.Scrittura Then
            Exit Sub
        End If

        Dim NotaUtilizzo_Cod As Integer = enum_Note_Intervento_Utilizzo.QuadernoCampagna

        If Not Tipo_OperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
            NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
        End If

        ''''''''''''''''''''''''''''''''''''''''
        ''''''''''' CARICO LE NOTE '''''''''''''
        ''''''''''''''''''''''''''''''''''''''''
        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
        Dim DT_note As DataTable

        DT_note = objProfilazioneR.LeggiProfilazioneNote_In_Cascata(piva,
                                                                   NotaUtilizzo_Cod.ToString(),
                                                                    Lav_Cod,
                                                                     Veg_Cod,
                                                                     objParametri_Server)

        For x = 0 To CBL_Consigli.Items.Count - 1
            CBL_Consigli.Items(x).Selected = False
        Next
        If CBL_Meteo.Visible = True Then
            For x = 0 To CBL_Meteo.Items.Count - 1
                CBL_Meteo.Items(x).Selected = False
            Next
        End If
        If CBL_VentoIntensita.Visible = True Then
            For x = 0 To CBL_VentoIntensita.Items.Count - 1
                CBL_VentoIntensita.Items(x).Selected = False
            Next
        End If
        If CBL_VentoDirezione.Visible = True Then
            For x = 0 To CBL_VentoDirezione.Items.Count - 1
                CBL_VentoDirezione.Items(x).Selected = False
            Next
        End If
        If CBL_Temperatura.Visible = True Then
            For x = 0 To CBL_Temperatura.Items.Count - 1
                CBL_Temperatura.Items(x).Selected = False
            Next
        End If
        If CBL_Orario.Visible = True Then
            For x = 0 To CBL_Orario.Items.Count - 1
                CBL_Orario.Items(x).Selected = False
            Next
        End If
        If CBL_Motivazione.Visible = True Then
            For x = 0 To CBL_Motivazione.Items.Count - 1
                CBL_Motivazione.Items(x).Selected = False
            Next
        End If


        Dim i As Integer = 0
        Dim j As Integer = 0
        For i = 0 To DT_note.Rows.Count - 1
            For j = 0 To CBL_Consigli.Items.Count - 1
                If CBL_Consigli.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                    CBL_Consigli.Items(j).Selected = True
                    Exit For
                End If
            Next
            If CBL_Meteo.Visible = True Then
                For j = 0 To CBL_Meteo.Items.Count - 1
                    If CBL_Meteo.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_Meteo.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
            If CBL_VentoIntensita.Visible = True Then
                For j = 0 To CBL_VentoIntensita.Items.Count - 1
                    If CBL_VentoIntensita.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_VentoIntensita.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
            If CBL_VentoDirezione.Visible = True Then
                For j = 0 To CBL_VentoDirezione.Items.Count - 1
                    If CBL_VentoDirezione.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_VentoDirezione.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
            If CBL_Temperatura.Visible = True Then
                For j = 0 To CBL_Temperatura.Items.Count - 1
                    If CBL_Temperatura.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_Temperatura.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
            If CBL_Orario.Visible = True Then
                For j = 0 To CBL_Orario.Items.Count - 1
                    If CBL_Orario.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_Orario.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
            If CBL_Motivazione.Visible = True Then
                For j = 0 To CBL_Motivazione.Items.Count - 1
                    If CBL_Motivazione.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_Motivazione.Items(j).Selected = True
                        Exit For
                    End If
                Next
            End If
        Next
    End Sub
#End Region

#Region "Gestione Kendo Grid Impianti Irrigazione"

    Public Function CreaKendoGrid_Impianti_Irrigazione(ByVal piva As String, ByVal veg_cod As Integer,
                                                       ByVal id_cod As Integer, ByVal cul_cod As String, ByVal sa_cod As Integer,
                                                       ByVal data As String,
                                                        ByVal tipo_operazione As Integer, ByVal tipo_operazione_agenda As Integer, ByVal lav_cod As Integer,
                                                        ByVal disciplinare As String, ByVal irrigazioni As JArray,
                                                        ByVal objParamAgendaImpianti As JArray, ByVal DSSIrrigazione_Autorizzato As Boolean,
                                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                                        ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim Dt As New DataTable
        Dim r As New RispostaStandard

        Try

            CaricaGriglia_Impianti_Irrigazione(Dt, piva, veg_cod, id_cod, cul_cod, sa_cod,
                                                data, tipo_operazione, lav_cod,
                                               disciplinare, irrigazioni, objParamAgendaImpianti, objParametri_Server, objParametri_Utenti)

            Dim Colonna_Editabile As Boolean = False

            If tipo_operazione <> enum_TipoOperazioneDB.Lettura Then
                Colonna_Editabile = True
            End If

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("SelezionaImpianto", "SelezionaImpianto", "string") With {
                  ._hidden = True, ._Display = False
                  })

            l.Add(New ColonneNome("Id_Grid_Irrigazione", "Id_Grid_Irrigazione", "string") With {
                  ._hidden = True, ._Display = False
                  })

            l.Add(New ColonneNome("kendoKey", "kendoKey", "string") With {
                  ._hidden = True, ._Display = False
                  })

            l.Add(New ColonneNome("PIVA", "PIVA", "string") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("SA_COD", "Codice Centro Aziendale", "number") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("APPEZZA", "APPEZZA", "number") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("ID_REG", "ID_REG", "number") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("Progetto_Cod", "Progetto_Cod", "number") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("Validita_Inizio_Distinta", "Validita_Inizio_Distinta", "date") With {
                  ._hidden = True, ._Display = False
                  })
            l.Add(New ColonneNome("Validita_Fine_Distinta", "Validita_Fine_Distinta", "date") With {
                  ._hidden = True, ._Display = False
                  })

            If Dt.Columns.Contains("ModifTipoIrriUtilizzata_Cod") Then
                l.Add(New ColonneNome("ModifTipoIrriUtilizzata_Cod", "Codice Irrigazione Utilizzata", "number") With {
                     ._hidden = True, ._Display = False, ._FiltrabileConCheck = False
                      })
            End If

            If Dt.Columns.Contains("Imp_Des") Then

                l.Add(New ColonneNome("Imp_Cod", "CodiceIrrigazione Associata all'Impianto", "number") With {
                      ._hidden = True, ._Display = False
                      })

                l.Add(New ColonneNome("Imp_Des", "Irrigazione Associata all'Impianto", "string") With {
                      ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                      })
            End If

            If Dt.Columns.Contains("ModifTipoIrriUtilizzata_Des") Then
                l.Add(New ColonneNome("ModifTipoIrriUtilizzata_Des", "Irrigazione Utilizzata", "string") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Filtrabile = False, ._Display = True, ._FiltrabileConCheck = False
                      })
            End If

            l.Add(New ColonneNome("Sa_Nome", "Centro Aziendale", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("Campo_Des", "Campo", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("App_Nome", "App.", "string") With {
                  ._RemoveHtmlEncode = True, ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("CodBioApp", "Cod. Biologico App.", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("RifNumerico", "Rif. Numerico App.", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("Catasto", "Catasto", "string") With {
                  ._RemoveHtmlEncode = True, ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            l.Add(New ColonneNome("Codici_Anagrafe_Des", "Destinazione d'uso", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("Cul_Des", "Varietà", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })
            l.Add(New ColonneNome("Sup_Imp", "Sup. [HA]", "number") With {
                  ._hidden = False, ._Display = True, ._formatNr = "n4"
                  })
            l.Add(New ColonneNome("Qta2", "Sup. Trattata [HA]", "number") With {
                  ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._formatNr = "n4"
                  })

            l.Add(New ColonneNome("Disciplinare", "Disciplinare", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            l.Add(New ColonneNome("Reg_Des", "Regolamento", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            l.Add(New ColonneNome("Grfi_Des", "Finalità", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            l.Add(New ColonneNome("Validita_Inizio", "Data Inizio Impianto", "date") With {
                  ._Editabile = False, ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Validita_Fine", "Data Fine Impianto", "date") With {
                  ._Editabile = False, ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Semina", "Data Semina/Trapianto", "date") With {
            ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Fioritura", "Data Fioritura", "date") With {
                  ._Editabile = False, ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Fioritura_Prevista", "Data Fioritura Prevista", "date") With {
                  ._Editabile = False, ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Raccolta_Prevista", "Data Raccolta Prevista", "date") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Raccolta", "Data Raccolta", "date") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("Data_Semina_Prevista", "Data Semina/Trapianto Prevista", "date") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

            l.Add(New ColonneNome("SpecieAgea", "Specie Agea", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            l.Add(New ColonneNome("CultivarAgea", "Varietà Agea", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

#Region "Colonne collegate ai DSS Irrigazione e ai consigli custom"

            l.Add(New ColonneNome("ID_DSS_Irrigazione", "Codice DSS Irrigazione", "number") With {
                     ._hidden = True, ._Display = False, ._FiltrabileConCheck = False
                      })

            'Nascondo la possibilità di scegliere un DSS se sto creando una ricetta o brogliaccio oppure se l'utente non ha il permesso per la pagina dei DSS Irrigazione

            Dim display_DSS As Boolean = True

            Dim hidden_DSS As Boolean = False

            Dim editabile_DSS As Boolean = Colonna_Editabile

            If tipo_operazione_agenda <> enum_Tipo_Operazione_Agenda.QuadernoDiCampagna OrElse Not DSSIrrigazione_Autorizzato Then
                display_DSS = False
                hidden_DSS = True
                editabile_DSS = False
            End If

            l.Add(New ColonneNome("Descrizione_DSS_Irrigazione", "Consiglio (Data - Dose Esecuzione)", "string") With {
                      ._Display = display_DSS,
                      ._Editabile = editabile_DSS, ._hidden = hidden_DSS, ._Filtrabile = False, ._FiltrabileConCheck = False
                      })


            l.Add(New ColonneNome("Qta_Acqua_DSS_Irrigazione", "Qta Acqua DSS Irrigazione", "number") With {
                     ._hidden = True, ._Display = False, ._FiltrabileConCheck = False
                      })


            l.Add(New ColonneNome("Udm_Cod_DSS_Irrigazione", "Udm Cod DSS Irrigazione", "number") With {
                     ._hidden = True, ._Display = False, ._FiltrabileConCheck = False
                      })


            'Nascondo le colonne data/dose consiglio custom se sto creando una ricetta o brogliaccio

            Dim display_consiglio_custom As Boolean = True

            Dim hidden_consiglio_custom As Boolean = False

            Dim editabile_consiglio_custom As Boolean = Colonna_Editabile

            If tipo_operazione_agenda <> enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                display_consiglio_custom = False
                hidden_consiglio_custom = True
                editabile_consiglio_custom = False
            End If

            l.Add(New ColonneNome("Data_Consiglio_Custom", "Data Consiglio", "date") With {
                      ._Display = display_consiglio_custom,
                      ._Editabile = editabile_consiglio_custom, ._hidden = hidden_consiglio_custom, ._Filtrabile = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                      })

            l.Add(New ColonneNome("Qta_Acqua_Consiglio_Custom", "Dose Acqua Consiglio", "number") With {
                      ._Display = display_consiglio_custom,
                      ._Editabile = editabile_consiglio_custom, ._hidden = hidden_consiglio_custom, ._Filtrabile = False, ._FiltrabileConCheck = False, ._formatNr = "n2"
                      })

            l.Add(New ColonneNome("Min_Data_Turno", "Min_Data_Turno", "date") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._Editabile = False
                  })

            l.Add(New ColonneNome("Max_Data_Turno", "Max_Data_Turno", "date") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = False, ._Editabile = False
                  })

#End Region

            'Dettagli di Irrigazione

            If Dt.Columns.Contains("Dose") Then
                l.Add(New ColonneNome("Dose", "Dose Acqua", "number") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._css = "DoseAcquaIrrigazione", ._FiltrabileConCheck = False, ._formatNr = "n2"
                      })
            End If

            If Dt.Columns.Contains("Ore") Then

                l.Add(New ColonneNome("Ore", "Ore di Irrigazione [H]", "number") With {
                         ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._css = "OrePortataIrrigazione", ._FiltrabileConCheck = False, ._formatNr = "n2"
                      })

            End If

            If Dt.Columns.Contains("Portata") Then
                l.Add(New ColonneNome("Portata", "Portata [l/H]", "number") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._css = "OrePortataIrrigazione", ._FiltrabileConCheck = False, ._formatNr = "0"
                      })
            End If

            If Dt.Columns.Contains("Qta_Totale") Then
                l.Add(New ColonneNome("Qta_Totale", "Quantità Totale di Acqua [M3]", "number") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._css = "QtaTotaleAcquaIrrigazione", ._FiltrabileConCheck = False, ._formatNr = "n4"
                      })
            End If

            If Dt.Columns.Contains("Data_Inizio") Then
                l.Add(New ColonneNome("Data_Inizio", "Data Inizio Irrigazione", "date") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                      })
            End If

            If Dt.Columns.Contains("Data_Fine") Then
                l.Add(New ColonneNome("Data_Fine", "Data Fine Irrigazione", "date") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                      })
            End If

            If Dt.Columns.Contains("Frequenza") Then
                l.Add(New ColonneNome("Frequenza", "Frequenza Irrigazione Media [GG]", "number") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False, ._formatNr = "0"
                      })
            End If

            l.Add(New ColonneNome("Qta_Totale_Acqua_Periodo", "Quantità Totale di Acqua nel Periodo [M3]", "number") With {
                      ._Editabile = Colonna_Editabile, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False, ._formatNr = "n3"
                      })

            l.Add(New ColonneNome("Progetto", "Lotto Impianto", "string") With {
                  ._hidden = True, ._Display = False, ._FiltrabileConCheck = True
                  })

            Dim ElencoChkImpostazioniColonne As New List(Of Object)

            ImpostazioniColonne(l, ElencoChkImpostazioniColonne, tipo_operazione_agenda, objParametri_Utenti)

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable

            r.RispostaOK = True

            r.RispostaStringa = js.JSON_DataTable_Kendo(Dt, l, AssegnaAutomaticamenteTipiDato:=False, tipoFiltroKendo:=TipiEnumerativi.TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.ParametroDue = True
            r.ParametroDue_stringa = JsonConvert.SerializeObject(ElencoChkImpostazioniColonne, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.ParametroDue = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Sub CaricaGriglia_Impianti_Irrigazione(ByRef Dt As DataTable,
                                                        ByVal piva As String, ByVal veg_cod As Integer,
                                                        ByVal id_cod As Integer, ByVal cul_cod As String, ByVal sa_cod As Integer,
                                                        ByVal data As String,
                                                        ByVal tipo_operazione As Integer, ByVal lav_cod As Integer,
                                                        ByVal disciplinare As String,
                                                        ByVal irrigazioni As JArray, ByVal objParamAgendaImpianti As JArray,
                                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                                        ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim Testo As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim AgendaData = DateTime.ParseExact(data.ToString, "yyyyMMdd", Nothing)
        Dim strErr As String = ""
        Dim ColturaProtetta As String = ""


        '----- Recupero l'elenco degli impianti

        ''------------------------------------------------------------------------------
        Dim Array() As String
        Array = Split(disciplinare, "/")
        Dim Grfi_Cod As Integer
        Dim Flag_Protetto As Integer
        Dim Flag_Disciplinare As Boolean
        Dim Flag_PubblicoPrivato As Integer = 0

        If Not IsNothing(Array) And Array.Length > 1 Then
            Grfi_Cod = Array(2)
            Flag_Protetto = Array(3)
            Flag_PubblicoPrivato = Array(4)
            Flag_Disciplinare = True
        Else
            Grfi_Cod = 0
            Flag_Protetto = 0
            Flag_PubblicoPrivato = 0
            Flag_Disciplinare = False
        End If


        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        'filtro in scrittura se ho impostato il filtro dal menu agenda
        Dim filtro As String = "|"
        If tipo_operazione = enum_TipoOperazioneDB.Scrittura Then
            If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso
                HttpContext.Current.Session("Filtro") <> "" Then

                filtro = HttpContext.Current.Session("Filtro")

            End If
        End If

        '----------------------------------
        Dim leggiAncheBloccati As Boolean = False
        If Not IsNothing(HttpContext.Current.Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI")) Then
            If HttpContext.Current.Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True Then
                leggiAncheBloccati = True
            End If
        End If

        Dim CampoCod As Integer = 0


        Dim FiltroAggiuntivo As String = ""

        Dim filtroFinale As String = filtro.Split("|")(1)


        If FiltroAggiuntivo <> "" Then
            FiltroAggiuntivo = "(" & FiltroAggiuntivo & ")"
            If filtroFinale <> "" Then
                filtroFinale = filtroFinale & " AND " & FiltroAggiuntivo
            Else
                filtroFinale = FiltroAggiuntivo
            End If
        End If

        Dt = objImpianti.Leggi_Impianti_xAgenda3(Flag_Disciplinare,
                                                piva,
                                                sa_cod,
                                                CampoCod,
                                                veg_cod,
                                                cul_cod,
                                                AgendaData,
                                                Grfi_Cod,
                                                Flag_Protetto,
                                                True,
                                                id_cod,
                                                filtroFinale, " Cul_Des, App_Nome, Progetto ",
                                                objParametri_Server, leggiAncheBloccati)
        '----------------------------------
        'lettura particelle impianti
        Dim DtParticelle As DataTable
        DtParticelle = objImpianti.Leggi_ParticelleImpianti_xAgenda2(Flag_Disciplinare,
                                             piva,
                                             sa_cod,
                                             veg_cod,
                                             cul_cod,
                                             AgendaData,
                                             Grfi_Cod,
                                             Flag_Protetto,
                                             id_cod,
                                             filtro.Split("|")(1), " Cul_Des, App_Nome, Progetto ",
                                             objParametri_Server, leggiAncheBloccati)

        '----------------------------------
        'lettura zone vulnerabili
        Dim DtPV As DataTable
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        DtPV = objPV.Leggi(-17,
                                   "", "", "", 0, 0, "",
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "", objParametri_Server)

        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Dim DtPVF As DataTable
        Try
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            DtPVF = objPVF.Leggi("", "", "", 0, 0, "", 0,
                                         " Fascia_Cod <>0 ",
                                         "", objParametri_Server)
        Catch ex As Exception

        End Try


        Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Raccolte_Precedenti", GetType(String)))

        Dim objRaccolte As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DTRAccolte As DataTable


        Dim ObjUtentiI As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim Dt_Impostazioni As New DataTable
        Dim Utilizza_SupApp As Boolean = False
        If tipo_operazione = enum_TipoOperazioneDB.Scrittura Or
                   tipo_operazione = enum_TipoOperazioneDB.Modifica Then
            Dt_Impostazioni = ObjUtentiI.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_UTILIZZA_SUP_APP_AGENDA,
                                                      lav_cod,
                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "",
                                                      "",
                                                      objParametri_Utenti)
            If Not Dt_Impostazioni Is Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then
                Utilizza_SupApp = True
            End If

        End If


        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            For j = 0 To Dt.Rows.Count - 1

                '--------------------------
                Select Case Dt.Rows(j).Item("Cop_Cod")
                    Case 0, 1, 3, 4, 5, 6 'nessuna copertura
                        ColturaProtetta = "No"
                        Flag_Protetto = 0
                    Case Else
                        ColturaProtetta = "Si"
                        Flag_Protetto = 1
                End Select

                Dt.Rows(j).Item("id_rcdpi") = 0

                If Utilizza_SupApp = True Then
                    Dt.Rows(j).Item("sup_imp") = Dt.Rows(j).Item("sup_app")
                End If

                '--------------------------
                'Formatto le date
                If IsDate(Dt.Rows(j).Item("Validita_Inizio")) Then
                    If CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Validita_Inizio") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Validita_Fine")) Then
                    If CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Validita_Fine") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina")) Then
                    If CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Semina") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Semina_Prevista") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Raccolta")) Then
                    If CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Raccolta") = ""
                    End If
                End If

                DTRAccolte = objRaccolte.Leggi_Raccolte(piva,
                                                          sa_cod,
                                                          0, 0, 0,
                                                          Dt.Rows(j).Item("appezza"),
                                                          Dt.Rows(j).Item("id_reg"),
                                                          0,
                                                          CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                                          AgendaData,
                                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          "", "", objParametri_Server)


                Dim strRaccolte As String = ""
                Dim r As Integer
                If Not DTRAccolte Is Nothing AndAlso DTRAccolte.Rows.Count > 0 Then
                    For r = 0 To DTRAccolte.Rows.Count - 1
                        strRaccolte &= DTRAccolte.Rows(r).Item("Data_Movimento").ToShortDateString & "<br>"
                    Next
                    If strRaccolte <> "" Then
                        strRaccolte = Left(strRaccolte, strRaccolte.Length - 4)
                    End If
                End If

                Dt.Rows(j).Item("Raccolte_Precedenti") = strRaccolte

                If IsDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Raccolta_Prevista") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura")) Then
                    If CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Fioritura") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Fioritura_Prevista") = ""
                    End If
                End If
                '========= PermessoDPI fine

                Dim strCatasto As String = ""
                Dim DrParticelle() As DataRow
                Dim DrPV() As DataRow
                Dim DrPVFA() As DataRow
                Dim DrPVFB() As DataRow
                Dim p As Integer

                Dim strParticella As String
                Dim strVulnerabile As String

                If Not DtParticelle Is Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                    DrParticelle = DtParticelle.Select("piva='" & Dt.Rows(j).Item("piva").ToString & "' and sa_cod=" & Dt.Rows(j).Item("sa_cod").ToString & " and appezza=" & Dt.Rows(j).Item("appezza").ToString & " and id_reg=" & Dt.Rows(j).Item("id_reg").ToString)
                    If Not DrParticelle Is Nothing AndAlso DrParticelle.Length > 0 Then
                        For p = 0 To DrParticelle.Length - 1
                            strParticella = DrParticelle(p).Item("prov") & "_" & DrParticelle(p).Item("com") & "_" & DrParticelle(p).Item("sezione") & "_" & DrParticelle(p).Item("foglio") & "_" & DrParticelle(p).Item("numero") & "_" & DrParticelle(p).Item("subalterno")
                            strVulnerabile = ""
                            If Not DtPV Is Nothing AndAlso DtPV.Rows.Count > 0 Then
                                DrPV = DtPV.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Sezione='" & DrParticelle(p).Item("sezione").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Numero=" & DrParticelle(p).Item("numero").ToString & " AND subalterno='" & DrParticelle(p).Item("subalterno").ToString & "'")
                                If Not DrPV Is Nothing AndAlso DrPV.Length > 0 Then
                                    strVulnerabile = " <b>(V)</b>"
                                End If
                            End If
                            If Not DtPVF Is Nothing AndAlso DtPVF.Rows.Count > 0 Then
                                DrPVFA = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=1")
                                If Not DrPVFA Is Nothing AndAlso DrPVFA.Length > 0 Then
                                    strVulnerabile = " <b>(V - Fascia A)</b>"
                                End If
                                DrPVFB = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=2")
                                If Not DrPVFB Is Nothing AndAlso DrPVFB.Length > 0 Then
                                    strVulnerabile = " <b>(V - Fascia B)</b>"
                                End If
                            End If
                            strCatasto &= strParticella & strVulnerabile & "<br>"
                        Next
                    End If
                End If
                If strCatasto <> "" Then
                    strCatasto = Left(strCatasto, strCatasto.Length - 4)
                End If

                Dt.Rows(j).Item("catasto") = strCatasto
            Next

        End If

        'Genero la chiave della griglia e la colonna SelezionaImpianto

        Dt.Columns.Add(New DataColumn("kendoKey", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Grid_Irrigazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("SelezionaImpianto", GetType(String)))

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            For j = 0 To Dt.Rows.Count - 1

                Dim kk As String =
                    Dt.Rows(j).Item("Piva") & "-" &
                    Dt.Rows(j).Item("Sa_Cod").ToString & "-" &
                    Dt.Rows(j).Item("Appezza").ToString & "-" &
                    Dt.Rows(j).Item("id_reg").ToString


                Dt.Rows(j).Item("kendoKey") = kk

                Dt.Rows(j).Item("Id_Grid_Irrigazione") = Guid.NewGuid

                Dt.Rows(j).Item("SelezionaImpianto") = "false"
            Next
        End If



        ' Aggiungo i dati di irrigazione
        If Not IsNothing(Dt) AndAlso Not IsNothing(irrigazioni) AndAlso Not IsNothing(objParamAgendaImpianti) Then


            '-----------------------------------------------------
            'se ho impianti selezionati li chekko!!!
            '-----------------------------------------------------

            'In scrittura non seleziono in automatico gli impianti
            For j = 0 To Dt.Rows.Count - 1
                For i = 0 To objParamAgendaImpianti.Count - 1
                    If objParamAgendaImpianti(i)("Piva") = Dt.Rows(j).Item("piva") And
                       objParamAgendaImpianti(i)("Sa_Cod") = Dt.Rows(j).Item("sa_cod") And
                       objParamAgendaImpianti(i)("Appezza") = Dt.Rows(j).Item("appezza") And
                       objParamAgendaImpianti(i)("ID_Reg") = Dt.Rows(j).Item("id_reg") Then

                        Dt.Rows(j).Item("SelezionaImpianto") = "true"
                    End If
                Next
            Next

            'Aggiungo le colonne al datatable
            Dt.Columns.Add(New DataColumn("Dose", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Ore", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Portata", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Data_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Fine", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Frequenza", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Qta_Totale", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("UDM_Dose", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("TipoIrrigazioneUtilizzata", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Qta2", GetType(Decimal)))

            Dt.Columns.Add(New DataColumn("ModifTipoIrriUtilizzata_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("ModifTipoIrriUtilizzata_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qta_Totale_Acqua_Periodo", GetType(Decimal)))

            Dt.Columns.Add(New DataColumn("ID_DSS_Irrigazione", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione_DSS_Irrigazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qta_Acqua_DSS_Irrigazione", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Udm_Cod_DSS_Irrigazione", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Data_Consiglio_DSS_Irrigazione", GetType(Date)))

            Dt.Columns.Add(New DataColumn("Qta_Acqua_Consiglio_Custom", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Data_Consiglio_Custom", GetType(Date)))

            Dt.Columns.Add(New DataColumn("Min_Data_Turno", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Max_Data_Turno", GetType(Date)))

            Dim Dt_ImpiantiIrrigazioni As DataTable = Nothing
            Dim Dt_DSS As DataTable = Nothing

            If irrigazioni.Count > 0 Then

                Dim objIrrigazioni As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
                Dt_ImpiantiIrrigazioni = objIrrigazioni.Leggi(-1,
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "",
                          "", objParametri_Server)

                Dim objDSS As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R
                Dt_DSS = objDSS.Leggi_Precedenti_Con_Turni(piva, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
            End If


            For x = 0 To Dt.Rows.Count - 1

                'Imposto i valori di default delle nuove colonne
                Dt(x)("Qta2") = 0
                Dt(x)("Dose") = 0
                Dt(x)("Ore") = 0
                Dt(x)("Portata") = 0
                Dt(x)("Qta_Totale") = 0

                'Default Data Inizio e Fine Irrigazione sono vuote,
                'non mostrare AGRODATAINIZIO e AGRODATAFINE
                Dt(x)("Data_Inizio") = DBNull.Value
                Dt(x)("Data_Fine") = DBNull.Value

                Dt(x)("Frequenza") = 0
                Dt(x)("Qta_Totale_Acqua_Periodo") = 0
                Dt(x)("ModifTipoIrriUtilizzata_Cod") = -1
                Dt(x)("ModifTipoIrriUtilizzata_Des") = "-Quella Dell'impianto-"

                Dt(x)("ID_DSS_Irrigazione") = 0
                Dt(x)("Descrizione_DSS_Irrigazione") = "Nessun Consiglio"
                Dt(x)("Qta_Acqua_DSS_Irrigazione") = 0
                Dt(x)("Udm_Cod_DSS_Irrigazione") = 0

                Dt(x)("Qta_Acqua_Consiglio_Custom") = 0
                Dt(x)("Data_Consiglio_Custom") = DBNull.Value

                Dt(x)("Min_Data_Turno") = DBNull.Value
                Dt(x)("Max_Data_Turno") = DBNull.Value

                For y = 0 To irrigazioni.Count - 1

                    If Dt(x)("kendokey").ToString = irrigazioni(y)("ImpiantoIrrigato").ToString Then

                        Dt(x)("Dose") = CDec(irrigazioni(y)("Dose").ToString)
                        Dt(x)("Ore") = CDec(irrigazioni(y)("Ore").ToString)


                        Dt(x)("Portata") = CInt(irrigazioni(y)("Portata").ToString)

                        'Default Data Inizio e Fine Irrigazione sono vuote,
                        'non mostrare AGRODATAINIZIO e AGRODATAFINE
                        If CDate(irrigazioni(y)("Data_Inizio").ToString) <> AGRODATAINIZIO Then
                            Dt(x)("Data_Inizio") = CDate(irrigazioni(y)("Data_Inizio").ToString)
                        End If

                        If CDate(irrigazioni(y)("Data_Fine").ToString) <> AGRODATAFINE Then
                            Dt(x)("Data_Fine") = CDate(irrigazioni(y)("Data_Fine").ToString)
                        End If


                        Dt(x)("Frequenza") = CInt(irrigazioni(y)("Frequenza").ToString)
                        Dt(x)("Qta_Totale") = CDec(irrigazioni(y)("Qta_Totale").ToString)
                        Dt(x)("UDM_Dose") = CInt(irrigazioni(y)("UDM_Dose").ToString)
                        Dt(x)("TipoIrrigazioneUtilizzata") = CInt(Dt(x)("Imp_Cod").ToString)


                        Dt(x)("ModifTipoIrriUtilizzata_Cod") = CInt(irrigazioni(y)("TipoIrrigazioneUtilizzata").ToString)


                        If Not IsNothing(Dt_ImpiantiIrrigazioni) AndAlso Dt_ImpiantiIrrigazioni.Rows.Count > 0 AndAlso Dt(x)("ModifTipoIrriUtilizzata_Cod") > 0 Then
                            Dim Dr_ImpiantiIrrigazioni = Dt_ImpiantiIrrigazioni.Select("Imp_Cod = " & Dt(x)("ModifTipoIrriUtilizzata_Cod"))

                            If Not IsNothing(Dr_ImpiantiIrrigazioni) AndAlso Dr_ImpiantiIrrigazioni.Count = 1 Then
                                Dt(x)("ModifTipoIrriUtilizzata_Des") = Dr_ImpiantiIrrigazioni(0)("Imp_Des")
                            End If


                        End If

                        'Se l'impianto non è selezionato Qta2 è uguale a 0.
                        If Dt(x)("SelezionaImpianto") = "true" Then
                            Dt(x)("Qta2") = CDec(irrigazioni(y)("Qta2").ToString)
                        End If

                        Dt(x)("ID_DSS_Irrigazione") = CInt(irrigazioni(y)("ID_DSS_Irrigazione").ToString)

                        If Not IsNothing(Dt_DSS) AndAlso Dt_DSS.Rows.Count > 0 AndAlso Dt(x)("ID_DSS_Irrigazione") > 0 Then
                            Dim Dr_DSS = Dt_DSS.Select("ID_DSS_Irrigazione = " & Dt(x)("ID_DSS_Irrigazione"))

                            If Not IsNothing(Dr_DSS) AndAlso Dr_DSS.Count = 1 Then
                                Dt(x)("Descrizione_DSS_Irrigazione") = Dr_DSS(0)("Descrizione_DSS_Irrigazione")
                                Dt(x)("Qta_Acqua_DSS_Irrigazione") = Dr_DSS(0)("Qta_Acqua_DSS_Irrigazione")
                                Dt(x)("Udm_Cod_DSS_Irrigazione") = Dr_DSS(0)("Udm_Cod_DSS_Irrigazione")
                                Dt(x)("Data_Consiglio_DSS_Irrigazione") = Dr_DSS(0)("Data_Consiglio")
                                Dt(x)("Min_Data_Turno") = Dr_DSS(0)("Min_Data_Turno")
                                Dt(x)("Max_Data_Turno") = Dr_DSS(0)("Max_Data_Turno")
                            End If
                        End If


                        Dt(x)("Qta_Acqua_Consiglio_Custom") = CDec(irrigazioni(y)("Qta_Acqua_Custom").ToString)

                        If CDate(irrigazioni(y)("Data_Custom").ToString) <> AGRODATAINIZIO AndAlso CDate(irrigazioni(y)("Data_Custom").ToString) <> AGRODATAFINE Then
                            Dt(x)("Data_Consiglio_Custom") = CDate(irrigazioni(y)("Data_Custom").ToString)
                        End If


                        'Imposto i valori di Dose e Data Consiglio Custom prendendoli dal DSS
                        If Dt(x)("ID_DSS_Irrigazione") > 0 AndAlso
                            (Dt(x)("Qta_Acqua_Consiglio_Custom") = 0 OrElse Dt(x)("Data_Consiglio_Custom") Is DBNull.Value) Then

                            Dt(x)("Qta_Acqua_Consiglio_Custom") = Dt(x)("Qta_Acqua_DSS_Irrigazione")
                            Dt(x)("Data_Consiglio_Custom") = Dt(x)("Data_Consiglio_DSS_Irrigazione")

                        End If

                        Exit For
                    End If
                Next

            Next
        End If


    End Sub


    Private Sub ImpostazioniColonne(ByRef l As List(Of ColonneNome), ByRef ElencoChkImpostazioniColonne As List(Of Object), ByVal tipo_operazione_agenda As Integer, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim ElencoColonneDaNascondereXRicette As New List(Of String)

        Dim Titolo_ID_DSS_Irrigazione = l.Find(Function(c) c._Nome_colonna_DT = "ID_DSS_Irrigazione")._Nome_colonna_Json

        Dim Titolo_Descrizione_DSS_Irrigazione = l.Find(Function(c) c._Nome_colonna_DT = "Descrizione_DSS_Irrigazione")._Nome_colonna_Json

        Dim Titolo_Qta_Acqua_DSS_Irrigazione = l.Find(Function(c) c._Nome_colonna_DT = "Qta_Acqua_DSS_Irrigazione")._Nome_colonna_Json

        Dim Titolo_Udm_Cod_DSS_Irrigazione = l.Find(Function(c) c._Nome_colonna_DT = "Udm_Cod_DSS_Irrigazione")._Nome_colonna_Json

        Dim Titolo_Qta_Acqua_Consiglio_Custom = l.Find(Function(c) c._Nome_colonna_DT = "Qta_Acqua_Consiglio_Custom")._Nome_colonna_Json

        Dim Titolo_Data_Consiglio_Custom = l.Find(Function(c) c._Nome_colonna_DT = "Data_Consiglio_Custom")._Nome_colonna_Json


        Dim Colonne_non_Selezionabili() As String = {
            "SelezionaImpianto", "kendoKey", "SA_COD", "Id_Grid_Irrigazione", "Sup_Imp", "Qta2",
            "Dose", "Ore", "Portata", "APPEZZA", "ID_REG", "Progetto_Cod",
            "Qta_Totale", "Data_Inizio", "Data_Fine", "Frequenza", "Imp_Cod", "PIVA",
            "Validita_Inizio_Distinta", "Validita_Fine_Distinta",
            "Qta_Totale_Acqua_Periodo", "ModifTipoIrriUtilizzata_Cod", "ModifTipoIrriUtilizzata_Des",
            "ID_DSS_Irrigazione", "Descrizione_DSS_Irrigazione", "Qta_Acqua_DSS_Irrigazione", "Udm_Cod_DSS_Irrigazione",
            "Qta_Acqua_Consiglio_Custom", "Data_Consiglio_Custom", "Min_Data_Turno", "Max_Data_Turno"
        }

        If tipo_operazione_agenda <> enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
            ElencoColonneDaNascondereXRicette.Add(Titolo_ID_DSS_Irrigazione)
            ElencoColonneDaNascondereXRicette.Add(Titolo_Descrizione_DSS_Irrigazione)
            ElencoColonneDaNascondereXRicette.Add(Titolo_Qta_Acqua_DSS_Irrigazione)
            ElencoColonneDaNascondereXRicette.Add(Titolo_Udm_Cod_DSS_Irrigazione)
            ElencoColonneDaNascondereXRicette.Add(Titolo_Qta_Acqua_Consiglio_Custom)
            ElencoColonneDaNascondereXRicette.Add(Titolo_Data_Consiglio_Custom)
        End If

        'carico le impostazioni utente

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim Dt As DataTable
        Dt = objUtenti.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                            LAVCOD_IRRIGAZIONE,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri_Utenti)


        Dim i As Integer = 0
        Dim str As String = ""

        'se esiste l'impostazione utilizzo quella altrimenti metto dei default (x i nuovi utenti per esempio)
        If Dt.Rows.Count > 0 Then
            str = Dt.Rows(0).Item("Str_0")
        End If
        If str = "" Then
            str = "Centro Aziendale|App.|Varietà"
        End If


        For i = 0 To str.Split("|").Length - 1
            Dim valore As String = str.Split("|")(i)

            If Not ElencoColonneDaNascondereXRicette.Contains(valore) Then
                Dim j = 0
                For j = 0 To l.Count - 1
                    If l(j)._Nome_colonna_Json = valore Then
                        l(j)._hidden = False
                        l(j)._Display = True
                    End If
                Next
            End If
        Next



        'Valorizzo la lista dei checkbox da mostrare nella dialog di selezione colonne
        For j = 0 To l.Count - 1

            If Not Colonne_non_Selezionabili.Contains(l(j)._Nome_colonna_DT) Then

                Dim mostra = False


                For i = 0 To str.Split("|").Length - 1
                    If str.Split("|")(i) = l(j)._Nome_colonna_Json Then
                        mostra = True
                        Exit For
                    End If
                Next

                ElencoChkImpostazioniColonne.Add(New With {
                                            .id = l(j)._Nome_colonna_DT,
                                            .text = l(j)._Nome_colonna_Json,
                                            .Mostra = mostra
                                            })
            End If
        Next

    End Sub

    Public Sub Salva_ImpostazioniKendoGrid_Impianti_Irrigazione(ByVal colonne_visibili As String,
                                                                ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim Flag_Connessione, Flag_Transazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri_Utenti,
                                         Flag_Connessione,
                                         Flag_Transazione)


            Dim objImpostazioniUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W
            'prima cancello le vechie impostazioni
            objImpostazioniUtente.Cancella(objParametri_Utenti.UtenteUsername,
                                            enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                                            "", objParametri_Utenti)

            If colonne_visibili.Length > 0 Then
                'salvo
                objImpostazioniUtente.Scrivi(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                                             LAVCOD_IRRIGAZIONE, colonne_visibili, AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            End If


            Utility.VerificaChiudiTransazione(objParametri_Utenti,
                                  Flag_Transazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Utenti,
                                               Flag_Transazione)
        Finally

            Utility.VerificaChiudiConnessione(objParametri_Utenti,
                                                   Flag_Connessione)

        End Try
    End Sub

#End Region

#Region "Gestione Kendo Grid Rilievi Pioggie"
    Public Function CercaPioggeIrrigazione(ByVal piva As String,
                                            ByVal data_da_string As String,
                                            ByVal data_a_string As String,
                                            ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messaggio_errore As String = String.Empty
        Dim data_da As Date
        Dim data_a As Date
        Dim Grid_RilieviPioggie As String = String.Empty

        If data_da_string <> "" AndAlso data_a_string <> "" AndAlso data_da_string <> "null" AndAlso data_a_string <> "null" Then
            Try
                data_da = DateTime.ParseExact(data_da_string.ToString, "yyyyMMdd", Nothing)
                data_a = DateTime.ParseExact(data_a_string.ToString, "yyyyMMdd", Nothing)
            Catch ex As Exception
                messaggio_errore = "Inserire un formato di data valido per l'intervallo temporale. <br> gg/mm/aaaa Es: 30/10/2012 ."
            End Try

        Else
            messaggio_errore = "E ' necessario specificare entrambe le date dell'intervallo temporale. <br> Se si desidera un solo giorno, specificare la stessa data in entrambe le caselle."
        End If

        Try
            VisualizzaRilieviPioggeDelPeriodo(piva, 0, data_da, data_a, Grid_RilieviPioggie, objParametri_Server)
        Catch ex As Exception
            messaggio_errore = "Errore nella creazione della griglia. <br> " & ex.Message
        End Try


        If String.IsNullOrEmpty(messaggio_errore) Then
            r.RispostaOK = True
            r.RispostaStringa = Grid_RilieviPioggie
        Else
            r.RispostaOK = False
            r.Errore = messaggio_errore
        End If

        Return r

    End Function

    Private Sub VisualizzaRilieviPioggeDelPeriodo(ByRef Piva As String, ByRef Sacod As Integer,
                                                  ByRef dataI As Date, ByRef dataF As Date,
                                                  ByRef Grid_RilieviPiogge As String,
                                                  ByVal objParametri_Server As AgronicaCoreParametri)

        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        Dim row As DataRow
        Dim dt_mov As DataTable = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R().LeggiPiogge(Piva, Sacod, 0, dataI, dataF, "", objParametri_Server)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable

        'sommo totali periodo per centro
        Dim ht As New Hashtable
        For i = 0 To dt_mov.Rows.Count() - 1

            If ht.Contains(dt_mov.Rows(i).Item("Sa_Cod")) Then
                ht.Item(dt_mov.Rows(i).Item("Sa_Cod")) = CDec(ht.Item(dt_mov.Rows(i).Item("Sa_Cod"))) + CDec(dt_mov.Rows(i).Item("Qta_Ril"))
            Else
                ht.Add(dt_mov.Rows(i).Item("Sa_Cod"), dt_mov.Rows(i).Item("Qta_Ril"))
            End If
        Next

        For i = 0 To dt_mov.Rows.Count() - 1
            row = dt.NewRow
            row.Item("Id_Grid_RilieviPioggie") = guid.NewGuid
            row.Item("PIVA") = dt_mov.Rows(i).Item("PIVA")
            row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(dt_mov.Rows(i).Item("PIVA"), objParametri_Server)
            row.Item("Sa_Cod") = dt_mov.Rows(i).Item("Sa_Cod")
            row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(dt_mov.Rows(i).Item("PIVA"), dt_mov.Rows(i).Item("Sa_Cod"), objParametri_Server)
            row.Item("ID_Agenda") = dt_mov.Rows(i).Item("ID_Agenda")
            row.Item("Data") = CDate(dt_mov.Rows(i).Item("validita_inizio"))
            Try
                row.Item("Ora") = CInt(Split(CDate(dt_mov.Rows(i).Item("Ora")).ToShortTimeString, ".")(0))
                row.Item("Minuti") = CInt(Split(CDate(dt_mov.Rows(i).Item("Ora")).ToShortTimeString, ".")(1))
            Catch ex As Exception
                row.Item("Ora") = 23
                row.Item("Minuti") = 0
            End Try
            row.Item("mm_pioggia") = dt_mov.Rows(i).Item("Qta_Ril")
            row.Item("Note") = dt_mov.Rows(i).Item("Mov_Desc")
            row.Item("temp_minima") = dt_mov.Rows(i).Item("Piezo1")
            row.Item("temp_massima") = dt_mov.Rows(i).Item("Piezo2")
            row.Item("umidita") = dt_mov.Rows(i).Item("Piezo3")

            row.Item("mm_pioggia_tot") = ht.Item(dt_mov.Rows(i).Item("Sa_Cod"))

            dt.Rows.Add(row)

        Next

        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("Id_Grid_RilieviPioggie", "Id_Grid_RilieviPioggie", "string") With {
                 ._hidden = True, ._Display = False
                  })

        l.Add(New ColonneNome("Sa_Nome", "Centro Aziendale", "string") With {
                 ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = True
                  })

        l.Add(New ColonneNome("Data", "Data Rilievo", "date") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"
                  })

        l.Add(New ColonneNome("mm_pioggia", "Pioggia [mm]", "string") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False
                  })

        l.Add(New ColonneNome("mm_pioggia_tot", "Totale Nel Periodo [mm]", "string") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False
                  })

        l.Add(New ColonneNome("Note", "Note", "string") With {
                 ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = True
                  })

        l.Add(New ColonneNome("temp_minima", "T. Minima [°C]", "string") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False
                  })

        l.Add(New ColonneNome("temp_massima", "T. Massima [°C]", "string") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False
                  })

        l.Add(New ColonneNome("umidita", "Umidità [%]", "string") With {
                ._Editabile = False, ._hidden = False, ._Display = True, ._FiltrabileConCheck = False
                  })


        Grid_RilieviPiogge = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipiEnumerativi.TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
    End Sub



    Private Function InizializzaDataTablePerGrigliePiogge() As DataTable
        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("Id_Grid_RilieviPioggie", GetType(String))
        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("ID_Agenda", GetType(Integer))
        Dt.Columns.Add("Data", GetType(Date))
        Dt.Columns.Add("Ora", GetType(Integer))
        Dt.Columns.Add("Minuti", GetType(Integer))
        Dt.Columns.Add("mm_pioggia", GetType(String))
        Dt.Columns.Add("mm_pioggia_tot", GetType(String))
        Dt.Columns.Add("Note", GetType(String))
        Dt.Columns.Add("temp_minima", GetType(Integer))
        Dt.Columns.Add("temp_massima", GetType(Integer))
        Dt.Columns.Add("umidita", GetType(Integer))



        Return Dt
    End Function

#End Region

#Region " Gestione Salvataggio Irrigazione"

    Public Function RegistraIrrigazione(ByVal model As String, ByVal ASG_ProgressivoGIAS As Integer, ByVal objParametri_Server As AgronicaCoreParametri)

        Dim r As New RispostaStandard

        Try

            Dim paramRichiesta As JArray = JArray.Parse(model)

            If Not IsNothing(paramRichiesta) AndAlso paramRichiesta.Count > 0 Then


                '-----------------------------------------------------
                '----------- CONNESSIONE E TRANSAZIONE ---------------
                Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
                '-----------------------------------------------------

                Dim BaseCode As Integer
                Dim TopCode As Integer

                '------------------------------------------------
                '----- Calcolo i valori di BaseCode e TopCode
                '------------------------------------------------

                Call Calcola_BaseCode_TopCode(BaseCode,
                                            TopCode,
                                            ASG_ProgressivoGIAS)

                Dim objMetaschemaSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

                Dim objMetaschemaVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R

                Dim objAgendaScrivi As New Agenda_Operazione_Helper

                Dim messaggio_errore As String = String.Empty

                For Each obj As JObject In paramRichiesta

                    Dim Piva As String = String.Empty

                    Dim Sa_Cod As Integer = 0

                    Dim Appezza As Integer = 0

                    Dim Id_Reg As Integer = 0

                    Dim Data As Date = AGRODATAINIZIO

                    Dim Dose As Decimal = 0

                    Dim Veg_Cod As Integer = 0

                    Dim Veg_Des As String = String.Empty

                    Dim Cul_Cod As Integer = 0

                    Dim Cul_Des As String = String.Empty

                    Dim Sup_Imp As Decimal = 0

                    Dim Qta_Totale As Decimal = 0

                    'Default 18 - millimetri
                    Dim Unita_di_Misura_Dose As String = enum_UnitaMisura.Millimetri

                    'Per ora solo scrittura
                    Dim TipoOperazioneDB As Integer = enum_TipoOperazioneDB.Scrittura

                    If Not IsNothing(obj("Piva")) Then
                        Piva = obj("Piva").ToString
                    Else
                        Throw New Exception("Piva non valorizzata.")
                    End If

                    If Not IsNothing(obj("Sa_Cod")) Then
                        Sa_Cod = CInt(obj("Sa_Cod").ToString)
                    Else
                        Throw New Exception("Sa_Cod non valorizzato.")
                    End If

                    If Not IsNothing(obj("Appezza")) Then
                        Appezza = CInt(obj("Appezza").ToString)
                    Else
                        Throw New Exception("Appezza non valorizzata.")
                    End If

                    If Not IsNothing(obj("Id_Reg")) Then
                        Id_Reg = CInt(obj("Id_Reg").ToString)
                    Else
                        Throw New Exception("Id_Reg non valorizzato.")
                    End If

                    If Not IsNothing(obj("Data")) Then
                        Data = CDate(obj("Data"))
                    Else
                        Throw New Exception("Data non valorizzata.")
                    End If

                    If Not IsNothing(obj("Dose")) Then
                        Dose = CDec(obj("Dose").ToString)
                    Else
                        Throw New Exception("Dose non valorizzata.")
                    End If

                    If Not IsNothing(obj("Veg_Cod")) Then
                        Veg_Cod = CInt(obj("Veg_Cod").ToString)
                    Else
                        Throw New Exception("Veg_Cod non valorizzato.")
                    End If

                    If Not IsNothing(obj("Cul_Cod")) Then
                        Cul_Cod = CInt(obj("Cul_Cod").ToString)
                    Else
                        Throw New Exception("Cul_Cod non valorizzato.")
                    End If

                    If Not IsNothing(obj("Sup_Imp")) Then
                        Sup_Imp = CDec(obj("Sup_Imp").ToString)
                    Else
                        Throw New Exception("Sup_Imp non valorizzato.")
                    End If

                    If Not IsNothing(obj("Unita_di_Misura_Dose")) Then
                        Unita_di_Misura_Dose = obj("Unita_di_Misura_Dose").ToString
                    End If

                    Veg_Des = objMetaschemaSpecie.VegDes_from_VegCod(Veg_Cod, objParametri_Server)

                    Cul_Des = objMetaschemaVarieta.CulDes_from_CulCod(Cul_Cod, objParametri_Server)

                    Dim Note As New NoteModel

                    Note.Txt_Note = ""

                    Note.ElencoNoteSelezionate = New List(Of String)

                    Dim parametri As New SalvaIrrigazioneModel

                    parametri.Unita_di_Misura_Dose = Unita_di_Misura_Dose

                    parametri.TipoOperazioneDB = TipoOperazioneDB

                    parametri.Piva = Piva

                    parametri.Data = Data

                    parametri.Id_Agenda = 0

                    parametri.Lav_Cod = LAVCOD_IRRIGAZIONE

                    parametri.Lav_Des = "Irrigazione"

                    parametri.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale

                    parametri.Cau_Mov = CStr(enum_Agenda_Causali.LAVORAZIONE)

                    parametri.Centro_Aziendale = Sa_Cod

                    parametri.Specie = Veg_Cod

                    parametri.Specie_Des = Veg_Des

                    parametri.Note = Note

                    parametri.Verifica_Compatibilita_MicroIrrigazione = "false"

                    parametri.Varieta = Cul_Cod


                    'Calcolo la Qta_Totale di Acqua

                    Dim Dose_Da_Calcolare As Decimal = Dose

                    If CInt(Unita_di_Misura_Dose) = enum_UnitaMisura.Millimetri Then
                        Dose_Da_Calcolare *= 10
                    ElseIf CInt(Unita_di_Misura_Dose) = enum_UnitaMisura.METRI3__HA Then
                        Dose_Da_Calcolare = Dose
                    Else
                        Dose_Da_Calcolare = 0
                    End If

                    Qta_Totale = Math.Round(Dose_Da_Calcolare * Sup_Imp, 4)

                    Dim tuttelerighe As New List(Of CampiGrigliaIrrigazioneModel)

                    tuttelerighe.Add(New CampiGrigliaIrrigazioneModel With {
                                        .SelezionaImpianto = "true",
                                        .PIVA = Piva,
                                        .SA_COD = Sa_Cod,
                                        .APPEZZA = Appezza,
                                        .ID_REG = Id_Reg,
                                        .Dose = Dose,
                                        .Ore = 0,
                                        .Portata = 0,
                                        .Qta_Totale = Qta_Totale,
                                        .Frequenza = 0,
                                        .Qta_Totale_Acqua_Periodo = 0,
                                        .ModifTipoIrriUtilizzata_Cod = -1,
                                        .Imp_Cod = 0,
                                        .Cul_Des = Cul_Des,
                                        .Sup_Imp = Sup_Imp,
                                        .Qta2 = Sup_Imp,
                                        .Selected = True
                                    })

                    Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = GetImpiantiIrrigazione(tuttelerighe, parametri)

                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpianti, parametri.Centro_Aziendale,
                                                                       messaggio_errore, parametri, tuttelerighe,
                                                                       BaseCode, TopCode)
                    If IsNothing(Agenda) Then
                        Throw New Exception("Non è stato possibile creare l'operazione." & messaggio_errore)
                    End If

                    objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                Next

            Else
                Throw New Exception("model non valorizzato.")
            End If

            r.RispostaOK = True

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        Return r
    End Function

    Public Sub SalvaTuttoIrrigazione(ByRef model As String,
                                     ByRef messaggio_errore As String,
                                     ByRef Id_Agenda As Integer,
                                     ByVal BaseCode As Integer,
                                     ByVal TopCode As Integer,
                                     ByVal TipoSalvataggio As Integer,
                                     ByRef link_redirect As String,
                                     ByVal Qs_Operazione_Ricetta As String,
                                     ByVal Qs_Ricetta_Cod As String,
                                     ByVal objParametri_Server As AgronicaCoreParametri,
                                     ByVal objParametriAgenda As ParametriAgenda)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim parametri As SalvaIrrigazioneModel = JsonConvert.DeserializeObject(Of SalvaIrrigazioneModel)(model, settingLoc)


        If parametri Is Nothing Then
            messaggio_errore = "Parametri mancanti per effettuare la Registrazione con Successo!"
            Exit Sub
        End If

        Dim messaggio_errore_dettaglio = ""

        'Controllo che tutti i parametri dell 'Irrigazione da salvare siano valorizzati.
        ContinuaSalvataggioIrrigazioneModel(parametri, messaggio_errore_dettaglio, Qs_Ricetta_Cod)

        If String.IsNullOrEmpty(messaggio_errore_dettaglio) Then


            Dim res = SalvaOperazioneAgendaRicetta(parametri, messaggio_errore_dettaglio, objParametri_Server,
                                                    BaseCode, TopCode, Qs_Operazione_Ricetta, Qs_Ricetta_Cod)

            If parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then

                If res.SalvataggioCompletato = True AndAlso res.Id_Agenda <> 0 Then

                    gestisciTipoSalvataggio(TipoSalvataggio, parametri.TipoOperazioneAgenda,
                                            parametri.PaginaSitoOrigine, Qs_Ricetta_Cod,
                                            parametri.tipo_salva_parametri_Irrigazione,
                                            parametri.Piva, parametri.Specie, parametri.Centro_Aziendale,
                                            parametri.Data, link_redirect, objParametri_Server, objParametriAgenda)

                    Id_Agenda = res.Id_Agenda
                Else
                    messaggio_errore += "Attenzione, l'operazione non è stata registrata! <br>" & messaggio_errore_dettaglio
                End If

            ElseIf parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse
                parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then

                If res.SalvataggioCompletato = True Then
                    gestisciTipoSalvataggio(TipoSalvataggio, parametri.TipoOperazioneAgenda,
                                            parametri.PaginaSitoOrigine, Qs_Ricetta_Cod,
                                            parametri.tipo_salva_parametri_Irrigazione,
                                            parametri.Piva, parametri.Specie, parametri.Centro_Aziendale,
                                            parametri.Data, link_redirect, objParametri_Server, objParametriAgenda)
                Else
                    messaggio_errore += "Attenzione, l'operazione non è stata registrata! <br>" & messaggio_errore_dettaglio
                End If
            End If

        Else
            messaggio_errore += "Attenzione, l'operazione non è stata registrata! <br>" & messaggio_errore_dettaglio
        End If

    End Sub


    Private Sub ContinuaSalvataggioIrrigazioneModel(ByVal parametri As SalvaIrrigazioneModel,
                                                    ByRef messaggio_errore As String,
                                                    ByVal Qs_Ricetta_Cod As String)

        If Not parametri.Irrigazione Is Nothing AndAlso
           Not parametri.Piva = "" AndAlso Not parametri.Centro_Aziendale Is Nothing AndAlso
           Not parametri.Id_Agenda Is Nothing AndAlso Not parametri.TipoOperazioneDB Is Nothing AndAlso
          Not parametri.Lav_Cod Is Nothing AndAlso Not parametri.Data Is Nothing AndAlso
          Not parametri.Specie = "" AndAlso Not parametri.Note.ElencoNoteSelezionate Is Nothing AndAlso
          Not parametri.Cau_Mov = "" AndAlso Not parametri.Unita_di_Misura_Dose = "" AndAlso
          Not parametri.Verifica_Compatibilita_MicroIrrigazione = "" AndAlso Not parametri.TargetOperazione Is Nothing AndAlso
          Not parametri.PaginaSitoOrigine = "" AndAlso Not parametri.TipoOperazioneAgenda Is Nothing AndAlso
          Not parametri.Programmazione_Cod Is Nothing AndAlso Not parametri.TipoRicetta Is Nothing AndAlso
          Not parametri.SitoOrigine Is Nothing Then

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim righeInserite As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheInserite, settingLoc)
            Dim righeModificate As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheModificate, settingLoc)
            Dim righeEliminate As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheEliminate, settingLoc)

            Dim tuttelerighe As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.TutteLeRighe, settingLoc)

            If tuttelerighe Is Nothing OrElse tuttelerighe.Count = 0 Then
                messaggio_errore = "Selezionare almeno un impianto colturale!"
            End If

            If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura AndAlso
               Qs_Ricetta_Cod = "0" AndAlso parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then

                If Not parametri.RicettaTestata Is Nothing Then

                    Dim RicettaTestata = parametri.RicettaTestata

                    If RicettaTestata.Ricetta_Descrizione = "" OrElse
                        RicettaTestata.Ricetta_Numero = "" OrElse
                        RicettaTestata.Data_Inizio_Ricetta Is Nothing OrElse
                        RicettaTestata.Data_Fine_Ricetta Is Nothing Then

                        messaggio_errore = "Parametri mancanti per effettuare la Registrazione con Successo!"

                    End If

                Else
                    messaggio_errore = "Parametri mancanti per effettuare la Registrazione con Successo!"
                End If

            End If

        Else
            messaggio_errore = "Parametri mancanti per effettuare la Registrazione con Successo!"
        End If

    End Sub

    Private Function GetImpiantiIrrigazione(ByVal tuttelerighe As List(Of CampiGrigliaIrrigazioneModel), ByVal parametri As SalvaIrrigazioneModel)

        Dim ListaImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        Dim Imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto

        If parametri.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then

            For Each rigaIrrigazione As CampiGrigliaIrrigazioneModel In tuttelerighe

                If CBool(rigaIrrigazione.Selected) = True Then

                    Imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                    Imp.Reale_Or_Planning = enum_Tipo_Operazione_Agenda_Target.Reale

                    Imp.Piva = rigaIrrigazione.PIVA.ToString()
                    If Not rigaIrrigazione.SA_COD Is Nothing Then
                        Imp.Sa_Cod = CInt(rigaIrrigazione.SA_COD)
                    End If
                    If Not rigaIrrigazione.APPEZZA Is Nothing Then
                        Imp.Appezza = CInt(rigaIrrigazione.APPEZZA)
                    End If
                    If Not rigaIrrigazione.ID_REG Is Nothing Then
                        Imp.ID_Reg = CInt(rigaIrrigazione.ID_REG)
                    End If
                    If Not rigaIrrigazione.Progetto_Cod Is Nothing Then
                        Imp.Progetto_Cod = CInt(rigaIrrigazione.Progetto_Cod)
                    End If
                    If Not rigaIrrigazione.Sup_Imp Is Nothing Then
                        Imp.Sup_Imp = CDec(rigaIrrigazione.Sup_Imp)
                    End If
                    If Not rigaIrrigazione.Validita_Inizio_Distinta Is Nothing Then
                        Imp.Validita_Inizio_Distinta = CDate(rigaIrrigazione.Validita_Inizio_Distinta)
                    End If
                    If Not rigaIrrigazione.Validita_Fine_Distinta Is Nothing Then
                        Imp.Validita_Fine_Distinta = CDate(rigaIrrigazione.Validita_Fine_Distinta)
                    End If
                    Imp.App_Nome = rigaIrrigazione.App_Nome.ToString()
                    If Not rigaIrrigazione.Qta2 Is Nothing Then
                        Imp.Qta2 = rigaIrrigazione.Qta2.ToString()
                    End If

                    Imp.Cul_Des = rigaIrrigazione.Cul_Des.ToString()

                    If Not rigaIrrigazione.Data_Raccolta Is Nothing AndAlso Not String.IsNullOrEmpty(rigaIrrigazione.Data_Raccolta) Then
                        Imp.Data_Raccolta = rigaIrrigazione.Data_Raccolta
                    End If
                    If Not rigaIrrigazione.Data_Raccolta_Prevista Is Nothing AndAlso Not String.IsNullOrEmpty(rigaIrrigazione.Data_Raccolta_Prevista) Then
                        Imp.Data_Raccolta_Prevista = rigaIrrigazione.Data_Raccolta_Prevista
                    End If

                    If Not rigaIrrigazione.Data_Fioritura Is Nothing AndAlso Not String.IsNullOrEmpty(rigaIrrigazione.Data_Fioritura) Then
                        Imp.Data_Fioritura = rigaIrrigazione.Data_Fioritura
                    End If
                    If Not rigaIrrigazione.Data_Fioritura_Prevista Is Nothing AndAlso Not String.IsNullOrEmpty(rigaIrrigazione.Data_Fioritura_Prevista) Then
                        Imp.Data_Fioritura_Prevista = rigaIrrigazione.Data_Fioritura_Prevista
                    End If

                    Imp.Codici_Anagrafe_Des = rigaIrrigazione.Codici_Anagrafe_Des

                    Imp.Sa_Nome = rigaIrrigazione.Sa_Nome
                    Imp.Campo_Des = rigaIrrigazione.Campo_Des

                    ListaImpianti.Add(Imp)

                End If

            Next

        End If

        Return ListaImpianti

    End Function

    Private Function SalvaOperazioneAgendaRicetta(ByVal parametri As SalvaIrrigazioneModel,
                                           ByRef messaggio_errore As String,
                                           ByVal objParametri_Server As AgronicaCoreParametri,
                                           ByVal BaseCode As Integer, ByVal TopCode As Integer,
                                           ByRef Qs_Operazione_Ricetta As String, ByRef Qs_Ricetta_Cod As String) As Object



        Dim res As Boolean = False


        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim righeInserite As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheInserite, settingLoc)
        Dim righeModificate As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheModificate, settingLoc)
        Dim righeEliminate As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.RigheEliminate, settingLoc)
        Dim tuttelerighe As List(Of CampiGrigliaIrrigazioneModel) = JsonConvert.DeserializeObject(Of List(Of CampiGrigliaIrrigazioneModel))(parametri.Irrigazione.TutteLeRighe, settingLoc)

        '---------------------------------------

        For Each r In tuttelerighe
            r.App_Nome = RispristinaApp_Nome(r.App_Nome)
        Next

        For Each r In righeInserite
            r.App_Nome = RispristinaApp_Nome(r.App_Nome)
        Next

        For Each r In righeModificate
            r.App_Nome = RispristinaApp_Nome(r.App_Nome)
        Next

        For Each r In righeEliminate
            r.App_Nome = RispristinaApp_Nome(r.App_Nome)
        Next

        ' recupero gli IMPIANTI SELEZIONATI
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        If tuttelerighe.Count > 0 Then

            ListaImpianti = GetImpiantiIrrigazione(tuttelerighe, parametri)

            If ListaImpianti.Count = 0 Then
                messaggio_errore = "Selezionare almeno un impianto colturale!"
                Return New With {.SalvataggioCompletato = False, .Id_Agenda = 0}
            Else

                'controllo se ci sono degli impianti con Sup Trattata (Qta2 a 0)

                Dim imp_con_Sup_Trattata_0 As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = ListaImpianti.Where(Function(imp) imp.Qta2 = 0).ToList()

                If imp_con_Sup_Trattata_0.Count > 0 Then

                    Dim nome_impianti As List(Of String) = (From x As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti Select x.App_Nome).Distinct().ToList()

                    If nome_impianti.Count > 0 Then
                        If nome_impianti.Count = 1 Then
                            messaggio_errore = "Indicare la Sup. Trattata [HA] nell' impianto " & nome_impianti(0) & "!"
                        Else
                            messaggio_errore = "Indicare la Sup. Trattata [HA] negli impianti " & String.Join(", ", nome_impianti) & "!"
                        End If
                    End If

                    Return New With {.SalvataggioCompletato = False, .Id_Agenda = 0}
                End If

            End If
        Else
            Return New With {.SalvataggioCompletato = False, .Id_Agenda = 0}
        End If

        '---------------------------------------

        'Se è stato selezionato 'Tutti i centri aziendali' ma tutti gli impianti appartengono ad un solo centro, imposto il valore sull'objparametriAgenda
        Dim listaSaCodImpianti As List(Of Integer) = (From x As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti Select x.Sa_Cod).Distinct().ToList()

        If listaSaCodImpianti.Count = 1 Then
            parametri.Centro_Aziendale = CInt(listaSaCodImpianti.First())
        End If


        '---------------------------------------

        'Blocco il salvataggio se ci sono delle Qta a 0

        If tuttelerighe.Count > 0 Then
            Dim righeconAcqua0 As List(Of String) = (From x As CampiGrigliaIrrigazioneModel In tuttelerighe Where x.Selected = True AndAlso x.Dose <= 0 Select x.App_Nome).Distinct().ToList()

            If Not IsNothing(righeconAcqua0) AndAlso righeconAcqua0.Count > 0 Then
                If righeconAcqua0.Count = 1 Then
                    messaggio_errore = String.Format(My.Resources.AgronicaCoreModelloRes.QuantitaTotaleAcquaImpiantoValorizzata, righeconAcqua0(0))
                Else
                    messaggio_errore = String.Format(My.Resources.AgronicaCoreModelloRes.QuantitaTotaleAcquaImpiantiValorizzata, String.Join(", ", righeconAcqua0))
                End If

                Return New With {.SalvataggioCompletato = False, .Id_Agenda = 0}
            End If
        End If

        '---------------------------------------

        Dim Id_Agenda As Integer

        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Select Case parametri.Centro_Aziendale

                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- OPERAZIONE MULTI-CENTRO    -------------
                        '--------------------------------------------------
                        '--------------------------------------------------
                Case 0

                    'creazione lista centri coinvolti nell'operazione
                    Dim ListaSacod As New List(Of Integer)
                    Dim Trovato As Boolean
                    For i = 0 To ListaImpianti.Count - 1
                        Trovato = False
                        For j = 0 To ListaSacod.Count - 1
                            If ListaImpianti(i).Sa_Cod = ListaSacod(j) Then
                                Trovato = True
                                Exit For
                            End If
                        Next
                        If Trovato = False Then
                            ListaSacod.Add(ListaImpianti(i).Sa_Cod)
                        End If
                    Next

                    '----------

                    'Inizio il ciclo sui centri Aziendali
                    'Ciclo per ogni centro aziendale
                    For i = 0 To ListaSacod.Count - 1


                        Dim ListaImpiantixQuestoSaCod As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                        'Seleziono solamente gli impianti relativi a questo centro aziendale
                        For j = 0 To ListaImpianti.Count - 1
                            If ListaImpianti(j).Sa_Cod = ListaSacod(i) Then
                                ListaImpiantixQuestoSaCod.Add(ListaImpianti(j))
                            End If
                        Next



                        Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpiantixQuestoSaCod, ListaSacod(i),
                                                                                    messaggio_errore, parametri, tuttelerighe,
                                                                                    BaseCode, TopCode)

                        If IsNothing(Agenda) Then
                            Throw New Exception(" Non è stato possibile creare l'operazione. " & messaggio_errore)
                        End If

                        'Salvataggio Tabelle Agenda
                        If parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                            Dim objAgendaScrivi As New Agenda_Operazione_Helper

                            If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                                'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                                allinea_DataUsernameCreazione(Agenda, objParametri_Server, Attivita.Tipo_Attivita.QuadernoDiCampagna, 0)

                                Dim CancellataOperazione As Boolean = False
                                CancellataOperazione = objAgendaScrivi.Cancella(parametri.Piva,
                                                                                    parametri.Centro_Aziendale,
                                                                                    parametri.Id_Agenda, False,
                                                                                    objParametri_Server, logCancellazione:=False)
                            End If

                            Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                            'se sono in scrittura devo agganciare la ricetta se presente
                            If Id_Agenda <> 0 And parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                                ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'ìoperazione
                                If Not IsNothing(HttpContext.Current.Session("UtilizzataRicetta")) AndAlso
                                       HttpContext.Current.Session("UtilizzataRicetta") Then

                                    Dim ricetta_cod As String = HttpContext.Current.Session("ricetta_cod")
                                    Dim Ricetta_Operazione_Cod As String = HttpContext.Current.Session("Ricetta_Operazione_Cod")

                                    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                    Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                                    1)
                                    If Impostazione_RicetteXagenda <> "0" Then
                                        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                        If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                            Throw New Exception("<b>Non è riuscito l'aggancio della ricetta!</b> <br>")
                                        End If

                                        Dim objAppHelper As New AppHelper
                                        If Not objAppHelper.Aggiorna_Agenda_Dati_APP(Id_Agenda, ricetta_cod, Ricetta_Operazione_Cod, objParametri_Server) Then
                                            Throw New Exception("<b>Non è riuscito l'aggancio della ricetta arrivata dal sistema esterno!</b> <br>")
                                        End If

                                    End If

                                End If
                            End If
                            objAgendaScrivi = Nothing

                            'Salvataggio Tabelle Ricette
                        ElseIf parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse
                                parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then

                            Dim listaXMLRicettaOperazione As New List(Of String)

                            Dim rif_Lav_Cod As Integer
                            Dim rif_Des_Lib As String = ""
                            Dim rif_Data As Date
                            Dim rif_Note As String = ""

                            Dim Dpi_Cod As Integer = 0
                            Dim Dpi_PubblicoPrivato As Integer = 0

                            'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
                            Dim StringaXmlOperazione As String = ""
                            Dim Helper As New Agenda_Operazione_Helper
                            StringaXmlOperazione = Helper.GeneraXML_CAU_LAVORAZIONI(Agenda)

                            If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                                'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                                allinea_DataUsernameCreazione(Agenda, objParametri_Server, Attivita.Tipo_Attivita.Ricetta, Qs_Ricetta_Cod)
                            End If

                            'creo la ricetta_operazione dall'xml dell'agenda
                            Dim objRicettaOp As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                            Dim StringaXmlRicettaOperazione As String = objRicettaOp.XML_GeneraStringa_Ricetta_Operazione(parametri.TipoOperazioneDB, Qs_Ricetta_Cod, Qs_Operazione_Ricetta,
                                                                                                                          parametri.TipoOperazioneAgenda, HttpContext.Current.Session("ASG_ProgressivoGIAS"), objParametri_Server,
                                                                                                                          StringaXmlOperazione, rif_Lav_Cod, rif_Des_Lib, rif_Data, rif_Note,
                                                                                                                          Agenda.Data_Creazione, Agenda.Username_Creazione)

                            listaXMLRicettaOperazione.Add(StringaXmlRicettaOperazione)

                            '---------------------------------------------------
                            ' SALVATAGGIO SE RICETTA
                            '---------------------------------------------------

                            If listaXMLRicettaOperazione.Count > 0 Then

                                If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                                    Dim objRicettaOp_Read As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                                    Dim daCancellare As String = objRicettaOp_Read.Ricetta_Operazioni_Leggi(Qs_Ricetta_Cod, Qs_Operazione_Ricetta, 0, 0, AGRODATAINIZIO, AGRODATAFINE, True, objParametri_Server)

                                    Dim ret_Operazione_Ricetta As Integer = 0
                                    Dim objRicettaOp_Write As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
                                    objRicettaOp_Write.Ricetta_Operazione_Scrivi(parametri.Piva, parametri.Centro_Aziendale, daCancellare, Qs_Ricetta_Cod, enum_TipoRicetta.Standard_Destinazioni, ret_Operazione_Ricetta, objParametri_Server)
                                End If

                                'Se sono in fase di passaggio da "Da Fare" a "Fatto" allora recupero il numero ricetta
                                Dim ricetta_numero As String = ""
                                If parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio AndAlso Qs_Operazione_Ricetta <> "" Then
                                    'Recupero il ricetta_numero per non perderlo
                                    Dim Ric_R As New AgronicaCoreContabDAL.Ricette_R
                                    Dim dtRicetta As DataTable = Ric_R.Leggi(Qs_Ricetta_Cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                    ricetta_numero = DBNullToNothing(dtRicetta.Rows(0).Item("ricetta_numero"))
                                End If

                                'Creo la ricetta con la testata se Qs_Ricetta_Cod = "0"
                                'aggiungo solo le operazioni della ricetta se Qs_Ricetta_Cod <> "0" 
                                Dim objRicetteR As New AgronicaCoreContabBIZ.Ricette_R
                                Dim Ricetta_Des As String = parametri.Lav_Des
                                Dim Ricetta_Nota As String = ""
                                Dim Ricetta_DataInizio As Date = parametri.Data
                                Dim Ricetta_DataFine As Date = parametri.Data

                                Dim Ricetta_Testata = parametri.RicettaTestata

                                If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura AndAlso
                                   parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta AndAlso
                                    Qs_Ricetta_Cod = "0" Then

                                    Ricetta_Des = Ricetta_Testata.Ricetta_Descrizione
                                    ricetta_numero = Ricetta_Testata.Ricetta_Numero
                                    'prendo la data inizio editata solo se <= alla data dell'operazione
                                    If Ricetta_Testata.Data_Inizio_Ricetta <= Ricetta_DataInizio Then
                                        Ricetta_DataInizio = Ricetta_Testata.Data_Inizio_Ricetta
                                    End If
                                    'prendo la data fine editata solo se >= alla data dell'operazione
                                    If Ricetta_Testata.Data_Fine_Ricetta >= Ricetta_DataFine Then
                                        Ricetta_DataFine = Ricetta_Testata.Data_Fine_Ricetta
                                    End If
                                    Ricetta_Nota = Ricetta_Testata.Ricetta_Nota

                                    Dim dataOperazione As Date = parametri.Data

                                    If dataOperazione < Ricetta_DataInizio Then
                                        Ricetta_DataInizio = dataOperazione
                                    End If

                                    If dataOperazione > Ricetta_DataFine Then
                                        Ricetta_DataFine = dataOperazione
                                    End If

                                ElseIf Qs_Ricetta_Cod <> "0" AndAlso
                                        parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then

                                    Ricetta_Des = Ricetta_Testata.Ricetta_Descrizione
                                    ricetta_numero = Ricetta_Testata.Ricetta_Numero
                                    'prendo la data inizio editata solo se <= alla data dell'operazione
                                    If Ricetta_Testata.Data_Inizio_Ricetta <= Ricetta_DataInizio Then
                                        Ricetta_DataInizio = Ricetta_Testata.Data_Inizio_Ricetta
                                    End If
                                    'prendo la data fine editata solo se >= alla data dell'operazione
                                    If Ricetta_Testata.Data_Inizio_Ricetta >= Ricetta_DataFine Then
                                        Ricetta_DataFine = Ricetta_Testata.Data_Fine_Ricetta
                                    End If
                                    Ricetta_Nota = Ricetta_Testata.Ricetta_Nota

                                    impostaDateRicetta(Ricetta_DataInizio, Ricetta_DataFine, Qs_Ricetta_Cod, parametri.Data, objParametri_Server)

                                End If
                                'caso salva e nuova ricetta
                                If Ricetta_Des = "" Then
                                    Ricetta_Des = parametri.Lav_Des
                                End If

                                Dim pivaR = parametri.Piva
                                Dim sa_codR = parametri.Centro_Aziendale
                                Dim Veg_CodR = 0
                                Dim Id_CodR = 0
                                Dim Programmazione_CodR = 0
                                If IsNumeric(parametri.Programmazione_Cod) Then
                                    Programmazione_CodR = CInt(parametri.Programmazione_Cod)
                                End If

                                If parametri.Specie <> "" Then
                                    Dim array_Specie = parametri.Specie.Split("/")

                                    If array_Specie.Length >= 1 Then
                                        If array_Specie.Length = 2 Then
                                            Veg_CodR = array_Specie(1)
                                        Else
                                            Id_CodR = array_Specie(0)
                                        End If
                                    End If
                                End If

                                If parametri.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua And Qs_Ricetta_Cod <> "" AndAlso Qs_Ricetta_Cod <> "0" Then
                                    Dim objRicetteRDAL As New AgronicaCoreContabDAL.Ricette_R
                                    Dim dtTestata = objRicetteRDAL.Leggi(Qs_Ricetta_Cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                                    If dtTestata IsNot Nothing AndAlso dtTestata.Rows.Count > 0 Then
                                        pivaR = dtTestata.Rows(0)("Piva")
                                        sa_codR = dtTestata.Rows(0)("sa_cod")
                                        Veg_CodR = dtTestata.Rows(0)("Veg_Cod")
                                        Programmazione_CodR = dtTestata.Rows(0)("Programmazione_Cod")
                                    End If
                                End If

                                Dim strXMLRicetta As String = objRicetteR.XML_GeneraStringa_Ricetta(pivaR, sa_codR, Veg_CodR,
                                                                                                Qs_Ricetta_Cod, Ricetta_Des, Ricetta_DataInizio, Ricetta_DataFine, ricetta_numero, Ricetta_Nota,
                                                                                                parametri.TipoOperazioneDB, parametri.TipoRicetta, Programmazione_CodR,
                                                                                                listaXMLRicettaOperazione,
                                                                                                HttpContext.Current.Session("ASG_ProgressivoGIAS"), objParametri_Server)

                                Dim objRicetta_Write As New AgronicaCoreContabBIZ.Ricette_W
                                Dim Ricetta_Cod As Integer = 0
                                objRicetta_Write.Ricetta_Scrivi(strXMLRicetta, Ricetta_Cod, objParametri_Server)

                                If Qs_Ricetta_Cod = "0" Then
                                    Qs_Ricetta_Cod = Ricetta_Cod
                                End If

                            End If


                        End If

                    Next



                Case Else

                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE SINGOLO CENTRO    -----------
                    '--------------------------------------------------
                    '--------------------------------------------------


                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpianti, parametri.Centro_Aziendale,
                                                                           messaggio_errore, parametri, tuttelerighe,
                                                                           BaseCode, TopCode)
                    If IsNothing(Agenda) Then
                        Throw New Exception("Non è stato possibile creare l'operazione." & messaggio_errore)
                    End If

                    'Salvataggio Tabelle Agenda
                    If parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                        Dim objAgendaScrivi As New Agenda_Operazione_Helper




                        If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                            'Recupero i vecchi costi CdG prima che vengano cancellati
                            Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                            Dim lista_mdRif As List(Of Movimento_Dettaglio_Riferimento) = mdr_Rif.LeggiRiferimentiAgenda(Agenda.Piva, 0, Agenda.Id_Agenda, 0, "", objParametri_Server)

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Attivita.Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(parametri.Piva,
                                                                                                parametri.Centro_Aziendale,
                                                                                                parametri.Id_Agenda, False,
                                                                                                objParametri_Server, logCancellazione:=False)

                            'Riscrivo i vecchi costi
                            Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                                If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                    mdRif_helper.Scrivi(mdRif, objParametri_Server)
                                    Exit For
                                End If
                            Next

                        End If

                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        objAgendaScrivi = Nothing

                        'se sono n scrittura devo agganciare la ricetta se presente
                        If Id_Agenda <> 0 And parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                            ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'ìoperazione
                            If Not IsNothing(HttpContext.Current.Session("UtilizzataRicetta")) AndAlso
                                    HttpContext.Current.Session("UtilizzataRicetta") Then

                                Dim ricetta_cod As String = HttpContext.Current.Session("ricetta_cod")
                                Dim Ricetta_Operazione_Cod As String = HttpContext.Current.Session("Ricetta_Operazione_Cod")

                                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                                1)
                                If Impostazione_RicetteXagenda <> "0" Then
                                    Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                    If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                        Throw New Exception("<b>Non è riuscito l'aggancio della ricetta!</b><br>")
                                    End If

                                    Dim objAppHelper As New AppHelper
                                    If Not objAppHelper.Aggiorna_Agenda_Dati_APP(Id_Agenda, ricetta_cod, Ricetta_Operazione_Cod, objParametri_Server) Then
                                        Throw New Exception("<b>Non è riuscito l'aggancio della ricetta arrivata dal sistema esterno!</b> <br>")
                                    End If

                                End If



                            End If
                        End If

                        'Salvataggio Tabelle Ricette
                    ElseIf parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse
                            parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then

                        Dim listaXMLRicettaOperazione As New List(Of String)

                        Dim rif_Lav_Cod As Integer
                        Dim rif_Des_Lib As String = ""
                        Dim rif_Data As Date
                        Dim rif_Note As String = ""

                        Dim Dpi_Cod As Integer = 0
                        Dim Dpi_PubblicoPrivato As Integer = 0


                        'una volta creato l'oggetto agenda posso invocare il suo metodo che mi genera l'xml
                        Dim StringaXmlOperazione As String = ""
                        Dim Helper As New Agenda_Operazione_Helper

                        StringaXmlOperazione = Helper.GeneraXML_CAU_LAVORAZIONI(Agenda)

                        If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Attivita.Tipo_Attivita.Ricetta, Qs_Ricetta_Cod)
                        End If

                        'creo la ricetta_operazione dall'xml dell'agenda
                        Dim objRicettaOp As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                        Dim StringaXmlRicettaOperazione As String = objRicettaOp.XML_GeneraStringa_Ricetta_Operazione(parametri.TipoOperazioneDB, Qs_Ricetta_Cod, Qs_Operazione_Ricetta,
                                                                                                                      parametri.TipoOperazioneAgenda, HttpContext.Current.Session("ASG_ProgressivoGIAS"), objParametri_Server,
                                                                                                                      StringaXmlOperazione, rif_Lav_Cod, rif_Des_Lib, rif_Data, rif_Note,
                                                                                                                      Agenda.Data_Creazione, Agenda.Username_Creazione)

                        listaXMLRicettaOperazione.Add(StringaXmlRicettaOperazione)

                        '---------------------------------------------------
                        ' SALVATAGGIO SE RICETTA
                        '---------------------------------------------------

                        If listaXMLRicettaOperazione.Count > 0 Then

                            If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                                Dim objRicettaOp_Read As New AgronicaCoreContabBIZ.Ricette_Operazioni_R
                                Dim daCancellare As String = objRicettaOp_Read.Ricetta_Operazioni_Leggi(Qs_Ricetta_Cod, Qs_Operazione_Ricetta, 0, 0, AGRODATAINIZIO, AGRODATAFINE, True, objParametri_Server)

                                Dim ret_Operazione_Ricetta As Integer = 0
                                Dim objRicettaOp_Write As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
                                objRicettaOp_Write.Ricetta_Operazione_Scrivi(parametri.Piva, parametri.Centro_Aziendale, daCancellare, Qs_Ricetta_Cod, enum_TipoRicetta.Standard_Destinazioni, ret_Operazione_Ricetta, objParametri_Server)
                            End If

                            'Se sono in fase di passaggio da "Da Fare" a "Fatto" allora recupero il numero ricetta
                            Dim ricetta_numero As String = ""
                            If parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio AndAlso Qs_Operazione_Ricetta <> "" Then
                                'Recupero il ricetta_numero per non perderlo
                                Dim Ric_R As New AgronicaCoreContabDAL.Ricette_R
                                Dim dtRicetta As DataTable = Ric_R.Leggi(Qs_Ricetta_Cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                ricetta_numero = DBNullToNothing(dtRicetta.Rows(0).Item("ricetta_numero"))
                            End If

                            'Creo la ricetta con la testata se Qs_Ricetta_Cod = "0"
                            'aggiungo solo le operazioni della ricetta se Qs_Ricetta_Cod <> "0" 
                            Dim objRicetteR As New AgronicaCoreContabBIZ.Ricette_R
                            Dim Ricetta_Des As String = parametri.Lav_Des
                            Dim Ricetta_Nota As String = ""
                            Dim Ricetta_DataInizio As Date = parametri.Data
                            Dim Ricetta_DataFine As Date = parametri.Data

                            Dim Ricetta_Testata = parametri.RicettaTestata

                            If parametri.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura AndAlso
                                   parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta AndAlso
                                    Qs_Ricetta_Cod = "0" Then

                                Ricetta_Des = Ricetta_Testata.Ricetta_Descrizione
                                ricetta_numero = Ricetta_Testata.Ricetta_Numero
                                'prendo la data inizio editata solo se <= alla data dell'operazione
                                If Ricetta_Testata.Data_Inizio_Ricetta <= Ricetta_DataInizio Then
                                    Ricetta_DataInizio = Ricetta_Testata.Data_Inizio_Ricetta
                                End If
                                'prendo la data fine editata solo se >= alla data dell'operazione
                                If Ricetta_Testata.Data_Fine_Ricetta >= Ricetta_DataFine Then
                                    Ricetta_DataFine = Ricetta_Testata.Data_Fine_Ricetta
                                End If
                                Ricetta_Nota = Ricetta_Testata.Ricetta_Nota

                                Dim dataOperazione As Date = parametri.Data

                                If dataOperazione < Ricetta_DataInizio Then
                                    Ricetta_DataInizio = dataOperazione
                                End If

                                If dataOperazione > Ricetta_DataFine Then
                                    Ricetta_DataFine = dataOperazione
                                End If

                            ElseIf Qs_Ricetta_Cod <> "0" AndAlso
                                        parametri.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then

                                Ricetta_Des = Ricetta_Testata.Ricetta_Descrizione
                                ricetta_numero = Ricetta_Testata.Ricetta_Numero
                                'prendo la data inizio editata solo se <= alla data dell'operazione
                                If Ricetta_Testata.Data_Inizio_Ricetta <= Ricetta_DataInizio Then
                                    Ricetta_DataInizio = Ricetta_Testata.Data_Inizio_Ricetta
                                End If
                                'prendo la data fine editata solo se >= alla data dell'operazione
                                If Ricetta_Testata.Data_Inizio_Ricetta >= Ricetta_DataFine Then
                                    Ricetta_DataFine = Ricetta_Testata.Data_Fine_Ricetta
                                End If
                                Ricetta_Nota = Ricetta_Testata.Ricetta_Nota

                                impostaDateRicetta(Ricetta_DataInizio, Ricetta_DataFine, Qs_Ricetta_Cod, parametri.Data, objParametri_Server)

                            End If

                            'caso salva e nuova ricetta
                            If Ricetta_Des = "" Then
                                Ricetta_Des = parametri.Lav_Des
                            End If

                            Dim pivaR = parametri.Piva
                            Dim sa_codR = parametri.Centro_Aziendale
                            Dim Veg_CodR = 0
                            Dim Id_CodR = 0
                            Dim Programmazione_CodR = 0
                            If IsNumeric(parametri.Programmazione_Cod) Then
                                Programmazione_CodR = CInt(parametri.Programmazione_Cod)
                            End If

                            If parametri.Specie <> "" Then
                                Dim array_Specie = parametri.Specie.Split("/")

                                If array_Specie.Length >= 1 Then
                                    If array_Specie.Length = 2 Then
                                        Veg_CodR = array_Specie(1)
                                    Else
                                        Id_CodR = array_Specie(0)
                                    End If
                                End If
                            End If


                            If parametri.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua And Qs_Ricetta_Cod <> "" AndAlso Qs_Ricetta_Cod <> "0" Then
                                Dim objRicetteRDAL As New AgronicaCoreContabDAL.Ricette_R
                                Dim dtTestata = objRicetteRDAL.Leggi(Qs_Ricetta_Cod, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                                If dtTestata IsNot Nothing AndAlso dtTestata.Rows.Count > 0 Then
                                    pivaR = dtTestata.Rows(0)("Piva")
                                    sa_codR = dtTestata.Rows(0)("sa_cod")
                                    Veg_CodR = dtTestata.Rows(0)("Veg_Cod")
                                    Programmazione_CodR = dtTestata.Rows(0)("Programmazione_Cod")
                                End If
                            End If

                            Dim strXMLRicetta As String = objRicetteR.XML_GeneraStringa_Ricetta(pivaR, sa_codR, Veg_CodR,
                                                                                                Qs_Ricetta_Cod, Ricetta_Des, Ricetta_DataInizio, Ricetta_DataFine, ricetta_numero, Ricetta_Nota,
                                                                                                parametri.TipoOperazioneDB, parametri.TipoRicetta, Programmazione_CodR,
                                                                                                listaXMLRicettaOperazione,
                                                                                                HttpContext.Current.Session("ASG_ProgressivoGIAS"), objParametri_Server)

                            Dim objRicetta_Write As New AgronicaCoreContabBIZ.Ricette_W
                            Dim Ricetta_Cod As Integer = 0
                            objRicetta_Write.Ricetta_Scrivi(strXMLRicetta, Ricetta_Cod, objParametri_Server)

                            If Qs_Ricetta_Cod = "0" Then
                                Qs_Ricetta_Cod = Ricetta_Cod
                            End If

                        End If

                    End If


            End Select


            res = True


            Dim msg As String
            SalvaModificheImpianti(msg, objParametri_Server, tuttelerighe)
            If msg <> "" Then
                Throw New Exception("Non è stato possibile salvare le modifiche agli impianti. <br> " & ":" & msg)
            End If


            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            messaggio_errore = ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        Return New With {.SalvataggioCompletato = res, .Id_Agenda = Id_Agenda, .Qs_Ricetta_Cod = Qs_Ricetta_Cod}

    End Function

    Private Function CreaOggettoAgenda(ByVal ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto),
                                       ByVal Sa_Cod As Integer, ByRef messaggio_errore As String,
                                       ByVal parametri As SalvaIrrigazioneModel,
                                       ByVal tuttelerighe As List(Of CampiGrigliaIrrigazioneModel), ByVal BaseCode As Integer,
                                       ByVal TopCode As Integer) As Operazione_Agenda


        Dim Agenda As Operazione_Agenda

        'usato solo per trappole e massa, ignorato per conf e dis sessuale
        Dim inneschiNumTotale As Integer = 0

        '--------------AGENDA------------------
        If Not Crea_Agenda(ListaImp, Sa_Cod, Agenda, messaggio_errore, parametri, BaseCode, TopCode) Then
            Return Nothing
        End If


        '--------------NOTE------------------
        If Not Crea_Agenda_Note(Agenda, parametri) Then
            Return Nothing
        End If


        '---------------MOVIMENTI COSTI ACCESSORI--------
        'If Not Crea_Agenda_Movimento_CostiAccessori(Agenda) Then
        '    Return Nothing
        'End If


        '------------- MOVIMENTO RILIEVO / TRATTAMENTO---------
        'inneschiNumTotale usato solo per trappole e massa, ignorato per conf e dis sessuale
        Select Case CInt(parametri.Lav_Cod)

            Case LAVCOD_IRRIGAZIONE
                If Not Crea_Agenda_Movimento_Irrig(inneschiNumTotale, Agenda, messaggio_errore, parametri, tuttelerighe) Then
                    Return Nothing
                End If

            Case Else
                Throw New NotImplementedException
        End Select


        Return Agenda

    End Function


    Private Function Crea_Agenda(ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto),
                                 ByRef Sa_Cod As Integer, ByRef Agenda As Operazione_Agenda,
                                 ByRef messaggio_errore As String, ByVal parametri As SalvaIrrigazioneModel,
                                 ByVal BaseCode As Integer, ByVal TopCode As Integer) As Boolean

        '---------------------------------------
        ' recupero la DATA

        Dim Data As Date
        If parametri.Data = AGRODATAINIZIO Then
            messaggio_errore = "Indicare una data!"
            Return False
        Else
            Data = parametri.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Id_Cod As String = ""
        Dim Veg_Des As String = ""
        If parametri.Specie = "-1" Or parametri.Specie = "0" Or parametri.Specie = "" Then
            messaggio_errore = messaggio_errore & "Selezionare una specie vegetale!"
            Return False
        Else

            Dim array_Specie = parametri.Specie.Split("/")

            If array_Specie.Length >= 1 Then
                If array_Specie.Length = 2 Then
                    Veg_Cod = array_Specie(1)
                Else
                    Id_Cod = array_Specie(0)
                End If
            End If

            Veg_Des = parametri.Specie_Des
        End If


        '---------------------------------------
        'recupero la OPERAZIONE
        Dim Lav_Des As String = ""
        If parametri.Lav_Cod = 0 Then
            messaggio_errore = messaggio_errore & "Selezionare un'operazione!"
            Return False
        Else
            Lav_Des = parametri.Lav_Des
        End If




        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------

        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim StrVarieta As String = ""
        For i = 0 To ListaImp.Count - 1
            trovato = False
            For j = 0 To ListaVarieta.Count - 1
                If ListaVarieta(j) = ListaImp(i).Cul_Des Then
                    trovato = True
                    Exit For
                End If
            Next
            If trovato = False Then
                ListaVarieta.Add(ListaImp(i).Cul_Des)
                StrVarieta += ", " + ListaImp(i).Cul_Des
            End If
        Next

        StrVarieta = StrVarieta.Substring(2, (StrVarieta.Length - 2))


        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = parametri.TipoOperazioneDB
        Agenda.Id_Agenda = parametri.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = parametri.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = parametri.Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" + StrVarieta + "])"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        Return True
    End Function


    Private Function Crea_Agenda_Note(ByRef Agenda As Operazione_Agenda,
                                      ByVal parametri As SalvaIrrigazioneModel) As Boolean
        Dim Nota As Nota


        If parametri.Note.ElencoNoteSelezionate.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To parametri.Note.ElencoNoteSelezionate.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = parametri.Id_Agenda
                Nota.Nota_Cod = parametri.Note.ElencoNoteSelezionate(i)
                Agenda.Note.Add(Nota)
            Next
        End If


        Return True
    End Function

    Private Function Crea_Agenda_Movimento_Irrig(ByRef inneschiNumTotale As Integer, ByRef Agenda As Operazione_Agenda,
                                                 ByRef messaggio_errore As String, ByVal parametri As SalvaIrrigazioneModel,
                                                 ByVal tuttelerighe As List(Of CampiGrigliaIrrigazioneModel)) As Boolean


        Select Case CInt(parametri.Lav_Cod)

            Case LAVCOD_IRRIGAZIONE
                'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
            Case Else
                Throw New Exception
        End Select

        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = parametri.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = parametri.Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = parametri.Cau_Mov
        Movimento_OperazioneColturale.Mov_Desc = parametri.Note.Txt_Note
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode

        '----------------------------------------------------------
        '----- MOVIMENTI DETTAGLI , DET.TECNICI, DESTINAZIONI -----
        '----------------------------------------------------------
        'per ciascuna riga, quindi per ciascun impianto
        'creo un movimento dettaglio (se è inserito almeno un dato),
        'un mov dettaglio tecnico e 
        'un movimento destinazione

        'CALCOLO LA QTA2
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = GetImpiantiIrrigazione(tuttelerighe, parametri)
        Dim ListaImpiantixQuestoSaCod As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) =
                                                    (From x As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti
                                                     Where x.Sa_Cod = Movimento_OperazioneColturale.Sa_Cod).ToList()

        Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In ListaImpiantixQuestoSaCod
                Select CType(ST.Qta2, Decimal)
            ).Sum


        Dim iRiga = 1

        For Each rigaIrrigazione As CampiGrigliaIrrigazioneModel In tuttelerighe 'scansiono le righe 
            'Solo gli impianti selezionati
            If CBool(rigaIrrigazione.Selected) = True Then
                'ricavo il dato da salvare nella cella
                Dim dose As String = String.Empty
                If Not rigaIrrigazione.Dose Is Nothing Then
                    dose = rigaIrrigazione.Dose.ToString().Trim()
                End If

                Dim ore As String = String.Empty
                If Not rigaIrrigazione.Ore Is Nothing Then
                    ore = rigaIrrigazione.Ore.ToString().Trim()
                End If

                Dim portata As String = String.Empty
                If Not rigaIrrigazione.Portata Is Nothing Then
                    portata = rigaIrrigazione.Portata.ToString().Trim()
                End If

                Dim Qta_Tot As String = String.Empty
                If Not rigaIrrigazione.Qta_Totale Is Nothing Then
                    Qta_Tot = rigaIrrigazione.Qta_Totale.ToString().Trim()
                End If

                Dim data_i As String = String.Empty
                If Not rigaIrrigazione.Data_Inizio Is Nothing Then
                    data_i = rigaIrrigazione.Data_Inizio.ToString().Trim()
                End If

                Dim data_f As String = String.Empty
                If Not rigaIrrigazione.Data_Fine Is Nothing Then
                    data_f = rigaIrrigazione.Data_Fine.ToString().Trim()
                End If

                Dim frequenza As String = String.Empty
                If Not rigaIrrigazione.Frequenza Is Nothing Then
                    frequenza = rigaIrrigazione.Frequenza.ToString().Trim()
                End If

                Dim TipoIrrig As String = String.Empty
                If Not rigaIrrigazione.ModifTipoIrriUtilizzata_Cod Is Nothing Then
                    TipoIrrig = rigaIrrigazione.ModifTipoIrriUtilizzata_Cod.ToString().Trim()
                End If

                Dim ID_DSS_Irrigazione As Integer = 0
                If Not rigaIrrigazione.ID_DSS_Irrigazione Is Nothing Then
                    ID_DSS_Irrigazione = rigaIrrigazione.ID_DSS_Irrigazione
                End If

                Dim Data_Consiglio_Custom As Date = AGRODATAINIZIO
                If Not rigaIrrigazione.Data_Consiglio_Custom Is Nothing Then
                    Data_Consiglio_Custom = rigaIrrigazione.Data_Consiglio_Custom
                End If

                Dim Qta_Acqua_Consiglio_Custom As Decimal = 0
                If Not rigaIrrigazione.Qta_Acqua_Consiglio_Custom Is Nothing Then
                    Qta_Acqua_Consiglio_Custom = rigaIrrigazione.Qta_Acqua_Consiglio_Custom
                End If

                If TipoIrrig = "-1" Then
                    'uso quella dell'impianto
                    TipoIrrig = CStr(rigaIrrigazione.Imp_Cod)
                End If
                If TipoIrrig = "-1" Then
                    'Se ancora -1 Allora non è impostata nell'impianto, uso 0
                    TipoIrrig = 0
                End If

                'controllo i dati da salvare, se controllo tutti anche quelli di altri centri sono sicuro che 
                'non scriverò sulo alcune operazioni di agenda e altre no

                'se la data non è impostata in nessuno assumo agrodatainizio e fine di default
                If data_i = "" AndAlso data_f = "" Then
                    data_i = AGRODATAINIZIO
                    data_f = AGRODATAFINE
                End If

                'se la frequenza non è impostata lascio 0 di default
                If frequenza = "" Then
                    frequenza = "0"
                End If

                'se la frequenza non è impostata lascio 0 di default
                If portata = "" Then
                    portata = "0"
                End If

                'se la frequenza non è impostata lascio 0 di default
                If ore = "" Then
                    ore = "0"
                End If

                If Not checkNumber(dose, iRiga, 4, "dose", messaggio_errore, parametri.Lav_Cod) Or
                          Not checkNumber(ore, iRiga, 5, "ore", messaggio_errore, parametri.Lav_Cod) Or
                          Not checkNumber(portata, iRiga, 6, "portata", messaggio_errore, parametri.Lav_Cod) Or
                          Not checkNumber(Qta_Tot, iRiga, 7, "Qta_Tot", messaggio_errore, parametri.Lav_Cod) Or
                          Not checkDate(data_i, iRiga, 8, messaggio_errore) Or
                          Not checkDate(data_f, iRiga, 9, messaggio_errore) Or
                          Not checkNumber(frequenza, iRiga, 10, "frequenza", messaggio_errore, parametri.Lav_Cod) Then
                    Return False
                End If

                If CDec(ore) = 0 And CInt(portata) <> 0 Then
                    messaggio_errore &= String.Format("Il dato nella Riga:  {0}  non è ammissibile! <br/>", iRiga)
                    messaggio_errore &= "Se le ore sono '0' anche la portata deve esserlo."
                    Return False
                End If

                If CDec(ore) <> 0 And CInt(portata) = 0 Then
                    messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br/>", iRiga)
                    messaggio_errore &= "Se la portata è '0' anche le ore  devono esserlo"
                    Return False
                End If

                'se la data non è impostata in nessuno assumo agrodatainizio e fine di default
                If data_i = AGRODATAINIZIO AndAlso data_f = AGRODATAFINE AndAlso frequenza = "0" Then
                    'ok
                ElseIf data_i <> AGRODATAINIZIO AndAlso data_f <> AGRODATAFINE AndAlso frequenza <> "0" Then
                    'ok
                Else
                    messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br/>", iRiga)
                    messaggio_errore &= "Data inizio, data fine e frequenza devono essere tutti vuoti o tutti valorizzati"
                    Return False
                End If

                'se la datai è maggiore della dataf fermo
                If DateTime.Compare(data_i, data_f) > 0 Then
                    messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br />", iRiga)
                    messaggio_errore &= "Data inizio è successivo a  data fine"
                    Return False
                End If

                'controllo microirrigazione
                If parametri.Verifica_Compatibilita_MicroIrrigazione = "true" Then

                    If data_i <> AGRODATAINIZIO And TipoIrrig <> "31" And TipoIrrig <> "33" And TipoIrrig <> "111" And TipoIrrig <> "112" Then
                        If TipoIrrig = "-1" Then
                            'non dovrebbe succedere, verificato prima se -1 e assegnato tipoirr impianto
                            Throw New NotImplementedException
                        End If
                        messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br/>", iRiga)
                        messaggio_errore &= "Non è possibile salvare la Microirrigazione per un impianto di Irrigazione non compatibile. <br/>"
                        messaggio_errore &= "Cambiare il tipo di irrigazione o togliere la data e la frequenza."
                        Return False
                    End If
                End If

                If Data_Consiglio_Custom <> AGRODATAINIZIO AndAlso Qta_Acqua_Consiglio_Custom = 0 Then
                    messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br/>", iRiga)
                    messaggio_errore &= "Indicare una Dose Consiglio. <br/>"
                    Return False
                End If

                If Data_Consiglio_Custom = AGRODATAINIZIO AndAlso Qta_Acqua_Consiglio_Custom > 0 Then
                    messaggio_errore &= String.Format("Il dato nella Riga:{0}  non è ammissibile! <br/>", iRiga)
                    messaggio_errore &= "Indicare una Data Consiglio. <br/>"
                    Return False
                End If

                '------------------------------
                '----- MOVIMENTO DETTAGLIO-----
                '------------------------------
                Dim Movimento_Dettaglio_Contabilizzato As Integer = 1
                Dim Movimento_Dettaglio_Pendente As Integer = 3
                Dim Movimento_Dettaglio_Extra_Date As Date = AGRODATAINIZIO
                Dim Movimento_Dettaglio_Anno As Integer = 1900

                Dim Movimento_Dettaglio As New Movimento_Dettaglio
                Movimento_Dettaglio.Id_Agenda = parametri.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio.Elem_Cod = 0
                Movimento_Dettaglio.Pro_Cod = 0
                Movimento_Dettaglio.Udm_Cod = 0
                Movimento_Dettaglio.Qta = 0
                Movimento_Dettaglio.Contabilizzato = Movimento_Dettaglio_Contabilizzato
                Movimento_Dettaglio.Pendente = Movimento_Dettaglio_Pendente
                Movimento_Dettaglio.Extra_Date = Movimento_Dettaglio_Extra_Date
                Movimento_Dettaglio.Anno = Movimento_Dettaglio_Anno
                Movimento_Dettaglio.Data = Agenda.Data
                Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                Movimento_Dettaglio.TopCode = Agenda.TopCode

                'impostazioni in base alla lavorazione
                Select Case CInt(parametri.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                    Case Else
                        Throw New NotImplementedException
                End Select


                '------------------------------
                '----- MOVIMENTO DET TECNICO --
                '------------------------------
                Dim Movimento_Dettaglio_Tecnico As New Movimento_Dettaglio_Tecnico
                Movimento_Dettaglio_Tecnico.Id_Agenda = parametri.Id_Agenda
                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio_Tecnico.Data = Agenda.Data
                Movimento_Dettaglio_Tecnico.Ditta_cod = 0
                Movimento_Dettaglio_Tecnico.ff_classe = "0"
                Movimento_Dettaglio_Tecnico.Sigla_av = "0"
                Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode

                Select Case CInt(parametri.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                        Movimento_Dettaglio_Tecnico.Qta_Ril = CDec(dose)
                        Movimento_Dettaglio_Tecnico.dett_cod = CInt(parametri.Unita_di_Misura_Dose)
                        Movimento_Dettaglio_Tecnico.Dose = CDec(ore)
                        Movimento_Dettaglio_Tecnico.Parziale = CInt(portata)
                        Movimento_Dettaglio_Tecnico.Nitrati = CInt(frequenza)
                        If TipoIrrig = "-1" Then
                            TipoIrrig = "0" 'superfluo, già controllato sopra
                        End If
                        Movimento_Dettaglio_Tecnico.Freatimetro = CInt(TipoIrrig)
                        Movimento_Dettaglio_Tecnico.Inn1_data = CDate(data_i)
                        Movimento_Dettaglio_Tecnico.Inn2_data = CDate(data_f)
                        Movimento_Dettaglio_Tecnico.Extra_Int = ID_DSS_Irrigazione
                        Movimento_Dettaglio_Tecnico.ExtraStr = Qta_Acqua_Consiglio_Custom.ToString()
                        Movimento_Dettaglio_Tecnico.Extra_Date = Data_Consiglio_Custom
                    Case Else
                        Throw New NotImplementedException
                End Select


                '-----------------------------------
                '----- MOVIMENTI DESTINAZIONE ------
                '-----------------------------------
                Dim Movimento_Destinazione As New Movimento_Destinazione
                Movimento_Destinazione.Id_Agenda = parametri.Id_Agenda
                Movimento_Destinazione.Piva = rigaIrrigazione.PIVA
                Movimento_Destinazione.Sa_Cod = rigaIrrigazione.SA_COD

                If Movimento_Destinazione.Piva <> Agenda.Piva Or Movimento_Destinazione.Sa_Cod <> Agenda.Sa_Cod Then
                    'la riga non appartiene al centro per cui si sta creando l'operazione agenda, quindi salto e passo alal riga seguente
                    Dim esco As Boolean = True 'variabile per debug
                Else

                    Movimento_Destinazione.Appezza = rigaIrrigazione.APPEZZA
                    Movimento_Destinazione.Id_Destinazione = rigaIrrigazione.ID_REG

                    Movimento_Destinazione.BaseCode = Agenda.BaseCode
                    Movimento_Destinazione.TopCode = Agenda.TopCode

                    Movimento_Destinazione.Qta = 0 'aggiorno dopo mentre leggo righe per fare i Movimento_Dettaglio_Tecnico
                    Select Case CInt(parametri.Lav_Cod)
                        Case LAVCOD_IRRIGAZIONE
                            Movimento_Destinazione.Qta = CDec(Qta_Tot)
                            Movimento_Destinazione.Data = Agenda.Data


                            For Each imp In ListaImpiantixQuestoSaCod
                                If Movimento_Destinazione.Piva = imp.Piva AndAlso Movimento_Destinazione.Sa_Cod = imp.Sa_Cod _
                                       AndAlso Movimento_Destinazione.Appezza = imp.Appezza AndAlso Movimento_Destinazione.Id_Destinazione = imp.ID_Reg Then

                                    Movimento_Destinazione.Qta2 = imp.Qta2

                                    If xCalcolo_QD_SuperficieTotale <> 0 Then
                                        Movimento_Destinazione.QuotaDistribuzione = imp.Qta2 / xCalcolo_QD_SuperficieTotale
                                    End If

                                End If
                            Next

                        Case Else
                            Throw New NotImplementedException
                    End Select


                    'aggiungo il movimento destinazione al movimento detaglio
                    Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                    'aggiungo il movimento dettaglio tecnico al movimento dettaglio
                    Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                    'aggiungo il movimento dettaglio al movimento
                    Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)

                End If
            End If

            iRiga = iRiga + 1

        Next

        'Se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        '(il movimento dettaglio contiene una destinazione e un mov dettaglio tecnico)
        'Se il movimento  non ha almeno un dettaglio allora genero una eccezione,
        'perchè una situazione del genere non dovrebbe mai accadere in quanto deve essere creato almeno un 
        'movimento dettaglio per ciascun centro, e tutti gli impianti selezionati nella master devono 
        'creare un movimentoi dettaglio corrispondente, anche se su centri diversi.

        If Movimento_OperazioneColturale.Movimenti_Dettagli.Count > 0 Then
            Agenda.Movimenti.Add(Movimento_OperazioneColturale)
        Else
            messaggio_errore = messaggio_errore & "Errore non previsto!"
            Return False
        End If

        Return True

    End Function

    Private Function SalvaModificheImpianti(ByRef App_Non_Modificati As String, ByVal objParametri_Server As AgronicaCoreParametri,
                                            ByVal tuttelerighe As List(Of CampiGrigliaIrrigazioneModel)) As Boolean

        Dim Imp_Cod_New As Integer
        Dim Imp_Cod_Old As Integer
        Dim ModificatoImp As Boolean
        Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write


        For Each righeIrrigazione As CampiGrigliaIrrigazioneModel In tuttelerighe

            If CBool(righeIrrigazione.Selected) = True Then
                If Not righeIrrigazione.ModifTipoIrriUtilizzata_Cod Is Nothing Then
                    Imp_Cod_New = CInt(righeIrrigazione.ModifTipoIrriUtilizzata_Cod)
                End If

                If Not righeIrrigazione.Imp_Cod Is Nothing Then
                    Imp_Cod_Old = CStr(righeIrrigazione.Imp_Cod)
                End If

                If Not righeIrrigazione.PIVA = "" AndAlso
                   Not righeIrrigazione.SA_COD Is Nothing AndAlso
                   Not righeIrrigazione.APPEZZA Is Nothing AndAlso
                   Not righeIrrigazione.ID_REG Is Nothing Then

                    If Imp_Cod_New <> -1 Then

                        If Imp_Cod_Old <> Imp_Cod_New Then
                            ModificatoImp = objImpianto.ModificaSingolo_CampoNumerico(CStr(righeIrrigazione.PIVA),
                                                          CInt(righeIrrigazione.SA_COD),
                                                          CInt(righeIrrigazione.APPEZZA),
                                                          CInt(righeIrrigazione.ID_REG),
                                                          "Imp_Cod",
                                                          Imp_Cod_New,
                                                          "", objParametri_Server)

                            If ModificatoImp = False Then
                                App_Non_Modificati &= righeIrrigazione.App_Nome.ToString() & ","
                            End If
                        End If

                    End If
                End If
            End If
        Next



        Return True

    End Function


    Private Function checkNumber(ByVal valoreRilevato As String, ByVal riga As Integer,
                                 ByVal colonna As Integer, ByVal NomeColonna As String, ByRef messaggio_errore As String, ByVal Lav_Cod As Integer) As Boolean

        Dim causa As String = ""
        If valoreRilevato.Contains(".") Then
            causa = "Utilizzare la virgola ',' e non il punto '.' per i decimali"
        End If
        If causa = "" Then
            Select Case CInt(Lav_Cod)
                Case LAVCOD_IRRIGAZIONE
                    If Not IsNumeric(valoreRilevato) Then
                        causa = "Non è un numero"
                    End If
                    If IsNumeric(valoreRilevato) AndAlso CInt(valoreRilevato) < 0 Then
                        causa = "Non è un numero positivo"
                    End If

                Case Else
                    Throw New NotImplementedException
            End Select
        End If


        If causa <> "" Then

            If Not String.IsNullOrEmpty(NomeColonna) Then
                messaggio_errore &= String.Format("Il dato nella cella R: {0} - C:{1} non è ammissibile! ( {2} - {3} )", riga, NomeColonna, valoreRilevato, causa)
                Return False
            End If

        End If
        Return True
    End Function

    Private Function checkDate(ByVal valoreRilevato As String, ByVal riga As Integer, ByVal colonna As Integer, ByRef messaggio_errore As String) As Boolean

        Dim causa As String = ""
        If Not IsDate(valoreRilevato) Then
            causa = "Data non valida. Impostare tutti i valori data inizio, data fine e frequenza, oppure lasciarli tutti vuoti."
        End If

        If causa <> "" Then
            messaggio_errore &= String.Format("Il dato nella cella riga: {0} non è ammissibile! ( {1} - {2} )", riga, valoreRilevato, causa)
            Return False
        End If
        Return True
    End Function

    Private Sub impostaDateRicetta(ByRef Ricetta_DataInizio As Date, ByRef Ricetta_DataFine As Date,
                                        ByVal Ricetta_Cod As Integer, ByVal dataOperazione As Date,
                                        ByVal objParametri_Server As AgronicaCoreParametri)

        Dim objRicette_Operazioni As New AgronicaCoreContabDAL.Ricette_R
        Dim maxData = objRicette_Operazioni.Leggi_MaxData(Ricetta_Cod, "", objParametri_Server)
        Dim minData = objRicette_Operazioni.Leggi_MinData(Ricetta_Cod, "", objParametri_Server)

        If dataOperazione > maxData Then
            maxData = dataOperazione
        End If

        If dataOperazione < minData Then
            minData = dataOperazione
        End If

        If Ricetta_DataInizio > minData Then
            Ricetta_DataInizio = minData
        End If

        If Ricetta_DataFine < maxData Then
            Ricetta_DataFine = maxData
        End If

    End Sub

    Private Sub gestisciTipoSalvataggio(ByVal TipoSalvataggio As Integer,
                                        ByVal TipoOperazioneAgenda As Integer,
                                        ByVal PaginaSitoOrigine As String,
                                        ByVal Qs_Ricetta_Cod As String,
                                        ByVal tipo_salva_parametri_Irrigazione As String,
                                        ByVal piva As String,
                                        ByVal veg_cod As String,
                                        ByVal sa_cod As Integer,
                                        ByVal data As Date,
                                        ByRef link As String,
                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                        ByVal objParametriAgenda As ParametriAgenda)

        HttpContext.Current.Session("UtilizzataRicetta") = False

        objParametriAgenda.Data = data

        If TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then

            Select Case TipoSalvataggio
                Case enum_Tipo_Salvataggio.Salva_e_Esci
                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim DTConfigSiti As DataTable

                    Try
                        Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
                        Dim paginaOnLineRitorno As Integer = PaginaSitoOrigine

                        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                   enum_PagineAgenda_2010.Menu, piva, "", "", 0, "")

                        ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                            Enum_SiteRedirector.GiasNG,
                                                                            objParametriAgenda.PaginaSitoOrigine,
                                                                            link,
                                                                            objParametri_Server)

                        Else
                            DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
                            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                                link = "../menu/menubs_agenda_nuovo.aspx"
                            End If
                        End If

                    Catch ex As Exception
                        DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
                        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                            link = "../menu/menubs_agenda_nuovo.aspx"
                        End If
                    End Try

                    RipristinaSessione()

                Case enum_Tipo_Salvataggio.Salva_e_Nuovo

                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Note = New List(Of Nota)
                    objParametriAgenda.Movimenti = New List(Of Movimento)

                    objParametriAgenda.Veg_Cod = veg_cod
                    objParametriAgenda.Sa_Cod = sa_cod

                    link = "../Operazioni/IrrigazioneBS.aspx"

                Case enum_Tipo_Salvataggio.Salva_e_Duplica

                    'Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

                    '    Dim PaginaLink As String = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                    '    Dim link As String = ""
                    '    Try
                    '        Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                    '        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                    '        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                    '            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                    '                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    '                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                    '                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    '        Else
                    '            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    '        End If

                    '    Catch ex As Exception
                    '        link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    '    End Try

                    '    PaginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                    '                  "&id_agenda=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                    '                  "&origine=" & Stringa_Codifica(link, AgroKey_EncoderDecoder) &
                    '                  "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)

                    'Dim strJS As New StringBuilder
                    'strJS.AppendLine("$(document).ready(function () { ")
                    'strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                    'strJS.AppendLine("      window.location = '" & PaginaLink & "'; ")
                    'strJS.AppendLine(" });")

                    'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                    '                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                    'Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                    '                      CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                    'RipristinaSessione()
                    'sblocca()

            End Select

        ElseIf TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta OrElse
                TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio Then

            Select Case TipoSalvataggio

                Case enum_Tipo_Salvataggio.Salva_e_Esci

                    If PaginaSitoOrigine <> enum_PagineAgenda_2010.Gis Then


                        Try
                            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
                            Dim paginaOnLineRitorno As Integer = PaginaSitoOrigine

                            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                           enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                           enum_PagineAgenda_2010.Menu, piva, "", "", 0, "")

                            ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                Enum_SiteRedirector.GiasNG,
                                                                                objParametriAgenda.PaginaSitoOrigine,
                                                                                link,
                                                                                objParametri_Server)

                            Else

                                link = "../menu/menubs_agenda_nuovo.aspx"
                            End If

                        Catch ex As Exception
                            link = "../menu/menubs_agenda_nuovo.aspx"
                        End Try
                    End If

                    RipristinaSessione()


                Case enum_Tipo_Salvataggio.Salva_e_Nuovo

                    objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                    objParametriAgenda.Note = New List(Of Nota)
                    objParametriAgenda.Movimenti = New List(Of Movimento)

                    objParametriAgenda.Veg_Cod = veg_cod
                    objParametriAgenda.Sa_Cod = sa_cod

                    link = "../Operazioni/IrrigazioneBS.aspx"

                Case enum_Tipo_Salvataggio.Salva_e_Duplica

                Case 5 'salva ricetta e nuovo dettaglio

                    'tipo_salva_parametri_Irrigazione sarebbe nella Trattamenti_2 l'hdf:
                    Dim strParametriAgg = tipo_salva_parametri_Irrigazione
                    Dim ricetta_operazione_cod As Integer = 0
                    If strParametriAgg <> "" Then
                        Dim obj = JObject.Parse(strParametriAgg)
                        ricetta_operazione_cod = CInt(obj("ricetta_operazione_cod"))
                        If ricetta_operazione_cod <> 0 Then
                            objParametriAgenda.Data = CDate(CStr(obj("data")))
                            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
                        Else
                            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                        End If
                    End If

                    If Qs_Ricetta_Cod <> "0" And ricetta_operazione_cod = 0 Then
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                        objParametriAgenda.Veg_Cod = veg_cod
                        objParametriAgenda.Sa_Cod = sa_cod
                        'se sono in una ricetta e faccio salva e nuovo , creo la nuova ricetta come figlia della stessa testata
                        link = "../Operazioni/IrrigazioneBS.aspx?r=" & Stringa_Codifica(Qs_Ricetta_Cod, AgroKey_EncoderDecoder, Nothing) & ""
                    ElseIf Qs_Ricetta_Cod <> "0" And ricetta_operazione_cod <> 0 Then
                        objParametriAgenda.Veg_Cod = veg_cod
                        objParametriAgenda.Sa_Cod = sa_cod
                        link = "../Operazioni/IrrigazioneBS.aspx?operazione_ricetta=" & Stringa_Codifica(ricetta_operazione_cod, AgroKey_EncoderDecoder, Nothing) & "&r=" & Stringa_Codifica(Qs_Ricetta_Cod, AgroKey_EncoderDecoder, Nothing) & ""
                    Else

                        link = "../Operazioni/IrrigazioneBS"
                    End If

            End Select
        End If

    End Sub



    Private Sub RipristinaSessione()
        Dim objParametriAgenda As New ParametriAgenda
        ripristinaObjParametriAgenda(objParametriAgenda)
    End Sub

    Private Sub ripristinaObjParametriAgenda(ByVal objParametriAgenda As ParametriAgenda)
        objParametriAgenda.Svuota_DatiOperazione()
        objParametriAgenda.OperazioneMulticentro = True
    End Sub

    ''' <summary>
    ''' Sostiusco i caratteri %&£ con i doppi apici
    ''' </summary>
    Private Function RispristinaApp_Nome(ByVal App_Nome_OLD As String) As String

        Dim App_Nome_NEW As String = String.Empty

        If Not IsNothing(App_Nome_OLD) AndAlso Not String.IsNullOrEmpty(App_Nome_OLD) Then
            App_Nome_NEW = Replace(App_Nome_OLD, "$%&£", Chr(34))
        End If

        Return App_Nome_NEW
    End Function


#End Region



    Private Class SalvaIrrigazioneModel
        Public TipoOperazioneDB As Integer? = Nothing
        Public Piva As String = ""
        Public Data As Date? = Nothing
        '####da objParametriAgenda
        Public Id_Agenda As String = ""
        Public Lav_Des As String = ""
        Public Lav_Cod As Integer? = Nothing
        Public TargetOperazione As Integer? = Nothing
        Public Cau_Mov As String = ""
        Public PaginaSitoOrigine As String = ""
        Public Campo As String = ""
        Public Varieta As String = ""
        Public TipoOperazioneAgenda As Integer? = Nothing
        Public Programmazione_Cod As Integer? = Nothing
        Public TipoRicetta As Integer? = Nothing
        Public SitoOrigine As Integer? = Nothing
        '#####
        Public Centro_Aziendale As Integer? = Nothing
        Public Specie As String = ""
        Public Specie_Des As String = ""
        Public Tipo_Irrigazione As Integer? = Nothing
        Public Unita_di_Misura_Dose As String = ""
        Public Verifica_Compatibilita_MicroIrrigazione As String = ""
        Public Irrigazione As IrrigazioneModel
        Public Note As NoteModel
        Public RicettaTestata As RicettaModel
        Public tipo_salva_parametri_Irrigazione As String = ""
    End Class

    Private Class NoteModel
        Public Txt_Note As String = ""
        Public ElencoNoteSelezionate As List(Of String)
    End Class

    Private Class RicettaModel
        Public Ricetta_Descrizione As String
        Public Ricetta_Numero As String
        Public Data_Inizio_Ricetta As Date? = Nothing
        Public Data_Fine_Ricetta As Date? = Nothing
        Public Ricetta_Nota As String
    End Class

    Private Class CampiGrigliaIrrigazioneModel
        Public APPEZZA As Integer? = Nothing
        Public ID_REG As Integer? = Nothing
        Public Progetto_Cod As Integer? = Nothing
        Public PIVA As String = ""
        Public App_Nome As String = ""
        Public Campo_Des As String = ""
        Public Catasto As String = ""
        Public CodBioApp As String = ""
        Public Codici_Anagrafe_Des As String = ""
        Public Cul_Des As String = ""
        Public CultivarAgea As String = ""
        Public Data_Fine As Date? = Nothing
        Public Data_Inizio As Date? = Nothing
        Public Data_Fioritura As Date? = Nothing
        Public Data_Fioritura_Prevista As Date? = Nothing
        Public Data_Raccolta As Date? = Nothing
        Public Data_Raccolta_Prevista As Date? = Nothing
        Public Data_Semina As Date? = Nothing
        Public Data_Semina_Prevista As Date? = Nothing
        Public Validita_Inizio As Date? = Nothing
        Public Validita_Fine As Date? = Nothing
        Public Validita_Inizio_Distinta As Date? = Nothing
        Public Validita_Fine_Distinta As Date? = Nothing
        Public Disciplinare As String = ""
        Public Dose As Decimal? = Nothing
        Public Frequenza As Integer? = Nothing
        Public Grfi_Des As String = ""
        Public Id_Grid_Irrigazione As String = ""
        Public Imp_Cod As Integer? = Nothing
        Public Imp_Des As String = ""
        Public ModifTipoIrriUtilizzata_Cod As Integer? = Nothing
        Public Ore As Decimal? = Nothing
        Public Portata As Integer? = Nothing
        Public Qta2 As Decimal? = Nothing
        Public Qta_Totale As Decimal? = Nothing
        Public Qta_Totale_Acqua_Periodo As Decimal? = Nothing
        Public Reg_Des As String = ""
        Public RifNumerico As String = ""
        Public SA_COD As Integer? = Nothing
        Public Sa_Nome As String = ""
        Public Selected As String = ""
        Public SelezionaImpianto As String = ""
        Public SpecieAgea As String = ""
        Public Sup_Imp As Decimal? = Nothing
        Public kendoKey As String = ""
        Public ID_DSS_Irrigazione As Integer? = 0
        Public Descrizione_DSS_Irrigazione As String = ""
        Public Qta_Acqua_DSS_Irrigazione As Decimal = 0
        Public Udm_Cod_DSS_Irrigazione As Integer = 0
        Public Qta_Acqua_Consiglio_Custom As Decimal? = Nothing
        Public Data_Consiglio_Custom As Date? = Nothing
        Public Min_Data_Turno As Date? = Nothing
        Public Max_Data_Turno As Date? = Nothing
    End Class

    Private Class IrrigazioneModel : Inherits GrigliaModel

    End Class

    Private Class GrigliaModel
        Public RigheInserite As String
        Public RigheModificate As String
        Public RigheEliminate As String
        Public TutteLeRighe As String
    End Class

End Class
