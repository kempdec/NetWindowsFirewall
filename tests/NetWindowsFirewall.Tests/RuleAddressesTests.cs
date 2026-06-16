using KempDec.NetWindowsFirewall;
using System.Net;
using Xunit;

namespace NetWindowsFirewall.Tests;

public class RuleAddressesTests
{
    [Fact]
    public void Remove_NaoAfetaEnderecosComPrefixoEmComum()
    {
        var result = RuleAddresses.Remove("10.0.0.1,10.0.0.10,10.0.0.100", IPAddress.Parse("10.0.0.1"));

        Assert.Equal("10.0.0.10,10.0.0.100", result);
    }

    [Fact]
    public void Remove_IgnoraMascaraDeSubrede()
    {
        var result = RuleAddresses.Remove("10.0.0.1/255.255.255.255,10.0.0.2/255.255.255.255",
            IPAddress.Parse("10.0.0.1"));

        Assert.Equal("10.0.0.2/255.255.255.255", result);
    }

    [Fact]
    public void Add_NaoDuplicaEnderecoExistente()
    {
        var result = RuleAddresses.Add("10.0.0.1,10.0.0.2", IPAddress.Parse("10.0.0.1"));

        Assert.Equal("10.0.0.1,10.0.0.2", result);
    }

    [Fact]
    public void Add_AcrescentaEnderecoNovo()
    {
        var result = RuleAddresses.Add("10.0.0.1", IPAddress.Parse("10.0.0.2"));

        Assert.Equal("10.0.0.1,10.0.0.2", result);
    }

    [Fact]
    public void Add_DefineUnicoEnderecoQuandoListaVazia()
    {
        var result = RuleAddresses.Add("", IPAddress.Parse("10.0.0.1"));

        Assert.Equal("10.0.0.1", result);
    }

    [Fact]
    public void Parse_RemoveMascarasEDuplicatas()
    {
        var ips = RuleAddresses.Parse("10.0.0.1/255.255.255.255, 10.0.0.1, 10.0.0.2");

        Assert.Equal([IPAddress.Parse("10.0.0.1"), IPAddress.Parse("10.0.0.2")], ips);
    }

    [Fact]
    public void Parse_RetornaListaVaziaParaEntradaVazia()
    {
        Assert.Empty(RuleAddresses.Parse(""));
    }
}
