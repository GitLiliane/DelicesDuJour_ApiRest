using System.Text.Json.Serialization;

namespace DelicesDuJour_ApiRest.Domain.DTO.Out
{
    public class TupleDTO<T, V>
    {
        [JsonPropertyName("id_recette")]
        public T t { get; set; }

        [JsonPropertyName("numero")]
        public V v { get; set; }
    }
}