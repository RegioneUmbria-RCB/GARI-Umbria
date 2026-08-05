/**
 * Funzione per generare descrizioni personalizzate.
 * Crea la chiave da usare con transloco per ottenere la traduzione corretta.
 */
function getDescrizione(value: string | enum_UdM_Dose): string {
  return "Zoo_" + value;
}

//#region Zootecnia NEW

export enum enum_Tipo_RisorsaUmana {
    Proprietario = "proprietario",
    Veterinario = "veterinario"
}

export enum enum_UdM_Dose {
  undefined = 0,
  ml_su_100Kg = 2152,
  ml_su_Capo = 2153,
  n_su_Capo = 2154,
  g_su_100Kg = 2155,
}

export const enumUdMDoseArray = Object.entries(enum_UdM_Dose)
  .filter(([key, value]) => typeof value === 'number') // Filtra solo i valori numerici
  .map(([key, value]) => ({
    id: value as number, // Il valore numerico dell'enumerativo
    descrizione: getDescrizione(key) // Descrizione personalizzata
  }));

export enum enum_TypeTab_Zootecnia {
  ZooOperations = 0,
  Prescriptions = 1,
  FuturePrescriptions = 2,
  Indications = 3,
  Therapies = 4
}


export enum enum_FarmacoCategoria {
  Analgesico = 1,
  Antibiotici = 2,
  Corticosteroidi = 3,
  Antidoti = 4,
  Antimicotici = 5,
  Antinfiammatori = 6,
  Antiparassitari = 7,
  Antipertensivi = 8,
  Antiprotozoari = 9,
  Antipsicotici = 10,
  Antistaminici = 11,
  Diagnostici = 12,
  Integratore = 13,
  Mucolitici = 14,
  Oftalmici = 15,
  Ormoni = 16,
  Sedativo = 17,
  Vaccini = 18,
  Vitamine = 19,
  Antiemorragici = 20,
  Epatobiliare = 21
}
  
//#endregion