import { OverlayRef } from "@angular/cdk/overlay";
import { ComponentPortal } from "@angular/cdk/portal";
import { Injectable, InjectionToken } from "@angular/core";
import { DynamicOverlay } from "./dynamic-overlay";
import { BehaviorSubject, Observable } from "rxjs";
import { GiasWaitFrameComponent } from "../gias-wait-frame/gias-wait-frame.component";
import { LoadingComponentObject } from "./models";

export const LOADING_TOKEN = new InjectionToken<LoadingService>('app.loading.service');

@Injectable()
export class LoadingService {
  private timeout: number = 500;
  private loadingObject: LoadingComponentObject = { isLoading: false, message: '', component: null };

  private isLoadingSource = new BehaviorSubject(this.loadingObject);
  currentIsLoading: Observable<LoadingComponentObject> = this.isLoadingSource.asObservable();

  private overlayComponentRef: OverlayRef = null;
  private isAlreadyLoading = false;

  constructor(private dynamicOverlay: DynamicOverlay) {
    this.isLoadingSource.subscribe(loadingParam => this.handleLoading(loadingParam));
  }

  private handleLoading(loadingParam: LoadingComponentObject) {
    if (loadingParam) {
      if (loadingParam.isLoading) {
        setTimeout(() => {
          this.handleShowLoading(loadingParam);
        }, this.timeout);
      } else {
        this.handleHideLoading(loadingParam)
      }
    }
  }

  private handleShowLoading(loadingParam: LoadingComponentObject) {
    const isLoading = this.isLoadingSource.getValue();

    if (isLoading.isLoading && !this.isAlreadyLoading) {
      this.overlayComponentRef = this.dynamicOverlay.createWithDefaultConfig(loadingParam.component.nativeElement);

      const spinnerOverlayPortal = new ComponentPortal(GiasWaitFrameComponent);
      const component = this.overlayComponentRef.attach(spinnerOverlayPortal);
      if (isLoading.message && isLoading.message != '') {
        component.instance.message = isLoading.message;
      }
      this.isAlreadyLoading = true;

    }
  }

  private handleHideLoading(loadingParam: LoadingComponentObject) {
    if (!!this.overlayComponentRef) {
      this.isAlreadyLoading = false;
      this.overlayComponentRef.detach();
    }
  }

  set_isLoading(loadingObject: LoadingComponentObject) {
    this.isLoadingSource.next(loadingObject);
  }

  get_isLoading(): LoadingComponentObject {
    return this.isLoadingSource.getValue();
  }
}