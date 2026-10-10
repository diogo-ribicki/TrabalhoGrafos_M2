
using System.Diagnostics;

namespace TrabalhoGrafos;

/// <summary>Resultado de um algoritmo de coloração.</summary>
public sealed class ResultadoColoracao
{
    public string NomeAlgoritmo { get; }
    public int[] Cores { get; }
    public int QuantidadeCores { get; }
    public TimeSpan TempoExecucao { get; }

    public ResultadoColoracao(string nomeAlgoritmo, int[] cores, TimeSpan tempoExecucao)
    {
        NomeAlgoritmo = nomeAlgoritmo;
        Cores = cores;
        QuantidadeCores = cores.Length == 0 ? 0 : cores.Max() + 1;
        TempoExecucao = tempoExecucao;
    }
}

/// <summary>Aresta selecionada na árvore/floresta geradora.</summary>
public sealed record ArestaResultado(int Origem, int Destino, float Peso);

/// <summary>Resultado de Prim ou Kruskal.</summary>
public sealed class ResultadoArvoreGeradora
{
    public string NomeAlgoritmo { get; }
    public List<ArestaResultado> Arestas { get; }
    public float PesoTotal { get; }
    public int QuantidadeComponentes { get; }
    public int QuantidadeVertices { get; }
    public TimeSpan TempoExecucao { get; }

    public bool TemArvoreGeradora => QuantidadeVertices > 0 && QuantidadeComponentes == 1;

    public ResultadoArvoreGeradora(
        string nomeAlgoritmo,
        List<ArestaResultado> arestas,
        float pesoTotal,
        int quantidadeComponentes,
        int quantidadeVertices,
        TimeSpan tempoExecucao)
    {
        NomeAlgoritmo = nomeAlgoritmo;
        Arestas = arestas;
        PesoTotal = pesoTotal;
        QuantidadeComponentes = quantidadeComponentes;
        QuantidadeVertices = quantidadeVertices;
        TempoExecucao = tempoExecucao;
    }
}

/// <summary>
/// Algoritmos da parte 1 da M2: coloração de grafos e árvore geradora mínima.
/// Usa a interface Grafos do trabalho M1 e, portanto, funciona com suas representações.
/// Para os arquivos grandes, a aplicação carrega o grafo por lista de adjacência.
/// </summary>
public static class AlgoritmosM2
{
    // Coloração ---------------------------------------------------------------

    /// <summary>
    /// Heurística gulosa sem ordenação especial: visita os vértices na ordem do arquivo
    /// (0, 1, 2, ...) e escolhe a menor cor disponível.
    /// </summary>
    public static ResultadoColoracao ColoracaoGulosa(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        List<int>[] adjacencia = PrepararAdjacenciaColoracao(grafo);
        int n = adjacencia.Length;
        int[] cores = Enumerable.Repeat(-1, n).ToArray();

        for (int vertice = 0; vertice < n; vertice++)
            cores[vertice] = MenorCorDisponivel(vertice, cores, adjacencia);

        cronometro.Stop();
        return new ResultadoColoracao("Gulosa (ordem dos vértices)", cores, cronometro.Elapsed);
    }

    /// <summary>Heurística Welsh-Powell: ordena por grau decrescente e cria classes de cor.</summary>
    public static ResultadoColoracao WelshPowell(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        List<int>[] adjacencia = PrepararAdjacenciaColoracao(grafo);
        int n = adjacencia.Length;
        int[] cores = Enumerable.Repeat(-1, n).ToArray();
        int[] ordem = Enumerable.Range(0, n)
            .OrderByDescending(v => adjacencia[v].Count)
            .ThenBy(v => v)
            .ToArray();

        int corAtual = 0;
        foreach (int vertice in ordem)
        {
            if (cores[vertice] != -1)
                continue;

            cores[vertice] = corAtual;

            // Tenta colocar na mesma cor todos os vértices restantes não adjacentes
            // aos vértices já coloridos com corAtual.
            foreach (int candidato in ordem)
            {
                if (cores[candidato] != -1)
                    continue;

                bool conflito = false;
                foreach (int vizinho in adjacencia[candidato])
                {
                    if (cores[vizinho] == corAtual)
                    {
                        conflito = true;
                        break;
                    }
                }

                if (!conflito)
                    cores[candidato] = corAtual;
            }

            corAtual++;
        }

        cronometro.Stop();
        return new ResultadoColoracao("Welsh-Powell", cores, cronometro.Elapsed);
    }

