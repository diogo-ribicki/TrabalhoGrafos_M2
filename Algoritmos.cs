namespace TrabalhoGrafos;

public static class Algoritmos
{
    public static List<int> BuscaLargura(Grafos grafo, int origem)
    {
        if (origem < 0 || origem >= grafo.QuantidadeVertices)
            throw new ArgumentOutOfRangeException(nameof(origem));

        var visitado = new bool[grafo.QuantidadeVertices];
        var fila = new Queue<int>();
        var ordem = new List<int>();

        visitado[origem] = true;
        fila.Enqueue(origem);

        while (fila.Count > 0)
        {
            int atual = fila.Dequeue();
            ordem.Add(atual);

            foreach (int vizinho in grafo.RetornarVizinhos(atual))
            {
                if (!visitado[vizinho])
                {
                    visitado[vizinho] = true;
                    fila.Enqueue(vizinho);
                }
            }
        }

        return ordem;
    }

    public static List<int> BuscaProfundidade(Grafos grafo, int origem)
    {
        if (origem < 0 || origem >= grafo.QuantidadeVertices)
            throw new ArgumentOutOfRangeException(nameof(origem));

        var visitado = new bool[grafo.QuantidadeVertices];
        var ordem = new List<int>();

        DFS(grafo, origem, visitado, ordem);

        return ordem;
    }

    private static void DFS(
        Grafos grafo,
        int atual,
        bool[] visitado,
        List<int> ordem)
    {
        visitado[atual] = true;
        ordem.Add(atual);

        foreach (int vizinho in grafo.RetornarVizinhos(atual))
        {
            if (!visitado[vizinho])
                DFS(grafo, vizinho, visitado, ordem);
        }
    }

    public static ResultadoDijkstra Dijkstra(Grafos grafo, int origem)
    {
        if (origem < 0 || origem >= grafo.QuantidadeVertices)
            throw new ArgumentOutOfRangeException(nameof(origem));

        if (!grafo.Ponderado)
            throw new InvalidOperationException(
                "O algoritmo de Dijkstra exige um grafo ponderado.");

        int n = grafo.QuantidadeVertices;

        var distancia = Enumerable.Repeat(float.PositiveInfinity, n).ToArray();
        var anterior = Enumerable.Repeat(-1, n).ToArray();
        var visitado = new bool[n];

        distancia[origem] = 0;

        // Versão simples de Dijkstra usando apenas as operações
        // da classe Grafos. Funciona tanto para matriz quanto para lista.
        for (int i = 0; i < n; i++)
        {
            int atual = -1;
            float menorDistancia = float.PositiveInfinity;

            for (int j = 0; j < n; j++)
            {
                if (!visitado[j] && distancia[j] < menorDistancia)
                {
                    menorDistancia = distancia[j];
                    atual = j;
                }
            }

            if (atual == -1)
                break;

            visitado[atual] = true;

            foreach (int vizinho in grafo.RetornarVizinhos(atual))
            {
                if (visitado[vizinho])
                    continue;

                float peso = grafo.PesoAresta(atual, vizinho);

                if (peso < 0)
                    throw new InvalidOperationException(
                        "Dijkstra não aceita arestas com peso negativo.");

                float novaDistancia = distancia[atual] + peso;

                if (novaDistancia < distancia[vizinho])
                {
                    distancia[vizinho] = novaDistancia;
                    anterior[vizinho] = atual;
                }
            }
        }

        return new ResultadoDijkstra(distancia, anterior, origem);
    }
}

public class ResultadoDijkstra
{
    public float[] Distancias { get; }
    public int[] Anteriores { get; }
    public int Origem { get; }

    public ResultadoDijkstra(float[] distancias, int[] anteriores, int origem)
    {
        Distancias = distancias;
        Anteriores = anteriores;
        Origem = origem;
    }

    public List<int> CaminhoAte(int destino)
    {
        if (destino < 0 || destino >= Distancias.Length)
            throw new ArgumentOutOfRangeException(nameof(destino));

        if (float.IsPositiveInfinity(Distancias[destino]))
            return new List<int>();

        var caminho = new List<int>();

        for (int atual = destino; atual != -1; atual = Anteriores[atual])
            caminho.Add(atual);

        caminho.Reverse();
        return caminho;
    }

    public void Imprimir(Grafos grafo)
    {
        Console.WriteLine("=== Dijkstra ===");
        Console.WriteLine($"Origem: {grafo.LabelVertice(Origem)}");
        Console.WriteLine();

        for (int i = 0; i < Distancias.Length; i++)
        {
            Console.Write($"{grafo.LabelVertice(Origem)} -> {grafo.LabelVertice(i)}: ");

            if (float.IsPositiveInfinity(Distancias[i]))
            {
                Console.WriteLine("inalcançável");
                continue;
            }

            Console.WriteLine($"distância = {Distancias[i]:0.##}");

            var caminho = CaminhoAte(i);

            Console.WriteLine(
                "Caminho: " +
                string.Join(" -> ", caminho.Select(v => grafo.LabelVertice(v))));
        }
    }
}