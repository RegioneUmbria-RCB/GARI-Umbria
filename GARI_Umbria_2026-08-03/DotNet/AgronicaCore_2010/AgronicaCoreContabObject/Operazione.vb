Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Reflection
Imports AgronicaCoreDataProvider.AgronicaCoreParametri



'Public Class Giustificazioni
'    Public Chiave As String
'    Public Nome As String
'    Public selected As Boolean
'    Public Sub New()
'        selected = False
'    End Sub

'End Class


Public Class Operazione

    Public Sub New()
        _ListaImpianti = New List(Of Impianto)
        _ListaNote = New List(Of Nota_Operazione)
        _ListaCostiAccessori = New List(Of CostiAccessori)
    End Sub

    Public Sub New(ByVal piva As String, ByVal lav_cod As Integer, ByVal Data As Date)

        _Piva = piva
        _Lav_Cod = lav_cod
        _Data_Operazione = Data

        _ListaImpianti = New List(Of Impianto)
        _ListaNote = New List(Of Nota_Operazione)
        _ListaCostiAccessori = New List(Of CostiAccessori)

        _Disciplinare_Cod = "0"


    End Sub


#Region "Campi chiave"
    Private _Piva As String
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Private _Sa_Cod As Integer
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property

    Private _ID_Agenda As Integer
    Public Property ID_Agenda() As Integer
        Get
            Return _ID_Agenda
        End Get
        Set(ByVal value As Integer)
            _ID_Agenda = value
        End Set
    End Property
#End Region


#Region "proprieta generiche"
    Private _ListaImpianti As List(Of Impianto)
    Public Property ListaImpianti() As List(Of Impianto)
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As List(Of Impianto))
            _ListaImpianti = value
        End Set
    End Property

    Private _ListaNote As List(Of Nota_Operazione)
    Public Property ListaNote() As List(Of Nota_Operazione)
        Get
            Return _ListaNote
        End Get
        Set(ByVal value As List(Of Nota_Operazione))
            _ListaNote = value
        End Set
    End Property


    Private _ListaCostiAccessori As List(Of CostiAccessori)
    Public Property ListaCostiAccessori() As List(Of CostiAccessori)
        Get
            Return _ListaCostiAccessori
        End Get
        Set(ByVal value As List(Of CostiAccessori))
            _ListaCostiAccessori = value
        End Set
    End Property





    Private _ID_Cod As Integer
    Public Property ID_Cod() As Integer
        Get
            Return _ID_Cod
        End Get
        Set(ByVal value As Integer)
            _ID_Cod = value
        End Set
    End Property

    Private _Nota As String
    Public Property Nota() As String
        Get
            Return _Nota
        End Get
        Set(ByVal value As String)
            _Nota = value
        End Set
    End Property

    Private _AcquaSiNo As Boolean
    Public Property AcquaSiNo() As Boolean
        Get
            Return _AcquaSiNo
        End Get
        Set(ByVal value As Boolean)
            _AcquaSiNo = value
        End Set
    End Property

    Private _AcquaHaTot As Integer
    Public Property AcquaHaTot() As Integer
        Get
            Return _AcquaHaTot
        End Get
        Set(ByVal value As Integer)
            _AcquaHaTot = value
        End Set
    End Property

    Private _Acqua As Decimal
    Public Property Acqua() As Decimal
        Get
            Return _Acqua
        End Get
        Set(ByVal value As Decimal)
            _Acqua = value
        End Set
    End Property

    Private _Lav_Cod As Integer
    Public Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal value As Integer)
            _Lav_Cod = value
        End Set
    End Property

    Private _Data_Operazione As String
    Public Property Data_Operazione() As String
        Get
            Return _Data_Operazione
        End Get
        Set(ByVal value As String)
            _Data_Operazione = value
        End Set
    End Property

    Private _Veg_Cod As Integer
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property

    Private _Fabbricato_Cod As String
    Public Property Fabbricato_Cod() As String
        Get
            Return _Fabbricato_Cod
        End Get
        Set(ByVal value As String)
            _Fabbricato_Cod = value
        End Set
    End Property

    Private _Disciplinare_Cod As String
    Public Property Disciplinare_Cod() As String
        Get
            Return _Disciplinare_Cod
        End Get
        Set(ByVal value As String)
            _Disciplinare_Cod = value
        End Set
    End Property

    Private _Codice_Filtro_Ricerca As Integer
    Public Property Codice_Filtro_Ricerca() As Integer
        Get
            Return _Codice_Filtro_Ricerca
        End Get
        Set(ByVal value As Integer)
            _Codice_Filtro_Ricerca = value
        End Set
    End Property


