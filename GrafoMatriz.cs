namespace TrabalhoGrafos;

public class GrafoMatriz : Grafos
{
    private List<List<float?>> matriz = new();

    public GrafoMatriz(bool direcionado, bool ponderado)
        : base(direcionado, ponderado)
    {
    }

    public override bool InserirVertice(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return false;

        if (vertices.Contains(label))
            return false;

        vertices.Add(label);

        foreach (var linha in matriz)
            linha.Add(null);

        var novaLinha = new List<float?>();
        for (int i = 0; i < vertices.Count; i++)
            novaLinha.Add(null);

        matriz.Add(novaLinha);
        return true;
    }

    public override bool RemoverVertice(int indice)
    {
        if (indice < 0 || indice >= vertices.Count)
            return false;

        vertices.RemoveAt(indice);
        matriz.RemoveAt(indice);

        foreach (var linha in matriz)
            linha.RemoveAt(indice);

        return true;
    }

    public override bool InserirAresta(int origem, int destino, float peso = 1)
    {
        if (!IndicesValidos(origem, destino))
            return false;

        if (ExisteAresta(origem, destino))
            return false;

        float pesoFinal = PesoNormalizado(peso);
        matriz[origem][destino] = pesoFinal;

        // Em grafo não direcionado, a aresta existe nos dois sentidos.
        if (!Direcionado && origem != destino)
            matriz[destino][origem] = pesoFinal;

        return true;
    }

    public override bool RemoverAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino) || !ExisteAresta(origem, destino))
            return false;

        matriz[origem][destino] = null;

        if (!Direcionado && origem != destino)
            matriz[destino][origem] = null;

        return true;
    }

    public override bool ExisteAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino))
            return false;

        return matriz[origem][destino].HasValue;
    }

    public override float PesoAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino))
            throw new ArgumentOutOfRangeException();

        if (!matriz[origem][destino].HasValue)
            throw new InvalidOperationException("Aresta inexistente.");

        return matriz[origem][destino]!.Value;
    }

    public override List<int> RetornarVizinhos(int vertice)
    {
        ValidarIndice(vertice);

        var vizinhos = new List<int>();

        for (int i = 0; i < vertices.Count; i++)
        {
            if (matriz[vertice][i].HasValue)
                vizinhos.Add(i);
        }

        return vizinhos;
    }

    public override void ImprimeGrafo()
    {
        Console.WriteLine("=== Grafo por Matriz de Adjacência ===");

        Console.Write("     ");
        for (int i = 0; i < vertices.Count; i++)
            Console.Write($"{vertices[i],8}");

        Console.WriteLine();

        for (int i = 0; i < vertices.Count; i++)
        {
            Console.Write($"{vertices[i],4} ");

            for (int j = 0; j < vertices.Count; j++)
            {
                string valor = matriz[i][j].HasValue
                    ? matriz[i][j]!.Value.ToString("0.##")
                    : "0";

                Console.Write($"{valor,8}");
            }

            Console.WriteLine();
        }
    }
}
