using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using TestDIntegration.Fixtures;

namespace TestDIntegration
{
    // Définition explicite d'une collection non parallélisée (sûr même si l'assembly le désactive déjà)
    [CollectionDefinition("IntegrationTests", DisableParallelization = true)]
    public class IntegrationTestsCollectionDefinition { }

    [Collection("IntegrationTests")]
    public class RecettesControllerUpdateIntegrationTest : IntegrationTest
    {
        public RecettesControllerUpdateIntegrationTest(APiWebApplicationFactory webApi) : base(webApi)
        {
        }

        [Fact(DisplayName = "PUT api/recettes/{id} — modifie une recette et persiste la modification")]
        public async System.Threading.Tasks.Task UpdateRecette_EndToEnd_UpdatesAndPersistsAsync()
        {
            // Arrange
            await Login("fredon", "fredon");

            int recetteId = 83; // id présent dans le script d'initialisation CreateDatabase.sql

            var updateDto = new UpdateRecetteDTO
            {
                Id = recetteId,
                nom = "Soupe antillaise - IT",
                temps_preparation = TimeSpan.FromMinutes(35),
                temps_cuisson = TimeSpan.FromMinutes(130),
                difficulte = 2,
                etapes = new List<UpdateEtapeDTO>
                {
                    new UpdateEtapeDTO
                    {
                        id_recette = recetteId,
                        numero = 1,
                        titre = "Préparation (IT)",
                        texte = "Laisser mijoter 10 minutes de plus."
                    }
                },
                ingredients = new List<IngredientDTO>
                {
                    new IngredientDTO { id = 1, nom = "Sel", quantite = "2 pincées" }
                },
                categories = new List<CategorieDTO>
                {
                    new CategorieDTO { id = 1, nom = "Plat" }
                },
                photo = null
            };

            // Act
            var putResponse = await httpClient.PutAsJsonAsync($"api/recettes/{recetteId}", updateDto);

            // Assert - réponse du PUT
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var returned = await putResponse.Content.ReadFromJsonAsync<RecetteDTO>();            
            Assert.NotNull(returned);
            Assert.Equal(recetteId, returned.Id);
            Assert.Equal(updateDto.nom, returned.nom);
            Assert.Equal(updateDto.difficulte, returned.difficulte);
            Assert.Equal(updateDto.etapes.Count, returned.etapes.Count);
            Assert.Equal(updateDto.ingredients.Count, returned.ingredients.Count);
            Assert.Equal(updateDto.categories.Count, returned.categories.Count);
            
            // Vérifier la persistance via GET
            var getResponse = await httpClient.GetAsync($"api/recettes/{recetteId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var getDto = await getResponse.Content.ReadFromJsonAsync<RecetteDTO>();
            Assert.NotNull(getDto);
            Assert.Equal(updateDto.nom, getDto.nom);
            Assert.Equal(updateDto.difficulte, getDto.difficulte);
            Assert.Equal(updateDto.etapes.Count, getDto.etapes.Count);
            Assert.Equal(updateDto.ingredients.Count, getDto.ingredients.Count);
            Assert.Equal(updateDto.categories.Count, getDto.categories.Count);
        }
    }
}
