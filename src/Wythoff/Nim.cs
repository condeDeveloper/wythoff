namespace Conde.Wythoff;

/// <summary>
/// O Nim de duas pilhas, que é o mesmo jogo de Wythoff SEM o lance da diagonal.
///
/// Ele está aqui como termo de comparação. O jogo mais famoso tem uma regra a
/// menos e uma resposta que cabe numa linha: perde quem recebe as duas pilhas
/// iguais. Medir os dois lado a lado mostra quanto um único lance a mais custa.
/// </summary>
public sealed class Nim
{
    private readonly bool[,] _perde;

    public int Limite { get; }

    public Nim(int limite)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(limite);
        Limite = limite;
        _perde = new bool[limite + 1, limite + 1];

        // Mesma definição do juiz de Wythoff, com o conjunto de lances menor.
        for (var a = 0; a <= limite; a++)
        {
            for (var b = 0; b <= limite; b++)
            {
                var perde = true;
                for (var k = 1; k <= a && perde; k++)
                {
                    if (_perde[a - k, b])
                    {
                        perde = false;
                    }
                }

                for (var k = 1; k <= b && perde; k++)
                {
                    if (_perde[a, b - k])
                    {
                        perde = false;
                    }
                }

                _perde[a, b] = perde;
            }
        }
    }

    public bool Perde(int a, int b) => _perde[a, b];

    /// <summary>A regra de Bouton para duas pilhas: o ou-exclusivo zera.</summary>
    public static bool PerdeporBouton(int a, int b) => (a ^ b) == 0;

    /// <summary>
    /// Quantas posições os dois jogos julgam diferente, e em quantas o lance da
    /// diagonal é o único lance vencedor.
    /// </summary>
    public static (int Divergem, int SoDiagonal) Confronto(Juiz wythoff, Nim nim)
    {
        var limite = Math.Min(wythoff.Limite, nim.Limite);
        var divergem = 0;
        var soDiagonal = 0;

        for (var a = 0; a <= limite; a++)
        {
            for (var b = 0; b <= limite; b++)
            {
                if (wythoff.Perde(a, b) != nim.Perde(a, b))
                {
                    divergem++;
                }

                var bons = wythoff.LancesVencedores(a, b);
                if (bons.Count > 0 && bons.TrueForAll(p => a - p.A == b - p.B && a - p.A > 0))
                {
                    soDiagonal++;
                }
            }
        }

        return (divergem, soDiagonal);
    }
}
