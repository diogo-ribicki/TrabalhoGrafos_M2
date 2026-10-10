namespace TrabalhoGrafos;

public class Aresta
{
    public int Destino { get; set; }
    public float Peso { get; set; }

    public Aresta(int destino, float peso = 1)
    {
        Destino = destino;
        Peso = peso;
    }
}
