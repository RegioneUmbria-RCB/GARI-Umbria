
Public Class InsertTrattamento_Semplificato

	Public regscoNumero As String
	Public regscoQuantitativo As Double
	Public tratSpecieCodice As String
	Public tratCategoriaClassyfarmCodice As String
	Public tratNote As String

	Public Sub New(ByVal RegSco_Numero As String,
				   ByVal RegSco_Qta As Double,
				   ByVal Specie_Codice As String,
				   ByVal Categorie_ClassyFarm As String,
				   ByVal Note As String)
		regscoNumero = RegSco_Numero
		regscoQuantitativo = RegSco_Qta
		tratSpecieCodice = Specie_Codice
		tratCategoriaClassyfarmCodice = Categorie_ClassyFarm
		tratNote = Note
	End Sub

End Class
