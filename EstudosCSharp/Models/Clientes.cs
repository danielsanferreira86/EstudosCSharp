namespace EstudosCSharp.Models {
    public class Clientes {
        public int ID { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string? Email { get; set; }

        public int? ID_Cidade { get; set; }

        // Propriedade de navegação.
        public Cidade? Cidade { get; set; }
    }
}