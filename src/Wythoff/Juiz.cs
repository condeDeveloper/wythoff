namespace Conde.Wythoff;

/// <summary>
/// O juiz do repositório: decide quem ganha cada posição pela DEFINIÇÃO, sem
/// saber nada sobre Wythoff, sobre o número de ouro ou sobre a construção das
/// posições frias.
///
/// A definição é só esta: uma posição é perdedora para quem joga quando todo
/// lance leva a uma posição vencedora; é vencedora quando existe pelo menos um
/// lance que leva a uma perdedora. A posição vazia é perdedora, porque quem
/// recebe o tabuleiro vazio não tem lance e já perdeu.
///
/// É o juiz que dá autoridade a tudo o mais que está neste repositório: a
/// fórmula do número de ouro e a construção por mex são conferidas contra ele,
/// posição por posição, e não contra um livro.
/// </summary>
public sealed class Juiz
{
    private readonly bool[,] _perde;
    private readonly int[,] _grundy;

    public int Limite { get; }

    public long PosicoesAbertas { get; private set; }

    public Juiz(int limite)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(limite);
        Limite = limite;
        _perde = new bool[limite + 1, limite + 1];
        _grundy = new int[limite + 1, limite + 1];
        Preencher();
    }

    private void Preencher()
    {
        // A ordem importa: todo lance de Wythoff só diminui coordenada, então
        // varrer de (0,0) para cima garante que o que eu preciso ler já está
        // calculado. Sem isso seria preciso recursão com memória, que é a mesma
        // conta com pilha de chamada em cima.
        var vistos = new bool[2 * Limite + 4];

        for (var a = 0; a <= Limite; a++)
        {
            for (var b = 0; b <= Limite; b++)
            {
                var posicao = new Posicao(a, b);
                var perde = true;
                var usados = 0;

                foreach (var destino in posicao.Lances())
                {
                    PosicoesAbertas++;
                    if (_perde[destino.A, destino.B])
                    {
                        perde = false;
                    }

                    var g = _grundy[destino.A, destino.B];
                    if (g < vistos.Length && !vistos[g])
                    {
                        vistos[g] = true;
                        usados++;
                    }
                }

                _perde[a, b] = perde;

                var mex = 0;
                while (mex < vistos.Length && vistos[mex])
                {
                    mex++;
                }

                _grundy[a, b] = mex;

                // Limpar só o que foi marcado: zerar o vetor inteiro a cada
                // posição transformaria a varredura em quadrática na toa.
                if (usados > 0)
                {
                    foreach (var destino in posicao.Lances())
                    {
                        var g = _grundy[destino.A, destino.B];
                        if (g < vistos.Length)
                        {
                            vistos[g] = false;
                        }
                    }
                }
            }
        }
    }

    /// <summary>Quem joga nesta posição perde, com os dois jogando bem.</summary>
    public bool Perde(int a, int b) => _perde[a, b];

    public bool Perde(Posicao posicao) => Perde(posicao.A, posicao.B);

    /// <summary>O valor de Grundy, que é o mex dos valores dos lances.</summary>
    public int Grundy(int a, int b) => _grundy[a, b];

    /// <summary>
    /// Todos os lances que levam a uma posição perdedora para o adversário.
    /// Numa posição perdedora a lista sai vazia, que é o que significa perder.
    /// </summary>
    public List<Posicao> LancesVencedores(int a, int b)
    {
        var bons = new List<Posicao>();
        foreach (var destino in new Posicao(a, b).Lances())
        {
            if (_perde[destino.A, destino.B])
            {
                bons.Add(destino);
            }
        }

        return bons;
    }

    /// <summary>As posições perdedoras normalizadas, em ordem.</summary>
    public List<Posicao> Frias()
    {
        var frias = new List<Posicao>();
        for (var a = 0; a <= Limite; a++)
        {
            for (var b = a; b <= Limite; b++)
            {
                if (_perde[a, b])
                {
                    frias.Add(new Posicao(a, b));
                }
            }
        }

        return frias;
    }
}
