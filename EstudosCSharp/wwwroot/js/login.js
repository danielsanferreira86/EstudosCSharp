document.addEventListener("DOMContentLoaded", function() {

    const campoSenha = document.getElementById("senha");

    const botaoExibirSenha =
        document.getElementById("btnExibirSenha");

    if (!campoSenha || !botaoExibirSenha) {
        return;
    }

    botaoExibirSenha.addEventListener("click", function() {

        const senhaOculta =
            campoSenha.type === "password";

        campoSenha.type =
            senhaOculta ? "text" : "password";

        botaoExibirSenha.textContent =
            senhaOculta ? "Ocultar" : "Mostrar";

        botaoExibirSenha.setAttribute(
            "aria-pressed",
            String(senhaOculta)
        );

        botaoExibirSenha.setAttribute(
            "aria-label",
            senhaOculta ? "Ocultar senha" : "Exibir senha"
        );

    });

});