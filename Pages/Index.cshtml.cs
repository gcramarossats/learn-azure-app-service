using learn_azure_webapp.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data.Common;

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