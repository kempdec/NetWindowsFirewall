# NetWindowsFirewall

[![Conferir](https://github.com/kempdec/NetWindowsFirewall/actions/workflows/ci.yml/badge.svg)](https://github.com/kempdec/NetWindowsFirewall/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/KempDec.NetWindowsFirewall)](https://www.nuget.org/packages/KempDec.NetWindowsFirewall)

Gerencie as regras do Firewall do Windows a partir do seu programa .NET: bloqueie ou permita um endereço de IP, e
acrescente ou tire endereços de uma regra que já existe.

```csharp
using KempDec.NetWindowsFirewall;
using System.Net;

// Bloqueia, na entrada e na saída, as conexões com 203.0.113.10.
NetWindowsFirewall.Instance.AddBlockIpRule("Endereços bloqueados", IPAddress.Parse("203.0.113.10"));
```

O que ele garante:

- **Tirar um endereço tira só ele.** Tirar `1.2.3.4` de uma regra não tira `11.2.3.45` nem `1.2.3.40`.
- **Uma regra nunca fica sem endereço sem você pedir.** Uma regra sem endereço vale para todos os endereços: uma regra de
  bloqueio passaria a bloquear tudo. Tirar o último endereço lança um erro e não altera nada.
- **Acrescentar um endereço que já está na regra não o duplica.**
- **As regras de entrada e de saída andam juntas.** `AddBlockIpRule` e `AddAllowIpRule` criam duas regras com o mesmo
  nome, e acrescentar ou tirar um endereço vale para as duas.

A biblioteca usa a interface COM do próprio Windows para o Firewall (`NetFwTypeLib`), sem programas externos e sem
chamar o `netsh`.

## Índice

- [Requisitos](#requisitos)
- [Instalação](#instalação)
- [Como usar](#como-usar)
- [Métodos disponíveis](#métodos-disponíveis)
- [Erros](#erros)
- [Segurança](#segurança)
- [Contribuição](#contribuição)
- [Licença](#licença)

## Requisitos

- **Windows.** O Firewall do Windows só existe no Windows, e o serviço dele (`mpssvc`) precisa estar ativo.
- **.NET 8 ou mais novo**, com o sistema de destino Windows (`net8.0-windows` ou `net10.0-windows`, por exemplo).
- **Permissão de administrador** para criar, alterar ou apagar regras.

## Instalação

1. Instale o pacote pelo NuGet:

   ```sh
   dotnet add package KempDec.NetWindowsFirewall
   ```

2. Mude o **sistema de destino** do seu projeto para Windows. No arquivo `.csproj`, acrescente `-windows` depois da
   versão do .NET:

   ```xml
   <!-- Antes. -->
   <TargetFramework>net8.0</TargetFramework>
   <!-- Depois. -->
   <TargetFramework>net8.0-windows</TargetFramework>
   ```

   No Visual Studio, o mesmo fica nas **Propriedades do projeto (Alt + Enter)**, em **SO de Destino**:

   ![SO de Destino no Visual Studio](assets/target-os.png)

3. Rode o programa como administrador. Num serviço do Windows, use uma conta com essa permissão.

## Como usar

Use a instância compartilhada, `NetWindowsFirewall.Instance`, ou crie a sua com `new NetWindowsFirewall()`. As duas
fazem o mesmo.

### Bloquear e permitir um endereço

```csharp
using KempDec.NetWindowsFirewall;
using System.Net;

var ipAddress = IPAddress.Parse("203.0.113.10");

// Bloqueia o endereço na entrada e na saída (cria duas regras com o mesmo nome).
NetWindowsFirewall.Instance.AddBlockIpRule("Endereços bloqueados", ipAddress);

// Bloqueia só na entrada, com uma descrição que aparece no Firewall do Windows.
NetWindowsFirewall.Instance.AddBlockInIpRule("Entrada bloqueada", ipAddress, description: "Bloqueado pelo meu programa.");

// Permite só na saída.
NetWindowsFirewall.Instance.AddAllowOutIpRule("Saída permitida", ipAddress);
```

### Limitar a regra a portas deste computador

O parâmetro `remotePorts` recebe as **portas locais**, ou seja, as portas deste computador, e não as do outro lado. O
nome ficou assim para não quebrar quem já usa a biblioteca. Quando ele é informado, a regra passa a valer só para TCP.

```csharp
// Permite que 203.0.113.10 se conecte às portas 80 e 443 deste computador, por TCP.
NetWindowsFirewall.Instance.AddAllowInIpRule("Site", ipAddress, remotePorts: "80,443");

// Também aceita faixas de portas.
NetWindowsFirewall.Instance.AddBlockInIpRule("Jogo", ipAddress, remotePorts: "27015-27030");
```

### Acrescentar e tirar endereços de uma regra

```csharp
// Acrescenta um endereço às regras com esse nome (a de entrada e a de saída).
NetWindowsFirewall.AddIpToRule("Endereços bloqueados", IPAddress.Parse("203.0.113.20"));

// Lista os endereços da regra.
List<IPAddress> addresses = NetWindowsFirewall.GetIpsFromRule("Endereços bloqueados");

// Tira só esse endereço das regras.
NetWindowsFirewall.RemoveIpToRule("Endereços bloqueados", IPAddress.Parse("203.0.113.10"));

// Apaga as regras com esse nome, quando elas não são mais necessárias.
NetWindowsFirewall.RemoveRule("Endereços bloqueados");
```

`AddIpToRule` e `RemoveIpToRule` devolvem `false` quando não existe regra com o nome informado, e `RemoveRule` devolve
quantas regras apagou.

A [amostra](samples/NetWindowsFirewall.Sample/Program.cs) mostra tudo isso junto e apaga no fim o que criou.

## Métodos disponíveis

Os métodos de instância criam regras novas:

```csharp
// Permissão.
public void AddAllowIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddAllowInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddAllowOutIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddAllowIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
    string description = "", string remotePorts = "");

// Bloqueio.
public void AddBlockIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddBlockInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddBlockOutIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "");
public void AddBlockIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
    string description = "", string remotePorts = "");

// Uma regra montada por você (veja GetNewRule).
public void AddRule(INetFwRule rule);

// A política do Firewall do Windows, para o que a biblioteca não cobre.
public INetFwPolicy2 Firewall { get; }
```

| Parâmetro | O que é |
| --------- | ------- |
| `name` | O nome da regra, como aparece no Firewall do Windows. |
| `ipAddress` | O endereço remoto: de onde vem ou para onde vai a conexão. |
| `description` | A descrição da regra, como aparece no Firewall do Windows. |
| `remotePorts` | **As portas locais** (deste computador), como `"80"` ou `"80,443,8000-8010"`. Vazio, a regra vale para todas as portas e protocolos. |
| `direction` | `NET_FW_RULE_DIR_IN` (entrada) ou `NET_FW_RULE_DIR_OUT` (saída). |

Os métodos estáticos trabalham com regras que já existem, pelo nome:

```csharp
public static List<IPAddress> GetIpsFromRule(string ruleName);
public static bool AddIpToRule(string ruleName, IPAddress ipAddress);
public static bool RemoveIpToRule(string ruleName, IPAddress ipAddress);
public static int RemoveRule(string ruleName);

// Acesso direto às regras, para o que a biblioteca não cobre.
public static INetFwRules GetRules();
public static INetFwRule GetNewRule();
```

`GetIpsFromRule` devolve os endereços sem a máscara (o Windows grava `1.2.3.4` como `1.2.3.4/255.255.255.255`) e deixa
de fora as faixas e as palavras reservadas, como `LocalSubnet`.

Os métodos que recebem ou devolvem tipos do Firewall (`NET_FW_RULE_DIRECTION_`, `INetFwRule`, `INetFwRules`,
`INetFwPolicy2`) pedem a referência COM `NetFwTypeLib` também no seu projeto:

```xml
<ItemGroup>
  <COMReference Include="NetFwTypeLib">
    <WrapperTool>tlbimp</WrapperTool>
    <VersionMinor>0</VersionMinor>
    <VersionMajor>1</VersionMajor>
    <Guid>58fbcf7c-e7a9-467c-80b3-fc65e8fcca08</Guid>
    <Lcid>0</Lcid>
    <Isolated>false</Isolated>
    <EmbedInteropTypes>true</EmbedInteropTypes>
  </COMReference>
</ItemGroup>
```

Um projeto com referência COM compila pelo Visual Studio ou pelo `msbuild`, e não pelo `dotnet build`.

## Erros

As mensagens de erro dizem o que aconteceu e o que fazer.

| Erro | Quando |
| ---- | ------ |
| `UnauthorizedAccessException` | O programa não roda como administrador e tentou alterar o Firewall. |
| `InvalidOperationException` | O Firewall do Windows não está disponível neste computador, ou `RemoveIpToRule` tiraria o último endereço de uma regra. |
| `ArgumentNullException` | `ipAddress` é `null` em `AddIpToRule` ou `RemoveIpToRule`. |

## Segurança

O Firewall protege o computador inteiro, e uma regra errada abre ou fecha mais do que deveria:

- **Uma regra sem endereço vale para todos os endereços.** Por isso, `RemoveIpToRule` não tira o último endereço de uma
  regra: para deixar de usar a regra, apague-a com `RemoveRule`. Para trocar o endereço, acrescente o novo antes de
  tirar o antigo.
- **O nome é o que identifica a regra.** `AddIpToRule`, `RemoveIpToRule` e `RemoveRule` alteram todas as regras com o
  nome informado, inclusive as que outro programa criou com o mesmo nome. Use nomes próprios do seu programa.
- **Rode como administrador só o que precisa disso.** Num serviço, separe a parte que altera o Firewall do resto do
  programa.
- **Confira o endereço que vem de fora.** Use `IPAddress.TryParse` antes de criar uma regra com um endereço digitado ou
  recebido pela rede.

Para relatar uma vulnerabilidade, siga a [política de segurança](SECURITY.md), e não abra uma issue pública.

## Contribuição

Correções e melhorias são bem-vindas. Veja [como contribuir](CONTRIBUTING.md): como compilar, testar e escrever as
mensagens de commit.

O NetWindowsFirewall é mantido pela [KempDec](https://kempdec.com).

## Licença

[MIT](LICENSE)
