using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TestDIntegration.Fixtures;

namespace TestDIntegration
{
    public class RecettesControllerTest : IntegrationTest
    {
        public RecettesControllerTest(APiWebApplicationFactory webApi) : base(webApi)
        {
        }

        [Fact]
        public async Task GetAllRecettes_Authenticated_ReturnsListOfRecette()
        {
            // Arrange

        await Login("fredon", "fredon");
            List<RecetteDTO> recettesExpected = new List<RecetteDTO>()
            {
                new()
                {
                    Id = 83,
                    nom = "Soupe antillaise",
                    temps_preparation = TimeSpan.FromMinutes(30),
                    temps_cuisson = TimeSpan.FromMinutes(120),
                    difficulte = 3,
                    photo = "/images/recettes/c613e516-ee0e-4d79-8d3b-aee896650031.png"
                },

                new()
                {
                    Id = 85,
                    nom = "Soupe au boeuf et légumes",
                    temps_preparation = TimeSpan.FromMinutes(25),
                    temps_cuisson = TimeSpan.FromMinutes(120),
                    difficulte = 2,
                    photo = "/images/recettes/7016568c-5bbc-4d7f-8db0-9e4083bf56e1.png"
                },

                new()
                {
                    Id = 79,
                    nom = "soupe vietnamienne",
                    temps_preparation = TimeSpan.FromMinutes(30),
                    temps_cuisson = TimeSpan.FromMinutes(150),
                    difficulte = 3,
                    photo = "/images/recettes/2a1ed09a-8f08-4014-a15c-4a648350c16e.png"
                }

            };

            
            // Act
            var list = await httpClient.GetFromJsonAsync<List<RecetteDTO>>("api/recettes");
            //var response = await httpClient.GetAsync("api/recettes");

            // Assert
            Assert.NotNull(list);
            Assert.Equivalent(recettesExpected, list);
        }
    }
}
