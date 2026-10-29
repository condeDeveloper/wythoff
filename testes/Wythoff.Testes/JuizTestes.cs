using Conde.Wythoff;
using Xunit;

namespace Conde.Wythoff.Testes;

public class JuizTestes
{
    private static readonly Juiz Pequeno = new(60);

    [Fact]
    public void OTabuleiroVazioEPerdedor()
    {
        Assert.True(Pequeno.Perde(0, 0));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(1, 1)]
    [InlineData(0, 7)]
    [InlineData(5, 5)]
    public void QuemPodeEsvaziarEmUmLanceGanha(int a, int b)
    {
        Assert.False(Pequeno.Perde(a, b));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(3, 5)]
    [InlineData(5, 3)]
    [InlineData(4, 7)]
    [InlineData(6, 10)]
    [InlineData(8, 13)]
    [InlineData(9, 15)]
    public void AsPrimeirasPosicoesFriasSaoPerdedoras(int a, int b)
    {
        Assert.True(Pequeno.Perde(a, b));
    }

    [Fact]
    public void ODefinicaoSeSustenta_DePerdedoraTodoLanceLevaAVencedora()
    {
        for (var a = 0; a <= Pequeno.Limite; a++)
        {
            for (var b = 0; b <= Pequeno.Limite; b++)
            {
                if (!Pequeno.Perde(a, b))
                {
                    continue;
                }

                foreach (var destino in new Posicao(a, b).Lances())
                {
                    Assert.False(Pequeno.Perde(destino.A, destino.B));
                }
            }
        }
    }

    [Fact]
    public void ODefinicaoSeSustenta_DeVencedoraExisteLanceParaPerdedora()
    {
        for (var a = 0; a <= Pequeno.Limite; a++)
        {
            for (var b = 0; b <= Pequeno.Limite; b++)
            {
                if (Pequeno.Perde(a, b))
                {
                    continue;
                }

                Assert.NotEmpty(Pequeno.LancesVencedores(a, b));
            }
        }
    }

    [Fact]
    public void PosicaoPerdedoraNaoTemLanceVencedor()
    {
        foreach (var fria in Pequeno.Frias())
        {
            Assert.Empty(Pequeno.LancesVencedores(fria.A, fria.B));
        }
    }

    [Fact]
    public void OJuizNaoSeImportaComAOrdemDasPilhas()
    {
        for (var a = 0; a <= Pequeno.Limite; a++)
        {
            for (var b = 0; b <= Pequeno.Limite; b++)
            {
                Assert.Equal(Pequeno.Perde(a, b), Pequeno.Perde(b, a));
                Assert.Equal(Pequeno.Grundy(a, b), Pequeno.Grundy(b, a));
            }
        }
    }

    [Fact]
    public void GrundyZeroEExatamenteAPosicaoPerdedora()
    {
        for (var a = 0; a <= Pequeno.Limite; a++)
        {
            for (var b = 0; b <= Pequeno.Limite; b++)
            {
                Assert.Equal(Pequeno.Perde(a, b), Pequeno.Grundy(a, b) == 0);
            }
        }
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 1)]
    [InlineData(0, 2, 2)]
    [InlineData(1, 1, 2)]
    [InlineData(1, 2, 0)]
    [InlineData(2, 2, 1)]
    public void OsPrimeirosValoresDeGrundy(int a, int b, int esperado)
    {
        Assert.Equal(esperado, Pequeno.Grundy(a, b));
    }

    [Fact]
    public void CadaLinhaDoGrundyNaoRepeteValor()
    {
        // O valor de Grundy é o mex dos lances, e tirar de uma pilha alcança
        // toda a linha: duas casas da mesma linha com o mesmo valor seriam uma
        // contradição com a própria definição de mex.
        for (var a = 0; a <= Pequeno.Limite; a++)
        {
            var vistos = new HashSet<int>();
            for (var b = 0; b <= Pequeno.Limite; b++)
            {
                Assert.True(vistos.Add(Pequeno.Grundy(a, b)));
            }
        }
    }

    [Fact]
    public void ExisteUmaPosicaoFriaParaCadaDiferenca()
    {
        var frias = Pequeno.Frias();
        var diferencas = frias.Select(p => p.B - p.A).ToList();
        Assert.Equal(diferencas.Count, diferencas.Distinct().Count());
        Assert.Equal(Enumerable.Range(0, diferencas.Count).ToList(), diferencas.Order().ToList());
    }

    [Fact]
    public void CadaInteiroAparecemUmaVezSoEntreAsPilhasDasFrias()
    {
        var frias = Pequeno.Frias();
        var numeros = new List<int>();
        foreach (var p in frias.Where(p => !p.Final))
        {
            // A posicao vazia entra com as duas pilhas em zero: ela e a unica
            // em que A e B sao o mesmo numero, e por isso fica de fora da conta.
            numeros.Add(p.A);
            numeros.Add(p.B);
        }

        Assert.Equal(numeros.Count, numeros.Distinct().Count());
    }

    [Fact]
    public void ACantoDaTabelaTemMaisPosicoesAbertasQueCasas()
    {
        Assert.True(Pequeno.PosicoesAbertas > (long)(Pequeno.Limite + 1) * (Pequeno.Limite + 1));
    }

    [Fact]
    public void LimiteNegativoNaoEAceito()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Juiz(-1));
    }
}
