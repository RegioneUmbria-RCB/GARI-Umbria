Public Class DistribuzioneFrazionata_FertiManager

    Private Class NPKValues
        Public data As DateTime
        Public d As NPK
        Public sd As NPK
        Public inf As NPK
        Public sup As NPK
        Public Sub New(ByVal _data_ As DateTime)
            data = _data_
            d = New NPK
            sd = New NPK
            inf = New NPK
            sup = New NPK
        End Sub
    End Class
    Public Function DistribFrazFertiManager(ByVal input As FertiManager_input) As FertiAdvice

        'Ordinare per il campo Data la lista input.fertiSommiList
        input.fertiSommiList.Sort(Function(x As FertiSommiG, y As FertiSommiG)
                                      If x.Data < y.Data Then
                                          Return -1
                                      ElseIf x.Data > y.Data Then
                                          Return 1
                                      End If
                                      Return 0
                                  End Function)

        Dim GDDff_start As Decimal = input.Tsum(input.dataFaseStart.DayOfYear - 1)

        Dim ff_idx As Integer = 1

        'DOMANDA: La faseStart è sempre la seconda nella lista delle fenofasi? La prima la devo ignorare?

        'DOMANDA: il valore SommaT in fertiParsPhenophase fa riferimento alla somma termica che occorre per far scattare
        ' la fenofase seguente oppure è la somma termica che occorre per far scattare la fenonfase stessa?
        ' esempio:
        ' F1    pre-emergenza       140                     F1  pre-emergenza       0
        ' F2    semina/trapianto    410                     F2  semina/trapianto    140 
        ' F3    ...                 250                     F3  ...                 410
        ' F4    ...                 275                     F4  ...                 250
        ' F5    ...                 484                     F5  ...                 275
        ' F6    ...                 400                     F6  ...                 484
        ' F7    ...                 0                       F7  ...                 400

        'Nel primo caso la F2 "dura" 410 GDD, nel secondo caso la F3 scatta 410 GDD dopo F2
        'con F2 -> faseStart
        'L'algoritmo implementato funziona SOLO NEL PRIMO CASO

        Dim ff As FertiParsPhenophase = input.fenofasi(ff_idx)
        Dim GDDff_sum As Decimal = ff.SommaT

        Dim fert_idx As Integer = 0
        Dim fert_sum As New NPK
        'ci sono fertilizzazioni antecedenti alla data di fase start? sommo
        While fert_idx < input.fertiSommiList.Count AndAlso input.fertiSommiList(fert_idx).Data < input.dataFaseStart
            fert_sum += input.fertiSommiList(fert_idx).Titolo()
            fert_idx += 1
        End While

        Dim gg_data As DateTime = input.dataFaseStart
        Dim gg As Integer = gg_data.DayOfYear
        Dim gg_input As Integer = input.data.DayOfYear
        Dim d_sum As New NPK

        Dim NPKlist As New List(Of NPKValues)

        While gg < input.Tsum.Length '<= gg_input

            'Devo cambiare fase fenologica?
            If (input.Tsum(gg - 1) - GDDff_start) >= GDDff_sum Then
                If ff_idx + 1 < input.fenofasi.Count Then
                    ff_idx += 1
                    ff = input.fenofasi(ff_idx)
                    GDDff_sum += ff.SommaT
                Else
                    Exit While
                End If
            End If

            'Ho fertilizzato? (se ci sono date uguali devo sommare tutti i valori)
            While fert_idx < input.fertiSommiList.Count AndAlso gg_data = input.fertiSommiList(fert_idx).Data
                fert_sum += input.fertiSommiList(fert_idx).Titolo()
                fert_idx += 1
            End While

            Dim GDDd As Decimal = input.Tsum(gg) - input.Tsum(gg - 1)

            Dim vNPK As New NPKValues(gg_data)

            'Nd = Ntot x Nff% x GDDd / GDDff
            vNPK.d = input.fertiTot() * (ff.FabbisognoPerc() / 100D) * GDDd / ff.SommaT
            d_sum += vNPK.d
            vNPK.sd = fert_sum - d_sum
            'Ninf = Ntot x Nff% x Nalfa%
            vNPK.inf = input.fertiTot() * (ff.FabbisognoPerc() / 100D) * (ff.RiservaPerc() / 100D)
            'Nsup = Ninf + Nimp
            vNPK.sup = vNPK.inf + ff.InterventoKgh()

            NPKlist.Add(vNPK)

            gg_data = gg_data.AddDays(1)
            gg += 1

        End While

        'valori di riferimento alla data di interrogazione
        Dim gg_vNPK As NPKValues = NPKlist(gg_input - NPKlist(0).data.DayOfYear)

        'Dose di fertilizzazione -> Nfert = Nd x turno (turno = input.dataIrriPrevista1 - input.data in gg)
        'se Nfert > Nsup - Ninf -> Nfert = Nsup - Ninf

        'se somma(Nfert) <= Ntot -> Nfert
        'se somma(Nfert) > Ntot -> 0
        'se Ntot - somma(Nfert) < Nfert -> NFert = Ntot - somma(Nfert)

        Dim turno As Integer = input.dataIrriPrevista1.DayOfYear - gg_input
        Dim fert As NPK = gg_vNPK.d * turno
        Dim soglia As NPK = gg_vNPK.sup - gg_vNPK.inf 'soglia al momento dell'interrogazione 
        'DOMANDA: non devo considerare la soglia al momento dell'irrigazione? potrebbe cambiare Fase Fenologica ed avere valori diversi
        fert.ApplicaSogliaSup(soglia)

        'fert_sum -> fertilizzazioni effettuate fino alla data di interrogazione...
        Dim fert_residua As NPK = input.fertiTot() - fert_sum
        fert.ApplicaSogliaSup(fert_residua)

        'cerco nella lista NPKList quando ogni elemento finisce sotto la soglia inf
        Dim idx_sottosogliaN As Integer = -1
        Dim bForzaturaN As Boolean = False
        Dim idx_sottosogliaP As Integer = -1
        Dim bForzaturaP As Boolean = False
        Dim idx_sottosogliaK As Integer = -1
        Dim bForzaturaK As Boolean = False

        'parto dal giorno successivo a quello della data di interrogazione
        Dim idx As Integer = gg_input - NPKlist(0).data.DayOfYear + 1

        While idx < NPKlist.Count _
            AndAlso (idx_sottosogliaN < 0 OrElse idx_sottosogliaP < 0 OrElse idx_sottosogliaK < 0) _
            AndAlso NPKlist(idx).data < input.dataIrriPrevista1

            If idx_sottosogliaN < 0 AndAlso NPKlist(idx).sd.N < NPKlist(idx).inf.N Then
                idx_sottosogliaN = idx
                bForzaturaN = NPKlist(idx_sottosogliaN).data < input.dataIrriPrevista1
            End If

            If idx_sottosogliaP < 0 AndAlso NPKlist(idx).sd.P < NPKlist(idx).inf.P Then
                idx_sottosogliaP = idx
                bForzaturaP = NPKlist(idx_sottosogliaP).data < input.dataIrriPrevista1
            End If

            If idx_sottosogliaK < 0 AndAlso NPKlist(idx).sd.K < NPKlist(idx).inf.K Then
                idx_sottosogliaK = idx
                bForzaturaK = NPKlist(idx_sottosogliaK).data < input.dataIrriPrevista1
            End If

            idx += 1

        End While

        Dim bForzatura As Boolean = bForzaturaN Or bForzaturaP Or bForzaturaK
        Dim DataIrriPrevista As DateTime = input.dataIrriPrevista1

        If bForzatura Then
            'irrigazione anticipata rispetto alla prevista
            'DOMANDA: devo ricalcolare la dose? 
            'DOMANDA: per tutti gli elementi o solo per quello/i che scendono sotto la soglia prima della data prossima irrigazione?
            idx = NPKlist.Count
            If bForzaturaN Then
                idx = Math.Min(idx, idx_sottosogliaN)
            End If
            If bForzaturaP Then
                idx = Math.Min(idx, idx_sottosogliaP)
            End If
            If bForzaturaK Then
                idx = Math.Min(idx, idx_sottosogliaK)
            End If

            Dim f_vNPK As NPKValues = NPKlist(idx)
            DataIrriPrevista = f_vNPK.data
            Dim f_turno As Integer = DataIrriPrevista.DayOfYear - gg_input
            fert = f_vNPK.d * f_turno
            'DOMANDA: mantengo la soglia precedente oppure devo ricalcolarla sulla base della data dell'irrigazione forzata?
            fert.ApplicaSogliaSup(soglia)
            fert.ApplicaSogliaSup(fert_residua)

        Else
            'Calcolo se alla prossima irrigazione (dataIrriPrevista2) scenderò sotto soglia con questa fertirrigazione...
            idx = gg_input - input.dataIrriPrevista2.DayOfYear
            'Elementi che avrei nel suolo alla data della prossima irrigazione
            Dim curr_sd As NPK = NPKlist(idx).sd + fert
            'Differenza tra la quantità presunta ed il limite inferiore
            Dim necessario As NPK = NPKlist(idx).inf - curr_sd
            'Aumento la dose di fertilizzazione della quantità che mi permette di non scendere sotto soglia fino alla data della prossima fertilizzazione
            fert += necessario

            Dim irri_vNPK As NPKValues = NPKlist(input.dataIrriPrevista1.DayOfYear - NPKlist(0).data.DayOfYear)
            soglia = irri_vNPK.sup - irri_vNPK.inf 'soglia al momento della data di irrigazione

            fert.ApplicaSogliaSup(soglia)
            fert.ApplicaSogliaSup(fert_residua)
        End If

        Dim output As New FertiAdvice

        output.Data = input.data
        output.DayDoseN = Math.Round(fert.N, 2)
        output.DayDoseP = Math.Round(fert.P, 2)
        output.DayDoseK = Math.Round(fert.K, 2)
        output.DayconsumptionN = Math.Round(gg_vNPK.d.N, 2)
        output.DayconsumptionP = Math.Round(gg_vNPK.d.P, 2)
        output.DayconsumptionK = Math.Round(gg_vNPK.d.K, 2)
        output.NextDate = DataIrriPrevista
        output.Forzatura = bForzatura
        output.Result = 1
        output.Errore = "MESSAGGIO ERRORE"

        Dim cnt As Integer = NPKlist.Count
        Array.Resize(output.NSogliaInf, cnt)
        Array.Resize(output.NSogliaSup, cnt)
        Array.Resize(output.Nsd, cnt)
        Array.Resize(output.PSogliaInf, cnt)
        Array.Resize(output.PSogliaSup, cnt)
        Array.Resize(output.Psd, cnt)
        Array.Resize(output.KSogliaInf, cnt)
        Array.Resize(output.KSogliaSup, cnt)
        Array.Resize(output.Ksd, cnt)
        cnt = 0
        For Each vNPK In NPKlist
            output.NSogliaInf(cnt) = Math.Round(vNPK.inf.N, 2)
            output.NSogliaSup(cnt) = Math.Round(vNPK.sup.N, 2)
            output.Nsd(cnt) = Math.Round(vNPK.sd.N, 2)
            output.PSogliaInf(cnt) = Math.Round(vNPK.inf.P, 2)
            output.PSogliaSup(cnt) = Math.Round(vNPK.sup.P, 2)
            output.Psd(cnt) = Math.Round(vNPK.sd.P, 2)
            output.KSogliaInf(cnt) = Math.Round(vNPK.inf.K, 2)
            output.KSogliaSup(cnt) = Math.Round(vNPK.sup.K, 2)
            output.Ksd(cnt) = Math.Round(vNPK.sd.K, 2)
            cnt += 1
        Next

        Return output

    End Function

