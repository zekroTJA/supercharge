/** @format */

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MainRouteComponent } from './routes/main-route/main-route.component';
import { SummonerRouteComponent } from './routes/summoner-route/summoner-route.component';

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
