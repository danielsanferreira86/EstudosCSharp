using EstudosCSharp.Models;
using EstudosCSharp.Repositories;
using EstudosCSharp.Security;
using EstudosCSharp.Services;
using EstudosCSharp.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

using System.Text.Json;

namespace EstudosCSharp.Pages.Cadastros.Estados {
    [Authorize(Policy = Permissoes.GerenciarEstados)]
    public class EditModel : PageModel {
        private readonly EstadoRepository _repository;
        private readonly IbgeService _ibgeService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            EstadoRepository repository,
            IbgeService ibgeService,
            ILogger<EditModel> logger) {
            _repository = repository;
            _ibgeService = ibgeService;
            _logger = logger;
        }

        // IDENTIFICADOR DO REGISTRO
        public int ID { get; private set; }

        // CONTROLE DE RELACIONAMENTOS
        public bool PossuiCidades { get; private set; }

        // DADOS DO FORMULÁRIO
        [BindProperty]
        public EstadoViewModel Estado { get; set; } = new();

        // CARREGAR O REGISTRO
        public async Task<IActionResult> OnGetAsync(int id) {
            var estado = await _repository
                .BuscarPorIdAsync(id);

            if (estado is null) {
                return NotFound();
            }

            ID = estado.ID;

            PossuiCidades = await _repository
                .PossuiCidadesAsync(id);

            Estado = new EstadoViewModel {
                Nome = estado.Nome,
                UF = estado.UF,
                Codigo_IBGE = estado.Codigo_IBGE
            };

            return Page();
        }

        // PROCESSAR A ALTERAÇÃO
        public async Task<IActionResult> OnPostAsync(
            int id,
            CancellationToken cancellationToken) {
            ID = id;

            try {
                // RECUPERAR O REGISTRO ORIGINAL
                var estadoOriginal = await _repository
                    .BuscarPorIdAsync(id);

                if (estadoOriginal is null) {
                    return NotFound();
                }

                // VERIFICAR RELACIONAMENTOS
                PossuiCidades = await _repository
                    .PossuiCidadesAsync(id);

                // VALIDAR O FORMULÁRIO
                if (!ModelState.IsValid) {
                    return Page();
                }

                // NORMALIZAR OS DADOS
                Estado.Nome = Estado.Nome.Trim();

                Estado.UF = Estado.UF
                    .Trim()
                    .ToUpperInvariant();

                Estado.Codigo_IBGE =
                    Estado.Codigo_IBGE.Trim();

                // PROTEGER OS RELACIONAMENTOS
                if (PossuiCidades &&
                    Estado.Codigo_IBGE !=
                    estadoOriginal.Codigo_IBGE) {
                    ModelState.AddModelError(
                        "Estado.Codigo_IBGE",
                        "Não é possível alterar o código IBGE "
                        + "de um estado que possui cidades vinculadas.");

                    return Page();
                }

                // CONSULTAR A BASE OFICIAL
                var estadoOficial = await _ibgeService
                    .BuscarEstadoPorCodigoAsync(
                        Estado.Codigo_IBGE,
                        cancellationToken);

                if (estadoOficial is null) {
                    ModelState.AddModelError(
                        "Estado.Codigo_IBGE",
                        "Código inexistente na base oficial do IBGE.");

                    return Page();
                }

                // VALIDAR A CONSISTÊNCIA DOS DADOS
                if (!string.Equals(
                        Estado.Nome,
                        estadoOficial.Nome,
                        StringComparison.OrdinalIgnoreCase)

                    || !string.Equals(
                        Estado.UF,
                        estadoOficial.UF,
                        StringComparison.OrdinalIgnoreCase)) {
                    ModelState.AddModelError(
                        string.Empty,
                        "Os dados informados não correspondem "
                        + "à base oficial do IBGE.");

                    return Page();
                }

                // VALIDAR DUPLICIDADE DA UF
                // IGNORANDO O PRÓPRIO REGISTRO
                if (await _repository.ExisteUFAsync(
                    estadoOficial.UF, id)) {
                    ModelState.AddModelError(
                        "Estado.UF",
                        "Já existe outro estado com esta UF.");
                }

                // VALIDAR DUPLICIDADE DO IBGE
                // IGNORANDO O PRÓPRIO REGISTRO
                if (await _repository.ExisteCodigoIBGEAsync(
                    Estado.Codigo_IBGE, id)) {
                    ModelState.AddModelError(
                        "Estado.Codigo_IBGE",
                        "Já existe outro estado com este código IBGE.");
                }

                if (!ModelState.IsValid) {
                    return Page();
                }

                // MONTAR A ENTIDADE PARA ATUALIZAÇÃO
                var estadoAtualizado = new Estado {
                    ID = estadoOriginal.ID,

                    Nome = estadoOficial.Nome,

                    UF = estadoOficial.UF,

                    Codigo_IBGE =
                        estadoOficial.CodigoIbge.ToString()
                };

                // ATUALIZAR O BANCO DE DADOS
                await _repository.AtualizarAsync(
                    estadoAtualizado);

                // REGISTRAR MENSAGEM DE SUCESSO
                TempData["MensagemSucesso"] =
                    $"Estado {estadoAtualizado.Nome} "
                    + "alterado com sucesso.";

                return RedirectToPage("./Index");
            } catch (Exception ex)
                  when (!cancellationToken.IsCancellationRequested
                      && (ex is HttpRequestException
                          or OperationCanceledException
                          or JsonException
                          or InvalidDataException)) {
                _logger.LogWarning(
                    ex,
                    "Erro na validação do IBGE para o estado {ID}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível validar o estado "
                    + "na base oficial do IBGE.");

                return Page();
            } catch (SqlException ex)
                  when (ex.Number is 2601 or 2627) {
                ModelState.AddModelError(
                    string.Empty,
                    "A UF ou o código IBGE já está cadastrado.");

                return Page();
            } catch (SqlException ex) {
                _logger.LogError(
                    ex,
                    "Erro ao alterar o estado {ID}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível alterar o estado.");

                return Page();
            }
        }
    }
}