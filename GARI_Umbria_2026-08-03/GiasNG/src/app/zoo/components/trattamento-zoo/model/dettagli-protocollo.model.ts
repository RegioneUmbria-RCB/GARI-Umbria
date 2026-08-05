export class DettagliProtocollo {
    idProtocol: number;
    tipoPrescrizione: number;
    massivo: boolean;
    quantitaTotaleReale: number;
    qtaDose: number;
    udmDose: number;
    durataTrattamento: number;
    arrotondamentoPeso: number;

    constructor(idProtocol: number, tipoPrescrizione: number, massivo: boolean, quantitaTotaleReale: number, qtaDose: number, udmDose: number, durataTrattamento: number, arrotondamentoPeso: number) {
        this.idProtocol = idProtocol;
        this.tipoPrescrizione = tipoPrescrizione;
        this.massivo = massivo;
        this.quantitaTotaleReale = quantitaTotaleReale;
        this.qtaDose = qtaDose;
        this.udmDose = udmDose;
        this.durataTrattamento = durataTrattamento;
        this.arrotondamentoPeso = arrotondamentoPeso;
    }
}