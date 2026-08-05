import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { MasterService } from './master.service';

interface Css {
    name: string;
    src: string;
}

declare let document: any;

@Injectable({ providedIn: 'root' })
export class CssService {
    private css_s: any = {};

    private CssStore: Css[] = [];

    constructor(private masterService: MasterService,
        @Inject(DOCUMENT) private document: Document) {

        this.CssStore.push({
            name: 'kendoCommonBootstrap',
            src: this.masterService.link_GiasBase + '/kendoui/2021.1.119/styles/kendo.common-bootstrap.min.css'
        });

        this.CssStore.push({
            name: 'kendoBootstrap',
            src: this.masterService.link_GiasBase + '/kendoui/2021.1.119/styles/kendo.bootstrap.min.css'
        });

        this.CssStore.push({
            name: 'Gias_Kendo',
            src: this.masterService.link_GiasBase + '/kendoui/StylesGiasKendo/2021.1.119/Gias_Kendo.css'
        });

        this.CssStore.forEach((css: any) => {
            this.css_s[css.name] = {
                loaded: false,
                src: css.src
            };
        });
    }

    load(...css_s: string[]) {
        return new Promise(async (resolve, reject) => {
            const arrProm = new Array();
            if (css_s.length>0){
                css_s.forEach(
                    async (css) => {
                        arrProm.push(this.loadCss(css));
                    }
                );
                const resp = await Promise.all(arrProm);
            }
            resolve(true);
        });
    }

    loadCss(name: string) {
        return new Promise((resolve, reject) => {
            // resolve if already loaded
            if (this.css_s[name].loaded) {
                resolve({ css: name, loaded: true, status: 'Already Loaded' });
            } else {
                // load css
                const css = document.createElement('link');
                css.type = 'text/css';
                css.rel = 'stylesheet';
                css.href = this.css_s[name].src;
                css.id = name;
                if (css.readyState) {  // IE
                    css.onreadystatechange = () => {
                        if (css.readyState === 'loaded' || css.readyState === 'complete') {
                            css.onreadystatechange = null;
                            this.css_s[name].loaded = true;
                            resolve({ css: name, loaded: true, status: 'Loaded' });
                        }
                    };
                } else {  // Others
                    css.onload = () => {
                        this.css_s[name].loaded = true;
                        resolve({ css: name, loaded: true, status: 'Loaded' });
                    };
                }
                css.onerror = (error: any) => resolve({ css: name, loaded: false, status: 'Loaded' });
                document.getElementsByTagName('head')[0].appendChild(css);
            }
        });
    }

    /**
     * Converts a relative URL to an absolute URL to prevent PRSSI attacks
     * @param relativeUrl The relative URL to convert
     * @returns The absolute URL
     */
    private getAbsoluteUrl(relativeUrl: string): string {
        // If the URL is already absolute (starts with http:// or https://), return it as is
        if (relativeUrl.startsWith('http://') || relativeUrl.startsWith('https://')) {
            return relativeUrl;
        }

        // Get the current base URL from the base tag or window.location
        let baseUrl = '';
        const baseElement = this.document.querySelector('base');
        
        if (baseElement && baseElement.href) {
            baseUrl = baseElement.href;
        } else {
            // Fallback to window.location if no base tag is found
            baseUrl = window.location.origin + window.location.pathname;
            // Remove the filename from the path to get the directory
            const lastSlashIndex = baseUrl.lastIndexOf('/');
            if (lastSlashIndex !== -1) {
                baseUrl = baseUrl.substring(0, lastSlashIndex + 1);
            }
        }

        // Ensure baseUrl ends with a slash
        if (!baseUrl.endsWith('/')) {
            baseUrl += '/';
        }

        // Remove leading slash from relativeUrl if present to avoid double slashes
        if (relativeUrl.startsWith('/')) {
            relativeUrl = relativeUrl.substring(1);
        }

        return baseUrl + relativeUrl;
    }

    loadStyle(styleName: string, id: string) {
        const head = this.document.getElementsByTagName('head')[0];

        const themeLink = this.document.getElementById(
            id
        ) as HTMLLinkElement;
        
        // Convert to absolute URL to prevent PRSSI attacks
        const absoluteUrl = this.getAbsoluteUrl(styleName);
        
        if (themeLink) {
            themeLink.href = absoluteUrl;
        } else {
            const style = this.document.createElement('link');
            style.id = id;
            style.rel = 'stylesheet';
            style.href = absoluteUrl;

            head.appendChild(style);
        }
    }

}
