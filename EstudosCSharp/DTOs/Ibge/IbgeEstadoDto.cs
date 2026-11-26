using System.Text.Json.Serialization;

namespace EstudosCSharp.DTOs.Ibge {
    public class IbgeEstadoDto {
        [JsonPropertyName("id")]
        public int CodigoIbge { get; set; }

        [JsonPropertyName("sigla")]
        public string UF { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
    }
}