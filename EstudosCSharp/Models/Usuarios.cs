namespace EstudosCSharp.Models {
    public class Usuarios {
        public int ID { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public string Tipo_Usuario { get; set; } = "Padrao";

        public bool Ativo { get; set; } = true;

        // Propriedade calculada.
        public bool IsAdministrador =>
            Tipo_Usuario == "Administrador";
    }
}
