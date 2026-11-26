using EstudosCSharp.Models;
using EstudosCSharp.Repositories;
using EstudosCSharp.Security;
using EstudosCSharp.ViewModels;
using EstudosCSharp.Services;
using System.Text.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace EstudosCSharp.Pages.Cadastros.Estados {
    [Authorize(Policy = Permissoes.GerenciarEstados)]
    public class CreateModel : PageModel {
        private readonly EstadoRepository _repository;
        private readonly ILogger<CreateModel> _logger;
        private readonly IbgeService _ibgeService;

        public CreateModel(
            EstadoRepository repository,
            ILogger<CreateModel> logger,
            IbgeService ibgeService) {
            _repository = repository;
            _logger = logger;
            _ibgeService = ibgeService;
        }

        // DADOS DO FORMULÁRIO
        [BindProperty]
        public EstadoViewModel Estado { get; set; } = new();

        // ABERTURA DO FORMULÁRIO
        public void OnGet() {
        }

        // PROCESSAMENTO DA INCLUSÃO
        public async Task<IActionResult> OnPostAsync(
            CancellationToken cancellationToken) {
            // VALIDAR OS DADOS RECEBIDOS

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

            try {
                // CONSULTAR A BASE OFICIAL DO IBGE

                var estadoOficial = await _ibgeService
                    .BuscarEstadoPorCodigoAsync(
                        Estado.Codigo_IBGE,
                        cancellationToken);

                // VALIDAR A EXISTÊNCIA DO CÓDIGO

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

                // VALIDAR DUPLICIDADE DE UF

                if (await _repository.ExisteUFAsync(
                    estadoOficial.UF)) {
                    ModelState.AddModelError(
                        "Estado.UF",
                        "Já existe um estado com esta UF.");
                }

                // VALIDAR DUPLICIDADE DE CÓDIGO IBGE

                if (await _repository.ExisteCodigoIBGEAsync(
                    Estado.Codigo_IBGE)) {
                    ModelState.AddModelError(
                        "Estado.Codigo_IBGE",
                        "Já existe um estado com este código IBGE.");
                }

                if (!ModelState.IsValid) {
                    return Page();
                }

                // UTILIZAR OS DADOS OFICIAIS NA GRAVAÇÃO

                var novoEstado = new Estado {
                    Nome = estadoOficial.Nome,

                    UF = estadoOficial.UF,

                    Codigo_IBGE =
                        estadoOficial.CodigoIbge.ToString()
                };

                // INSERIR NO BANCO DE DADOS

                await _repository.InserirAsync(novoEstado);

                // MENSAGEM DE SUCESSO

                TempData["MensagemSucesso"] =
                    $"Estado {novoEstado.Nome} "
                    + "cadastrado com sucesso.";

                return RedirectToPage("./Index");
            } catch (Exception ex)
                  when (!cancellationToken.IsCancellationRequested
                      && (ex is HttpRequestException
                          or OperationCanceledException
                          or JsonException
                          or InvalidDataException)) {
                _logger.LogWarning(
                    ex,
                    "Erro durante a validação do IBGE.");

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível validar o Estado "
                    + "na base oficial do IBGE. "
                    + "Tente novamente mais tarde.");

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
                    "Erro ao cadastrar estado com UF {UF}.",
                    Estado.UF);

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o estado.");

                return Page();
            }
        }

        // CONSULTAR ESTADO NA BASE OFICIAL DO IBGE
        public async Task<IActionResult> OnGetConsultarIbgeAsync(
            string? codigoIbge,
            CancellationToken cancellationToken) {
            try {
                var estado = await _ibgeService
                    .BuscarEstadoPorCodigoAsync(
                        codigoIbge,
                        cancellationToken);

                if (estado is null) {
                    return new JsonResult(new {
                        sucesso = false,
                        mensagem = "Código IBGE não localizado."
                    }) {
                        StatusCode = 404
                    };
                }

                return new JsonResult(new {
                    sucesso = true,
                    nome = estado.Nome,
                    uf = estado.UF,
                    codigoIbge = estado.CodigoIbge.ToString()
                });
            } catch (Exception ex)
                  when (!cancellationToken.IsCancellationRequested
                      && (ex is HttpRequestException
                          or OperationCanceledException
                          or JsonException
                          or InvalidDataException)) {
                _logger.LogWarning(
                    ex,
                    "Não foi possível consultar a API do IBGE.");

                return new JsonResult(new {
                    sucesso = false,
                    mensagem = "Não foi possível consultar o IBGE. "
                        + "Tente novamente mais tarde."
                }) {
                    StatusCode = 503
                };
            }
        }
    }
}