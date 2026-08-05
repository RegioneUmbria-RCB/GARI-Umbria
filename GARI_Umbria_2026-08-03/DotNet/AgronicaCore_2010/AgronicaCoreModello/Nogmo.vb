Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Nogmo

    Public Class DatiNogmo
        Inherits Object
        Public Nation As String 'IT FR or IE
        Public ExporterID As Integer 'codice esportatore
        Public BovineList As List(Of Animale) '
    End Class
    Public Class Animale
        <JsonIgnore>
        Public Piva As String
        <JsonIgnore>
        Public STA_NUM As Integer
        <JsonIgnore>
        Public Cod_Progetto As Integer
        <JsonIgnore>
        Public BDN_Codice_Azienda As String
        <JsonIgnore>
        Public chiave As String
        <JsonIgnore>
        Public Sa_Cod As Integer
        <JsonIgnore>
        Public Errori As String = ""

        Public Identification As String 'identificato Animale
        Public BreedingNumber As String 'Codice Stalla Origine
        Public Sex As String 'Sesso
        Public Price As Integer
        Public DateBirth As String 'Data di nascita 
        Public Race As String 'Razza 
        Public FatherRace As String 'Razza padre
        Public MotherRace As String 'Razza madre
        Public Weight As Integer 'Peso
        Public Exporter As String 'Esportatore o fornitore
        <JsonProperty("Operator")>
        Public Operator2 As String 'Operatore
        Public DateTrans As String ' data transito animale
        Public DateLoad As String 'NON INDICATO NELLA DOCUMENTAZIONE
        Public DateStart As String 'Data partenza da stalla di origine
        Public DateArrival As String 'Data Arrivo in stalla ingrasso
        Public FoodProvider As String 'Fornitore Alimenti
        Public AllotmentCentre As String 'Centro di raccolta 
        Public N_certif_sanit_export As String 'Numero Certificato sanitario
        Public FrCustomer As String 'Nome allevatore di oStringrigine
        Public FrAddress As String 'Indirizzo stalla di origine
        Public FrTelephone As String  'Telefono stalla di origine
        Public FrMobile As String 'Cellulare allevatore di origine
        Public FrPostalCode As String 'Cap stalla di origine
        Public FrCom As String 'Comune stalla di origine
        Public ItStable As String 'codice stalla di ingrasso
        Public ItCustomer As String 'Allevamento di ingrasso
        Public ItPostalCode As String 'Cap stalla di ingrasso
        Public ApproachConcerns As String 'filiera di pertinenza
        Public N_chep_elev_trans As String 'codice stalla di svezzamento
        Public Coord_elev_trans As String 'indirizzo stalla di svezzamento
        Public CP_elev_trans As String 'Cap stalla di svezzamento
        Public DateMiserepous As String 'Data di svezzamento
        Public Weaning_Code As String 'Codice stalla di svezzamento
        Public Weaning_Stable_Breeder_Name As String 'Nome allevatore svezzamento
        Public Weaning_Stable_Place As String 'Comeune stall di svezzamento
        Public Weaning_Stable_Phone As String 'Telefono stall di svezzamento
        Public Sub New(infoAnimale As JToken)
            Identification = infoAnimale("Matricola")
            BreedingNumber = infoAnimale("Stalla")


            'NON nogmo data
            Cod_Progetto = infoAnimale("Cod_Progetto")
            STA_NUM = infoAnimale("STA_NUM")
            chiave = infoAnimale("chiave")
            Piva = infoAnimale("Piva")
            Sa_Cod = infoAnimale("Sa_Cod")
            BDN_Codice_Azienda = infoAnimale("Stalla")
        End Sub



        Public Sub aggiungiDatiAnimaleDaExpando(dati As IDictionary(Of String, Object), razze As List(Of IDictionary(Of String, Object)))
            If dati Is Nothing Then
                Return
            End If

            Sex = dati("Sesso").ToString
            Price = 0
            DateBirth = convertDate(dati("DAT_NASCITA").ToString)

            MotherRace = trovaRazzaCod(dati("GEN_COD"), dati("SPE_COD"), dati("Razza_Madre"), razze)
            FatherRace = trovaRazzaCod(dati("GEN_COD"), dati("SPE_COD"), dati("Razza_Padre"), razze)
            Race = trovaRazzaCod(dati("GEN_COD"), dati("SPE_COD"), dati("RAZ_COD"), razze)
            If IsNothing(Race) Then
                Race = MotherRace
            End If

            Weight = 300 'dati("PESO")
            N_certif_sanit_export = dati("Certificato").ToString
            N_chep_elev_trans = dati("AUSL_AZI_NASCITA").Replace("FR", "")

            DateArrival = convertDate(dati("Validita_Inizio"))
            DateStart = convertDate(DateTime.Parse(dati("Validita_Inizio")).AddDays(-1))
            DateMiserepous = convertDate(DateTime.Parse(DateStart).AddDays(-1)) 'DateMiserepous come DateStart -1 giorno
            DateTrans = DateStart

            Weaning_Stable_Breeder_Name = dati("StallaSvezz_RagSoc")
            Weaning_Stable_Phone = dati("StallaSvezz_Telefono")
            Weaning_Stable_Place = dati("StallaSvezz_Comune")
            Weaning_Code = dati("StallaSvezz_Codice")

            Dim k = 1
            If k = 0 Then 'Serve a far fallire l'invio del capo per test
                DateMiserepous = "21/02/23"
                DateLoad = "21/02/23"
                DateArrival = "21/02/23"
                DateStart = "21/02/23"
                DateTrans = "21/02/23"
            End If

        End Sub
        Public Sub aggiungiDatiStallaDaExpando(stallaAttuale As IDictionary(Of String, Object), stallaDiNascita As IDictionary(Of String, Object), azi_nascita As String)

            ItStable = BreedingNumber

            If stallaAttuale IsNot Nothing Then
                ItPostalCode = stallaAttuale("CAP")

            End If

            FrTelephone = ""
            FrMobile = ""
            FrPostalCode = ""

            CP_elev_trans = ""
            FrCustomer = ""
            FrCom = ""
            ItCustomer = stallaAttuale("STA_DES")




            'If 1 = 0 AndAlso stallaDiNascita IsNot Nothing Then
            'If stallaDiNascita IsNot Nothing Then
            '    N_chep_elev_trans = stallaDiNascita("BDN_Codice_Azienda").ToString
            '    FrAddress = stallaDiNascita("ind_des")
            '    Coord_elev_trans = stallaDiNascita("ind_des")
            '    CP_elev_trans = stallaDiNascita("CAP")
            '    Weaning_Stable_Breeder_Name = ""
            '    Weaning_Stable_Place = stallaDiNascita("com_des")
            '    Weaning_Stable_Phone = ""
            'Else
            '    Coord_elev_trans = ""
            '    N_chep_elev_trans = ""
            '    Weaning_Stable_Breeder_Name = ""
            '    Weaning_Stable_Phone = ""
            '    Weaning_Stable_Place = ""
            'End If

            FrAddress = ""
            Coord_elev_trans = ""
            FrAddress = ""

        End Sub

        Public Sub aggiungiDatiGenerici(operatore As String, _exporter As String, _AllotmentCentre As String, _ApproachConcerns As String, _FoodProvider As String)
            Operator2 = operatore
            Exporter = _exporter
            AllotmentCentre = _AllotmentCentre
            ApproachConcerns = _ApproachConcerns
            FoodProvider = _FoodProvider
        End Sub
        Private Function trovaRazzaCod(gen_cod As String, spe_cod As String, raz_cod As String, razze As List(Of IDictionary(Of String, Object))) As String
            Return razze.Where(Function(razza)
                                   Return razza("GEN_COD") = gen_cod AndAlso razza("RAZ_COD") = raz_cod AndAlso razza("SPE_COD") = spe_cod
                               End Function).Select(Of String)(Function(razza)
                                                                   Return razza("CODICE")
                                                               End Function).FirstOrDefault
        End Function

        Private Function convertDate(sDate As String) As String
            If String.IsNullOrEmpty(sDate) Then
                Return Nothing
            End If

            Return DateTime.Parse(sDate).ToString("u").Replace(" ", "T")
        End Function

    End Class


    Class CheckBovine
        Public Identification As List(Of String) 'Lista identificativi animali
        Public DateLoad As String = ""
        Public BreedingNumber As String = "" 'Stalla
    End Class


End Class
