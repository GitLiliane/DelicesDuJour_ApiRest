using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Categories;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Etapes;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Ingredients;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;
using DelicesDuJour_ApiRest.DataAccessLayer.Repositories.Recettes;
using DelicesDuJour_ApiRest.DataAccessLayer.Unit_of_Work;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DelicesDuJour_ApiRest.Tests.Services;

public class BiblioServiceTest
{
    #region Tests Gestion des Recettes

    [Fact]
    public async Task GetAllRecettesAsync_ReturnsAllRecettes()
    {
        // Arrange
        var expectedRecettes = new List<Recette>
        {
            new Recette { Id = 1, nom = "Recette1", difficulte = 1 },
            new Recette { Id = 2, nom = "Recette2", difficulte = 2 }
        };

        var uow = new FakeUoW(getAllRecettesResult: expectedRecettes);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetAllRecettesAsync();

        // Assert
        var recettes = result.ToList();
        Assert.Equal(2, recettes.Count);
        Assert.Equal("Recette1", recettes[0].nom);
        Assert.Equal("Recette2", recettes[1].nom);
    }

    [Fact]
    public async Task GetRecetteByIdAsync_ReturnsCompleteRecette_WhenFound()
    {
        // Arrange
        var recette = new Recette
        {
            Id = 10,
            nom = "Test",
            temps_preparation = TimeSpan.FromMinutes(15),
            temps_cuisson = TimeSpan.FromMinutes(30),
            difficulte = 2,
            photo = "/images/test.jpg"
        };

        var etapes = new List<Etape> { new Etape { id_recette = 10, numero = 1, titre = "Etape1", texte = "Faire" } };
        var ingredients = new List<Ingredient> { new Ingredient { id = 1, nom = "Sel", quantite = "1g" } };
        var categories = new List<Categorie> { new Categorie { id = 1, nom = "Dessert" } };

        var uow = new FakeUoW(
            getRecetteByIdResult: recette,
            getEtapesByIdRecetteResult: etapes,
            getIngredientsByIdRecetteResult: ingredients,
            getCategoriesByIdRecetteResult: categories
        );

        var service = new BiblioService(uow);

        // Act
        var result = await service.GetRecetteByIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Test", result.nom);
        Assert.Single(result.etapes);
        Assert.Single(result.ingredients);
        Assert.Single(result.categories);
        Assert.True(uow.CommitCalled);
    }

    [Fact]
    public async Task AddRecetteAsync_CreatesRecetteWithRelations()
    {
        // Arrange
        var newRecette = new Recette
        {
            nom = "New Recipe",
            temps_preparation = TimeSpan.FromMinutes(10),
            temps_cuisson = TimeSpan.FromMinutes(20),
            difficulte = 1,
            ingredients = new List<Ingredient> { new Ingredient { nom = "Sugar", quantite = "100g" } },
            etapes = new List<Etape> { new Etape { numero = 1, titre = "Step1", texte = "Do this" } },
            categories = new List<Categorie> { new Categorie { id = 1, nom = "Dessert" } }
        };

        var createdRecette = new Recette
        {
            Id = 123,
            nom = newRecette.nom,
            temps_preparation = newRecette.temps_preparation,
            temps_cuisson = newRecette.temps_cuisson,
            difficulte = newRecette.difficulte
        };

        var existingIngredients = new List<Ingredient>();

        var uow = new FakeUoW(
            createRecetteResult: createdRecette,
            getAllIngredientsResult: existingIngredients,
            createIngredientResult: new Ingredient { id = 1, nom = "Sugar" },
            createEtapeResult: new Etape { id_recette = 123, numero = 1, titre = "Step1", texte = "Do this" },
            addRecetteCategorieRelationshipResult: true
        );

        var service = new BiblioService(uow);

        // Act
        var result = await service.AddRecetteAsync(newRecette, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.Id);
        Assert.Equal("New Recipe", result.nom);
        Assert.Single(result.ingredients);
        Assert.Single(result.etapes);
        Assert.Single(result.categories);
        Assert.True(uow.CommitCalled);
    }

