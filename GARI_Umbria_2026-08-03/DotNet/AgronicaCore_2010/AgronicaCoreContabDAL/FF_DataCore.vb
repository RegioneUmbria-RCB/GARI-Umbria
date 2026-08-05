
Imports System.Text

Public Class FF_DataCore



    Public Shared Sub leggiParametriOmniFF_FROM(ByVal Parametro As String, ByVal Tabella_Origine_Cal_Cod As String, ByRef stb As StringBuilder)

        'OModuli_Referenze_Config_Testata (OFiltro_veg_Cod (elenco di specie con separatore) )
        'OModuli_Referenze_Config_Dettagli OChkEtichetta = 1 --> va stampata in etichetta

        stb.Append(vbCrLf)
        stb.Append(" left join Materie_Prime_Campionature c" & Parametro & " " & vbCrLf)
        stb.Append("         on c" & Parametro & ".Progressivo = " & Tabella_Origine_Cal_Cod & ".Cal_Cod   " & vbCrLf)
        stb.Append("         and c" & Parametro & ".Tipo = 'O" & Parametro & "'  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join OTabelle ot" & Parametro & " " & vbCrLf)
        stb.Append("        on 'O' + ot" & Parametro & ".Tabella_Des = c" & Parametro & ".tipo " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    left join OTabelle_Parametri otp" & Parametro & " " & vbCrLf)
        stb.Append("        on otp" & Parametro & ".Tabella_Par_Cod = c" & Parametro & ".tipo_cod" & vbCrLf)

        stb.Append("    left join ( select distinct piva, Codice_Generazione, Mat_Cod from OGenerazioni_Anagrafe_Log where modulo_generazione = 2 )  ogenLog" & Parametro & " " & vbCrLf)
        stb.Append("        on ogenLog" & Parametro & ".Piva = " & Tabella_Origine_Cal_Cod & ".Piva  " & vbCrLf)
        stb.Append("        and ogenLog" & Parametro & ".Codice_generazione = otp" & Parametro & ".Codice_Generazione_Link  " & vbCrLf)



        stb.Append("   left join materie_prime mp" & Parametro & vbCrLf)
        stb.Append("        on mp.mat_Cod = ogenLog" & Parametro & ".Mat_Cod  " & vbCrLf)



    End Sub

End Class
