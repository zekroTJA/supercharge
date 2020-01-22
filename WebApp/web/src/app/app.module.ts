/** @format */

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RestAPIService } from './services/api/rest.service';
import { HeaderComponent } from './components/header/header.component';
import { DropDownComponent } from './components/drop-down/drop-down.component';

@NgModule({
  declarations: [AppComponent, HeaderComponent, DropDownComponent],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule],
  providers: [{ provide: 'APIService', useClass: RestAPIService }],
  bootstrap: [AppComponent],
})
export class AppModule {}
