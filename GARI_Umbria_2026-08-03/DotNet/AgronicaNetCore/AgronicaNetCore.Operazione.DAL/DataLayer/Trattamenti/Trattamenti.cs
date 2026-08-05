using AgronicaCoreDTOStd.InData;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.DataLayer.Trattamenti
{
    public class Trattamenti : DAL_Base, ITrattamenti
    {
        public Trattamenti(IServiceProvider provider) : base(provider)
        {
        }

        public async Task<DataTable> LeggiTrattamentiPerAvversitaAsync(string Piva, int Sa_Cod, int APPEZZA, int ID_REG, int Av_Cod, int Intervallo, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            var sqlParams = new Dictionary<string, object>();

            stbQuery.AppendLine("SELECT");
            stbQuery.AppendLine("       PIVA, ");
            stbQuery.AppendLine("       SA_COD, ");
            stbQuery.AppendLine("       APPEZZA, ");
            //stbQuery.AppendLine("       APP_NOME, ");
            stbQuery.AppendLine("       ID_REG, ");
            stbQuery.AppendLine("       Veg_Cod, ");
            stbQuery.AppendLine("       Veg_Des, ");
            stbQuery.AppendLine("       Cul_Cod, ");
            stbQuery.AppendLine("       Cul_Des, ");
            stbQuery.AppendLine("       Av_Cod, ");
            stbQuery.AppendLine("       Av_Des_Vol, ");
            stbQuery.AppendLine("       Av_Des_Lat, ");
            stbQuery.AppendLine("       Av_Gru, ");
            stbQuery.AppendLine("       Av_Gru_Des, ");
            stbQuery.AppendLine("       Av_Gru_Des_Lat, ");
            stbQuery.AppendLine("       Data_Ultimo_Trattamento, ");
            stbQuery.AppendLine("       Lav_Cod, ");
            stbQuery.AppendLine("       Lav_Des, ");
            stbQuery.AppendLine("       Fr_Cod, ");
            stbQuery.AppendLine("       Fr_Des, ");
            stbQuery.AppendLine("       DoseEtichetta_Value ");
            stbQuery.AppendLine("FROM (");

            stbQuery.AppendLine("   SELECT ");
            stbQuery.AppendLine("       Reg_Impianti.PIVA, ");
            stbQuery.AppendLine("       Reg_Impianti.SA_COD, ");
            stbQuery.AppendLine("       Reg_Impianti.APPEZZA, ");
            //stbQuery.AppendLine("       Appezzamento.APP_NOME, ");
            stbQuery.AppendLine("       Reg_Impianti.ID_REG, ");
            stbQuery.AppendLine("       SpecieVegetali.Veg_Cod, ");
            stbQuery.AppendLine("       SpecieVegetali.Veg_Des, ");
            stbQuery.AppendLine("       Cultivar.Cul_Cod, ");
            stbQuery.AppendLine("       Cultivar.Cul_Des, ");
            stbQuery.AppendLine("       Avversita.Av_Cod, ");
            stbQuery.AppendLine("       Avversita.Av_Des_Vol, ");
            stbQuery.AppendLine("       Avversita.Av_Des_Lat, ");
            stbQuery.AppendLine("       GruppoAvversita.Av_Gru, ");
            stbQuery.AppendLine("       GruppoAvversita.Av_Gru_Des, ");
            stbQuery.AppendLine("       GruppoAvversita.Av_Gru_Des_Lat, ");
            stbQuery.AppendLine("       Operazioni.Lav_Cod, ");
            stbQuery.AppendLine("       Operazioni.Lav_Des, ");
            stbQuery.AppendLine("       Formulati.Fr_Cod, ");
            stbQuery.AppendLine("       Formulati.Fr_Des, ");
            stbQuery.AppendLine("       Movimenti_dettagli.DoseEtichetta_Value, ");
            stbQuery.AppendLine("       Movimenti.Data_Movimento as Data_Trattamento, ");
            stbQuery.AppendLine("       MAX(Movimenti.Data_Movimento) OVER (PARTITION BY ");
            stbQuery.AppendLine("           Reg_Impianti.PIVA, ");
            stbQuery.AppendLine("           Reg_Impianti.Sa_Cod, ");
            stbQuery.AppendLine("           Reg_Impianti.APPEZZA, ");
            stbQuery.AppendLine("           Reg_Impianti.ID_REG, ");
            stbQuery.AppendLine("           Avversita.Av_Cod, ");
            stbQuery.AppendLine("           GruppoAvversita.Av_Gru) as Data_Ultimo_Trattamento ");
            stbQuery.AppendLine("   FROM ");
            stbQuery.AppendLine("       Agenda ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Movimenti ");
            stbQuery.AppendLine("       on Agenda.Id_Agenda = Movimenti.Id_Agenda ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Movimenti_dettagli ");
            stbQuery.AppendLine("       on Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ");
            stbQuery.AppendLine("       and Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Mov_Destinazioni ");
            stbQuery.AppendLine("       on Movimenti_dettagli.Piva = Mov_Destinazioni.Piva ");
            stbQuery.AppendLine("       and Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ");
            stbQuery.AppendLine("       and Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ");
            //stbQuery.AppendLine("   inner JOIN ");
            //stbQuery.AppendLine("       Appezzamento ");
            //stbQuery.AppendLine("       on Mov_Destinazioni.PIVA = Appezzamento.Piva ");
            //stbQuery.AppendLine("       and Mov_Destinazioni.Sa_Cod = Appezzamento.Sa_Cod ");
            //stbQuery.AppendLine("       and Mov_Destinazioni.Appezza = Appezzamento.APPEZZA ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Reg_Impianti ");
            stbQuery.AppendLine("       on Mov_Destinazioni.PIVA = Reg_Impianti.Piva ");
            stbQuery.AppendLine("       and Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod ");
            stbQuery.AppendLine("       and Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA ");
            stbQuery.AppendLine("       and Mov_Destinazioni.Id_Destinazione =  Reg_Impianti.ID_REG ");
            stbQuery.AppendLine("   left join ");
            stbQuery.AppendLine("       Cultivar");
            stbQuery.AppendLine("       on Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ");
            stbQuery.AppendLine("   left join ");
            stbQuery.AppendLine("       SpecieVegetali ");
            stbQuery.AppendLine("       on SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Operazioni ");
            stbQuery.AppendLine("       on Agenda.Lav_Cod = Operazioni.Lav_Cod ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Mov_Dettaglio_Tecnico ");
            stbQuery.AppendLine("       on Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ");
            stbQuery.AppendLine("   left JOIN ");
            stbQuery.AppendLine("       Avversita ");
            stbQuery.AppendLine("       on Mov_Dettaglio_Tecnico.Av_Cod = Avversita.Av_Cod ");
            stbQuery.AppendLine("   left JOIN ");
            stbQuery.AppendLine("       GruppoAvversita ");
            stbQuery.AppendLine("       on Mov_Dettaglio_Tecnico.Av_Gru = GruppoAvversita.Av_Gru ");
            stbQuery.AppendLine("   inner JOIN ");
            stbQuery.AppendLine("       Formulati ");
            stbQuery.AppendLine("       on Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ");

            stbQuery.AppendLine("   WHERE ");
            stbQuery.AppendLine("       Movimenti.Cau_Mov = '" + CAU_MOV.CAU_TRATTAMENTO + "' ");
            //stbQuery.AppendLine("     AND Agenda.Lav_Cod = " + LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO + " ");
            stbQuery.AppendLine("       AND Movimenti_dettagli.Elem_Cod = " + ELEM_COD.FORMULATI + " ");
            stbQuery.AppendLine("       AND (Avversita.Av_Cod is not null OR GruppoAvversita.Av_Gru is not null) ");

            if (!string.IsNullOrEmpty(Piva))
            {
                sqlParams.TryAdd("@piva", Piva);
                stbQuery.AppendLine("       AND Reg_Impianti.PIVA = @piva ");
            }

            if (Sa_Cod > 0)
            {
                sqlParams.TryAdd("@Sa_Cod", Sa_Cod);
                stbQuery.AppendLine("       AND Reg_Impianti.Sa_Cod = @Sa_Cod ");
            }

            if (APPEZZA > 0)
            {
                sqlParams.TryAdd("@APPEZZA", APPEZZA);
                stbQuery.AppendLine("       AND Reg_Impianti.APPEZZA = @APPEZZA ");
            }

            if (ID_REG > 0)
            {
                sqlParams.TryAdd("@ID_REG", ID_REG);
                stbQuery.AppendLine("       AND Reg_Impianti.ID_REG = @ID_REG ");
            }

            if (Av_Cod > 0)
            {
                sqlParams.TryAdd("@Av_Cod", Av_Cod);
                stbQuery.AppendLine("       AND Avversita.Av_Cod = @Av_Cod ");
            }

            if (Intervallo > 0)
            {
                var Data_Inizio = DateTime.Now.AddDays(-Intervallo);

                sqlParams.TryAdd("@Data_Inizio", Data_Inizio);
                stbQuery.AppendLine("       AND Movimenti.Data_Movimento >= @Data_Inizio ");
            }

            stbQuery.AppendLine(") as X ");
            stbQuery.AppendLine("WHERE Data_Trattamento = Data_Ultimo_Trattamento ");

            try
            {
                return await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString(), sqlParams);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
