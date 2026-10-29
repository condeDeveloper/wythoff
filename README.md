# Jogo de Wythoff

Duas pilhas de fichas. Na sua vez você tira quantas quiser de uma pilha, ou quantas
quiser da outra, ou **a mesma quantidade das duas**. Quem leva a última ficha ganha.

É o Nim de duas pilhas com um lance a mais, e esse lance a mais destrói a resposta do
Nim por completo. A resposta certa foi publicada por Willem Wythoff em 1907 e envolve o
número de ouro, o que é estranho num jogo que só tem fichas e contagem.

Aqui o jogo é **varrido**: todas as posições de um tabuleiro são julgadas pela
definição, sem fórmula nenhuma, e só depois a fórmula é chamada para ser conferida
contra o resultado.

```
dotnet test testes/Wythoff.Testes/Wythoff.Testes.csproj -c Release
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- frias
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- confronto
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- lances
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- densidade
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- double 1300000000
```

## O juiz

O `Juiz` não sabe nada sobre Wythoff. Ele aplica a definição e só ela:

> uma posição é **perdedora** para quem joga quando todo lance leva a uma vencedora, e
> **vencedora** quando existe pelo menos um lance que leva a uma perdedora.

Com o tabuleiro vazio perdedor na base, isso preenche a tabela inteira de baixo para
cima. É esse juiz que dá autoridade a todo o resto: a fórmula do número de ouro e a
construção gulosa são conferidas contra ele, posição por posição, e não contra um livro.

## As três maneiras de achar as posições frias

As posições perdedoras são chamadas de frias. Há três caminhos independentes até elas, e
eles existem para discordar:

| caminho | como |
|---|---|
| `PorMex` | guloso, em inteiros: a menor pilha é o menor inteiro que ainda não apareceu, e a maior é ela mais o índice. Sem constante nenhuma |
| `PorRaiz` | a fórmula de Beatty com o número de ouro, em aritmética exata: o piso de `n·φ` é `(n + ⌊√(5n²)⌋) / 2`, com a raiz calculada por Newton em `BigInteger` |
| `PorDouble` | a mesma fórmula com `double`, que é como quase todo mundo escreve |

As vinte primeiras:

```
n     mex        juiz  raiz  double
0     (0,0)       sim   sim   sim
1     (1,2)       sim   sim   sim
2     (3,5)       sim   sim   sim
3     (4,7)       sim   sim   sim
4     (6,10)      sim   sim   sim
5     (8,13)      sim   sim   sim
6     (9,15)      sim   sim   sim
7     (11,18)     sim   sim   sim
8     (12,20)     sim   sim   sim
9     (14,23)     sim   sim   sim
10    (16,26)     sim   sim   sim
...
19    (30,49)     sim   sim   sim
```

## Os achados

### 1. A conta com `double` acerta 102.334.154 valores seguidos e erra no seguinte, que é o quadragésimo número de Fibonacci

Esta é a medida mais cara do repositório e a que mais me surpreendeu. A fórmula de
Beatty com `double` não erra devagar, nem erra a partir de um ponto: ela acerta **cem
milhões de valores seguidos** e então erra, pela primeira vez, exatamente em
`n = 102.334.155`.

```
n              exato          double         familia
102334155      165580140      165580141      Fibonacci
141422324      228826126      228826127      Lucas
180510493      292072112      292072113      F+F
204668310      331160281      331160282      F+F
219598662      355318098      355318099      outro
243756479      394406267      394406268      outro
267914296      433494436      433494437      Fibonacci
282844648      457652253      457652254      F+F
```

O primeiro erro é `F(40)`. O segundo é um número de Lucas. Três dos oito são soma de
dois Fibonaccis. **Nenhum dos oito é um número qualquer**, e o motivo é que o erro não é
aleatório: para índice par, `F(n)·φ` cai logo **abaixo** de `F(n+1)`, e a distância até o
inteiro encolhe mais rápido que a precisão do `double`. Quando a distância fica menor que
o último bit, o arredondamento cruza o inteiro e o piso sai um a mais. O erro é sempre de
exatamente uma unidade, sempre para cima, e sempre nos números que mais se aproximam de
um múltiplo inteiro do número de ouro.

Quem escreve `(long)(n * 1.618...)` não está errado por pouco tempo: está certo por muito
tempo e errado no ponto exato em que o problema é sobre o número de ouro.

### 2. O Nim e o Wythoff concordam sobre exatamente uma posição: o tabuleiro vazio

