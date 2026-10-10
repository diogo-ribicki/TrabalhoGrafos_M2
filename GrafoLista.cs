namespace TrabalhoGrafos;

public class GrafoLista : Grafos
{
    private readonly List<List<Aresta>> lista = new();

    public GrafoLista(bool direcionado, bool ponderado)
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
        lista.Add(new List<Aresta>());
        return true;
    }

    public override bool RemoverVertice(int indice)
    {
        if (indice < 0 || indice >= vertices.Count)
            return false;

        vertices.RemoveAt(indice);
        lista.RemoveAt(indice);

        // Remove as arestas que chegavam ao vértice removido
        // e ajusta os índices dos vértices maiores que ele.
        foreach (var arestas in lista)
        {
            arestas.RemoveAll(a => a.Destino == indice);

            foreach (var aresta in arestas)
            {
                if (aresta.Destino > indice)
                    aresta.Destino--;
            }
        }

        return true;
    }

    public override bool InserirAresta(int origem, int destino, float peso = 1)
    {
        if (!IndicesValidos(origem, destino))
            return false;

        if (ExisteAresta(origem, destino))
            return false;

        float pesoFinal = PesoNormalizado(peso);
        lista[origem].Add(new Aresta(destino, pesoFinal));

        if (!Direcionado && origem != destino)
            lista[destino].Add(new Aresta(origem, pesoFinal));

        return true;
    }

    public override bool RemoverAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino))
            return false;

        var aresta = lista[origem].FirstOrDefault(a => a.Destino == destino);

        if (aresta == null)
            return false;

        lista[origem].Remove(aresta);

        if (!Direcionado && origem != destino)
        {
            var retorno = lista[destino].FirstOrDefault(a => a.Destino == origem);
            if (retorno != null)
                lista[destino].Remove(retorno);
        }

        return true;
    }

    public override bool ExisteAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino))
            return false;

        return lista[origem].Any(a => a.Destino == destino);
    }

    public override float PesoAresta(int origem, int destino)
    {
        if (!IndicesValidos(origem, destino))
            throw new ArgumentOutOfRangeException();

        var aresta = lista[origem].FirstOrDefault(a => a.Destino == destino);

        if (aresta == null)
            throw new InvalidOperationException("Aresta inexistente.");

        return aresta.Peso;
    }

    public override List<int> RetornarVizinhos(int vertice)
    {
        ValidarIndice(vertice);

        return lista[vertice]
            .Select(a => a.Destino)
            .ToList();
    }

    public override void ImprimeGrafo()
    {
        Console.WriteLine("=== Grafo por Lista de Adjacência ===");

        for (int i = 0; i < vertices.Count; i++)
        {
            Console.Write($"{i} ({vertices[i]}) -> ");

            foreach (var aresta in lista[i])
            {
                Console.Write(
                    $"{aresta.Destino} ({vertices[aresta.Destino]})"
                    + (Ponderado ? $"[{aresta.Peso:0.##}]" : "")
                    + " -> "
                );
            }

            Console.WriteLine("NULL");
        }
    }
}
