# Política de segurança

## Versões que recebem correções

| Versão | Recebe correções de segurança |
| ------ | ----------------------------- |
| A mais nova publicada no NuGet | Sim |
| As anteriores | Não: atualize para a mais nova |

## Como relatar uma vulnerabilidade

Não abra uma issue pública. Use o [relato privado de vulnerabilidades do GitHub](https://github.com/kempdec/NetWindowsFirewall/security/advisories/new)
ou escreva para **contato@kempdec.com**, e diga:

- o que acontece e o que poderia ser feito com isso;
- a versão do NetWindowsFirewall, a do .NET e a do Windows;
- os passos para reproduzir, sem endereços nem dados reais.

A KempDec responde em até 5 dias úteis com a avaliação e o prazo da correção, e avisa quando ela for publicada.

## O que já é cuidado pela biblioteca

- Tirar um endereço de uma regra compara o endereço exato, e nunca tira o último endereço de uma regra: uma regra sem
  endereço valeria para todos os endereços.
- As alterações no Firewall exigem permissão de administrador, que é dada pelo Windows, e não pela biblioteca.