```
limite  posicoes  frias W  frias Nim  divergem  so diagonal
10      121       9        11         18        16
20      441       17       21         36        60
50      2601      39       51         88        379
100     10201     77       101        176       1492
200     40401     153      201        352       5920
```

Em todo tabuleiro, o número de divergências é exatamente `frias do Nim + frias do Wythoff
− 2`, o que só pode acontecer se a interseção tiver **um** elemento. Esse elemento é
`(0,0)`. Fora dele, não existe uma única posição em que os dois jogos deem a mesma
resposta sobre quem ganha.

Os dois jogos têm as mesmas peças, as mesmas fichas e quase as mesmas regras. Um lance a
mais não mudou um pedaço da resposta: mudou toda ela.

### 3. Nenhuma posição tem mais de três lances vencedores

```
lances  posicoes
0       77
1       15468
2       3600
3       1156
```

No Nim de duas pilhas o lance vencedor é sempre único, e eu esperava que o lance a mais
trouxesse muita escolha. Trouxe pouca: das 20.224 posições vencedoras de um tabuleiro de
200 por 200, **76,5% têm um único lance que ganha** e nenhuma passa de três. O jogo é mais
apertado do que a quantidade de lances disponíveis faz parecer — numa posição como
`(200, 200)` existem 600 lances possíveis e no máximo três deles servem.

### 4. Em quase 15% das posições vencedoras, o único lance que ganha é o da diagonal

Das 40.248 posições vencedoras do tabuleiro de 200 por 200, em **5.920** todos os lances
vencedores são diagonais. São posições em que tirar de uma pilha só, que é tudo o que o
Nim sabe fazer, **perde o jogo em qualquer quantidade**. O lance novo não é um atalho:
em uma de cada sete posições ele é a única saída.

## A densidade

As posições frias são raras e o espaçamento é o número de ouro:

```
limite  frias   limite/fi
10      7       6.2
100     63      61.8
1000    619     618
10000   6181    6180.3
100000  61805   61803.4
```

A diferença nunca passa de duas posições em nenhum limite medido.

## O que as medidas me corrigiram

- **Eu chutei um valor de Grundy dentro do arquivo que existe para impedir chute.**
  Escrevi `G(2,2) = 4` num teste porque me pareceu razoável. O valor medido é **1**. A
  conta é curta e eu podia ter feito à mão: os lances de `(2,2)` levam a valores `{0, 2}`,
  e o menor ausente é 1. Foi o teste que me pegou, que é exatamente para isso que ele
  existe, mas a lição é outra: o arquivo que prende os números é o último lugar onde se
  pode digitar um palpite.

- **Eu esperava que o `double` quebrasse cedo e sem graça.** Imaginei um erro acumulando
  devagar a partir de algum ponto arbitrário. O que existe é o contrário: cem milhões de
  acertos seguidos e um erro cirúrgico num número de Fibonacci. Eu tinha escrito "a conta
  com ponto flutuante vai errando conforme n cresce", que é uma frase errada com cara de
  certa, e a varredura até 100 milhões não teria me mostrado nada — o primeiro erro está
  2,3 milhões depois desse limite. **Quase medi o suficiente para confirmar a coisa errada.**

- **A posição vazia tem as duas pilhas iguais, e eu me esqueci dela duas vezes.** Dois
  testes meus afirmavam que as pilhas das posições frias nunca repetem um inteiro, e os
  dois quebraram em `(0,0)`, onde `A` e `B` são o mesmo zero. A afirmação estava certa
  para todas as posições menos uma, e "todas menos uma" não é "todas".

- **Eu ia descrever a diagonal como um atalho.** A palavra estava no rascunho: um lance
  extra que às vezes acelera. A medida mostra que em 14,7% das posições vencedoras ele é
  o **único** lance que ganha. Atalho é o que encurta um caminho que existe; aqui, sem a
  diagonal, não há caminho.

- **Zerar o vetor do mex a cada posição deixava a varredura quadrática à toa.** A primeira
  versão limpava o vetor inteiro de marcas a cada casa da tabela. Passar a desmarcar só o
  que foi marcado não mudou um número, e é a diferença entre a tabela de 200 por 200 sair
  em segundos ou não sair.

## Estrutura

```
src/Wythoff/
  Posicao.cs    o par de pilhas e os tres tipos de lance
  Juiz.cs       a definicao de ganhar e perder, e os valores de Grundy
  Frias.cs      as tres construcoes das posicoes frias e a raiz inteira
  Nim.cs        o mesmo jogo sem a diagonal, para comparar
  Medidas.cs    as tabelas deste README
testes/Wythoff.Testes/
ferramentas/Medidor/
```

116 testes.
