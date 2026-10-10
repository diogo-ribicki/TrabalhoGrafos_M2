namespace TrabalhoGrafos;

public abstract class Grafos
{
    protected readonly List<string> vertices = new();

    public bool Direcionado { get; }
    public bool Ponderado { get; }

    protected Grafos(bool direcionado, bool ponderado)
    {
        Direcionado = direcionado;
        Ponderado = ponderado;
    }

    public int QuantidadeVertices => vertices.Count;

    public abstract bool InserirVertice(string label);
    public abstract bool RemoverVertice(int indice);
    public abstract bool InserirAresta(int origem, int destino, float peso = 1);
    public abstract bool RemoverAresta(int origem, int destino);
    public abstract bool ExisteAresta(int origem, int destino);
    public abstract float PesoAresta(int origem, int destino);
    public abstract List<int> RetornarVizinhos(int vertice);
    public abstract void ImprimeGrafo();

    public string LabelVertice(int indice)
    {
        ValidarIndice(indice);
        return vertices[indice];
    }

    public List<string> LabelsVertices()
    {
        return new List<string>(vertices);
    }

    protected void ValidarIndice(int indice)
    {
        if (indice < 0 || indice >= vertices.Count)
            throw new ArgumentOutOfRangeException(nameof(indice), $"Vértice inválido: {indice}.");
    }

    protected bool IndicesValidos(int origem, int destino)
    {
        return origem >= 0 && origem < vertices.Count &&
               destino >= 0 && destino < vertices.Count;
    }

    protected float PesoNormalizado(float peso)
    {
        return Ponderado ? peso : 1;
    }

    public virtual void LimparGrafo()
    {
        while (vertices.Count > 0)
            RemoverVertice(vertices.Count - 1);
    }

    public bool EhConexo()
    {
        if (vertices.Count == 0)
            return true;

        var alcancados = Algoritmos.BuscaLargura(this, 0);
        return alcancados.Count == vertices.Count;
    }
}