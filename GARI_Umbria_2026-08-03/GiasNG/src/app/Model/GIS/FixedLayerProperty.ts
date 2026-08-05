export class FixedLayerProperty {

    // Trasparenza percentuale: da 0 a 100
    public Trasparenza: number;

    // Informazioni su click mappa
    public InfoClickMappa: boolean;

    constructor() {
        this.Trasparenza = null;
        this.InfoClickMappa = false;
    }

    getTrasparenzaPercentuale(): number {
        return this.Trasparenza / 100;
    }

    getOpacitaPercentuale(): number {
        return (1 - this.getTrasparenzaPercentuale());
    }

}
