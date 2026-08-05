using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Zoo
{
    public class Prescrizione
    {
        public string PresAziendaCodice { get; set; }
        public string PresDetentoreDenominazione { get; set; }
        public string PresDetentoreIdFiscale { get; set; }
        public string PresDtEmissione { get; set; }
        public string PresNote { get; set; }
        public string PresNumero { get; set; }
        public string PresPin { get; set; }
        public string PresProprietarioDenominazione { get; set; }
        public string PresProprietarioIdFiscale { get; set; }
        public string PresStatoCodice { get; set; }
        public string PresStatoDescrizione { get; set; }
        public string PresStrutturaCodice { get; set; }
        public string PresStrutturaDenominazione { get; set; }
        public string PresTipoCodice { get; set; }
        public string PresTipoDescrizione { get; set; }
        public string PresVeterinarioDenominazione { get; set; }
        public string PresVeterinarioIdFiscale { get; set; }
        public string PresRigaCardinalita { get; set; }
        public string ProtCatClassyfarmCodice { get; set; }
        public string ProtCatClassyfarmDescrizione { get; set; }
        public string ProtDiagnosiCodice {  get; set; }
        public string ProtDiagnosiDescrizione { get; set; }
        public string ProtSpecieCodice { get; set; }
        public string ProtSpecieDescrizione { get; set; }
        public List<RigaPrescrizione> RighePrescrizione { get; set; }
    }

    public class RigaPrescrizione
    {
        public int PrescapoCardinalita { get; set; }
        public int PrescapoTotale { get; set; }
        public string PresrigaDescrizione { get; set; }
        public DateTime PresrigaDtFineTrattamento { get; set; }
        public DateTime PresrigaDtInizioTrattamento { get; set; }
        public int PresrigaDurataTrattamento { get; set; }
        public bool PresrigaFlAntimicrobico { get; set; }
        public bool PresrigaFlOrmonale { get; set; }
        public string PresrigaFlTipoMedicinale { get; set; }
        public bool PresrigaFlVaccino { get; set; }
        public string PresrigaMangimeComposizione { get; set; }
        public string PresrigaMangimeDenominazione { get; set; }
        public string PresrigaNumero { get; set; }
        public string PresrigaNote { get; set; }
        public string PresrigaPosologia { get; set; }
        public string PresrigaProdottoAic { get; set; }
        public string PresrigaProdottoConfezione { get; set; }
        public string PresrigaProdottoDenominazione { get; set; }
        public string PresrigaProdottoFamigliaAic { get; set; }
        public string PresrigaProdottoFamigliaDenominazione { get; set; }
        public string PresrigaProdottoUnitaMisuraCodice { get; set; }
        public double PresrigaQuantitativo { get; set; }
        public string RegscoNumero { get; set; }
        public List<CapoRigaPrescrizione> CapiRigaPrescrizione { get; set; }
    }

    public class CapoRigaPrescrizione
    {
        public string PrescapoCodificaCodice { get; set; }
        public string PrescapoCodificaDescrizione { get; set; }
        public string PrescapoDiagnosiCodice { get; set; }
        public string PrescapoDiagnosiDescrizione { get; set; }
        public object PrescapoDtNascita { get; set; }
        public string PrescapoFlStatoAnomalia { get; set; }
        public string PrescapoIdentificativo { get; set; }
        public string PrescapoNote {  get; set; }
        public string PrescapoNumero { get; set; }
        public string PrescapoNumeroAnimali { get; set; }
        public string PrescapoSesso { get; set; }
        public string PrescapoSommministrazioneCodice { get; set; }
        public string PrescapoSommministrazioneDescrizione { get; set; }
        public string PrescapoSottocategoriaCodice { get; set; }
        public string PrescapoSottocategoriaDescrizione { get; set; }
        public string PrescapoSpecieCodice { get; set; }
        public string PrescapoSpecieDescrizione { get; set; }
        public List<TempoSospensioneCapo> PrescapoTempiSospensione { get; set; }
    }

    public class TempoSospensioneCapo
    {
        public string TempososTipoAlimentoCodice { get; set; }
        public string TempososTipoAlimentoDescrizione { get; set; }
        public string TempososUnitaMisuraCodice { get; set; }
        public string TempososUnitaMisuraDescrizione { get; set; }
        public int TempososValore { get; set; }
    }
}
