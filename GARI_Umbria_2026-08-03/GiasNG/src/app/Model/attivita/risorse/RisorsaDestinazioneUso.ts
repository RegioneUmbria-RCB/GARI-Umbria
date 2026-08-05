import { DestinazioneUso } from "app/Model/metaschema/utilizzi/DestinazioneUso";
import { Risorsa } from "./Risorsa";

export class RisorsaDestinazioneUso extends Risorsa {

    destinazioneUso: DestinazioneUso;

    constructor() {
        super();
        this.destinazioneUso = new DestinazioneUso();
        this.classType = 'RisorsaDestinazioneUso';
    }

}