using System.Net;

namespace KempDec.NetWindowsFirewall;

/// <summary>
/// Lê e altera a lista de endereços remotos de uma regra do Firewall do Windows (a propriedade
/// <c>RemoteAddresses</c>), sem acessar o Firewall.
/// </summary>
/// <remarks>
/// A lista é um texto com os itens separados por vírgula. Cada item pode ser um endereço (<c>1.2.3.4</c>), um endereço
/// com máscara (<c>1.2.3.4/255.255.255.255</c> ou <c>1.2.3.0/24</c>), uma faixa (<c>1.2.3.4-1.2.3.10</c>), uma palavra
/// reservada (<c>LocalSubnet</c>) ou <c>*</c>, que quer dizer todos os endereços. O Windows grava um endereço sozinho
/// com a máscara, então a comparação de endereços ignora a máscara. Os itens que não são um endereço são mantidos como
/// estão.
/// </remarks>
internal static class RemoteAddressList
{
    /// <summary>
    /// O item que quer dizer todos os endereços.
    /// </summary>
    public const string AllAddresses = "*";

    /// <summary>
    /// Separa a lista de endereços em itens, sem espaços e sem itens vazios.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <returns>Os itens da lista, na ordem em que aparecem.</returns>
    public static List<string> Split(string? remoteAddresses) =>
        string.IsNullOrWhiteSpace(remoteAddresses)
            ? []
            : [.. remoteAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

    /// <summary>
    /// Indica se a lista vale para todos os endereços: vazia ou com o item <c>*</c>.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <returns><see langword="true"/> se a lista vale para todos os endereços.</returns>
    public static bool CoversAllAddresses(string? remoteAddresses)
    {
        List<string> entries = Split(remoteAddresses);

        return entries.Count == 0 || entries.Contains(AllAddresses);
    }

    /// <summary>
    /// Obtém o endereço de um item da lista, sem a máscara.
    /// </summary>
    /// <param name="entry">O item da lista.</param>
    /// <param name="address">O endereço do item, quando ele é um endereço.</param>
    /// <returns><see langword="true"/> se o item é um endereço, com ou sem máscara.</returns>
    public static bool TryGetAddress(string entry, out IPAddress? address)
    {
        int maskStart = entry.IndexOf('/');
        string addressText = (maskStart >= 0 ? entry[..maskStart] : entry).Trim();

        return IPAddress.TryParse(addressText, out address);
    }

    /// <summary>
    /// Obtém os endereços da lista, sem as máscaras. Faixas e palavras reservadas ficam de fora.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <returns>Os endereços da lista, na ordem em que aparecem.</returns>
    public static List<IPAddress> GetAddresses(string? remoteAddresses)
    {
        var result = new List<IPAddress>();

        foreach (string entry in Split(remoteAddresses))
        {
            if (TryGetAddress(entry, out IPAddress? address))
            {
                result.Add(address!);
            }
        }

        return result;
    }

    /// <summary>
    /// Indica se o endereço está na lista, ignorando a máscara.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <param name="ipAddress">O endereço procurado.</param>
    /// <returns><see langword="true"/> se algum item da lista é exatamente esse endereço.</returns>
    public static bool Contains(string? remoteAddresses, IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        return Split(remoteAddresses).Any(entry => IsAddress(entry, ipAddress));
    }

    /// <summary>
    /// Acrescenta o endereço ao fim da lista, quando ele ainda não está nela.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <param name="ipAddress">O endereço a ser acrescentado.</param>
    /// <returns>
    /// A nova lista. Ela volta igual quando o endereço já está nela ou quando ela já vale para todos os endereços
    /// (vazia ou com <c>*</c>), porque acrescentar um endereço a ela não mudaria nada.
    /// </returns>
    public static string Add(string? remoteAddresses, IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        if (CoversAllAddresses(remoteAddresses) || Contains(remoteAddresses, ipAddress))
        {
            return remoteAddresses ?? string.Empty;
        }

        List<string> entries = Split(remoteAddresses);
        entries.Add(ipAddress.ToString());

        return string.Join(',', entries);
    }

    /// <summary>
    /// Tira da lista os itens que são exatamente o endereço, ignorando a máscara. Endereços parecidos (como
    /// <c>11.2.3.45</c> para <c>1.2.3.4</c>), faixas e palavras reservadas ficam.
    /// </summary>
    /// <param name="remoteAddresses">A lista de endereços da regra.</param>
    /// <param name="ipAddress">O endereço a ser tirado.</param>
    /// <returns>
    /// A nova lista. Ela fica vazia quando o endereço era o único item; quem chama decide o que fazer nesse caso,
    /// porque uma regra sem endereço vale para todos os endereços.
    /// </returns>
    public static string Remove(string? remoteAddresses, IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        List<string> entries = Split(remoteAddresses);
        entries.RemoveAll(entry => IsAddress(entry, ipAddress));

        return string.Join(',', entries);
    }

    /// <summary>
    /// Indica se o item da lista é exatamente o endereço, ignorando a máscara.
    /// </summary>
    private static bool IsAddress(string entry, IPAddress ipAddress) =>
        TryGetAddress(entry, out IPAddress? address) && address!.Equals(ipAddress);
}