    /// <summary>
    /// Heurística DSATUR. A cada passo escolhe o vértice não colorido com maior saturação;
    /// em empate, escolhe o de maior grau. Uma PriorityQueue evita reexaminar todos os vértices.
    /// </summary>
    public static ResultadoColoracao DSATUR(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        List<int>[] adjacencia = PrepararAdjacenciaColoracao(grafo);
        int n = adjacencia.Length;
        int[] cores = Enumerable.Repeat(-1, n).ToArray();
        var coresVizinhas = new HashSet<int>[n];
        var versoes = new int[n];

        for (int v = 0; v < n; v++)
            coresVizinhas[v] = new HashSet<int>();

        // Prioridade menor = melhor: -saturação, -grau, índice, versão.
        var fila = new PriorityQueue<(int Vertice, int Versao), (int NegSaturacao, int NegGrau, int Vertice, int Versao)>();
        for (int v = 0; v < n; v++)
            fila.Enqueue((v, versoes[v]), (0, -adjacencia[v].Count, v, versoes[v]));

        for (int quantidadeColorida = 0; quantidadeColorida < n; quantidadeColorida++)
        {
            int atual = -1;

            while (fila.TryDequeue(out var item, out _))
            {
                if (cores[item.Vertice] == -1 && item.Versao == versoes[item.Vertice])
                {
                    atual = item.Vertice;
                    break;
                }
            }

            if (atual == -1)
                throw new InvalidOperationException("Não foi possível selecionar o próximo vértice no DSATUR.");

            int cor = MenorCorDisponivel(atual, cores, adjacencia);
            cores[atual] = cor;

            foreach (int vizinho in adjacencia[atual])
            {
                if (cores[vizinho] != -1)
                    continue;

                // Só há alteração de saturação quando surge uma nova cor na vizinhança.
                if (coresVizinhas[vizinho].Add(cor))
                {
                    versoes[vizinho]++;
                    fila.Enqueue(
                        (vizinho, versoes[vizinho]),
                        (-coresVizinhas[vizinho].Count, -adjacencia[vizinho].Count, vizinho, versoes[vizinho]));
                }
            }
        }

        cronometro.Stop();
        return new ResultadoColoracao("DSATUR", cores, cronometro.Elapsed);
    }

    /// <summary>
    /// Força bruta com retrocesso: tenta 2 cores, depois 3, até n cores.
    /// A ordem interna é por grau decrescente para reduzir o número de tentativas.
    /// Como as cores são simétricas, só introduz a próxima cor ainda não usada; isso não
    /// elimina nenhuma partição de coloração possível e melhora o desempenho.
    /// </summary>
    public static ResultadoColoracao ForcaBruta(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        List<int>[] adjacencia = PrepararAdjacenciaColoracao(grafo);
        int n = adjacencia.Length;

        if (n == 0)
        {
            cronometro.Stop();
            return new ResultadoColoracao("Força bruta", Array.Empty<int>(), cronometro.Elapsed);
        }

        // Se não há arestas, todos os vértices podem receber a mesma cor.
        // Essa exceção também trata corretamente o número cromático igual a 1.
        if (adjacencia.All(vizinhos => vizinhos.Count == 0))
        {
            cronometro.Stop();
            return new ResultadoColoracao("Força bruta", new int[n], cronometro.Elapsed);
        }

        int[] ordem = Enumerable.Range(0, n)
            .OrderByDescending(v => adjacencia[v].Count)
            .ThenBy(v => v)
            .ToArray();

        for (int limiteCores = 2; limiteCores <= n; limiteCores++)
        {
            int[] cores = Enumerable.Repeat(-1, n).ToArray();
            if (TentarColorir(0, -1, limiteCores, ordem, cores, adjacencia))
            {
                cronometro.Stop();
                return new ResultadoColoracao("Força bruta", cores, cronometro.Elapsed);
            }
        }

        cronometro.Stop();
        throw new InvalidOperationException("Não foi encontrada uma coloração válida. Verifique se o grafo contém laços.");
    }

