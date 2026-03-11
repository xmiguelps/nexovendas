using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Importa a biblioteca do MySQL
using MySql.Data.MySqlClient;

namespace NexoVendas
{
    public class ConexaoBanco
    {
        // String de conexão (Atenção: coloquem a senha do MySQL de vocês aqui)
        private readonly string stringConexao = "Server=localhost;Database=nexovendas;Uid=root;Pwd=root;";

        // Método que devolve a conexão pronta
        public MySqlConnection ObterConexao()
        {
            // Retorna um novo objeto de conexão configurado
            return new MySqlConnection(stringConexao);
        }
    }
}