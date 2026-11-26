namespace EstudosCSharp.Models {
    public class Cidade {
        public int ID { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Codigo_IBGE { get; set; } = string.Empty;

        public int ID_Estado { get; set; }

        // Propriedade de navegação.
        public Estado? Estado { get; set; }
    }
}