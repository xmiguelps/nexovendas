using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO; // Necessário para gerar arquivos CSV
using MySql.Data.MySqlClient;
using System.Globalization; // Necessário para formatação de moeda

namespace NexoVendas
{
    public class SistemaDAO
    {
        // Variável que guarda a nossa configuração de conexão
        private readonly ConexaoBanco _conexaoBanco;

        // Construtor: sempre que o DAO for chamado, ele prepara a conexão
        public SistemaDAO()
        {
            _conexaoBanco = new ConexaoBanco();
        }

        // ==========================================
        // SESSÃO DE CLIENTES
        // ==========================================

        // Método para salvar um novo cliente no banco
        public void CadastrarCliente(Cliente cliente)
        {
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // Comando SQL para inserir dados. Usamos @ para evitar invasões (SQL Injection)
                string query = "INSERT INTO tb_cliente (nm_cliente, ds_email) VALUES (@nome, @email)";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    // Trocamos os @ pelos dados que vieram do objeto cliente
                    comando.Parameters.AddWithValue("@nome", cliente.Nome);
                    comando.Parameters.AddWithValue("@email", cliente.Email);
                    conexao.Open(); // Abre a porta do banco
                    comando.ExecuteNonQuery(); // Executa a inserção
                }
            }
            Console.WriteLine("Cliente cadastrado com sucesso!");
        }

        // Método para ler e mostrar todos os clientes direto do banco
        public void ConsultarClientes()
        {
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                string query = "SELECT cd_cliente, nm_cliente, ds_email FROM tb_cliente";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    conexao.Open();
                    // O ExecuteReader lê as linhas retornadas pelo banco
                    using (var reader = comando.ExecuteReader())
                    {
                        Console.WriteLine("\n--- LISTA DE CLIENTES ---");
                        // O laço while repete enquanto houver linhas para ler no banco
                        while (reader.Read())
                        {
                            // Imprime direto na tela, linha por linha
                            Console.WriteLine($"ID: {reader["cd_cliente"]} | Nome: {reader["nm_cliente"]} | Email: {reader["ds_email"]}");
                        }
                    }
                }
            }
        }

        // Método para atualizar os dados de um cliente existente
    public void AlterarCliente(int id, string novoNome, string novoEmail)
    {
        // Prepara a conexão com o banco de dados
        using (var conexao = _conexaoBanco.ObterConexao())
        {
            // Comando UPDATE com a função IF: se o valor recebido for vazio (''), mantém o dado atual da coluna
            string query = @"UPDATE tb_cliente 
                             SET nm_cliente = IF(@nome = '', nm_cliente, @nome), 
                                 ds_email = IF(@email = '', ds_email, @email) 
                             WHERE cd_cliente = @id";
            
            // Prepara o comando
            using (var comando = new MySqlCommand(query, conexao))
            {
                // Vincula o novo nome digitado ao parâmetro @nome
                comando.Parameters.AddWithValue("@nome", novoNome);
                // Vincula o novo email digitado ao parâmetro @email
                comando.Parameters.AddWithValue("@email", novoEmail);
                // Vincula o ID do cliente que será alterado ao parâmetro @id
                comando.Parameters.AddWithValue("@id", id);
                
                // Abre a conexão
                conexao.Open();
                // Executa a atualização no banco de dados
                comando.ExecuteNonQuery();
            }
        }
        // Avisa ao usuário que os dados foram atualizados
        Console.WriteLine("Dados do cliente atualizados com sucesso!");
    }

    // ==========================================
    // SESSÃO DE PRODUTOS
    // ==========================================

    // Método para cadastrar um novo produto no banco
    public void CadastrarProduto(Produto produto)
    {
        // Inicia a conexão
        using (var conexao = _conexaoBanco.ObterConexao())
        {
            // Define o comando SQL de inserção na tabela de produtos
            string query = "INSERT INTO tb_produto (nm_produto, vl_preco) VALUES (@nome, @preco)";
            
            // Cria o comando
            using (var comando = new MySqlCommand(query, conexao))
            {
                // Passa o nome do produto para o parâmetro
                comando.Parameters.AddWithValue("@nome", produto.Nome);
                // Passa o preço do produto para o parâmetro
                comando.Parameters.AddWithValue("@preco", produto.Preco);
                
                // Abre a conexão
                conexao.Open();
                // Executa a inserção
                comando.ExecuteNonQuery();
            }
        }
        // Exibe a mensagem de sucesso
        Console.WriteLine("Produto cadastrado com sucesso!");
    }

    // Método para listar todos os produtos cadastrados
    public void ConsultarProdutos()
    {
        // Inicia a conexão
        using (var conexao = _conexaoBanco.ObterConexao())
        {
            // Define o comando SQL para buscar os produtos
            string query = "SELECT cd_produto, nm_produto, vl_preco FROM tb_produto";
            
            // Prepara o comando
            using (var comando = new MySqlCommand(query, conexao))
            {
                // Abre a conexão
                conexao.Open();
                
                // Executa a leitura dos dados
                using (var reader = comando.ExecuteReader())
                {
                    // Imprime o cabeçalho
                    Console.WriteLine("\n--- LISTA DE PRODUTOS ---");
                    
                    // Lê cada linha retornada pelo banco
                    while (reader.Read())
                    {
                        // Imprime o ID, Nome e Preço do produto formatado na tela
                        Console.WriteLine($"ID: {reader["cd_produto"]} | Produto: {reader["nm_produto"]} | Preço: R${reader["vl_preco"]}");
                    }
                }
            }
        }
    }

        // Método para alterar dados de um produto existente
        public void AlterarProduto(int id, string novoNome, string novoPrecoTexto)
        {
            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // O comando REPLACE substitui a vírgula (padrão BR) por ponto (padrão SQL) para não dar erro no CAST DECIMAL
                string query = @"UPDATE tb_produto 
                                SET nm_produto = IF(@nome = '', nm_produto, @nome), 
                                    vl_preco = IF(@preco = '', vl_preco, CAST(REPLACE(@preco, ',', '.') AS DECIMAL(10,2))) 
                                WHERE cd_produto = @id";
                
                // Prepara o comando
                using (var comando = new MySqlCommand(query, conexao))
                {
                    // Vincula o novo nome ao parâmetro
                    comando.Parameters.AddWithValue("@nome", novoNome);
                    // Vincula o novo preço (em texto) ao parâmetro
                    comando.Parameters.AddWithValue("@preco", novoPrecoTexto);
                    // Vincula o ID do produto
                    comando.Parameters.AddWithValue("@id", id);
                    
                    // Abre a conexão
                    conexao.Open();
                    // Executa a atualização
                    comando.ExecuteNonQuery();
                }
            }
            // Informa que a alteração deu certo
            Console.WriteLine("Dados do produto atualizados com sucesso!");
        }

        // ==========================================
        // SESSÃO DE VENDAS (CARRINHO)
        // ==========================================

        // Passo 1: Abre a nota fiscal e retorna o número dela
        public int IniciarVenda(int idCliente)
        {
            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // Insere a venda na tabela mestre e logo em seguida busca o último ID gerado pelo AUTO_INCREMENT
                string query = "INSERT INTO tb_venda (fk_cliente) VALUES (@idC); SELECT LAST_INSERT_ID();";
                
                // Prepara o comando
                using (var comando = new MySqlCommand(query, conexao))
                {
                    // Adiciona o ID do cliente que está comprando
                    comando.Parameters.AddWithValue("@idC", idCliente);
                    // Abre a conexão
                    conexao.Open();
                    
                    // O ExecuteScalar executa o comando e retorna apenas a primeira coluna da primeira linha (o ID da venda)
                    return Convert.ToInt32(comando.ExecuteScalar());
                }
            }
        }

        // Passo 2: Adiciona um item no carrinho de compras
        public double AdicionarItemVenda(int idVenda, int idProduto, int quantidade)
        {
            // Variável para guardar o nome do produto e mostrar na tela
            string nomeProduto = "";
            // Variável para guardar o preço unitário vindo do banco
            double precoProduto = 0;

            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // Primeiro comando SQL: busca o nome e o preço do produto pelo ID
                string queryPreco = "SELECT nm_produto, vl_preco FROM tb_produto WHERE cd_produto = @idP";
                
                // Prepara a consulta de preço
                using (var cmdPreco = new MySqlCommand(queryPreco, conexao))
                {
                    // Adiciona o parâmetro do ID do produto
                    cmdPreco.Parameters.AddWithValue("@idP", idProduto);
                    // Abre a conexão
                    conexao.Open();
                    
                    // Executa a leitura
                    using (var reader = cmdPreco.ExecuteReader())
                    {
                        // Se não encontrar nenhum produto com esse ID, retorna o valor 0 (cancela a adição)
                        if (!reader.Read()) return 0; 
                        
                        // Salva o nome do produto encontrado na variável
                        nomeProduto = reader["nm_produto"].ToString();
                        // Salva e converte o preço encontrado para a variável double
                        precoProduto = Convert.ToDouble(reader["vl_preco"]);
                    }
                }

                // Calcula o subtotal deste item multiplicando preço pela quantidade
                double subtotal = precoProduto * quantidade;

                // Segundo comando SQL: insere os dados calculados na tabela de itens da venda
                string queryItem = "INSERT INTO tb_itens_venda (fk_venda, fk_produto, qt_produto, vl_subtotal) VALUES (@idV, @idP, @qtd, @sub)";
                
                // Prepara a inserção do item
                using (var cmdItem = new MySqlCommand(queryItem, conexao))
                {
                    // Vincula o ID da Venda (nota fiscal)
                    cmdItem.Parameters.AddWithValue("@idV", idVenda);
                    // Vincula o ID do Produto
                    cmdItem.Parameters.AddWithValue("@idP", idProduto);
                    // Vincula a Quantidade solicitada
                    cmdItem.Parameters.AddWithValue("@qtd", quantidade);
                    // Vincula o Subtotal calculado
                    cmdItem.Parameters.AddWithValue("@sub", subtotal);
                    
                    // Executa a inserção na tabela detalhe
                    cmdItem.ExecuteNonQuery();
                }
                
                // Exibe o feedback para o usuário de que o item foi adicionado com sucesso
                Console.WriteLine($"\n[+] {quantidade}x {nomeProduto} lançado(s)! Subtotal do item: R$ {subtotal.ToString("N2", new CultureInfo("pt-BR"))}");
                
                // Devolve o subtotal calculado para o C# somar no total geral da compra
                return subtotal; 
            }
        }

        // Passo 2.1: Remove um item lançado errado da nota
        public double RemoverItemVenda(int idVenda, int idProduto)
        {
            // Variável para guardar o valor que será devolvido do total
            double subtotalAbatido = 0;
            // Variável para guardar o ID exato da linha que será apagada
            int idItem = 0;

            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // SQL: Busca o ÚLTIMO item lançado com esse produto nessa venda específica (ORDER BY DESC LIMIT 1)
                string queryBusca = "SELECT cd_item, vl_subtotal FROM tb_itens_venda WHERE fk_venda = @idV AND fk_produto = @idP ORDER BY cd_item DESC LIMIT 1";
                
                // Prepara a busca
                using (var cmdBusca = new MySqlCommand(queryBusca, conexao))
                {
                    // Vincula a nota fiscal
                    cmdBusca.Parameters.AddWithValue("@idV", idVenda);
                    // Vincula o produto que o usuário quer remover
                    cmdBusca.Parameters.AddWithValue("@idP", idProduto);
                    
                    // Abre a conexão
                    conexao.Open();
                    
                    // Executa a leitura
                    using (var reader = cmdBusca.ExecuteReader())
                    {
                        // Se encontrar a linha de registro...
                        if (reader.Read())
                        {
                            // Pega o ID único da linha de registro
                            idItem = Convert.ToInt32(reader["cd_item"]);
                            // Pega o valor que custou aquele item para abater
                            subtotalAbatido = Convert.ToDouble(reader["vl_subtotal"]);
                        }
                    }
                }

                // Se o ID do item for maior que 0, significa que o produto foi encontrado na nota
                if (idItem > 0)
                {
                    // SQL: Apaga apenas a linha exata que encontramos
                    string queryDelete = "DELETE FROM tb_itens_venda WHERE cd_item = @idItem";
                    
                    // Prepara a exclusão
                    using (var cmdDelete = new MySqlCommand(queryDelete, conexao))
                    {
                        // Passa o ID único da linha
                        cmdDelete.Parameters.AddWithValue("@idItem", idItem);
                        // Executa o delete no banco
                        cmdDelete.ExecuteNonQuery();
                    }
                    // Avisa que a remoção deu certo
                    Console.WriteLine($"\n[-] Item removido da nota! Valor estornado: R$ {subtotalAbatido.ToString("N2", new CultureInfo("pt-BR"))}");
                }
                else
                {
                    // Se não achou, avisa que o produto não existe na nota
                    Console.WriteLine("\n[!] Produto não encontrado nesta nota para remover.");
                }
            }
            
            // Devolve o valor que foi abatido para atualizar o total geral
            return subtotalAbatido;
        }

        // Passo 3: Encerra a venda e salva o valor total
        public void FinalizarVenda(int idVenda, double valorTotal)
        {
            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // SQL: Atualiza a tabela mestre guardando o somatório total da compra
                string query = "UPDATE tb_venda SET vl_total_venda = @total WHERE cd_venda = @idV";
                
                // Prepara a atualização
                using (var comando = new MySqlCommand(query, conexao))
                {
                    // Passa o valor total acumulado no carrinho
                    comando.Parameters.AddWithValue("@total", valorTotal);
                    // Passa o ID da nota fiscal
                    comando.Parameters.AddWithValue("@idV", idVenda);
                    
                    // Abre a conexão
                    conexao.Open();
                    // Executa a gravação do total
                    comando.ExecuteNonQuery();
                }
            }

            // Calcula 10% do valor total para definir a comissão do vendedor
            double valorComissao = valorTotal * 0.10;
            
            // Configura a formatação para o padrão brasileiro de dinheiro
            CultureInfo culturaBR = new CultureInfo("pt-BR");

            // Exibe o resumo final da nota fiscal no console
            Console.WriteLine($"\n--- Venda #{idVenda} Finalizada com Sucesso! ---");
            // Mostra o total com vírgula para centavos e ponto para milhar
            Console.WriteLine($"Valor Total da Nota: R$ {valorTotal.ToString("N2", culturaBR)}");
            // Mostra o valor da comissão calculada
            Console.WriteLine($"Total de comissões a serem pagas: R$ {valorComissao.ToString("N2", culturaBR)}");
        }

        // ==========================================
        // SESSÃO DE RELATÓRIO
        // ==========================================

        // Método para exportar os dados cruzados para um arquivo Excel (CSV)
        public void GerarRelatorioCSV()
        {
            // Define o nome do arquivo físico que será criado
            string caminhoArquivo = "RelatorioGeral.csv";
            
            // Define a primeira linha do arquivo contendo os títulos das colunas
            string conteudoCSV = "ID Venda;Data;Cliente;Produto;Qtd;Subtotal;Total da Venda\n";

            // Inicia a conexão
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                // SQL: Usa múltiplos INNER JOIN para juntar as 4 tabelas e trazer todos os dados textuais de uma vez
                string query = @"
                    SELECT 
                        v.cd_venda, v.dt_venda, c.nm_cliente, p.nm_produto, 
                        i.qt_produto, i.vl_subtotal, v.vl_total_venda
                    FROM tb_venda v
                    INNER JOIN tb_cliente c ON v.fk_cliente = c.cd_cliente
                    INNER JOIN tb_itens_venda i ON v.cd_venda = i.fk_venda
                    INNER JOIN tb_produto p ON i.fk_produto = p.cd_produto
                    ORDER BY v.cd_venda ASC";
                
                // Prepara a consulta
                using (var comando = new MySqlCommand(query, conexao))
                {
                    // Abre a conexão
                    conexao.Open();
                    
                    // Executa a leitura geral
                    using (var reader = comando.ExecuteReader())
                    {
                        // Lê cada linha gerada pelo cruzamento do banco
                        while (reader.Read())
                        {
                            // Concatena as informações da linha atual com ponto e vírgula (;), padrão do Excel no Brasil, e pula linha (\n)
                            conteudoCSV += $"{reader["cd_venda"]};{reader["dt_venda"]};{reader["nm_cliente"]};{reader["nm_produto"]};{reader["qt_produto"]};{reader["vl_subtotal"]};{reader["vl_total_venda"]}\n";
                        }
                    }
                }
            }
            
            // Escreve toda a string montada diretamente no arquivo físico na pasta do projeto
            File.WriteAllText(caminhoArquivo, conteudoCSV);
            // Exibe o caminho completo de onde o arquivo foi salvo no computador
            Console.WriteLine($"\nRelatório gerado com sucesso em: {Path.GetFullPath(caminhoArquivo)}");
        }
    }
}
