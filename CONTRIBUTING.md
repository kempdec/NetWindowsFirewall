# Como contribuir

Obrigado pelo interesse em contribuir com o NetWindowsFirewall! Toda ajuda é bem-vinda: correções, melhorias,
documentação ou ideias.

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) ou superior.
- Windows (a biblioteca usa a API COM do Firewall do Windows).
- Visual Studio 2022+ ou o **MSBuild** que o acompanha.

> A biblioteca referencia a interface COM `NetFwTypeLib` (`<COMReference>`). Por isso, o `dotnet build` **não**
> compila o projeto: a tarefa `ResolveComReference` só existe no MSBuild do Visual Studio. Compile pelo Visual
> Studio ou com `msbuild`.

## Compilar e testar

```powershell
msbuild NetWindowsFirewall.sln -restore -t:Build -p:Configuration=Release
dotnet test --no-build -c Release
```

Os testes da lógica de manipulação de endereços não dependem do Firewall e rodam em qualquer máquina Windows.

## Padrão de commits

As mensagens de commit seguem o padrão de [Commits Convencionais](https://www.conventionalcommits.org/pt-br/),
pois alimentam a geração automática de release. Use os prefixos `feat`, `fix`, `chore`, `docs` e `refactor`.

## Pull requests

1. Crie um branch a partir da `main`.
2. Mantenha o estilo de código definido no `.editorconfig`.
3. Inclua testes quando alterar comportamento.
4. Descreva claramente o que muda e por quê.
