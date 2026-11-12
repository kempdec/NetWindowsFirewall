using KempDec.NetWindowsFirewall;
using System.Net;
using System.Security.Principal;

// Exemplo do NetWindowsFirewall: bloqueia um endereço, acrescenta e tira outro da mesma regra, e apaga a regra no fim,
// para não deixar nada no Firewall de quem roda o exemplo. Os endereços são da faixa reservada para documentação
// (203.0.113.0/24), que não pertence a ninguém.

const string RuleName = "Exemplo do NetWindowsFirewall";

if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
{
    Console.WriteLine("O exemplo altera o Firewall do Windows, e isso exige permissão de administrador.");
    Console.WriteLine("Abra o terminal como administrador e rode o exemplo de novo.");

    return 1;
}

IPAddress firstAddress = IPAddress.Parse("203.0.113.10");
IPAddress secondAddress = IPAddress.Parse("203.0.113.20");

// Cria duas regras com o mesmo nome: uma de entrada e uma de saída.
NetWindowsFirewall.Instance.AddBlockIpRule(RuleName, firstAddress,
    description: "Criada pelo exemplo do NetWindowsFirewall.");
Console.WriteLine($"Regra \"{RuleName}\" criada, bloqueando {firstAddress}.");

// Acrescenta um segundo endereço às duas regras. Acrescentar de novo não duplica.
NetWindowsFirewall.AddIpToRule(RuleName, secondAddress);
NetWindowsFirewall.AddIpToRule(RuleName, secondAddress);
Console.WriteLine($"Endereços bloqueados: {string.Join(", ", NetWindowsFirewall.GetIpsFromRule(RuleName))}.");

// Tira o primeiro endereço; o segundo continua bloqueado.
NetWindowsFirewall.RemoveIpToRule(RuleName, firstAddress);
Console.WriteLine($"Endereços bloqueados: {string.Join(", ", NetWindowsFirewall.GetIpsFromRule(RuleName))}.");

// Tirar o último endereço lançaria um erro, porque uma regra sem endereço valeria para todos. Para deixar de bloquear,
// apague a regra.
int removed = NetWindowsFirewall.RemoveRule(RuleName);
Console.WriteLine($"Regras apagadas: {removed}. O Firewall voltou a ser como era antes do exemplo.");

return 0;
