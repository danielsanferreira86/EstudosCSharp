namespace EstudosCSharp.ViewModels.Componentes {
    public class CabecalhoCadastroViewModel {
        public string Caminho { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string Subtitulo { get; set; } = string.Empty;

        public string? TextoAcao { get; set; }

        public string? PaginaAcao { get; set; }

        public bool ExibirAcao { get; set; }
    }
}