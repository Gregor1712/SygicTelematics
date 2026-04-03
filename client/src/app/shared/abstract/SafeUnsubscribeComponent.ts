import { OnDestroy, Directive } from '@angular/core';
import { Subject } from 'rxjs';

@Directive()
export abstract class SafeUnsubscribeComponent implements OnDestroy {
  protected unsubscribe$: Subject<void> = new Subject<void>();

  ngOnDestroy() {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