    [Fact]
    public async Task ModifyRecetteAsync_UpdatesRecetteWithRelations()
    {
        // Arrange
        var updateRecette = new Recette
        {
            Id = 5,
            nom = "Updated Recipe",
            temps_preparation = TimeSpan.FromMinutes(15),
            temps_cuisson = TimeSpan.FromMinutes(25),
            difficulte = 2,
            ingredients = new List<Ingredient> { new Ingredient { nom = "Salt", quantite = "5g" } },
            etapes = new List<Etape> { new Etape { numero = 1, titre = "UpdatedStep", texte = "Do that" } },
            categories = new List<Categorie> { new Categorie { id = 2, nom = "Plat" } }
        };

        var existingIngredients = new List<Ingredient>();

        var uow = new FakeUoW(
            getRecetteByIdResult: new Recette { Id = 5, nom = "Old", photo = null },
            getAllIngredientsResult: existingIngredients,
            deleteRecetteRelationsIngredientResult: true,
            createIngredientResult: new Ingredient { id = 2, nom = "Salt" },
            modifyRecetteResult: updateRecette,
            deleteEtapesRelationByIdRecetteResult: true,
            createEtapeResult: new Etape { id_recette = 5, numero = 1, titre = "UpdatedStep", texte = "Do that" },
            deleteRecetteRelationsResult: true,
            addRecetteCategorieRelationshipResult: true
        );

        var service = new BiblioService(uow);

        // Act
        var result = await service.ModifyRecetteAsync(updateRecette);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Updated Recipe", result.nom);
        Assert.Single(result.ingredients);
        Assert.Single(result.etapes);
        Assert.Single(result.categories);
        Assert.True(uow.CommitCalled);
    }

    [Fact]
    public async Task DeleteRecetteAsync_DeletesRecetteAndRelations_ReturnsTrue()
    {
        // Arrange
        var uow = new FakeUoW(
            deleteRecetteRelationsIngredientResult: true,
            deleteRecetteRelationsResult: true,
            deleteEtapesRelationByIdRecetteResult: true,
            deleteRecetteResult: true
        );

        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteRecetteAsync(10);

        // Assert
        Assert.True(result);
        Assert.True(uow.CommitCalled);
    }

    [Fact]
    public async Task DeleteRecetteAsync_ReturnsFalse_WhenDeletionFails()
    {
        // Arrange
        var uow = new FakeUoW(
            deleteRecetteRelationsIngredientResult: true,
            deleteRecetteRelationsResult: true,
            deleteEtapesRelationByIdRecetteResult: true,
            deleteRecetteResult: false
        );

        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteRecetteAsync(10);

        // Assert
        Assert.False(result);
        Assert.False(uow.CommitCalled);
    }

    #endregion

    #region Tests Gestion des Etapes

    [Fact]
    public async Task GetAllEtapesAsync_ReturnsAllEtapes()
    {
        // Arrange
        var expectedEtapes = new List<Etape>
        {
            new Etape { id_recette = 1, numero = 1, titre = "Etape1" },
            new Etape { id_recette = 1, numero = 2, titre = "Etape2" }
        };

        var uow = new FakeUoW(getAllEtapesResult: expectedEtapes);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetAllEtapesAsync();

        // Assert
        var etapes = result.ToList();
        Assert.Equal(2, etapes.Count);
        Assert.Equal("Etape1", etapes[0].titre);
    }

    [Fact]
    public async Task GetEtapesByIdRecetteAsync_ReturnsEtapesForRecipe()
    {
        // Arrange
        var expectedEtapes = new List<Etape>
        {
            new Etape { id_recette = 5, numero = 1, titre = "Step1" }
        };

        var uow = new FakeUoW(getEtapesByIdRecetteResult: expectedEtapes);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetEtapesByIdRecetteAsync(5);

        // Assert
        var etapes = result.ToList();
        Assert.Single(etapes);
        Assert.Equal("Step1", etapes[0].titre);
    }

