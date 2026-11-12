# Como contribuir

Correções e melhorias são bem-vindas. Para uma mudança grande, abra uma issue antes, para combinar o caminho.

## Compilar e testar

A biblioteca usa a referência COM do Firewall do Windows (`NetFwTypeLib`), então ela só compila no **Windows**, com o
SDK do .NET da versão do [`global.json`](global.json) e o MSBuild do Visual Studio (o `dotnet build` não compila uma
referência COM):

```powershell
msbuild NetWindowsFirewall.sln -restore -p:Configuration=Release
dotnet test --project tests/NetWindowsFirewall.Tests --configuration Release --no-build
```

Os testes cobrem a lógica que não depende do Firewall, como a lista de endereços de uma regra, e rodam em qualquer
sistema, inclusive sem o Windows:

```sh
dotnet test --project tests/NetWindowsFirewall.Tests
```

Os testes nunca alteram o Firewall de quem roda. Para ver uma mudança no Firewall de verdade, use a
[amostra](samples/NetWindowsFirewall.Sample/Program.cs) como administrador, de preferência numa máquina virtual.

A cada push e pull request, o GitHub Actions compila, testa e gera o pacote no Windows.

## Mudanças na API

Quem usa a biblioteca não pode quebrar ao atualizar:

- não mude nem tire um método público, nem o nome de um parâmetro (quem chama pelo nome quebraria);
- não mude o comportamento de um método sem um motivo forte, e diga no pull request o que muda para quem já usa;
- uma mensagem de erro diz o que aconteceu e o que fazer.

## Mensagens de commit

As releases são geradas pelo [Auto Release](https://github.com/kempdec/AutoRelease) a partir das mensagens de commit.
Escreva só a linha de título, com um destes tipos e uma descrição que começa por um verbo no infinitivo e termina em
ponto:

```text
feat: Adicionar a regra por faixa de endereços.
fix: Corrigir a remoção de um endereço com máscara.
docs: Atualizar os exemplos do README.
```

| Tipo | Quando |
| ---- | ------ |
| `feat` | Um recurso novo para quem usa a biblioteca. |
| `fix` | Uma correção que quem usa a biblioteca percebe. |
| `docs` | Documentação. |
| `refactor` | Uma mudança no código que não muda o comportamento. |
| `chore` | Build, CI e o que é interno. Com `#` (`chore#:`), fica fora das notas da release. |

Os verbos são: Adicionar, Alterar, Atualizar, Remover, Refatorar, Mover, Corrigir e Aprimorar.

## Publicação

A KempDec publica as versões. O fluxo [`publish.yml`](.github/workflows/publish.yml) publica a versão que está em
`<Version>` no projeto da biblioteca quando ela muda no `main`.
