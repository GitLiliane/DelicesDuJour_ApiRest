using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DelicesDuJour_ApiRest.Controllers;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.DTOIn;
using DelicesDuJour_ApiRest.Domain.DTO.DTOOut;
using DelicesDuJour_ApiRest.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Xunit;

namespace DelicesDuJour_ApiRest.Tests.Controllers;

public class RecettesControllerTest
{
    [Fact]
    public async Task UpdateRecette_ReturnsOk_WhenServiceReturnsRecette()
    {
        // Arrange
        var expectedId = 42;

        var updateDto = new UpdateRecetteDTO
        {
            Id = expectedId,
            nom = "Test",
            temps_preparation = TimeSpan.FromMinutes(10),
            temps_cuisson = TimeSpan.FromMinutes(20),
            difficulte = 2,
            ingredients = new List<IngredientDTO>
            {
                new IngredientDTO { id = 1, nom = "Sel", quantite = "1t" }
            },
            etapes = new List<UpdateEtapeDTO>
            {
                new UpdateEtapeDTO { id_recette = expectedId, numero = 1, titre = "Etape1", texte = "Faire ceci" }
            },
            categories = new List<CategorieDTO>
            {
                new CategorieDTO { id = 3, nom = "Dessert" }
            },
            photo = null
        };

        var requestJson = JsonSerializer.Serialize(updateDto);

        var returnedRecette = new Recette
        {
            Id = expectedId,
            nom = updateDto.nom,
            temps_preparation = updateDto.temps_preparation,
            temps_cuisson = updateDto.temps_cuisson,
            difficulte = updateDto.difficulte,
            ingredients = updateDto.ingredients.Select(i => new Ingredient { id = i.id, nom = i.nom, quantite = i.quantite }).ToList(),
            etapes = updateDto.etapes.Select(e => new Etape { id_recette = e.id_recette, numero = e.numero, titre = e.titre, texte = e.texte }).ToList(),
            categories = updateDto.categories.Select(c => new Categorie { id = c.id, nom = c.nom }).ToList(),
            photo = null
        };

        var service = new FakeBiblioService(modifyRecetteResult: returnedRecette);
        IValidator<UpdateRecetteDTO> validator = new FakeValidator<UpdateRecetteDTO>();

        var controller = new RecettesController(service);

        // Act
        var result = await controller.UpdateRecette(validator, expectedId, requestJson, null);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<RecetteDTO>(ok.Value);
        Assert.Equal(expectedId, dto.Id);
        Assert.Equal(updateDto.nom, dto.nom);
        Assert.Equal(updateDto.difficulte, dto.difficulte);
        Assert.Equal(1, dto.ingredients.Count);
        Assert.Equal(1, dto.etapes.Count);
        Assert.Equal(1, dto.categories.Count);
    }

    [Fact]
    public async Task UpdateRecette_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        // Arrange
        var updateDto = new UpdateRecetteDTO
        {
            Id = 5,
            nom = "Test2",
            temps_preparation = TimeSpan.Zero,
            temps_cuisson = TimeSpan.Zero,
            difficulte = 1,
            ingredients = new List<IngredientDTO>(),
            etapes = new List<UpdateEtapeDTO>(),
            categories = new List<CategorieDTO>(),
            photo = null
        };

        var requestJson = JsonSerializer.Serialize(updateDto);

        var service = new FakeBiblioService(modifyRecetteResult: null);
        IValidator<UpdateRecetteDTO> validator = new FakeValidator<UpdateRecetteDTO>();
        var controller = new RecettesController(service);

