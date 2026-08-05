// Metodo ImpostaSelezioneAlbero in GiasOnline_2010.
export enum AnagraficaLevels {
    Imprese = 1,
    Centri = 2,
    Campi = 3,
    Appezzamenti = 4,
    Impianti = 5,
    Esercizi = 14,
    Catasto = 6,
    Macchine = 12,
    Contatti = 11,
    Fabbricati = 10,
}

const IMPRESE = AnagraficaLevels.Imprese;
const CENTRI = AnagraficaLevels.Centri;
const CAMPI = AnagraficaLevels.Campi;
const Appezzamenti = AnagraficaLevels.Appezzamenti;
const IMPIANTI = AnagraficaLevels.Impianti;
const ESERCIZI = AnagraficaLevels.Esercizi;
const CATASTO = AnagraficaLevels.Catasto;
const MACCHINE = AnagraficaLevels.Macchine;
const CONTATII = AnagraficaLevels.Contatti;
const FABBRICATI  = AnagraficaLevels.Fabbricati;


export class AnagraficaRoutes {
    static Imprese = new AnagraficaRoutes('/Anagrafica/Imprese', IMPRESE);
    static Centri = new AnagraficaRoutes('/Anagrafica/Centri', CENTRI);
    static Campi = new AnagraficaRoutes('/Anagrafica/Campi', CAMPI);
    static Appezzamenti = new AnagraficaRoutes('/Anagrafica/Impianti', Appezzamenti);
    static Impianti =  new AnagraficaRoutes('/Anagrafica/Impianti', IMPIANTI);
    static Esercizi =  new AnagraficaRoutes('/Anagrafica/Impianti', ESERCIZI);
    static Catasto = new AnagraficaRoutes('/Anagrafica/Catasto', CATASTO);
    static Macchine = new AnagraficaRoutes('/Anagrafica/Macchine', MACCHINE);
    static Contatti = new AnagraficaRoutes('/Anagrafica/Contatti', CONTATII);
    static Fabbricati = new AnagraficaRoutes('/Anagrafica/Fabbricati',FABBRICATI);

    constructor(public link: string, public lvl: AnagraficaLevels) { }
}
