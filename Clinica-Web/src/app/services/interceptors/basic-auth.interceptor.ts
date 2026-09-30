import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import { SecurityService } from '../security.service';

@Injectable()
export class BasicAuthInterceptor implements HttpInterceptor {
  constructor(private securityServ:SecurityService) { }

  intercept(request: HttpRequest<null>, next: HttpHandler): Observable<HttpEvent<null>> {
    if (!request.headers.has('Authorization')) {
      const auth = this.securityServ.MCAuthorizationHeaderValue();
      if (auth) {
        request = request.clone({
          setHeaders: {
            Authorization: auth,
          },
        });
      }
    }

    return next.handle(request);
  }
}
