using Azure.Core;
using Azure.Identity;
using learn_azure_webapp.DataModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace learn_azure_webapp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string dataSource = "memory";

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            List<TableRowItem> capitalCountriesList;

            try
            {
                capitalCountriesList = GetDatabaseCountries();
                dataSource = "database";
            }
            catch (Exception exc)
            {
                _logger.LogError("Error retrieving countries list from database. Error is " + exc.Message);
                capitalCountriesList = GetOfflineCapitalCountries();
            }

            ViewData["dataSource"] = dataSource;
            ViewData["capitalCountriesList"] = capitalCountriesList;
        }

        private List<TableRowItem> GetDatabaseCountries() 
        {
            _logger.LogInformation("Getting countries list from the database");
            List<TableRowItem> rowItems = new List<TableRowItem>();

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATION_CORE_DBCONNECTIONSTRINGENVIRONMENTVARIABLE")))
            {
                string dbConnectionStringEnvironmentVariable = Environment.GetEnvironmentVariable("APPLICATION_CORE_DBCONNECTIONSTRINGENVIRONMENTVARIABLE");

                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(dbConnectionStringEnvironmentVariable)))
                {
                    string connectionString = Environment.GetEnvironmentVariable(dbConnectionStringEnvironmentVariable);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand("SELECT * FROM capitalcities", connection);
                        if (Environment.GetEnvironmentVariable("APPLICATION_CORE_MANAGEDIDENTITYCLIENTID") != null)
                        {
                            string managedIdentityClientId = Environment.GetEnvironmentVariable("APPLICATION_CORE_MANAGEDIDENTITYCLIENTID");
                            DefaultAzureCredential defaultAzureCredential;
                            if (managedIdentityClientId.Trim() == "")
                            {
                                defaultAzureCredential = new DefaultAzureCredential();
                                _logger.LogInformation("Using the system assigned Azure managed identity of the application");
                            }
                            else
                            {
                                defaultAzureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = managedIdentityClientId });
                                _logger.LogInformation("Using the user assigned Azure managed identity with client id" + managedIdentityClientId);
                            }

                            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATION_CORE_DBCONTEXTURL")))
                            {
                                AccessToken token = defaultAzureCredential.GetToken(new TokenRequestContext(new[] { Environment.GetEnvironmentVariable("APPLICATION_CORE_DBCONTEXTURL") }));
                                connection.AccessToken = token.Token;
                                _logger.LogError("The managed identity has been set successfully. Connecting with the selected managed identity");
                            }
                            else
                            {
                                _logger.LogError("APPLICATION_CORE_DBCONTEXTURL environment variable not set. You need to set this environment variable in order to get an acess token that can be used for the chosen database");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("To connect to the database using a managed identity, set the APPLICATION_CORE_MANAGEDIDENTITYCLIENTID environment variable");
                        }
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rowItems.Add(new TableRowItem
                                {
                                    Capital = reader["capital"].ToString(),
                                    Country = reader["country"].ToString()
                                });
                            }

                            connection.Close();
                        }
                    }
                }
                else
                {
                    throw new ApplicationException("The connection string was not found.");
                }
            }
            else
            {
                throw new ApplicationException("The APPLICATION_CORE_DBCONNECTIONSTRINGENVIRONMENTVARIABLE environment variable is not defined");
            }

            return rowItems;
        }

        private List<TableRowItem> GetOfflineCapitalCountries()
        {
            _logger.LogInformation("Getting countries list from the memory");
            return new List<TableRowItem>()
            {
                new TableRowItem
                {
                    Capital = "Paris",
                    Country = "France"
                },
                new TableRowItem
                {
                    Capital = "Rome",
                    Country = "Italy"
                },
                new TableRowItem
                {
                    Capital = "Berlin",
                    Country = "Germany"
                }
            };
        }
    }
}