using Conde.Wythoff;
using Xunit;

namespace Conde.Wythoff.Testes;

/// <summary>
/// Os números do README presos aqui. Se o código mudar e algum número mudar,
/// este arquivo quebra antes de o texto mentir. Já aconteceu de eu digitar
/// dentro de um arquivo como este um valor que eu achava certo em vez do valor
/// medido, então aqui não entra nada que eu não tenha visto o medidor imprimir.
/// </summary>
public class MedidasTestes
{
    [Fact]
    public void ATabelaDasFriasFechaNosTresCaminhos()
    {
        var juiz = new Juiz(200);
        var linhas = Medidas.Frias(20, juiz);

        Assert.Equal(20, linhas.Count);
        Assert.All(linhas, l => Assert.True(l.JuizConcorda));
        Assert.All(linhas, l => Assert.True(l.RaizConcorda));
        Assert.All(linhas, l => Assert.True(l.DoubleConcorda));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(5, 8, 13)]
    [InlineData(10, 16, 26)]
    [InlineData(15, 24, 39)]
    [InlineData(19, 30, 49)]
    public void AsLinhasDaTabelaDasFrias(int n, int a, int b)
    {
        var juiz = new Juiz(200);
        var linha = Medidas.Frias(20, juiz)[n];
        Assert.Equal(a, linha.A);
        Assert.Equal(b, linha.B);
    }

    [Theory]
    [InlineData(10, 121, 9, 11, 18, 16)]
    [InlineData(20, 441, 17, 21, 36, 60)]
    [InlineData(50, 2601, 39, 51, 88, 379)]
    [InlineData(100, 10201, 77, 101, 176, 1492)]
    [InlineData(200, 40401, 153, 201, 352, 5920)]
    public void ATabelaDoConfrontoComONim(int limite, int posicoes, int friasW, int friasNim, int divergem, int soDiagonal)
    {
        var linha = Medidas.Confronto([limite])[0];
        Assert.Equal(posicoes, linha.Posicoes);
        Assert.Equal(friasW, linha.FriasWythoff);
        Assert.Equal(friasNim, linha.FriasNim);
        Assert.Equal(divergem, linha.Divergem);
        Assert.Equal(soDiagonal, linha.SoDiagonal);
    }

    [Fact]
    public void ONimTemSempreMaisPosicoesFriasQueOWythoff()
    {
        foreach (var l in Medidas.Confronto([10, 20, 50, 100, 200]))
        {
            Assert.True(l.FriasNim > l.FriasWythoff);
        }
    }

    [Theory]
    [InlineData(0, 77)]
    [InlineData(1, 15468)]
    [InlineData(2, 3600)]
    [InlineData(3, 1156)]
    public void ATabelaDosLancesVencedores(int lances, int quantas)
    {
        var juiz = new Juiz(200);
        var linha = Medidas.Lances(juiz).Single(l => l.Lances == lances);
        Assert.Equal(quantas, linha.Quantas);
    }

    [Fact]
    public void NenhumaPosicaoTemMaisDeTresLancesVencedores()
    {
        var juiz = new Juiz(200);
        var linhas = Medidas.Lances(juiz);
        Assert.Equal(3, linhas.Max(l => l.Lances));
        Assert.Equal(20301, linhas.Sum(l => l.Quantas));
    }

    [Theory]
    [InlineData(10, 7, 6.2)]
    [InlineData(100, 63, 61.8)]
    [InlineData(1000, 619, 618.0)]
    [InlineData(10000, 6181, 6180.3)]
    [InlineData(100000, 61805, 61803.4)]
    public void ATabelaDaDensidade(int limite, int frias, double porFi)
    {
        var linha = Medidas.Densidade([limite])[0];
        Assert.Equal(frias, linha.Frias);
        Assert.Equal(porFi, linha.PorFi);
    }

    [Fact]
    public void ADensidadeNuncaSeAfastaMaisQueDuasPosicoesDaConta()
    {
        foreach (var l in Medidas.Densidade([10, 100, 1000, 10000, 100000]))
        {
            Assert.True(Math.Abs(l.Frias - l.PorFi) < 2.0);
        }
    }

    [Fact]
    public void ODoubleAcertaOsPrimeirosCemMilhoes()
    {
        Assert.Equal(0, Medidas.DoubleQuebra(100_000_000));
    }

    [Fact]
    public void ATabelaDosDesacordos()
    {
        var d = Medidas.Desacordos(300_000_000, 8);
        Assert.Equal(8, d.Count);
        Assert.Equal(102334155, d[0].N);
        Assert.Equal("Fibonacci", d[0].Familia);
        Assert.Equal(141422324, d[1].N);
        Assert.Equal("Lucas", d[1].Familia);
        Assert.Equal(180510493, d[2].N);
        Assert.Equal(204668310, d[3].N);
        Assert.Equal(219598662, d[4].N);
        Assert.Equal(243756479, d[5].N);
        Assert.Equal(267914296, d[6].N);
        Assert.Equal("Fibonacci", d[6].Familia);
        Assert.Equal(282844648, d[7].N);
    }

    [Fact]
    public void NenhumDosOitoPrimeirosDesacordosEUmNumeroQualquer()
    {
        var d = Medidas.Desacordos(300_000_000, 8);
        Assert.Equal(2, d.Count(x => x.Familia == "Fibonacci"));
        Assert.Equal(1, d.Count(x => x.Familia == "Lucas"));
        Assert.Equal(3, d.Count(x => x.Familia == "F+F"));
        Assert.Equal(2, d.Count(x => x.Familia == "outro"));
    }
}
