using Conde.Wythoff;
using Xunit;

namespace Conde.Wythoff.Testes;

public class NimTestes
{
    private static readonly Nim Dele = new(60);
    private static readonly Juiz Wythoff = new(60);

    [Fact]
    public void BoutonAcertaTodasAsPosicoes()
    {
        for (var a = 0; a <= Dele.Limite; a++)
        {
            for (var b = 0; b <= Dele.Limite; b++)
            {
                Assert.Equal(Dele.Perde(a, b), Nim.PerdeporBouton(a, b));
            }
        }
    }

    [Fact]
    public void NoNimPerdeQuemRecebeAsPilhasIguais()
    {
        for (var k = 0; k <= Dele.Limite; k++)
        {
            Assert.True(Dele.Perde(k, k));
        }
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(3, 5)]
    [InlineData(7, 2)]
    public void NoNimAsPilhasDiferentesSaoVencedoras(int a, int b)
    {
        Assert.False(Dele.Perde(a, b));
    }

    [Fact]
    public void AsPosicoesIguaisSaoFriasNoNimEQuentesNoWythoff()
    {
        // É o lance da diagonal que faz isso: com as pilhas iguais ele esvazia
        // o tabuleiro de uma vez.
        for (var k = 1; k <= 60; k++)
        {
            Assert.True(Dele.Perde(k, k));
            Assert.False(Wythoff.Perde(k, k));
        }
    }

    [Fact]
    public void OsDoisJogosConcordamEmUmaPosicaoSo()
    {
        var concordam = new List<Posicao>();
        for (var a = 0; a <= 60; a++)
        {
            for (var b = 0; b <= 60; b++)
            {
                if (Dele.Perde(a, b) && Wythoff.Perde(a, b))
                {
                    concordam.Add(new Posicao(a, b));
                }
            }
        }

        Assert.Single(concordam);
        Assert.Equal(new Posicao(0, 0), concordam[0]);
    }

    [Fact]
    public void ONumeroDeDivergenciasEASomaDasFriasMenosDuas()
    {
        var (divergem, _) = Nim.Confronto(Wythoff, Dele);
        var friasNim = 0;
        var friasWythoff = 0;
        for (var a = 0; a <= 60; a++)
        {
            for (var b = 0; b <= 60; b++)
            {
                if (Dele.Perde(a, b))
                {
                    friasNim++;
                }

                if (Wythoff.Perde(a, b))
                {
                    friasWythoff++;
                }
            }
        }

        Assert.Equal(friasNim + friasWythoff - 2, divergem);
    }

    [Fact]
    public void ExisteLanceSoDeDiagonalEmPosicoesDeVerdade()
    {
        var (_, soDiagonal) = Nim.Confronto(Wythoff, Dele);
        Assert.True(soDiagonal > 0);
    }

    [Fact]
    public void LimiteNegativoNaoEAceito()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Nim(-1));
    }
}
