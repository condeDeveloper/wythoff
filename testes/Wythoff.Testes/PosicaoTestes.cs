using Conde.Wythoff;
using Xunit;

namespace Conde.Wythoff.Testes;

public class PosicaoTestes
{
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(3, 5, 3, 5)]
    [InlineData(5, 3, 3, 5)]
    [InlineData(10, 1, 1, 10)]
    public void NormalPoeAMenorPrimeiro(int a, int b, int esperadoA, int esperadoB)
    {
        var normal = new Posicao(a, b).Normal;
        Assert.Equal(new Posicao(esperadoA, esperadoB), normal);
    }

    [Fact]
    public void SoAPosicaoVaziaEFinal()
    {
        Assert.True(new Posicao(0, 0).Final);
        Assert.False(new Posicao(0, 1).Final);
        Assert.False(new Posicao(1, 0).Final);
        Assert.False(new Posicao(1, 1).Final);
    }

    [Fact]
    public void DaPosicaoVaziaNaoSaiLance()
    {
        Assert.Empty(new Posicao(0, 0).Lances());
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(1, 1, 3)]
    [InlineData(2, 3, 7)]
    [InlineData(3, 5, 11)]
    [InlineData(4, 4, 12)]
    public void AQuantidadeDeLancesEAeMaisBeMaisOMenor(int a, int b, int esperado)
    {
        Assert.Equal(esperado, new Posicao(a, b).Lances().Count());
        Assert.Equal(a + b + Math.Min(a, b), esperado);
    }

    [Fact]
    public void TodoLanceDiminuiOTotalDeFichas()
    {
        var origem = new Posicao(6, 9);
        foreach (var destino in origem.Lances())
        {
            Assert.True(destino.A + destino.B < origem.A + origem.B);
            Assert.True(destino.A >= 0 && destino.B >= 0);
        }
    }

    [Fact]
    public void NenhumLanceAumentaAlgumaPilha()
    {
        var origem = new Posicao(7, 4);
        foreach (var destino in origem.Lances())
        {
            Assert.True(destino.A <= origem.A);
            Assert.True(destino.B <= origem.B);
        }
    }

    [Fact]
    public void OsLancesDaDiagonalMantemADiferenca()
    {
        var origem = new Posicao(5, 8);
        var diagonais = origem.Lances().Where(p => origem.A - p.A == origem.B - p.B && origem.A - p.A > 0).ToList();
        Assert.Equal(5, diagonais.Count);
        Assert.All(diagonais, p => Assert.Equal(3, p.B - p.A));
    }

    [Fact]
    public void NaoHaLanceRepetidoQuandoAsPilhasSaoDiferentes()
    {
        var lances = new Posicao(3, 7).Lances().ToList();
        Assert.Equal(lances.Count, lances.Distinct().Count());
    }

    [Fact]
    public void OTextoMostraOPar()
    {
        Assert.Equal("(3, 5)", new Posicao(3, 5).ToString());
    }
}
