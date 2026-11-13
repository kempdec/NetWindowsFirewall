using System.Net;

namespace KempDec.NetWindowsFirewall.Tests;

public class RemoteAddressListTests
{
    private static readonly IPAddress s_address = IPAddress.Parse("1.2.3.4");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" , ,")]
    public void Split_SemItens_VoltaVazia(string? remoteAddresses) =>
        Assert.Empty(RemoteAddressList.Split(remoteAddresses));

    [Fact]
    public void Split_TiraEspacosEItensVazios() =>
        Assert.Equal(["1.2.3.4/255.255.255.255", "LocalSubnet", "10.0.0.1-10.0.0.9"],
            RemoteAddressList.Split(" 1.2.3.4/255.255.255.255 ,, LocalSubnet,10.0.0.1-10.0.0.9 "));

    [Theory]
    [InlineData("1.2.3.4", "1.2.3.4")]
    [InlineData("1.2.3.4/255.255.255.255", "1.2.3.4")]
    [InlineData("1.2.3.0/24", "1.2.3.0")]
    [InlineData("2001:db8::1/128", "2001:db8::1")]
    [InlineData(" 1.2.3.4 ", "1.2.3.4")]
    public void TryGetAddress_ComOuSemMascara_DevolveOEndereco(string entry, string expected)
    {
        Assert.True(RemoteAddressList.TryGetAddress(entry, out IPAddress? address));
        Assert.Equal(IPAddress.Parse(expected), address);
    }

    [Theory]
    [InlineData("*")]
    [InlineData("LocalSubnet")]
    [InlineData("10.0.0.1-10.0.0.9")]
    public void TryGetAddress_ItemQueNaoEEndereco_DevolveFalso(string entry) =>
        Assert.False(RemoteAddressList.TryGetAddress(entry, out _));

    [Fact]
    public void GetAddresses_TiraAsMascarasEDeixaDeForaFaixasEPalavras() =>
        Assert.Equal([IPAddress.Parse("1.2.3.4"), IPAddress.Parse("2001:db8::1")],
            RemoteAddressList.GetAddresses("1.2.3.4/255.255.255.255,LocalSubnet,10.0.0.1-10.0.0.9,2001:db8::1/128"));

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("*", true)]
    [InlineData("1.2.3.4,*", true)]
    [InlineData("1.2.3.4", false)]
    [InlineData("LocalSubnet", false)]
    public void CoversAllAddresses_VazioOuAsterisco(string? remoteAddresses, bool expected) =>
        Assert.Equal(expected, RemoteAddressList.CoversAllAddresses(remoteAddresses));

    [Theory]
    [InlineData("1.2.3.4", true)]
    [InlineData("1.2.3.4/255.255.255.255", true)]
    [InlineData("5.6.7.8,1.2.3.4/255.255.255.255", true)]
    [InlineData("11.2.3.45/255.255.255.255", false)]
    [InlineData("1.2.3.40,21.2.3.4", false)]
    [InlineData("1.2.3.4-1.2.3.9", false)]
    [InlineData("*", false)]
    public void Contains_ComparaOEnderecoExato(string remoteAddresses, bool expected) =>
        Assert.Equal(expected, RemoteAddressList.Contains(remoteAddresses, s_address));

    [Fact]
    public void Add_EnderecoNovo_VaiParaOFim() =>
        Assert.Equal("5.6.7.8/255.255.255.255,1.2.3.4",
            RemoteAddressList.Add("5.6.7.8/255.255.255.255", s_address));

    [Theory]
    [InlineData("1.2.3.4")]
    [InlineData("1.2.3.4/255.255.255.255")]
    [InlineData("5.6.7.8/255.255.255.255,1.2.3.4/255.255.255.255")]
    public void Add_EnderecoQueJaEsta_NaoDuplica(string remoteAddresses) =>
        Assert.Equal(remoteAddresses, RemoteAddressList.Add(remoteAddresses, s_address));

    [Fact]
    public void Add_EnderecoParecido_NaoContaComoRepetido() =>
        Assert.Equal("11.2.3.45/255.255.255.255,1.2.3.4",
            RemoteAddressList.Add("11.2.3.45/255.255.255.255", s_address));

    [Theory]
    [InlineData("*")]
    [InlineData("")]
    public void Add_ListaQueValeParaTodos_NaoMuda(string remoteAddresses) =>
        Assert.Equal(remoteAddresses, RemoteAddressList.Add(remoteAddresses, s_address));

    [Fact]
    public void Remove_TiraSoOEnderecoExato() =>
        Assert.Equal("11.2.3.45/255.255.255.255,1.2.3.40/255.255.255.255,21.2.3.4/255.255.255.255",
            RemoteAddressList.Remove(
                "11.2.3.45/255.255.255.255,1.2.3.4/255.255.255.255,1.2.3.40/255.255.255.255,21.2.3.4/255.255.255.255",
                s_address));

    [Fact]
    public void Remove_IgnoraAMascara() =>
        Assert.Equal("5.6.7.8", RemoteAddressList.Remove("1.2.3.4/255.255.255.255,5.6.7.8,1.2.3.4", s_address));

    [Fact]
    public void Remove_MantemFaixasEPalavras() =>
        Assert.Equal("1.2.3.4-1.2.3.9,LocalSubnet",
            RemoteAddressList.Remove("1.2.3.4-1.2.3.9,1.2.3.4,LocalSubnet", s_address));

    [Fact]
    public void Remove_UnicoEndereco_DevolveVazio() =>
        Assert.Equal(string.Empty, RemoteAddressList.Remove("1.2.3.4/255.255.255.255", s_address));

    [Fact]
    public void Remove_EnderecoQueNaoEsta_NaoMuda() =>
        Assert.Equal("5.6.7.8,*", RemoteAddressList.Remove("5.6.7.8,*", s_address));

    [Fact]
    public void Remove_IPv6_ComparaOEnderecoExato() =>
        Assert.Equal("2001:db8::10/128",
            RemoteAddressList.Remove("2001:db8::1/128,2001:db8::10/128", IPAddress.Parse("2001:db8::1")));
}
