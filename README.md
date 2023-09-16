# learn-azure-webapp
A simple application that has been developed in order to demonstrate how to use Azure App service with a database and manged identities.

With this simple application it is possible to:

- connect to a database using username/password authentication (not recommended) or managed identity (user assigned or system assigned - strongly recommended)
- set a background color via environment variable in order to show some release deployment concepts such as blue/green deployment or canary deployment by using deployment slots and deployment slot specific environment variable
- use Application Insights in order to reactively review application execution data to determine the cause of an incident

# Environment variables used by the application

| Variable Name                                          | Description                                                                                         |
| ------------------                                     | ---------------------------------------------------------------                                     |
| APPLICATION_UI_BACKGROUNDCOLOR                         | If set, set the background color of the website. The value should be a valid CSS color value        |
| APPLICATION_CORE_DBCONNECTIONSTRINGENVIRONMENTVARIABLE | The name of the connection string created in the connection string section. The vaue a prefix plus the name of the connection string. The prefix should be <ul><li>For Azure SQL Database: SQLAZURECONNSTR</li><li>For Azure Database for MySQL: MYSQLCONNSTR_<li>For Azure Database for PostgreSQL: POSTGRESQLCONNSTR_</li></ul>                           |
| APPLICATION_CORE_MANAGEDIDENTITYCLIENTID               | The client id associated to the user assigned managed identity. If the value is empty, it takes the system assigned. If used, the **APPLICATION_CORE_DBCONTEXTURL** environment variable is mandatory  |
| APPLICATION_CORE_DBCONTEXTURL                          | Contains the context to assign to the managed identity used by the connection string, if set. Values are <br /><ul><li>For Azure SQL Database: https://database.windows.net/.default</li><li>For Azure Database for MySQL: https://ossrdbms-aad.database.windows.net/.default</li><li>For Azure Database for PostgreSQL: https://ossrdbms-aad.database.windows.net/.default</li></ul> |

