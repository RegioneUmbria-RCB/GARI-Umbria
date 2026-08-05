import { Utente } from "./models";

export const GIAS_USERNAME_TOKEN = 'GIAS_USERNAME_TOKEN';

export interface IUsernameService {
  getCurrentUser(): Utente;
}
