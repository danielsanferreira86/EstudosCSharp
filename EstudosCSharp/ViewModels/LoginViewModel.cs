using System.ComponentModel.DataAnnotations;

namespace EstudosCSharp.ViewModels {
    public class LoginViewModel {
        [Required(ErrorMessage = "Informe o usuário.")]
        [StringLength(50)]
        [Display(Name = "Usuário")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;
    }
}