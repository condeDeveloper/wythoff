using System.Numerics;

namespace Conde.Wythoff;

/// <summary>
/// Três maneiras independentes de produzir as posições frias (as perdedoras
/// para quem joga), nenhuma delas olhando para o jogo.
///
/// Elas existem para discordar. Se as três concordam entre si e com o
/// <see cref="Juiz"/>, o número é bom; quando uma discorda, é ela que tem o
/// defeito, e achar onde foi o trabalho mais interessante deste repositório.
/// </summary>
public static class Frias
{
    /// <summary>
    /// A construção gulosa, com aritmética inteira e sem constante nenhuma:
    /// a n-ésima posição fria tem a pilha menor igual ao menor inteiro que
    /// ainda não apareceu, e a maior igual a ela mais n.
    ///
    /// Não há número de ouro aqui, nem raiz, nem divisão. Esta é a construção
    /// em que eu confio.
    /// </summary>
    public static List<Posicao> PorMex(int quantas)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantas);
        var usados = new HashSet<int>();
        var frias = new List<Posicao>(quantas);
        var candidato = 0;

        for (var n = 0; n < quantas; n++)
        {
            while (usados.Contains(candidato))
            {
                candidato++;
            }

            var a = candidato;
            var b = a + n;
            frias.Add(new Posicao(a, b));
            usados.Add(a);
            usados.Add(b);
        }

        return frias;
    }

    /// <summary>
    /// A fórmula de Beatty com o número de ouro, calculada em inteiros exatos:
    /// o piso de n vezes fi é (n + piso da raiz de 5n²) dividido por 2.
    ///
    /// A raiz inteira é calculada por Newton em <see cref="BigInteger"/>, então
    /// não existe ponto flutuante em lugar nenhum deste caminho. É a fórmula
    /// do livro, escrita de um jeito que não pode arredondar errado.
    /// </summary>
    public static (long A, long B) PorRaiz(long n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        var raiz = RaizInteira(BigInteger.Pow(n, 2) * 5);
        var a = (long)((n + raiz) / 2);
        return (a, a + n);
    }

    /// <summary>
    /// A mesma fórmula com ponto flutuante de dupla precisão, que é como
    /// praticamente todo mundo escreve. Ela está aqui para ser medida, não para
    /// ser usada.
    /// </summary>
    public static (long A, long B) PorDouble(long n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        const double Fi = 1.6180339887498948482045868343656381177;
        var a = (long)(n * Fi);
        return (a, a + n);
    }

    /// <summary>Raiz quadrada inteira por Newton, exata e sem flutuante.</summary>
    public static BigInteger RaizInteira(BigInteger valor)
    {
        if (valor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor));
        }

        if (valor < 2)
        {
            return valor;
        }

        var x = valor;
        var y = (x + 1) / 2;
        while (y < x)
        {
            x = y;
            y = (x + valor / x) / 2;
        }

        return x;
    }

    /// <summary>
    /// A mesma conta exata da <see cref="PorRaiz"/>, mas em <c>long</c>, para a
    /// varredura longa aguentar. Vale enquanto 5n² couber em 64 bits, o que dá
    /// n até cerca de 1,3 bilhão.
    ///
    /// A raiz começa no valor flutuante e é CORRIGIDA para cima e para baixo até
    /// ficar exata, então o resultado não depende da precisão do chute. Um teste
    /// confere esta versão contra a de <see cref="BigInteger"/>.
    /// </summary>
    public static long PisoFiExato(long n)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(n);
        var v = 5L * n * n;
        var r = (long)Math.Sqrt(v);
        while (r > 0 && r > v / r)
        {
            r--;
        }

        while ((r + 1) <= v / (r + 1))
        {
            r++;
        }

        return (n + r) / 2;
    }

    /// <summary>
    /// O primeiro n em que a conta com ponto flutuante discorda da exata, ou
    /// zero se as duas concordarem até o limite dado.
    /// </summary>
    public static long OndeODoubleQuebra(long limite)
    {
        var desacordos = Desacordos(limite, 1);
        return desacordos.Count == 0 ? 0 : desacordos[0].N;
    }

    public readonly record struct Desacordo(long N, long Exato, long Flutuante, string Familia);

    /// <summary>
    /// Os primeiros valores de n em que a conta com <c>double</c> discorda da
    /// exata, com o par de respostas e se aquele n é número de Fibonacci.
    ///
    /// A pergunta por trás da tabela é por que a conta erra onde erra, e não
    /// simplesmente a partir de onde. A resposta não estava no enunciado.
    /// </summary>
    public static List<Desacordo> Desacordos(long limite, int quantos)
    {
        const double Fi = 1.6180339887498948482045868343656381177;
        var fibonaccis = Fibonaccis(limite);
        var lucas = Lucas(limite);
        var achados = new List<Desacordo>();

        for (long n = 1; n <= limite && achados.Count < quantos; n++)
        {
            var flutuante = (long)(n * Fi);
            var exato = PisoFiExato(n);
            if (flutuante != exato)
            {
                var familia = fibonaccis.Contains(n) ? "Fibonacci"
                    : lucas.Contains(n) ? "Lucas"
                    : SomaDeDois(n, fibonaccis) ? "F+F"
                    : "outro";
                achados.Add(new Desacordo(n, exato, flutuante, familia));
            }
        }

        return achados;
    }

    /// <summary>Os números de Fibonacci até o limite.</summary>
    public static HashSet<long> Fibonaccis(long limite) => Sequencia(0, 1, limite);

    /// <summary>Os números de Lucas até o limite, que seguem a mesma regra de 2 e 1.</summary>
    public static HashSet<long> Lucas(long limite) => Sequencia(2, 1, limite);

    private static HashSet<long> Sequencia(long a, long b, long limite)
    {
        var conjunto = new HashSet<long>();
        while (a <= limite)
        {
            conjunto.Add(a);
            (a, b) = (b, a + b);
        }

        return conjunto;
    }

    private static bool SomaDeDois(long n, HashSet<long> conjunto)
    {
        foreach (var x in conjunto)
        {
            if (x > 0 && x <= n && conjunto.Contains(n - x))
            {
                return true;
            }
        }

        return false;
    }
}
