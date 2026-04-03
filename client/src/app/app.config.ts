import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from "@angular/common/http";

import { routes } from './app.routes';
import { API_BASE_URL } from "./service/api-client";
import { jwtInterceptor } from './interceptors/jwt-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    //provideHttpClient(),
    //provideHttpClient(withInterceptors([errorInterceptor, jwtInterceptor, loadingInterceptor])),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), {provide: API_BASE_URL, useValue: 'http://localhost:5100'}
  ]
};
