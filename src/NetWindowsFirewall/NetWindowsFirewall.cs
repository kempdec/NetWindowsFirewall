using NetFwTypeLib;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace KempDec.NetWindowsFirewall;

/// <summary>
/// Métodos auxiliares para gerenciar as regras do Firewall do Windows.
/// </summary>
public class NetWindowsFirewall
{
    private static readonly Lazy<NetWindowsFirewall> s_instance = new(() => new NetWindowsFirewall());

    /// <summary>
    /// Inicializa uma nova instância de <see cref="NetWindowsFirewall"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">A interface COM do Firewall do Windows
    /// (<c>HNetCfg.FwPolicy2</c>) não pôde ser obtida ou instanciada.</exception>
    public NetWindowsFirewall()
    {
        Type firewallType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2")
            ?? throw new InvalidOperationException("Não foi possível obter o tipo da interface do Firewall do " +
                "Windows.");

        Firewall = Activator.CreateInstance(firewallType) as INetFwPolicy2
            ?? throw new InvalidOperationException("Não foi possível criar uma instância do Firewall do Windows.");
    }

    /// <summary>
    /// Obtém a instância compartilhada de <see cref="NetWindowsFirewall"/>.
    /// </summary>
    public static NetWindowsFirewall Instance => s_instance.Value;

    /// <summary>
    /// Obtém a política do Firewall do Windows associada a esta instância.
    /// </summary>
    public INetFwPolicy2 Firewall { get; }

    /// <summary>
    /// Adiciona uma regra de permissão (de entrada) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddAllowInIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, localPorts);

    /// <summary>
    /// Adiciona uma regra de permissão (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddAllowIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "")
    {
        AddAllowInIpRule(name, ipAddress, description, localPorts);
        AddAllowOutIpRule(name, ipAddress, description, localPorts);
    }

    /// <summary>
    /// Adiciona uma regra de permissão para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido pela regra.</param>
    /// <param name="direction">A direção da regra de permissão.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddAllowIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string localPorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, direction, description, localPorts);