End Class

Public Class NPK
    Public N As Decimal
    Public P As Decimal
    Public K As Decimal
    Public Sub New()
        N = 0
        P = 0
        K = 0
    End Sub
    Public Sub New(ByVal other As NPK)
        N = other.N
        P = other.P
        K = other.K
    End Sub
    Public Sub New(ByVal n_ As Decimal?, ByVal p_ As Decimal?, ByVal k_ As Decimal?)
        N = If(n_ IsNot Nothing, CDec(n_), 0)
        P = If(p_ IsNot Nothing, CDec(p_), 0)
        K = If(k_ IsNot Nothing, CDec(k_), 0)
    End Sub
    Public Shared Operator +(ByVal npk1 As NPK, ByVal npk2 As NPK) As NPK
        Return New NPK(npk1.N + npk2.N, npk1.P + npk2.P, npk1.K + npk2.K)
    End Operator
    Public Shared Operator -(ByVal npk1 As NPK, ByVal npk2 As NPK) As NPK
        Return New NPK(Math.Max(0, npk1.N - npk2.N), Math.Max(0, npk1.P - npk2.P), Math.Max(0, npk1.K - npk2.K))
    End Operator
    Public Shared Operator *(ByVal npk1 As NPK, ByVal npk2 As NPK) As NPK
        Return New NPK(npk1.N * npk2.N, npk1.P * npk2.P, npk1.K * npk2.K)
    End Operator
    Public Shared Operator *(ByVal _npk As NPK, ByVal v As Decimal) As NPK
        Return New NPK(_npk.N * v, _npk.P * v, _npk.K * v)
    End Operator
    Public Shared Operator /(ByVal _npk As NPK, ByVal v As Decimal) As NPK
        Return New NPK(_npk.N / v, _npk.P / v, _npk.K / v)
    End Operator
    Public Sub ApplicaSogliaSup(ByVal soglia As NPK)
        If N > soglia.N Then
            N = soglia.N
        End If
        If P > soglia.P Then
            P = soglia.P
        End If
        If K > soglia.K Then
            K = soglia.K
        End If
    End Sub