    private static bool TentarColorir(
        int posicao,
        int maiorCorUsada,
        int limiteCores,
        int[] ordem,
        int[] cores,
        List<int>[] adjacencia)
    {
        if (posicao == ordem.Length)
            return true;

        int vertice = ordem[posicao];
        var proibidas = new bool[limiteCores];

        foreach (int vizinho in adjacencia[vertice])
        {
            int corVizinho = cores[vizinho];
            if (corVizinho >= 0)
                proibidas[corVizinho] = true;
        }

        // Evita considerar permutações equivalentes de nomes de cores.
        int ultimaCor = Math.Min(limiteCores - 1, maiorCorUsada + 1);
        for (int cor = 0; cor <= ultimaCor; cor++)
        {
            if (proibidas[cor])
                continue;

            cores[vertice] = cor;
            if (TentarColorir(posicao + 1, Math.Max(maiorCorUsada, cor), limiteCores, ordem, cores, adjacencia))
                return true;
        }

        cores[vertice] = -1;
        return false;
    }

    private static int MenorCorDisponivel(int vertice, int[] cores, List<int>[] adjacencia)
    {
        var proibidas = new HashSet<int>();
        foreach (int vizinho in adjacencia[vertice])
        {
            if (cores[vizinho] >= 0)
                proibidas.Add(cores[vizinho]);
        }

        int cor = 0;
        while (proibidas.Contains(cor))
            cor++;

        return cor;
    }

    private static List<int>[] PrepararAdjacenciaColoracao(Grafos grafo)
    {
        if (grafo.Direcionado)
            throw new InvalidOperationException("Os algoritmos de coloração desta atividade exigem um grafo não direcionado.");

        int n = grafo.QuantidadeVertices;
        var adjacencia = new List<int>[n];

        for (int v = 0; v < n; v++)
        {
            adjacencia[v] = grafo.RetornarVizinhos(v).Distinct().ToList();
            if (adjacencia[v].Contains(v))
                throw new InvalidOperationException($"O vértice {grafo.LabelVertice(v)} tem um laço e não pode ser colorido corretamente.");
        }

        return adjacencia;
    }

    // Árvores geradoras mínimas ------------------------------------------------

    /// <summary>
    /// Algoritmo de Prim usando fila de prioridades. Para grafo desconexo, reinicia
    /// em cada componente e devolve uma floresta geradora mínima.
    /// </summary>
    public static ResultadoArvoreGeradora Prim(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        ValidarGrafoParaAGM(grafo);
        int n = grafo.QuantidadeVertices;
        var visitado = new bool[n];
        var resultado = new List<ArestaResultado>();
        float soma = 0;
        int componentes = 0;

        for (int inicio = 0; inicio < n; inicio++)
        {
            if (visitado[inicio])
                continue;

            componentes++;
            visitado[inicio] = true;
            var fila = new PriorityQueue<ArestaResultado, (float Peso, int Origem, int Destino)>();
            AdicionarArestasDePrim(inicio, grafo, visitado, fila);

            while (fila.TryDequeue(out ArestaResultado aresta, out _))
            {
                if (visitado[aresta.Destino])
                    continue;

                visitado[aresta.Destino] = true;
                resultado.Add(aresta);
                soma += aresta.Peso;
                AdicionarArestasDePrim(aresta.Destino, grafo, visitado, fila);
            }
        }

        cronometro.Stop();
        return new ResultadoArvoreGeradora("Prim", resultado, soma, componentes, n, cronometro.Elapsed);
    }

