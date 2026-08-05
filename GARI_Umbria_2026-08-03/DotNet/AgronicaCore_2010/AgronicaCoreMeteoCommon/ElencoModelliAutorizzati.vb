Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ElencoModelliAutorizzati

    Private Class Specie
        Implements IComparable(Of Specie)

        Public Class ModelloXSpecie
            Implements IComparable(Of ModelloXSpecie)

            Public Mod_Cod As Integer
            Public Mod_Des As String
            Public Avv_Cod As Integer
            Public Avv_Des As String
            Public Alg_Cod As Integer
            Public Alg_Des As String
            Public Mod_Des_Agg As String
            Public HasIndic As Boolean
            Public InizioPeriodo_gg As Integer
            Public FinePeriodo_gg As Integer
            Public ReadOnly Property Full_Des() As String
                Get
                    Dim des As String = Mod_Des
                    If Not String.IsNullOrEmpty(Alg_Des) Then
                        des &= " (" & Alg_Des & ")"
                    End If
                    If Not String.IsNullOrEmpty(Mod_Des_Agg) Then
                        des &= " [" & Mod_Des_Agg & "]"
                    End If
                    'des &= " " & Avv_Des
                    Return des
                End Get
            End Property

            Public Function CompareTo(other As ModelloXSpecie) As Integer Implements IComparable(Of ModelloXSpecie).CompareTo
                Dim cmp As Integer = Mod_Des.CompareTo(other.Mod_Des)
                If cmp = 0 Then
                    cmp = Alg_Des.CompareTo(other.Alg_Des)
                    If cmp = 0 Then
                        cmp = Avv_Des.CompareTo(other.Avv_Des)
                    End If
                End If
                Return cmp
            End Function
        End Class

        Public Veg_Cod As Integer
        Public Veg_Des As String
        Public Modelli As List(Of ModelloXSpecie)

        Public Function CompareTo(other As Specie) As Integer Implements IComparable(Of Specie).CompareTo
            Return Veg_Des.CompareTo(other.Veg_Des)
        End Function
    End Class

    Public Function Genera(jarr_modelli As JArray, objParametri_Server As AgronicaCoreParametri) As String

        'L'array è (deve essere) ordinato per Veg_Cod, Mod_Cod, Av_Cod, Alg_Cod

        Dim dt_impostazioni As DataTable = (New AgronicaCoreMeteoDAL.DSS_ModelliImpostazioni).Leggi("", "", objParametri_Server)

        Dim elenco As New List(Of Specie)

        For Each jobj In jarr_modelli

            Dim VegCod As Integer = CInt(jobj("Veg_Cod"))
            Dim ModCod As Integer = CInt(jobj("Mod_Cod"))
            Dim AvvCod As Integer = CInt(jobj("Av_Cod"))
            Dim AlgCod As Integer = CInt(jobj("Alg_Cod"))
            Dim objIndic As Integer? = jobj("IndicatoreDisponibile")
            Dim HasIndic As Boolean = Not objIndic.HasValue OrElse objIndic.Value = 1

            Dim Inizio_gg As Integer = 1
            Dim Fine_gg As Integer = 365

            If dt_impostazioni IsNot Nothing AndAlso dt_impostazioni.Rows.Count > 0 Then
                Dim r_impo = dt_impostazioni.Select.Where(Function(x As DataRow)
                                                              Return CInt(x("Mod_Cod")) = ModCod AndAlso CInt(x("Veg_Cod")) = VegCod AndAlso CInt(x("Avv_Cod")) = AvvCod AndAlso CInt(x("Alg_Cod")) = AlgCod
                                                          End Function)
                If r_impo IsNot Nothing AndAlso r_impo.Any() Then
                    Inizio_gg = CInt(r_impo(0)("DatiMeteoInizio_gg"))
                    Fine_gg = CInt(r_impo(0)("DatiMeteoFine_gg"))
                End If
            End If

            If elenco.Count = 0 OrElse elenco.Last().Veg_Cod <> VegCod Then
                elenco.Add(New Specie With {
                           .Veg_Cod = VegCod,
                           .Veg_Des = jobj("Veg_Des").ToString,
                           .Modelli = New List(Of Specie.ModelloXSpecie)
                           })
            End If

            elenco.Last().Modelli.Add(New Specie.ModelloXSpecie With {
                                      .Mod_Cod = ModCod,
                                      .Mod_Des = jobj("Mod_Des").ToString(),
                                      .Avv_Cod = AvvCod,
                                      .Avv_Des = jobj("Av_Des_Lat").ToString(),
                                      .Alg_Cod = AlgCod,
                                      .Alg_Des = jobj("Alg_Des").ToString(),
                                      .Mod_Des_Agg = jobj("Mod_Des_Agg").ToString(),
                                      .HasIndic = HasIndic,
                                      .InizioPeriodo_gg = Inizio_gg,
                                      .FinePeriodo_gg = Fine_gg
                                      })
        Next

        If elenco.Any() Then

            elenco.Sort()

            For Each s In elenco

                s.Modelli.Sort()
            Next
        End If

        Return JsonConvert.SerializeObject(elenco,
                                           Newtonsoft.Json.Formatting.None,
                                           New JsonSerializerSettings With {
                                           .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                           .NullValueHandling = NullValueHandling.Ignore
                                           })
    End Function
End Class