namespace Conde.Wythoff;

/// <summary>
/// As tabelas do README. Todo número que aparece lá sai de um método daqui, e
/// cada um deles está preso num teste: se o código mudar e o número mudar, o
/// teste quebra antes de o README mentir.
/// </summary>
public static class Medidas
{
    public readonly record struct LinhaFria(int N, int A, int B, bool JuizConcorda, bool RaizConcorda, bool DoubleConcorda);

    public readonly record struct LinhaConfronto(int Limite, int Posicoes, int FriasWythoff, int FriasNim, int Divergem, int SoDiagonal);

    public readonly record struct LinhaLances(int Lances, int Quantas);

    public readonly record struct LinhaDensidade(int Limite, int Frias, double PorFi);

    /// <summary>
    /// As primeiras posições frias pelos três caminhos, conferidas contra o juiz.
    /// </summary>
    public static List<LinhaFria> Frias(int quantas, Juiz juiz)
    {
        var porMex = Wythoff.Frias.PorMex(quantas);
        var linhas = new List<LinhaFria>(quantas);

        for (var n = 0; n < quantas; n++)
        {
            var p = porMex[n];
            var raiz = Wythoff.Frias.PorRaiz(n);
            var flutuante = Wythoff.Frias.PorDouble(n);
            var cabe = p.A <= juiz.Limite && p.B <= juiz.Limite;

            linhas.Add(new LinhaFria(
                n,
                p.A,
                p.B,
                cabe && juiz.Perde(p.A, p.B),
                raiz.A == p.A && raiz.B == p.B,
                flutuante.A == p.A && flutuante.B == p.B));
        }

        return linhas;
    }

    /// <summary>Wythoff contra o Nim de duas pilhas, tabuleiro por tabuleiro.</summary>
    public static List<LinhaConfronto> Confronto(IEnumerable<int> limites)
    {
        var linhas = new List<LinhaConfronto>();

        foreach (var limite in limites)
        {
            var juiz = new Juiz(limite);
            var nim = new Nim(limite);
            var (divergem, soDiagonal) = Nim.Confronto(juiz, nim);

            var friasNim = 0;
            for (var a = 0; a <= limite; a++)
            {
                for (var b = 0; b <= limite; b++)
                {
                    if (nim.Perde(a, b))
                    {
                        friasNim++;
                    }
                }
            }

            var friasWythoff = 0;
            for (var a = 0; a <= limite; a++)
            {
                for (var b = 0; b <= limite; b++)
                {
                    if (juiz.Perde(a, b))
                    {
                        friasWythoff++;
                    }
                }
            }

            linhas.Add(new LinhaConfronto(
                limite,
                (limite + 1) * (limite + 1),
                friasWythoff,
                friasNim,
                divergem,
                soDiagonal));
        }

        return linhas;
    }

    /// <summary>
    /// Quantos lances vencedores cada posição quente tem. No Nim de duas pilhas
    /// a resposta é sempre um; aqui não é, e a tabela diz quanto não é.
    /// </summary>
    public static List<LinhaLances> Lances(Juiz juiz)
    {
        var conta = new Dictionary<int, int>();

        for (var a = 0; a <= juiz.Limite; a++)
        {
            for (var b = a; b <= juiz.Limite; b++)
            {
                var quantos = juiz.LancesVencedores(a, b).Count;
                conta[quantos] = conta.GetValueOrDefault(quantos) + 1;
            }
        }

        var linhas = new List<LinhaLances>();
        foreach (var chave in conta.Keys.Order())
        {
            linhas.Add(new LinhaLances(chave, conta[chave]));
        }

        return linhas;
    }

    /// <summary>
    /// Quantas posições frias cabem até cada limite, contra o limite dividido
    /// pelo número de ouro, que é o que a fórmula de Beatty promete.
    /// </summary>
    public static List<LinhaDensidade> Densidade(IEnumerable<int> limites)
    {
        const double Fi = 1.6180339887498948482045868343656381177;
        var linhas = new List<LinhaDensidade>();

        foreach (var limite in limites)
        {
            var frias = 0;
            for (var n = 0; ; n++)
            {
                var (a, _) = Wythoff.Frias.PorRaiz(n);
                if (a > limite)
                {
                    break;
                }

                frias++;
            }

            linhas.Add(new LinhaDensidade(limite, frias, Math.Round(limite / Fi, 1)));
        }

        return linhas;
    }

    /// <summary>
    /// Onde a conta com ponto flutuante começa a errar. O limite é um
    /// argumento porque esta é a medida mais cara do repositório.
    /// </summary>
    public static long DoubleQuebra(long limite) => Wythoff.Frias.OndeODoubleQuebra(limite);

    /// <summary>Os primeiros desacordos entre a conta exata e a de double.</summary>
    public static List<Wythoff.Frias.Desacordo> Desacordos(long limite, int quantos)
        => Wythoff.Frias.Desacordos(limite, quantos);
}
