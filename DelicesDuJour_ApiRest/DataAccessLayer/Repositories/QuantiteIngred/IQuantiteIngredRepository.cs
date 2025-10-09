using DelicesDuJour_ApiRest.DataAccessLayer.Session;
using DelicesDuJour_ApiRest.Domain.BO;
using DelicesDuJour_ApiRest.Domain.DTO.In;
using DelicesDuJour_ApiRest.Domain.DTO.Out;

namespace DelicesDuJour_ApiRest.DataAccessLayer.Repositories.QuantiteIngred;

public interface IQuantiteIngredRepository : IGenericReadRepository<(int, int), QuantiteIngredients>, IGenericWriteRepository<(int, int), QuantiteIngredients>
{
    #region Relation Recette Ingrédient

    Task<IEnumerable<QuantiteIngredients>> GetAllAsync();   

    Task<QuantiteIngredients> GetAsync((int, int) key); 

    Task<QuantiteIngredients> CreateAsync(QuantiteIngredients CreateRelationRI);   

    Task<QuantiteIngredients> ModifyAsync(QuantiteIngredients updateBook);

    Task<bool> DeleteAsync((int, int) key);   

    Task<IEnumerable<Recette>> GetRecettesByIdIngredientAsync(int idIngredient);  

    Task<IEnumerable<QuantiteIngredients>> GetIngredientsByIdRecetteAsync(int idRecette);    

    Task<bool> DeleteRecetteRelationsIngredientAsync(int idRecette);  

    Task<bool> DeleteIngredientRelationsRecetteAsync(int idIngredient);
   

    #endregion Fin Relation Recette Ingrédient
}
