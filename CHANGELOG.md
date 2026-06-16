# Changelog

Todas as mudanças relevantes deste projeto são documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e o projeto adota o
[Versionamento Semântico](https://semver.org/lang/pt-BR/).

## [1.5.0]

### Adicionado

- `RemoveIpFromRule`, em substituição ao `RemoveIpToRule`.
- Validação de argumentos nos métodos públicos.
- Projeto de testes automatizados para a manipulação de endereços das regras.

### Corrigido

- A remoção de um IP passou a comparar endereços por igualdade, e não por substring: remover `10.0.0.1` não
  afeta mais `10.0.0.10` ou `10.0.0.100`.
- `GetIpsFromRule`, `AddIpToRule` e `RemoveIpFromRule` percorrem todas as regras com o mesmo nome, e não apenas
  a primeira (as regras de entrada e saída compartilham o nome).
- `AddIpToRule` não duplica mais endereços já presentes na regra.

### Alterado

- O parâmetro de portas dos métodos `Add*IpRule` foi renomeado de `remotePorts` para `localPorts`, refletindo o
  valor que de fato é configurado na regra.
- `Instance` passou a ser inicializado de forma thread-safe.
- O empacotamento passou a ocorrer no CI e os arquivos `.nupkg` deixaram de ser versionados.

### Obsoleto

- `RemoveIpToRule`: use `RemoveIpFromRule`.

[1.5.0]: https://github.com/kempdec/NetWindowsFirewall/releases/tag/v1.5.0
