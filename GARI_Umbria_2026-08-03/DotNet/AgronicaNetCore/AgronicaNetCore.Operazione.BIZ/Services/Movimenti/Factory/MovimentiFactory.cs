using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.costanti;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli.Factory;
using InData.Anagrafica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti.Factory
{
    public class MovimentiFactory : BaseServiceOperazioneBIZ, IMovimentiFactory
    {
        private readonly IMovDettagliFactory _movDettagliFactory;

        public MovimentiFactory(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _movDettagliFactory = provider.GetRequiredService<IMovDettagliFactory>();
        }
        private Movimento GetBaseMovimento(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = new();
            mov.Id_Agenda = agenda.Id_Agenda;
            mov.Piva = agenda.Piva;
            mov.Sa_Cod = agenda.Sa_Cod;
            mov.Data = agenda.Data;
            mov.Ora = new DateTime(agenda.Data.Year, agenda.Data.Month, agenda.Data.Day, attivita.oraInizio.Hour, attivita.oraInizio.Minute, attivita.oraInizio.Second);
            mov.OraFine = new DateTime(agenda.Data.Year, agenda.Data.Month, agenda.Data.Day, attivita.oraFine.Hour, attivita.oraFine.Minute, attivita.oraFine.Second);
            mov.Lav_Cod = agenda.Lav_Cod;
            mov.Cau_Mov = info.Cau_Mov;
            mov.Mov_Desc = attivita.note;
            mov.Num_Protocollo = 0;
            mov.Extra_Int = 0;
            mov.Mezzo = 0;
            mov.Modalita = attivita.modalita;
            if (attivita.modalitaApplicazione != null)
            {
                mov.Modalita_Applicazione = attivita.modalitaApplicazione.codice;
            }
            mov.BaseCode = info.BaseCode;
            mov.TopCode = info.TopCode;
            return mov;
        }

        private Movimento GetMovimentoTrattamento(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = GetBaseMovimento(attivita, agenda, info);
            throw new NotImplementedException();
            return mov;
        }

        private Movimento GetMovimentoFertilizzazione(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = GetBaseMovimento(attivita, agenda, info);
            throw new NotImplementedException();
            return mov;
        }

        private Movimento GetMovimentoRilievo(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = GetBaseMovimento(attivita, agenda, info);
            mov.Movimenti_Dettagli = _movDettagliFactory.GetMovDettagliList(attivita, agenda, info, 0);
            return mov;
        }


        private Movimento GetMovimentoRaccolta(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = GetBaseMovimento(attivita, agenda, info);

            Attivita.Tipo_Raccolta harvestType = Attivita.Tipo_Raccolta.Fast;
            IEnumerable<RisorsaProdotto> harvestDetails = attivita.risorse
                .FindAll(r => r.classType == ClassType.DettaglioRaccolta)
                .Cast<RisorsaProdotto>();
            if (harvestDetails.Any(d => d.prodotto != null && d.prodotto.codice != 0))
            {
                harvestType = Attivita.Tipo_Raccolta.Leggera;
                if (harvestDetails.Select(d => d.MagazziniMovimentazioni)
                    .Any(m => m.Count > 0 && m.First().Magazzino.primaryKey.codice != 0))
                {
                    harvestType = Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino;
                }
            }
            attivita.tipoRaccolta = harvestType;
            mov.Extra_Int = (int)harvestType;

            if (harvestDetails.Any())
            {
                mov.Mezzo = (int)((DettaglioRaccolta)harvestDetails.First()).Opzioni_Raccolta.Ripartizione;
            } else
            {
                mov.Mezzo = (int)Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_SUPERFICIE;
            }

            return mov;
        }

        private Movimento GetMovimentoSemina(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento mov = GetBaseMovimento(attivita, agenda, info);
            throw new NotImplementedException();
            return mov;
        }

        public Movimento GetMovimentoCampagna(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            if (info.IsTrattamento)
                return GetMovimentoTrattamento(attivita, agenda, info);
            else if (info.IsFertilizzazione)
                return GetMovimentoFertilizzazione(attivita, agenda, info);
            else if (info.IsRilievo)
                return GetMovimentoRilievo(attivita, agenda, info);
            else if (info.IsRaccolta)
                return GetMovimentoRaccolta(attivita, agenda, info);
            else if (info.IsSemina)
                return GetMovimentoSemina(attivita, agenda, info);
            else
                return GetBaseMovimento(attivita, agenda, info);
        }
    }
}
