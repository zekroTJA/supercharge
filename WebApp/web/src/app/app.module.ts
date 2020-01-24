/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RestAPIService } from './services/api/rest.service';
import { HeaderComponent } from './components/header/header.component';
import { DropDownComponent } from './components/drop-down/drop-down.component';
import { MainRouteComponent } from './routes/main-route/main-route.component';
import { SummonerRouteComponent } from './routes/summoner-route/summoner-route.component';

import { ChartsModule } from 'ng2-charts';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    DropDownComponent,
    MainRouteComponent,
    SummonerRouteComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, ChartsModule],
  providers: [{ provide: 'APIService', useClass: RestAPIService }],
  bootstrap: [AppComponent],
})
export class AppModule {}