    [Fact]
    public async Task AddEtapeAsync_CreatesAndReturnsEtape()
    {
        // Arrange
        var newEtape = new Etape { id_recette = 1, numero = 1, titre = "New", texte = "Text" };
        var uow = new FakeUoW(createEtapeResult: newEtape);
        var service = new BiblioService(uow);

        // Act
        var result = await service.AddEtapeAsync(newEtape);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New", result.titre);
    }

    [Fact]
    public async Task ModifyEtapeAsync_UpdatesAndReturnsEtape()
    {
        // Arrange
        var updateEtape = new Etape { id_recette = 1, numero = 1, titre = "Updated", texte = "Updated text" };
        var uow = new FakeUoW(modifyEtapeResult: updateEtape);
        var service = new BiblioService(uow);

        // Act
        var result = await service.ModifyEtapeAsync(updateEtape);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.titre);
    }

    [Fact]
    public async Task DeleteEtapeAsync_ReturnsTrue_WhenDeleted()
    {
        // Arrange
        var uow = new FakeUoW(deleteEtapeResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteEtapeAsync((1, 1));

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Tests Gestion des Categories

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var expectedCategories = new List<Categorie>
        {
            new Categorie { id = 1, nom = "Dessert" },
            new Categorie { id = 2, nom = "Plat" }
        };

        var uow = new FakeUoW(getAllCategoriesResult: expectedCategories);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetAllCategoriesAsync();

        // Assert
        var categories = result.ToList();
        Assert.Equal(2, categories.Count);
        Assert.Equal("Dessert", categories[0].nom);
    }

    [Fact]
    public async Task GetCategorieByIdAsync_ReturnsCategorie()
    {
        // Arrange
        var expectedCategorie = new Categorie { id = 1, nom = "Dessert" };
        var uow = new FakeUoW(getCategorieByIdResult: expectedCategorie);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetCategorieByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dessert", result.nom);
    }

    [Fact]
    public async Task AddCategorieAsync_CreatesAndReturnsCategorie()
    {
        // Arrange
        var newCategorie = new Categorie { nom = "Entrée" };
        var createdCategorie = new Categorie { id = 3, nom = "Entrée" };
        var uow = new FakeUoW(createCategorieResult: createdCategorie);
        var service = new BiblioService(uow);

        // Act
        var result = await service.AddCategorieAsync(newCategorie);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.id);
        Assert.Equal("Entrée", result.nom);
    }

    [Fact]
    public async Task ModifyCategorieAsync_UpdatesAndReturnsCategorie()
    {
        // Arrange
        var updateCategorie = new Categorie { id = 1, nom = "Dessert Updated" };
        var uow = new FakeUoW(modifyCategorieResult: updateCategorie);
        var service = new BiblioService(uow);

        // Act
        var result = await service.ModifyCategorieAsync(updateCategorie);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Dessert Updated", result.nom);
    }

    [Fact]
    public async Task DeleteCategorieAsync_ReturnsTrue_WhenNoRelations()
    {
        // Arrange
        var uow = new FakeUoW(hasRecetteRelationsResult: false, deleteCategorieResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteCategorieAsync(1);

        // Assert
        Assert.True(result);
        Assert.True(uow.CommitCalled);
    }

    [Fact]
    public async Task DeleteCategorieAsync_ThrowsException_WhenHasRelations()
    {
        // Arrange
        var uow = new FakeUoW(hasRecetteRelationsResult: true);
        var service = new BiblioService(uow);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteCategorieAsync(1));
        Assert.True(uow.RollbackCalled);
    }

    #endregion

    #region Tests Gestion des Ingredients

    [Fact]
    public async Task GetAllIngredientsAsync_ReturnsAllIngredients()
    {
        // Arrange
        var expectedIngredients = new List<Ingredient>
        {
            new Ingredient { id = 1, nom = "Sel" },
            new Ingredient { id = 2, nom = "Poivre" }
        };

        var uow = new FakeUoW(getAllIngredientsResult: expectedIngredients);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetAllIngredientsAsync();

        // Assert
        var ingredients = result.ToList();
        Assert.Equal(2, ingredients.Count);
        Assert.Equal("Sel", ingredients[0].nom);
    }

    [Fact]
    public async Task GetIngredientByIdAsync_ReturnsIngredient()
    {
        // Arrange
        var expectedIngredient = new Ingredient { id = 1, nom = "Sel" };
        var uow = new FakeUoW(getIngredientByIdResult: expectedIngredient);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetIngredientByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sel", result.nom);
    }

    [Fact]
    public async Task AddIngredientAsync_CreatesAndReturnsIngredient()
    {
        // Arrange
        var newIngredient = new Ingredient { nom = "Sugar" };
        var createdIngredient = new Ingredient { id = 5, nom = "Sugar" };
        var uow = new FakeUoW(createIngredientResult: createdIngredient);
        var service = new BiblioService(uow);

        // Act
        var result = await service.AddIngredientAsync(newIngredient);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.id);
        Assert.Equal("Sugar", result.nom);
    }

    [Fact]
    public async Task ModifyIngredientAsync_UpdatesAndReturnsIngredient()
    {
        // Arrange
        var updateIngredient = new Ingredient { id = 1, nom = "Sel de mer" };
        var uow = new FakeUoW(modifyIngredientResult: updateIngredient);
        var service = new BiblioService(uow);

        // Act
        var result = await service.ModifyIngredientAsync(updateIngredient);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Sel de mer", result.nom);
    }

    [Fact]
    public async Task DeleteIngredientAsync_ReturnsTrue_WhenDeleted()
    {
        // Arrange
        var uow = new FakeUoW(deleteIngredientResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteIngredientAsync(1);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Tests Relations Recettes-Categories

    [Fact]
    public async Task GetAllRecettesCategoriesAsync_ReturnsAllRelations()
    {
        // Arrange
        var expectedRelations = new List<RecetteCategorieRelationship>
        {
            new RecetteCategorieRelationship { id_recette = 1, id_categorie = 1 }
        };

        var uow = new FakeUoW(getAllRecetteCategorieRelationshipResult: expectedRelations);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetAllRecettesCategoriesAsync();

        // Assert
        var relations = result.ToList();
        Assert.Single(relations);
    }

    [Fact]
    public async Task AddRecetteCategorieRelationshipAsync_ReturnsTrue()
    {
        // Arrange
        var uow = new FakeUoW(addRecetteCategorieRelationshipResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.AddRecetteCategorieRelationshipAsync(1, 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RemoveRecetteCategorieRelationshipAsync_ReturnsTrue()
    {
        // Arrange
        var uow = new FakeUoW(removeRecetteCategorieRelationshipResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.RemoveRecetteCategorieRelationshipAsync(1, 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetRecettesByIdCategorieAsync_ReturnsRecettes()
    {
        // Arrange
        var expectedRecettes = new List<Recette>
        {
            new Recette { Id = 1, nom = "Recipe1" }
        };

        var uow = new FakeUoW(getRecettesByIdCategorieResult: expectedRecettes);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetRecettesByIdCategorieAsync(1);

        // Assert
        var recettes = result.ToList();
        Assert.Single(recettes);
    }

    [Fact]
    public async Task GetCategoriesByIdRecetteAsync_ReturnsCategories()
    {
        // Arrange
        var expectedCategories = new List<Categorie>
        {
            new Categorie { id = 1, nom = "Dessert" }
        };

        var uow = new FakeUoW(getCategoriesByIdRecetteResult: expectedCategories);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetCategoriesByIdRecetteAsync(1);

        // Assert
        var categories = result.ToList();
        Assert.Single(categories);
    }

    #endregion

    #region Tests Relations Recettes-Ingredients

    [Fact]
    public async Task GetQuantiteIngredientsAsync_ReturnsAllQuantites()
    {
        // Arrange
        var expectedQuantites = new List<QuantiteIngredients>
        {
            new QuantiteIngredients { id_recette = 1, id_ingredient = 1, quantite = "100g" }
        };

        var uow = new FakeUoW(getAllQuantiteIngredientsResult: expectedQuantites);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetQuantiteIngredientsAsync();

        // Assert
        var quantites = result.ToList();
        Assert.Single(quantites);
    }

    [Fact]
    public async Task AddRecetteIngredientRelationshipAsync_CreatesRelation()
    {
        // Arrange
        var newRelation = new QuantiteIngredients { id_recette = 1, id_ingredient = 1, quantite = "50g" };
        var uow = new FakeUoW(createQuantiteIngredientResult: newRelation);
        var service = new BiblioService(uow);

        // Act
        var result = await service.AddRecetteIngredientRelationshipAsync(newRelation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("50g", result.quantite);
    }

    [Fact]
    public async Task GetIngredientsByIdRecetteAsync_ReturnsIngredients()
    {
        // Arrange
        var expectedQuantites = new List<QuantiteIngredients>
        {
            new QuantiteIngredients { id_recette = 1, id_ingredient = 1, quantite = "100g" }
        };

        var uow = new FakeUoW(getIngredientsByIdRecetteQuantiteResult: expectedQuantites);
        var service = new BiblioService(uow);

        // Act
        var result = await service.GetIngredientsByIdRecetteAsync(1);

        // Assert
        var quantites = result.ToList();
        Assert.Single(quantites);
    }

    [Fact]
    public async Task DeleteRecetteRelationsIngredientAsync_ReturnsTrue()
    {
        // Arrange
        var uow = new FakeUoW(deleteRecetteRelationsIngredientResult: true);
        var service = new BiblioService(uow);

        // Act
        var result = await service.DeleteRecetteRelationsIngredientAsync(1);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Fake Implementation of IUoW

    private class FakeUoW : IUoW
    {
        public bool CommitCalled { get; private set; }
        public bool RollbackCalled { get; private set; }

        private readonly FakeRecettesRepository _recettes;
        private readonly FakeEtapesRepository _etapes;
        private readonly FakeCategoriesRepository _categories;
        private readonly FakeIngredientsRepository _ingredients;
        private readonly FakeQuantiteIngredientsRepository _quantiteIngred;

        public FakeUoW(
            IEnumerable<Recette>? getAllRecettesResult = null,
            Recette? getRecetteByIdResult = null,
            Recette? createRecetteResult = null,
            Recette? modifyRecetteResult = null,
            bool? deleteRecetteResult = null,
            IEnumerable<Etape>? getAllEtapesResult = null,
            IEnumerable<Etape>? getEtapesByIdRecetteResult = null,
            Etape? createEtapeResult = null,
            Etape? modifyEtapeResult = null,
            bool? deleteEtapeResult = null,
            bool? deleteEtapesRelationByIdRecetteResult = null,
            IEnumerable<Categorie>? getAllCategoriesResult = null,
            Categorie? getCategorieByIdResult = null,
            Categorie? createCategorieResult = null,
            Categorie? modifyCategorieResult = null,
            bool? deleteCategorieResult = null,
            bool? hasRecetteRelationsResult = null,
            IEnumerable<Ingredient>? getAllIngredientsResult = null,
            Ingredient? getIngredientByIdResult = null,
            Ingredient? createIngredientResult = null,
            Ingredient? modifyIngredientResult = null,
            bool? deleteIngredientResult = null,
            IEnumerable<Ingredient>? getIngredientsByIdRecetteResult = null,
            IEnumerable<Categorie>? getCategoriesByIdRecetteResult = null,
            IEnumerable<QuantiteIngredients>? getAllQuantiteIngredientsResult = null,
            QuantiteIngredients? createQuantiteIngredientResult = null,
            IEnumerable<QuantiteIngredients>? getIngredientsByIdRecetteQuantiteResult = null,
            bool? deleteRecetteRelationsIngredientResult = null,
            bool? deleteRecetteRelationsResult = null,
            bool? addRecetteCategorieRelationshipResult = null,
            bool? removeRecetteCategorieRelationshipResult = null,
            IEnumerable<Recette>? getRecettesByIdCategorieResult = null,
            IEnumerable<RecetteCategorieRelationship>? getAllRecetteCategorieRelationshipResult = null)
        {
            _recettes = new FakeRecettesRepository(
                getAllRecettesResult,
                getRecetteByIdResult,
                createRecetteResult,
                modifyRecetteResult,
                deleteRecetteResult,
                deleteRecetteRelationsResult,
                addRecetteCategorieRelationshipResult,
                removeRecetteCategorieRelationshipResult,
                getRecettesByIdCategorieResult,
                getAllRecetteCategorieRelationshipResult);

            _etapes = new FakeEtapesRepository(
                getAllEtapesResult,
                getEtapesByIdRecetteResult,
                createEtapeResult,
                modifyEtapeResult,
                deleteEtapeResult,
                deleteEtapesRelationByIdRecetteResult);

            _categories = new FakeCategoriesRepository(
                getAllCategoriesResult,
                getCategorieByIdResult,
                createCategorieResult,
                modifyCategorieResult,
                deleteCategorieResult,
                hasRecetteRelationsResult,
                getCategoriesByIdRecetteResult);

            _ingredients = new FakeIngredientsRepository(
                getAllIngredientsResult,
                getIngredientByIdResult,
                createIngredientResult,
                modifyIngredientResult,
                deleteIngredientResult,
                getIngredientsByIdRecetteResult);

            _quantiteIngred = new FakeQuantiteIngredientsRepository(
                getAllQuantiteIngredientsResult,
                createQuantiteIngredientResult,
                getIngredientsByIdRecetteQuantiteResult,
                deleteRecetteRelationsIngredientResult);
        }

        public IRecetteRepository Recettes => _recettes;
        public IEtapeRepository Etapes => _etapes;
        public ICategorieRepository Categories => _categories;
        public IIngredientRepository Ingredients => _ingredients;
        public IQuantiteIngredRepository QuantiteIngred => _quantiteIngred;
        public bool HasActiveTransaction { get; } 

        public void BeginTransaction() { }
        public void Commit() => CommitCalled = true;
        public void Rollback() => RollbackCalled = true;

        public void Dispose() => Rollback();
    }

    private class FakeRecettesRepository : IRecetteRepository
    {
        private readonly IEnumerable<Recette>? _getAllResult;
        private readonly Recette? _getResult;
        private readonly Recette? _createResult;
        private readonly Recette? _modifyResult;
        private readonly bool? _deleteResult;
        private readonly bool? _deleteRelationsResult;
        private readonly bool? _addRelationshipResult;
        private readonly bool? _removeRelationshipResult;
        private readonly IEnumerable<Recette>? _getRecettesByIdCategorieResult;
        private readonly IEnumerable<RecetteCategorieRelationship>? _getAllRelationshipResult;

        public FakeRecettesRepository(
            IEnumerable<Recette>? getAllResult,
            Recette? getResult,
            Recette? createResult,
            Recette? modifyResult,
            bool? deleteResult,
            bool? deleteRelationsResult,
            bool? addRelationshipResult,
            bool? removeRelationshipResult,
            IEnumerable<Recette>? getRecettesByIdCategorieResult,
            IEnumerable<RecetteCategorieRelationship>? getAllRelationshipResult)
        {
            _getAllResult = getAllResult;
            _getResult = getResult;
            _createResult = createResult;
            _modifyResult = modifyResult;
            _deleteResult = deleteResult;
            _deleteRelationsResult = deleteRelationsResult;
            _addRelationshipResult = addRelationshipResult;
            _removeRelationshipResult = removeRelationshipResult;
            _getRecettesByIdCategorieResult = getRecettesByIdCategorieResult;
            _getAllRelationshipResult = getAllRelationshipResult;
        }

        public Task<IEnumerable<Recette>> GetAllAsync() => Task.FromResult(_getAllResult ?? Enumerable.Empty<Recette>());
        public Task<Recette> GetAsync(int id) => Task.FromResult(_getResult!);
        public Task<Recette> CreateAsync(Recette entity) => Task.FromResult(_createResult ?? entity);
        public Task<Recette> ModifyAsync(Recette entity) => Task.FromResult(_modifyResult ?? entity);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(_deleteResult ?? true);
        public Task<bool> DeleteRecetteRelationsAsync(int idRecette) => Task.FromResult(_deleteRelationsResult ?? true);
        public Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette) => Task.FromResult(_addRelationshipResult ?? true);
        public Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette) => Task.FromResult(_removeRelationshipResult ?? true);
        public Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie) => Task.FromResult(_getRecettesByIdCategorieResult ?? Enumerable.Empty<Recette>());
        public Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecetteCategorieRelationshipAsync() => Task.FromResult(_getAllRelationshipResult ?? Enumerable.Empty<RecetteCategorieRelationship>());
    }

    private class FakeEtapesRepository : IEtapeRepository
    {
        private readonly IEnumerable<Etape>? _getAllResult;
        private readonly IEnumerable<Etape>? _getByIdRecetteResult;
        private readonly Etape? _createResult;
        private readonly Etape? _modifyResult;
        private readonly bool? _deleteResult;
        private readonly bool? _deleteRelationResult;

        public FakeEtapesRepository(
            IEnumerable<Etape>? getAllResult,
            IEnumerable<Etape>? getByIdRecetteResult,
            Etape? createResult,
            Etape? modifyResult,
            bool? deleteResult,
            bool? deleteRelationResult)
        {
            _getAllResult = getAllResult;
            _getByIdRecetteResult = getByIdRecetteResult;
            _createResult = createResult;
            _modifyResult = modifyResult;
            _deleteResult = deleteResult;
            _deleteRelationResult = deleteRelationResult;
        }

        public Task<IEnumerable<Etape>> GetAllAsync() => Task.FromResult(_getAllResult ?? Enumerable.Empty<Etape>());
        public Task<Etape> GetAsync((int, int) key) => Task.FromResult<Etape>(null!);
        public Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int idRecette) => Task.FromResult(_getByIdRecetteResult ?? Enumerable.Empty<Etape>());
        public Task<Etape> CreateAsync(Etape entity) => Task.FromResult(_createResult ?? entity);
        public Task<Etape> ModifyAsync(Etape entity) => Task.FromResult(_modifyResult ?? entity);
        public Task<bool> DeleteAsync((int, int) key) => Task.FromResult(_deleteResult ?? true);
        public Task<bool> DeleteEtapesRelationByIdRecetteAsync(int idRecette) => Task.FromResult(_deleteRelationResult ?? true);
    }

    private class FakeCategoriesRepository : ICategorieRepository
    {
        private readonly IEnumerable<Categorie>? _getAllResult;
        private readonly Categorie? _getResult;
        private readonly Categorie? _createResult;
        private readonly Categorie? _modifyResult;
        private readonly bool? _deleteResult;
        private readonly bool? _hasRelationsResult;
        private readonly IEnumerable<Categorie>? _getCategoriesByIdRecetteResult;

        public FakeCategoriesRepository(
            IEnumerable<Categorie>? getAllResult,
            Categorie? getResult,
            Categorie? createResult,
            Categorie? modifyResult,
            bool? deleteResult,
            bool? hasRelationsResult,
            IEnumerable<Categorie>? getCategoriesByIdRecetteResult)
        {
            _getAllResult = getAllResult;
            _getResult = getResult;
            _createResult = createResult;
            _modifyResult = modifyResult;
            _deleteResult = deleteResult;
            _hasRelationsResult = hasRelationsResult;
            _getCategoriesByIdRecetteResult = getCategoriesByIdRecetteResult;
        }

        public Task<IEnumerable<Categorie>> GetAllAsync() => Task.FromResult(_getAllResult ?? Enumerable.Empty<Categorie>());
        public Task<Categorie> GetAsync(int id) => Task.FromResult(_getResult!);
        public Task<Categorie> CreateAsync(Categorie entity) => Task.FromResult(_createResult ?? entity);
        public Task<Categorie> ModifyAsync(Categorie entity) => Task.FromResult(_modifyResult ?? entity);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(_deleteResult ?? true);
        public Task<bool> HasRecetteRelationsAsync(int idCategorie) => Task.FromResult(_hasRelationsResult ?? false);
        public Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idRecette) => Task.FromResult(_getCategoriesByIdRecetteResult ?? Enumerable.Empty<Categorie>());
        public Task<bool> DeleteCategorieRelationsAsync(int idCategorie) => Task.FromResult(true);
    }

    private class FakeIngredientsRepository : IIngredientRepository
    {
        private readonly IEnumerable<Ingredient>? _getAllResult;
        private readonly Ingredient? _getResult;
        private readonly Ingredient? _createResult;
        private readonly Ingredient? _modifyResult;
        private readonly bool? _deleteResult;
        private readonly IEnumerable<Ingredient>? _getByIdRecetteResult;

        public FakeIngredientsRepository(
            IEnumerable<Ingredient>? getAllResult,
            Ingredient? getResult,
            Ingredient? createResult,
            Ingredient? modifyResult,
            bool? deleteResult,
            IEnumerable<Ingredient>? getByIdRecetteResult)
        {
            _getAllResult = getAllResult;
            _getResult = getResult;
            _createResult = createResult;
            _modifyResult = modifyResult;
            _deleteResult = deleteResult;
            _getByIdRecetteResult = getByIdRecetteResult;
        }

        public Task<IEnumerable<Ingredient>> GetAllAsync() => Task.FromResult(_getAllResult ?? Enumerable.Empty<Ingredient>());
        public Task<Ingredient> GetAsync(int id) => Task.FromResult(_getResult!);
        public Task<Ingredient> CreateAsync(Ingredient entity) => Task.FromResult(_createResult ?? entity);
        public Task<Ingredient> ModifyAsync(Ingredient entity) => Task.FromResult(_modifyResult ?? entity);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(_deleteResult ?? true);
        public Task<IEnumerable<Ingredient>> GetIngredientsByIdRecetteAsync(int idRecette) => Task.FromResult(_getByIdRecetteResult ?? Enumerable.Empty<Ingredient>());
    }

    private class FakeQuantiteIngredientsRepository : IQuantiteIngredRepository
    {
        private readonly IEnumerable<QuantiteIngredients>? _getAllResult;
        private readonly QuantiteIngredients? _createResult;
        private readonly IEnumerable<QuantiteIngredients>? _getByIdRecetteResult;
        private readonly bool? _deleteRelationResult;

        public FakeQuantiteIngredientsRepository(
            IEnumerable<QuantiteIngredients>? getAllResult,
            QuantiteIngredients? createResult,
            IEnumerable<QuantiteIngredients>? getByIdRecetteResult,
            bool? deleteRelationResult)
        {
            _getAllResult = getAllResult;
            _createResult = createResult;
            _getByIdRecetteResult = getByIdRecetteResult;
            _deleteRelationResult = deleteRelationResult;
        }

        public Task<IEnumerable<QuantiteIngredients>> GetAllAsync() => Task.FromResult(_getAllResult ?? Enumerable.Empty<QuantiteIngredients>());
        public Task<QuantiteIngredients> GetAsync((int, int) key) => Task.FromResult<QuantiteIngredients>(null!);
        public Task<QuantiteIngredients> CreateAsync(QuantiteIngredients entity) => Task.FromResult(_createResult ?? entity);
        public Task<QuantiteIngredients> ModifyAsync(QuantiteIngredients entity) => Task.FromResult(entity);
        public Task<bool> DeleteAsync((int, int) key) => Task.FromResult(true);
        public Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette) => Task.FromResult(_getByIdRecetteResult ?? Enumerable.Empty<QuantiteIngredients>());
        public Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient) => Task.FromResult(Enumerable.Empty<Recette>());
        public Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette) => Task.FromResult(_deleteRelationResult ?? true);
        public Task<bool> DeleteIngredientRelationsRecetteAsync(int idIngredient) => Task.FromResult(true);
    }

    #endregion
}
