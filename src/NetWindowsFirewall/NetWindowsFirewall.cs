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
    public NetWindowsFirewall() => Firewall = CreatePolicy();

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
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
    public void AddAllowInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de permissão (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
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
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
    public void AddAllowIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string remotePorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, direction, description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de permissão (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de permissão a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será permitido na regra de permissão a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
    public void AddAllowOutIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_ALLOW, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de entrada) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
    public void AddBlockInIpRule(string name, IPAddress ipAddress, string description = "", string remotePorts = "") =>
        AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN,
            description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de entrada e saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
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
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
    public void AddBlockIpRule(string name, IPAddress ipAddress, NET_FW_RULE_DIRECTION_ direction,
        string description = "", string remotePorts = "") =>
            AddIpRule(name, ipAddress, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, direction, description, remotePorts);

    /// <summary>
    /// Adiciona uma nova regra de bloqueio (de saída) para um endereço de IP no Firewall do Windows.
    /// </summary>
    /// <param name="name">O nome da regra de bloqueio a ser adicionada.</param>
    /// <param name="ipAddress">O endereço de IP que será bloqueado na regra de bloqueio a ser adicionada.</param>
    /// <param name="description">A descrição da regra a ser adicionada.</param>
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
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
    /// <param name="remotePorts">
    /// As portas <b>locais</b> (TCP) da regra, como <c>"80"</c> ou <c>"80,443,8000-8010"</c>: as portas deste
    /// computador. Apesar do nome, o valor vai para as portas locais da regra (<c>LocalPorts</c>), e não para as
    /// remotas; o nome ficou assim para não quebrar quem já usa. Quando informado, a regra passa a valer só para TCP.
    /// Vazio, a regra vale para todas as portas e protocolos.
    /// </param>
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
    /// <exception cref="UnauthorizedAccessException">O programa não roda como administrador.</exception>
    public void AddRule(INetFwRule rule) => RequireAdministrator(() => Firewall.Rules.Add(rule));

    /// <summary>
    /// Obtém uma nova regra do Firewall do Windows.
    /// </summary>
    /// <returns>A nova regra do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static INetFwRule GetNewRule() => CreateComObject<INetFwRule>(RuleProgId, "uma regra nova");

    /// <summary>
    /// Obtém as regras do Firewall do Windows.
    /// </summary>
    /// <returns>As regras do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static INetFwRules GetRules() => CreatePolicy().Rules;

    /// <summary>
    /// Obtém os endereços de IPs associados à regra de firewall, se houver alguma com o nome especificado.
    /// </summary>
    /// <remarks>
    /// Os endereços vêm sem a máscara (<c>1.2.3.4/255.255.255.255</c> vira <c>1.2.3.4</c>). Faixas de endereços e
    /// palavras reservadas, como <c>LocalSubnet</c>, ficam de fora. Quando há mais de uma regra com o nome
    /// especificado, como a de entrada e a de saída criadas por <see cref="AddBlockIpRule(string, IPAddress, string,
    /// string)"/>, os endereços vêm da primeira.
    /// </remarks>
    /// <param name="ruleName">O nome da regra no qual os endereços IPs serão buscados.</param>
    /// <returns>Os endereços de IPs associados à regra de firewall, se houver alguma com
    /// o nome especificado.</returns>
    public static List<IPAddress> GetIpsFromRule(string ruleName)
    {
        INetFwRule? rule = FindRules(ruleName).FirstOrDefault();

        return rule is null ? [] : RemoteAddressList.GetAddresses(rule.RemoteAddresses);
    }

    /// <summary>
    /// Adiciona o endereço de IP especificado para a regra especificada.
    /// </summary>
    /// <remarks>
    /// O endereço é adicionado a todas as regras com o nome especificado, como a de entrada e a de saída criadas por
    /// <see cref="AddBlockIpRule(string, IPAddress, string, string)"/>. Uma regra que já tem o endereço (com qualquer
    /// máscara) ou que já vale para todos os endereços (<c>*</c>) não muda.
    /// </remarks>
    /// <param name="ruleName">O nome da regra ao qual o endereço de IP será adicionado.</param>
    /// <param name="ipAddress">O endereço de IP a ser adicionado.</param>
    /// <returns>Um sinalizador indicando se o endereço de IP especificado foi adicionado para a regra
    /// especificada. Quando o sinalizador é <see langword="false"/> indica que a regra não existe no
    /// Firewall.</returns>
    public static bool AddIpToRule(string ruleName, IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        List<INetFwRule> rules = FindRules(ruleName);

        foreach (INetFwRule rule in rules)
        {
            string remoteAddresses = RemoteAddressList.Add(rule.RemoteAddresses, ipAddress);

            if (remoteAddresses != rule.RemoteAddresses)
            {
                RequireAdministrator(() => rule.RemoteAddresses = remoteAddresses);
            }
        }

        return rules.Count > 0;
    }

    /// <summary>
    /// Remove o endereço de IP especificado para a regra especificada.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Sai da regra só o endereço especificado, com qualquer máscara: tirar <c>1.2.3.4</c> não tira
    /// <c>11.2.3.45</c>. O endereço sai de todas as regras com o nome especificado, como a de entrada e a de saída
    /// criadas por <see cref="AddBlockIpRule(string, IPAddress, string, string)"/>.
    /// </para>
    /// <para>
    /// Uma regra sem endereço vale para todos os endereços: uma regra de bloqueio passaria a bloquear tudo. Por isso,
    /// quando o endereço é o único de alguma das regras, nada é alterado e uma <see cref="InvalidOperationException"/>
    /// é lançada. Para deixar de usar a regra, apague-a com <see cref="RemoveRule(string)"/>.
    /// </para>
    /// </remarks>
    /// <param name="ruleName">O nome da regra ao qual o endereço de IP será removido.</param>
    /// <param name="ipAddress">O endereço de IP a ser removido.</param>
    /// <returns>Um sinalizador indicando se o endereço de IP especificado foi removido da regra
    /// especificada. Quando o sinalizador é <see langword="false"/> indica que a regra não existe no
    /// Firewall.</returns>
    /// <exception cref="InvalidOperationException">O endereço é o único de alguma das regras.</exception>
    public static bool RemoveIpToRule(string ruleName, IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);

        List<INetFwRule> rules = FindRules(ruleName);
        var changes = new List<(INetFwRule Rule, string RemoteAddresses)>();

        // Confere todas as regras antes de alterar alguma: se uma delas ficaria sem endereço, nenhuma muda.
        foreach (INetFwRule rule in rules)
        {
            if (!RemoteAddressList.Contains(rule.RemoteAddresses, ipAddress))
            {
                continue;
            }

            string remoteAddresses = RemoteAddressList.Remove(rule.RemoteAddresses, ipAddress);

            if (remoteAddresses.Length == 0)
            {
                throw new InvalidOperationException($"O endereço {ipAddress} é o único da regra \"{ruleName}\", e " +
                    "uma regra sem endereço vale para todos os endereços. Para deixar de usar a regra, apague-a com " +
                    $"{nameof(RemoveRule)}; para trocar o endereço, adicione o novo antes de remover o antigo.");
            }

            changes.Add((rule, remoteAddresses));
        }

        foreach ((INetFwRule rule, string remoteAddresses) in changes)
        {
            RequireAdministrator(() => rule.RemoteAddresses = remoteAddresses);
        }

        return rules.Count > 0;
    }

    /// <summary>
    /// Remove do Firewall do Windows todas as regras com o nome especificado, como a de entrada e a de saída criadas
    /// por <see cref="AddBlockIpRule(string, IPAddress, string, string)"/>.
    /// </summary>
    /// <param name="ruleName">O nome das regras a serem removidas.</param>
    /// <returns>A quantidade de regras removidas. Zero indica que nenhuma regra tem o nome especificado.</returns>
    public static int RemoveRule(string ruleName)
    {
        INetFwRules rules = GetRules();
        int count = FindRules(rules, ruleName).Count;

        // Remove uma de cada vez e confere o que sobrou, porque a remoção pelo nome não garante tirar todas as regras
        // com o mesmo nome numa chamada só. O limite evita repetir sem fim se alguma regra não puder ser removida.
        for (int i = 0; i < count && FindRules(rules, ruleName).Count > 0; i++)
        {
            RequireAdministrator(() => rules.Remove(ruleName));
        }

        return count - FindRules(rules, ruleName).Count;
    }

    /// <summary>
    /// Obtém as regras do Firewall do Windows com o nome especificado.
    /// </summary>
    /// <param name="ruleName">O nome das regras.</param>
    /// <returns>As regras com o nome especificado, na ordem do Firewall.</returns>
    private static List<INetFwRule> FindRules(string ruleName) => FindRules(GetRules(), ruleName);

    /// <summary>
    /// Obtém as regras com o nome especificado.
    /// </summary>
    /// <param name="rules">As regras do Firewall do Windows.</param>
    /// <param name="ruleName">O nome das regras.</param>
    /// <returns>As regras com o nome especificado, na ordem do Firewall.</returns>
    private static List<INetFwRule> FindRules(INetFwRules rules, string ruleName)
    {
        var result = new List<INetFwRule>();

        foreach (INetFwRule rule in rules)
        {
            if (rule.Name == ruleName)
            {
                result.Add(rule);
            }
        }

        return result;
    }

    /// <summary>
    /// O identificador COM da política do Firewall do Windows.
    /// </summary>
    private const string PolicyProgId = "HNetCfg.FwPolicy2";

    /// <summary>
    /// O identificador COM de uma regra do Firewall do Windows.
    /// </summary>
    private const string RuleProgId = "HNetCfg.FWRule";

    /// <summary>
    /// Cria uma nova instância da política do Firewall do Windows.
    /// </summary>
    /// <returns>A política do Firewall do Windows.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static INetFwPolicy2 CreatePolicy() => CreateComObject<INetFwPolicy2>(PolicyProgId, "as regras");

    /// <summary>
    /// Cria uma instância do objeto COM com o identificador especificado.
    /// </summary>
    /// <typeparam name="T">A interface do objeto COM.</typeparam>
    /// <param name="progId">O identificador COM do objeto.</param>
    /// <param name="what">O que se queria do Firewall, para a mensagem de erro (como "as regras").</param>
    /// <returns>A instância do objeto COM.</returns>
    /// <exception cref="InvalidOperationException">O Firewall do Windows não está disponível neste computador.
    /// </exception>
    private static T CreateComObject<T>(string progId, string what)
        where T : class
    {
        Type? type = Type.GetTypeFromProgID(progId);

        if (type is not null && Activator.CreateInstance(type) is T instance)
        {
            return instance;
        }

        throw new InvalidOperationException($"Não foi possível obter {what} do Firewall do Windows neste computador. " +
            "Confira se o programa roda no Windows e se o serviço Firewall do Windows (mpssvc) está ativo.");
    }


    /// <summary>
    /// Executa uma alteração no Firewall do Windows, trocando a recusa por falta de permissão por uma mensagem que
    /// diz o que fazer.
    /// </summary>
    /// <param name="change">A alteração no Firewall do Windows.</param>
    /// <exception cref="UnauthorizedAccessException">O programa não roda como administrador.</exception>
    private static void RequireAdministrator(Action change)
    {
        try
        {
            change();
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException("O Windows recusou a alteração no Firewall porque o programa não " +
                "roda como administrador. Rode o programa como administrador e tente de novo.", ex);
        }
    }
}
