namespace TrabalhoGrafos;

public class Program
{
    public static void Main()
    {
        Grafos? grafo = null;

        MostrarTelaInicial();

        while (true)
        {
            MostrarMenu(grafo);
            int escolha = LerInteiro("Escolha uma opção: ", 0, 13);
            Console.WriteLine();

            try
            {
                switch (escolha)
                {
                    case 1:
                        grafo = CarregarGrafo();
                        break;

                    case 2:
                        ExibirGrafo(grafo);
                        break;

                    case 3:
                        ExecutarBusca(grafo, profundidade: false);
                        break;

                    case 4:
                        ExecutarBusca(grafo, profundidade: true);
                        break;

                    case 5:
                        ExecutarDijkstra(grafo);
                        break;

                    case 6:
                        VerificarConectividade(grafo);
                        break;

                    case 7:
                        ExigirGrafo(grafo);
                        ExibirColoracao(grafo, AlgoritmosM2.ColoracaoGulosa(grafo!));
                        break;

                    case 8:
                        ExigirGrafo(grafo);
                        ExibirColoracao(grafo, AlgoritmosM2.WelshPowell(grafo!));
                        break;

                    case 9:
                        ExigirGrafo(grafo);
                        ExibirColoracao(grafo, AlgoritmosM2.DSATUR(grafo!));
                        break;

                    case 10:
                        ExecutarForcaBruta(grafo);
                        break;

                    case 11:
                        ExigirGrafo(grafo);
                        ExibirArvoreGeradora(grafo, AlgoritmosM2.Prim(grafo!));
                        break;

                    case 12:
                        ExigirGrafo(grafo);
                        ExibirArvoreGeradora(grafo, AlgoritmosM2.Kruskal(grafo!));
                        break;

                    case 13:
                        ExibirInformacoes(grafo);
                        break;

                    case 0:
                        Console.WriteLine("Encerrando o programa.");
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
            }

            Console.WriteLine();
            PausarAntesDoMenu();
        }
    }

    private static void PausarAntesDoMenu()
    {
        Console.WriteLine();
        Console.Write("Pressione qualquer tecla para voltar ao menu...");

        try
        {
            Console.ReadKey(intercept: true);
        }
        catch (InvalidOperationException)
        {
            // Fallback para quando a entrada do console estiver redirecionada.
            Console.ReadLine();
        }

        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            Console.WriteLine();
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine();
        }
    }

