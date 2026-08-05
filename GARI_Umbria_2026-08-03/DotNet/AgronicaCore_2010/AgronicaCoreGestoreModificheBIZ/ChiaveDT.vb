Public Class ChiaveDT

    Private mChiave As ChiaveTabella

    Private mDT_Modifiche As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable

    Dim dr As DS_AnalizzaModifiche.DT_AnalizzaModificheRow

    Public Sub New()

        mChiave = New ChiaveTabella
        InizializzaDT(mDT_Modifiche)

    End Sub

    'Azienda
    Public Sub New(ByVal piva As String, ByVal Tabella_Nome As String)

        'Dim i As Integer
        'Dim dr As DS_AnalizzaModifiche.DT_AnalizzaModificheRow
        'Dim enumer As New AgronicaCoreDataProvider.TipiEnumerativi

        mChiave = New ChiaveTabella(piva, Tabella_Nome)
        InizializzaDT(mDT_Modifiche)


    End Sub

    'ImpresexParticelle_Codici
    Public Sub New(ByVal ID As Int32, ByVal Tabella_Nome As String)

        mChiave = New ChiaveTabella(ID, Tabella_Nome)
        InizializzaDT(mDT_Modifiche)

    End Sub

    'Contatto
    Public Sub New(ByVal piva As String, ByVal Cod_Contatto As String, ByVal Tabella_Nome As String)

        mChiave = New ChiaveTabella(piva, Cod_Contatto, Tabella_Nome)
        InizializzaDT(mDT_Modifiche)

    End Sub


    'Macchine,Fabbricati,Allevamenti
    Public Sub New(ByVal Piva As String, ByVal Sa_Cod As Int32, ByVal Cod As Int32, ByVal Tabella_Nome As String)

        mChiave = New ChiaveTabella(Piva, Sa_Cod, Cod, Tabella_Nome)
        InizializzaDT(mDT_Modifiche)

    End Sub

    'Particelle
    Public Sub New(ByVal Prov As String, ByVal Com As String, ByVal Sezione As String, ByVal Foglio As Int32, ByVal Numero As Int32, ByVal Subalterno As String, ByVal Tabella_Nome As String)

        mChiave = New ChiaveTabella(Prov, Com, Sezione, Foglio, Numero, Subalterno, Tabella_Nome)
        InizializzaDT(mDT_Modifiche)

    End Sub


    Public Property Chiave() As ChiaveTabella
        Get
            Return mChiave
        End Get
        Set(ByVal Value As ChiaveTabella)
            mChiave = Value
        End Set
    End Property


    Public Property DT_Modifiche() As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable
        Get
            Return mDT_Modifiche
        End Get
        Set(ByVal Value As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable)
            mDT_Modifiche = Value
        End Set
    End Property

    Public Sub InizializzaDT(ByRef dt As DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable)

        Dim i As Int32

        dt = New DS_AnalizzaModifiche.DT_AnalizzaModificheDataTable

        For i = 0 To 31

            dr = dt.NewDT_AnalizzaModificheRow
            dr.id = i
            dr.old_value = ""
            dr.new_value = ""
            dr.stato = False

            dt.Rows.Add(dr)

        Next

    End Sub

End Class
