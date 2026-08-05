Public Class Appezzamento
    Public Property id_appezzamento_padre As String
    Public Property id_appezzamento As String
    Public Property denominazione As String
    Public Property cod_nazionale As String
    Public Property area_mq As Int64
    Public Property data_inizio_validita As String
    Public Property data_fine_validita As String
    Public Property gis As GISPolygon
    Public Property dichiarazioni As List(Of Coltura)
    Public Property ultimo_aggiornamento As String
    Public Property chiave As AttributiChiaveAgeaAppezzamento
    Public Property is_certified As Boolean = True

    Public Sub New()
        dichiarazioni = New List(Of Coltura)
    End Sub
End Class

Public Class GISPolygon
    Public Property sr_code As String
    Public Property wkt As String
End Class

Public Class Coltura
    Public Property id_dichiarazione As String
    Public Property codice_coltura As String
    Public Property coltura As String
    Public Property area_mq As Int64
    Public Property data_inizio_validita As String
    Public Property data_fine_validita As String
    Public Property data_impianto As String
    Public Property bbch As List(Of Rilievi)
    Public Property protocolli As List(Of Protocollo)
    Public Property chiave As AttributiChiaveAgeaColtura
    Public Property datiAggiuntiviPCG As DatiAggiuntiviPCG
    Public Property is_overturned As Boolean = False
    Public Sub New()
        bbch = New List(Of Rilievi)
        protocolli = New List(Of Protocollo)
    End Sub
End Class

Public Class Protocollo
    Public Property protocollo As String
End Class

Public Class Rilievi
    Public Property codice As String
    Public Property data_accadimento As String
End Class

Public Class PianoColturaleGrafico
    Public Property nextKey As String
    Public Property records As List(Of Appezzamento)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of Appezzamento)
    End Sub
End Class

Public Class AppezzamentoSync
    Inherits Appezzamento

    Public Property cuaa As String
    Public Property campagna As Int64
    Public Property tipo_modifica As String
    Public Property data_eliminazione As String

End Class

Public Class PianoColturaleGraficoChangeLog
    Public Property nextKey As String
    Public Property records As List(Of AppezzamentoSync)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of AppezzamentoSync)
    End Sub
End Class

Public Class AttributiChiaveAgeaAppezzamento
    Public Property idSchedaValidazione As String
    Public Property identificativoPianoColtivazione As String
    Public Property codiBarrSchedaValidazione As String
    Public Property identificativoIsola As String
    Public Property identificativoAppezzamento As String
    Public Property idAppezzamentoOrig As String
End Class

Public Class AttributiChiaveAgeaColtura
    Public Property idColt As String
End Class

Public Class DatiAggiuntiviPCG
    Public Property zootechnical_effluents_id As Nullable(Of Integer)
    Public Property zootechnical_effluents As DichiarazioneInfoAggAnagrafica
    Public Property irrigation_potential_id As Nullable(Of Integer)
    Public Property irrigation_potential As DichiarazioneInfoAggAnagrafica
    Public Property company_structures_id As Nullable(Of Integer)
    Public Property company_structures As DichiarazioneInfoAggAnagrafica
    Public Property sowing_type_id As Nullable(Of Integer)
    Public Property sowing_type As DichiarazioneInfoAggAnagrafica
    Public Property breeding_phase_id As Nullable(Of Integer)
    Public Property breeding_phase As DichiarazioneInfoAggAnagrafica
    Public Property breeding_form_id As Nullable(Of Integer)
    Public Property breeding_form As DichiarazioneInfoAggAnagrafica
    Public Property breeding_type_id As Nullable(Of Integer)
    Public Property breeding_type As DichiarazioneInfoAggAnagrafica
    Public Property irrigation_type_id As Nullable(Of Integer)
    Public Property irrigation_type As DichiarazioneInfoAggAnagrafica
    Public Property year_month As String
End Class

Public Class DichiarazioneInfoAggAnagrafica
    Public Property type As String
    Public Property id As Integer
    Public Property code As String
    Public Property key As String
    Public Property name As String
    Public Property created_at As String        'datetime-string (da agea arriva in formato unix)
    Public Property updated_at As String        'datetime-string (da agea arriva in formato unix)
    Public Property deleted_at As String        'datetime-string (da agea arriva in formato unix)

End Class