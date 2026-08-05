
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface UserModel {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone: string;
}

//const link = 'https://localhost/AgronicaCoreAPI'
const link = 'https://localhost:44350'

@Injectable({
    providedIn: 'root',
})
export class AuthService {

    userProfile: BehaviorSubject<UserModel> = new BehaviorSubject<UserModel>({
        email: '',
        firstName: '',
        id: 0,
        lastName: '',
        phone: '',
    });

    private userToken: string = "";

    constructor(private http: HttpClient) { }


    login(user: any) {
        let headers = new HttpHeaders({
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': link,
            'Access-Control-Allow-Headers': 'X-Requested-With,content-type',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
            'Access-Control-Allow-Credentials': 'true',
        });
        let options = { headers: headers, withCredentials: true };

        return this.http.post(link + '/Login', user, options);
    }

    saveUserToLocalStorage(user: UserModel) {
        this.userProfile.next(user);
        localStorage.setItem('user-profile', JSON.stringify(user));
    }

    getJWT() {
         let headers = new HttpHeaders({
             'Content-Type': 'application/json',
             'Access-Control-Allow-Origin': link,
             'Access-Control-Allow-Headers': 'X-Requested-With,content-type',
             'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PUT, PATCH, DELETE',
             'Access-Control-Allow-Credentials': 'true',
         });
        let options = { headers: headers, withCredentials: true };
        return this.http.get(link + '/Login', options);
    }

    getVersione() {
        return this.http.get(link + '/Login/Versione', {responseType: 'text'});
    }

}
