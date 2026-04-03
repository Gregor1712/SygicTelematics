import { Routes } from '@angular/router';
import { HomeComponent } from './home/home';
import { ProductsComponent } from './products/products';
import { Login } from './account/login/login';
import { Register } from './account/register/register';
import { CpuComponent } from './cpu/cpu';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'products', component: ProductsComponent },
  { path: 'cpu', component: CpuComponent },
  { path: 'account/login', component: Login},
  { path: 'account/register', component: Register},
];
