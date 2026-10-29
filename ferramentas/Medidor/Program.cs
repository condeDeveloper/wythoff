using Conde.Wythoff;

if (args.Length == 0)
{
    Console.WriteLine("uso: medidor <frias|confronto|lances|densidade|double>");
    return 1;
}

switch (args[0])
{
    case "frias":
        Frias();
        break;
    case "confronto":
        Confronto();
        break;
    case "lances":
        Lances();
        break;
    case "densidade":
        Densidade();
        break;
    case "double":
        Double(args.Length > 1 ? long.Parse(args[1]) : 100_000_000L);
        break;
    default:
        Console.WriteLine($"comando desconhecido: {args[0]}");
        return 1;
}

return 0;

static void Frias()
{
    var juiz = new Juiz(200);
    Console.WriteLine("As posicoes frias pelos tres caminhos");
    Console.WriteLine("n     mex        juiz  raiz  double");
    foreach (var l in Medidas.Frias(20, juiz))
    {
        Console.WriteLine($"{l.N,-5} ({l.A},{l.B})".PadRight(18)
            + $"{Sim(l.JuizConcorda),-6}{Sim(l.RaizConcorda),-6}{Sim(l.DoubleConcorda)}");
    }
}

static void Confronto()
{
    Console.WriteLine("Wythoff contra o Nim de duas pilhas");
    Console.WriteLine("limite  posicoes  frias W  frias Nim  divergem  so diagonal");
    foreach (var l in Medidas.Confronto([10, 20, 50, 100, 200]))
    {
        Console.WriteLine($"{l.Limite,-8}{l.Posicoes,-10}{l.FriasWythoff,-9}{l.FriasNim,-11}{l.Divergem,-10}{l.SoDiagonal}");
    }
}

static void Lances()
{
    var juiz = new Juiz(200);
    Console.WriteLine("Quantos lances vencedores tem cada posicao (ate 200 por 200)");
    Console.WriteLine("lances  posicoes");
    foreach (var l in Medidas.Lances(juiz))
    {
        Console.WriteLine($"{l.Lances,-8}{l.Quantas}");
    }
}

static void Densidade()
{
    Console.WriteLine("Quantas posicoes frias cabem ate cada limite");
    Console.WriteLine("limite  frias   limite/fi");
    foreach (var l in Medidas.Densidade([10, 100, 1000, 10000, 100000]))
    {
        Console.WriteLine($"{l.Limite,-8}{l.Frias,-8}{l.PorFi}");
    }
}

static void Double(long limite)
{
    Console.WriteLine($"Onde a conta com ponto flutuante erra (varrendo ate {limite})");
    var desacordos = Medidas.Desacordos(limite, 8);
    if (desacordos.Count == 0)
    {
        Console.WriteLine($"concordaram em todos os {limite} valores");
        return;
    }

    Console.WriteLine("n              exato          double         familia");
    foreach (var d in desacordos)
    {
        Console.WriteLine($"{d.N,-15}{d.Exato,-15}{d.Flutuante,-15}{d.Familia}");
    }
}

static string Sim(bool valor) => valor ? "sim" : "NAO";
