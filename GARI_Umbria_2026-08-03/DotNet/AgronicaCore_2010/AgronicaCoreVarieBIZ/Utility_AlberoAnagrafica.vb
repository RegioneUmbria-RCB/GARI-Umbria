Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Utility_AlberoAnagrafica

    Public Shared Function PreparaDatiNodoAppezzamento(
        ByVal DR_Appezzamenti As DataRow,
        ByVal VisualizzaRiferimentoAlfanumericoImpianto As Boolean
        ) As DatiNodoAppezzamento

        Dim datiNodoAppezzamento As New DatiNodoAppezzamento

        datiNodoAppezzamento.xAppezza = DR_Appezzamenti.Item("Appezza")

        datiNodoAppezzamento.xAppNome = DR_Appezzamenti.Item("App_Nome")

        If VisualizzaRiferimentoAlfanumericoImpianto Then

            Dim RiferimentoAlfanumerico As String = DR_Appezzamenti.Item("RiferimentoAlfanumerico")

            If RiferimentoAlfanumerico <> "" Then

                datiNodoAppezzamento.xAppNome = RiferimentoAlfanumerico & " - " & datiNodoAppezzamento.xAppNome

            End If

        End If

        datiNodoAppezzamento.ValidazioneNodo = DR_Appezzamenti.Item("Validazione")

        datiNodoAppezzamento.xValidita_Inizio = DR_Appezzamenti.Item("Validita_Inizio")

        datiNodoAppezzamento.xValidita_Fine = DR_Appezzamenti.Item("Validita_Fine")

        datiNodoAppezzamento.style = If(DR_Appezzamenti.Item("Blk_Flag") = -1, "red", If(datiNodoAppezzamento.xValidita_Fine < Date.Now.Date, "DimGray", ""))

        'Composizione descrizione

        datiNodoAppezzamento.DescrizioneNodo = ""

        datiNodoAppezzamento.DescrizioneNodo &= "{" & My.Resources.AgronicaCoreVarieBIZ.App & "} : "

        datiNodoAppezzamento.DescrizioneNodo &= datiNodoAppezzamento.xAppNome

        datiNodoAppezzamento.DescrizioneNodo &= " : {" & CDbl(DR_Appezzamenti.Item("Sup_App")).ToString("0.####") & " ha}"

        'Verifico lo stato di validazione

        If datiNodoAppezzamento.ValidazioneNodo = "-1" Then
            datiNodoAppezzamento.DescrizioneNodo = datiNodoAppezzamento.DescrizioneNodo & " ..... (§§§ da confermare §§§)"
        End If

        Return datiNodoAppezzamento

    End Function

    Public Shared Function PreparaDatiNodoImpianto(
        ByVal DR_Impianti As DataRow,
        ByVal xTipoNodoChiaveParametro As Integer
        ) As DatiNodoImpianto

        Const CulCod_TerrenoNudo = 0

        Dim datiNodoImpianto As New DatiNodoImpianto

        Dim stbDescrizioneNodo As New StringBuilder

        stbDescrizioneNodo.Length = 0

        datiNodoImpianto.xID_Imp = DR_Impianti.Item("Id_Reg")

        datiNodoImpianto.xCul_Cod = If(IsDBNull(DR_Impianti.Item("Cul_Cod")), 0, DR_Impianti.Item("Cul_Cod"))

        datiNodoImpianto.xGru_Cod = If(IsDBNull(DR_Impianti.Item("Gru_Cod")), 0, DR_Impianti.Item("Gru_Cod"))

        datiNodoImpianto.xVeg_Cod = If(IsDBNull(DR_Impianti.Item("Veg_Cod")), 0, DR_Impianti.Item("Veg_Cod"))

        datiNodoImpianto.xDestinazioneUso_Cod = IIf(IsDBNull(DR_Impianti.Item("DestinazioneUsoCodice")), 0, DR_Impianti.Item("DestinazioneUsoCodice"))

        datiNodoImpianto.xValidita_Inizio = CDate(DR_Impianti.Item("Validita_Inizio"))

        datiNodoImpianto.xValidita_Fine = CDate(DR_Impianti.Item("Validita_Fine"))

        datiNodoImpianto.ValidazioneNodo = DR_Impianti.Item("Validazione")

        datiNodoImpianto.Id_Consociazione = CInt(DR_Impianti.Item("Id_Consociazione"))

        datiNodoImpianto.style = If(DR_Impianti.Item("Blk_Flag") = -1, "red", If(datiNodoImpianto.xValidita_Fine < Date.Now.Date, "DimGray", If(datiNodoImpianto.Id_Consociazione <> 0, "blue", "")))

        'Composizione descrizione

        stbDescrizioneNodo.Append(AgroPrefix_Impianto)

        If datiNodoImpianto.xCul_Cod = CulCod_TerrenoNudo Then

            If xTipoNodoChiaveParametro = 0 Then
                datiNodoImpianto.xTipoNodoChiave = enum_TipoNodo.ImpiantoNudo
            Else
                datiNodoImpianto.xTipoNodoChiave = xTipoNodoChiaveParametro
            End If
            datiNodoImpianto.TipoNodo = enum_TipoNodo.ImpiantoNudo

            'Controllo se è laghetto, boschetto o altro

            If Not IsDBNull(DR_Impianti.Item("DestinazioneUso")) Then
                stbDescrizioneNodo.Append(datiNodoImpianto.xValidita_Inizio.ToShortDateString)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(DR_Impianti.Item("DestinazioneUso"))
            Else
                stbDescrizioneNodo.Append(datiNodoImpianto.xValidita_Inizio.ToShortDateString)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append("Terreno Nudo")
            End If

        Else

            'Verifico il gruppo vegetale per scegliere l'icona

            Select Case datiNodoImpianto.xGru_Cod
                Case 1
                    datiNodoImpianto.TipoNodo = enum_TipoNodo.ImpiantoArborea
                Case 2
                    datiNodoImpianto.TipoNodo = enum_TipoNodo.ImpiantoErbacea
                Case 3
                    datiNodoImpianto.TipoNodo = enum_TipoNodo.ImpiantoOrticola
            End Select

            'Imposto anche il tipo nodo chiave altrimenti se si imposta il tipo dell'impianto precedente e
            'se è diverso viene impostata una chiave albero erronea

            If xTipoNodoChiaveParametro = 0 Then
                Select Case datiNodoImpianto.xGru_Cod
                    Case 1
                        datiNodoImpianto.xTipoNodoChiave = enum_TipoNodo.ImpiantoArborea
                    Case 2
                        datiNodoImpianto.xTipoNodoChiave = enum_TipoNodo.ImpiantoErbacea
                    Case 3
                        datiNodoImpianto.xTipoNodoChiave = enum_TipoNodo.ImpiantoOrticola
                End Select
            Else
                datiNodoImpianto.xTipoNodoChiave = xTipoNodoChiaveParametro
            End If

            'Descrizione del nodo

            stbDescrizioneNodo.Append(datiNodoImpianto.xValidita_Inizio.ToShortDateString)
            stbDescrizioneNodo.Append(" - ")
            stbDescrizioneNodo.Append(DR_Impianti.Item("Veg_Des"))
            stbDescrizioneNodo.Append(" - ")
            stbDescrizioneNodo.Append(DR_Impianti.Item("Cul_Des"))

        End If

        'Impianto consociato
        If datiNodoImpianto.Id_Consociazione <> 0 Then
            stbDescrizioneNodo.Append(" (c)")
        End If

        'Informazioni sulla superficie

        stbDescrizioneNodo.Append(" : {" & CDbl(DR_Impianti.Item("Sup_Imp")).ToString("0.####") & " ha}")

        'Verifico lo stato di validazione

        If datiNodoImpianto.ValidazioneNodo = "-1" Then
            stbDescrizioneNodo.Append(" ..... (§§§ da confermare §§§)")
        End If

        datiNodoImpianto.DescrizioneNodo = stbDescrizioneNodo.ToString()

        Return datiNodoImpianto

    End Function

End Class

Public Class DatiNodoAppezzamento

    Public xAppezza As String
    Public xAppNome As String
    Public ValidazioneNodo As String
    Public DescrizioneNodo As String
    Public xValidita_Inizio As String
    Public xValidita_Fine As String
    Public style As String

End Class

Public Class DatiNodoImpianto

    Public xID_Imp As Integer
    Public TipoNodo As Integer
    Public xTipoNodoChiave As Integer
    Public xCul_Cod As String
    Public xGru_Cod As String
    Public xVeg_Cod As String
    Public xDestinazioneUso_Cod As String
    Public Id_Consociazione As Integer
    Public ValidazioneNodo As String
    Public DescrizioneNodo As String
    Public xValidita_Inizio As DateTime
    Public xValidita_Fine As DateTime
    Public style As String

End Class
