using System.Numerics;
using Conde.Wythoff;
using Xunit;

namespace Conde.Wythoff.Testes;

public class FriasTestes
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 2)]
    [InlineData(2, 3, 5)]
    [InlineData(3, 4, 7)]
    [InlineData(4, 6, 10)]
    [InlineData(5, 8, 13)]
    [InlineData(6, 9, 15)]
    [InlineData(7, 11, 18)]
    [InlineData(8, 12, 20)]
    [InlineData(9, 14, 23)]
    public void AConstrucaoGulosaDaAsPosicoesConhecidas(int n, int a, int b)
    {
        var frias = Conde.Wythoff.Frias.PorMex(n + 1);
        Assert.Equal(new Posicao(a, b), frias[n]);
    }

    [Fact]
    public void AMaiorMenosAMenorEOIndice()
    {
        var frias = Conde.Wythoff.Frias.PorMex(200);
        for (var n = 0; n < frias.Count; n++)
        {
            Assert.Equal(n, frias[n].B - frias[n].A);
        }
    }

    [Fact]
    public void AsDuasSequenciasParticionamOsInteiros()
    {
        var frias = Conde.Wythoff.Frias.PorMex(300);
        var vistos = new HashSet<int>();
        vistos.Add(0);
        foreach (var p in frias.Where(p => !p.Final))
        {
            // Fora a posicao vazia, nenhum inteiro aparece duas vezes: as duas
            // sequencias partem os inteiros em dois times sem sobra.
            Assert.True(vistos.Add(p.A));
            Assert.True(vistos.Add(p.B));
        }

        // Todo inteiro pequeno precisa ter aparecido exatamente uma vez.
        for (var k = 0; k < 300; k++)
        {
            Assert.Contains(k, vistos);
        }
    }

    [Fact]
    public void AFormulaDaRaizConcordaComAConstrucaoGulosa()
    {
        var frias = Conde.Wythoff.Frias.PorMex(2000);
        for (var n = 0; n < frias.Count; n++)
        {
            var (a, b) = Conde.Wythoff.Frias.PorRaiz(n);
            Assert.Equal(frias[n].A, (int)a);
            Assert.Equal(frias[n].B, (int)b);
        }
    }

    [Fact]
    public void OJuizConcordaComAConstrucaoGulosa()
    {
        var juiz = new Juiz(120);
        var frias = Conde.Wythoff.Frias.PorMex(100).Where(p => p.B <= 120).ToList();
        Assert.Equal(juiz.Frias().Count, frias.Count);
        foreach (var p in frias)
        {
            Assert.True(juiz.Perde(p.A, p.B));
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(8, 2)]
    [InlineData(9, 3)]
    [InlineData(15, 3)]
    [InlineData(16, 4)]
    [InlineData(10000, 100)]
    public void ARaizInteiraArredondaParaBaixo(int valor, int esperado)
    {
        Assert.Equal(new BigInteger(esperado), Conde.Wythoff.Frias.RaizInteira(valor));
    }

    [Fact]
    public void ARaizInteiraNuncaPassaDoQuadrado()
    {
        for (var v = 0; v < 2000; v++)
        {
            var r = Conde.Wythoff.Frias.RaizInteira(v);
            Assert.True(r * r <= v);
            Assert.True((r + 1) * (r + 1) > v);
        }
    }

    [Fact]
    public void ARaizInteiraRecusaNegativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Conde.Wythoff.Frias.RaizInteira(-1));
    }

    [Fact]
    public void AVersaoEmLongConcordaComAVersaoExataDeBigInteger()
    {
        // A versão rápida existe só para a varredura longa aguentar. Se ela se
        // afastar da exata em algum ponto, a medida mais cara do repositório
        // vira ficção, então as duas são comparadas aqui.
        for (long n = 0; n < 5000; n++)
        {
            Assert.Equal(Conde.Wythoff.Frias.PorRaiz(n).A, Conde.Wythoff.Frias.PisoFiExato(n));
        }

        foreach (var n in new long[] { 102334155, 102334154, 141422324, 1000000007, 1234567890 })
        {
            Assert.Equal(Conde.Wythoff.Frias.PorRaiz(n).A, Conde.Wythoff.Frias.PisoFiExato(n));
        }
    }

    [Fact]
    public void ODoubleAcertaOsPrimeirosMilhoes()
    {
        Assert.Equal(0, Conde.Wythoff.Frias.OndeODoubleQuebra(2_000_000));
    }

    [Fact]
    public void OPrimeiroErroDoDoubleEstaNoQuadragesimoFibonacci()
    {
        var desacordos = Conde.Wythoff.Frias.Desacordos(200_000_000, 1);
        Assert.Single(desacordos);
        var d = desacordos[0];
        Assert.Equal(102334155, d.N);
        Assert.Equal(165580140, d.Exato);
        Assert.Equal(165580141, d.Flutuante);
        Assert.Equal("Fibonacci", d.Familia);
    }

    [Fact]
    public void OErroDoDoubleESempreDeUmaUnidadeParaCima()
    {
        foreach (var d in Conde.Wythoff.Frias.Desacordos(300_000_000, 8))
        {
            Assert.Equal(d.Exato + 1, d.Flutuante);
        }
    }

    [Fact]
    public void OsFibonaccisParesSaoOsQueQuaseViramInteiro()
    {
        // Para índice par o produto cai logo ABAIXO de um inteiro, e é por isso
        // que é lá que o arredondamento do double estoura.
        var fib = new List<long> { 0, 1 };
        while (fib[^1] < 1_000_000)
        {
            fib.Add(fib[^1] + fib[^2]);
        }

        for (var i = 2; i < fib.Count - 1; i++)
        {
            var piso = Conde.Wythoff.Frias.PisoFiExato(fib[i]);
            var esperado = i % 2 == 0 ? fib[i + 1] - 1 : fib[i + 1];
            Assert.Equal(esperado, piso);
        }
    }

    [Fact]
    public void OsNumerosDeFibonacciEDeLucasSeguemAMesmaRegra()
    {
        var fib = Conde.Wythoff.Frias.Fibonaccis(1000);
        var lucas = Conde.Wythoff.Frias.Lucas(1000);
        Assert.Contains(610L, fib);
        Assert.Contains(987L, fib);
        Assert.Contains(843L, lucas);
        Assert.Contains(521L, lucas);
        Assert.DoesNotContain(843L, fib);
    }

    [Fact]
    public void PedirQuantidadeNegativaNaoEAceito()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Conde.Wythoff.Frias.PorMex(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Conde.Wythoff.Frias.PorRaiz(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Conde.Wythoff.Frias.PorDouble(-1));
    }
}
