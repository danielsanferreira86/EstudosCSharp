using EstudosCSharp.DTOs.Ibge;
using EstudosCSharp.Services;
using EstudosCSharp.Security;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages.Testes {
    [Authorize(Policy = Permissoes.GerenciarEstados)]
    public class IbgeServiceTesteModel : PageModel {
        private readonly IbgeService _ibgeService;

        public IbgeServiceTesteModel(
            IbgeService ibgeService) {
            _ibgeService = ibgeService;
        }

        public int TotalEstados { get; private set; }

        public IbgeEstadoDto? MatoGrosso {
            get;
            private set;
        }

        public bool CodigoInvalido { get; private set; }

        public bool EstadoValido { get; private set; }

        public bool EstadoInvalido { get; private set; }

        public long TempoPrimeiraConsulta {
            get;
            private set;
        }

        public long TempoSegundaConsulta {
            get;
            private set;
        }

        public string? Erro { get; private set; }

        public async Task OnGetAsync(
            CancellationToken cancellationToken) {
            try {
                var cronometro =
                    System.Diagnostics.Stopwatch.StartNew();

                var estados = await _ibgeService
                    .ListarEstadosAsync(cancellationToken);

                cronometro.Stop();

                TempoPrimeiraConsulta =
                    cronometro.ElapsedMilliseconds;

                TotalEstados = estados.Count;

                cronometro.Restart();

                MatoGrosso = await _ibgeService
                    .BuscarEstadoPorCodigoAsync(
                        "51",
                        cancellationToken);

                cronometro.Stop();

                TempoSegundaConsulta =
                    cronometro.ElapsedMilliseconds;

                CodigoInvalido = await _ibgeService
                    .BuscarEstadoPorCodigoAsync(
                        "99",
                        cancellationToken) is null;

                EstadoValido = await _ibgeService
                    .ValidarEstadoAsync(
                        "51",
                        "Mato Grosso",
                        "MT",
                        cancellationToken);

                EstadoInvalido = !await _ibgeService
                    .ValidarEstadoAsync(
                        "51",
                        "São Paulo",
                        "SP",
                        cancellationToken);
            } catch (Exception ex)
                  when (!cancellationToken.IsCancellationRequested) {
                Erro = ex.Message;
            }
        }
    }
}