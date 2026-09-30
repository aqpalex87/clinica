import { Injectable } from '@angular/core';
import {Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class LoadingService {

  private loading$ : Subject<boolean>;

  constructor() {
    this.loading$ = new Subject();
  }

  loading(data: boolean) {
    this.loading$.next(data)
  }

  getLoading(): Observable<boolean> {
    return this.loading$.asObservable()
  }

}