#End Region

End Class


Public Class Nota_Operazione
    Private _Id_Agenda As Integer
    Private _Nota_Cod As Integer
    Private _Descrizione As String
    Private _selected As Boolean
    Sub New()
        _Id_Agenda = 0
        _Nota_Cod = 0
        _Descrizione = ""
        _selected = False
    End Sub

    Public Property selected() As Boolean
        Get
            Return _selected
        End Get
        Set(ByVal value As Boolean)
            _selected = value
        End Set
    End Property



    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As String)
            _Descrizione = value
        End Set
    End Property


    Public Property Id_Agenda() As Integer
        Get
            Return _Id_Agenda
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda = value
        End Set
    End Property

    Public Property Nota_Cod() As Integer
        Get
            Return _Nota_Cod
        End Get
        Set(ByVal value As Integer)
            _Nota_Cod = value
        End Set
    End Property
End Class


Public Class Impianto
    Public selected As Boolean
    Public Piva As String
    Public Sa_Cod As Integer
    Public Appezza As Integer
    Public Campo_Cod As Integer
    Public ID_Reg As Integer

    Public App_Nome As String

    Public Progetto_Cod As Integer
    Public Sup_Imp As Decimal
    Public Validita_Inizio_Distinta As Date
    Public Validita_Fine_Distinta As Date
    Public Cul_Des As String
    Public Data_Raccolta As String
    Public Data_Raccolta_Prevista As String
    Public Data_Fioritura As String
    Public Data_Fioritura_Prevista As String

    Public Veg_Cod As Integer
    Public Id_Cod As Integer
    Public Cul_Cod As Integer
    Public Rag_Soc As String
    Public Veg_Des As String

    Public Validita_Inizio As Date
    Public Validita_Fine As Date

    Private _Qta As Decimal
    Private _Qta2 As Decimal
    Public Codici_Anagrafe_Des As String

    Public Sa_Nome As String
    Public Campo_Des As String
    Public RifNumerico As String
    Public CodBioApp As String
    Public Catasto As String

    Public Disciplinare As String
    Public Reg_Des As String
    Public Capitolato_Privato_Des As String
    Public Grfi_Des As String
    Public Data_Semina As String
    Public Progetto As String
    Public Copertura As String

    Public Data_Semina_Prevista As String
    Public N_Massimo As String
    Public P_Massimo As String
    Public K_Massimo As String
    Public Mg_Massimo As String
    Public SpecieAgea As String
    Public CultivarAgea As String
    Public Tra_Fila As String
    Public Su_Fila As String
    Public P_Impianto As String
    Public Foral_Des As String
    Public Port_Des As String
    Public Finanziamento As String
    Public Regolamento As String
    Public Imp_Des As String



    Property Qta() As String
        Get
            Return _Qta
        End Get
        Set(ByVal Value As String)
            _Qta = Value.Replace(".", ",")
        End Set
    End Property
    Property Qta2() As String
        Get
            Return _Qta2
        End Get
        Set(ByVal Value As String)
            _Qta2 = Value.Replace(".", ",")
        End Set
    End Property


    Public Shared Function getListaImpianti(ByVal Piva As String, ByVal Sa_Cod As Integer,
                                     ByVal Veg_Cod As Integer, ByVal Id_Cod As Integer,
                                     ByVal Data As String,
                                     ByVal Grfi_Cod As Integer, ByVal Flag_Protetto As Integer, ByVal Flag_Disciplinare As Boolean, ByVal Flag_PubblicoPrivato As Integer,
                                     ByVal leggiAncheBloccati As Boolean,
                                     ByVal FiltroAggiuntivo As String,
                                ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Flag_SoloTerreniNudi As Boolean = True) As List(Of Impianto)

        Dim nuovaLista As New List(Of Impianto)

        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read


        Dim dtImpianti As DataTable

        dtImpianti = objImpianti.Leggi_Impianti_xAgenda3(Flag_Disciplinare,
                                    Piva,
                                    Sa_Cod,
                                    0,
                                    Veg_Cod,
                                    "",
                                    Data,
                                    Grfi_Cod,
                                    Flag_Protetto,
                                    Flag_SoloTerreniNudi,
                                    Id_Cod,
                                    FiltroAggiuntivo, " Cul_Des, App_Nome, Progetto ",
                                    objParametri, leggiAncheBloccati)
        '----------------------------------
        'lettura particelle impianti
        Dim DtParticelle As DataTable
        DtParticelle = objImpianti.Leggi_ParticelleImpianti_xAgenda2(Flag_Disciplinare,
                                     Piva,
                                     Sa_Cod,
                                     Veg_Cod,
                                     "",
                                     Data,
                                     Grfi_Cod,
                                     Flag_Protetto,
                                     Id_Cod,
                                     "", " Cul_Des, App_Nome, Progetto ",
                                      objParametri, leggiAncheBloccati)

        'lettura zone vulnerabili
        Dim DtPV As DataTable
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        DtPV = objPV.Leggi(-17,
                           "", "", "", 0, 0, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "", objParametri)

        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Dim DtPVF As DataTable
        Try
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            DtPVF = objPVF.Leggi("", "", "", 0, 0, "", 0,
                                 " Fascia_Cod <>0 ",
                                 "", objParametri)
        Catch ex As Exception

        End Try




        Dim j As Integer
        For j = 0 To dtImpianti.Rows.Count - 1
            Dim nImpianto As New Impianto

            nImpianto.Veg_Cod = Veg_Cod
            nImpianto.Id_Cod = Id_Cod

            'seleziono true 
            nImpianto.selected = True
            nImpianto.Rag_Soc = dtImpianti.Rows(j).Item("Rag_Soc")
            nImpianto.Sa_Nome = dtImpianti.Rows(j).Item("Sa_Nome")
            nImpianto.Campo_Des = dtImpianti.Rows(j).Item("Campo_Des")
            nImpianto.App_Nome = dtImpianti.Rows(j).Item("App_Nome")


            nImpianto.RifNumerico = dtImpianti.Rows(j).Item("RifNumerico")
            nImpianto.CodBioApp = dtImpianti.Rows(j).Item("CodBioApp")

            nImpianto.Codici_Anagrafe_Des = dtImpianti.Rows(j).Item("Codici_Anagrafe_Des")
            nImpianto.Cul_Des = dtImpianti.Rows(j).Item("Cul_Des")
            nImpianto.Sup_Imp = dtImpianti.Rows(j).Item("Sup_Imp")
            nImpianto.Qta2 = dtImpianti.Rows(j).Item("Sup_Imp")
            nImpianto.Disciplinare = IIf(IsDBNull(dtImpianti.Rows(j).Item("Disciplinare")), "", dtImpianti.Rows(j).Item("Disciplinare"))
            nImpianto.Reg_Des = dtImpianti.Rows(j).Item("Reg_Des")
            nImpianto.Capitolato_Privato_Des = dtImpianti.Rows(j).Item("Capitolato_Privato_Des")
            nImpianto.Grfi_Des = dtImpianti.Rows(j).Item("Grfi_Des")



            nImpianto.Validita_Inizio = SistemaDate(dtImpianti.Rows(j).Item("Validita_Inizio"))
            nImpianto.Data_Semina = SistemaDate(dtImpianti.Rows(j).Item("Data_Semina"))
            nImpianto.Data_Fioritura = SistemaDate(dtImpianti.Rows(j).Item("Data_Fioritura"))
            nImpianto.Data_Fioritura_Prevista = SistemaDate(dtImpianti.Rows(j).Item("Data_Fioritura_Prevista"))
            nImpianto.Data_Fioritura_Prevista = SistemaDate(dtImpianti.Rows(j).Item("Data_Fioritura_Prevista"))
            nImpianto.Data_Raccolta_Prevista = SistemaDate(dtImpianti.Rows(j).Item("Data_Raccolta_Prevista"))
            nImpianto.Data_Semina_Prevista = SistemaDate(dtImpianti.Rows(j).Item("Data_Semina_Prevista"))
            nImpianto.Data_Raccolta = SistemaDate(dtImpianti.Rows(j).Item("Data_Raccolta"))


            nImpianto.Progetto = dtImpianti.Rows(j).Item("Progetto")
            nImpianto.Copertura = dtImpianti.Rows(j).Item("Copertura")




            nImpianto.N_Massimo = dtImpianti.Rows(j).Item("N_Massimo")
            nImpianto.P_Massimo = dtImpianti.Rows(j).Item("P_Massimo")
            nImpianto.K_Massimo = dtImpianti.Rows(j).Item("K_Massimo")
            nImpianto.Mg_Massimo = dtImpianti.Rows(j).Item("Mg_Massimo")

            nImpianto.SpecieAgea = dtImpianti.Rows(j).Item("SpecieAgea")
            nImpianto.CultivarAgea = dtImpianti.Rows(j).Item("CultivarAgea")
            nImpianto.Tra_Fila = dtImpianti.Rows(j).Item("Tra_Fila")
            nImpianto.Su_Fila = dtImpianti.Rows(j).Item("Su_Fila")

            nImpianto.P_Impianto = dtImpianti.Rows(j).Item("P_Impianto")

            nImpianto.Foral_Des = dtImpianti.Rows(j).Item("Foral_Des")
            nImpianto.Port_Des = dtImpianti.Rows(j).Item("Port_Des")
            nImpianto.Finanziamento = dtImpianti.Rows(j).Item("Finanziamento")


            nImpianto.Regolamento = dtImpianti.Rows(j).Item("Regolamento")
            nImpianto.Imp_Des = dtImpianti.Rows(j).Item("Imp_Des")






            '-----------------------------------------
            'aggiunta indicazione catasto (19/09/2012)
            Dim strCatasto As String = ""
            Dim DrParticelle() As DataRow
            Dim DrPV() As DataRow
            Dim DrPVFA() As DataRow
            Dim DrPVFB() As DataRow
            Dim p As Integer

            Dim strParticella As String
            Dim strVulnerabile As String

            If Not DtParticelle Is Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                DrParticelle = DtParticelle.Select("piva='" & dtImpianti.Rows(j).Item("piva").ToString &
                                                   "' and sa_cod=" & dtImpianti.Rows(j).Item("sa_cod").ToString &
                                                   " and appezza=" & dtImpianti.Rows(j).Item("appezza").ToString & " and id_reg=" & dtImpianti.Rows(j).Item("id_reg").ToString)
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

            nImpianto.Catasto = strCatasto



            'aggiungo 
            nuovaLista.Add(nImpianto)
        Next

        Return nuovaLista
    End Function

    Private Shared Function SistemaDate(ByVal data As String) As String
        If (data = "") Then
            Return ""
        End If

        If (data = AGRODATAINIZIO) Then
            Return ""
        End If
        If (data = AGRODATAFINE) Then
            Return ""
        End If
        Return data
    End Function


    Public Sub New()
        selected = True
    End Sub
End Class



Public Class CostiAccessori
    Private _CentroCosto As String
    Public Property CentroCosto() As String
        Get
            Return _CentroCosto
        End Get
        Set(ByVal value As String)
            _CentroCosto = value
        End Set
    End Property


    Private _CategoriaRisorsa As String
    Public Property CategoriaRisorsa() As String
        Get
            Return _CategoriaRisorsa
        End Get
        Set(ByVal value As String)
            _CategoriaRisorsa = value
        End Set
    End Property


    Private _CodiceRisorsa As String
    Public Property CodiceRisorsa() As String
        Get
            Return _CodiceRisorsa
        End Get
        Set(ByVal value As String)
            _CodiceRisorsa = value
        End Set
    End Property


    Private _Udm_Cod As Integer
    Public Property Udm_Cod() As Integer
        Get
            Return _Udm_Cod
        End Get
        Set(ByVal value As Integer)
            _Udm_Cod = value
        End Set
    End Property


    Private _Qta As Decimal
    Public Property Qta() As Decimal
        Get
            Return _Qta
        End Get
        Set(ByVal value As Decimal)
            _Qta = value
        End Set
    End Property


End Class
