using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO; //Necessario para gerar arquivos CSV
using MySql.Data.MySqlClient;
using System.Globalization;

namespace NexoVendas
{
    public class SistemaDAO
    {
        private readonly ConexaoBanco _conexaoBanco;

        public SistemaDAO()
        {
            _conexaoBanco = new ConexaoBanco();
        }

        public void CadastrarCliente(Cliente cliente)
        {
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                string query = "INSERT INTO tb_cliente (nm_cliente, ds_email) VALUES (@nome, @email)";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    comando.Parameters.AddWithValue("@nome", cliente.Nome);
                    comando.Parameters.AddWithValue("@email", cliente.Email);
                    conexao.Open();
                    comando.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Cliente cadastrado com sucesso!");
        }

        public void ConsultarCliente()
        {
            using(var conexao = _conexaoBanco.ObterConexao())
            {
                string query = "SELECT id_cliente, nm_cliente, ds_email FROM tb_cliente";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    conexao.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        Console.WriteLine("\n--- LISTA DE CLIENTES ---");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["id_cliente"]} | Nome: {reader["nm_cliente"]} | Email: {reader["ds_email"]}");
                        }
                    }
                }
            }
        }

        public void AlterarCliente(int id, string novoNome, string novoEmail)
        {
            using (var conexao = _conexaoBanco.ObterConexao())
            {
                string query = "UPDATE tb_cliente SET nm_cliente = @nome, ds_email = @email WHERE cd_cliente = @id";
                using (var comando = new MySqlCommand(query, conexao))
                {
                    
                }
            }
        }
    }
}