import { AfterViewInit, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ErrorTree, GiasFormErrorVisualizerService } from './gias-form-error-visualizer.service';

@Component({ // TODO include this in kendo grid 
  standalone: false,
  selector: 'gias-form-error-visualizer',
  templateUrl: './gias-form-error-visualizer.component.html',
  styleUrls: ['./gias-form-error-visualizer.component.css']
})
export class GiasFormErrorVisualizerComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() form: FormGroup;
  public signal$: Subject<void> = new Subject();

  public errorTree: ErrorTree[];

  public windowS = window;
  constructor(private formErrorVisualizerService: GiasFormErrorVisualizerService,
    private route: ActivatedRoute) { }

  ngAfterViewInit(): void {
    try {
      if (this.fragment) {
        document.querySelector('#' + this.fragment).scrollIntoView();
      }
    } catch (e) { }
  }

  ngOnDestroy(): void {
    this.signal$.next();
    this.signal$.complete();
  }

  fragment: string;

  ngOnInit(): void {
    this.formErrorVisualizerService.resetFormError();
    this.formErrorVisualizerService.currentErrors.pipe(takeUntil(this.signal$)).subscribe((errors) => {
      this.errorTree = errors;
    });
    this.route.fragment.subscribe(fragment => { this.fragment = fragment; });
  }

  goToAnchorTag(idtogo: string) {
    let el = document.getElementById(idtogo);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }
}
