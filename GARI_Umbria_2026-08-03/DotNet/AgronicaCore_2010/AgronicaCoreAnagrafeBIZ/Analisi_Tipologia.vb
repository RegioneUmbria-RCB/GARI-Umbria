Imports System.Web
Imports AgronicaCoreDataProvider

Public Class Analisi_Tipologia_Laboratori
    Public Sub New()
        _Tipologia = New Analisi_Tipologia
    End Sub
#Region "private"
    Private _PivaSuperUser As String
    Private _Cod_Risum As Integer
    Private _Analisi_Tipologia_Cod As Integer
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _Tipologia As Analisi_Tipologia
#End Region
#Region "Propery"
    Public Property PivaSuperUser As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property
    Public Property Cod_Risum As Integer
        Get
            Return _Cod_Risum
        End Get
        Set(ByVal value As Integer)
            _Cod_Risum = value
        End Set
    End Property
    Public Property Analisi_Tipologia_Cod As Integer
        Get
            Return _Analisi_Tipologia_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Tipologia_Cod = value
        End Set
    End Property
    Public Property Validita_Inizio As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property
    Public Property Validita_Fine As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property
    Public Property Tipologia As Analisi_Tipologia
        Get
            Return _Tipologia
        End Get
        Set(ByVal value As Analisi_Tipologia)
            _Tipologia = value
        End Set
    End Property
#End Region
End Class

'######################################################
'######################################################
Public Class Analisi_Tipologia

    Public Sub New()
        _Dettagli = New List(Of Analisi_Tipologia_Dettagli)
    End Sub
#Region "private"
    Private _PivaSuperUser As String
    Private _Analisi_Tipologia_Cod As Integer
    Private _Analisi_Tipologia_Des As String
    Private _Analisi_Tipologia_Des_Long As String
    Private _Numero_Determinazioni As String
    Private _Analisi_Tipologia_Tipo As Integer
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _Dettagli As List(Of Analisi_Tipologia_Dettagli)
#End Region
#Region "Propery"
    Public Property PivaSuperUser As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Numero_Determinazioni As String
        Get
            Return _Numero_Determinazioni
        End Get
        Set(ByVal value As String)
            _Numero_Determinazioni = value
        End Set
    End Property


    Public Property Analisi_Tipologia_Cod As Integer
        Get
            Return _Analisi_Tipologia_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Tipologia_Cod = value
        End Set
    End Property
    Public Property Analisi_Tipologia_Des As String
        Get
            Return _Analisi_Tipologia_Des
        End Get
        Set(ByVal value As String)
            _Analisi_Tipologia_Des = value
        End Set
    End Property
    Public Property Analisi_Tipologia_Des_Long As String
        Get
            Return _Analisi_Tipologia_Des_Long
        End Get
        Set(ByVal value As String)
            _Analisi_Tipologia_Des_Long = value
        End Set
    End Property
    Public Property Analisi_Tipologia_Tipo As Integer
        Get
            Return _Analisi_Tipologia_Tipo
        End Get
        Set(ByVal value As Integer)
            _Analisi_Tipologia_Tipo = value
        End Set
    End Property
    Public Property Validita_Inizio As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property
    Public Property Validita_Fine As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property
    Public Property Dettagli As List(Of Analisi_Tipologia_Dettagli)
        Get
            Return _Dettagli
        End Get
        Set(ByVal value As List(Of Analisi_Tipologia_Dettagli))
            _Dettagli = value
        End Set
    End Property
#End Region
End Class

'######################################################
'######################################################
Public Class Analisi_Tipologia_Dettagli

#Region "private"
    Private _PivaSuperUser As String
    Private _Analisi_Tipologia_Cod As Integer
    'Positivo per la valorizzazione dei principi attivi , negativo per le famiglie
    Private _Analisi_Parametro_Cod As Integer
    Private _Udm_Cod As Integer
    Private _LDM As Decimal
    Private _Ordinamento As Integer
#End Region


#Region "Propery"
    Public Property PivaSuperUser As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property
    Public Property LDM As Decimal
        Get
            Return _LDM
        End Get
        Set(ByVal value As Decimal)
            _LDM = value
        End Set
    End Property
    Public Property Analisi_Parametro_Cod As Integer
        Get
            Return _Analisi_Parametro_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Parametro_Cod = value
        End Set
    End Property
    Public Property Analisi_Tipologia_Cod As Integer
        Get
            Return _Analisi_Tipologia_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Tipologia_Cod = value
        End Set
    End Property
    Public Property Udm_Cod As Integer
        Get
            Return _Udm_Cod
        End Get
        Set(ByVal value As Integer)
            _Udm_Cod = value
        End Set
    End Property
    Public Property Ordinamento As Integer
        Get
            Return _Ordinamento
        End Get
        Set(ByVal value As Integer)
            _Ordinamento = value
        End Set
    End Property