End Class

Public Class FertiParsPhenophase
    Public SequeIrri As Integer         'ordine della Fenofase
    Public SommaT As Decimal            'GDD_ff
    Public FabbisognoNperc As Decimal   'N_ff%          Opzionale ?
    Public InterventoNKgh As Decimal    'N_imp          Opzionale ?
    Public RiservaNperc As Decimal      'N_alfa%        Opzionale ?
    Public FabbisognoPperc As Decimal   'P_ff%          Opzionale ?
    Public InterventoPKgh As Decimal    'P_imp          Opzionale ?
    Public RiservaPperc As Decimal      'P_alfa%        Opzionale ?
    Public FabbisognoKperc As Decimal   'K_ff%          Opzionale ? 
    Public InterventoKKgh As Decimal    'K_imp          Opzionale ?
    Public RiservaKperc As Decimal      'K_alfa%        Opzionale ?
    Public Function FabbisognoPerc() As NPK
        Return New NPK(FabbisognoNperc, FabbisognoPperc, FabbisognoKperc)
    End Function
    Public Function InterventoKgh() As NPK
        Return New NPK(InterventoNKgh, InterventoPKgh, InterventoKKgh)
    End Function
    Public Function RiservaPerc() As NPK
        Return New NPK(RiservaNperc, RiservaPperc, RiservaKperc)
    End Function
