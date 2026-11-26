using Microsoft.AspNetCore.Mvc.RazorPages;
using EstudosCSharp.Models;

namespace EstudosCSharp.Pages {
    public class TesteModelsModel : PageModel {
        public Clientes ClienteTeste { get; private set; } = new();

        public void OnGet() {
            // Instanciando um Estado.
            Estado estado = new Estado {
                ID = 11,
                Nome = "Mato Grosso",
                UF = "MT",
                Codigo_IBGE = "51"
            };

            // Instanciando uma Cidade.
            Cidade cidade = new Cidade {
                ID = 1,
                Nome = "Cuiabá",
                Codigo_IBGE = "5103403",
                ID_Estado = estado.ID,
                Estado = estado
            };

            // Instanciando um Cliente.
            ClienteTeste = new Clientes {
                ID = 1,
                Nome = "Cliente Teste",
                CPF = "00000000000",
                Telefone = "65999990000",
                Email = "cliente@example.com",
                ID_Cidade = cidade.ID,
                Cidade = cidade
            };
        }
    }
}