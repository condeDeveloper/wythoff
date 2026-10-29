namespace Conde.Wythoff;

/// <summary>
/// Uma posição do jogo de Wythoff: duas pilhas de fichas.
///
/// As duas pilhas são intercambiáveis, então toda posição tem uma forma
/// normalizada com a pilha menor primeiro. Guardar as duas ordens como posições
/// diferentes dobraria a tabela sem mudar uma resposta.
/// </summary>
public readonly record struct Posicao(int A, int B)
{
    public Posicao Normal => A <= B ? this : new Posicao(B, A);

    public bool Final => A == 0 && B == 0;

    /// <summary>
    /// Os três tipos de lance: tirar de uma pilha, tirar da outra, ou tirar a
    /// mesma quantidade das duas. O terceiro é a única diferença entre Wythoff
    /// e o Nim de duas pilhas, e é ele que destrói a conta do ou-exclusivo.
    /// </summary>
    public IEnumerable<Posicao> Lances()
    {
        for (var k = 1; k <= A; k++)
        {
            yield return new Posicao(A - k, B);
        }

        for (var k = 1; k <= B; k++)
        {
            yield return new Posicao(A, B - k);
        }

        var menor = Math.Min(A, B);
        for (var k = 1; k <= menor; k++)
        {
            yield return new Posicao(A - k, B - k);
        }
    }

    public override string ToString() => $"({A}, {B})";
}