End Class

Public Class FertiSommiG
    Public Data As DateTime 'Data somministrazione (Opzionale ?)
    Public TitoloN As Decimal? 'Titolo N 
    Public TitoloP As Decimal? 'Titolo P₂O₅ 
    Public TitoloK As Decimal? 'Titolo K₂O 
    Public DoseKg As Decimal? 'Dose distribuita Kg 
    Public Function Titolo() As NPK
        Return New NPK(TitoloN, TitoloP, TitoloK)
    End Function
End Class

Public Class FertiManager_input
    Public data As DateTime
    Public Tsum As Decimal()
    Public fertiTotN As Decimal
    Public fertiTotP As Decimal
    Public fertiTotK As Decimal
    Public dataFaseStart As DateTime
    Public fenofasi As List(Of FertiParsPhenophase)
    Public fertiSommiList As List(Of FertiSommiG)
    Public frazResiduaN As Decimal 'Opzionale ?
    Public frazResiduaP As Decimal 'Opzionale ?
    Public frazResiduaK As Decimal 'Opzionale ?
    Public dataIrriPrevista1 As DateTime
    Public dataIrriPrevista2 As DateTime
    Public Function fertiTot() As NPK
        Return New NPK(fertiTotN, fertiTotP, fertiTotK)
    End Function
End Class

Public Class FertiAdvice
    Public Data As DateTime
    Public DayDoseN As Decimal 'Apporti nutritivi da distribuire N (kg/superficie) (Opzionale ?)
    Public DayDoseP As Decimal 'Apporti nutritivi da distribuire P (kg/superficie) (Opzionale ?)
    Public DayDoseK As Decimal 'Apporti nutritivi da distribuire K (kg/superficie) (Opzionale ?)
    Public DayconsumptionN As Decimal 'Consumo giornaliero N (kg/ha) (Opzionale ?)
    Public DayconsumptionP As Decimal 'Consumo giornaliero P (kg/ha) (Opzionale ?)
    Public DayconsumptionK As Decimal 'Consumo giornaliero K (kg/ha) (Opzionale ?)
    Public NextDate As DateTime
    Public Forzatura As Boolean
    Public Result As Integer 'Enumerativo che restituisce il risultato (es: 1=Ok, 2=StopConsiglio,...)   
    Public Errore As String 'Eventuale messaggio di errore
    'Array giornalieri soglie per grafico
    Public NSogliaInf As Decimal()
    Public NSogliaSup As Decimal()
    Public Nsd As Decimal()
    Public PSogliaInf As Decimal()
    Public PSogliaSup As Decimal()
    Public Psd As Decimal()
    Public KSogliaInf As Decimal()
    Public KSogliaSup As Decimal()
    Public Ksd As Decimal()
End Class
