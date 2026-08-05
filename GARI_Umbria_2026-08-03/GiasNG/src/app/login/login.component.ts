import { Component } from '@angular/core';
import { AuthService } from './auth.service';

@Component({
  standalone: false,
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

    loginUser: any = {
        username: '',
        password: '',
        pivaSuperUser: '03761180961',
        versioneAPP: "1",
        coreWSBaseURL: "http://localhost/AgronicaCoreWS"
    };

    constructor(private auth: AuthService) { }

    login() {
        this.auth.login(this.loginUser).subscribe({
            next: (resp) => {
                alert('login successful');
                //console.log(resp);
            },
            error: (error) => {
                alert('login failed');
                //console.log(error);
            },
        });
    }

    getUt() {
        this.auth.getJWT().subscribe({
            next: (resp) => {
                alert('Successful');
                // console.log(resp);
            },
            error: (error) => {
                alert('Failed');
                // console.log(error);
            },
        });

    }

    getVersione() {
        this.auth.getVersione().subscribe(val => {
            // console.log(val);
            alert(val);
        });
    }
}