    private static void AdicionarArestasDePrim(
        int origem,
        Grafos grafo,
        bool[] visitado,
        PriorityQueue<ArestaResultado, (float Peso, int Origem, int Destino)> fila)
    {
        foreach (int destino in grafo.RetornarVizinhos(origem))
        {
            if (destino == origem || visitado[destino])
                continue;

            float peso = grafo.PesoAresta(origem, destino);
            var aresta = new ArestaResultado(origem, destino, peso);
            fila.Enqueue(aresta, (peso, origem, destino));
        }
    }

    /// <summary>Kruskal com ordenação das arestas e estrutura Union-Find.</summary>
    public static ResultadoArvoreGeradora Kruskal(Grafos grafo)
    {
        var cronometro = Stopwatch.StartNew();
        ValidarGrafoParaAGM(grafo);
        int n = grafo.QuantidadeVertices;
        var arestas = new List<ArestaResultado>();

        for (int origem = 0; origem < n; origem++)
        {
            foreach (int destino in grafo.RetornarVizinhos(origem))
            {
                // Como o grafo é não direcionado, guarda cada aresta uma única vez.
                if (destino > origem)
                    arestas.Add(new ArestaResultado(origem, destino, grafo.PesoAresta(origem, destino)));
            }
        }

        arestas.Sort((a, b) =>
        {
            int comparacaoPeso = a.Peso.CompareTo(b.Peso);
            if (comparacaoPeso != 0)
                return comparacaoPeso;

            int comparacaoOrigem = a.Origem.CompareTo(b.Origem);
            return comparacaoOrigem != 0 ? comparacaoOrigem : a.Destino.CompareTo(b.Destino);
        });

        var conjuntos = new UnionFind(n);
        var resultado = new List<ArestaResultado>();
        float soma = 0;

        foreach (ArestaResultado aresta in arestas)
        {
            if (conjuntos.Unir(aresta.Origem, aresta.Destino))
            {
                resultado.Add(aresta);
                soma += aresta.Peso;
                if (resultado.Count == n - 1)
                    break;
            }
        }

        cronometro.Stop();
        return new ResultadoArvoreGeradora(
            "Kruskal", resultado, soma, conjuntos.QuantidadeComponentes, n, cronometro.Elapsed);
    }

    private static void ValidarGrafoParaAGM(Grafos grafo)
    {
        if (grafo.Direcionado)
            throw new InvalidOperationException("Prim e Kruskal exigem um grafo não direcionado.");
    }

    private sealed class UnionFind
    {
        private readonly int[] pai;
        private readonly int[] tamanho;

        public int QuantidadeComponentes { get; private set; }

        public UnionFind(int quantidade)
        {
            pai = new int[quantidade];
            tamanho = new int[quantidade];
            QuantidadeComponentes = quantidade;

            for (int i = 0; i < quantidade; i++)
            {
                pai[i] = i;
                tamanho[i] = 1;
            }
        }

        private int Encontrar(int vertice)
        {
            while (vertice != pai[vertice])
            {
                pai[vertice] = pai[pai[vertice]];
                vertice = pai[vertice];
            }

            return vertice;
        }

        public bool Unir(int a, int b)
        {
            int raizA = Encontrar(a);
            int raizB = Encontrar(b);

            if (raizA == raizB)
                return false;

            if (tamanho[raizA] < tamanho[raizB])
                (raizA, raizB) = (raizB, raizA);

            pai[raizB] = raizA;
            tamanho[raizA] += tamanho[raizB];
            QuantidadeComponentes--;
            return true;
        }
    }
}
