# RightmoveDownloader

Given a location and search criteria finds properties and puts them in google sheet for review


## Prerequisites
- Google API Console Project (https://developers.google.com/api-client-library/dotnet/get_started)
  - Service Account
  - Api Key (may require billing account, there is a free tier)
- Google Spreadsheet with 2 sheets: "properties", "times"
  - Grant access to service account
  
## Run from code
- Save service account .json into google-service-account.json
- Put the rest of the settings in appsettings.json or enviroment variables
  - Google Api Key - it will look like *AbcDeFghuJKLMN0pQR0Tz0ABADASQWEADADADc*
  - Rightmove locationIdentifier - get it from a url on rightmove.co.uk by searching your desired location - it will look like this: *POSTCODE%5E1274909*
  - Destination latitude, longitude for the same location as above - can get them a url in google maps - it will look like this: *51.5165114,-0.1239118*
  - Google spreadsheetId - get it from a url of an open spreadsheet - it will look like this *1_abcDefGhIj-kLMNoPQ_RsTU0_vw0xY000zabCdefg0*

## Run from Docker image
### Windows/Powershell
~~~~
docker run --name rightmove `
-p 321:80 -d --restart unless-stopped `
-v ABSOLUTE_PATH_TO\google-service-account.json:/app/google-service-account.json `
-e LocationIdentifier="POSTCODE%5E1274909" `
-e ToLocation="51.5165114,-0.1239118" `
-e DownloadPropertiesSchedule="0 4 * * *" `
-e DownloadDistancesSchedule="0 6 * * *" `
-e Radius=20 `
-e MinBedrooms=2 `
-e MaxBedrooms=3 `
-e MinPrice=1400 `
-e MaxPrice=1800 `
-e Channel=RENT ` # RENT|BUY
-e GoogleMapsApiKey=AbcDeFghuJKLMN0pQR0Tz0ABADASQWEADADADc `
-e GoogleAppName=rightmove `
-e GoogleSpreadsheetId=1_abcDefGhIj-kLMNoPQ_RsTU0_vw0xY000zabCdefg0 `
-e SENTRY_DSN="https://abcdef1234567890abcdef1234567890@sentry.io/1234567" `
mikeon/rightmove
~~~~

### Linux/Bash
~~~~
docker run --name rightmove \
-p 321:80 -d --restart unless-stopped \
-v ABSOLUTE_PATH_TO/google-service-account.json:/app/google-service-account.json \
-e LocationIdentifier="POSTCODE%5E1274909" \
-e ToLocation="51.5165114,-0.1239118" \
-e DownloadPropertiesSchedule="0 4 * * *" \
-e DownloadDistancesSchedule="0 6 * * *" \
-e Radius=20 \
-e MinBedrooms=2 \
-e MaxBedrooms=3 \
-e MinPrice=1400 \
-e MaxPrice=1800 \
-e Channel=RENT \ # RENT|BUY
-e GoogleMapsApiKey=AbcDeFghuJKLMN0pQR0Tz0ABADASQWEADADADc \
-e GoogleAppName=rightmove \
-e GoogleSpreadsheetId=1_abcDefGhIj-kLMNoPQ_RsTU0_vw0xY000zabCdefg0 \
-e SENTRY_DSN="https://abcdef1234567890abcdef1234567890@sentry.io/1234567" \
mikeon/rightmove
~~~~

### Optional
By default Hangfire will use InMemoryStorage. If you mount a folder to /app/Hangfire LiteDB will be used instead.
~~~~
-v ABSOLUTE_PATH_TO/Hangfire:/app/Hangfire
~~~~
