import {AfterContentChecked, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, ValidationErrors, Validators} from "@angular/forms";
import {faEye, faEyeSlash, faInfoCircle} from "@fortawesome/free-solid-svg-icons";
import {TranslocoService} from "@jsverse/transloco";
import {TextBoxComponent} from "@progress/kendo-angular-inputs";
import {cloneDeep} from "lodash";
import {Subject, takeUntil} from 'rxjs';

/** @usageNotes Da per scontato che l'oggetto passato abbia un campo `Password`.
 */
@Component({
  standalone: false,
  selector: 'app-grid-password',
  templateUrl: './grid-password.component.html',
  styleUrls: ['./grid-password.component.css']
})
export class GridPasswordComponent implements OnInit, AfterContentChecked, OnDestroy {
    /** Il dataItem rappresentante la riga corrente */
    @Input() input: any;
    /** Indica il campo in dataItem di tipo DynamicInputSetting a cui fare
     *  riferimento per usare il componente.
     *  Necessario specificarlo per ogni riga in modo da rendere la cosa
     *  dinamica.
     */
    @Input() field: any;
    @Input() edit = false;
    /** FormGroup usato per l'editing della riga corrente */
    @Input() formGroup: FormGroup;


    @ViewChild('pwd') pwd: TextBoxComponent;
    @ViewChild('pwdConfirm') pwdConfirm: TextBoxComponent;
    @ViewChild('pwdError') pwdError: ElementRef;
    @ViewChild('pwdConfirmError') pwdConfirmError: ElementRef;

    public form: FormGroup;
    public isPwdVisible = false;
    public isPwdConfirmVisible = false;
    public faEye = faEye;
    public faEyeSlash = faEyeSlash;
    public pwdLabel = this.transloco.translate('Password');
    public pwdConfirmLabel = this.transloco.translate('ConfermaPassword');

    private oldPassword: string;
    private AT_LEAST_ONE_NUMBER = /[0-9]/;
    private AT_LEAST_ONE_UPPER = /[A-Z]/;
    private AT_LEAST_ONE_LOWER = /[a-z]/;
    private AT_LEAST_ONE_SPECIAL = /[!?_$%&']/;
    private $signal = new Subject<boolean>();

    constructor(private transloco: TranslocoService) { }

  /** Progettata specificatamente per distinguere i nuovi elementi in {@link MenuProfilazioneGridConfigService}.
   * Da rivedere in caso fosse necessario utilizzare il componente in altro contesto.
   * @private
   */
    private get isNewItem(): boolean {
      return this.input.kendoKey && +this.input.kendoKey < 0;
      // return !this.input.kendoKey && this.input.kendoKey !== 0;
    }

    ngOnInit(): void {
        // Quando viene creato il component input è il dataItem
        this.oldPassword = this.input[this.field] || '';
        this.initForm();
        this.handleValidationChange();
        this.handleFormChange();
    }
    ngAfterContentChecked(): void {
        if (this.pwd) {
            this.pwd.input.nativeElement.type = this.isPwdVisible ? 'text' : 'password';
            this.computePwdError();
        }
        if (this.pwdConfirm) {
            this.pwdConfirm.input.nativeElement.type = this.isPwdConfirmVisible ? 'text' : 'password';
            this.computePwdConfirmError();
        }
    }
    ngOnDestroy(): void {
      this.$signal.next(true);
      this.$signal.complete();
    }


    private initForm(): void {
        this.form = new FormGroup({
            old: new FormControl(this.oldPassword),
            password: new FormControl(''),
            passwordConfirm: new FormControl('')
        });
        if (this.isNewItem) {
          this.form.get('password').patchValue(this.oldPassword);
          this.form.get('passwordConfirm').patchValue(this.oldPassword);
        } else {
          this.form.get('password').addValidators(this.diffPrevPassword);
        }
        this.form.get('password').addValidators([
            Validators.required,
            Validators.minLength(8),
            Validators.pattern(this.AT_LEAST_ONE_NUMBER),
            Validators.pattern(this.AT_LEAST_ONE_UPPER),
            Validators.pattern(this.AT_LEAST_ONE_LOWER),
            Validators.pattern(this.AT_LEAST_ONE_SPECIAL)
        ]);
        this.form.get('passwordConfirm').addValidators([
            Validators.required,
            this.matchPassword
        ]);
    }

    private handleFormChange() {
      this.form.get('password').valueChanges.pipe(takeUntil(this.$signal))
        .subscribe(change => {
          if (this.form.controls.passwordConfirm.value) {
            if (this.form.controls.passwordConfirm.value !== change) {
              this.form.controls.passwordConfirm.setErrors({ 'noMatch': true });
            } else {
              this.form.controls.passwordConfirm.setErrors(null);
            }
          }
        });
    }

    private matchPassword(control: AbstractControl): ValidationErrors | null {
        const parent = control.parent.value;
        if (parent.password !== control.value) {
            return { 'noMatch': true };
        }
        return null;
    }

    private diffPrevPassword(control: AbstractControl): ValidationErrors | null {
        const parent = control.parent.value;
        if (control.value === parent.old) {
            return { 'samePassword': true };
        }
        return null;
    }

    private handleValidationChange(): void {
        this.form.statusChanges.subscribe(ch => {
            if (ch === 'VALID') {
                const newPassword = this.form.get('password').value;
                this.formGroup.get('Password').patchValue(newPassword);
                // `markAsDirty` necessario per permettere alla griglia di rilevare le modifiche
                this.formGroup.get('Password').markAsDirty();
            } else if (!!this.oldPassword && !this.isNewItem) {
                this.formGroup.get('Password')?.patchValue(cloneDeep(this.oldPassword));
            }
        });
    }

    private computePwdError() {
        let err = this.form.get('password').errors;
        if (!err)
            this.pwdError.nativeElement.innerText = '';
        else if (err.required)
            this.pwdError.nativeElement.innerText = this.transloco.translate('RequiredPassword');
        else if (err.samePassword && !this.isNewItem)
            this.pwdError.nativeElement.innerText = this.transloco.translate('prof.NecessariaPasswordDiversaPrecedente');
        else {
            this.pwdError.nativeElement.innerText = this.transloco.translate('prof.RequisitiPasswordCompleti');
        }
    }

    private computePwdConfirmError() {
        let err = this.form.get('passwordConfirm').errors;
        if (!err)
            this.pwdConfirmError.nativeElement.innerText = '';
        else if (err.required)
            this.pwdConfirmError.nativeElement.innerText = this.transloco.translate('RequiredPassword');
        else
            this.pwdConfirmError.nativeElement.innerText = this.transloco.translate('PasswordDiConfermaNonCorrisponde');
    }

}
