Imports AgronicaCoreDataProvider

Public Class OModuli_Referenze_Config


    Public Function LeggiConfigurazione(
        ByVal piva As String,
        ByVal Id_Testata As Integer,
        ByVal SoloCampiAbilitatiEtichetta As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal objParametri_Server As AgronicaCoreParametri
    ) As OModuli_Referenze_Config_Testata_obj

        Dim rval As New OModuli_Referenze_Config_Testata_obj

        Dim leggiCFGDet As New OModuli_Referenze_Config_Dettagli_R
        Dim dt1 As DataTable =
            leggiCFGDet.Leggi(
                 piva,
                 Id_Testata,
                 SoloCampiAbilitatiEtichetta,
                 xFiltroAggiuntivo,
                 xOrderBy,
                 objParametri_Server
            )

        Dim leggiCFGLbl As New OModuli_Referenze_Config_Dettagli_Label_R
        Dim dt2 As DataTable

        For Each DDR In dt1.Rows
            Dim cD As New OModuli_Referenze_Config_Dettagli_obj With {
                .Piva = DDR("Piva"),
                .ID_Testata = Id_Testata,
                .Tabella_Key = DDR("Tabella_Key"),
                .Tipo = DDR("Tipo"),
                .Configurazione_Des = DDR("Configurazione_DES"),
                .ChkReferenza = DDR("ChkReferenza"),
                .ChkOrdine = DDR("Ordine"),
                .ChkOmni_Invisibili = DDR("ChkOmni_Invisibili"),
                .ChkEtichetta = DDR("ChkEtichetta"),
                .ChkEdit = DDR("ChkEdit")
            }

            dt2 = leggiCFGLbl.Leggi(
                DDR("Piva"),
                DDR("Id_Testata"),
                DDR("Tipo"),
                DDR("Tabella_id"),
                0,
                "",
                "",
                objParametri_Server
            )

            For Each DDR2 In dt2.Rows
                cD.CFG_Etichette.Add(
                    New OModuli_Referenze_Config_Dettagli_Label_obj With {
                        .IFF_Etichette_tipo = DDR2("iFF_Etichette_tipo"),
                        .NomeCampoAlias = DDR2("NomeCampoAlias")
                    }
                )
            Next

            rval.Configurazioni.Add(cD)
        Next


        Return rval

    End Function

End Class


