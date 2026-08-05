
export enum Enum_OrigineRichiestaVerificaConformita {
  verifica_massiva_webservice = 0,
  verifica_massiva_engine = 1,
  gsb_massivo_webservice = 2,
  gsb_massivo_engine = 3,
  operazione_edit_salvataggio_ng = 4,
  operazione_edit_verifica_ng = 5,
}

// TODO: aggiungere traduzioni (per ora mi era stato detto di non farlo e aspettare più avanti)
export enum Enum_OrigineRichiestaDescrizione {
  verifica_massiva_webservice = 'Verifica Conformità Quaderno',
  verifica_massiva_engine = 'Verifica Conformità Quaderno (new!)',
  gsb_massivo_webservice = '[GSB] Verifica Conformità Quaderno',
  gsb_massivo_engine = '[GSB] Verifica Conformità Quaderno (new!)',
  operazione_edit_salvataggio_ng = 'Verifica Conformità Operazione Edit (salvataggio)',
  operazione_edit_verifica_ng = 'Verifica Conformità Operazione Edit (verifica)',
}
