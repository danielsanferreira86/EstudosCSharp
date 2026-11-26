using System.ComponentModel.DataAnnotations;

namespace EstudosCSharp.ViewModels {
    public class EstadoViewModel {
        [Display(Name = "Nome do Estado")]
        [Required(ErrorMessage =
            "Informe o nome do Estado.")]
        [StringLength(50, ErrorMessage =
            "O nome deve possuir no máximo 50 caracteres.")]
        public string Nome { get; set; }
            = string.Empty;


        [Display(Name = "UF")]
        [Required(ErrorMessage =
            "Informe a UF.")]
        [StringLength(2, MinimumLength = 2,
            ErrorMessage =
            "A UF deve possuir exatamente 2 caracteres.")]
        [RegularExpression(@"^[A-Za-z]{2}$",
            ErrorMessage =
            "A UF deve conter apenas letras.")]
        public string UF { get; set; }
            = string.Empty;


        [Display(Name = "Código IBGE")]
        [Required(ErrorMessage =
            "Informe o código IBGE.")]
        [StringLength(2, MinimumLength = 2,
            ErrorMessage =
            "O código IBGE deve possuir exatamente 2 dígitos.")]
        [RegularExpression(@"^[0-9]{2}$",
            ErrorMessage =
            "O código IBGE deve conter apenas números.")]
        public string Codigo_IBGE { get; set; }
            = string.Empty;
    }
}