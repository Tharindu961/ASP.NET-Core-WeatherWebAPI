# ASP.NET-Core-C-Web-API
This is ASP.NET Core C# Web API App for integrate with public weather api

First i have created a git repo and create web api project push to the branch as initial commit.
Then i have selected 3rd party public api for the integration as OpenWeatherMap.
Then i have tested this open api with Postman with applying API_KEY provided by website.(https://openweathermap.org/api)

After that i decided what are data i should database in the table(columns of the table)
First i have created a DB(WeatherDb) and WeatherRecord Table
Columns -> Id, City, Country, Temperature, Description, Humidity, Pressure, WindSpeed, Cloudiness, LastUpdated

I have added the connection string appsettings.json and installed sqlclient package to project
Then i have created model class  of WeatherRecord

After that i have created the folder structuture of the project
Controllers -> Managers -> Services and i created intefaces classes as well.

After adding those classes and injected as a service with interfaces in program.cs file.