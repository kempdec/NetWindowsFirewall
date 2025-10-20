using NetFwTypeLib;
using System.Net;

namespace KempDec.NetWindowsFirewall;

/// <summary>
/// Classe com métodos auxiliares para o Firewall do Windows.
/// </summary>
public class NetWindowsFirewall
{
    /// <summary>
    /// Inicializa uma nova instância de <see cref="NetWindowsFirewall"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public NetWindowsFirewall()
    {
        Type firewallType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2")
            ?? throw new InvalidOperationException("Não foi possível obter o tipo da interface do Firewall do " +
                "Windows.");

        INetFwPolicy2 firewall = Activator.CreateInstance(firewallType) as INetFwPolicy2
            ?? throw new InvalidOperationException("Não foi possível criar uma instância do Firewall do Windows.");

        Firewall = firewall;
    }

    /// <summary>
    /// A instância de <see cref="NetWindowsFirewall"/>.
    /// </summary>
    private static NetWindowsFirewall? s_instance;

    /// <summary>
    /// Obtém a instância do Firewall do Windows.
    /// </summary>
    public INetFwPolicy2 Firewall { get; }

    /// <summary>
    /// Obtém a instância de <see cref="NetWindowsFirewall"/>.
    /// </summary>
    public static NetWindowsFirewall Instance => s_instance ??= new();

    /// <summary>
    /// Adiciona uma nova regra de permissão (de entrada) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddAllowInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de permissão (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddAllowIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "")
    {
        AddAllowInIpRule(name, ipAddress, description, remotePorts);
        AddAllowOutIpRule(name, ipAddress, description, remotePorts);
    }

    /// <summary>
    /// Adiciona uma nova regra de permissão para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="direction">A direção da regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddAllowIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string remotePorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, direction, description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de permissão (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddAllowOutIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de entrada) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddBlockInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddBlockIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "")
    {
        AddBlockInIpRule(name, ipAddress, description, remotePorts);
        AddBlockOutIpRule(name, ipAddress, description, remotePorts);
    }

    /// <summary>
    /// Adiciona uma nova regra de bloqueio para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="direction">A direção da regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddBlockIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string remotePorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, direction, description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    public void AddBlockOutIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP da regra a ser adicionada.</param>
    /// <param name="action">A ação da regra a ser adicionada.</param>
    /// <param name="direction">A direção da regra a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">As portas remotas (TCP) da regra a ser adicionada.</param>
    private void AddIpRule(string name, IPAddress ipAddress, NET_FW_ACTION_ action, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string remotePorts = "")
    {
        INetFwRule rule = GetNewRule();

        rule.Action = action;
        rule.Description = description;
        rule.Direction = direction;
        rule.Enabled = true;
        rule.InterfaceTypes = "All";
        rule.Name = name;
        rule.RemoteAddresses = ipAddress.ToString();

        if (!string.IsNullOrWhiteSpace(remotePorts))
        {
            rule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            rule.LocalPorts = remotePorts;
        }

        AddRule(rule);
    }

    /// <summary>
    /// Adiciona a regra especificada para o Firewall do Windows.
    /// </summary>
    /// <param name="rule">A regra a ser adicionada para o Firewall do Windows.</param>
    public void AddRule(INetFwRule rule) => Firewall.Rules.Add(rule);

    /// <summary>
    /// Obtém uma nova regra do Firewall do Windows.
    /// </summary>
    /// <returns>A nova regra do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException"></exception>
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
    /// Obtém as regras do Firewall do Windows.
    /// </summary>
    /// <returns>As regras do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static INetFwRules GetRules()
    {
        Type policyType = Type.GetTypeFromProgID("HNetCfg.FwPolicy2")
            ?? throw new InvalidOperationException("Não foi possível obter o tipo da interface de política do " +
            "Firewall do Windows.");

        INetFwPolicy2 policy = Activator.CreateInstance(policyType) as INetFwPolicy2
            ?? throw new InvalidOperationException("Não foi possível obter a política do Firewall.");

        return policy.Rules;
    }

    /// <summary>
    /// Obtém os endereços de IPs associados à regra de firewall, se houver alguma com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome da regra no qual os endereços IPs serão buscados.</param>
    /// <returns>Os endereços de IPs associados à regra de firewall, se houver alguma com 
    /// o nome especificado.</returns>
    public static List<IPAddress> GetIpsFromRule(string ruleName)
    {
        var result = new List<IPAddress>();

        INetFwRules rules = GetRules();

        foreach (INetFwRule rule in rules)
        {
            if (rule.Name == ruleName)
            {
                var ips = rule.RemoteAddresses.Split(',', StringSplitOptions.RemoveEmptyEntries
                    | StringSplitOptions.TrimEntries);

                foreach (var ipStr in ips)
                {
                    var cleanIp = ipStr.Split('/')[0];

                    if (IPAddress.TryParse(cleanIp, out IPAddress? ip))
                    {
                        result.Add(ip);
                    }
                }

                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Adiciona o endereço de IP especificado para a regra especificada.
    /// </summary>
    /// <param name="ruleName">O nome da regra ao qual o endereço de IP será adicionado.</param>
    /// <param name="ipAddress">O endereço de IP a ser adicionado.</param>
    /// <returns>Um sinalizador indicando se o endereço de IP especificado foi adicionado para a regra
    /// especificada. Quando o sinalizador é <see langword="false"/> indica que a regra não existe no
    /// Firewall.</returns>
    public static bool AddIpToRule(string ruleName, IPAddress ipAddress)
    {
        INetFwRules rules = GetRules();

        foreach (INetFwRule2 rule in rules)
        {
            if (rule.Name != ruleName)
            {
                continue;
            }

            rule.RemoteAddresses += $",{ipAddress}";

            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove o endereço de IP especificado para a regra especificada.
    /// </summary>
    /// <param name="ruleName">O nome da regra ao qual o endereço de IP será removido.</param>
    /// <param name="ipAddress">O endereço de IP a ser removido.</param>
    /// <returns>Um sinalizador indicando se o endereço de IP especificado foi removido da regra
    /// especificada. Quando o sinalizador é <see langword="false"/> indica que a regra não existe no
    /// Firewall.</returns>
    public static bool RemoveIpToRule(string ruleName, IPAddress ipAddress)
    {
        INetFwRules rules = GetRules();

        foreach (INetFwRule rule in rules)
        {
            if (rule.Name != ruleName)
            {
                continue;
            }

            var ips = rule.RemoteAddresses.Split(',').ToList();

            ips.RemoveAll(e => e.Contains(ipAddress.ToString()));

            rule.RemoteAddresses = string.Join(',', ips);

            return true;
        }

        return false;
    }
}
