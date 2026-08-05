
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class v_m_RipartoCatastoCfgSalvataggio
    Public Property GlobalSalvaSelezione As Boolean
    Public Property GlobalEliminaBlu As Boolean
    Public Property GlobalSalvaInReport As Boolean

End Class

Public Class v_m_verifichePreSalvataggio

    Public EsistonoOperazioniSuImpianti As Boolean
    Public MessaggioOperazioniSuImpianti As String

    Public EsistonoDatiGisSuElementiAnagraficiCollegati As Boolean
    Public MessaggioDatiGisSuElementiAnagraficiCollegati As String

    Public EsisteCatastoAssociatoAdAppezzamento As Boolean
    Public MessaggioCatastoAssociatoAdAppezzamento As String

    Public EsistonoImpiantiDaEntita As Boolean
    Public MessaggioEsistonoImpiantiDaEntita As String


End Class


Public Class v_m_DatiGisSuElementiAnagraficiCollegati

    Public DatiGisSuElementiAnagraficiCollegati_Lista_Entita_Cod As List(Of v_m_ObjImpiantoPiccolo)
    Public MessaggioDatiGisSuElementiAnagraficiCollegati As String

End Class

Public Class v_m_ObjImpiantoPiccolo
    Inherits AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo

    Public Entita_Cod As Integer

    Public movimentipresenti As Boolean
    Public trattamentipresenti As Boolean

End Class


Public Class InSalvaGraficaModel_Factory

    Public Function GetInSalvaGraficaModel(type As enum_Gis_LayerElementiGrafici_std) As InSalvaGraficaModel

    End Function

End Class

Public Class InSalvaGraficaModel

    ''' <summary>
    ''' Costruttore generico senza catasto
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="sa_cod"></param>
    ''' <param name="Entita_cod"></param>
    ''' <param name="hiddenPunti"></param>
    ''' <param name="programmazione_cod"></param>
    ''' <param name="programmazione_entita_cod"></param>
    ''' <param name="Tipo_entita_cod"></param>
    ''' <param name="objImpianto"></param>
    ''' <param name="ElementoGrafico_DES"></param>
    ''' <param name="layer_codAtt"></param>
    ''' <param name="id_agenda"></param>
    ''' <param name="id_Mov_Det"></param>
    ''' <param name="analisi_campione_cod"></param>
    Public Sub New(piva As String, sa_cod As String, Entita_cod As Integer, hiddenPunti As String, programmazione_cod As Integer, programmazione_entita_cod As Integer, Tipo_entita_cod As Integer, objImpianto As Appezzamento_Piccolo, ElementoGrafico_DES As String, layer_codAtt As Integer, id_agenda As Integer, id_Mov_Det As Integer, analisi_campione_cod As Integer)
        Me.Piva = piva
        SaCod = sa_cod
        EntitaCod = Entita_cod
        Me.HiddenPunti = hiddenPunti
        ProgrammazioneCod = programmazione_cod
        ProgrammazioneEntitaCod = programmazione_entita_cod
        TipoEntitaCod = Tipo_entita_cod
        Me.ObjImpianto = objImpianto
        ElementoGraficoDes = ElementoGrafico_DES
        LayerCodAtt = layer_codAtt
        IdAgenda = id_agenda
        IdMovDet = id_Mov_Det
        AnalisiCampioneCod = analisi_campione_cod
        CatastoProv = "0"
        CatastoCom = "0"
        CatastoSezione = "-1"
        CatastoFoglio = -1
        CatastoNumero = -1
        CatastoSubalterno = "-1"
    End Sub

    ''' <summary>
    ''' Costruttore per il catasto
    ''' </summary>
    ''' <param name="Entita_cod"></param>
    ''' <param name="CatastoProv"></param>
    ''' <param name="CatastoCom"></param>
    ''' <param name="CatastoSezione"></param>
    ''' <param name="CatastoFoglio"></param>
    ''' <param name="CatastoNumero"></param>
    ''' <param name="CatastoSubalterno"></param>
    ''' <param name="HiddenPunti"></param>
    ''' <param name="Tipo_entita_cod"></param>
    ''' <param name="ElementoGrafico_DES"></param>
    ''' <param name="layer_codAtt"></param>
    Public Sub New(
        Entita_cod As Integer,
        CatastoProv As String,
        CatastoCom As String,
        CatastoSezione As String,
        CatastoFoglio As Integer,
        CatastoNumero As Integer,
        CatastoSubalterno As String,
        HiddenPunti As String,
        Tipo_entita_cod As Integer,
        ElementoGrafico_DES As String,
        layer_codAtt As Integer)

        Me.Piva = ""
        SaCod = 0
        EntitaCod = Entita_cod
        Me.HiddenPunti = HiddenPunti
        ProgrammazioneCod = 0
        ProgrammazioneEntitaCod = 0
        TipoEntitaCod = Tipo_entita_cod
        Me.ObjImpianto = New Appezzamento_Piccolo With {
            .Piva = "",
            .Sa_Cod = 0,
            .Appezza = 0,
            .Id_reg = 0,
            .campo_cod = 0
            }
        ElementoGraficoDes = ElementoGrafico_DES
        LayerCodAtt = layer_codAtt
        IdAgenda = 0
        IdMovDet = 0
        AnalisiCampioneCod = 0


        'corregge i default per il catasto
        If CatastoProv = "" Then
            CatastoProv = "0"
        End If

        If CatastoCom = "" Then
            CatastoCom = "0"
        End If


        If CatastoSezione = "" Then
            CatastoSezione = "-1"
        End If

        If CatastoSubalterno = "" Then
            CatastoSubalterno = "-1"
        End If

        If CatastoFoglio = 0 Then
            CatastoFoglio = -1
        End If

        If CatastoNumero = 0 Then
            CatastoNumero = -1
        End If
        'fine corregge i default per il catasto

        Me.CatastoProv = CatastoProv
        Me.CatastoCom = CatastoCom
        Me.CatastoSezione = CatastoSezione
        Me.CatastoFoglio = CatastoFoglio
        Me.CatastoNumero = CatastoNumero
        Me.CatastoSubalterno = CatastoSubalterno
    End Sub

    Public Property CatastoProv As String
    Public Property CatastoCom As String
    Public Property CatastoSezione As String
    Public Property CatastoFoglio As Integer
    Public Property CatastoNumero As Integer
    Public Property CatastoSubalterno As String

    Public Property Piva As String
    Public Property SaCod As String
    Public Property EntitaCod As Integer
    Public Property HiddenPunti As String
    Public Property ProgrammazioneCod As Integer
    Public Property ProgrammazioneEntitaCod As Integer
    Public Property TipoEntitaCod As Integer
    Public Property ObjImpianto As Appezzamento_Piccolo
    Public Property ElementoGraficoDes As String
    Public Property LayerCodAtt As Integer
    Public Property IdAgenda As Integer
    Public Property IdMovDet As Integer
    Public Property AnalisiCampioneCod As Integer
    Public Property StaticMapCFG As AgronicaCoreGisBIZ.GeneraMappaStaticaInData

End Class