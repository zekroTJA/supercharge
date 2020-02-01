/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RestAPIService } from './services/api/rest.service';
import { HeaderComponent } from './components/header/header.component';
import { DropDownComponent } from './components/drop-down/drop-down.component';
import { MainRouteComponent } from './routes/main-route/main-route.component';
import { SummonerRouteComponent } from './routes/summoner-route/summoner-route.component';

import { ChartsModule } from 'ng2-charts';
import { DateTimePipe } from './pipes/date-time.pipe';
import { ConfirmRouteComponent } from './routes/confirm-route/confirm-route.component';
import { DetailsRouteComponent } from './routes/details-route/details-route.component';
import { TagSelectComponent } from './components/tag-select/tag-select.component';
import { AboutRouteComponent } from './routes/about-route/about-route.component';
import { LoadingBarComponent } from './components/loading-bar/loading-bar.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    DropDownComponent,
    MainRouteComponent,
    SummonerRouteComponent,
    DateTimePipe,
    ConfirmRouteComponent,
    DetailsRouteComponent,
    TagSelectComponent,
    AboutRouteComponent,
    LoadingBarComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ChartsModule,
    FormsModule,
  ],
  providers: [{ provide: 'APIService', useClass: RestAPIService }],
  bootstrap: [AppComponent],
})
export class AppModule {}
