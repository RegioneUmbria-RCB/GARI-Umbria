export enum enum_TipoPrescrizione {
  UNDEFINED = 0,
  Veterinaria = 1,
  Protocollo_Terapeutico = 2,
  Da_Protocollo = 3,
  Indicazione_Terapeutica = 4,
  Rifornimento_Scorta = 5,
  Da_Protocollo_GIAS = 6,
  Protocollo_Terapeutico_Programmato = 7,
}


export enum enum_TipoGrigliaTrattamentiZoo {
    None = 0,
    Protocolli = 1,              // griglia Protocolli
    ProtocolliInCorso = 2,       // griglia Protocolli In Corso
    PrescrizioniVeterinarie = 3, // griglia Prescrizioni Veterinarie e Indicazioni Terapeutiche
}