    /// <summary>
    /// Adiciona uma regra de permissão (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddAllowOutIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT,
            description, localPorts);

    /// <summary>
    /// Adiciona uma regra de bloqueio (de entrada) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddBlockInIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, localPorts);

    /// <summary>
    /// Adiciona uma regra de bloqueio (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddBlockIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "")
    {
        AddBlockInIpRule(name, ipAddress, description, localPorts);
        AddBlockOutIpRule(name, ipAddress, description, localPorts);
    }

    /// <summary>
    /// Adiciona uma regra de bloqueio para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado pela regra.</param>
    /// <param name="direction">A direção da regra de bloqueio.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddBlockIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string localPorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, direction, description, localPorts);

    /// <summary>
    /// Adiciona uma regra de bloqueio (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado pela regra.</param>
    /// <param name="description">A descrição da regra.</param>
    /// <param name="localPorts">As portas locais (TCP) às quais a regra se aplica.</param>
    public void AddBlockOutIpRule(string name, IPAddress ipAddress, string description = "", string localPorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT,
            description, localPorts);

    private void AddIpRule(string name, IPAddress ipAddress, NET_FW_ACTION_ action, NET_FW_RULE_DIRECTION_ direction,
        string description, string localPorts)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(ipAddress);

        INetFwRule rule = GetNewRule();

        rule.Action = action;
        rule.Description = description;
        rule.Direction = direction;
        rule.Enabled = true;
        rule.InterfaceTypes = "All";
        rule.Name = name;
        rule.RemoteAddresses = ipAddress.ToString();

        if (!string.IsNullOrWhiteSpace(localPorts))
        {
            rule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            rule.LocalPorts = localPorts;
        }

        AddRule(rule);
    }

    /// <summary>
    /// Adiciona a regra especificada ao Firewall do Windows.
    /// </summary>
    /// <param name="rule">A regra a ser adicionada.</param>
    /// <exception cref="ArgumentNullException"><paramref name="rule"/> é <see langword="null"/>.</exception>
    public void AddRule(INetFwRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        Firewall.Rules.Add(rule);
    }

    /// <summary>
    /// Cria uma nova regra (vazia) do Firewall do Windows.
    /// </summary>
    /// <returns>A nova regra do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException">A interface COM da regra (<c>HNetCfg.FWRule</c>) não pôde ser
    /// obtida ou instanciada.</exception>
    public static INetFwRule GetNewRule()
    {
        Type ruleType = Type.GetTypeFromProgID("HNetCfg.FWRule")
            ?? throw new InvalidOperationException("Não foi possível obter o tipo da interface da regra do " +
                "Firewall do Windows.");

        return Activator.CreateInstance(ruleType) as INetFwRule
            ?? throw new InvalidOperationException("Não foi possível criar uma instância da regra do Firewall do " +
                "Windows.");
    }

    /// <summary>
    /// Obtém todas as regras do Firewall do Windows.
    /// </summary>
    /// <returns>As regras do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException">A política do Firewall do Windows não pôde ser obtida.</exception>
    public static INetFwRules GetRules() => Instance.Firewall.Rules;

    /// <summary>
    /// Obtém os endereços de IP associados às regras com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome das regras nas quais os endereços de IP serão buscados.</param>
    /// <returns>Os endereços de IP associados às regras com o nome especificado, sem duplicatas. A lista é vazia
    /// quando nenhuma regra com esse nome existe.</returns>
    /// <exception cref="ArgumentException"><paramref name="ruleName"/> é <see langword="null"/> ou vazio.</exception>
    public static List<IPAddress> GetIpsFromRule(string ruleName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleName);

        var ips = new List<IPAddress>();

        foreach (INetFwRule rule in FindRulesByName(ruleName))
        {
            foreach (var address in SplitAddresses(rule.RemoteAddresses))
            {
                if (TryParseAddress(address, out IPAddress? ip) && !ips.Contains(ip))
                {
                    ips.Add(ip);
                }
            }
        }

        return ips;
    }

    /// <summary>
    /// Adiciona o endereço de IP especificado às regras com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome das regras às quais o endereço de IP será adicionado.</param>
    /// <param name="ipAddress">O endereço de IP a ser adicionado.</param>
    /// <returns><see langword="true"/> se ao menos uma regra com o nome especificado foi encontrada; caso
    /// contrário, <see langword="false"/>. Endereços já presentes na regra não são duplicados.</returns>
    /// <exception cref="ArgumentException"><paramref name="ruleName"/> é <see langword="null"/> ou vazio.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ipAddress"/> é <see langword="null"/>.</exception>
    public static bool AddIpToRule(string ruleName, IPAddress ipAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleName);
        ArgumentNullException.ThrowIfNull(ipAddress);

        var found = false;

        foreach (INetFwRule rule in FindRulesByName(ruleName))
        {
            found = true;

            var addresses = SplitAddresses(rule.RemoteAddresses);

            if (addresses.Any(address => MatchesAddress(address, ipAddress)))
            {
                continue;
            }

            rule.RemoteAddresses = addresses.Length == 0 ? ipAddress.ToString() : $"{rule.RemoteAddresses},{ipAddress}";
        }

        return found;
    }

    /// <summary>
    /// Remove o endereço de IP especificado das regras com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome das regras das quais o endereço de IP será removido.</param>
    /// <param name="ipAddress">O endereço de IP a ser removido.</param>
    /// <returns><see langword="true"/> se ao menos uma regra com o nome especificado foi encontrada; caso
    /// contrário, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="ruleName"/> é <see langword="null"/> ou vazio.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="ipAddress"/> é <see langword="null"/>.</exception>
    public static bool RemoveIpFromRule(string ruleName, IPAddress ipAddress)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ruleName);
        ArgumentNullException.ThrowIfNull(ipAddress);

        var found = false;

        foreach (INetFwRule rule in FindRulesByName(ruleName))
        {
            found = true;

            var addresses = SplitAddresses(rule.RemoteAddresses).Where(address => !MatchesAddress(address, ipAddress));

            rule.RemoteAddresses = string.Join(',', addresses);
        }

        return found;
    }

    /// <summary>
    /// Remove o endereço de IP especificado das regras com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome das regras das quais o endereço de IP será removido.</param>
    /// <param name="ipAddress">O endereço de IP a ser removido.</param>
    /// <returns><see langword="true"/> se ao menos uma regra com o nome especificado foi encontrada; caso
    /// contrário, <see langword="false"/>.</returns>
    [Obsolete("Renomeado para RemoveIpFromRule. Este método será removido em uma versão futura.")]
    public static bool RemoveIpToRule(string ruleName, IPAddress ipAddress) => RemoveIpFromRule(ruleName, ipAddress);

    private static IEnumerable<INetFwRule> FindRulesByName(string ruleName)
    {
        foreach (INetFwRule rule in GetRules())
        {
            if (rule.Name == ruleName)
            {
                yield return rule;
            }
        }
    }

    private static string[] SplitAddresses(string remoteAddresses) =>
        remoteAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool MatchesAddress(string address, IPAddress ipAddress) =>
        TryParseAddress(address, out IPAddress? parsed) && parsed.Equals(ipAddress);

    private static bool TryParseAddress(string address, [NotNullWhen(true)] out IPAddress? ipAddress) =>
        IPAddress.TryParse(address.Split('/', 2)[0], out ipAddress);
}
