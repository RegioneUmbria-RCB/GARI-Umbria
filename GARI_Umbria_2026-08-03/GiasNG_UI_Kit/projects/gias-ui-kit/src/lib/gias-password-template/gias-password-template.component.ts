import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormGroup, FormGroupDirective, Validators } from "@angular/forms";

@Component({
  standalone: false,
  selector: 'gias-password-template',
  templateUrl: './gias-password-template.component.html',
  styleUrls: ['./gias-password-template.component.css']
})
export class GiasPasswordTemplateComponent implements OnInit, AfterViewInit {
  @ViewChild('textbox') textbox;
  @Input() name: string;
  @Input() placeholder: string;
  @Input() obligatory: boolean;
  @Input() isDisabled: boolean = false;
  @Input() giasFormControlName: string;
  @Input() parentGroup: FormGroup;
  @Input() maxlength: number;
  @Input() minlength: number;
  @Output() valueChange = new EventEmitter<string>();
  
  showPassword: boolean = false;
  iconClass : string = 'k-icon k-font-icon k-i-eye-slash k-i-preview-off';
  constructor(private rootFormGroup: FormGroupDirective) { }

  ngOnInit(): void {
    this.iconClass = 'k-icon k-font-icon k-i-eye-slash k-i-preview-off';
    if (!this.parentGroup)
      this.parentGroup = this.rootFormGroup?.form;
    this.addValidators();
    if (this.isDisabled)
      this.parentGroup.get(this.giasFormControlName).disable();
  }

  ngAfterViewInit(): void {
    this.textbox.input.nativeElement.type = 'password';
  }

  private addValidators() {
    if (this.obligatory) {
      this.parentGroup.get(this.giasFormControlName).addValidators(Validators.required);
    }
    if (this.minlength) {
      this.parentGroup.get(this.giasFormControlName).addValidators(Validators.minLength(this.minlength));
    }
    if (this.maxlength) {
      this.parentGroup.get(this.giasFormControlName).addValidators(Validators.maxLength(this.maxlength));
    }
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
    this.iconClass = this.showPassword ? 'k-icon k-font-icon k-i-eye k-i-preview' : 'k-icon k-font-icon k-i-eye-slash k-i-preview-off';
    this.textbox.input.nativeElement.type = this.showPassword ? 'text' : 'password';
  }
}
