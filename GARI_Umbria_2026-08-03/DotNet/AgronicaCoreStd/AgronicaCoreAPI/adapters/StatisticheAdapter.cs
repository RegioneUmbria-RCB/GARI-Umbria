using AgronicaCoreAPI.models.entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using static AgronicaCoreAPI.models.StatisticheEntity;

namespace AgronicaCoreAPI.adapters
{
    public class StatisticheAdapter
    {
        public List<RilievoProduzione> leggiRilieviProduzione(string dati)
        {
            var dt = JsonConvert.DeserializeObject<DataTable>(dati);
            var rilieviProduzione = new List<RilievoProduzione>();

            foreach (DataRow row in dt.Rows)
            {
                rilieviProduzione.Add(new RilievoProduzione()
                {
                    partitaIva = Convert.ToString(row["piva"]),
                    centroAziendaleCod = Convert.ToInt32(row["sa_cod"]),
                    appezzamentoCod = Convert.ToInt32(row["appezza"]),
                    impiantoCod = Convert.ToInt32(row["id_reg"]),
                    esercizioCod = Convert.ToInt32(row["progetto_cod"]),
                    resaRilievo = Convert.ToDouble(row["resa_rilievo"]),
                    dataRilievo = Convert.ToDateTime(row["data_rilievo"]),
                    utenteRilievo = Convert.ToString(row["utente_rilievo"])
                });
            }

            return rilieviProduzione;
        }

        public List<StimeProduzione> leggiStimeProduzione(string dati)
        {
            // return JsonConvert.DeserializeObject<List<StimeProduzione>>(dati);            
            var dt = JsonConvert.DeserializeObject<DataTable>(dati);

            var stimeProduzione = new List<StimeProduzione>();
            foreach (DataRow row in dt.Rows)
            {
                stimeProduzione.Add(new StimeProduzione()
                {
                    partitaIva = Convert.ToString(row["piva"]),
                    ragioneSociale = Convert.ToString(row["rag_soc"]),
                    centroAziendaleCod = Convert.ToInt32(row["sa_cod"]),
                    centroAziendaleNome = Convert.ToString(row["sa_nome"]),
                    appezzamentoCod = Convert.ToInt32(row["appezza"]),
                    appezzamentoNome = Convert.ToString(row["app_nome"]),
                    impiantoCod = Convert.ToInt32(row["id_reg"]),
                    esercizioCod = Convert.ToInt32(row["progetto_cod"]),
                    specieCod = Convert.ToInt32(row["veg_cod"]),
                    specie = Convert.ToString(row["veg_des"]),
                    varietaCod = Convert.ToInt32(row["cul_cod"]),
                    varieta = Convert.ToString(row["cul_des"]),
                    produzione = Convert.ToString(row["produzione"]),
                    superficie = Convert.ToDouble(row["sup_imp"]),
                    supAbbattuta = Convert.ToDouble(row["sup_abbattuta"]),
                    rilievoDanni = Convert.ToDouble(row["perc_piante_morte"]),
                    resaPrevista = Convert.ToDouble(row["produzione_prevista"]),
                    stimaProduzione = Convert.ToDouble(row["stime_produzione"]),
                    dataPrimaProduzione = Convert.ToDateTime(row["annoprimaproduzione"])
                });
            }

            return stimeProduzione;
        }
    }
}
