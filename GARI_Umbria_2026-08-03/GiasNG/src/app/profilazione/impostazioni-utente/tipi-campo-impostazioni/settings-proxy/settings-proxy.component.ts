import {Component, Input, OnInit} from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import {debounceTime} from "rxjs/operators";

@Component({
    standalone: false,
    selector: 'app-settings-proxy',
    templateUrl: './settings-proxy.component.html',
    styleUrls: ['./settings-proxy.component.css']
})
export class SettingsProxyComponent implements OnInit {
    @Input('formGroup') form: FormGroup;

    proxyForm: FormGroup = new FormGroup({
        enabled: new FormControl(false),
        username: new FormControl(''),
        password: new FormControl(''),
        host: new FormControl('')
    });

    public get isDisabled(): boolean {
      return this.form.disabled;
    }

    public get valori(): Array<any> {
        return this.form?.get('valori')?.value || [];
    }

    public get isProxyEnabled(): boolean {
        return this.proxyForm.controls['enabled'].value;
    }

    ngOnInit(): void {
        this.form.valueChanges.subscribe(val => console.log('myChange',val));
        this.initForm();
        this.handleChanges();
    }

    private handleChanges() {
        this.proxyForm.get('enabled').valueChanges
            .subscribe(enabled => this.updateProxySettings());
        this.proxyForm.get('username').valueChanges
            .pipe(debounceTime(200))
            .subscribe(user => this.updateProxySettings());
        this.proxyForm.get('password').valueChanges
            .pipe(debounceTime(200))
            .subscribe(pass => this.updateProxySettings());
        this.proxyForm.get('host').valueChanges
            .pipe(debounceTime(200))
            .subscribe(host => this.updateProxySettings());
    }

    private initForm() {
        let valori = this.form?.value?.valori;
        this.proxyForm.get('enabled'). patchValue(valori[0]?.value || false);
        this.proxyForm.get('username'). patchValue(valori[1]?.value || '');
        this.proxyForm.get('password'). patchValue(valori[2]?.value || '');
        this.proxyForm.get('host'). patchValue(valori[3]?.value || '');
        if (this.isDisabled) {
          this.proxyForm.disable();
        }
    }

    private updateProxySettings() {
        let newValue = this.proxyForm.value;
        if (!newValue.enabled) {
            newValue.username = '';
            newValue.password = '';
            newValue.host = '';
        }
        this.form.get('valoreCorrente').patchValue(JSON.stringify(newValue));
    }

}
