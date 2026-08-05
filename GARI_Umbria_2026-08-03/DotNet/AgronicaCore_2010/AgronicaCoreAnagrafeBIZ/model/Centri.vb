Namespace AnagrafeNG
    Public Class Centri
        Public Impresa As Imprese

        Public Piva As String

        Public Sa_Cod As Integer
        Public Sa_Des As String
        Public Tipologia As Integer
        Public Titolo_Di_Possesso As Integer
        Public Lat As Decimal
        Public Lng As Decimal
        Public Rubrica As Rubrica()
        Public Validita_Inizio As Date
        Public Validita_Fine As Date
        Public Codici As Codici()
        Public Indirizzo As Indirizzi

        '''Tab Biologico
        Public Cod_Operatore As String
        Public Automatico As Boolean
        Public Attivita As String

        Public OTE As String()

    End Class

    Public Class CentroDropdownLists
        Public CodiceOperatore As String
        Public Tipologia As Web.UI.WebControls.DropDownList
        Public TitoloPossesso As Web.UI.WebControls.DropDownList
        Public Provincie As Web.UI.WebControls.DropDownList
        Public Comune As Web.UI.WebControls.DropDownList
        Public Stati As Web.UI.WebControls.DropDownList
        Public Codici As Web.UI.WebControls.DropDownList
        Public TipoAttivita As Web.UI.WebControls.DropDownList
        Public OrganismiControllo As Web.UI.WebControls.DropDownList
        Public CentroAziendaleEsternoCollegato As Web.UI.WebControls.DropDownList
        Public Otes As Web.UI.WebControls.DropDownList
    End Class

End Namespace

