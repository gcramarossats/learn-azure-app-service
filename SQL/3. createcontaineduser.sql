-- replace <your-app-service-name> with your user managed identity name

CREATE USER <your-app-service-name> FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER <your-app-service-name>;

-- ALTER ROLE db_datawriter ADD MEMBER <your-app-service-name>;