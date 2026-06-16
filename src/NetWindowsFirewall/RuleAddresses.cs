using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace KempDec.NetWindowsFirewall;

/// <summary>
/// Operações sobre a lista de endereços remotos de uma regra do Firewall do Windows, representada como uma
/// sequência de endereços separados por vírgula (formato de <c>INetFwRule.RemoteAddresses</c>).
/// </summary>
internal static class RuleAddresses
{
    /// <summary>
    /// Obtém os endereços de IP da lista, sem máscaras de sub-rede e sem duplicatas.
    /// </summary>
    public static List<IPAddress> Parse(string remoteAddresses)
    {
        var ips = new List<IPAddress>();

        foreach (var token in Split(remoteAddresses))
        {
            if (TryParse(token, out IPAddress? ip) && !ips.Contains(ip))
            {
                ips.Add(ip);
            }
        }

        return ips;
    }

    /// <summary>
    /// Adiciona o endereço à lista, sem duplicá-lo caso já esteja presente.
    /// </summary>
    public static string Add(string remoteAddresses, IPAddress ipAddress)
    {
        var tokens = Split(remoteAddresses);

        if (tokens.Any(token => Matches(token, ipAddress)))
        {
            return remoteAddresses;
        }

        return tokens.Length == 0 ? ipAddress.ToString() : $"{remoteAddresses},{ipAddress}";
    }

    /// <summary>
    /// Remove o endereço da lista, comparando por igualdade (e não por substring).
    /// </summary>
    public static string Remove(string remoteAddresses, IPAddress ipAddress) =>
        string.Join(',', Split(remoteAddresses).Where(token => !Matches(token, ipAddress)));

    private static string[] Split(string remoteAddresses) =>
        remoteAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool Matches(string token, IPAddress ipAddress) =>
        TryParse(token, out IPAddress? parsed) && parsed.Equals(ipAddress);

    private static bool TryParse(string token, [NotNullWhen(true)] out IPAddress? ipAddress) =>
        IPAddress.TryParse(token.Split('/', 2)[0], out ipAddress);
}
