namespace EstudosCSharp.ViewModels {
    public class ResultadoAutenticacao {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public int ID_Usuario { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Tipo_Usuario { get; set; } = string.Empty;
    }
}