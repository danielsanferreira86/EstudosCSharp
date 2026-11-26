using System.Net.Http.Json;

using EstudosCSharp.DTOs.Ibge;

using Microsoft.Extensions.Caching.Memory;

namespace EstudosCSharp.Services {
    public class IbgeService {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        private const string CacheKey = "IBGE_ESTADOS";

        public IbgeService(
            HttpClient httpClient,
            IMemoryCache cache,
            IConfiguration configuration) {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
        }

        // LISTAR ESTADOS OFICIAIS DO IBGE
        public async Task<IReadOnlyList<IbgeEstadoDto>>
            ListarEstadosAsync(
                CancellationToken cancellationToken = default) {
            // CONSULTAR O CACHE
            if (_cache.TryGetValue(
                CacheKey,
                out IReadOnlyList<IbgeEstadoDto>? estadosCache)
                && estadosCache is not null) {
                return estadosCache;
            }

            // CONSULTAR A API OFICIAL
            var estados = await _httpClient
                .GetFromJsonAsync<List<IbgeEstadoDto>>(
                    "estados",
                    cancellationToken);

            // VALIDAR A RESPOSTA RECEBIDA
            if (estados is null
                || estados.Count == 0
                || estados.Any(e =>
                    e.CodigoIbge < 10
                    || e.CodigoIbge > 99
                    || string.IsNullOrWhiteSpace(e.Nome)
                    || e.UF is null
                    || e.UF.Length != 2)) {
                throw new InvalidDataException(
                    "O IBGE retornou dados inválidos.");
            }

            // ORDENAR OS REGISTROS
            IReadOnlyList<IbgeEstadoDto> resultado =
                estados
                    .OrderBy(e => e.Nome)
                    .ToList()
                    .AsReadOnly();

            // RECUPERAR O TEMPO DE CACHE
            int cacheHoras = _configuration
                .GetValue<int>("Ibge:CacheHoras", 24);

            // ARMAZENAR EM MEMÓRIA
            _cache.Set(
                CacheKey,
                resultado,
                TimeSpan.FromHours(cacheHoras));

            return resultado;
        }

        // BUSCAR ESTADO PELO CÓDIGO IBGE
        public async Task<IbgeEstadoDto?>
            BuscarEstadoPorCodigoAsync(
                string? codigoIbge,
                CancellationToken cancellationToken = default) {
            // VALIDAR O FORMATO DO CÓDIGO
            if (string.IsNullOrWhiteSpace(codigoIbge)
                || codigoIbge.Length != 2
                || !codigoIbge.All(char.IsAsciiDigit)
                || !int.TryParse(codigoIbge, out int codigo)) {
                return null;
            }

            var estados = await ListarEstadosAsync(
                cancellationToken);

            return estados.FirstOrDefault(
                e => e.CodigoIbge == codigo);
        }

        // VALIDAR OS DADOS DE UM ESTADO
        public async Task<bool> ValidarEstadoAsync(
            string codigoIbge,
            string nome,
            string uf,
            CancellationToken cancellationToken = default) {
            var estado = await BuscarEstadoPorCodigoAsync(
                codigoIbge,
                cancellationToken);

            if (estado is null) {
                return false;
            }

            return string.Equals(
                       estado.Nome,
                       nome.Trim(),
                       StringComparison.OrdinalIgnoreCase)

                && string.Equals(
                       estado.UF,
                       uf.Trim(),
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}