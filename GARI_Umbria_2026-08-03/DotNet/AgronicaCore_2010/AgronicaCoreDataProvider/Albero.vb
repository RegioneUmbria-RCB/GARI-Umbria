Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Albero

    Private Const SeparatoreAlberoJson As String = "§"


    Public Class posizioneChiave
        Public Property Posizione As Integer
        Public Property Query As String
    End Class
    Public Class Albero_Icone
        Public Shared ReadOnly Property PathGiasBase As String
            Get
                Return "/GiasBase/agronica"
            End Get
        End Property

        Public Shared ReadOnly Property IconaUtente As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x01_Utente.png"
            End Get
        End Property


        Public Shared ReadOnly Property IconaImpresa As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x02_Impresa.png"
            End Get
        End Property
        Public Shared ReadOnly Property IconaCentroAziendale As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x03_Centro.png"
            End Get
        End Property
        Public Shared ReadOnly Property IconaCatasto As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/Catasto_02.ico"
            End Get
        End Property
        Public Shared ReadOnly Property IconaParticella As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x12_Particella.ico"
            End Get
        End Property
        Public Shared ReadOnly Property IconaPlanning As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/Planning.png"
            End Get
        End Property
        Public Shared ReadOnly Property IconaAnagrafica As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/X24 - Campo.bmp"
            End Get
        End Property
        Public Shared ReadOnly Property IconaCampo As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/CampoNew2.ico"
            End Get
        End Property
        Public Shared ReadOnly Property IconaAppezzamento As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x05_Appezzamento.png"
            End Get
        End Property
        Public Shared ReadOnly Property IconaImpianto As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/Icone24/x01_Utente.png"
            End Get
        End Property
        Public Shared ReadOnly Property IconeVegetaliBasePath As String
            Get
                Return "/GiasBase/agronica/AB_Immagini/IconeVegetali/"
            End Get
        End Property
        Public Shared ReadOnly Property IconaTerrenoNudo As String
            Get
                Return "9999999.ico"
            End Get
        End Property

        Private Shared Function Setta_Path_GiasBase() As String

        End Function

    End Class
    Public Class Albero_Posizioni

        Public Shared ReadOnly Property EsempioMaschera As String
            Get
                Return "0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0"
            End Get
        End Property
        Public Shared ReadOnly Property TipoNodo As Integer
            Get
                Return 0
            End Get
        End Property
        Public Shared ReadOnly Property Piva As Integer
            Get
                Return 1
            End Get
        End Property
        Public Shared ReadOnly Property Sa_Cod As Integer
            Get
                Return 2
            End Get
        End Property
        Public Shared ReadOnly Property Campo_Cod As Integer
            Get
                Return 3
            End Get
        End Property
        Public Shared ReadOnly Property Appezza As Integer
            Get
                Return 4
            End Get
        End Property
        Public Shared ReadOnly Property Id_Imp As Integer
            Get
                Return 5
            End Get
        End Property
        Public Shared ReadOnly Property p_Part_Cod As Integer
            Get
                Return 6
            End Get
        End Property
        Public Shared ReadOnly Property p_Provincia_Cod As Integer
            Get
                Return 7
            End Get
        End Property
        Public Shared ReadOnly Property p_Comune_Cod As Integer
            Get
                Return 8
            End Get
        End Property
        Public Shared ReadOnly Property p_Sezione As Integer
            Get
                Return 9
            End Get
        End Property
        Public Shared ReadOnly Property p_Foglio As Integer
            Get
                Return 10
            End Get
        End Property
        Public Shared ReadOnly Property p_Numero As Integer
            Get
                Return 11
            End Get
        End Property
        Public Shared ReadOnly Property p_Subalterno As Integer
            Get
                Return 12
            End Get
        End Property
        Public Shared ReadOnly Property Cod_Fiscale As Integer
            Get
                Return 13
            End Get
        End Property
        Public Shared ReadOnly Property Fabbricato_Cod As Integer
            Get
                Return 14
            End Get
        End Property
        Public Shared ReadOnly Property Prodotto_Cod As Integer
            Get
                Return 15
            End Get
        End Property
        Public Shared ReadOnly Property Data_Lavorazione As Integer
            Get
                Return 16
            End Get
        End Property
        Public Shared ReadOnly Property Analisi_Certificato_Cod As Integer
            Get
                Return 17
            End Get
        End Property
        Public Shared ReadOnly Property Analisi_Testata_Cod As Integer
            Get
                Return 18
            End Get
        End Property
        Public Shared ReadOnly Property Analisi_Dettaglio_Cod As Integer
            Get
                Return 19
            End Get
        End Property
        Public Shared ReadOnly Property Analisi_Campione_Cod As Integer
            Get
                Return 20
            End Get
        End Property
        Public Shared ReadOnly Property PianoConcimazione_Testata_Cod As Integer
            Get
                Return 21
            End Get
        End Property
        Public Shared ReadOnly Property Progetto_Cod As Integer
            Get
                Return 22
            End Get
        End Property
        Public Shared ReadOnly Property Programmazione_Cod As Integer
            Get
                Return 23
            End Get
        End Property
        Public Shared ReadOnly Property Programmazione_Entita_Cod As Integer
            Get
                Return 24
            End Get
        End Property
        Public Shared ReadOnly Property id_agenda As Integer
            Get
                Return 25
            End Get
        End Property
        Public Shared ReadOnly Property PivaPadre As Integer
            Get
                Return 26
            End Get
        End Property
        Public Shared ReadOnly Property Ricetta_Cod As Integer
            Get
                Return 27
            End Get
        End Property
        Public Shared ReadOnly Property ricetta_Operazione_cod As Integer
            Get
                Return 28
            End Get
        End Property


    End Class


    Public Class Albero_Descrizioni_Query
        Public Shared Function DQ_CampoDet(ByVal aliasTabellaCampi As String, aliasColonnaDescrizioneCampo As String, ColonnaSuperficie As String) As String

            '"{Campo} : Vanni TEST"
            Dim rval As New StringBuilder
            rval.AppendLine("'{Campo}' + " & aliasTabellaCampi & "." & aliasColonnaDescrizioneCampo)
            Return rval.ToString()

        End Function

        Public Shared Function DQ_PlanningEntita(ByVal aliasTabellaEntita As String, aliasTabellaDescrizioneSpecieCultivar As String, aliasColonnaDescrizioneSpecieCultivar As String, ByVal aliasTabellaEColonnaAppNome As String, ColonnaSuperficie As String) As String

            '"[01/01/2020 .. 31/12/2020] - App. 009 - Vite - Altre: { 1,2813 Ha }"
            Dim rval As New StringBuilder
            DQ_Date(aliasTabellaEntita, rval)
            rval.Append("     " & aliasTabellaEColonnaAppNome & " + ' - '  + ")
            rval.Append("     " & aliasTabellaDescrizioneSpecieCultivar & "." & aliasColonnaDescrizioneSpecieCultivar & " + ' - '  + ")
            DQ_Superficie(aliasTabellaEntita, ColonnaSuperficie, rval)
            Return rval.ToString()

        End Function

        Private Shared Sub DQ_Superficie(aliasTabellaSup As String, colonnaSup As String, rval As StringBuilder)
            rval.Append("    '{' + replace(cast(" & aliasTabellaSup & "." & colonnaSup & " as varchar(20)), '.', ',') + ' Ha }'")
        End Sub

        Private Shared Sub DQ_Date(aliasTabellaDate As String, rval As StringBuilder)
            rval.Append("'[' + ")
            rval.Append(DQ_Case_hlp(aliasTabellaDate & ".Validita_inizio", "Convert(varchar(50), " & aliasTabellaDate & ".Validita_inizio, 103)", UtilityProvider.Agro_SQL_SaveDate(AGRODATAINIZIO), "_") & "    + ' .. ' + ")
            rval.Append(DQ_Case_hlp(aliasTabellaDate & ".Validita_fine", "Convert(varchar(50), " & aliasTabellaDate & ".Validita_fine, 103)", UtilityProvider.Agro_SQL_SaveDate(AGRODATAFINE), "_") & " + ")
            rval.Append(" '] - '  + ")
        End Sub

        Public Shared Function DQ_Appezzamento(ByVal aliasTabellaAppezzamento As String) As String

            '"[01/01/2020 .. 31/12/2020] - App. 009 : { 1,2813 Ha }"
            Dim rval As New StringBuilder
            DQ_Date(aliasTabellaAppezzamento, rval)
            rval.Append("     " & aliasTabellaAppezzamento & ".App_Nome + ' - '  + ")
            DQ_Superficie(aliasTabellaAppezzamento, "Sup_app", rval)
            Return rval.ToString()

        End Function

        Public Shared Function DQ_Style_Appezzamento(ByVal aliasTabellaAppezzamento As String) As String

            ' --   'Dim style As String = If(DT_Impianti.Rows(i).Item("Blk_Flag") = -1, "red", If(xValidita_Fine < Date.Now.Date, "DimGray", If(Id_Consociazione <> 0, "blue", "")))
            Dim stb As New StringBuilder
            stb.AppendLine(" Case when " & aliasTabellaAppezzamento & ".Blk_Flag = -1 then 'red' else  ")
            stb.AppendLine("      Case when " & aliasTabellaAppezzamento & ".Validita_Fine < getdate() then 'DimGray' else '' ")
            stb.AppendLine("      End ")
            stb.AppendLine(" End")

            Return stb.ToString()

        End Function

        Public Shared Function DQ_Style_Impianto(ByVal aliasTabellaAppezzamento As String, ByVal aliasTabellaImpianto As String) As String

            ' --   'Dim style As String = If(DT_Impianti.Rows(i).Item("Blk_Flag") = -1, "red", If(xValidita_Fine < Date.Now.Date, "DimGray", If(Id_Consociazione <> 0, "blue", "")))
            Dim stb As New StringBuilder
            stb.AppendLine(" Case when " & aliasTabellaAppezzamento & ".Blk_Flag = -1 then 'red' else  ")
            stb.AppendLine("      Case when " & aliasTabellaAppezzamento & ".Validita_Fine < getdate() then 'DimGray' else  ")
            stb.AppendLine("          Case when " & aliasTabellaImpianto & ".id_consociazione <> 0 then 'blue' else '' end ")
            stb.AppendLine("      End ")
            stb.AppendLine(" End")
            Return stb.ToString()

        End Function
        Public Shared Function DQ_Impianto(ByVal aliasTabellaImpianti As String, aliasTabellaDescrizioneSpecieCultivar As String, aliasColonnaDescrizioneSpecieCultivar As String, ByVal aliasAppNome As String, ColonnaSuperficie As String) As String

            '"[01/01/2020 .. 31/12/2020] - App. 009 - Vite - Altre: { 1,2813 Ha }"
            Return DQ_PlanningEntita(aliasTabellaImpianti, aliasTabellaDescrizioneSpecieCultivar, aliasColonnaDescrizioneSpecieCultivar, aliasAppNome, ColonnaSuperficie)

        End Function

        Public Shared Function DQ_Particella(ByVal aliasParticelle As String, ByVal aliasIstat As String, ByVal aliasConduzione As String, aliasColonnaSuperficie As String) As String

            '"{RM : M297 : FIUMICINO : __ : ___314 : ___289 : __} ..... 132,9241 [ha] ..... Proprietà"
            Dim rval As New StringBuilder
            rval.Append("'{' +")
            rval.Append(aliasIstat & ".Comuni_Prov + ' : ' + ")
            rval.Append(aliasIstat & ".CodiceCatastale + ' : ' + ")
            rval.Append(aliasIstat & ".Localita + ' : ' + ")
            rval.Append(" RIGHT('_______' + " & DQ_Case_hlp(aliasParticelle & ".Sezione", aliasParticelle & ".Sezione", "'0'", "_") & " , 2) + ' : ' + ")
            rval.Append(" RIGHT('_______' + cast(" & aliasParticelle & ".Foglio as varchar(100)), 6) + ' : ' + ")
            rval.Append(" RIGHT('_______' + cast(" & aliasParticelle & ".Numero as varchar(100)), 6) + ' : ' + ")
            rval.Append(" RIGHT('_______' + " & DQ_Case_hlp(aliasParticelle & ".Subalterno", aliasParticelle & ".Subalterno", "'0'", "_") & " , 2) + '}' + ")
            rval.Append(" '..... ' + replace(cast(" & aliasParticelle & "." & aliasColonnaSuperficie & " as varchar(100)), '.', ',')  + ' [ha] ..... ' ")

            If Not String.IsNullOrEmpty(aliasConduzione) Then
                rval.Append(" + " & aliasConduzione & ".TitoloPossessoDes")
            End If

            Return rval.ToString()

        End Function

        Public Shared Function DQ_Case_hlp(StatementConValoreDaConfrontare As String, StatementConValoreSelect As String, val As String, padchar As String) As String
            Dim rval As String =
                " case when " & StatementConValoreDaConfrontare & " = " & val & " then '" & padchar & "' else " & StatementConValoreSelect & " end "
            Return rval
        End Function
    End Class

    Public Class Albero_Posizioni_Query

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaImpresa">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Impresa(aliasTabellaImpresa As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.Impresa
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaImpresa & "Piva + '"
                 })
            Return rval

        End Function


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaCentro">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_CentroAziendale(aliasTabellaCentro As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.Centro
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaCentro & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaCentro & "Sa_cod as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaCatasto">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Catasto(aliasTabellaCatasto As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.CatastoAziendale
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaCatasto & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaCatasto & "Sa_cod as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaCampo">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Campo(aliasTabellaCampo As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.Campo
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaCampo & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaCampo & "Sa_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Campo_Cod,
                    .Query = "' + cast(" & aliasTabellaCampo & "Campo_cod as varchar(100)) + '"
                 })
            Return rval

        End Function

        Public Shared Function PQ_PlanningEtichetta() As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            Return rval

        End Function

        Public Shared Function PQ_Anagrafica() As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaAppezzamento">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Appezzamento(
            TipoNodo As enum_TipoNodo,
            aliasTabellaAppezzamento As String,
            aliasAnag As String
        ) As List(Of posizioneChiave)

            Dim QryTipoNodo As String = TipoNodo
            If TipoNodo = enum_TipoNodo.Impianto_Generico Then

                Dim stb As New StringBuilder
                stb.AppendLine("' + Case when " & aliasAnag & "gru_cod = " & enum_GruppoVegetale.Arboree & " then '" & enum_TipoNodo.ImpiantoArborea & "' else  ")
                stb.AppendLine("    Case when " & aliasAnag & "gru_cod = " & enum_GruppoVegetale.Erbacee & " then '" & enum_TipoNodo.ImpiantoErbacea & "' else ")
                stb.AppendLine("        Case when " & aliasAnag & "gru_cod = " & enum_GruppoVegetale.OrtoFloroVivaismo & " then '" & enum_TipoNodo.ImpiantoOrticola & "' else '" & enum_TipoNodo.Impianto_Generico & "' ")
                stb.AppendLine("        End ")
                stb.AppendLine("    End ")
                stb.AppendLine("End")
                stb.Append(" + '")

                QryTipoNodo = stb.ToString()
            End If

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = QryTipoNodo
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaAppezzamento & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaAppezzamento & "Sa_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Campo_Cod,
                    .Query = "' + cast(" & aliasTabellaAppezzamento & "Campo_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Appezza,
                    .Query = "' + cast(" & aliasTabellaAppezzamento & "Appezza as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaImpresa">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Impianto(aliasTabellaAppezzamento As String, aliasTabellaImpianto As String, aliasAnag As String) As List(Of posizioneChiave)

            Dim rval As List(Of posizioneChiave)
            rval = PQ_Appezzamento(enum_TipoNodo.Impianto_Generico, aliasTabellaAppezzamento, aliasAnag)

            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Id_Imp,
                    .Query = "' + cast(" & aliasTabellaImpianto & "Id_reg as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaTestata">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_PlanningTestata(aliasTabellaTestata As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.PlanningTestata
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaTestata & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaTestata & "Sa_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Programmazione_Cod,
                    .Query = "' + cast(" & aliasTabellaTestata & "programmazione_Cod as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaEntita">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_PlanningEntita(aliasTabellaEntita As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.PlanningEntitaImpianto
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaEntita & "Piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaEntita & "Sa_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Programmazione_Cod,
                    .Query = "' + cast(" & aliasTabellaEntita & "programmazione_Cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Programmazione_Entita_Cod,
                    .Query = "' + cast(" & aliasTabellaEntita & "programmazione_Entita_Cod as varchar(100)) + '"
                 })
            Return rval

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aliasTabellaParticelleXAtro">alias nella query compreso il punto</param>
        ''' <returns></returns>
        Public Shared Function PQ_Particella(aliasTabellaParticelleXAtro As String, aliasTabellaParticelle As String) As List(Of posizioneChiave)

            Dim rval As New List(Of posizioneChiave)
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.TipoNodo,
                    .Query = enum_TipoNodo.Particella
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Piva,
                    .Query = "' + " & aliasTabellaParticelleXAtro & "piva + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.Sa_Cod,
                    .Query = "' + cast(" & aliasTabellaParticelleXAtro & "sa_cod as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Part_Cod,
                    .Query = "' + cast(" & aliasTabellaParticelle & "part_cod  as varchar(100)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Provincia_Cod,
                    .Query = "' + " & aliasTabellaParticelleXAtro & "prov + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Comune_Cod,
                    .Query = "' + " & aliasTabellaParticelleXAtro & "com + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Sezione,
                    .Query = "' + " & aliasTabellaParticelleXAtro & "sezione + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Foglio,
                    .Query = "' + cast(" & aliasTabellaParticelleXAtro & "foglio as varchar(10)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Numero,
                    .Query = "' + cast(" & aliasTabellaParticelleXAtro & "numero as varchar(10)) + '"
                 })
            rval.Add(New posizioneChiave With {
                    .Posizione = Albero_Posizioni.p_Subalterno,
                    .Query = "' + " & aliasTabellaParticelleXAtro & "subalterno + '"
                 })
            Return rval

        End Function
    End Class

    '################################################################################
    Public Shared Function PCEntitaCod_from_ChiaveAlberoImprese(ByVal Chiave_Albero_Imprese As String) As enum_Entita_PianoConcimazione

        Dim xTipoNodo As enum_TipoNodo
        Dim PC_Entita_Cod As enum_Entita_PianoConcimazione

        Dim Piva, CodFiscale As String
        Dim Sa_Cod, Campo_Cod, Appezza, Id_Imp, Fabbricato_Cod As Integer
        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero, Part_Cod As Integer


        If Chiave_Albero_Imprese <> "" Then

            '##############################################################
            '#####  Decodifico la chiave  #################################
            '##############################################################

            'Identifico il tipo di nodo 
            Call ChiaveAlbero_Decodifica_TipoNodo_x_json(Chiave_Albero_Imprese, xTipoNodo)

            'Chiamo la decodifica opportuna a seconda del tipo di nodo
            Select Case xTipoNodo


                Case enum_TipoNodo.Impresa,
                     enum_TipoNodo.Centro,
                     enum_TipoNodo.Campo,
                     enum_TipoNodo.Appezzamento,
                     enum_TipoNodo.ImpiantoArborea,
                     enum_TipoNodo.ImpiantoErbacea,
                     enum_TipoNodo.ImpiantoOrticola,
                     enum_TipoNodo.ImpiantoNudo,
                     enum_TipoNodo.CatastoAziendale

                    'Data la chiave ricavo gli elementi che la compongono
                    Call ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                                        Chiave_Albero_Imprese,
                                                        xTipoNodo,
                                                        Piva,
                                                        Sa_Cod,
                                                        Campo_Cod,
                                                        Appezza,
                                                        Id_Imp,
                                                        CodFiscale,
                                                        Fabbricato_Cod)
                    '-----------------------------------------

                Case enum_TipoNodo.Persona

                    'Data la chiave ricavo gli elementi che la compongono
                    Call ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                                        Chiave_Albero_Imprese,
                                                        xTipoNodo,
                                                        Piva,
                                                        Sa_Cod,
                                                        Campo_Cod,
                                                        Appezza,
                                                        Id_Imp,
                                                        CodFiscale,
                                                        Fabbricato_Cod)
                    '-----------------------------------------


                Case enum_TipoNodo.f_Abitazione,
                     enum_TipoNodo.f_CellaFrigorifera,
                     enum_TipoNodo.f_ImpiantoLavorazione,
                     enum_TipoNodo.f_Magazzino,
                     enum_TipoNodo.f_Silos,
                     enum_TipoNodo.f_Stalla,
                     enum_TipoNodo.f_Fienile,
                     enum_TipoNodo.Fabbricato_Generico


                    'Data la chiave ricavo gli elementi che la compongono
                    Call ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                                        Chiave_Albero_Imprese,
                                                        xTipoNodo,
                                                        Piva,
                                                        Sa_Cod,
                                                        Campo_Cod,
                                                        Appezza,
                                                        Id_Imp,
                                                        CodFiscale,
                                                        Fabbricato_Cod)
                    '-----------------------------------------


                Case enum_TipoNodo.Particella

                    'Data la chiave ricavo gli elementi che la compongono
                    Call ChiaveAlbero_Decodifica_Particelle_x_json(
                                                    Chiave_Albero_Imprese,
                                                    xTipoNodo,
                                                    Piva,
                                                    Sa_Cod,
                                                    Prov,
                                                    Com,
                                                    Sezione,
                                                    Foglio,
                                                    Numero,
                                                    Subalterno,
                                                    Part_Cod)
                    '-----------------------------------------

            End Select


            Select Case xTipoNodo

                Case enum_TipoNodo.Impresa
                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Impresa
                    '--------------------

                Case enum_TipoNodo.Centro
                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Centro
                    '--------------------

                Case enum_TipoNodo.Campo
                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Campo
                    '--------------------

                Case enum_TipoNodo.Appezzamento
                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Appezzamento
                    '--------------------

                Case enum_TipoNodo.ImpiantoArborea,
                     enum_TipoNodo.ImpiantoErbacea,
                     enum_TipoNodo.ImpiantoOrticola,
                     enum_TipoNodo.ImpiantoNudo

                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Impianto
                    '--------------------

                Case enum_TipoNodo.f_Abitazione,
                     enum_TipoNodo.f_CellaFrigorifera,
                     enum_TipoNodo.f_ImpiantoLavorazione,
                     enum_TipoNodo.f_Magazzino,
                     enum_TipoNodo.f_Silos,
                     enum_TipoNodo.f_Stalla,
                     enum_TipoNodo.f_Fienile,
                     enum_TipoNodo.Fabbricato_Generico

                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Fabbricato
                    '--------------------

                Case enum_TipoNodo.Particella

                    PC_Entita_Cod = enum_Entita_PianoConcimazione.Particella
                    '--------------------

            End Select

        Else
            Throw New Exception("Chiave_Albero_Imprese non definita!")
        End If

        Return PC_Entita_Cod


    End Function

    '###############################################################################
    'Public Shared Sub ChiaveAlbero_Decodifica_TipoNodo( _
    '                            ByVal Chiave As String, _
    '                            ByRef TipoNodo As enum_TipoNodo)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero l'elemento di indice 0 (TipoNodo)
    '    If ArrayKey(0) <> "" Then
    '        TipoNodo = CInt(ArrayKey(0))
    '    End If


    'End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_TipoNodo_x_json(
                               ByVal Chiave As String,
                               ByRef TipoNodo As enum_TipoNodo)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero l'elemento di indice 0 (TipoNodo)
        If ArrayKey(0) <> "" Then
            TipoNodo = CInt(ArrayKey(0))
        End If


    End Sub

    '###############################################################################
    'Public Shared Sub ChiaveAlbero_Decodifica_ImpiantiVegetali( _
    '                            ByVal Chiave As String, _
    '                            ByRef TipoNodo As enum_TipoNodo, _
    '                            ByRef Piva As String, _
    '                            ByRef Sa_Cod As Integer, _
    '                            ByRef Campo_Cod As Integer, _
    '                            ByRef Appezza As Integer, _
    '                            ByRef Id_Imp As Integer, _
    '                            ByRef Cod_Fiscale As String, _
    '                            ByRef Fabbricato_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    TipoNodo = CInt(ArrayKey(0))
    '    Piva = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Campo_Cod = CInt(ArrayKey(3))
    '    Appezza = CInt(ArrayKey(4))
    '    Id_Imp = CInt(ArrayKey(5))
    '    Cod_Fiscale = CStr(ArrayKey(13))
    '    Fabbricato_Cod = CInt(ArrayKey(14))

    'End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                ByVal Chiave As String,
                                ByRef TipoNodo As enum_TipoNodo,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Campo_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Id_Imp As Integer,
                                ByRef Cod_Fiscale As String,
                                ByRef Fabbricato_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)
        If ArrayKey.Length < 2 Then
            ArrayKey = Split(Chiave, "\")
        End If

        'Recupero gli elementi
        TipoNodo = CInt(ArrayKey(0))
        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Campo_Cod = CInt(ArrayKey(3))
        Appezza = CInt(ArrayKey(4))
        Id_Imp = CInt(ArrayKey(5))
        Cod_Fiscale = CStr(ArrayKey(13))
        Fabbricato_Cod = CInt(ArrayKey(14))

    End Sub


    '###############################################################################
    'Public Shared Sub ChiaveAlbero_Decodifica_Particelle( _
    '                            ByVal Chiave As String, _
    '                            ByRef TipoNodo As enum_TipoNodo, _
    '                            ByRef Piva As String, _
    '                            ByRef Sa_Cod As Integer, _
    '                            ByRef Provincia_Cod As String, _
    '                            ByRef Comune_Cod As String, _
    '                            ByRef Sezione As String, _
    '                            ByRef Foglio As Integer, _
    '                            ByRef Numero As Integer, _
    '                            ByRef Subalterno As String, _
    '                            ByRef Part_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    TipoNodo = CInt(ArrayKey(0))
    '    Piva = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Provincia_Cod = CStr(ArrayKey(7))
    '    Comune_Cod = CStr(ArrayKey(8))
    '    Sezione = CStr(ArrayKey(9))
    '    Foglio = CInt(ArrayKey(10))
    '    Numero = CInt(ArrayKey(11))
    '    Subalterno = CStr(ArrayKey(12))
    '    Part_Cod = CInt(ArrayKey(6))

    'End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_Particelle_x_json(
                                ByVal Chiave As String,
                                ByRef TipoNodo As enum_TipoNodo,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Provincia_Cod As String,
                                ByRef Comune_Cod As String,
                                ByRef Sezione As String,
                                ByRef Foglio As Integer,
                                ByRef Numero As Integer,
                                ByRef Subalterno As String,
                                ByRef Part_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        TipoNodo = CInt(ArrayKey(0))
        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Provincia_Cod = CStr(ArrayKey(7))
        Comune_Cod = CStr(ArrayKey(8))
        Sezione = CStr(ArrayKey(9))
        Foglio = CInt(ArrayKey(10))
        Numero = CInt(ArrayKey(11))
        Subalterno = CStr(ArrayKey(12))
        Part_Cod = CInt(ArrayKey(6))

    End Sub

    ' ###############################################################################
    Public Shared Sub ChiaveAlbero_Codifica(
                                ByRef Chiave As String,
                                ByVal TipoNodo As enum_TipoNodo,
                                Optional ByVal Piva As String = "0",
                                Optional ByVal Sa_Cod As Integer = 0,
                                Optional ByVal Campo_Cod As Integer = 0,
                                Optional ByVal Appezza As Integer = 0,
                                Optional ByVal Id_Imp As Integer = 0,
                                Optional ByVal p_Part_Cod As Integer = 0,
                                Optional ByVal p_Provincia_Cod As String = "0",
                                Optional ByVal p_Comune_Cod As String = "0",
                                Optional ByVal p_Sezione As String = "0",
                                Optional ByVal p_Foglio As Integer = 0,
                                Optional ByVal p_Numero As Integer = 0,
                                Optional ByVal p_Subalterno As String = "0",
                                Optional ByVal Cod_Fiscale As String = "0",
                                Optional ByVal Fabbricato_Cod As Integer = 0,
                                Optional ByVal Prodotto_Cod As Integer = 0,
                                Optional ByVal Data_Lavorazione As String = "0",
                                Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                                Optional ByVal Analisi_Testata_Cod As Integer = 0,
                                Optional ByVal Analisi_Dettaglio_Cod As Integer = 0,
                                Optional ByVal Analisi_Campione_Cod As Integer = 0,
                                Optional ByVal PianoConcimazione_Testata_Cod As Integer = 0,
                                Optional ByVal Progetto_Cod As Integer = 0,
                                Optional ByVal Programmazione_Cod As Integer = 0,
                                Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                                Optional ByVal id_agenda As Integer = 0,
                                Optional ByVal PivaPadre As String = "",
                                Optional ByVal Ricetta_Cod As Integer = 0,
                                Optional ByVal ricetta_Operazione_cod As Integer = 0
                        )
        '------------------------------------------------------------------------
        Dim StrKey As String = ""

        StrKey = StrKey & CStr(TipoNodo)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Piva)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Sa_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Campo_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Appezza)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Id_Imp)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Part_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Provincia_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Comune_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Sezione)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Foglio)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Numero)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(p_Subalterno)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Cod_Fiscale)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Fabbricato_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Prodotto_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Data_Lavorazione)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Analisi_Certificato_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Analisi_Testata_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Analisi_Dettaglio_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Analisi_Campione_Cod)
        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(PianoConcimazione_Testata_Cod)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Progetto_Cod)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Programmazione_Cod)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Programmazione_Entita_Cod)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(id_agenda)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(PivaPadre)


        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(Ricetta_Cod)

        StrKey = StrKey & "\"
        StrKey = StrKey & CStr(ricetta_Operazione_cod)

        'Restituisco il risultato
        Chiave = StrKey

    End Sub



    Public Shared Function ChiaveAlbero_Codifica_x_json_solo_valorizzati(
                            ByRef Chiave As String,
                            ByVal TipoNodo As enum_TipoNodo,
                            Optional ByVal Piva As String = "0",
                            Optional ByVal Sa_Cod As Integer = 0,
                            Optional ByVal Campo_Cod As Integer = 0,
                            Optional ByVal Appezza As Integer = 0,
                            Optional ByVal Id_Imp As Integer = 0,
                            Optional ByVal p_Part_Cod As Integer = 0,
                            Optional ByVal p_Provincia_Cod As String = "0",
                            Optional ByVal p_Comune_Cod As String = "0",
                            Optional ByVal p_Sezione As String = "0",
                            Optional ByVal p_Foglio As Integer = 0,
                            Optional ByVal p_Numero As Integer = 0,
                            Optional ByVal p_Subalterno As String = "0",
                            Optional ByVal Cod_Fiscale As String = "0",
                            Optional ByVal Fabbricato_Cod As Integer = 0,
                            Optional ByVal Prodotto_Cod As Integer = 0,
                            Optional ByVal Data_Lavorazione As String = "0",
                            Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                            Optional ByVal Analisi_Testata_Cod As Integer = 0,
                            Optional ByVal Analisi_Dettaglio_Cod As Integer = 0,
                            Optional ByVal Analisi_Campione_Cod As Integer = 0,
                            Optional ByVal PianoConcimazione_Testata_Cod As Integer = 0,
                            Optional ByVal Progetto_Cod As Integer = 0,
                            Optional ByVal Programmazione_Cod As Integer = 0,
                            Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                            Optional ByVal id_agenda As Integer = 0,
                            Optional ByVal PivaPadre As String = "",
                            Optional ByVal Ricetta_Cod As Integer = 0,
                            Optional ByVal ricetta_Operazione_cod As Integer = 0
                    ) As String
        '------------------------------------------------------------------------
        Dim StrKey As String

        StrKey = ""

        StrKey = StrKey & CStr(TipoNodo)
        Dim lSperator As String = SeparatoreAlberoJson
        If Piva <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Piva)
        End If
        If Sa_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Sa_Cod)
        End If
        If TipoNodo = enum_TipoNodo.Appezzamento Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Campo_Cod)
        Else
            If Campo_Cod <> 0 Then
                StrKey = StrKey & lSperator
                StrKey = StrKey & CStr(Campo_Cod)
            End If
        End If

        If Appezza <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Appezza)
        End If
        If Id_Imp <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Id_Imp)
        End If
        If p_Part_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Part_Cod)
        End If
        If p_Provincia_Cod <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Provincia_Cod)
        End If
        If p_Comune_Cod <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Comune_Cod)
        End If

        If p_Sezione <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Sezione)
        End If
        If p_Foglio <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Foglio)
        End If
        If p_Numero <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Numero)
        End If

        If p_Subalterno <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(p_Subalterno)
        End If
        If Cod_Fiscale <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Cod_Fiscale)
        End If
        If Fabbricato_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Fabbricato_Cod)
        End If
        If Prodotto_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Prodotto_Cod)
        End If
        If Data_Lavorazione <> "0" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Data_Lavorazione)
        End If
        If Analisi_Certificato_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Analisi_Certificato_Cod)
        End If

        If Analisi_Testata_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Analisi_Testata_Cod)
        End If
        If Analisi_Dettaglio_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Analisi_Dettaglio_Cod)
        End If
        If Analisi_Campione_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Analisi_Campione_Cod)
        End If
        If PianoConcimazione_Testata_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(PianoConcimazione_Testata_Cod)
        End If



        If Progetto_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Progetto_Cod)
        End If
        If Programmazione_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Programmazione_Cod)
        End If
        If Programmazione_Entita_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Programmazione_Entita_Cod)
        End If
        If id_agenda <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(id_agenda)
        End If



        If PivaPadre <> "0" OrElse PivaPadre <> "" Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(PivaPadre)
        End If
        If Ricetta_Cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(Ricetta_Cod)
        End If
        If ricetta_Operazione_cod <> 0 Then
            StrKey = StrKey & lSperator
            StrKey = StrKey & CStr(ricetta_Operazione_cod)
        End If

        'Restituisco il risultato
        Chiave = StrKey.ToString
        Return Chiave

    End Function


    ' ##### NB : il parametro Vas_Cod (nella decodifica) in realtà è il nostro Progetto_Cod nella codifica
    ' ### L'errore è nella decodifica e non il contrario (all'indice 22 del vettore splittato)

    Public Shared Sub ChiaveAlbero_Codifica_x_json(
                            ByRef Chiave As String,
                            ByVal TipoNodo As enum_TipoNodo,
                            Optional ByVal Piva As String = "0",
                            Optional ByVal Sa_Cod As Integer = 0,
                            Optional ByVal Campo_Cod As Integer = 0,
                            Optional ByVal Appezza As Integer = 0,
                            Optional ByVal Id_Imp As Integer = 0,
                            Optional ByVal p_Part_Cod As Integer = 0,
                            Optional ByVal p_Provincia_Cod As String = "0",
                            Optional ByVal p_Comune_Cod As String = "0",
                            Optional ByVal p_Sezione As String = "0",
                            Optional ByVal p_Foglio As Integer = 0,
                            Optional ByVal p_Numero As Integer = 0,
                            Optional ByVal p_Subalterno As String = "0",
                            Optional ByVal Cod_Fiscale As String = "0",
                            Optional ByVal Fabbricato_Cod As Integer = 0,
                            Optional ByVal Prodotto_Cod As Integer = 0,
                            Optional ByVal Data_Lavorazione As String = "0",
                            Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                            Optional ByVal Analisi_Testata_Cod As Integer = 0,
                            Optional ByVal Analisi_Dettaglio_Cod As Integer = 0,
                            Optional ByVal Analisi_Campione_Cod As Integer = 0,
                            Optional ByVal PianoConcimazione_Testata_Cod As Integer = 0,
                            Optional ByVal Progetto_Cod As Integer = 0,
                            Optional ByVal Programmazione_Cod As Integer = 0,
                            Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                            Optional ByVal id_agenda As Integer = 0,
                            Optional ByVal PivaPadre As String = "",
                            Optional ByVal Ricetta_Cod As Integer = 0,
                            Optional ByVal ricetta_Operazione_cod As Integer = 0
                    )
        '------------------------------------------------------------------------
        Dim StrKey As New StringBuilder

        StrKey.Append(CStr(TipoNodo))
        Dim lSperator As String = SeparatoreAlberoJson
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Piva))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Sa_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Campo_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Appezza))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Id_Imp))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Part_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Provincia_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Comune_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Sezione))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Foglio))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Numero))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(p_Subalterno))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Cod_Fiscale))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Fabbricato_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Prodotto_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Data_Lavorazione))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(Analisi_Certificato_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Analisi_Testata_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Analisi_Dettaglio_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(Analisi_Campione_Cod))
        StrKey.Append(lSperator)
        StrKey.Append(CStr(PianoConcimazione_Testata_Cod))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(Progetto_Cod))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(Programmazione_Cod))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(Programmazione_Entita_Cod))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(id_agenda))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(PivaPadre))


        StrKey.Append(lSperator)
        StrKey.Append(CStr(Ricetta_Cod))

        StrKey.Append(lSperator)
        StrKey.Append(CStr(ricetta_Operazione_cod))


        'Restituisco il risultato
        Chiave = StrKey.ToString

    End Sub



    Public Shared Function ChiaveAlbero_Codifica_x_json_faster(
                            ByRef Chiave As String,
                            ByVal TipoNodo As enum_TipoNodo,
                            Optional ByVal Piva As String = "0",
                            Optional ByVal Sa_Cod As Integer = 0,
                            Optional ByVal Campo_Cod As Integer = 0,
                            Optional ByVal Appezza As Integer = 0,
                            Optional ByVal Id_Imp As Integer = 0,
                            Optional ByVal p_Part_Cod As Integer = 0,
                            Optional ByVal p_Provincia_Cod As String = "0",
                            Optional ByVal p_Comune_Cod As String = "0",
                            Optional ByVal p_Sezione As String = "0",
                            Optional ByVal p_Foglio As Integer = 0,
                            Optional ByVal p_Numero As Integer = 0,
                            Optional ByVal p_Subalterno As String = "0",
                            Optional ByVal Cod_Fiscale As String = "0",
                            Optional ByVal Fabbricato_Cod As Integer = 0,
                            Optional ByVal Prodotto_Cod As Integer = 0,
                            Optional ByVal Data_Lavorazione As String = "0",
                            Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                            Optional ByVal Analisi_Testata_Cod As Integer = 0,
                            Optional ByVal Analisi_Dettaglio_Cod As Integer = 0,
                            Optional ByVal Analisi_Campione_Cod As Integer = 0,
                            Optional ByVal PianoConcimazione_Testata_Cod As Integer = 0,
                            Optional ByVal Progetto_Cod As Integer = 0,
                            Optional ByVal Programmazione_Cod As Integer = 0,
                            Optional ByVal Programmazione_Entita_Cod As Integer = 0,
                            Optional ByVal id_agenda As Integer = 0,
                            Optional ByVal PivaPadre As String = "",
                            Optional ByVal Ricetta_Cod As Integer = 0,
                            Optional ByVal ricetta_Operazione_cod As Integer = 0
                    ) As String
        '------------------------------------------------------------------------
        Dim stb As New StringBuilder

        stb.Append(CStr(TipoNodo))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Piva))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Sa_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Campo_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Appezza))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Id_Imp))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Part_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Provincia_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Comune_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Sezione))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Foglio))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Numero))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(p_Subalterno))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Cod_Fiscale))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Fabbricato_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Prodotto_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Data_Lavorazione))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Analisi_Certificato_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Analisi_Testata_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Analisi_Dettaglio_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Analisi_Campione_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(PianoConcimazione_Testata_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Progetto_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Programmazione_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Programmazione_Entita_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(id_agenda))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(PivaPadre))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(Ricetta_Cod))
        stb.Append(SeparatoreAlberoJson)
        stb.Append(CStr(ricetta_Operazione_cod))



        'Restituisco il risultato
        Chiave = stb.ToString
        Return Chiave

    End Function








    '-----------------------SEGUONO QUELLI TOLTI DA AGRONICACOREUTILITY------------------



    ''' <summary>
    ''' decodifica tutta la chiave inizializzando tutte le variabili
    ''' </summary>
    ''' <remarks></remarks>
    'Public Shared Sub ChiaveAlbero_Decodifica(ByRef Chiave As String, _
    '                             ByRef Piva As String, _
    '                             ByRef Sa_Cod As Integer, _
    '                             ByRef Campo_Cod As Integer, _
    '                            ByRef Appezza As Integer, _
    '                            ByRef Id_Imp As Integer, _
    '                            ByRef p_Part_Cod As Integer, _
    '                            ByRef p_Provincia_Cod As String, _
    '                            ByRef p_Comune_Cod As String, _
    '                            ByRef p_Sezione As String, _
    '                            ByRef p_Foglio As Integer, _
    '                            ByRef p_Numero As Integer, _
    '                            ByRef p_Subalterno As String, _
    '                            ByRef Cod_Fiscale As String, _
    '                            ByRef Fabbricato_Cod As Integer, _
    '                            ByRef Prodotto_Cod As Integer, _
    '                            ByRef Data_Lavorazione As String, _
    '                            ByRef Analisi_Certificato_Cod As Integer, _
    '                            ByRef Analisi_Testata_Cod As Integer, _
    '                            ByRef Analisi_Dettaglio_Cod As Integer, _
    '                            ByRef Analisi_Campione_Cod As Integer, _
    '                            ByRef Piano_Cod As Integer, _
    '                            ByRef Vas_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    If Chiave.Split("\").Length > 30 Then
    '        ArrayKey = Split(Chiave, "\\")
    '    Else
    '        ArrayKey = Split(Chiave, "\")
    '    End If


    '    'Recupero l'elemento di indice 0 (TipoNodo)
    '    Piva = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Campo_Cod = CInt(ArrayKey(3))
    '    Appezza = CInt(ArrayKey(4))
    '    Id_Imp = CInt(ArrayKey(5))
    '    p_Part_Cod = CInt(ArrayKey(6))
    '    p_Provincia_Cod = CStr(ArrayKey(7))
    '    p_Comune_Cod = CStr(ArrayKey(8))
    '    p_Sezione = CStr(ArrayKey(9))
    '    p_Foglio = CInt(ArrayKey(10))
    '    p_Numero = CInt(ArrayKey(11))
    '    p_Subalterno = CStr(ArrayKey(12))
    '    Cod_Fiscale = CStr(ArrayKey(13))
    '    Fabbricato_Cod = CInt(ArrayKey(14))
    '    Prodotto_Cod = CInt(ArrayKey(15))
    '    Data_Lavorazione = CInt(ArrayKey(16))
    '    Analisi_Certificato_Cod = CInt(ArrayKey(17))
    '    Analisi_Testata_Cod = CInt(ArrayKey(18))
    '    Analisi_Dettaglio_Cod = CInt(ArrayKey(19))
    '    Analisi_Campione_Cod = CInt(ArrayKey(20))
    '    Piano_Cod = CInt(ArrayKey(21))
    '    Vas_Cod = CInt(ArrayKey(22))

    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_x_json(ByRef Chiave As String,
                                 ByRef Piva As String,
                                 ByRef Sa_Cod As Integer,
                                 ByRef Campo_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Id_Imp As Integer,
                                ByRef p_Part_Cod As Integer,
                                ByRef p_Provincia_Cod As String,
                                ByRef p_Comune_Cod As String,
                                ByRef p_Sezione As String,
                                ByRef p_Foglio As Integer,
                                ByRef p_Numero As Integer,
                                ByRef p_Subalterno As String,
                                ByRef Cod_Fiscale As String,
                                ByRef Fabbricato_Cod As Integer,
                                ByRef Prodotto_Cod As Integer,
                                ByRef Data_Lavorazione As String,
                                ByRef Analisi_Certificato_Cod As Integer,
                                ByRef Analisi_Testata_Cod As Integer,
                                ByRef Analisi_Dettaglio_Cod As Integer,
                                ByRef Analisi_Campione_Cod As Integer,
                                ByRef Piano_Cod As Integer,
                                ByRef Vas_Cod As Integer,
                                ByRef id_agenda As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave

        ArrayKey = Split(Chiave, SeparatoreAlberoJson)



        'Recupero l'elemento di indice 0 (TipoNodo)

        'in base al tipo nodo so l'ordine delle chiavi

        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Campo_Cod = CInt(ArrayKey(3))
        Appezza = CInt(ArrayKey(4))
        Id_Imp = CInt(ArrayKey(5))
        p_Part_Cod = CInt(ArrayKey(6))
        p_Provincia_Cod = CStr(ArrayKey(7))
        p_Comune_Cod = CStr(ArrayKey(8))
        p_Sezione = CStr(ArrayKey(9))
        p_Foglio = CInt(ArrayKey(10))
        p_Numero = CInt(ArrayKey(11))
        p_Subalterno = CStr(ArrayKey(12))
        Cod_Fiscale = CStr(ArrayKey(13))
        Fabbricato_Cod = CInt(ArrayKey(14))
        Prodotto_Cod = CInt(ArrayKey(15))
        Data_Lavorazione = CStr(ArrayKey(16))
        Analisi_Certificato_Cod = CInt(ArrayKey(17))
        Analisi_Testata_Cod = CInt(ArrayKey(18))
        Analisi_Dettaglio_Cod = CInt(ArrayKey(19))
        Analisi_Campione_Cod = CInt(ArrayKey(20))
        Piano_Cod = CInt(ArrayKey(21))
        Vas_Cod = CInt(ArrayKey(22)) 'Progetto_Cod
        id_agenda = CInt(ArrayKey(25))
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TipoNodo"></param>
    ''' <param name="IconaPersonalizzata"></param>
    ''' <returns></returns>
    Public Shared Function RitornaPathImg(ByVal TipoNodo As enum_TipoNodo,
                                   Optional ByVal IconaPersonalizzata As String = ""
                                   ) _
                               As String

        If IconaPersonalizzata <> "" Then
            Return IconaPersonalizzata

        Else

            Select Case TipoNodo
                Case enum_TipoNodo.Anagrafica_Generica
                    Return "/AB_Immagini/icone24/X24 - Campo.bmp"
                Case enum_TipoNodo.Utente
                    Return "/AB_Immagini/icone24/x01_Utente.png"

                Case enum_TipoNodo.Impresa
                    Return "/AB_Immagini/icone24/x02_Impresa.png"

                Case enum_TipoNodo.Centro
                    Return "/AB_Immagini/icone24/x03_Centro.png"

                Case enum_TipoNodo.Campo
                    Return "/AB_Immagini/icone24/CampoNew2.ico"

                Case enum_TipoNodo.Serra
                    Return "/AB_Immagini/icone24/anagrafica_24_serra.ico"

                Case enum_TipoNodo.Appezzamento
                    Return "/AB_Immagini/icone24/x05_Appezzamento.png"

                Case enum_TipoNodo.ImpiantoNudo
                    Return "/AB_Immagini/icone24/x06_TerrenoNudo.png"

                Case enum_TipoNodo.ImpiantoArborea
                    Return "/AB_Immagini/icone24/x07_Arboree.png"

                Case enum_TipoNodo.ImpiantoErbacea
                    Return "/AB_Immagini/icone24/x08_Erbacee.png"

                Case enum_TipoNodo.ImpiantoOrticola
                    Return "/AB_Immagini/icone24/ImpiantoOrticoleNew.ico"

                Case enum_TipoNodo.Particella
                    Return "/AB_Immagini/icone24/x12_Particella.ico"

                Case enum_TipoNodo.f_Abitazione
                    Return "/AB_Immagini/icone24/Casa_02.ico"

                Case enum_TipoNodo.f_Magazzino
                    Return "/AB_Immagini/icone24/Magazzino_02.ico"

                Case enum_TipoNodo.f_Silos
                    Return "/AB_Immagini/icone24/Silos_02.ico"

                Case enum_TipoNodo.f_CellaFrigorifera
                    Return "/AB_Immagini/icone24/CellaFrigorifera_02.ico"

                Case enum_TipoNodo.f_ImpiantoLavorazione
                    Return "/AB_Immagini/icone24/ImpiantiLavorazione_04.ico"

                Case enum_TipoNodo.f_Stalla
                    Return "/AB_Immagini/icone24/Stalla_01.ico"

                Case enum_TipoNodo.Persona
                    Return "/AB_Immagini/icone24/PersonaArancio.ico"

                Case enum_TipoNodo.CatastoAziendale
                    Return "/AB_Immagini/icone24/Catasto_02.ico"

                Case enum_TipoNodo.p_PortafoglioProdotti
                    Return "/AB_Immagini/icone24/PortafoglioProdotti.ico"

                Case enum_TipoNodo.p_Prodotto
                    Return "/AB_Immagini/icone24/Prodotto_01.ico"

                Case enum_TipoNodo.p_Preparazione
                    Return "/AB_Immagini/icone24/Prodotto_02.ico"

                Case enum_TipoNodo.x_PreparazioniAlimentari
                    Return "/AB_Immagini/icone24/Prodotto_02.ico"

                Case enum_TipoNodo.x_MovimentiMagazzino
                    Return "/AB_Immagini/icone24/MovimentiMagazzino01.ico"

                Case enum_TipoNodo.x_GiacenzeMagazzino
                    Return "/AB_Immagini/icone24/MagazzinoGiacenze24.ico"

                Case enum_TipoNodo.x_ConsistenzeAnimali
                    Return "/AB_Immagini/icone24/Consistenze01.ico"

                Case enum_TipoNodo.x_VariazioniConsistenzeAnimali
                    Return "/AB_Immagini/icone24/MovimentiMagazzino01.ico"

                Case enum_TipoNodo.f_Fienile
                    Return "/AB_Immagini/icone24/Fienile.ico"

                Case enum_TipoNodo.Fabbricato_Generico
                    Return "/AB_Immagini/icone24/FabbricatoGenerico.ico"

                Case enum_TipoNodo.x_ParcoMacchine
                    Return "/AB_Immagini/icone24/ParcoMacchine24.ico"

                Case enum_TipoNodo.x_Contatti
                    Return "/AB_Immagini/icone24/Contatti24.ico"

                Case enum_TipoNodo.x_ListaFabbricatiAziendali
                    Return "/AB_Immagini/icone24/FabbricatoGenerico.ico"

                Case enum_TipoNodo.x_Cooperativa
                    Return "/AB_Immagini/icone24/Cooperativa24c.ico"

                Case enum_TipoNodo.x_Consorzio
                    Return "/AB_Immagini/icone24/Cooperativa24a.ico"

                Case enum_TipoNodo.x_OP
                    Return "/AB_Immagini/icone24/Cooperativa24b.ico"


                Case enum_TipoNodo.Analisi_Certificato
                    Return "/AB_Immagini/icone24/certificato24.ico"

                Case enum_TipoNodo.Analisi_Testata
                    Return "/AB_Immagini/icone24/ingredienti.ico"

                Case enum_TipoNodo.Analisi_Dettaglio
                    Return "/AB_Immagini/icone24/dose24.ico"

                Case enum_TipoNodo.Analisi_Campione
                    Return "/AB_Immagini/icone24/dose24.ico"

                Case enum_TipoNodo.PianoConcimazione_Testata
                    Return "/AB_Immagini/icone24/PianoConcimazione_24.ico"

                Case enum_TipoNodo.PlanningTestata
                    Return "/AB_Immagini/icone24/Planning.png"

                Case enum_TipoNodo.PlanningEntita
                    Return "/AB_Immagini/icone24/Planning.png"


                Case enum_TipoNodo.ricette_Testata
                    Return "/AB_Immagini/icone24/doc4.ico"

                Case enum_TipoNodo.ricette_dettaglio
                    Return "/AB_Immagini/icone24/Agenda24.ico"


                Case enum_TipoNodo.Agenda
                    Return "/AB_Immagini/icone24/Agenda24.png"

                Case enum_TipoNodo.DistintaDiProduzione
                    Return "/AB_Immagini/Icone24/Ope_Colturali_24.ico"
            End Select
        End If
        Return ""

    End Function

    Public Shared Sub Selezionatore_Icone(ByRef IconaPreferita As String,
                                ByVal TipoNodo As enum_TipoNodo,
                                Optional ByVal Veg_Cod As Integer = 0,
                                Optional ByVal DestinazioneUso As Integer = 0,
                                          Optional ByVal ElencoIcone As String = "")


        Dim Elenco_Icone_SpecieVegetali As String
        Dim Vettore_Veg_Cod As String()
        Dim Str_Veg_cod As String
        Dim i As Integer

        Select Case TipoNodo

            Case enum_TipoNodo.Impianto_Generico
                IconaPreferita = "/AB_Immagini/IconeVegetali/9999999.ico"

            Case enum_TipoNodo.ImpiantoArborea,
                 enum_TipoNodo.ImpiantoErbacea,
                 enum_TipoNodo.ImpiantoOrticola

                'Verifico se la specie vegetale ha una sua icona

                'Recupero l'elenco delle icone disponibili
                Dim ElencoIconeSession As String = Web.HttpContext.Current.Session("Elenco_Icone_SpecieVegetali")
                If String.IsNullOrEmpty(ElencoIconeSession) Then
                    ElencoIconeSession = ElencoIcone
                End If
                Elenco_Icone_SpecieVegetali = ElencoIconeSession

                'Scompongo la stringa
                Vettore_Veg_Cod = Split(Elenco_Icone_SpecieVegetali, ":")

                'Preparo la stringa obiettivo
                Str_Veg_cod = Veg_Cod.ToString

                'Cerco la specie vegetale
                For i = 0 To UBound(Vettore_Veg_Cod)
                    If Vettore_Veg_Cod(i) = Str_Veg_cod Then
                        'Formatto il Veg_Cod
                        Str_Veg_cod = Right("0000000" & Str_Veg_cod, 7)

                        'Preparo il nome dell'icona
                        IconaPreferita = "/AB_Immagini/IconeVegetali/" & Str_Veg_cod & ".ico"
                        Exit Sub
                    End If
                Next i
                IconaPreferita = "/AB_Immagini/IconeVegetali/9999999.ico"

            Case enum_TipoNodo.ImpiantoNudo
                IconaPreferita = "/AB_Immagini/icone24/x06_TerrenoNudo.png"

        End Select
    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objServer"></param>
    ''' <param name="objSession"></param>
    ''' <param name="objPage"></param>
    ''' <param name="Tipo_Fabbricato_Cod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function TipoNodoxFabbricato(ByRef objServer As Web.HttpServerUtility,
                                   ByRef objSession As Web.SessionState.HttpSessionState,
                                   ByRef objPage As Web.UI.Page,
                                   ByVal Tipo_Fabbricato_Cod As Integer) As enum_TipoNodo

        Dim TipoNodo As enum_TipoNodo

        Select Case Tipo_Fabbricato_Cod

            Case 10
                TipoNodo = enum_TipoNodo.f_Abitazione
            Case 20, 50, 120, 121, 122, 123
                TipoNodo = enum_TipoNodo.f_Magazzino
            Case 30, 130, 131, 132, 133, 134, 135
                TipoNodo = enum_TipoNodo.f_Silos
            Case 40, 41, 42, 140, 141
                TipoNodo = enum_TipoNodo.f_CellaFrigorifera
            Case 60, 61, 62, 63, 160
                TipoNodo = enum_TipoNodo.f_ImpiantoLavorazione
            Case 70, 170, 171, 172, 173, 174, 175, 176
                TipoNodo = enum_TipoNodo.f_Stalla
            Case 180, 181, 182
                TipoNodo = enum_TipoNodo.f_Fienile
            Case 1000
                TipoNodo = enum_TipoNodo.Fabbricato_Generico

        End Select

        Return TipoNodo

    End Function



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Chiave"></param>
    ''' <param name="PartitaIVA"></param>
    ''' <remarks></remarks>
    'Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA(ByVal Chiave As String, _
    '                           ByRef PartitaIVA As String)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero l'elemento di indice 0 (TipoNodo)
    '    PartitaIVA = CStr(ArrayKey(1))

    'End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_x_json(ByVal Chiave As String,
                             ByRef PartitaIVA As String)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero l'elemento di indice 0 (TipoNodo)
        PartitaIVA = CStr(ArrayKey(1))

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Chiave"></param>
    ''' <param name="PartitaIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <remarks></remarks>
    'Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod(ByVal Chiave As String, _
    '                           ByRef PartitaIVA As String, _
    '                           ByRef Sa_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))


    'End Sub


    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(ByVal Chiave As String,
                               ByRef PartitaIVA As String,
                               ByRef Sa_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))


    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    'Public Shared Sub ChiaveAlbero_Decodifica_PlanningEntita(ByVal Chiave As String, _
    '                           ByRef PartitaIVA As String, _
    '                           ByRef Sa_Cod As Integer, _
    '                           ByRef Programmazione_Cod As Integer, _
    '                           ByRef Programmazione_Entita_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Programmazione_Cod = CInt(ArrayKey(23))
    '    Programmazione_Entita_Cod = CInt(ArrayKey(24))
    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_PlanningEntita_x_json(ByVal Chiave As String,
                             ByRef PartitaIVA As String,
                             ByRef Sa_Cod As Integer,
                             ByRef Programmazione_Cod As Integer,
                             ByRef Programmazione_Entita_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Programmazione_Cod = CInt(ArrayKey(23))
        Programmazione_Entita_Cod = CInt(ArrayKey(24))
    End Sub


    'Public Shared Sub ChiaveAlbero_Decodifica_PlanningTestata( _
    '                           ByVal Chiave As String, _
    '                           ByRef PivaPadre As String, _
    '                           ByRef PartitaIVA As String, _
    '                           ByRef Sa_Cod As Integer, _
    '                           ByRef Programmazione_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Programmazione_Cod = CInt(ArrayKey(23))
    '    PivaPadre = ArrayKey(AgronicaCoreDataProvider.TipiEnumerativi.enum_PosizioniNodo.PivaPadre)

    'End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_PlanningTestata_x_json(
                               ByVal Chiave As String,
                               ByRef PivaPadre As String,
                               ByRef PartitaIVA As String,
                               ByRef Sa_Cod As Integer,
                               ByRef Programmazione_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Programmazione_Cod = CInt(ArrayKey(23))
        PivaPadre = ArrayKey(enum_PosizioniNodo.PivaPadre)

    End Sub



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Chiave"></param>
    ''' <param name="PartitaIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <remarks></remarks>
    'Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod(ByVal Chiave As String, _
    '                           ByRef PartitaIVA As String, _
    '                           ByRef Sa_Cod As Integer, ByRef Campo_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Campo_Cod = CInt(ArrayKey(3))

    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(ByVal Chiave As String,
                              ByRef PartitaIVA As String,
                              ByRef Sa_Cod As Integer, ByRef Campo_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Campo_Cod = CInt(ArrayKey(3))

    End Sub

    Public Shared Sub ChiaveAlbero_Decodifica_Sementiero_x_json(ByVal Chiave As String,
                                 ByRef Sementiero As String
                                 )
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        Sementiero = CStr(ArrayKey(26))

    End Sub

    'Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza(ByVal Chiave As String, _
    '                           ByRef PartitaIVA As String, _
    '                           ByRef Sa_Cod As Integer, ByRef Appezza As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Appezza = CInt(ArrayKey(4))

    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_x_json(ByVal Chiave As String,
                             ByRef PartitaIVA As String,
                             ByRef Sa_Cod As Integer, ByRef Appezza As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Appezza = CInt(ArrayKey(4))

    End Sub


    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campo_Appezza_x_json(ByVal Chiave As String,
                                                                                      ByRef PartitaIVA As String,
                                                                                      ByRef Sa_Cod As Integer, ByRef Campo_Cod As Integer, ByRef Appezza As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Campo_Cod = CInt(ArrayKey(3))
        Appezza = CInt(ArrayKey(4))

    End Sub


    'Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_Impianto( _
    '                        ByVal Chiave As String, _
    '                        ByRef PartitaIVA As String, _
    '                        ByRef Sa_Cod As Integer, _
    '                        ByRef Appezza As Integer, _
    '                        ByRef reg_impianto As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Appezza = CInt(ArrayKey(4))
    '    reg_impianto = CInt(ArrayKey(5))

    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_Impianto_x_json(
                           ByVal Chiave As String,
                           ByRef PartitaIVA As String,
                           ByRef Sa_Cod As Integer,
                           ByRef Appezza As Integer,
                           ByRef reg_impianto As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Appezza = CInt(ArrayKey(4))
        reg_impianto = CInt(ArrayKey(5))

    End Sub


    'Public Shared Sub ChiaveAlbero_Decodifica_Ricetta_cod( _
    '                        ByVal Chiave As String, _
    '                        ByRef PartitaIVA As String, _
    '                        ByRef Sa_Cod As Integer, _
    '                        ByRef Appezza As Integer, _
    '                        ByRef reg_impianto As Integer, _
    '                        ByRef Ricetta_cod As Integer)
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")


    '    'Recupero gli elementi
    '    PartitaIVA = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Appezza = CInt(ArrayKey(4))
    '    reg_impianto = CInt(ArrayKey(5))
    '    Ricetta_cod = CInt(ArrayKey(27))
    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_Ricetta_cod_x_json(
                            ByVal Chiave As String,
                            ByRef PartitaIVA As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef reg_impianto As Integer,
                            ByRef Ricetta_cod As Integer)
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Appezza = CInt(ArrayKey(4))
        reg_impianto = CInt(ArrayKey(5))
        Ricetta_cod = CInt(ArrayKey(27))
    End Sub


    Public Shared Sub ChiaveAlbero_Decodifica_Ricetta_Operazione_cod_x_json(
                            ByVal Chiave As String,
                            ByRef PartitaIVA As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef reg_impianto As Integer,
                            ByRef Ricetta_cod As Integer)
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Appezza = CInt(ArrayKey(4))
        reg_impianto = CInt(ArrayKey(5))
        Ricetta_cod = CInt(ArrayKey(28))
    End Sub



    Public Shared Sub ChiaveAlbero_Decodifica_Agenda__x_json(
                            ByVal Chiave As String,
                            ByRef PartitaIVA As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef reg_impianto As Integer,
                            ByRef id_agenda As Integer)
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Appezza = CInt(ArrayKey(4))
        reg_impianto = CInt(ArrayKey(5))
        id_agenda = CInt(ArrayKey(25))
    End Sub


    ''' <summary>
    ''' restituisce il tipo di nodo del fabbricato
    ''' </summary>
    ''' <param name="Tipo_Fabbricato_Cod"></param>
    ''' <returns></returns>
    Public Shared Function TipoNodoxFabbricato(ByVal Tipo_Fabbricato_Cod As Integer) As enum_TipoNodo

        Dim TipoNodo As enum_TipoNodo

        Select Case Tipo_Fabbricato_Cod

            Case 10
                TipoNodo = enum_TipoNodo.f_Abitazione
            Case 20, 50, 120, 121, 122, 123
                TipoNodo = enum_TipoNodo.f_Magazzino
            Case 30, 130, 131, 132, 133, 134, 135
                TipoNodo = enum_TipoNodo.f_Silos
            Case 40, 41, 42, 140, 141
                TipoNodo = enum_TipoNodo.f_CellaFrigorifera
            Case 60, 61, 62, 63, 160
                TipoNodo = enum_TipoNodo.f_ImpiantoLavorazione
            Case 70, 170, 171, 172, 173, 174, 175, 176
                TipoNodo = enum_TipoNodo.f_Stalla
            Case 180, 181, 182
                TipoNodo = enum_TipoNodo.f_Fienile
            Case 1000
                TipoNodo = enum_TipoNodo.Fabbricato_Generico

        End Select

        Return TipoNodo

    End Function


    'Public Shared Sub ChiaveAlbero_Decodifica_Analisi(ByVal Chiave As String, _
    '                            ByRef TipoNodo As enum_TipoNodo, _
    '                            ByRef Piva As String, _
    '                            ByRef Sa_Cod As Integer, _
    '                            ByRef Analisi_Certificato_Cod As Integer, _
    '                            ByRef Analisi_Testata_Cod As Integer, _
    '                            ByRef Analisi_Dettaglio_Cod As Integer, _
    '                            ByRef Analisi_Campione_Cod As Integer)
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    TipoNodo = CInt(ArrayKey(0))
    '    Piva = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Analisi_Certificato_Cod = CInt(ArrayKey(17))
    '    Analisi_Testata_Cod = CInt(ArrayKey(18))
    '    Analisi_Dettaglio_Cod = CInt(ArrayKey(19))
    '    Analisi_Campione_Cod = CStr(ArrayKey(20))

    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_Analisi_x_json(ByVal Chiave As String,
                               ByRef TipoNodo As enum_TipoNodo,
                               ByRef Piva As String,
                               ByRef Sa_Cod As Integer,
                               ByRef Analisi_Certificato_Cod As Integer,
                               ByRef Analisi_Testata_Cod As Integer,
                               ByRef Analisi_Dettaglio_Cod As Integer,
                               ByRef Analisi_Campione_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        TipoNodo = CInt(ArrayKey(0))
        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Analisi_Certificato_Cod = CInt(ArrayKey(17))
        Analisi_Testata_Cod = CInt(ArrayKey(18))
        Analisi_Dettaglio_Cod = CInt(ArrayKey(19))
        Analisi_Campione_Cod = CInt(ArrayKey(20))

    End Sub

    'Public Shared Sub ChiaveAlbero_Decodifica_Cantine( _
    '                            ByVal Chiave As String, _
    '                            ByRef TipoNodo As enum_TipoNodo, _
    '                            ByRef Piva As String, _
    '                            ByRef Sa_Cod As Integer, _
    '                            ByRef Piano_Cod As Integer, _
    '                            ByRef Vas_Cod As Integer _
    '                            )
    '    '------------------------------------------------------------------------
    '    Dim ArrayKey() As String

    '    'Spezzo la chiave
    '    ArrayKey = Split(Chiave, "\")

    '    'Recupero gli elementi
    '    TipoNodo = CInt(ArrayKey(0))
    '    Piva = CStr(ArrayKey(1))
    '    Sa_Cod = CInt(ArrayKey(2))
    '    Piano_Cod = CStr(ArrayKey(21))
    '    Vas_Cod = CStr(ArrayKey(22))
    'End Sub
    Public Shared Sub ChiaveAlbero_Decodifica_Cantine_x_json(
                               ByVal Chiave As String,
                               ByRef TipoNodo As enum_TipoNodo,
                               ByRef Piva As String,
                               ByRef Sa_Cod As Integer,
                               ByRef Piano_Cod As Integer,
                               ByRef Vas_Cod As Integer
                               )
        '------------------------------------------------------------------------
        Dim ArrayKey As String()

        'Spezzo la chiave
        ArrayKey = Split(Chiave, SeparatoreAlberoJson)

        'Recupero gli elementi
        TipoNodo = CInt(ArrayKey(0))
        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Piano_Cod = CInt(ArrayKey(21))
        Vas_Cod = CInt(ArrayKey(22))
    End Sub

End Class
