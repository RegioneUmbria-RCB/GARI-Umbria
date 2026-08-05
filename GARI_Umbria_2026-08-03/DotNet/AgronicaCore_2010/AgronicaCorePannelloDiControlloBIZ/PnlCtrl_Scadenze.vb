Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePannelloDiControlloDAL

Public Class Scadenze_R

    Dim CHIAVE_ARRAY_CONTATTI As String() = {"Piva", "Cod_Contatto"}

    Public Function leggi_PnlCtrl_Elementi( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer _
                                ) As List(Of PnlCtrl_Scadenze)

        Return leggi_Scadenze(objParametri, ID, Nothing, Nothing, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_Scadenze( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer?, _
                                ByVal ID_Categoria As Integer?, _
                                ByVal Piva As String, _
                                ByVal Utente As String, _
                                ByVal DataScadenza As DateTime?, _
                                ByVal ID_ListaAllegati As Integer? _
                              ) As List(Of PnlCtrl_Scadenze)

        Dim r As New AgronicaCorePannelloDiControlloDAL.Scadenze_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Scadenze)

        Dim dt As DataTable = r.Leggi_Scadenze(ID, ID_Categoria, Piva, Utente, DataScadenza, _
                                                ID_ListaAllegati, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows

            Dim elem As New PnlCtrl_Scadenze( _
                                          UtilityProvider.DBNullToNothing(dRow("ID")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Categoria")), _
                                          UtilityProvider.DBNullToNothing(dRow("Piva")), _
                                          UtilityProvider.DBNullToNothing(dRow("Utente")), _
                                          UtilityProvider.DBNullToNothing(dRow("DataScadenza")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_ListaAllegati")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Gravita")), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          UtilityProvider.DBNullToNothing(dRow("TabDettaglio_Nome")), _
                                          UtilityProvider.DBNullToNothing(dRow("TabDettaglio_Chiave")), _
                                          UtilityProvider.DBNullToNothing(dRow("Note")), _
                                          UtilityProvider.DBNullToNothing(dRow("Ricorrenza")), _
                                          UtilityProvider.DBNullToNothing(dRow("PreavvisoInfo")), _
                                          UtilityProvider.DBNullToNothing(dRow("PreavvisoWarning")), _
                                          UtilityProvider.DBNullToNothing(dRow("PreavvisoError")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

    Public Function Leggi_Scadenze_Patentino_DaAggiungereInPnlCtrl( _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As List(Of PnlCtrl_Scadenze)

        Dim scad_r As New AgronicaCorePannelloDiControlloDAL.Scadenze_R
        Dim cat_r As New AgronicaCorePannelloDiControlloDAL.Categorie_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Scadenze)

        'leggo tutti gli elementi patentino mai inseriti nel Pannello di controllo
        Dim dt_scad As DataTable = scad_r.Leggi_Scadenze_Patentino_DaAggiungereInPnlCtrl( _
                                                xFiltroAggiuntivo, xOrderBy, objParametri)

        'proseguo solo se ho elementi
        If dt_scad.Rows.Count > 0 Then

            'leggo il codice della categoria patentino
            Dim dt_cat As DataTable = cat_r.Leggi_Categorie(Nothing, Nothing, "Persone", "Patentino", _
                                                            "", "", objParametri)

            If dt_cat.Rows.Count < 1 Then
                'dare errore
            End If

            Dim idCat_PersonePatentino As Integer = dt_cat.Rows(0).Item("ID")

            For Each dRow As DataRow In dt_scad.Rows
                'creo un oggetto scadenza con i parametri di default
                Dim elem As New PnlCtrl_Scadenze()

                'metto in una stringa ben formata i dettagli del patentino
                Dim note As String = ""
                If Not IsDBNull(dRow("Data_Rilascio_Patentino")) OrElse _
                    Not IsDBNull(dRow("Ente_di_rilascio")) OrElse Not IsDBNull(dRow("Patentino")) Then

                    note = "Dettaglio Patentino: "
                    note &= IIf(IsDBNull(dRow("Patentino")), "", "Num=" & dRow("Patentino") & " - ")
                    note &= IIf(IsDBNull(dRow("Data_Rilascio_Patentino")), "", "Data rilascio=" & dRow("Data_Rilascio_Patentino") & " - ")
                    note &= IIf(IsDBNull(dRow("Ente_di_rilascio")), "", "Ente di rilascio=" & dRow("Ente_di_rilascio"))

                    note = IIf(note.EndsWith(" - "), note.Substring(0, note.Length - 3), note)
                End If

                'estraggo la chiave della tabella contatti
                Dim tabDettaglio_Chiave As String = ""
                For Each x As String In CHIAVE_ARRAY_CONTATTI
                    tabDettaglio_Chiave &= (x & "=" & dRow(x) & "~")
                Next
                tabDettaglio_Chiave = tabDettaglio_Chiave.TrimEnd("~")

                'imposto i valori specifici della scadenza
                elem.Piva = dRow("Piva")
                elem.ID_Categoria = idCat_PersonePatentino
                elem.Utente = "Ricerca Automatica"
                elem.DataScadenza = dRow("Data_Scadenza_Patentino")
                elem.Descrizione = "Patentino di " & dRow("Nome") & " " & dRow("Cognome") & " (CF:" & dRow("Cod_Contatto") & ")"
                elem.TabDettaglio_Nome = "Contatti"
                elem.TabDettaglio_Chiave = tabDettaglio_Chiave
                elem.Note = note

                elem.ID_Categoria = idCat_PersonePatentino

                'aggiungo l'oggetto alla lista
                listaObjPnlCtrl.Add(elem)
            Next

        End If

        Return listaObjPnlCtrl

    End Function

    Public Function Leggi_Scadenze_Patentino_DaModificareInPnlCtrl( _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As List(Of PnlCtrl_Scadenze)

        Dim scad_r As New AgronicaCorePannelloDiControlloDAL.Scadenze_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Scadenze)

        'leggo tutti gli elementi patentino mai inseriti nel Pannello di controllo
        Dim dt_scad As DataTable = scad_r.Leggi_Scadenze_Patentino_DaModficareInPnlCtrl( _
                                                xFiltroAggiuntivo, xOrderBy, objParametri)

        For Each dRow As DataRow In dt_scad.Rows
            'leggo il vecchio elemento
            Dim elem As PnlCtrl_Scadenze = leggi_PnlCtrl_Elementi(objParametri, dRow("ID")).First()

            'metto in una stringa ben formata i dettagli del patentino
            Dim note As String = ""
            If Not IsDBNull(dRow("Data_Rilascio_Patentino")) OrElse _
                Not IsDBNull(dRow("Ente_di_rilascio")) OrElse Not IsDBNull(dRow("Patentino")) Then

                note = "Dettaglio Patentino: "
                note &= IIf(IsDBNull(dRow("Patentino")), "", "Num=" & dRow("Patentino") & " - ")
                note &= IIf(IsDBNull(dRow("Data_Rilascio_Patentino")), "", "Data rilascio=" & dRow("Data_Rilascio_Patentino") & " - ")
                note &= IIf(IsDBNull(dRow("Ente_di_rilascio")), "", "Ente di rilascio=" & dRow("Ente_di_rilascio"))

                note = IIf(note.EndsWith(" - "), note.Substring(0, note.Length - 3), note)
            End If

            'imposto i valori specifici della scadenza
            elem.Utente = "Ricerca Automatica"
            elem.DataScadenza = dRow("Data_Scadenza_Patentino")
            elem.Descrizione = "Patentino di " & dRow("Nome") & " " & dRow("Cognome") & " (CF:" & dRow("Cod_Contatto") & ")"
            elem.Note = note

            'aggiungo l'oggetto alla lista
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class Scadenze_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                        ByVal elem As PnlCtrl_Scadenze
                        ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Scadenze_W
        Dim res As Boolean = w.Scrivi(objParametri, _
                        elem.ID, elem.ID_Categoria, elem.Piva, _
                        elem.Utente, elem.DataScadenza, elem.ID_ListaAllegati, elem.ID_Gravita, _
                        elem.Descrizione, elem.TabDettaglio_Nome, elem.TabDettaglio_Chiave, elem.Note, _
                        elem.Ricorrenza, elem.PreavvisoInfo, elem.PreavvisoWarning, elem.PreavvisoError)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer, _
                                ByVal ID_Categoria As Integer, _
                                ByVal Piva As String, _
                                ByVal Utente As String, _
                                ByVal DataScadenza As DateTime, _
                                ByVal ID_ListaAllegati As Integer?, _
                                ByVal ID_Gravita As Integer?, _
                                ByVal Descrizione As String, _
                                ByVal TabDettaglio_Nome As String, _
                                ByVal TabDettaglio_Chiave As String, _
                                ByVal Note As String, _
                                ByVal Ricorrenza As String, _
                                ByVal PreavvisoInfo As Integer?, _
                                ByVal PreavvisoWarning As Integer?, _
                                ByVal PreavvisoError As Integer? _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Scadenze_W
        Dim res As Boolean = w.Scrivi(objParametri, ID, ID_Categoria, Piva, _
                         Utente, DataScadenza, ID_ListaAllegati, ID_Gravita, _
                         Descrizione, TabDettaglio_Nome, TabDettaglio_Chiave, Note, _
                         Ricorrenza, PreavvisoInfo, PreavvisoWarning, PreavvisoError)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_ID As Integer, _
                                ByVal New_ID_Categoria As Integer, _
                                ByVal New_Piva As String, _
                                ByVal New_Utente As String, _
                                ByVal New_DataScadenza As DateTime?, _
                                ByVal New_ID_ListaAllegati As Integer?, _
                                ByVal New_ID_Gravita As Integer?, _
                                ByVal New_Descrizione As String, _
                                ByVal New_TabDettaglio_Nome As String, _
                                ByVal New_TabDettaglio_Chiave As String, _
                                ByVal New_Note As String, _
                                ByVal New_Ricorrenza As String, _
                                ByVal New_PreavvisoInfo As Integer?, _
                                ByVal New_PreavvisoWarning As Integer?, _
                                ByVal New_PreavvisoError As Integer? _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Scadenze_W
        Dim res As Boolean = w.Modifica(objParametri, Old_ID, New_ID_Categoria, New_Piva, New_Utente, _
                         New_DataScadenza, New_ID_ListaAllegati, New_ID_Gravita, New_Descrizione, _
                         New_TabDettaglio_Nome, New_TabDettaglio_Chiave, New_Note, _
                         New_Ricorrenza, New_PreavvisoInfo, New_PreavvisoWarning, New_PreavvisoError)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal s As PnlCtrl_Scadenze _
                              ) As Boolean

        Dim res As Boolean = modifica(objParametri, s.ID, s.ID_Categoria, s.Piva, _
                         s.Utente, s.DataScadenza, s.ID_ListaAllegati, s.Descrizione, _
                         s.TabDettaglio_Nome, s.TabDettaglio_Chiave, s.Note, s.Ricorrenza, s.ID_Gravita, _
                         s.PreavvisoInfo, s.PreavvisoWarning, s.PreavvisoError)

        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByVal ID As Integer _
                                       ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.Scadenze_W
        Dim res As Boolean = True

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica

        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        res = elemW.Cancella("", ID, objParametri)

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res

    End Function

End Class

Public Class PnlCtrl_Scadenze
    Public Property ID As Integer
    Public Property ID_Categoria As Integer
    Public Property Piva As String
    Public Property Utente As String
    Public Property DataScadenza As DateTime
    Public Property ID_ListaAllegati As Integer?
    Public Property ID_Gravita As Integer?
    Public Property Descrizione As String
    Public Property TabDettaglio_Nome As String
    Public Property TabDettaglio_Chiave As String
    Public Property Note As String
    Public Property Ricorrenza As String
    Public Property PreavvisoInfo As Integer?
    Public Property PreavvisoWarning As Integer?
    Public Property PreavvisoError As Integer?


    Public Sub New(ID As Integer, _
                    ID_Categoria As Integer, _
                    Piva As String, _
                    Utente As String, _
                    DataScadenza As DateTime, _
                    ID_ListaAllegati As Integer?, _
                    ID_Gravita As Integer?, _
                    Descrizione As String, _
                    TabDettaglio_Nome As String, _
                    TabDettaglio_Chiave As String, _
                    Note As String, _
                    Ricorrenza As String, _
                    PreavvisoInfo As Integer?, _
                    PreavvisoWarning As Integer?, _
                    PreavvisoError As Integer? _
                    )

        _ID = ID
        _ID_Categoria = ID_Categoria
        _Piva = Piva
        _Utente = Utente
        _DataScadenza = DataScadenza
        _ID_ListaAllegati = ID_ListaAllegati
        _ID_Gravita = ID_Gravita
        _Descrizione = Descrizione
        _TabDettaglio_Nome = TabDettaglio_Nome
        _TabDettaglio_Chiave = TabDettaglio_Chiave
        _Note = Note
        _Ricorrenza = Ricorrenza
        _PreavvisoInfo = PreavvisoInfo
        _PreavvisoWarning = PreavvisoWarning
        _PreavvisoError = PreavvisoError

    End Sub

    Public Sub New()
        _ID = -1
        _ID_Categoria = -1
        _Piva = ""
        _Utente = ""
        _DataScadenza = Date.Now
        _ID_ListaAllegati = Nothing
        _ID_Gravita = Nothing
        _Descrizione = Nothing
        _TabDettaglio_Nome = Nothing
        _TabDettaglio_Chiave = Nothing
        _Note = Nothing
        _Ricorrenza = Nothing
        _PreavvisoInfo = Nothing
        _PreavvisoWarning = Nothing
        _PreavvisoError = Nothing
    End Sub

End Class