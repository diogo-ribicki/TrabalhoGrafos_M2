TRABALHO DE GRAFOS - M2 (C# / .NET 8)
Baseado no código do M1 enviado.

FUNCIONALIDADES
- Leitura de grafos em arquivo texto no formato V A D P.
- BFS, DFS e Dijkstra do M1 preservados.
- Coloração gulosa simples, usando a ordem dos vértices.
- Coloração Welsh-Powell.
- Coloração DSATUR.
- Coloração por força bruta com retrocesso, tentando 2 cores, depois 3 e assim por diante.
- Árvore geradora mínima com Prim.
- Árvore geradora mínima com Kruskal e estrutura Union-Find.
- Medição de tempo de execução com Stopwatch.
- Para grafos com menos de 10 vértices, são impressas as cores por vértice e as arestas da árvore/floresta.

COMO EXECUTAR
1. Instale o .NET 8 SDK (ou superior).
2. Abra um terminal na pasta que contém TrabalhoGrafos.csproj.
3. Execute: dotnet run
4. Escolha a opção 1 e informe o caminho de um arquivo .txt, por exemplo:
   exemplo_grafo_n_n.txt
5. Escolha o algoritmo no menu.

FORMATO DO ARQUIVO
A primeira linha contém:
V A D P

V = quantidade de vértices
A = quantidade de arestas
D = 0 para não direcionado; 1 para direcionado
P = 0 para não ponderado; 1 para ponderado

Depois vêm A linhas com:
origem destino

Para arquivos ponderados:
origem destino peso

Exemplo não ponderado e não direcionado:
5 6 0 0
0 1
0 2
1 2
1 3
2 3
3 4

OBSERVAÇÕES IMPORTANTES
- O leitor cria automaticamente uma representação por lista de adjacência, adequada a grafos grandes.
- Os algoritmos de coloração e Prim/Kruskal exigem grafos não direcionados.
- Os algoritmos de coloração ignoram os pesos das arestas.
- Prim e Kruskal utilizam o peso informado no arquivo. Em grafo não ponderado, cada aresta tem peso 1.
- Se o grafo for desconexo, Prim e Kruskal exibem a floresta geradora mínima e a quantidade de componentes; não existe uma árvore geradora única nesse caso.
- Força bruta possui crescimento exponencial e pode demorar muito em grafos médios/grandes. O programa pede confirmação para grafos com mais de 25 vértices.
- Para um grafo sem arestas, a força bruta retorna uma cor, pois essa é a coloração mínima.
- Grafos com laços (aresta de um vértice para ele mesmo) não podem receber coloração própria e são reportados como erro.

ARQUIVOS PRINCIPAIS
- Grafos.cs, GrafoLista.cs, GrafoMatriz.cs, Aresta.cs: base do M1.
- LeitorGrafos.cs: parser do formato V A D P, com carregamento automático pela lista.
- Algoritmos.cs: BFS, DFS e Dijkstra do M1.
- AlgoritmosM2.cs: coloração, Prim, Kruskal e resultados/tempos.
- Program.cs: menu de execução.
