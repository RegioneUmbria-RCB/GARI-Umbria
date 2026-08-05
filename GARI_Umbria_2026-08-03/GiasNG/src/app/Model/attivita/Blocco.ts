export class Blocco{

  tipo: Tipo_Blocco;
  utente: string;
  data: Date;

  constructor(tipo: Tipo_Blocco, utente: string,data: Date) {
    this.tipo = tipo;
    this.utente = utente;
    this.data = data;
  }
}

export enum Tipo_Blocco{
  Nessuno = 0,
  QuadernoDiCampagna = 1
}
