Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreModello

Public Class FunzioniGLOBAL

    Private _objP_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _timeout_ws_Importa As Integer = 1800000

    Public Sub New(ByVal objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, Timeout_ws_Importa As Integer)
        _objP_Server = objParametri_server

        If Timeout_ws_Importa <> 0 Then
            _timeout_ws_Importa = Timeout_ws_Importa
        End If

    End Sub

#Region "Prorietà"

    Private _indirizzi As New List(Of G2G_Recode_Indirizzi)

    Private _planning As New List(Of recode_planning)
    Public Property Planning() As List(Of recode_planning)
        Get
            Return _planning
        End Get
        Set(value As List(Of recode_planning))
            _planning = value
        End Set
    End Property

    Private _Imprese As New List(Of G2G_Recode_Imprese)
    Public Property Imprese() As List(Of G2G_Recode_Imprese)
        Get
            Return _Imprese
        End Get
        Set(value As List(Of G2G_Recode_Imprese))
            _Imprese = value
        End Set
    End Property

    Private _Materie As New List(Of G2G_Recode_MateriePrime)
    Public Property Materie() As List(Of G2G_Recode_MateriePrime)
        Get
            Return _Materie
        End Get
        Set(value As List(Of G2G_Recode_MateriePrime))
            _Materie = value
        End Set
    End Property

    Private _ParcoMacchine As New List(Of G2G_Recode_Parco_Macchine)
    Public Property ParcoMacchine() As List(Of G2G_Recode_Parco_Macchine)
        Get
            Return _ParcoMacchine
        End Get
        Set(value As List(Of G2G_Recode_Parco_Macchine))
            _ParcoMacchine = value
        End Set
    End Property

    Private _Contatti As New List(Of G2G_Recode_Contatti)
    Public Property Contatti() As List(Of G2G_Recode_Contatti)
        Get
            Return _Contatti
        End Get
        Set(value As List(Of G2G_Recode_Contatti))
            _Contatti = value
        End Set
    End Property

    Private _NoteGruppi As New List(Of G2G_Recode_NoteGruppi)
    Public Property NoteGruppi() As List(Of G2G_Recode_NoteGruppi)
        Get
            Return _NoteGruppi
        End Get
        Set(value As List(Of G2G_Recode_NoteGruppi))
            _NoteGruppi = value
        End Set
    End Property

    Private _NoteIntervento As New List(Of G2G_Recode_NoteIntervento)
    Public Property NoteIntervento() As List(Of G2G_Recode_NoteIntervento)
        Get
            Return _NoteIntervento
        End Get
        Set(value As List(Of G2G_Recode_NoteIntervento))
            _NoteIntervento = value
        End Set
    End Property

    Private _NoteUtilizzo As New List(Of G2G_Recode_NoteUtilizzo)
    Public Property NoteUtilizzo() As List(Of G2G_Recode_NoteUtilizzo)
        Get
            Return _NoteUtilizzo
        End Get
        Set(value As List(Of G2G_Recode_NoteUtilizzo))
            _NoteUtilizzo = value
        End Set
    End Property

    Private _recodeAnalisi As New List(Of Recode_Analisi)
    Public Property RecodeAnalisi() As List(Of Recode_Analisi)
        Get
            Return _recodeAnalisi
        End Get
        Set(value As List(Of Recode_Analisi))
            _recodeAnalisi = value
        End Set
    End Property

    Private _recodeAnalisiTipologia As New List(Of recode_Analisi_Tipologia)
    Public Property RecodeAnalisiTipologia() As List(Of recode_Analisi_Tipologia)
        Get
            Return _recodeAnalisiTipologia
        End Get
        Set(value As List(Of recode_Analisi_Tipologia))
            _recodeAnalisiTipologia = value
        End Set
    End Property

    Public Property Indirizzi As List(Of G2G_Recode_Indirizzi)
        Get
            Return _indirizzi
        End Get
        Set(value As List(Of G2G_Recode_Indirizzi))
            _indirizzi = value
        End Set
    End Property

    Public Sub PlanningADD(ByVal i As recode_planning)
        If Not _planning.Contains(i) Then
            _planning.Add(i)
        End If
    End Sub

    Public Sub ImpreseADD(ByVal i As G2G_Recode_Imprese)
        If Not _Imprese.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Imprese.Add(i)
        End If
    End Sub

    Public Sub MaterieADD(ByVal i As G2G_Recode_MateriePrime)
        If Not _Materie.Contains(i) Then
            i.Username_Creazione = "1"
            i.Username_Modifica = _objP_Server.UtenteCodFiscale
            i.Data_Creazione = Now()
            i.Data_Modifica = Now()
            i.Validita_Inizio = AGRODATAINIZIO
            i.Validita_Fine = AGRODATAFINE
            i.inviato = "0"
            i.datainvio = Now()
            _Materie.Add(i)
        End If
    End Sub

    Public Sub ParcoMacchineADD(ByVal i As G2G_Recode_Parco_Macchine)
        If Not _ParcoMacchine.Contains(i) Then
            _ParcoMacchine.Add(i)
        End If
    End Sub

    Public Sub ContattiADD(ByVal i As G2G_Recode_Contatti)
        If Not _Contatti.Contains(i) Then
            _Contatti.Add(i)
        End If
    End Sub

    Public Sub NoteGruppiADD(ByVal i As G2G_Recode_NoteGruppi)
        If Not _NoteGruppi.Contains(i) Then
            _NoteGruppi.Add(i)
        End If
    End Sub

    Public Sub NoteInterventoADD(ByVal i As G2G_Recode_NoteIntervento)
        If Not _NoteIntervento.Contains(i) Then
            _NoteIntervento.Add(i)
        End If
    End Sub

    Public Sub NoteUtilizzoADD(ByVal i As G2G_Recode_NoteUtilizzo)
        If Not _NoteUtilizzo.Contains(i) Then
            _NoteUtilizzo.Add(i)
        End If
    End Sub

    Public Sub AnalisiADD(ByVal i As Recode_Analisi)
        If Not _recodeAnalisi.Contains(i) Then
            _recodeAnalisi.Add(i)
        End If
    End Sub

    Public Sub AnalisiTipologiaADD(ByVal i As recode_Analisi_Tipologia)
        If Not _recodeAnalisiTipologia.Contains(i) Then
            _recodeAnalisiTipologia.Add(i)
        End If
    End Sub

    'Private _SpecieVegetaliDefault As New List(Of recode_SpecieVegetaliDefault)
    'Public Property SpecieVegetaliDefault() As List(Of recode_SpecieVegetaliDefault)
    '    Get
    '        Return _SpecieVegetaliDefault
    '    End Get
    '    Set(value As List(Of recode_SpecieVegetaliDefault))
    '        _SpecieVegetaliDefault = value
    '    End Set
    'End Property

    Public Sub Recode_Salvataggio(ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByRef ErrFLAG As Integer)

        Dim NomeRoutine As String = "FunzioniGLOBAL.Recode_Salvataggio"



        Try
            For Each elemNew In (From i In _Imprese Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Imprese.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Materie Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_MateriePrime.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _NoteGruppi Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_NoteGruppi.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _NoteIntervento Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_NoteIntervento.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _NoteUtilizzo Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_NoteUtilizzo.Add(elemNew)
            Next
            efG2G.SaveChanges()

            For Each elemNew In (From i In _Contatti Where i.Username_Creazione = "1")
                elemNew.Username_Creazione = _objP_Server.UtenteCodFiscale
                efG2G.G2G_Recode_Contatti.Add(elemNew)
            Next
            efG2G.SaveChanges()

        Catch ex As Exception
            Log_G2G.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            Log_Errori.Append(" [" & NomeRoutine & "] : " & ex.Message.ToString & vbCrLf)
            ErrFLAG = 1
        End Try

        ErrFLAG = 0

    End Sub

    Public Function Recode_Caricamento(ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities) As Boolean


        _Imprese = (
            From ii In efG2G.G2G_Recode_Imprese
            ).ToList

        _NoteGruppi = (
            From ii In efG2G.G2G_Recode_NoteGruppi
            ).ToList

        _NoteIntervento = (
            From ii In efG2G.G2G_Recode_NoteIntervento
            ).ToList

        _NoteUtilizzo = (
            From ii In efG2G.G2G_Recode_NoteUtilizzo
            ).ToList

        _Materie = (
            From ii In efG2G.G2G_Recode_MateriePrime
            ).ToList

        _Contatti = (
            From ii In efG2G.G2G_Recode_Contatti
            ).ToList

        _ParcoMacchine = (
            From ii In efG2G.G2G_Recode_Parco_Macchine
            ).ToList

        _indirizzi = (
            From ii In efG2G.G2G_Recode_Indirizzi
            ).ToList()

        Return True


    End Function

#End Region

#Region "Note"

    Public Sub Elabora_Note_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder _
                                )

        Const nomeFunzione As String = "Elabora_Note_Salva"


        Try

            Dim objNoteGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R
            Dim objNoteGruppi_W As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_W

            Dim DT_NoteGruppi As DataTable = _
                objNoteGruppi_R.Leggi(0, _
                                       enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        "", _
                                        "", _
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                                        TipoG2G:=1 _
                )


            Dim NotaGruppo_cod_ORIGINE As Integer
            Dim NotaGruppo_cod_DESTINAZIONE As Integer

            If DT_NoteGruppi IsNot Nothing Then

                For Each rowNoteGruppi In DT_NoteGruppi.Rows

                    NotaGruppo_cod_DESTINAZIONE = objNoteGruppi_R.NuovoId_NoteInterventoGruppi( _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                    )

                    NotaGruppo_cod_ORIGINE = rowNoteGruppi("NotaGruppo_Cod")

                    NoteGruppiADD( _
                        New G2G_Recode_NoteGruppi With { _
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, _
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, _
                            .From_NotaGruppo_Cod = NotaGruppo_cod_ORIGINE, _
                            .To_NotaGruppo_Cod = NotaGruppo_cod_DESTINAZIONE _
                        })

                    objNoteGruppi_W.Scrivi(NotaGruppo_cod_DESTINAZIONE, _
                                            CType(rowNoteGruppi("NotaGruppo_Des"), String), _
                                            CType(rowNoteGruppi("Validita_Inizio"), Date), _
                                            CType(rowNoteGruppi("Validita_Fine"), Date), _
                                            False, _
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                    '  Vanni, 20/05/2014 11:00:47: 
                    'objNoteGruppi_W.Note_Intervento_Grupp_MarcaComeInviato( _
                    '   NotaGruppo_cod_ORIGINE, _
                    '    Now, _
                    '    "", _
                    '    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    ')


                Next 'x ogni gruppo

            End If

            '#######################################################

            Dim objNoteInt_R As New AgronicaCoreContabDAL.Note_Intervento_R
            Dim objNoteInt_W As New AgronicaCoreContabDAL.Note_Intervento_W

            Dim DT_NoteInt As DataTable = _
                objNoteInt_R.Leggi(0, _
                                   0, _
                                       enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        "", _
                                        "", _
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                                        TipoG2G:=1 _
                                    )


            Dim Nota_cod_ORIGINE As Integer
            Dim Nota_cod_DESTINAZIONE As Integer

            If DT_NoteInt IsNot Nothing Then

                For Each rowNote In DT_NoteInt.Rows

                    Nota_cod_DESTINAZIONE = objNoteInt_R.NuovoId_NoteIntervento( _
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                    )

                    Nota_cod_ORIGINE = rowNote("Nota_Cod")

                    NoteInterventoADD( _
                        New G2G_Recode_NoteIntervento With { _
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, _
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, _
                            .From_Nota_Cod = Nota_cod_ORIGINE, _
                            .To_Nota_Cod = Nota_cod_DESTINAZIONE _
                        })

                    objNoteInt_W.Scrivi(Nota_cod_DESTINAZIONE, _
                                            CType(rowNote("Nota_Des"), String), _
                                            recode_return_NotaGruppoCod(objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, CType(rowNote("NotaGruppo_Cod"), Integer)), _
                                            CType(rowNote("Validita_Inizio"), Date), _
                                            CType(rowNote("Validita_Fine"), Date), _
                                            False, _
                                            CType(rowNote("Note_Valore_Numerico"), Decimal), _
                                            CType(rowNote("Note_Valore_Stringa"), Decimal), _
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    '  Vanni, 20/05/2014 11:04:16: 
                    'objNoteInt_W.Note_Intervento_MarcaComeInviato( _
                    '    Nota_cod_ORIGINE, _
                    '    Now, _
                    '    "", _
                    '    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    ')


                Next 'x ogni nota

            End If

            '#######################################################

            Dim objNoteUtil_R As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R
            Dim objNoteUtil_W As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_W

            Dim DT_NoteUtil As DataTable = _
                objNoteUtil_R.Leggi(0, _
                                    objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                                    TipoG2G:=1 _
                                    )

            Dim NotaUtilizzo_Cod_ORIGINE As Integer
            Dim NotaUtilizzo_Cod_DESTINAZIONE As Integer

            If DT_NoteUtil IsNot Nothing Then

                For Each rowNoteUtil In DT_NoteUtil.Rows

                    NotaUtilizzo_Cod_ORIGINE = rowNoteUtil("NotaUtilizzo_Cod")

                    If NotaUtilizzo_Cod_ORIGINE < 0 Then
                        'i negativi sono quelli di base e devono rimanere con gli stessi codici
                        NotaUtilizzo_Cod_DESTINAZIONE = NotaUtilizzo_Cod_ORIGINE
                    Else
                        'creati dall'utente
                        NotaUtilizzo_Cod_DESTINAZIONE = objNoteUtil_R.NuovoId_NoteInterventoUtilizzo( _
                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                    )
                    End If

                    NoteUtilizzoADD( _
                        New G2G_Recode_NoteUtilizzo With { _
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, _
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, _
                            .From_NotaUtilizzo_Cod = NotaUtilizzo_Cod_ORIGINE, _
                            .To_NotaUtilizzo_Cod = NotaUtilizzo_Cod_DESTINAZIONE _
                        })

                    objNoteUtil_W.Scrivi(NotaUtilizzo_Cod_DESTINAZIONE, _
                                            CType(rowNoteUtil("NotaUtilizzo_Des"), String), _
                                            CType(rowNoteUtil("Validita_Inizio"), Date), _
                                            CType(rowNoteUtil("Validita_Fine"), Date), _
                                            False, _
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    '  Vanni, 20/05/2014 11:07:08: 
                    'objNoteUtil_W.Note_Intervento_Utilizzo_MarcaComeInviato( _
                    '    NotaUtilizzo_Cod_ORIGINE, _
                    '    Now, _
                    '    "", _
                    '    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    ')



                Next 'x ogni utilizzo

            End If

            '#######################################################

            Dim objNoteUtilGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_UtilizzoXGruppi_R
            Dim objNoteUtilGruppi_W As New AgronicaCoreContabDAL.Note_Intervento_UtilizzoXGruppi_W

            Dim DT_NoteUtilGruppi As DataTable = _
                objNoteUtilGruppi_R.Leggi(0, _
                                          0, _
                                          AGRODATAINIZIO, _
                                          AGRODATAFINE, _
                                           objOpzioni.objParametri_Server_GIAS_ORIGINE _
                                            )

            'Dim NotaUtilizzo_Cod_ORIGINE As Integer
            'Dim NotaUtilizzo_Cod_DESTINAZIONE As Integer
            'Dim NotaGruppo_cod_ORIGINE As Integer
            'Dim NotaGruppo_cod_DESTINAZIONE As Integer

            If DT_NoteUtilGruppi IsNot Nothing Then

                For Each rowNoteUtilGruppi In DT_NoteUtilGruppi.Rows

                    NotaUtilizzo_Cod_ORIGINE = rowNoteUtilGruppi("NotaUtilizzo_Cod")
                    NotaGruppo_cod_ORIGINE = rowNoteUtilGruppi("NotaGruppo_Cod")

                    NotaUtilizzo_Cod_DESTINAZIONE = recode_return_NotaUtilizzoCod(objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, CType(rowNoteUtilGruppi("NotaUtilizzo_Cod"), Integer))
                    NotaGruppo_cod_DESTINAZIONE = recode_return_NotaGruppoCod(objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, CType(rowNoteUtilGruppi("NotaGruppo_Cod"), Integer))

                    objNoteUtilGruppi_W.Scrivi(NotaGruppo_cod_DESTINAZIONE, _
                                               NotaUtilizzo_Cod_DESTINAZIONE, _
                                                CType(rowNoteUtilGruppi("Validita_Inizio"), Date), _
                                                CType(rowNoteUtilGruppi("Validita_Fine"), Date), _
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    '  Vanni, 20/05/2014 11:12:00: 
                    'objNoteUtilGruppi_W.Note_Intervento_UtilizzoxGruppi_MarcaComeInviato( _
                    '    NotaUtilizzo_Cod_ORIGINE, _
                    '    NotaGruppo_cod_ORIGINE, _
                    '    Now, _
                    '    "", _
                    '    objOpzioni.objParametri_Server_GIAS_ORIGINE _
                    ')

                Next 'x ogni utilizzoxgruppo

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub


    Public Function recode_return_NotaGruppoCod(ByVal oldPivaSuperUser As String, _
                                                ByVal oldNotaGruppoCod As Integer) As Integer
        Return ( _
            From ng In _NoteGruppi _
            Where ng.From_PivaSuperUser = oldPivaSuperUser _
            AndAlso ng.From_NotaGruppo_Cod = oldNotaGruppoCod _
            Select ng.To_NotaGruppo_Cod _
            ).FirstOrDefault

    End Function

    Public Function recode_return_NotaUtilizzoCod(ByVal oldPivaSuperUser As String, _
                                                    ByVal oldNotaUtilizzoCod As Integer) As Integer
        Return ( _
            From nu In _NoteUtilizzo _
            Where nu.From_PivaSuperUser = oldPivaSuperUser _
            AndAlso nu.From_NotaUtilizzo_Cod = oldNotaUtilizzoCod _
            Select nu.To_NotaUtilizzo_Cod _
            ).FirstOrDefault

    End Function

    Public Function recode_return_NotaCod(ByVal oldPivaSuperUser As String, _
                                            ByVal oldNotaCod As Integer) As Integer
        Return ( _
            From n In _NoteIntervento _
            Where n.From_PivaSuperUser = oldPivaSuperUser _
            AndAlso n.From_Nota_Cod = oldNotaCod _
            Select n.To_Nota_Cod _
            ).FirstOrDefault

    End Function


#End Region



#Region "Recodes"

    Public Sub Elabora_XML_Recodes_Salva(
        ByVal objOpzioni As clsOpzioni,
        ByVal DataRiferimento As Date,
        ByRef Log_Import As StringBuilder,
        ByRef Log_Errori As StringBuilder,
        ByRef Log_Riepilogo As StringBuilder
    )

        Const NomeFunzione As String = "Elabora_XML_Recodes_Salva"

        Try
            Dim leggiRequest As New AgronicaCoreG2GLocalDal.G2G_Recodes_R

            Dim oG2G_RecodeRequest As G2G_Recodes_Request = leggiRequest.LeggiPerRequestGias2Gias(DataRiferimento, objOpzioni.SuperUser_CodFiscale_ORIGINE, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlRequest As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Recodes_Request)(oG2G_RecodeRequest, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, "", "", objOpzioni)

            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlRequest, "//utente", "G2G_Recodes")

            Dim sStringaDaSalvare As String = mainDoc

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)
            Dim outputRecode As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'riporto della risposta
            Dim xDocResponse As XDocument = XDocument.Parse(outputRecode)
            Dim nsAgronicaCoreModello As XNamespace = "http://schemas.datacontract.org/2004/07/AgronicaCoreModello"
            Dim elemResponse As XElement = xDocResponse.Element("Risposta").Element("Risposta_DatiRecode").Element(nsAgronicaCoreModello + "G2G_Recode")
            Dim strResponse As String = elemResponse.ToString()
            Dim oResponse As G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of G2G_Recode)(strResponse, "")

            If String.IsNullOrEmpty(oResponse.MessaggioErrore) Then
                Dim Messaggio As String = G2GUtility.Scrivi_G2G_Recode(oResponse, objOpzioni.objParametri_Server_GIAS_ORIGINE, True)
                If String.IsNullOrEmpty(Messaggio) Then
                    G2GUtility.Log(Log_Import, "Allineamento recodes effettuato")
                Else
                    G2GUtility.Log(Log_Import, "Allineamento recodes ERRORE: " & Messaggio)
                End If
            Else
                Throw New Exception(oResponse.MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Recodes_Salva_Reverse(
        ByVal objOpzioni As clsOpzioni,
        ByVal DataRiferimento As Date,
        ByRef Log_Import As StringBuilder,
        ByRef Log_Errori As StringBuilder,
        ByRef Log_Riepilogo As StringBuilder
    )

        Const NomeFunzione As String = "Elabora_XML_Recodes_Salva_Reverse"

        Try
            Dim leggiRequest As New AgronicaCoreG2GLocalDal.G2G_Recodes_R

            Dim oG2G_RecodeRequest As G2G_Recodes_Request = leggiRequest.LeggiPerRequestGias2Gias(DataRiferimento, objOpzioni.SuperUser_CodFiscale_ORIGINE, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlRequest As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Recodes_Request)(oG2G_RecodeRequest, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, "", "", objOpzioni)

            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlRequest, "//utente", "G2G_Recodes_Reverse")

            Dim sStringaDaSalvare As String = mainDoc

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)
            Dim outputRecode As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'riporto della risposta
            Dim xDocResponse As XDocument = XDocument.Parse(outputRecode)
            Dim nsAgronicaCoreModello As XNamespace = "http://schemas.datacontract.org/2004/07/AgronicaCoreModello"
            Dim elemResponse As XElement = xDocResponse.Element("Risposta").Element("Risposta_DatiRecode").Element(nsAgronicaCoreModello + "G2G_Recode")
            Dim strResponse As String = elemResponse.ToString()
            Dim oResponse As G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of G2G_Recode)(strResponse, "")

            If String.IsNullOrEmpty(oResponse.MessaggioErrore) Then
                Dim Messaggio As String = G2GUtility.Scrivi_G2G_Recode(oResponse, objOpzioni.objParametri_Server_GIAS_ORIGINE, True)
                If String.IsNullOrEmpty(Messaggio) Then
                    G2GUtility.Log(Log_Import, "Allineamento recodes effettuato")
                Else
                    G2GUtility.Log(Log_Import, "Allineamento recodes ERRORE: " & Messaggio)
                End If
            Else
                Throw New Exception(oResponse.MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

#End Region

#Region "ImpresaReverse"

    Public Sub Elabora_XML_Impresa_Reverse_Salva(
        ByVal objOpzioni As clsOpzioni,
        ByVal DataRiferimento As Date,
        ByRef Log_Import As StringBuilder,
        ByRef Log_Errori As StringBuilder,
        ByRef Log_Riepilogo As StringBuilder
    )

        Const NomeFunzione As String = "Elabora_XML_Impresa_Reverse_Salva"

        Try
            Dim leggiRequest As New AgronicaCoreG2GLocalDal.G2G_Recodes_R

            Dim oG2G_RecodeRequest As G2G_Recodes_Request = leggiRequest.LeggiPerRequestGias2Gias(DataRiferimento, objOpzioni.SuperUser_CodFiscale_ORIGINE, objOpzioni.SuperUser_CodFiscale_DESTINAZIONE, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlRequest As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Recodes_Request)(oG2G_RecodeRequest, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, "", "", objOpzioni)

            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlRequest, "//utente", "flagimporta_g2g_reverse")

            Dim sStringaDaSalvare As String = mainDoc

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)
            Dim outputRecode As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'riporto della risposta
            Dim xDocResponse As XDocument = XDocument.Parse(outputRecode)
            Dim nsAgronicaCoreModello As XNamespace = "http://schemas.datacontract.org/2004/07/AgronicaCoreModello"
            Dim elemResponse As XElement = xDocResponse.Element("Risposta").Element("Risposta_DatiImpresaReverse").Element(nsAgronicaCoreModello + "G2G_Recode")
            Dim strResponse As String = elemResponse.ToString()
            Dim oResponse As G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of G2G_Recode)(strResponse, "")

            If String.IsNullOrEmpty(oResponse.MessaggioErrore) Then
                Dim Messaggio As String = G2GUtility.Scrivi_G2G_Recode(oResponse, objOpzioni.objParametri_Server_GIAS_ORIGINE, True)
                If String.IsNullOrEmpty(Messaggio) Then
                    G2GUtility.Log(Log_Import, "Allineamento recodes effettuato")
                Else
                    G2GUtility.Log(Log_Import, "Allineamento recodes ERRORE: " & Messaggio)
                End If
            Else
                Throw New Exception(oResponse.MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

#End Region


#Region "Rapporti_contabili"


    Public Sub Elabora_Rapporti_Contabili_Salva( _
                                ByVal objOpzioni As clsOpzioni, _
                                ByRef Log_Import As StringBuilder, _
                                ByRef Log_Errori As StringBuilder, _
                                ByRef Log_Riepilogo As StringBuilder _
                                )

        Const nomeFunzione As String = "Elabora_Rapporti_Contabili_Salva"


        Try

            Dim objRapp_R As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim objRapp_W As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W
            Dim str_errore As String = ""

            objRapp_R.RapportiContabiliDiBase_Verifica(str_errore, _
                                                          objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

            If str_errore <> "" Then
                Throw New Exception("Salvataggio rapporti contabili di base: " & str_errore)
            End If

            Dim DT_Rapp As DataTable = _
                objRapp_R.Contatti_RapportiContabili_Leggi(AgronicaCoreDataProvider.CostantiPersonalizzate.SACOD_CONTATTO_NONDEFINITO, _
                                                               0, _
                                                               False, False, False, False, False, False, False, _
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                " (Rapporti_Contabili.Cod_Rapporto > 0) and (Rapporti_Contabili.inviato >= 0) ", _
                                                                "", _
                                                                objOpzioni.objParametri_Server_GIAS_ORIGINE _
                                                                )

            Dim Cod_Rapporto_ORIGINE As Integer
            Dim Cod_Rapporto_DESTINAZIONE As Integer
            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            If DT_Rapp IsNot Nothing Then

                For Each rowRaPP In DT_Rapp.Rows

                    Cod_Rapporto_ORIGINE = rowRaPP("Cod_Rapporto")

                    If Cod_Rapporto_ORIGINE > 0 Then

                        'RAPPORTO CONTABILE PERSONALIZZATO
                        Cod_Rapporto_DESTINAZIONE = ObjSequenze.NuovoId_Tabella("Rapporti_Contabili", _
                                                                             objOpzioni.BaseCode_DESTINAZIONE, _
                                                                             objOpzioni.TopCode_DESTINAZIONE,
                                                                             objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                                                             )

                        objRapp_W.Scrivi(CType(rowRaPP("Sa_Cod"), Integer), _
                                           Cod_Rapporto_DESTINAZIONE, _
                                          CType(rowRaPP("Rapporto_Des"), String), _
                                          CType(rowRaPP("Cliente"), Integer), _
                                          CType(rowRaPP("Fornitore"), Integer), _
                                          CType(rowRaPP("Dipendente"), Integer), _
                                          CType(rowRaPP("Terzista"), Integer), _
                                          CType(rowRaPP("Legale"), Integer), _
                                          CType(rowRaPP("Agente"), Integer), _
                                          CType(rowRaPP("Consulente"), Integer), _
                                          CType(rowRaPP("Validita_Inizio"), Date), _
                                          CType(rowRaPP("Validita_Fine"), Date), _
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    End If

                Next

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try


    End Sub



#End Region



End Class