    private static void MostrarTelaInicial()
    {
        Console.WriteLine("====================================================");
        Console.WriteLine("          TRABALHO DE GRAFOS - M2");
        Console.WriteLine("       Coloração e árvore geradora mínima");
        Console.WriteLine("====================================================");
        Console.WriteLine();
        Console.WriteLine("Projeto baseado no trabalho M1 de Grafos.");
        Console.WriteLine();
        Console.Write("Pressione qualquer tecla para acessar o menu...");

        // Aguarda uma tecla antes de mostrar as opções do programa.
        Console.ReadKey(intercept: true);

        // Limpa a tela para deixar o menu mais organizado.
        // Se o terminal não permitir limpar a tela, o programa continua normalmente.
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            Console.WriteLine();
        }
    }

    private static void MostrarMenu(Grafos? grafo)
    {
        Console.WriteLine();
        Console.WriteLine("------------------------- MENU -------------------------");
        Console.WriteLine(grafo == null
            ? "Grafo atual: nenhum carregado"
            : $"Grafo atual: {grafo.QuantidadeVertices} vértices | " +
              $"{(grafo.Direcionado ? "direcionado" : "não direcionado")} | " +
              $"{(grafo.Ponderado ? "ponderado" : "não ponderado")}");
        Console.WriteLine("1  - Carregar/recarregar grafo de arquivo");
        Console.WriteLine("2  - Imprimir grafo");
        Console.WriteLine("3  - Busca em largura (BFS - M1)");
        Console.WriteLine("4  - Busca em profundidade (DFS - M1)");
        Console.WriteLine("5  - Dijkstra (M1)");
        Console.WriteLine("6  - Verificar conectividade");
        Console.WriteLine("7  - Coloração gulosa (ordem dos vértices)");
        Console.WriteLine("8  - Coloração Welsh-Powell");
        Console.WriteLine("9  - Coloração DSATUR");
        Console.WriteLine("10 - Coloração por força bruta");
        Console.WriteLine("11 - Árvore geradora mínima: Prim");
        Console.WriteLine("12 - Árvore geradora mínima: Kruskal");
        Console.WriteLine("13 - Informações do grafo");
        Console.WriteLine("0  - Sair");
        Console.WriteLine("--------------------------------------------------------");
    }

    private static Grafos CarregarGrafo()
    {
        Console.Write("Caminho do arquivo (ex.: exemplo_grafo_n_n.txt): ");
        string caminho = (Console.ReadLine() ?? string.Empty).Trim().Trim('"');

        if (string.IsNullOrWhiteSpace(caminho))
            throw new InvalidOperationException("Informe o caminho do arquivo.");

        // O leitor interpreta V A D P e cria automaticamente um grafo por lista de adjacência.
        // Isso evita uma matriz V x V para arquivos com milhares de vértices.
        Grafos novoGrafo = LeitorGrafos.CarregarListaDeArquivo(caminho);
        Console.WriteLine("Grafo carregado com sucesso.");
        Console.WriteLine($"Vértices: {novoGrafo.QuantidadeVertices}");
        Console.WriteLine($"Direcionado: {(novoGrafo.Direcionado ? "sim" : "não")}");
        Console.WriteLine($"Ponderado: {(novoGrafo.Ponderado ? "sim" : "não")}");
        return novoGrafo;
    }

    private static void ExibirGrafo(Grafos? grafo)
    {
        ExigirGrafo(grafo);
        if (grafo!.QuantidadeVertices > 100 && !LerSimNao(
                "O grafo tem mais de 100 vértices e a impressão pode ocupar muitas linhas. Continuar? (s/n): "))
            return;

        grafo.ImprimeGrafo();
    }

    private static void ExecutarBusca(Grafos? grafo, bool profundidade)
    {
        ExigirGrafo(grafo);
        int origem = LerInteiro($"Vértice de origem (0 a {grafo!.QuantidadeVertices - 1}): ",
            0, grafo.QuantidadeVertices - 1);

        List<int> ordem = profundidade
            ? Algoritmos.BuscaProfundidade(grafo, origem)
            : Algoritmos.BuscaLargura(grafo, origem);

        Console.WriteLine((profundidade ? "DFS" : "BFS") + ": " +
            string.Join(" -> ", ordem.Select(v => grafo.LabelVertice(v))));
        Console.WriteLine($"Vértices visitados: {ordem.Count}");
    }

    private static void ExecutarDijkstra(Grafos? grafo)
    {
        ExigirGrafo(grafo);
        int origem = LerInteiro($"Vértice de origem (0 a {grafo!.QuantidadeVertices - 1}): ",
            0, grafo.QuantidadeVertices - 1);
        ResultadoDijkstra resultado = Algoritmos.Dijkstra(grafo, origem);
        resultado.Imprimir(grafo);
    }

    private static void VerificarConectividade(Grafos? grafo)
    {
        ExigirGrafo(grafo);
        Console.WriteLine(grafo!.EhConexo()
            ? "O grafo é conexo (considerando a busca a partir do vértice 0)."
            : "O grafo não é conexo (considerando a busca a partir do vértice 0).");
    }

    private static void ExecutarForcaBruta(Grafos? grafo)
    {
        ExigirGrafo(grafo);

        if (grafo!.QuantidadeVertices > 25)
        {
            Console.WriteLine("Atenção: força bruta tem crescimento exponencial e pode demorar muito para este grafo.");
            if (!LerSimNao("Deseja executar mesmo assim? (s/n): "))
            {
                Console.WriteLine("Execução cancelada.");
                return;
            }
        }

        ExibirColoracao(grafo, AlgoritmosM2.ForcaBruta(grafo));
    }

    private static void ExibirColoracao(Grafos? grafo, ResultadoColoracao resultado)
    {
        ExigirGrafo(grafo);
        Console.WriteLine($"=== {resultado.NomeAlgoritmo} ===");
        Console.WriteLine($"Número de cores utilizadas: {resultado.QuantidadeCores}");
        Console.WriteLine($"Tempo de execução: {resultado.TempoExecucao.TotalMilliseconds:0.###} ms ({resultado.TempoExecucao.Ticks} ticks)");

        // O enunciado solicita a listagem dos vértices apenas para grafos com menos de 10 vértices.
        if (grafo!.QuantidadeVertices < 10)
        {
            Console.WriteLine("Cores por vértice:");
            for (int v = 0; v < resultado.Cores.Length; v++)
                Console.WriteLine($"  Vértice {grafo.LabelVertice(v)} -> cor {resultado.Cores[v] + 1}");
        }
    }

    private static void ExibirArvoreGeradora(Grafos? grafo, ResultadoArvoreGeradora resultado)
    {
        ExigirGrafo(grafo);
        Console.WriteLine($"=== Árvore geradora mínima - {resultado.NomeAlgoritmo} ===");
        Console.WriteLine($"Tempo de execução: {resultado.TempoExecucao.TotalMilliseconds:0.###} ms ({resultado.TempoExecucao.Ticks} ticks)");
        Console.WriteLine($"Soma dos pesos das arestas: {resultado.PesoTotal:0.###}");
        Console.WriteLine($"Arestas selecionadas: {resultado.Arestas.Count}");

        if (resultado.QuantidadeVertices == 0)
            Console.WriteLine("O grafo está vazio; não há árvore geradora.");
        else if (resultado.TemArvoreGeradora)
            Console.WriteLine("Árvore geradora mínima encontrada.");
        else
            Console.WriteLine($"O grafo é desconexo. Foi calculada uma floresta geradora mínima com {resultado.QuantidadeComponentes} componentes.");

        if (grafo!.QuantidadeVertices < 10)
        {
            Console.WriteLine("Arestas selecionadas:");
            foreach (ArestaResultado aresta in resultado.Arestas)
            {
                Console.WriteLine(
                    $"  {grafo.LabelVertice(aresta.Origem)} -- {grafo.LabelVertice(aresta.Destino)} (peso {aresta.Peso:0.###})");
            }
        }
    }

    private static void ExibirInformacoes(Grafos? grafo)
    {
        ExigirGrafo(grafo);
        int totalArestas = 0;
        for (int v = 0; v < grafo!.QuantidadeVertices; v++)
            totalArestas += grafo.RetornarVizinhos(v).Count;

        if (!grafo.Direcionado)
            totalArestas /= 2;

        Console.WriteLine("=== Informações do grafo ===");
        Console.WriteLine($"Vértices: {grafo.QuantidadeVertices}");
        Console.WriteLine($"Arestas: {totalArestas}");
        Console.WriteLine($"Direcionado: {(grafo.Direcionado ? "sim" : "não")}");
        Console.WriteLine($"Ponderado: {(grafo.Ponderado ? "sim" : "não")}");
        Console.WriteLine("Representação: lista de adjacência");
    }

    private static void ExigirGrafo(Grafos? grafo)
    {
        if (grafo == null)
            throw new InvalidOperationException("Carregue um grafo primeiro, usando a opção 1.");

        if (grafo.QuantidadeVertices == 0)
            throw new InvalidOperationException("O grafo carregado não possui vértices.");
    }

    private static bool LerSimNao(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            string resposta = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

            if (resposta is "s" or "sim")
                return true;
            if (resposta is "n" or "nao" or "não")
                return false;

            Console.WriteLine("Digite s ou n.");
        }
    }

    private static int LerInteiro(string mensagem, int? minimo = null, int? maximo = null)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (int.TryParse(Console.ReadLine(), out int valor) &&
                (!minimo.HasValue || valor >= minimo.Value) &&
                (!maximo.HasValue || valor <= maximo.Value))
                return valor;

            Console.WriteLine("Digite um número inteiro válido" +
                (minimo.HasValue && maximo.HasValue ? $" entre {minimo} e {maximo}." : "."));
        }
    }
}
