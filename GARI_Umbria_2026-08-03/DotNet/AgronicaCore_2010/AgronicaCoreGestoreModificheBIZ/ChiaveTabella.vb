Public Class ChiaveTabella

    Private mPiva As String
    Private mSa_Cod As Int32
    Private mFabbricato_Cod As Int32
    Private mMac_Cod As Int32
    Private mIstat_Com As String
    Private mIstat_Prov As String
    Private mSezione As String
    Private mFoglio As Int32
    Private mNumero As Int32
    Private mSubalterno As String
    Private mTabella_Nome As String
    Private mCod_Contatto As String
    Private mPiva_SuperUser As String
    Private mProgrammazione_Entita_Cod As Int32
    Private mID As Int32


    Public Sub New()


    End Sub


    Public Sub New(ByVal piva As String, ByVal Tabella_Nome As String)

        mPiva = piva
        mTabella_Nome = Tabella_Nome

    End Sub

    Public Sub New(ByVal piva As String, ByVal Cod As String, ByVal Tabella_Nome As String)

        Dim costanti As New AgronicaCoreDataProvider.CostantiPersonalizzate

        Select Case Tabella_Nome

            Case costanti.DatiAnagrafici_Contatti

                mPiva = piva
                mCod_Contatto = Cod
                mTabella_Nome = Tabella_Nome

            Case costanti.DatiAnagrafici_PianoColturale

                mPiva_SuperUser = piva
                mProgrammazione_Entita_Cod = Cod
                mTabella_Nome = Tabella_Nome

        End Select

    End Sub

    Public Sub New(ByVal Piva As String, ByVal Sa_Cod As Int32, ByVal Cod As Int32, ByVal Tabella_Nome As String)


        Dim costanti As New AgronicaCoreDataProvider.CostantiPersonalizzate

        mPiva = Piva
        mSa_Cod = Sa_Cod

        Select Case Tabella_Nome

            Case costanti.DatiAnagrafici_Fabbricati, _
                costanti.DatiAnagrafici_Allevamenti

                Me.mFabbricato_Cod = Cod

            Case costanti.DatiAnagrafici_Macchine

                Me.mMac_Cod = Cod

        End Select

        mTabella_Nome = Tabella_Nome

    End Sub

    Public Sub New(ByVal Cod As Int32, ByVal Tabella_Nome As String)


        Dim costanti As New AgronicaCoreDataProvider.CostantiPersonalizzate

        Select Case Tabella_Nome

            Case costanti.DatiAnagrafici_Catasto

                Me.mID = Cod

        End Select

        mTabella_Nome = Tabella_Nome

    End Sub

    Public Sub New(ByVal Prov As String, ByVal Com As String, ByVal Sezione As String, ByVal Foglio As Int32, ByVal Numero As Int32, ByVal Subalterno As String, ByVal Tabella_Nome As String)

        mIstat_Prov = Prov
        mIstat_Com = Com
        mSezione = Sezione
        mFoglio = Foglio
        mNumero = Numero
        mSubalterno = Subalterno
        mTabella_Nome = Tabella_Nome

    End Sub

    Public Property Piva_SuperUser() As String
        Get
            Return mPiva_SuperUser
        End Get
        Set(ByVal Value As String)
            mPiva_SuperUser = Value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return mPiva
        End Get
        Set(ByVal Value As String)
            mPiva = Value
        End Set
    End Property


    Public Property Sa_Cod() As Int32
        Get
            Return mSa_Cod
        End Get
        Set(ByVal Value As Int32)
            mSa_Cod = Value
        End Set
    End Property

    Public Property Fabbricato_Cod() As Int32
        Get
            Return mFabbricato_Cod
        End Get
        Set(ByVal Value As Int32)
            mFabbricato_Cod = Value
        End Set
    End Property

    Public Property Mac_Cod() As Int32
        Get
            Return mMac_Cod
        End Get
        Set(ByVal Value As Int32)
            mMac_Cod = Value
        End Set
    End Property

    Public Property Istat_Prov() As String
        Get
            Return mIstat_Prov
        End Get
        Set(ByVal Value As String)
            mIstat_Prov = Value
        End Set
    End Property

    Public Property Istat_Com() As String
        Get
            Return mIstat_Com
        End Get
        Set(ByVal Value As String)
            mIstat_Com = Value
        End Set
    End Property

    Public Property Sezione() As String
        Get
            Return mSezione
        End Get
        Set(ByVal Value As String)
            mSezione = Value
        End Set
    End Property

    Public Property Foglio() As Int32
        Get
            Return mFoglio
        End Get
        Set(ByVal Value As Int32)
            mFoglio = Value
        End Set
    End Property

    Public Property Numero() As Int32
        Get
            Return mNumero
        End Get
        Set(ByVal Value As Int32)
            mNumero = Value
        End Set
    End Property

    Public Property Subalterno() As String
        Get
            Return mSubalterno
        End Get
        Set(ByVal Value As String)
            mSubalterno = Value
        End Set
    End Property

    Public Property Tabella_Nome() As String
        Get
            Return mTabella_Nome
        End Get
        Set(ByVal Value As String)
            mTabella_Nome = Value
        End Set
    End Property

    Public Property Cod_Contatto() As String
        Get
            Return mCod_Contatto
        End Get
        Set(ByVal Value As String)
            mCod_Contatto = Value
        End Set
    End Property

    Public Property Programmazione_Entita_Cod() As Int32
        Get
            Return mProgrammazione_Entita_Cod
        End Get
        Set(ByVal Value As Int32)
            mProgrammazione_Entita_Cod = Value
        End Set
    End Property

    Public Property ID() As Int32
        Get
            Return mID
        End Get
        Set(ByVal Value As Int32)
            mID = Value
        End Set
    End Property

End Class
