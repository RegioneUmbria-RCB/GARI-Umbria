Imports AgronicaCoreModello.OperazioneAgenda_Temp

Public Interface iOperazioneGUI

    'Gestione del salvataggio da evento
    Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

    'Salvataggio vero e proprio
    Function SalvaOperazioneAgenda(ByRef messaggio_errore As String, ByRef Unid_Operazione As String) As Boolean

    'Operazioni successive al salvataggio
    Sub fine_salvataggio(ByVal TipoSalvataggio As Integer, ByVal Unid_Operazione As String)



    'Caricamento
    Sub Ripristina_Dati_nei_Controlli()
    Sub Ripristina_Dati_nei_Controlli_xRicetta(ByVal Xml_Operazione As String)
    Sub Ripristina_Dati_nei_ControlliDaRicetta()

End Interface


Public Interface iOperazioneGUI_Trattamento
    Inherits iOperazioneGUI

    'Salvataggio vero e proprio in trattamento
    Overloads Function SalvaOperazioneAgenda(
                           ByVal TipoSalvataggio As Integer, _
                           ByRef messaggio_errore As String, _
                           ByRef Unid_Operazione As String, _
                           ByRef ListaOpAgenda As List(Of Operazione_Agenda), _
                           ByRef AgendaScarico As Operazione_Agenda, _
                           ByRef messaggio_alert As String) As Boolean

End Interface



Public Interface iOperazioneGUI_Semina

    'Predispone la struttuda del datatable con le colonne
    Sub aggiungiColonneSemineImpianti(ByRef dt As DataTable)

    'Predispone i datakey e chiama funzioni di finalizzazione.
    Sub Finalizza_GridViewPlanning(dt As DataTable)
    Sub Finalizza_GridViewImpianti(dt As DataTable)

    'Ripristino dei dati letti quando si apre in info o modifica...
    Sub Ripristina_Dati_nei_Controlli_TabellaSemineImpianti()
    Sub Ripristina_Dati_nei_Controlli_TabellaSeminePlanning()
    Sub Ripristina_Dati_nei_Controlli_Tabella_Magazzino()

    'in modifica e lettura prima di caricare le tabelle leggo dall'operazione agenda i valori 
    'da inserire nelle liste dei valori e che serviranno per imostare i check e i valori nelle tabelle
    'e nei parametri agenda come fabbricato, data, specie
    Sub CaricaListeValori_e_LeggiParametri_DaAgenda()

End Interface