#End Region
End Class




'######################################################
'######################################################
'######################################################
'############ HELPER - HELPER - HELPER ################
'######################################################
'######################################################
'######################################################
'######################################################




Public Class Analisi_Tipologia_Laboratori_Helper
    Public Shared Sub CaricaLista(ByVal Cod_Risum As Integer, _
                             ByRef oggetto As List(Of Analisi_Tipologia_Laboratori), _
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        oggetto = New List(Of Analisi_Tipologia_Laboratori)

        Dim DT As DataTable
        Dim objTipologiaLaboratori As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_R
        DT = objTipologiaLaboratori.Leggi(0, Cod_Risum, "", "", objParametri)

        Dim i As Integer
        Dim appoggio As Analisi_Tipologia_Laboratori
        For i = 0 To DT.Rows.Count - 1
            appoggio = New Analisi_Tipologia_Laboratori With {
                .Analisi_Tipologia_Cod = CInt(DT.Rows(i).Item("Analisi_Tipologia_Cod")),
                .Cod_Risum = CInt(DT.Rows(i).Item("Cod_Risum")),
                .Validita_Inizio = CDate(DT.Rows(i).Item("Validita_Inizio")),
                .Validita_Fine = CDate(DT.Rows(i).Item("Validita_Fine"))
            }

                'Carico la tipologia
            Analisi_Tipologia_Helper.Carica(appoggio.Analisi_Tipologia_Cod,
                                            appoggio.Tipologia,
                                            objParametri)


            oggetto.Add(appoggio)
        Next


    End Sub


    'Public Shared Sub Carica(ByVal Cod_Risum As Integer, _
    '                        ByVal Analisi_tipologia_Cod As Integer, _
    '                        ByRef oggetto As Analisi_Tipologia_Laboratori, _
    '                        ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
    '    oggetto = New Analisi_Tipologia_Laboratori

    '    Dim DT As DataTable
    '    Dim objTipologiaLaboratori As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_R
    '    DT = objTipologiaLaboratori.Leggi(Analisi_tipologia_Cod, Cod_Risum, "", "", objParametri)

    '    Dim i As Integer
    '    If DT.Rows.Count > 0 Then

    '        oggetto.Analisi_Tipologia_Cod = DT.Rows(i).Item("Analisi_Tipologia_Cod")
    '        oggetto.Cod_Risum = DT.Rows(i).Item("Cod_Risum")
    '        oggetto.Validita_Inizio = DT.Rows(i).Item("Validita_Inizio")
    '        oggetto.Validita_Fine = DT.Rows(i).Item("Validita_Fine")

    '        'Carico la tipologia
    '        Analisi_Tipologia_Helper.Carica(oggetto.Analisi_Tipologia_Cod, _
    '                                        oggetto.Tipologia, _
    '                                        objParametri)

    '    End If


    'End Sub


    'Public Shared Function Salva(ByVal oggetto As List(Of Analisi_Tipologia_Laboratori), ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
    '    Dim Flag_Connessione, Flag_Transazione As Boolean
    '    Dim Risp As String = ""
    '    Try

    '        Utility.VerificaApriTransazione(objParametri, _
    '                                     Flag_Connessione, _
    '                                     Flag_Transazione)

    '        Dim objAnalisiTipologiaLaboratorio As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W
    '        Dim i As Integer
    '        For i = 0 To oggetto.Count - 1
    '            objAnalisiTipologiaLaboratorio.Scrivi(oggetto(i).Cod_Risum, _
    '                                                   oggetto(i).Analisi_Tipologia_Cod, _
    '                                                   oggetto(i).Validita_Inizio, _
    '                                                   oggetto(i).Validita_Fine, _
    '                                                   objParametri)

    '            'salvo anche le analisiTipologia Associate 
    '            Analisi_Tipologia_Helper.Salva(oggetto(i).Tipologia, objParametri)
    '        Next

    '        Utility.VerificaChiudiTransazione(objParametri, _
    '                                        Flag_Transazione)

    '    Catch ex As Exception
    '        Utility.VerificaAnnullaTransazione(objParametri, _
    '                                           Flag_Transazione)
    '        Risp = ex.Message
    '    Finally
    '        Utility.VerificaChiudiConnessione(objParametri, _
    '                                               Flag_Connessione)

    '    End Try
    '    Return Risp
    'End Function

    Public Shared Function Salva(ByRef oggetto As Analisi_Tipologia_Laboratori, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try

            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            Dim objAnalisiTipologiaLaboratorio As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W

            objAnalisiTipologiaLaboratorio.Scrivi(oggetto.Cod_Risum, _
                                                   oggetto.Analisi_Tipologia_Cod, _
                                                   oggetto.Validita_Inizio, _
                                                   oggetto.Validita_Fine, _
                                                   objParametri)

            'salvo anche le analisiTipologia Associate 
            Analisi_Tipologia_Helper.Salva(oggetto.Tipologia, objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function

    Public Shared Function Cancella(ByVal Cod_Risum As Integer, ByVal Analisi_Tipologia_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""

        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            'controllo che non sia utilizzata
            Dim objAnalisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
            Dim dt As DataTable = objAnalisi.Leggi(0, 0, 0, 0, " PDC_Analisi.analisi_tipologia_cod =" & Analisi_Tipologia_Cod, "", objParametri)
            If dt.Rows.Count > 0 Then
                Return "non è possibile eliminare il dato perché già associato a una o più analisi"
            End If

            'elimino tutti le tipologie
            Analisi_Tipologia_Helper.Cancella(Analisi_Tipologia_Cod, objParametri)

            'elimino questo
            Dim objLab As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W
            objLab.Cancella(Cod_Risum, Analisi_Tipologia_Cod, "", objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                               Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function


    Public Shared Function Modifica(ByRef oggetto As Analisi_Tipologia_Laboratori, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try

            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)
            'modifico  
            Dim objAnalisiTipologiaLaboratorio As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W

            objAnalisiTipologiaLaboratorio.Modifica(oggetto.Cod_Risum, _
                                                   oggetto.Analisi_Tipologia_Cod, _
                                                   oggetto.Validita_Inizio, _
                                                   oggetto.Validita_Fine, _
                                                   objParametri)

            'modifico Analisi_Tipologia

            'salvo anche le analisiTipologia Associate 
            Analisi_Tipologia_Helper.Modifica(oggetto.Tipologia, objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function


    Public Shared Function GetDT(ByRef oggetto As List(Of Analisi_Tipologia_Laboratori)) As DataTable

        Dim DT As New DataTable
        'creo la struttura del Datatable
        Dim Analisi_Tipologia_Cod As New DataColumn("Analisi_Tipologia_Cod") With {
            .DataType = System.Type.GetType("System.Int32")
        }
        DT.Columns.Add(Analisi_Tipologia_Cod)

        Dim Analisi_Tipologia_Des As New DataColumn("Analisi_Tipologia_Des") With {
            .DataType = System.Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Tipologia_Des)
        Dim dr As DataRow
        Dim i As Integer
        For i = 0 To oggetto.Count - 1
            dr = DT.NewRow
            dr.Item("Analisi_Tipologia_Cod") = oggetto(i).Analisi_Tipologia_Cod
            dr.Item("Analisi_Tipologia_Des") = oggetto(i).Tipologia.Analisi_Tipologia_Des
            DT.Rows.Add(dr)
        Next

        Return DT
    End Function



    Public Shared Function GetDT(ByVal Cod_Risum As Integer, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim DT As New DataTable

        Dim Oggetto As New List(Of Analisi_Tipologia_Laboratori)

        AgronicaCoreAnagrafeBIZ.Analisi_Tipologia_Laboratori_Helper.CaricaLista(Cod_Risum, _
                                                                                 Oggetto, _
                                                                                 objParametri)

        'creo la struttura del Datatable
        Dim Analisi_Tipologia_Cod As New DataColumn("Analisi_Tipologia_Cod") With {
            .DataType = System.Type.GetType("System.Int32")
        }
        DT.Columns.Add(Analisi_Tipologia_Cod)

        Dim Analisi_Tipologia_Des As New DataColumn("Analisi_Tipologia_Des") With {
            .DataType = System.Type.GetType("System.String")
        }
        DT.Columns.Add(Analisi_Tipologia_Des)
        Dim dr As DataRow
        Dim i As Integer
        For i = 0 To Oggetto.Count - 1
            dr = DT.NewRow
            dr.Item("Analisi_Tipologia_Cod") = Oggetto(i).Analisi_Tipologia_Cod
            dr.Item("Analisi_Tipologia_Des") = Oggetto(i).Tipologia.Analisi_Tipologia_Des
            DT.Rows.Add(dr)
        Next

        Return DT
    End Function

End Class




'######################################################
'######################################################
'######################################################
'######################################################


Public Class Analisi_Tipologia_Helper
    Public Shared Function Duplica(ByVal Cod_Risum As Integer,
                                   ByRef oggetto As Analisi_Tipologia,
                                   ByVal objParametri As AgronicaCoreParametri
                                   ) As String

        Dim Analisi_Tipologia_Cod_NEW As Integer

        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try
            Utility.VerificaApriTransazione(objParametri, Flag_Connessione, Flag_Transazione)
            'mi faccio dare un altro codice 
            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Analisi_Tipologia_Cod_NEW = objSequenze.NuovoId_Tabella("Analisi_Tipologia", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)



            Dim objAnalisiTipologiaLaboratorio As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W

            objAnalisiTipologiaLaboratorio.Scrivi(Cod_Risum, _
                                                   Analisi_Tipologia_Cod_NEW, _
                                                   oggetto.Validita_Inizio, _
                                                   oggetto.Validita_Fine, _
                                                   objParametri)



            Dim objAnalisiTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W
            objAnalisiTipologia.Scrivi(Analisi_Tipologia_Cod_NEW, _
                                       oggetto.Analisi_Tipologia_Des & "_Copy", _
                                       oggetto.Analisi_Tipologia_Des_Long, _
                                       oggetto.Analisi_Tipologia_Tipo, _
                                       oggetto.Numero_Determinazioni, _
                                       oggetto.Validita_Inizio, _
                                       oggetto.Validita_Fine, _
                                            objParametri)

            'Dettagli
            Dim objAnalisiTipologiaDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W
            Dim i As Integer
            For i = 0 To oggetto.Dettagli.Count - 1
                objAnalisiTipologiaDettagli.Scrivi(Analisi_Tipologia_Cod_NEW, _
                                                   oggetto.Dettagli(i).Analisi_Parametro_Cod, _
                                                   oggetto.Dettagli(i).Udm_Cod, _
                                                   oggetto.Dettagli(i).LDM, _
                                                   oggetto.Dettagli(i).Ordinamento, _
                                                   objParametri)
            Next


            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)
        End Try
        Return Risp

    End Function



    Public Shared Sub Carica(ByVal Analisi_Tipologia_Cod As Integer, _
                             ByRef oggetto As Analisi_Tipologia, _
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        oggetto = New Analisi_Tipologia

        Dim DT As DataTable
        Dim objTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R
        DT = objTipologia.Leggi(Analisi_Tipologia_Cod, 0, "", "", objParametri)

        If DT.Rows.Count > 0 Then
            oggetto.Analisi_Tipologia_Cod = CInt(DT.Rows(0).Item("Analisi_Tipologia_Cod"))
            oggetto.Analisi_Tipologia_Des = CStr(DT.Rows(0).Item("Analisi_Tipologia_Des"))
            oggetto.Analisi_Tipologia_Des_Long = CStr(DT.Rows(0).Item("Analisi_Tipologia_Des_Long"))
            oggetto.Analisi_Tipologia_Tipo = CInt(DT.Rows(0).Item("Analisi_Tipologia_Tipo"))
            oggetto.Numero_Determinazioni = If(IsDBNull(DT.Rows(0).Item("Numero_Determinazioni")), "", CStr(DT.Rows(0).Item("Numero_Determinazioni")))

            oggetto.Validita_Inizio = CDate(DT.Rows(0).Item("Validita_Inizio"))
            oggetto.Validita_Fine = CDate(DT.Rows(0).Item("Validita_Fine"))
            oggetto.PivaSuperUser = CStr(DT.Rows(0).Item("PivaSuperUser"))
            'carico la lista dei dettagli
            Analisi_Tipologia_Dettagli_Helper.CaricaLista(oggetto.Analisi_Tipologia_Cod, oggetto.Dettagli, objParametri)
        End If

    End Sub

    Public Shared Function Salva(ByRef oggetto As Analisi_Tipologia, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            Dim objAnalisiTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W

            objAnalisiTipologia.Scrivi(oggetto.Analisi_Tipologia_Cod, _
                                       oggetto.Analisi_Tipologia_Des, _
                                       oggetto.Analisi_Tipologia_Des_Long, _
                                       oggetto.Analisi_Tipologia_Tipo, _
                                       oggetto.Numero_Determinazioni, _
                                       oggetto.Validita_Inizio, _
                                       oggetto.Validita_Fine, _
                                            objParametri)

            'salvo anche i dettagli associati
            Analisi_Tipologia_Dettagli_Helper.Salva(oggetto.Dettagli, objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)
        End Try
        Return Risp
    End Function


    Public Shared Function Cancella(ByVal Analisi_Tipologia_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            'elimino tutti i dettagli
            Analisi_Tipologia_Dettagli_Helper.Cancella(Analisi_Tipologia_Cod, 0, objParametri)

            'elimino questo
            Dim objTipo As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W
            objTipo.Cancella(Analisi_Tipologia_Cod, "", objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                               Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function



    Public Shared Function Modifica(ByRef oggetto As Analisi_Tipologia, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)


            Dim objAnalisiTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W

            objAnalisiTipologia.Modifica(oggetto.Analisi_Tipologia_Cod, _
                                       oggetto.Analisi_Tipologia_Des, _
                                       oggetto.Analisi_Tipologia_Des_Long, _
                                       oggetto.Analisi_Tipologia_Tipo, _
                                       oggetto.Numero_Determinazioni, _
                                       oggetto.Validita_Inizio, _
                                       oggetto.Validita_Fine, _
                                            objParametri)

            'cancello i vecchi
            Analisi_Tipologia_Dettagli_Helper.Cancella(oggetto.Analisi_Tipologia_Cod, 0, objParametri)
            'salvo anche i dettagli associati
            Analisi_Tipologia_Dettagli_Helper.Salva(oggetto.Dettagli, objParametri)

            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)
        End Try
        Return Risp
    End Function

End Class


'######################################################
'######################################################
'######################################################
'######################################################


Public Class Analisi_Tipologia_Dettagli_Helper
    Public Shared Sub CaricaLista(ByVal Analisi_Tipologia_Cod As Integer, _
                             ByRef oggetto As List(Of Analisi_Tipologia_Dettagli), _
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        oggetto = New List(Of Analisi_Tipologia_Dettagli)

        Dim DT As DataTable
        Dim objTipologiaDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_R
        DT = objTipologiaDettagli.Leggi(Analisi_Tipologia_Cod, "", "", objParametri)

        Dim i As Integer
        Dim appoggio As Analisi_Tipologia_Dettagli
        For i = 0 To DT.Rows.Count - 1
            appoggio = New Analisi_Tipologia_Dettagli With {
                .PivaSuperUser = CStr(DT.Rows(i).Item("PivaSuperUser")),
                .Analisi_Tipologia_Cod = CInt(DT.Rows(i).Item("Analisi_Tipologia_Cod")),
                .Analisi_Parametro_Cod = CInt(DT.Rows(i).Item("Analisi_Parametro_Cod")),
                .Udm_Cod = CInt(DT.Rows(i).Item("Udm_Cod")),
                .LDM = CDec(DT.Rows(i).Item("LDM")),
                .Ordinamento = CInt(DT.Rows(i).Item("Ordinamento"))
            }
            oggetto.Add(appoggio)
        Next


    End Sub

    Public Shared Function Salva(ByRef oggetto As List(Of Analisi_Tipologia_Dettagli), ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try

            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)

            Dim objAnalisiTipologiaDettagli As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W
            Dim i As Integer
            For i = 0 To oggetto.Count - 1
                objAnalisiTipologiaDettagli.Scrivi(oggetto(i).Analisi_Tipologia_Cod, _
                                                   oggetto(i).Analisi_Parametro_Cod, _
                                                   oggetto(i).Udm_Cod, _
                                                   oggetto(i).LDM, _
                                                   oggetto(i).Ordinamento, _
                                                   objParametri)
            Next

            Utility.VerificaChiudiTransazione(objParametri, _
                                            Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
            Throw New Exception(Risp)

        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function


    Public Shared Function Cancella(ByVal Analisi_Tipologia_Cod As Integer, ByVal Analisi_Parametro_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Flag_Connessione, Flag_Transazione As Boolean
        Dim Risp As String = ""
        Try
            Utility.VerificaApriTransazione(objParametri, _
                                         Flag_Connessione, _
                                         Flag_Transazione)


            Dim objDett As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W
            objDett.Cancella(Analisi_Tipologia_Cod, Analisi_Parametro_Cod, "", objParametri)


            Utility.VerificaChiudiTransazione(objParametri, _
                                               Flag_Transazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, _
                                               Flag_Transazione)
            Risp = ex.Message
            Throw New Exception(Risp)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, _
                                                   Flag_Connessione)

        End Try
        Return Risp
    End Function




End Class




'######################################################
'######################################################
'######################################################
'######################################################
'######################################################
'######################################################
'######################################################
'######################################################
