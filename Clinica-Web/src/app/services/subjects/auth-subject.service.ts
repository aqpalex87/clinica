import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AuthSubjectService{

    private loadedUserSubject: Subject<boolean> = new Subject<boolean>();
    public isUserLoaded$: Observable<boolean> = this.loadedUserSubject.asObservable();

    constructor() {    
        
    }

    public setLoadedUser(value: boolean){
        this.loadedUserSubject.next(value);
    }

}