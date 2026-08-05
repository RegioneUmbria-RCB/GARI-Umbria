
export class Immagine {
    // Il nome del file, visibile in download.
    nome: string;
    // L'estensione del file.
    estensione: string;
    // L'immagine espressa in un array di byte che esprimono un immagine in base64.
    immagine: string;

    constructor(nome: string, estensione:string, immagine:string) {
        this.nome = nome;
        this.estensione = estensione;
        this.immagine = immagine;
    }
}
