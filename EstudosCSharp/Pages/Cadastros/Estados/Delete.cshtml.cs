using EstudosCSharp.Models;
using EstudosCSharp.Repositories;
using EstudosCSharp.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace EstudosCSharp.Pages.Cadastros.Estados {
    [Authorize(Policy = Permissoes.GerenciarEstados)]
    public class DeleteModel : PageModel {
        private readonly EstadoRepository _repository;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(
            EstadoRepository repository,
            ILogger<DeleteModel> logger) {
            _repository = repository;
            _logger = logger;
        }

        // REGISTRO SELECIONADO
        public Estado Estado { get; private set; } = new();

        // CONTROLE DE RELACIONAMENTOS
        public bool PossuiCidades { get; private set; }

        // CONTROLE DA EXCLUSÃO
        public bool PodeExcluir { get; private set; }

        // CARREGAR DADOS DO REGISTRO
        private async Task<bool> CarregarEstadoAsync(int id) {
            var estado = await _repository
                .BuscarPorIdAsync(id);

            if (estado is null) {
                return false;
            }

            Estado = estado;

            PossuiCidades = await _repository
                .PossuiCidadesAsync(id);

            PodeExcluir = !PossuiCidades;

            return true;
        }

        // EXIBIR A CONFIRMAÇÃO
        public async Task<IActionResult> OnGetAsync(
            [FromRoute] int id) {
            if (!await CarregarEstadoAsync(id)) {
                return NotFound();
            }

            return Page();
        }

        // PROCESSAR A EXCLUSÃO
        public async Task<IActionResult> OnPostAsync(
            [FromRoute] int id) {
            // RECUPERAR NOVAMENTE O REGISTRO
            if (!await CarregarEstadoAsync(id)) {
                return NotFound();
            }

            // VALIDAR RELACIONAMENTOS
            if (!PodeExcluir) {
                ModelState.AddModelError(
                    string.Empty,
                    "Não é possível excluir um estado "
                    + "que possui cidades vinculadas.");

                return Page();
            }

            try {
                // EXECUTAR A EXCLUSÃO
                bool excluido = await _repository
                    .ExcluirAsync(id);

                // VALIDAR O RESULTADO
                if (!excluido) {
                    return NotFound();
                }

                // REGISTRAR MENSAGEM DE SUCESSO
                TempData["MensagemSucesso"] =
                    $"Estado {Estado.Nome} "
                    + "excluído com sucesso.";

                return RedirectToPage("./Index");
            } catch (SqlException ex)
                  when (ex.Number == 547) {
                // BLOQUEAR NOVAS TENTATIVAS
                PodeExcluir = false;

                _logger.LogWarning(
                    ex,
                    "Exclusão bloqueada por integridade "
                    + "referencial. Estado ID: {ID}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "O estado possui registros vinculados "
                    + "e não pode ser excluído.");

                return Page();
            } catch (SqlException ex) {
                _logger.LogError(
                    ex,
                    "Erro ao excluir o estado {ID}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível excluir o estado. "
                    + "Tente novamente mais tarde.");

                return Page();
            }
        }
    }
}