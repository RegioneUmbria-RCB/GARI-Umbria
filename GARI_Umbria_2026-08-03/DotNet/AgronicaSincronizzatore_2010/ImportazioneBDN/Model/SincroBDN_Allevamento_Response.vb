
Public Class SincroBDN_Allevamento_Response
	Public cf_Detentore As String

	Public Detentore As String

	Public cf_Proprietario As String

	Public Proprietario As String
	Public NumeroCapiBDN As Integer


	Public listaCapi_Ingresso As List(Of Object)
	Public nCapi_Ingresso As Integer

	Public listaCapi_Uscita As List(Of Object)
	Public nCapi_Uscita As Integer

	Public capiDaControllare As SincroBDN_CapiDaControllare_Response
	Public Anomalie As DataTable
End Class

Public Class AnomalieAnagrafica
	Public Cod_Progetto As Integer
	Public Matricola As String
	Public Razza_Cod_BDN As String
	Public Razza_Des_BDN As String
	Public Razza_Cod_GIAS As Integer
	Public Razza_Des_GIAS As String
	Public Sesso_BDN As String
	Public Sesso_GIAS As Integer
	Public Matricola_Madre_BDN As String
	Public Matricola_Madre_GIAS As Integer
	Public Data_Nascita_BDN As Date
	Public Data_Nascita_GIAS As Date
	Public Anomalia_Cod As Integer
	Public Anomalia_Des As String
End Class

Public Class SincroBDN_CapiDaControllare_Response
	Public capiDB As List(Of String)
	Public capiBDN As List(Of String)
	Public MatricoleDuplicate As List(Of String)
End Class