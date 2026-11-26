using EstudosCSharp.Models;
using EstudosCSharp.Repositories;
using EstudosCSharp.ViewModels;

namespace EstudosCSharp.Services {
    public class AutenticacaoService {
        private readonly UsuarioRepository _usuariosRepository;

        // Injeção de dependência.
        public AutenticacaoService(
            UsuarioRepository usuarioRepository) {
            _usuariosRepository = usuarioRepository;
        }

        // AUTENTICAR USUÁRIO
        public async Task<ResultadoAutenticacao> AutenticarAsync(
            string login, string senha) {
            // Validação inicial das credenciais.
            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(senha) ||
                login.Length > 50) {
                return Falha();
            }

            // Localiza o usuário no banco de dados.
            Usuarios? usuarios =
                await _usuariosRepository.BuscarPorLoginAsync(
                    login.Trim());

            // Verifica se o usuário existe.
            if (usuarios is null) {
                return Falha();
            }

            // Verifica se o usuário está ativo.
            if (!usuarios.Ativo) {
                return Falha();
            }

            // Valida a senha utilizando BCrypt.
            bool senhaValida = BCrypt.Net.BCrypt.Verify(
                senha,
                usuarios.SenhaHash);

            if (!senhaValida) {
                return Falha();
            }

            // Valida o perfil cadastrado.
            if (usuarios.Tipo_Usuario != "Administrador" &&
                usuarios.Tipo_Usuario != "Padrao") {
                return Falha();
            }

            // Retorna os dados do usuário autenticado.
            return new ResultadoAutenticacao {
                Sucesso = true,
                Mensagem = "Usuário autenticado com sucesso.",
                ID_Usuario = usuarios.ID,
                Nome = usuarios.Nome,
                Login = usuarios.Login,
                Tipo_Usuario = usuarios.Tipo_Usuario
            };
        }

        // GERA O HASH DE UMA SENHA
        public string GerarHashSenha(string senha) {
            if (string.IsNullOrWhiteSpace(senha)) {
                throw new ArgumentException(
                    "A senha não pode ser vazia.");
            }

            return BCrypt.Net.BCrypt.HashPassword(senha, 12);
        }

        // VALIDA UMA SENHA CONTRA UM HASH
        public bool VerificarSenha(
            string senha, string senhaHash) {
            if (string.IsNullOrEmpty(senha) ||
                string.IsNullOrEmpty(senhaHash)) {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
        }

        // RETORNO PADRÃO PARA FALHAS DE AUTENTICAÇÃO
        private static ResultadoAutenticacao Falha() {
            return new ResultadoAutenticacao {
                Sucesso = false,
                Mensagem = "Usuário ou senha inválidos."
            };
        }
    }
}