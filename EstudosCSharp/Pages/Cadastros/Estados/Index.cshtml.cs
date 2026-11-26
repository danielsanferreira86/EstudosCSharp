using EstudosCSharp.Models;
using EstudosCSharp.Repositories;
using EstudosCSharp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages.Cadastros.Estados {
    [Authorize(Policy = Permissoes.ConsultarEstados)]
    public class IndexModel : PageModel {
        private readonly EstadoRepository _repository;

        public IndexModel(EstadoRepository repository) {
            _repository = repository;
        }

        // LISTA DE ESTADOS
        public List<Estado> Estados { get; private set; }
            = new();

        // FILTRO DE PESQUISA
        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        // P핯INA ATUAL
        [BindProperty(SupportsGet = true)]
        public int PaginaAtual { get; set; } = 1;

        // QUANTIDADE DE REGISTROS POR P핯INA
        [BindProperty(SupportsGet = true)]
        public int TamanhoPagina { get; set; } = 10;

        // TOTAL DE REGISTROS
        public int TotalRegistros { get; private set; }

        // TOTAL DE P핯INAS
        public int TotalPaginas { get; private set; }

        // TOTAL GERAL DE ESTADOS CADASTRADOS
        public int TotalGeral { get; private set; }

        // CONTROLE DA PAGINA플O
        public bool TemPaginaAnterior =>
            PaginaAtual > 1;

        public bool TemProximaPagina =>
            PaginaAtual < TotalPaginas;

        // POSI플O DOS REGISTROS
        public int PrimeiroRegistro =>
            Estados.Count == 0
                ? 0
                : ((PaginaAtual - 1) * TamanhoPagina) + 1;

        public int UltimoRegistro =>
            Estados.Count == 0
                ? 0
                : PrimeiroRegistro + Estados.Count - 1;

        // CARREGAMENTO DA P핯INA
        public async Task OnGetAsync() {
            // NORMALIZAR A PESQUISA
            Pesquisa = Pesquisa?.Trim();

            // LIMITAR O TAMANHO DA PESQUISA
            if (Pesquisa?.Length > 50) {
                Pesquisa = Pesquisa[..50];
            }

            // VALIDAR O TAMANHO DA P핯INA
            if (TamanhoPagina is not (5 or 10 or 20)) {
                TamanhoPagina = 10;
            }

            // RECUPERAR O TOTAL GERAL DE REGISTROS
            TotalGeral = await _repository.ContarAsync();

            // RECUPERAR O TOTAL DE REGISTROS DA PESQUISA
            TotalRegistros = string.IsNullOrWhiteSpace(Pesquisa)
                ? TotalGeral
                : await _repository.ContarAsync(Pesquisa);

            // CALCULAR O TOTAL DE P핯INAS
            TotalPaginas = Math.Max(
                1,
                (int)Math.Ceiling(
                    (double)TotalRegistros / TamanhoPagina));

            // VALIDAR A P핯INA SOLICITADA
            PaginaAtual = Math.Clamp(
                PaginaAtual,
                1,
                TotalPaginas);

            // SINCRONIZAR OS VALORES NORMALIZADOS
            ModelState.Remove(nameof(Pesquisa));
            ModelState.Remove(nameof(PaginaAtual));
            ModelState.Remove(nameof(TamanhoPagina));

            // RECUPERAR OS REGISTROS
            Estados = await _repository.PesquisarAsync(
                Pesquisa,
                PaginaAtual,
                TamanhoPagina);
        }
    }
}