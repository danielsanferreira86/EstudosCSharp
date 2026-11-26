document.addEventListener("DOMContentLoaded", function () {

    // LOCALIZAR OS ELEMENTOS DO FORMULÁRIO

    const codigo = document.querySelector("[data-ibge-codigo]");
    const nome = document.querySelector("[data-ibge-nome]");
    const uf = document.querySelector("[data-ibge-uf]");

    const btnConsultar =
        document.getElementById("btnConsultarIbge");

    const btnSalvar =
        document.getElementById("btnSalvarEstado");

    const mensagem =
        document.getElementById("mensagemIbge");

    const formulario =
        document.getElementById("formEstado");

    // VERIFICAR SE ESTAMOS NA PÁGINA CORRETA

    if (!codigo || !nome || !uf || !btnConsultar
        || !btnSalvar || !mensagem || !formulario) {

        return;
    }

    let codigoValidado = null;
    let controlador = null;

    // EXIBIR MENSAGEM

    function exibirMensagem(texto, tipo) {

        mensagem.textContent = texto;

        mensagem.className =
            "cad-ibge-message cad-ibge-" + tipo;

        mensagem.hidden = false;
    }

    // LIMPAR OS DADOS OFICIAIS

    function limparDados() {

        nome.value = "";
        uf.value = "";

        codigoValidado = null;
        btnSalvar.disabled = true;
    }

    // CONSULTAR O IBGE

    async function consultarIbge() {

        if (controlador) {

            controlador.abort();
        }

        limparDados();

        const codigoInformado = codigo.value.trim();

        if (!/^[0-9]{2}$/.test(codigoInformado)) {

            btnConsultar.disabled = false;

            exibirMensagem(
                "Informe um código IBGE de dois dígitos.",
                "error");

            return;
        }

        const requisicao = new AbortController();

        controlador = requisicao;

        btnConsultar.disabled = true;

        exibirMensagem(
            "Consultando a base oficial do IBGE...",
            "loading");

        try {

            // MONTAR A URL DA CONSULTA

            const url = new URL(
                formulario.dataset.ibgeUrl,
                window.location.href);

            url.searchParams.set(
                "codigoIbge",
                codigoInformado);

            // EXECUTAR A REQUISIÇÃO

            const resposta = await fetch(url, {

                method: "GET",

                signal: requisicao.signal,

                headers: {
                    "Accept": "application/json"
                }
            });

            const resultado = await resposta.json();

            // IGNORAR RESPOSTAS DE CONSULTAS ANTIGAS

            if (controlador !== requisicao
                || codigo.value.trim() !== codigoInformado) {

                return;
            }

            // VALIDAR O RESULTADO

            if (!resposta.ok || !resultado.sucesso) {

                exibirMensagem(
                    resultado.mensagem
                    || "Código IBGE não localizado.",
                    "error");

                return;
            }

            // PREENCHER OS CAMPOS

            nome.value = resultado.nome;
            uf.value = resultado.uf;

            codigoValidado = codigoInformado;

            btnSalvar.disabled = false;

            exibirMensagem(
                "Unidade federativa localizada com sucesso.",
                "success");

        }
        catch (erro) {

            if (erro.name === "AbortError"
                || controlador !== requisicao) {

                return;
            }

            limparDados();

            exibirMensagem(
                "Não foi possível realizar a consulta.",
                "error");
        }
        finally {

            if (controlador === requisicao) {

                btnConsultar.disabled = false;

                controlador = null;
            }
        }
    }

    // EVENTO DO BOTÃO CONSULTAR

    btnConsultar.addEventListener(
        "click",
        consultarIbge);

    // CONSULTA AUTOMÁTICA AO DIGITAR O CÓDIGO

    codigo.addEventListener("input", function () {

        if (controlador) {

            controlador.abort();
            controlador = null;
        }

        codigo.value = codigo.value
            .replace(/[^0-9]/g, "")
            .slice(0, 2);

        limparDados();

        btnConsultar.disabled = false;

        mensagem.hidden = true;

        if (codigo.value.length === 2) {

            consultarIbge();
        }
    });

    // VALIDAR ANTES DO ENVIO DO FORMULÁRIO

    formulario.addEventListener("submit", function (event) {

        if (codigoValidado !== codigo.value.trim()) {

            event.preventDefault();

            exibirMensagem(
                "Consulte um código IBGE válido antes de salvar.",
                "error");
        }
    });

    // REVALIDAR UM CÓDIGO EXISTENTE
    // EX.: RETORNO DO FORMULÁRIO APÓS ERRO NO POST

    if (/^[0-9]{2}$/.test(codigo.value.trim())) {

        consultarIbge();

    } else {

        limparDados();
    }

});