using KempDec.NetWindowsFirewall;
using System.Net;

// Este exemplo cria uma regra de bloqueio, adiciona e lista os IPs associados e, por fim, remove um deles.
// Execute como administrador: alterar o Firewall do Windows exige privilégios elevados.
// Os IPs abaixo pertencem à faixa de documentação (TEST-NET-3, RFC 5737) e não roteiam na internet.

const string ruleName = "NetWindowsFirewall - Exemplo de bloqueio";

var firewall = NetWindowsFirewall.Instance;
var ipAddress = IPAddress.Parse("203.0.113.10");

firewall.AddBlockIpRule(ruleName, ipAddress, description: "Regra criada pelo exemplo do NetWindowsFirewall.");
Console.WriteLine($"Regra \"{ruleName}\" criada bloqueando {ipAddress}.");

var anotherIp = IPAddress.Parse("203.0.113.20");
NetWindowsFirewall.AddIpToRule(ruleName, anotherIp);

Console.WriteLine("IPs associados à regra:");

foreach (var ip in NetWindowsFirewall.GetIpsFromRule(ruleName))
{
    Console.WriteLine($"  - {ip}");
}

NetWindowsFirewall.RemoveIpFromRule(ruleName, anotherIp);
Console.WriteLine($"IP {anotherIp} removido da regra.");
