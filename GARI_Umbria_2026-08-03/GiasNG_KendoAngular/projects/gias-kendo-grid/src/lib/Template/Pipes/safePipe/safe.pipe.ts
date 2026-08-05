import { Pipe, PipeTransform, SecurityContext } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Pipe({
    standalone: false, name: 'safe' })
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }
  transform(url: string): SafeResourceUrl {
    const sanitizedUrl = this.sanitizer.sanitize(
      SecurityContext.URL,
      url
    );
    const result = this.sanitizer.bypassSecurityTrustResourceUrl(sanitizedUrl);
    return result;
  }
}
