using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace TestDIntegration.Fixtures
{
    public abstract class IntegrationTest : IClassFixture<APiWebApplicationFactory>
    {
        public HttpClient httpClient { get; set; }
        private IConfiguration _configuration { get; set; }
        private string _connectionString { get; set; }

        public IntegrationTest(APiWebApplicationFactory webApi)
        {
            // instancier le client
            httpClient = webApi.CreateClient();
            _configuration = webApi.Configuration;
            _connectionString = _configuration.GetSection("DatabaseSettings").GetValue<string>("ConnectionString");
            // Startégie pour fixer ma basse de données
            // Drop la base de données
            DownDatabase();
            // Relancer jeu de données
            UpDatabase();
        }

        public async Task Login(string username, string password)
        {
            // Appel API pour s'authentifier

            var httpResponse = await httpClient.PostAsJsonAsync<LoginDTO>("api/Authentication/Login", new LoginDTO
            {
                Username = username,
                Password = password
            });

            if (httpResponse.IsSuccessStatusCode)
            {
                var jwtDTO = await httpResponse.Content.ReadFromJsonAsync<JwtDTO>();
                httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtDTO.Token);

            }
            else
                Assert.True(false, "Echec de l'authentification");

            // Récupérer le token
            // Ajouter le token dans le header du client
        }

        public void UpDatabase()
        {

            using (IDbConnection con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                string requeteSQL = File.ReadAllText("CreateDatabase.sql");
                var commande = con.CreateCommand();
                commande.CommandText = requeteSQL;
                commande.ExecuteNonQuery();

                con.Close();
            }
            
            // Relancer jeu de données
        }

        public void DownDatabase()
        {
            // Drop la base de données
            using (IDbConnection con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                string requeteSQL = "DROP SCHEMA IF EXISTS public CASCADE;";
                var commande = con.CreateCommand();
                commande.CommandText = requeteSQL;
                commande.ExecuteNonQuery();

                con.Close();
            }
            
        }
    }
}
