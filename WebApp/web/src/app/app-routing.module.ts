/** @format */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainRouteComponent } from './routes/main-route/main-route.component';
import { SummonerRouteComponent } from './routes/summoner-route/summoner-route.component';
import { ConfirmRouteComponent } from './routes/confirm-route/confirm-route.component';
import { DetailsRouteComponent } from './routes/details-route/details-route.component';
import { AboutRouteComponent } from './routes/about-route/about-route.component';

const routes: Routes = [
  {
    path: '',
    component: MainRouteComponent,
  },
  {
    path: 'summoner/:summonerName',
    component: SummonerRouteComponent,
  },
  {
    path: 'summoner/:summonerName/confirm',
    component: ConfirmRouteComponent,
  },
  {
    path: 'summoner/:summonerName/details',
    component: DetailsRouteComponent,
  },
  {
    path: 'about',
    component: AboutRouteComponent,
  },
  {
    path: '**',
    redirectTo: '/',
    pathMatch: 'full',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
