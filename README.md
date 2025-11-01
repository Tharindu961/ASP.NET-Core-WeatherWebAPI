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

After adding those classes and injected as a service with interfaces in program.cs file.(Registerd the services)

Then i started integration part with OpenWeather API and i added to API KEY in appsettings.json file, And i created the HttpClient
inside of the manager class and i have created model response class returned by OpenWeather API(mapped data with existing model class).

So I have added API to get the city weather all around via entering the city
Route -> GET/api/weather/{city}
In this API i have checked already added city weather data whether data exist or not, so i have check if data exist updated within 30 minutes 
it return already exist data, if not existing data updated over 30 minutes i have called to the api and get the response and updated the existing record
with new data and update, so there don't have have any exisiting record i insert to the database.

In here i have applied the validation messages to the necessory places that i have to validate
1)If the response code is not sucesses i have applied the validation
2)API return data after converting readable type to object, converted object is null i have applied the validation.
