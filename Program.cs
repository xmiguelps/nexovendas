using NexoVendas;
using System.Globalization; // Importa a classe para formatação de números no padrão brasileiro (ex: 15,50 ao invés de 15.50)

// Instancia a classe DAO, que conterá toda a lógica de acesso ao banco
SistemaDAO dao = new SistemaDAO();

// Variável booleana que mantém o programa rodando no laço de repetição
bool rodando = true;

// Laço while: continuará executando enquanto a variável 'rodando' for verdadeira
while (rodando)
{
    // Limpa o terminal para evitar poluição visual entre as telas
    Console.Clear();
    
    // Exibe o título e as opções do menu principal
    Console.WriteLine("=== SISTEMA NEXOVENDAS ===");
    Console.WriteLine("1. Cadastrar Cliente");
    Console.WriteLine("2. Consultar Clientes");
    Console.WriteLine("3. Alterar Cliente");
    Console.WriteLine("4. Cadastrar Produto");
    Console.WriteLine("5. Consultar Produtos");
    Console.WriteLine("6. Alterar Produto");
    Console.WriteLine("7. Realizar Venda (Carrinho)");
    Console.WriteLine("8. Gerar Relatório CSV Geral");
    Console.WriteLine("9. Sair");
    Console.Write("Escolha uma opção: ");

    // Tenta converter o texto que o usuário digitou para inteiro (int)
    // Se falhar (ex: digitou letra), o 'out' não é preenchido e entra no IF
    if (!int.TryParse(Console.ReadLine(), out int opcao))
    {
        // Exibe erro de digitação
        Console.WriteLine("\nEntrada inválida! Por favor, digite apenas números.");
        Console.WriteLine("Pressione qualquer tecla para tentar novamente...");
        // Espera o usuário apertar algo
        Console.ReadKey();
        // O comando 'continue' força o laço while a ignorar o resto do código e reiniciar
        continue; 
    }

    // O comando switch avalia a opção numérica escolhida e direciona o fluxo
    switch (opcao)
    {
        case 1:
            // Solicita os dados do novo cliente
            Console.Write("Nome do Cliente: ");
            string nome = Console.ReadLine();
            Console.Write("Email do Cliente: ");
            string email = Console.ReadLine();
            
            // Instancia um objeto Cliente e passa para o método Cadastrar do DAO
            dao.CadastrarCliente(new Cliente(nome, email));
            break;

        case 2:
            // Chama a função que exibe os clientes direto no console
            dao.ConsultarClientes();
            break;

        case 3:
            // Solicita o ID do cliente alvo
            Console.Write("Digite o ID do Cliente que deseja alterar: ");
            
            // Verifica se o ID digitado é um número válido
            if (int.TryParse(Console.ReadLine(), out int idAltC))
            {
                // Informa a regra de ignorar a alteração apertando ENTER
                Console.WriteLine("(Pressione ENTER para manter o valor atual)");
                
                Console.Write("Novo Nome: ");
                string novoNome = Console.ReadLine();
                
                Console.Write("Novo Email: ");
                string novoEmail = Console.ReadLine();
                
                // Envia o ID e os novos dados para o banco processar o UPDATE
                dao.AlterarCliente(idAltC, novoNome, novoEmail);
            }
            else
            {
                // Caso digite letra no ID
                Console.WriteLine("ID inválido.");
            }
            break;

        case 4:
            // Solicita dados do produto
            Console.Write("Nome do Produto: ");
            string prod = Console.ReadLine();
            Console.Write("Preço do Produto (Ex: 15,50): ");
            
            // Valida se o preço digitado pode ser convertido para número Double
            if (double.TryParse(Console.ReadLine(), out double preco))
            {
                // Instancia um Produto e salva no banco
                dao.CadastrarProduto(new Produto(prod, preco));
            }
            else 
            {
                // Erro de formatação do preço
                Console.WriteLine("Preço inválido. Use vírgula para centavos.");
            }
            break;

        case 5:
            // Chama a função de consulta de produtos
            dao.ConsultarProdutos();
            break;

        case 6:
            // Alteração de Produto
            Console.Write("Digite o ID do Produto que deseja alterar: ");
            
            // Valida o ID
            if (int.TryParse(Console.ReadLine(), out int idAltP))
            {
                Console.WriteLine("(Pressione ENTER para manter o valor atual)");
                
                Console.Write("Novo Nome do Produto: ");
                string novoProd = Console.ReadLine();
                
                Console.Write("Novo Preço (Ex: 25,90): ");
                // Recebe o preço como texto (string) para que o REPLACE do SQL possa agir
                string novoPreco = Console.ReadLine(); 
                
                // Manda para o DAO
                dao.AlterarProduto(idAltP, novoProd, novoPreco);
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
            break;

        case 7:
            // Início do módulo de Caixa / Carrinho de Compras
            Console.WriteLine("\n--- NOVA VENDA ---");
            
            Console.Write("ID do Cliente para iniciar a nota: ");
            
            // Valida se o ID do cliente é um número
            if (!int.TryParse(Console.ReadLine(), out int idC)) 
            {
                Console.WriteLine("ID inválido.");
                // Sai do Case 7 imediatamente e volta pro menu
                break;
            }

            // Chama a função de iniciar venda e armazena o número da nota fiscal gerada
            int idVendaAtual = dao.IniciarVenda(idC);
            
            // Variável local que vai somando o dinheiro total do carrinho
            double valorTotalDaVenda = 0;
            
            // Variável de controle do laço do carrinho de compras
            bool adicionandoItens = true;

            // Laço que mantém o caixa aberto passando os produtos
            while (adicionandoItens)
            {
                // Mostra as instruções de uso do caixa (Adicionar, Remover ou Finalizar)
                Console.Write("\nID do Produto [0 para Finalizar | ID Negativo (Ex: -2) para Remover]: ");
                
                // Tenta ler o código de barras (ID) do produto
                int.TryParse(Console.ReadLine(), out int idP);

                // Regra 1: Se o código for 0, finaliza a compra
                if (idP == 0)
                {
                    // Altera a variável booleana, forçando a saída do laço while do carrinho
                    adicionandoItens = false; 
                }
                // Regra 2: Se o código for negativo, aciona a função de remoção
                else if (idP < 0) 
                {
                    // A função Math.Abs() tira o sinal de menos (ex: -5 vira 5)
                    // Envia o ID limpo para o banco achar a linha e apagar
                    double valorEstornado = dao.RemoverItemVenda(idVendaAtual, Math.Abs(idP));
                    
                    // O valor que foi estornado do banco é subtraído da variável local do carrinho
                    valorTotalDaVenda -= valorEstornado; 
                }
                // Regra 3: Se o código for positivo normal, insere o produto na nota
                else 
                {
                    Console.Write("Quantidade: ");
                    
                    // Valida se a quantidade digitada é um número inteiro maior que zero
                    if (int.TryParse(Console.ReadLine(), out int qtd) && qtd > 0)
                    {
                        // Envia os dados para o banco registrar e recebe o custo daquele item
                        double subtotalItem = dao.AdicionarItemVenda(idVendaAtual, idP, qtd);
                        
                        // Se o subtotal for maior que 0, significa que o produto existia e foi registrado
                        if (subtotalItem > 0)
                        {
                            // Soma o custo do item à variável local do carrinho
                            valorTotalDaVenda += subtotalItem; 
                        }
                        else
                        {
                            // Se o banco retornar 0, o ID digitado não existe na tabela de produtos
                            Console.WriteLine("Produto não encontrado.");
                        }
                    }
                    else
                    {
                        // Tratamento de erro para letras ou quantidade zero/negativa
                        Console.WriteLine("Quantidade inválida.");
                    }
                }
                
                // A cada rodada do laço (seja adicionando ou removendo), mostra o visor do caixa atualizado
                Console.WriteLine($"=> Total Parcial da Nota: R$ {valorTotalDaVenda.ToString("N2", new CultureInfo("pt-BR"))}");
            }

            // Ao sair do laço (digitou 0), chama a função que grava o total final na tabela de vendas
            dao.FinalizarVenda(idVendaAtual, valorTotalDaVenda);
            break;

        case 8:
            // Executa a função que extrai as informações do banco para um arquivo físico
            dao.GerarRelatorioCSV();
            break;

        case 9:
            // Modifica a variável do laço principal do menu para encerrar a aplicação
            rodando = false;
            Console.WriteLine("Encerrando o sistema...");
            break;

        default:
            // Tratamento caso o usuário digite um número que não está no menu (ex: 15)
            Console.WriteLine("Opção não existe no menu.");
            break;
    }

    // Após rodar a opção escolhida, se o programa não foi encerrado (opção 9)...
    if (rodando)
    {
        // Dá uma pausa na tela para o aluno poder ler os resultados antes do Console.Clear() agir
        Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }
}