namespace EstudosCSharp.Models {
    public class Estado {
        public int ID { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string UF { get; set; } = string.Empty;

        public string Codigo_IBGE { get; set; } = string.Empty;
    }
}