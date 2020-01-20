# masterypointsstats *(provisional project name)*

> This project is currently in a very early development state.

## Information & Motivation

This web application allows collecting and analyzing League of Legends champion mastery points stats of summoners who have registered their account on the page to be tracked. This allows keeping track and analyzing your porgress on champion masteries and being able to observe your progerssion.

This porject is strongly inspired by [masterypoints.com](https://masterypoints.com), which provides a detailed insight about your chamion mastery points and world wide rankings, but leaks of providing detailed analysis and tracking of the progress of your mastery points.

## Structure

This project is split up in seperate "microservices" and access layers for better maintainability and deploy.

Every services is provided and deployed as docker image so that the whole server stack can be deployed with the provided docker-compose configuration.

### [Crawler](Crawler)

This .NET Core (3.1) console application crawls, depending on the registered accounts in the database, mastery point data of these users from the League of Legends API and saves them in the database.

### [DatabaseAccessLayer](DatabaseAccessLayer)

This is a .NET Standard 2.1 Class Library which acts as access layer for the database and provides the Entity Framework database context and the database access class which can be implemented via dependency injection.

### [RestAPI](RestAPI)

This ASP.NET Core (3.1) application provides a REST API gateway to the data provided by the database which is used by the front end web application.

### [RiotAPIAccessLayer](RiotAPIAccessLayer)

This .NET Standard 2.1 Class Library provides an access layer to the Riot League Of Legends REST API which is used by the Crawler and REST API application.

### [Shared](Shared)

This is another .NET Standard 2.1 Class Library which provides simple general purpose shared functionalities which are used by other class libraries or applications.
