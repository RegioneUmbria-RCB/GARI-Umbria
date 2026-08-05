import { Component, Input, OnInit, SecurityContext, Type } from "@angular/core";
import { DomSanitizer, SafeResourceUrl } from "@angular/platform-browser";
import { CustomComponent } from 'gias-kendo-grid';

@Component({
  standalone: false,
  selector: 'app-img64',
  templateUrl: './img-base64.component.html',
  styleUrls: ['./img-base64.component.css'],
  providers: []
})
export class ImgBase64Component implements OnInit, CustomComponent {
  @Input() input: string;
  @Input() edit: boolean;
  @Input() field: string;
  public imagePath: SafeResourceUrl;

  show: boolean;
  constructor(private _sanitizer: DomSanitizer) {
  }

  ngOnInit(): void {
    if (this.input[this.field] !== '') {
      //this.imagePath = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,' + this.input[this.field]);
      this.imagePath = this._sanitizer.sanitize(
        SecurityContext.URL,
        'data:image/jpg;base64,' + this.input[this.field]
      )
      this.show = true;
    } else {
      this.show = false;
    }
  }

}
