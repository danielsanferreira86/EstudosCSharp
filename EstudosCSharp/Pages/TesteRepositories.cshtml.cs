using Microsoft.AspNetCore.Mvc.RazorPages;
using EstudosCSharp.Models;
using EstudosCSharp.Repositories;

namespace EstudosCSharp.Pages {
    public class TesteRepositoriesModel : PageModel {
        private readonly EstadoRepository _estadoRepository;
        private readonly CidadeRepository _cidadeRepository;
        private readonly UsuariosRepository _usuariosRepository;

        public List<Estado> Estados { get; private set; } = new();

        public List<Cidade> Cidades { get; private set; } = new();

        public List<Usuarios> Usuarios { get; private set; } = new();

        public TesteRepositoriesModel(
            EstadoRepository estadoRepository,
            CidadeRepository cidadeRepository,
            UsuariosRepository usuarioRepository) {
            _estadoRepository = estadoRepository;
            _cidadeRepository = cidadeRepository;
            _usuariosRepository = usuarioRepository;
        }

        public async Task OnGetAsync() {
            Estados = await _estadoRepository.ListarAsync();

            Cidades = await _cidadeRepository.ListarAsync();

            Usuarios = await _usuariosRepository.ListarAsync();
        }
    }
}