        // Act
        var result = await controller.UpdateRecette(validator, updateDto.Id, requestJson, null);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid reciep.", bad.Value);
    }

    [Fact]
    public async Task GetRecettes_ReturnsOk_WithMappedList()
    {
        // Arrange
        var recettes = new List<Recette>
        {
            new Recette { Id = 1, nom = "R1", temps_preparation = TimeSpan.FromMinutes(5), temps_cuisson = TimeSpan.FromMinutes(10), difficulte = 1 }
        };

        var service = new FakeBiblioService(getAllRecettesResult: recettes);
        var controller = new RecettesController(service);

        // Act
        var result = await controller.GetRecettes();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<RecetteDTO>>(ok.Value);
        var dto = Assert.Single(list);
        Assert.Equal(1, dto.Id);
        Assert.Equal("R1", dto.nom);
    }

    [Fact]
    public async Task GetRecetteById_ReturnsOk_WithFullDto_WhenFound()
    {
        // Arrange
        var recette = new Recette
        {
            Id = 10,
            nom = "Found",
            temps_preparation = TimeSpan.FromMinutes(7),
            temps_cuisson = TimeSpan.FromMinutes(14),
            difficulte = 2,
            ingredients = new List<Ingredient> { new Ingredient { id = 1, nom = "Sugar", quantite = "1g" } },
            etapes = new List<Etape> { new Etape { id_recette = 10, numero = 1, titre = "T", texte = "Do" } },
            categories = new List<Categorie> { new Categorie { id = 2, nom = "Cat" } },
            photo = "/images/recettes/pic.jpg"
        };

        var service = new FakeBiblioService(getRecetteByIdResult: recette);
        var controller = new RecettesController(service);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        controller.ControllerContext.HttpContext.Request.Scheme = "https";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 5001);

        // Act
        var result = await controller.GetRecetteById(10);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<RecetteDTO>(ok.Value);
        Assert.Equal(10, dto.Id);
        Assert.Equal("Found", dto.nom);
        Assert.Equal("https://localhost:5001/images/recettes/pic.jpg", dto.photo);
        Assert.Single(dto.ingredients);
        Assert.Single(dto.etapes);
        Assert.Single(dto.categories);
    }

    [Fact]
    public async Task GetRecetteById_ReturnsNotFound_WhenNull()
    {
        // Arrange
        var service = new FakeBiblioService(getRecetteByIdResult: null);
        var controller = new RecettesController(service);

        // Act
        var result = await controller.GetRecetteById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateRecette_ReturnsCreated_WhenServiceReturnsRecette()
    {
        // Arrange
        var createDto = new CreateRecetteDTO
        {
            nom = "New",
            temps_preparation = TimeSpan.FromMinutes(3),
            temps_cuisson = TimeSpan.FromMinutes(6),
            difficulte = 1,
            ingredients = new List<IngredientDTO> { new IngredientDTO { id = 1, nom = "I", quantite = "q" } },
            etapes = new List<CreateEtapeDTO> { new CreateEtapeDTO { numero = 1, titre = "t", texte = "x" } },
            categories = new List<CategorieDTO> { new CategorieDTO { id = 1, nom = "c" } },
            photo = null
        };

        var requestJson = JsonSerializer.Serialize(createDto);

        var returned = new Recette
        {
            Id = 123,
            nom = createDto.nom,
            temps_preparation = createDto.temps_preparation,
            temps_cuisson = createDto.temps_cuisson,
            difficulte = createDto.difficulte,
            ingredients = createDto.ingredients.Select(i => new Ingredient { id = i.id, nom = i.nom, quantite = i.quantite }).ToList(),
            etapes = createDto.etapes.Select(e => new Etape { numero = e.numero, titre = e.titre, texte = e.texte }).ToList(),
            categories = createDto.categories.Select(c => new Categorie { id = c.id, nom = c.nom }).ToList(),
            photo = null
        };

        var service = new FakeBiblioService(addRecetteResult: returned);
        IValidator<CreateRecetteDTO> validator = new FakeValidator<CreateRecetteDTO>();
        var controller = new RecettesController(service);

        // Act
        var result = await controller.CreateRecette(validator, requestJson, null);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var dto = Assert.IsType<RecetteDTO>(created.Value);
        Assert.Equal(123, dto.Id);
        Assert.Equal(createDto.nom, dto.nom);
    }

    [Fact]
    public async Task CreateRecette_ReturnsBadRequest_WhenServiceReturnsNull()
    {
        // Arrange
        var createDto = new CreateRecetteDTO
        {
            nom = "New2",
            temps_preparation = TimeSpan.Zero,
            temps_cuisson = TimeSpan.Zero,
            difficulte = 1,
            ingredients = new List<IngredientDTO>(),
            etapes = new List<CreateEtapeDTO>(),
            categories = new List<CategorieDTO>(),
            photo = null
        };

        var requestJson = JsonSerializer.Serialize(createDto);

        var service = new FakeBiblioService(addRecetteResult: null);
        IValidator<CreateRecetteDTO> validator = new FakeValidator<CreateRecetteDTO>();
        var controller = new RecettesController(service);

        // Act
        var result = await controller.CreateRecette(validator, requestJson, null);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid Reciep data.", bad.Value);
    }

    [Fact]
    public async Task DeleteRecette_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        var service = new FakeBiblioService(deleteRecetteResult: true);
        var controller = new RecettesController(service);

        // Act
        var result = await controller.DeleteRecette(7);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteRecette_ReturnsNotFound_WhenFail()
    {
        // Arrange
        var service = new FakeBiblioService(deleteRecetteResult: false);
        var controller = new RecettesController(service);

        // Act
        var result = await controller.DeleteRecette(8);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // Minimal fake IBiblioService implementation for the tests with configurable results
    private class FakeBiblioService : IBiblioService
    {
        private readonly IEnumerable<Recette>? _getAllRecettesResult;
        private readonly Recette? _getRecetteByIdResult;
        private readonly Recette? _addRecetteResult;
        private readonly Recette? _modifyRecetteResult;
        private readonly bool? _deleteRecetteResult;

        public FakeBiblioService(
            IEnumerable<Recette>? getAllRecettesResult = null,
            Recette? getRecetteByIdResult = null,
            Recette? addRecetteResult = null,
            Recette? modifyRecetteResult = null,
            bool? deleteRecetteResult = null)
        {
            _getAllRecettesResult = getAllRecettesResult;
            _getRecetteByIdResult = getRecetteByIdResult;
            _addRecetteResult = addRecetteResult;
            _modifyRecetteResult = modifyRecetteResult;
            _deleteRecetteResult = deleteRecetteResult;
        }

        public Task<IEnumerable<Recette>> GetAllRecettesAsync() => Task.FromResult(_getAllRecettesResult ?? Enumerable.Empty<Recette>());
        public Task<Recette> GetRecetteByIdAsync(int id) => Task.FromResult(_getRecetteByIdResult!);
        public Task<Recette> AddRecetteAsync(Recette newRecette, IFormFile? photoFile) => Task.FromResult(_addRecetteResult!);
        public Task<Recette> ModifyRecetteAsync(Recette updateRecette) => Task.FromResult(_modifyRecetteResult!);
        public Task<bool> DeleteRecetteAsync(int id) => Task.FromResult(_deleteRecetteResult ?? true);

        public Task<IEnumerable<Etape>> GetAllEtapesAsync() => Task.FromResult(Enumerable.Empty<Etape>());
        public Task<IEnumerable<Etape>> GetEtapesByIdRecetteAsync(int id) => Task.FromResult(Enumerable.Empty<Etape>());
        public Task<Etape> AddEtapeAsync(Etape newEtape) => Task.FromResult<Etape?>(null!);
        public Task<Etape> ModifyEtapeAsync(Etape updateEtape) => Task.FromResult<Etape?>(null!);
        public Task<bool> DeleteEtapeAsync((int, int) key) => Task.FromResult(true);
        public Task<IEnumerable<Categorie>> GetAllCategoriesAsync() => Task.FromResult(Enumerable.Empty<Categorie>());
        public Task<Categorie> GetCategorieByIdAsync(int id) => Task.FromResult<Categorie?>(null!);
        public Task<Categorie> AddCategorieAsync(Categorie newCategorie) => Task.FromResult<Categorie?>(null!);
        public Task<Categorie> ModifyCategorieAsync(Categorie updateCategorie) => Task.FromResult<Categorie?>(null!);
        public Task<bool> DeleteCategorieAsync(int id) => Task.FromResult(true);
        public Task<IEnumerable<RecetteCategorieRelationship>> GetAllRecettesCategoriesAsync() => Task.FromResult(Enumerable.Empty<RecetteCategorieRelationship>());
        public Task<bool> AddRecetteCategorieRelationshipAsync(int idCategorie, int idRecette) => Task.FromResult(true);
        public Task<bool> RemoveRecetteCategorieRelationshipAsync(int idCategorie, int idRecette) => Task.FromResult(true);
        public Task<IEnumerable<Recette>> GetRecettesByIdCategorieAsync(int idCategorie) => Task.FromResult(Enumerable.Empty<Recette>());
        public Task<IEnumerable<Categorie>> GetCategoriesByIdRecetteAsync(int idCategorie) => Task.FromResult(Enumerable.Empty<Categorie>());
        public Task<bool> DeleteRecetteRelationsAsync(int idRecette) => Task.FromResult(true);
        public Task<bool> DeleteCategorieRelationsAsync(int idCategorie) => Task.FromResult(true);
        public Task<IEnumerable<Ingredient>> GetAllIngredientsAsync() => Task.FromResult(Enumerable.Empty<Ingredient>());
        public Task<Ingredient> GetIngredientByIdAsync(int id) => Task.FromResult<Ingredient?>(null!);
        public Task<Ingredient> AddIngredientAsync(Ingredient newIngredient) => Task.FromResult<Ingredient?>(null!);
        public Task<Ingredient> ModifyIngredientAsync(Ingredient updateIngredient) => Task.FromResult<Ingredient?>(null!);
        public Task<bool> DeleteIngredientAsync(int id) => Task.FromResult(true);
        public Task<IEnumerable<QuantiteIngredients>> GetQuantiteIngredientsAsync() => Task.FromResult(Enumerable.Empty<QuantiteIngredients>());
        public Task<QuantiteIngredients> GetQuantiteIngredientsByIdAsync((int, int) key) => Task.FromResult<QuantiteIngredients?>(null!);
        public Task<QuantiteIngredients> AddRecetteIngredientRelationshipAsync(QuantiteIngredients CreateRelationRI) => Task.FromResult<QuantiteIngredients?>(null!);
        public Task<QuantiteIngredients> updateRecetteIngredientRelationshipAsync(QuantiteIngredients updateRelationRI) => Task.FromResult<QuantiteIngredients?>(null!);
        public Task<bool> RemoveRecetteIngredientRelationshipAsync((int, int) key) => Task.FromResult(true);
        public Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient) => Task.FromResult(Enumerable.Empty<Recette>());
        public Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette) => Task.FromResult(Enumerable.Empty<QuantiteIngredients>());
        public Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette) => Task.FromResult(true);
        public Task<bool> DeleteIngredientRelationsAsync(int idIngredient) => Task.FromResult(true);
    }

    // Minimal fake validator that always succeeds
    private class FakeValidator<T> : AbstractValidator<T>, IValidator<T>
    {
        public FakeValidator()
        {
            // No rules - always valid
        }
    }
}