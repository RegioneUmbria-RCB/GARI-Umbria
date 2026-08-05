Imports AgronicaCoreCapitolatoClienteDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Imports <xmlns="http://ws_CapitolatoCliente_VerificaAnalisi">

Public Class Analisi_Testata
    Private _PivaSuperUser As String
    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Private _progressivo As String
    Public Property Progressivo() As String
        Get
            Return _progressivo
        End Get
        Set(value As String)
            _progressivo = value
        End Set
    End Property

    Private _descrizione As String
    Public Property Descrizione() As String
        Get
            Return _descrizione
        End Get
        Set(value As String)
            _descrizione = value
        End Set
    End Property

    Private _DataAnalisi As DateTime
    Public Property DataAnalisi() As DateTime
        Get
            Return _DataAnalisi
        End Get
        Set(value As DateTime)
            _DataAnalisi = value
        End Set
    End Property

    Private _Veg_Cod As Integer

    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(value As Integer)
            _Veg_Cod = value
        End Set
    End Property

    Private _Cul_Cod As String
    Public Property Cul_Cod() As String
        Get
            Return _Cul_Cod
        End Get
        Set(value As String)
            _Cul_Cod = value
        End Set
    End Property


    Private _PrincipiAttiviRilevati As Analisi_TestataXPrincipiAttiviRilevati()
    Public Property PrincipiAttiviRilevati() As Analisi_TestataXPrincipiAttiviRilevati()
        Get
            Return _PrincipiAttiviRilevati
        End Get
        Set(value As Analisi_TestataXPrincipiAttiviRilevati())
            _PrincipiAttiviRilevati = value
        End Set
    End Property

    Private _FamigliePrincipiAttiviRilevati As Analisi_TestataXFamigliePrincipiAttiviRilevati()
    Public Property FamigliePrincipiAttiviRilevati() As Analisi_TestataXFamigliePrincipiAttiviRilevati()
        Get
            Return _FamigliePrincipiAttiviRilevati
        End Get
        Set(value As Analisi_TestataXFamigliePrincipiAttiviRilevati())
            _FamigliePrincipiAttiviRilevati = value
        End Set
    End Property

    Private _DPI As Analisi_TestataXDPI()
    Public Property DPI() As Analisi_TestataXDPI()
        Get
            Return _DPI
        End Get
        Set(value As Analisi_TestataXDPI())
            _DPI = value
        End Set
    End Property

    Private _CapitolatoCliente As Analisi_TestataXCapitolatoCliente()
    Public Property CapitolatoCliente() As Analisi_TestataXCapitolatoCliente()
        Get
            Return _CapitolatoCliente
        End Get
        Set(value As Analisi_TestataXCapitolatoCliente())
            _CapitolatoCliente = value
        End Set
    End Property

    Public Sub Cancella(ByVal objParametri As AgronicaCoreParametri)
        Dim DAL_Analisi_Testata As New Analisi_Testata_W
        Dim DAL_Analisi_TestataXCapitolatoCliente As New Analisi_TestataXCapitolatoCliente_W
        Dim DAL_Analisi_TestataXDPI As New Analisi_TestataXDPI_W
        Dim DAL_Analisi_TestataXPrincipiAttiviRilevati As New Analisi_TestataXPrincipiAttiviRilevati_W



    End Sub

    ''' <summary>
    ''' salva l'istanza temporanea delle analisi per successive elaborazioni
    ''' </summary>
    ''' <param name="uniqueID"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>id univoco dell'istanza temporanea dove si sta lavorando</returns>
    ''' <remarks></remarks>
    Public Function Salva(ByVal objParametri As AgronicaCoreParametri, ByVal uniqueID As String) As String



        'Dim DPI_inseriti As New List(Of Analisi_TestataXDPI)
        Dim DPI_inseriti As New Hashtable

        Dim DAL_Analisi_Testata As New Analisi_Testata_W
        Dim DAL_Analisi_TestataXCapitolatoCliente As New Analisi_TestataXCapitolatoCliente_W
        Dim DAL_Analisi_TestataXDPI As New Analisi_TestataXDPI_W
        Dim DAL_Analisi_TestataXPrincipiAttiviRilevati As New Analisi_TestataXPrincipiAttiviRilevati_W
        Dim DAL_Analisi_TestataXFamigliePrincipiAttiviRilevati As New Analisi_TestataXFamigliePrincipiAttiviRilevati_W
        Dim DAL_Analisi_TestataXLFSCultivarRilevate As New Analisi_TestataXLFSCultivarRilevate

        DAL_Analisi_Testata.Scrivi(_PivaSuperUser, _progressivo, _descrizione, _DataAnalisi, _Veg_Cod, uniqueID, objParametri)

        For Each itemCultivar In _Cul_Cod.ToString.Split(",")
            DAL_Analisi_TestataXLFSCultivarRilevate.Scrivi(_PivaSuperUser, _progressivo, _Veg_Cod, CInt(itemCultivar), uniqueID, objParametri)
        Next

        For Each itemCapitolatoCliente As Analisi_TestataXCapitolatoCliente In _CapitolatoCliente
            DAL_Analisi_TestataXCapitolatoCliente.Scrivi(_PivaSuperUser, _progressivo, itemCapitolatoCliente.CapitolatoCod, uniqueID, objParametri)
        Next

        For Each itemDPI As Analisi_TestataXDPI In _DPI
            If Not DPI_inseriti.ContainsKey(itemDPI.Flag_Privato_Pubblico.ToString & "-" & itemDPI.Cod_Regolamento.ToString) Then
                DAL_Analisi_TestataXDPI.Scrivi(_PivaSuperUser, _progressivo, itemDPI.Flag_Privato_Pubblico, itemDPI.Cod_Regolamento, uniqueID, objParametri)
                DPI_inseriti.Add(itemDPI.Flag_Privato_Pubblico.ToString & "-" & itemDPI.Cod_Regolamento.ToString, itemDPI)
            End If

        Next

        For Each itemPA As Analisi_TestataXPrincipiAttiviRilevati In _PrincipiAttiviRilevati
            DAL_Analisi_TestataXPrincipiAttiviRilevati.Scrivi(_PivaSuperUser, _progressivo, itemPA.PA_Cod, itemPA.QtaRilevata, itemPA.QtaRilevataUdmGias, uniqueID, objParametri)
        Next

        For Each itemFPA As Analisi_TestataXFamigliePrincipiAttiviRilevati In _FamigliePrincipiAttiviRilevati
            DAL_Analisi_TestataXFamigliePrincipiAttiviRilevati.Scrivi(_PivaSuperUser, _progressivo, itemFPA.Fam_Cod, itemFPA.QtaRilevata, itemFPA.QtaRilevataUdmGias, uniqueID, objParametri)
        Next

        DAL_Analisi_TestataXDPI.CopiaDPI_IndicatiSuCapitolatoSuIstanzaTemporanea(_PivaSuperUser, _progressivo, uniqueID, objParametri)

        Return uniqueID
    End Function

    Private Function ReadXmlFromString(ByVal stringaXml As String) As XDocument
        Return XDocument.Parse(stringaXml)
    End Function

    Private Sub Verifica_Analisi_CancellaIstanzaAnalisi(ByVal objParametri As AgronicaCoreParametri, ByVal uniqueID As String)
        Dim appDal As New Analisi_Testata_W
        appDal.CancellaDatiTempByUniqueID(uniqueID, objParametri)
    End Sub

    Private Function Verifica_Analisi_SalvaIstanzaAnalisi(ByVal stringaXmlAnalisiDaVerificare As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim ObjSequenze = New Agro_Sequenze

        'Richiedo un nuovo codice temporaneo
        Dim uniqueID As String = ObjSequenze.NuovoId_Tabella( _
                        "CapitolatoClienteAnalisiUniqueID", _
                        0, _
                        Int32.MaxValue, _
                        objParametri)
        Dim xDoc As XDocument = ReadXmlFromString(stringaXmlAnalisiDaVerificare)


        Dim ElencoAnalisi As List(Of Analisi_Testata) = _
            (From a In xDoc.<root>.<analisi> _
             Select New Analisi_Testata With { _
                  .PivaSuperUser = a.@pivaSuperUser, _
                  .Progressivo = a.@id, _
                  .DataAnalisi = a.@dataAnalisi, _
                  .Veg_Cod = a.<datiInput>.<specie>.Value, _
                  .Cul_Cod = a.<datiInput>.<varieta>.Value, _
                  .PrincipiAttiviRilevati = ( _
                      From pa In a.<datiInput>.<principiAttivi> _
                      Select New Analisi_TestataXPrincipiAttiviRilevati With { _
                            .PA_Cod = pa.<paCod>.Value, _
                            .QtaRilevata = pa.<qtaRilevata>.Value.ToString().Replace(".", ","), _
                            .QtaRilevataUdmGias = pa.<qtaRilevataUdmGIAS>.Value _
                    }).ToArray, _
                  .FamigliePrincipiAttiviRilevati = ( _
                      From fpa In a.<datiInput>.<famigliePrincipiAttivi> _
                      Select New Analisi_TestataXFamigliePrincipiAttiviRilevati With { _
                            .Fam_Cod = fpa.<famCod>.Value, _
                            .QtaRilevata = fpa.<qtaRilevata>.Value.ToString().Replace(".", ","), _
                            .QtaRilevataUdmGias = fpa.<qtaRilevataUdmGIAS>.Value _
                    }).ToArray, _
                    .DPI = ( _
                        From dpi In a.<datiInput>.<dpiImpianto> _
                        Select New Analisi_TestataXDPI With { _
                            .Flag_Privato_Pubblico = dpi.<dpiFlagPrivatoPubblico>.Value, _
                            .Cod_Regolamento = dpi.<dpiCodRegolamento>.Value _
                    }).ToArray, _
                .CapitolatoCliente = ( _
                    From cc In a.<verifiche>.<capitolatiCliente>.<capitolatoCliente> _
                    Select New Analisi_TestataXCapitolatoCliente With { _
                        .CapitolatoCod = cc.<id>.Value _
                    }).ToArray
              }).ToList

        For Each analisi In ElencoAnalisi
            analisi.Salva(objParametri, uniqueID)
        Next


        Return uniqueID
    End Function

    Public Function Verifica_Analisi_EffettuaVerifica(ByVal StringaXMLAnalisiDaVerificare As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim FinalXDoc As XDocument
        Dim UniqueID As String
        Dim appoggio As New Analisi_TestataXCapitolatoCliente_R

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)


            'salva istanza temporanea
            'riversa i dati su database
            UniqueID = _
                Verifica_Analisi_SalvaIstanzaAnalisi(StringaXMLAnalisiDaVerificare, objParametri)


            FinalXDoc = XDocument.Parse(StringaXMLAnalisiDaVerificare)


            'creo la tb di appoggio
            appoggio.create_Tabelle_appggio(UniqueID, objParametri)

            'completo le tabelle di appoggio con dati mancanti per ovviare a problemi di codifiche et al.
            appoggio.completa_Tabelle_appggio(UniqueID, objParametri)


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '' Verifiche per la parte DPI

            'verifica che venga passato un DPI quando sul capitolato è indicato il flag "usa DPI impostato su impianto"
            verifica_passa_DPI_se_capitolato_ha_indicato_flagUsaImpianto(UniqueID, FinalXDoc, objParametri)

            'verifica principi attivi da DPI
            Verifica_Analisi_EffettuaVerifica_PA_DPI(UniqueID, FinalXDoc, objParametri)

            'verifica: 
            '  1. se il principio attivi è ammesso (esiste una RMA nell'ambito dei regolamenti impostati sui capitolati che vengono passati a questa funzione) 
            '  2. se la sua RMA viene superata: restituisce la sua RMA (se non ammesso RMA = -1)
            Verifica_Analisi_EffettuaVerifica_PA_Ammesso_DeddottoDa_RMA(UniqueID, FinalXDoc, objParametri)



            '' verifiche per la parte Capitolati privati
            ' di base
            Verifica_Analisi_EffettuaVerifica_PredisponiRisultatoVuoto(FinalXDoc)

            'verifica varietà ammesse dato capitolato
            Verifica_Analisi_EffettuaVerifica_VarietaAmmesse(UniqueID, FinalXDoc, objParametri)

            'verifica del conteggio delle percentuali sul singolo PA
            Verifica_Analisi_EffettuaVerifica_PercentualiResidui(UniqueID, FinalXDoc, objParametri)

            'verifica se esistono formulati validi
            Verifica_Analisi_EffettuaVerifica_Formulati(UniqueID, FinalXDoc, objParametri)

            'verifica se esistono formulati BIO
            Verifica_Analisi_EffettuaVerifica_FormulatiBIO(UniqueID, FinalXDoc, objParametri)


            'verifica del numero di PA
            Verifica_Analisi_EffettuaVerifica_NumeroPA(UniqueID, FinalXDoc, objParametri)

            'verifica della percentuale della Somma dei PA
            Verifica_Analisi_EffettuaVerifica_PercentualeSommaPa(UniqueID, FinalXDoc, objParametri)

            'vanni, 19/05/2017: verifica se l'azieda può partecipare al capitolato cliente
            Verifica_Analisi_EffettuaVerifica_EsclusioneImpresa(UniqueID, FinalXDoc, objParametri)

            'imposta l'esito globale sul capitolato
            Verifica_Analisi_EffettuaVerifica_PredisponiEsitoGlobaleCapitolatoCliente(FinalXDoc)


            'ripulisce i dati temporanei dal database
            'cancella se non esiste un elemento debug=true

            Dim xDoc As XDocument = ReadXmlFromString(StringaXMLAnalisiDaVerificare)


            PulisciDatiTemporanei(xDoc, FlagConnessioneLocale, FlagTransazioneLocale, objParametri, UniqueID)

            'delete la tb di appoggio
            PulisciTabelleAppoggio(appoggio, xDoc, FlagConnessioneLocale, FlagTransazioneLocale, objParametri, UniqueID)


            'If (From a In xDoc.<root>.<debug>).ToList.Count = 0 Then

            '    'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
            '                                                                            FlagTransazioneLocale, _
            '                                                                            objParametri)

            '    Verifica_Analisi_CancellaIstanzaAnalisi(objParametri, UniqueID)

            '    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            'End If

            'fine pulizia dati temporanei


        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            ''//////////////////////////////////////////////////////////////////////
            'MessaggioErrore = "(Piva=" + OUTPUT_Piva + ")" + _
            '                  "(Sa_Cod=" + CStr(OUTPUT_Sa_Cod) + ")" + _
            '                  "(Appezza=" + CStr(OUTPUT_Appezza) + ")" + _
            '                  " : " + ex.Message
            ''//////////////////////////////////////////////////////////////////////


            Throw New Exception("[" & "" & "] : " & "")

        Finally
            Dim xDoc As XDocument = ReadXmlFromString(StringaXMLAnalisiDaVerificare)
            PulisciDatiTemporanei(xDoc, FlagConnessioneLocale, FlagTransazioneLocale, objParametri, UniqueID)

            'delete la tb di appoggio
            PulisciTabelleAppoggio(appoggio, xDoc, FlagConnessioneLocale, FlagTransazioneLocale, objParametri, UniqueID)


            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            '    objParametri.objConnessione.Dispose()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return ResultToString(FinalXDoc)

    End Function

    Private Sub PulisciTabelleAppoggio(
        appoggio As Analisi_TestataXCapitolatoCliente_R,
        xDoc As XDocument, FlagConnessioneLocale As Boolean, FlagTransazioneLocale As Boolean,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal UniqueID As Integer)


        If (From a In xDoc.<root>.<debug>).ToList.Count = 0 Then

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)


            appoggio.drop_Tabella_appggio(UniqueID, objParametri)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        End If
    End Sub


    Private Sub PulisciDatiTemporanei(xDoc As XDocument, FlagConnessioneLocale As Boolean, FlagTransazioneLocale As Boolean, _
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                     ByVal UniqueID As Integer)


        If (From a In xDoc.<root>.<debug>).ToList.Count = 0 Then

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)

            Verifica_Analisi_CancellaIstanzaAnalisi(objParametri, UniqueID)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

        End If
    End Sub


    Private Sub verifica_passa_DPI_se_capitolato_ha_indicato_flagUsaImpianto(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)

        Dim AppCapitolato As New Analisi_TestataXCapitolatoCliente_R
        Dim AppDPI As New Analisi_TestataXDPI_R
        Dim DPObbligatori As DataTable = _
            AppCapitolato.verifica_passa_DPI_se_capitolato_ha_indicato_flagUsaImpianto(UniqueID, objParametri)

        Dim appdtDPI As DataTable = Nothing

        'scorre tutti i capitolati che non hanno un DP impostato e verifica che lo richiedano
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each cc In ( _
                From curNodoCC In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente> _
                Select curNodoCC).ToList

                Dim DPDato() As DataRow
                DPDato = DPObbligatori.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' " & _
                                                        "AND Progressivo = " & curNodoAnalisi.@id & _
                                                        "AND Capitolato_COD = " & cc.<id>.Value)

                Dim Impianto_dpiFlagPrivatoPubblico As Integer
                Dim Impianto_dpiCodRegolamento As Integer
                Dim Impianto_dpiDescrizione As String = ""


                Impianto_dpiFlagPrivatoPubblico = curNodoAnalisi.<datiInput>.<dpiImpianto>.<dpiFlagPrivatoPubblico>.Value
                Impianto_dpiCodRegolamento = curNodoAnalisi.<datiInput>.<dpiImpianto>.<dpiCodRegolamento>.Value


                If Not DP_Dato_EffettuaControlli(DPDato) Then
                    Dim noControl = <dpiCapitolatoImpianto>
                                        <noCheckDP/>
                                    </dpiCapitolatoImpianto>

                    cc.Add(noControl)

                Else

                    If DP_Dato_No_DPI(Impianto_dpiCodRegolamento, DPDato) Then

                        Dim warningDP = <dpiCapitolatoImpianto>
                                            <warning/>
                                        </dpiCapitolatoImpianto>

                        cc.Add(warningDP)
                    Else

                        If DPDato(0)("DPI_Impianto") = 0 Then
                            Dim dpiSpecifico = <dpiCapitolatoImpianto>
                                                   <dpiFlagPrivatoPubblico><%= DPDato(0)("Flag_Privato_Pubblico_DPI") %></dpiFlagPrivatoPubblico>
                                                   <dpiCodRegolamento><%= DPDato(0)("DPI_COD_REGOLAMENTO") %></dpiCodRegolamento>
                                                   <dpiDescrizione><%= DPDato(0)("DPI_Descrizione") %></dpiDescrizione>
                                               </dpiCapitolatoImpianto>
                            cc.Add(dpiSpecifico)
                        Else


                            appdtDPI = AppDPI.LeggiDPI(Impianto_dpiFlagPrivatoPubblico, Impianto_dpiCodRegolamento, objParametri)
                            If appdtDPI.Rows.Count <> 0 Then
                                Impianto_dpiDescrizione = appdtDPI.Rows(0)("NomeEsteso")
                            End If

                            Dim dpiSpecifico = <dpiCapitolatoImpianto>
                                                   <dpiFlagPrivatoPubblico><%= Impianto_dpiFlagPrivatoPubblico %></dpiFlagPrivatoPubblico>
                                                   <dpiCodRegolamento><%= Impianto_dpiCodRegolamento %></dpiCodRegolamento>
                                                   <dpiDescrizione><%= Impianto_dpiDescrizione %></dpiDescrizione>
                                               </dpiCapitolatoImpianto>
                            cc.Add(dpiSpecifico)

                        End If
                    End If

                End If

            Next

        Next
    End Sub

    Private Function DP_Dato_EffettuaControlli(ByVal DR As DataRow()) As Boolean
        If DR(0)("DPI_COD_Regolamento") Is DBNull.Value _
            AndAlso DR(0)("DPI_Impianto") = 0 Then

            Return False
        Else
            Return True
        End If
    End Function

    Private Function DP_Dato_No_DPI(ByVal Impianto_dpiCodRegolamento As Integer, ByVal DR As DataRow()) As Boolean
        If Impianto_dpiCodRegolamento = -1 And DR(0)("DPI_Impianto") = 1 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub Verifica_Analisi_EffettuaVerifica_PredisponiEsitoGlobaleCapitolatoCliente(ByRef XDoc As XDocument)


        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
               From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
               Select el).ToList

                If nodoCapitolato.<dpiCapitolatoImpianto>.<warning>.ToList.Count = 0 AndAlso _
                    nodoCapitolato.<dpiCapitolatoImpianto>.<dpiResponse>.ToList.Count > 0 AndAlso _
                    nodoCapitolato.<dpiCapitolatoImpianto>.<dpiResponse>.<esitoGlobale>.Value.ToLower = "true" Then

                    ' quelli vuoti sono test con esito positivo!
                    If nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "" Then
                        nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "True"
                    End If

                Else
                    If nodoCapitolato.<dpiCapitolatoImpianto>.<noCheckDP>.ToList.Count = 0 Then
                        nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "False"
                    Else
                        If nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "" Then
                            nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "True"
                        End If
                    End If

                End If


            Next



        Next



    End Sub

    Private Function ResultToString(ByVal sResult As XDocument) As String

        Return RemoveNamespace(sResult).ToString.Replace("<root>", "<root xmlns=""http://ws_CapitolatoCliente_VerificaAnalisi"">")

    End Function

    Private Sub Verifica_Analisi_EffettuaVerifica_PredisponiRisultatoVuoto(ByRef XDoc As XDocument)


        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList

                Dim xmlnodeDPI_EsitoGlobale = <capitolatoResponse>
                                                  <esitoGlobale></esitoGlobale>
                                              </capitolatoResponse>


                nodoCapitolato.Add(xmlnodeDPI_EsitoGlobale)


            Next
        Next


    End Sub






    Private Sub Verifica_Analisi_EffettuaVerifica_FormulatiBIO(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)
        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_VerificaPABIO(UniqueID, objParametri)

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList


                Dim dr() As DataRow
                dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                "Capitolato_COD = " & nodoCapitolato.<id>.Value)
                'scorre i principi attivi
                For Each curPA In dr

                    Dim icurPA As Integer = curPA("PA_o_Fam_COD")

                    If curPA("isFamiglia") = 0 Then

                         


                        Dim nodoPAEsiste = ( _
                            From nCur In nodoCapitolato.<capitolatoResponse>.<bioResponsePrincipioAttivo> _
                            Where nCur.<id>.Value = icurPA _
                            Select nCur).FirstOrDefault

                        If nodoPAEsiste Is Nothing Then
                            Dim xmlNode_checkFormulati_capitolato = <bioResponsePrincipioAttivo>
                                                                        <id><%= icurPA %></id>
                                                                        <checkFormulati>False</checkFormulati>
                                                                    </bioResponsePrincipioAttivo>
                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)
                        Else
                            nodoPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                        End If

                    Else

                        Dim nodoFamPAEsiste = ( _
                            From nCur In nodoCapitolato.<capitolatoResponse>.<bioResponseFamiglia> _
                            Where nCur.<id>.Value = icurPA _
                            Select nCur).FirstOrDefault

                        If nodoFamPAEsiste Is Nothing Then
                            Dim xmlNode_checkFormulati_capitolato = <bioResponseFamiglia>
                                                                        <id><%= icurPA %></id>
                                                                        <checkFormulati>False</checkFormulati>
                                                                    </bioResponseFamiglia>
                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)

                        Else
                            nodoFamPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                        End If
                    End If


                    nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "False"
                Next
            Next
        Next
    End Sub




    Private Sub Verifica_Analisi_EffettuaVerifica_EsclusioneImpresa(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)



        Dim xVerifica As New AgronicaCoreCapitolatoClienteDAL.Capitolato_R
        Dim dtVerifica As DataTable

        'scorre tutte le analisi
        For Each curNodoAnalisi In (
            From curElAnalisi In Xdoc.<root>.<analisi>
            Select curElAnalisi).ToList


            Dim pivaSuperUserAnalisi As String = curNodoAnalisi.@pivaSuperUser
            Dim pivaImpianto As String = curNodoAnalisi.@pivaImpianto


            Dim listaNodiCapitolati = (
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList

            Dim listaNodiCapitolatiCod = (From el1 In listaNodiCapitolati Select el1.<id>.Value).ToList
            Dim capitolatiDaVerificare As String = String.Join(",", listaNodiCapitolatiCod)
            dtVerifica = xVerifica.LeggiElencoCapitolatiEsclusiDataPiva(pivaSuperUserAnalisi, pivaImpianto, capitolatiDaVerificare, objParametri)

            For Each nodoCapitolato In listaNodiCapitolati

                Dim dr() As DataRow
                dr = dtVerifica.Select("Capitolato_COD = " & nodoCapitolato.<id>.Value)


                Dim flagAziendaEsclusaDaCapitolato = "False"
                If dr.Count > 0 Then
                    flagAziendaEsclusaDaCapitolato = "True"
                    nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "False"
                End If

                Dim nodoAziendaEsclusa = <aziendaEsclusaDaCapitolato><%= flagAziendaEsclusaDaCapitolato %></aziendaEsclusaDaCapitolato>
                nodoCapitolato.Add(nodoAziendaEsclusa)


            Next

        Next


    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_Formulati(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)
        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_ElencoPA_SenzaFormulatoValido_FAST(UniqueID, objParametri)

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList

                Dim dr() As DataRow
                dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                "Capitolato_COD = " & nodoCapitolato.<id>.Value)
                'scorre i principi attivi
                For Each curPA In dr

                    Dim icurPA As Integer = curPA("PA_o_Fam_COD")

                    If curPA("isFamiglia") = 0 Then


                        Dim nodoPAEsiste = ( _
                            From nCur In nodoCapitolato.<capitolatoResponse>.<principioAttivoNonConforme> _
                            Where nCur.<id>.Value = icurPA _
                            Select nCur).FirstOrDefault

                        If nodoPAEsiste Is Nothing Then
                            Dim xmlNode_checkFormulati_capitolato = <principioAttivoNonConforme>
                                                                        <id><%= icurPA %></id>
                                                                        <checkFormulati>False</checkFormulati>
                                                                    </principioAttivoNonConforme>
                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)

                        Else

                            nodoPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                        End If



                    Else

                        Dim nodoFamPAEsiste = ( _
                            From nCur In nodoCapitolato.<capitolatoResponse>.<famigliaPrincipioAttivoNonConforme> _
                            Where nCur.<id>.Value = icurPA _
                            Select nCur).FirstOrDefault

                        If nodoFamPAEsiste Is Nothing Then
                            Dim xmlNode_checkFormulati_capitolato = <famigliaPrincipioAttivoNonConforme>
                                                                        <id><%= icurPA %></id>
                                                                        <checkFormulati>False</checkFormulati>
                                                                    </famigliaPrincipioAttivoNonConforme>
                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)

                        Else
                            nodoFamPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                        End If
                    End If




                    nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "False"



                Next

            Next
        Next

        Verifica_Analisi_EffettuaVerifica_Formulati_DP(DT, UniqueID, Xdoc, objParametri)



    End Sub


    Private Sub Verifica_Analisi_EffettuaVerifica_Formulati_DP(ByVal DT As DataTable, ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)


        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                 From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                    Select el).ToList


                For Each nodoDPIImpianto In ( _
                    From el In nodoCapitolato.<dpiCapitolatoImpianto>
                    Select el).ToList

                    Dim dr() As DataRow
                    dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                    "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                    "Capitolato_COD = " & nodoCapitolato.<id>.Value)
                    Dim conforme As Boolean

                    'scorre i principi attivi
                    For Each curPA In dr

                        conforme = True

                        If Not (nodoDPIImpianto.<warning>.ToList.Count > 0 Or _
                        nodoDPIImpianto.<noCheckDP>.ToList.Count > 0) Then



                            Dim icurPA As Integer = curPA("PA_o_Fam_COD")

                            If curPA("isFamiglia") = 0 Then


                                Dim nodoPAEsiste = ( _
                                    From nCur In nodoDPIImpianto.<dpiResponse>.<principiAttiviNonConformi>.<principioAttivoNonConforme> _
                                    Where nCur.<id>.Value = icurPA _
                                    Select nCur).FirstOrDefault

                                If nodoPAEsiste Is Nothing Then
                                    Dim xmlNode_checkFormulati_capitolato = <principioAttivoNonConforme>
                                                                                <id><%= icurPA %></id>
                                                                                <checkFormulati>False</checkFormulati>
                                                                            </principioAttivoNonConforme>
                                    nodoDPIImpianto.<dpiResponse>.<principiAttiviNonConformi>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)

                                Else

                                    nodoPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                                End If


                            Else

                                Dim nodoFamPAEsiste = ( _
                                    From nCur In nodoDPIImpianto.<dpiResponse>.<FamiglieDiPrincipiAttiviNonConformi>.<FamigliaDiPrincipioAttivoNonConforme> _
                                    Where nCur.<id>.Value = icurPA _
                                    Select nCur).FirstOrDefault

                                If nodoFamPAEsiste Is Nothing Then
                                    Dim xmlNode_checkFormulati_capitolato = <FamigliaDiPrincipioAttivoNonConforme>
                                                                                <id><%= icurPA %></id>
                                                                                <checkFormulati>False</checkFormulati>
                                                                            </FamigliaDiPrincipioAttivoNonConforme>
                                    nodoDPIImpianto.<dpiResponse>.<FamiglieDiPrincipiAttiviNonConformi>.FirstOrDefault.Add(xmlNode_checkFormulati_capitolato)

                                Else
                                    nodoFamPAEsiste.<checkFormulati>.FirstOrDefault.Value = "False"
                                End If
                            End If


                            conforme = False



                            'imposto solo a false se necessario                            
                            If nodoDPIImpianto.<dpiResponse>.<esitoGlobale>.Value.ToLower <> conforme.ToString.ToLower And Not conforme Then
                                nodoDPIImpianto.<dpiResponse>.<esitoGlobale>.Value = IIf(conforme = False, "False", "True")
                            End If

                        End If
                    Next

                Next
            Next
        Next





    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_PercentualeSommaPa(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)

        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_PercentualeSommaPa_FAST(UniqueID, objParametri)

        Dim esito As Boolean

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente> _
                Select el).ToList

                Dim dr() As DataRow
                dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                "Capitolato_COD = " & nodoCapitolato.<id>.Value)

                Dim EsitoGlobale As Boolean = True

                For Each curPA In dr


                    If CBool(curPA("PercSuperata")) Then
                        EsitoGlobale = False
                    End If


                    Dim xmlNode_DiffRilevata_PercMax = <esitoPercentualeMax>
                                                           <sumQtaRilevata><%= curPA("sumQtaRilevata").ToString %></sumQtaRilevata>
                                                           <percSuperata><%= CBool(curPA("PercSuperata")).ToString %></percSuperata>
                                                       </esitoPercentualeMax>
                    nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_PercMax)

                    If nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "" And EsitoGlobale = False Then
                        nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "False"
                    End If


                Next

            Next


            'quelle orfane le imposto vuote ...
            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente> _
                Where el.<capitolatoResponse>.<esitoPercentualeMax>.Value Is Nothing _
                Select el).ToList



                Dim xmlNode_DiffRilevata_PercMax = <esitoPercentualeMax>
                                                       <sumQtaRilevata>-1</sumQtaRilevata>
                                                       <percSuperata>False</percSuperata>
                                                   </esitoPercentualeMax>
                nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_PercMax)


            Next

        Next




    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_NumeroPA(ByVal UniqueID As String, ByRef XDoc As XDocument, ByRef objParametri As AgronicaCoreParametri)

        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_NumeroPrincipiAttiviAmmessi(UniqueID, objParametri)

        Dim esito As Boolean

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList

                Dim dr() As DataRow
                dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                "Capitolato_COD = " & nodoCapitolato.<id>.Value)


                esito = (dr.Count > 0)

                Dim xmlNode_DiffRilevata_numeroPAcapitolato = <esitoNumeroPrincipiAttivi><%= IIf(esito = True, "True", "False") %></esitoNumeroPrincipiAttivi>
                nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_numeroPAcapitolato)

                If esito Then
                    nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "False"
                End If

            Next

        Next




    End Sub


    Private Sub Verifica_Analisi_EffettuaVerifica_PercentualiResidui(ByVal UniqueID As String, ByRef XDoc As XDocument, ByRef objParametri As AgronicaCoreParametri)

        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_FAST(UniqueID, objParametri)


        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each nodoCapitolato In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                Select el).ToList

                Dim dr() As DataRow
                dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                "Capitolato_COD = " & nodoCapitolato.<id>.Value)

                Dim EsitoGlobale As Boolean = True

                'scorre i principi attivi
                For Each curPA In dr
                    If curPA("percQtaRilevata") > 0 Then

                        If CBool(curPA("PercSuperata")) Then
                            EsitoGlobale = False
                        End If

                        If curPA("isFamiglia") = 0 Then
                            Dim xmlNode_DiffRilevata_RMA_capitolato = <principioAttivoNonConforme>
                                                                          <id><%= curPA("PA_COD") %></id>
                                                                          <percentuale><%= Math.Round(CType(curPA("percQtaRilevata"), Decimal), 2).ToString %></percentuale>
                                                                          <checkFormulati>True</checkFormulati>
                                                                          <percSuperata><%= CBool(curPA("PercSuperata")).ToString %></percSuperata>
                                                                          <rma><%= Math.Round(CType(curPA("RMA"), Decimal), 2).ToString %></rma>
                                                                          <Regolamento><%= curPA("Regolamento") %></Regolamento>
                                                                      </principioAttivoNonConforme>

                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_RMA_capitolato)
                        Else
                            Dim xmlNode_DiffRilevata_RMA_capitolato = <famigliaPrincipioAttivoNonConforme>
                                                                          <id><%= curPA("PA_COD") %></id>
                                                                          <percentuale><%= Math.Round(CType(curPA("percQtaRilevata"), Decimal), 2).ToString %></percentuale>
                                                                          <checkFormulati>True</checkFormulati>
                                                                          <percSuperata><%= CBool(curPA("PercSuperata")).ToString %></percSuperata>
                                                                          <rma><%= Math.Round(CType(curPA("RMA"), Decimal), 2).ToString %></rma>
                                                                          <Regolamento><%= curPA("Regolamento") %></Regolamento>
                                                                      </famigliaPrincipioAttivoNonConforme>

                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_RMA_capitolato)
                        End If


                    Else
                        EsitoGlobale = False
                        If curPA("isFamiglia") = 0 Then
                            Dim rma As String = Math.Round(CType(curPA("RMA"), Decimal), 2).ToString
                            Dim xmlNode_DiffRilevata_RMA_capitolato = <principioAttivoNonConforme>
                                                                          <id><%= curPA("PA_COD") %></id>
                                                                          <checkFormulati>True</checkFormulati>
                                                                          <percSuperata>True</percSuperata>
                                                                          <rma><%= rma %></rma>
                                                                          <Regolamento><%= curPA("Regolamento") %></Regolamento>
                                                                      </principioAttivoNonConforme>

                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_RMA_capitolato)

                        Else

                            Dim xmlNode_DiffRilevata_RMA_capitolato = <famigliaPrincipioAttivoNonConforme>
                                                                          <id><%= curPA("PA_COD") %></id>
                                                                          <checkFormulati>True</checkFormulati>
                                                                          <percSuperata>True</percSuperata>
                                                                          <rma><%= Math.Round(CType(curPA("RMA"), Decimal), 2).ToString %></rma>
                                                                          <Regolamento><%= curPA("Regolamento") %></Regolamento>
                                                                      </famigliaPrincipioAttivoNonConforme>

                            nodoCapitolato.<capitolatoResponse>.FirstOrDefault.Add(xmlNode_DiffRilevata_RMA_capitolato)

                        End If



                    End If

                    'vani, 14/06
                    If nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = "" And EsitoGlobale = False Then
                        nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.FirstOrDefault.Value = EsitoGlobale.ToString
                    End If



                Next

            Next
        Next


    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_VarietaAmmesse(ByVal UniqueID As String, ByRef XDoc As XDocument, ByRef objParametri As AgronicaCoreParametri)

        Dim AppCultivar As New Analisi_TestataXCapitolatoCliente_R
        Dim DT As DataTable = _
            AppCultivar.VerificaCapitolato_ElencoVarietaAmmesse(UniqueID, objParametri)

        Dim TrovataCultivarNONAmmessa As Boolean

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            For Each Cul_Cod_Corrente In curNodoAnalisi.<datiInput>.<varieta>.Value.Split(",")

                Dim listaNodiCapitolati = ( _
                    From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>
                    Select el).ToList
                Dim contaNodoCorrente As Integer = 0

                For Each nodoCapitolato In listaNodiCapitolati


                    'il datatable contiene le cultivar ammesse
                    Dim dr() As DataRow
                    dr = DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                    "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                    "Capitolato_COD = " & nodoCapitolato.<id>.Value & " AND " & _
                                                    "Cul_COD = " & Cul_Cod_Corrente)

                    'se non trovo il capitolato vuol dire che la cultivar non è ammessa (non presente in elenco, lenght = 0 )!
                    TrovataCultivarNONAmmessa = (dr.Length = 0)

                    Dim xmlnodeDPI_EsitoCultivarNonAmmesse = <cultivarNonAmmesse><%= IIf(TrovataCultivarNONAmmessa = True, "True", "False") %></cultivarNonAmmesse>
                    nodoCapitolato.Add(xmlnodeDPI_EsitoCultivarNonAmmesse)


                    If TrovataCultivarNONAmmessa Then

                        nodoCapitolato.<capitolatoResponse>.<esitoGlobale>.Value = "False"
                        nodoCapitolato.<cultivarNonAmmesse>.Value = "True"


                        If nodoCapitolato.<elencoCultivarNonAmmesse>.Count = 0 Then
                            Dim nodoElencoCultivarNonAmmesse = <elencoCultivarNonAmmesse></elencoCultivarNonAmmesse>
                            nodoCapitolato.Add(nodoElencoCultivarNonAmmesse)
                        End If

                        Dim sep As String = ","
                        If contaNodoCorrente = listaNodiCapitolati.Count = -1 Then
                            sep = ""
                        End If
                        nodoCapitolato.<elencoCultivarNonAmmesse>.Value &= Cul_Cod_Corrente.ToString & sep
                    Else
                        'aggiungo il nodo vuoto nel caso di false
                        Dim nodoElencoCultivarNonAmmesse = <elencoCultivarNonAmmesse></elencoCultivarNonAmmesse>
                        nodoCapitolato.Add(nodoElencoCultivarNonAmmesse)
                    End If


                Next

            Next

        Next


    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_PA_Ammesso_DeddottoDa_RMA(ByVal UniqueID As String, ByRef Xdoc As XDocument, ByRef objParametri As AgronicaCoreParametri)


        Dim AppDPI As New Analisi_TestataXDPI_R
        Dim DT As DataTable = _
            AppDPI.VerificaPA_AmmessoDedottoDA_RMA(UniqueID, objParametri)

        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In Xdoc.<root>.<analisi> _
            Select curElAnalisi).ToList

            'nuova versione
            For Each nodoDPIImpianto In ( _
                From el In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>.<dpiCapitolatoImpianto>
                Select el).ToList

                If Not (nodoDPIImpianto.<warning>.ToList.Count > 0 Or _
                    nodoDPIImpianto.<noCheckDP>.ToList.Count > 0) Then

                    GetConformePADPIDedotto(DT, curNodoAnalisi, nodoDPIImpianto)
                End If


            Next

            'vecchia versione
            'For Each nodoDPI In ( _
            '    From el In curNodoAnalisi.<verifiche>.<dpi>
            '    Select el).ToList

            '    GetConformePADPIDedotto(DT, curNodoAnalisi, nodoDPI)
            'Next

        Next


    End Sub

    Private Sub GetConformePADPIDedotto(ByVal DT As DataTable, ByRef curNodoAnalisi As XElement, ByRef nodoDPI As XElement)
        Dim conforme As Boolean = True
        For Each curResult In DT.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' " & _
                                                        "AND Progressivo = " & curNodoAnalisi.@id)

            Dim curPA As Integer = curResult("PA_COD")

            If curResult("isFamiglia") = 0 Then
                Dim curPAXML = ( _
                    From cpaxml In nodoDPI.<dpiResponse>.<principiAttiviNonConformi>.<principioAttivoNonConforme>
                    Where cpaxml.<id>.Value = curPA
                    Select cpaxml).FirstOrDefault

                If IsNothing(curPAXML) Then

                    Dim xmlnodeDPI_EsitoDettagli = <principioAttivoNonConforme>
                                                       <id><%= curResult("PA_Cod") %></id>
                                                       <rma><%= curResult("RMA").ToString.Replace(".", ",") %></rma>
                                                       <checkFormulati>True</checkFormulati>
                                                   </principioAttivoNonConforme>

                    nodoDPI.<dpiResponse>.<principiAttiviNonConformi>.FirstOrDefault.Add(xmlnodeDPI_EsitoDettagli)


                Else

                    Dim xmlNodeDPI_EsitoDettagliRMA = <rma><%= curResult("RMA").ToString.Replace(".", ",") %></rma>
                    curPAXML.Add(xmlNodeDPI_EsitoDettagliRMA)
                    Dim xmlNodeDPI_checkFormulati = <checkFormulati>True</checkFormulati>
                    curPAXML.Add(xmlNodeDPI_checkFormulati)

                End If

            Else

                Dim curFAMXML = ( _
                    From cpaxml In nodoDPI.<dpiResponse>.<FamiglieDiPrincipiAttiviNonConformi>.<FamigliaDiPrincipioAttivoNonConforme>
                    Where cpaxml.<id>.Value = curPA
                    Select cpaxml).FirstOrDefault


                If IsNothing(curFAMXML) Then

                    Dim xmlnodeDPI_EsitoDettagli = <FamigliaDiPrincipioAttivoNonConforme>
                                                       <id><%= curResult("PA_Cod") %></id>
                                                       <rma><%= curResult("RMA").ToString.Replace(".", ",") %></rma>
                                                       <checkFormulati>True</checkFormulati>
                                                   </FamigliaDiPrincipioAttivoNonConforme>

                    nodoDPI.<dpiResponse>.<FamiglieDiPrincipiAttiviNonConformi>.FirstOrDefault.Add(xmlnodeDPI_EsitoDettagli)


                Else

                    Dim xmlNodeDPI_EsitoDettagliRMA = <rma><%= curResult("RMA").ToString.Replace(".", ",") %></rma>
                    curFAMXML.Add(xmlNodeDPI_EsitoDettagliRMA)
                    Dim xmlNodeDPI_checkFormulati = <checkFormulati>True</checkFormulati>
                    curFAMXML.Add(xmlNodeDPI_checkFormulati)

                End If

            End If

            conforme = False

        Next

        'imposto solo a false se necessario
        If nodoDPI.<dpiResponse>.<esitoGlobale>.Value.ToLower <> conforme.ToString.ToLower And Not conforme Then
            nodoDPI.<dpiResponse>.<esitoGlobale>.Value = IIf(conforme = False, "False", "True")
        End If

    End Sub

    Private Sub Verifica_Analisi_EffettuaVerifica_PA_DPI(ByVal UniqueID As String, ByRef XDoc As XDocument, ByRef objParametri As AgronicaCoreParametri)


        Dim AppDPI As New Analisi_TestataXDPI_R
        Dim DT_PaNonConformi As DataTable = _
            AppDPI.VerificaPA_DPI(UniqueID, objParametri)

        Dim DT_FamPaNonConformi As DataTable = _
            AppDPI.VerificaFamigliePA_DPI(UniqueID, objParametri)



        'scorre tutte le analisi
        For Each curNodoAnalisi In ( _
            From curElAnalisi In XDoc.<root>.<analisi> _
            Select curElAnalisi).ToList


            'nuova versione, con nodo DP su Capitolato
            For Each nodoDpiImpianto In ( _
                From eli In curNodoAnalisi.<verifiche>.<capitolatiCliente>.<capitolatoCliente>.<dpiCapitolatoImpianto>
                Select eli).ToList

                If Not (nodoDpiImpianto.<warning>.ToList.Count > 0 Or _
                    nodoDpiImpianto.<noCheckDP>.ToList.Count > 0) Then

                    Dim xmlnodeDPI_EsitoGlobale As System.Xml.Linq.XElement = GetXmlnodeDPI_EsitoGlobale()
                    GetConforme(DT_PaNonConformi, DT_FamPaNonConformi, curNodoAnalisi, nodoDpiImpianto, xmlnodeDPI_EsitoGlobale)
                    nodoDpiImpianto.Add(xmlnodeDPI_EsitoGlobale)

                End If


            Next

            'vecchia versione, da commentare quanto prima
            'For Each nodoDPI In ( _
            '    From el In curNodoAnalisi.<verifiche>.<dpi>
            '    Select el).ToList

            '    Dim xmlnodeDPI_EsitoGlobale As System.Xml.Linq.XElement = GetXmlnodeDPI_EsitoGlobale()

            '    GetConforme(DT_PaNonConformi, DT_FamPaNonConformi, curNodoAnalisi, nodoDPI, xmlnodeDPI_EsitoGlobale)

            '    nodoDPI.Add(xmlnodeDPI_EsitoGlobale)
            'Next

        Next


    End Sub

    Private Shared Function GetXmlnodeDPI_EsitoGlobale() As System.Xml.Linq.XElement
        Dim xmlnodeDPI_EsitoGlobale = <dpiResponse>
                                          <esitoGlobale></esitoGlobale>
                                          <principiAttiviNonConformi></principiAttiviNonConformi>
                                          <FamiglieDiPrincipiAttiviNonConformi></FamiglieDiPrincipiAttiviNonConformi>
                                      </dpiResponse>
        Return xmlnodeDPI_EsitoGlobale
    End Function

    Private Shared Function GetConforme(ByVal DT_PaNonConformi As DataTable, ByVal DT_FamPaNonConformi As DataTable, ByVal curNodoAnalisi As System.Xml.Linq.XElement, ByVal nodoDPI As XElement, ByRef xmlnodeDPI_EsitoGlobale As XElement) As Boolean

        Dim conforme As Boolean = True
        For Each curResult In DT_PaNonConformi.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                                        "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                                        "Flag_Privato_Pubblico = " & nodoDPI.<dpiFlagPrivatoPubblico>.Value & "  AND " & _
                                                        "Cod_Regolamento = " & nodoDPI.<dpiCodRegolamento>.Value)


            Dim xmlnodeDPI_EsitoDettagli = <principioAttivoNonConforme>
                                               <esitoSoloDP/>
                                               <id><%= curResult("PrincipioAttivoNonDPI") %></id>
                                               <checkFormulati>True</checkFormulati>
                                           </principioAttivoNonConforme>
            xmlnodeDPI_EsitoGlobale.<principiAttiviNonConformi>.FirstOrDefault.Add(xmlnodeDPI_EsitoDettagli)
            conforme = False
        Next

        For Each curResult In DT_FamPaNonConformi.Select("PivaSuperUser = '" & curNodoAnalisi.@pivaSuperUser & " ' AND " & _
                                        "Progressivo = " & curNodoAnalisi.@id & "  AND " & _
                                        "Flag_Privato_Pubblico = " & nodoDPI.<dpiFlagPrivatoPubblico>.Value & "  AND " & _
                                        "Cod_Regolamento = " & nodoDPI.<dpiCodRegolamento>.Value)


            Dim xmlnodeDPI_EsitoFamDettagli = <FamigliaDiPrincipioAttivoNonConforme>
                                                  <esitoSoloDP/>
                                                  <id><%= curResult("FamigliaPrincipioAttivoNonDPI") %></id>
                                                  <checkFormulati>True</checkFormulati>
                                              </FamigliaDiPrincipioAttivoNonConforme>
            xmlnodeDPI_EsitoGlobale.<FamiglieDiPrincipiAttiviNonConformi>.FirstOrDefault.Add(xmlnodeDPI_EsitoFamDettagli)
            conforme = False
        Next


        xmlnodeDPI_EsitoGlobale.<esitoGlobale>.Value = IIf(conforme = False, "False", "True")
        Return conforme
    End Function



#Region "Utility xml"
    Private Function RemoveNamespace(xdoc As XDocument) As XDocument


        For Each e As XElement In xdoc.Root.DescendantsAndSelf()
            If e.Name.[Namespace] <> XNamespace.None Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next
        Return xdoc
    End Function
#End Region

End Class
