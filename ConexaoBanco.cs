using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace NexoVendas
{
    public class ConexaoBanco
    {
        private readonly string stringConexao = "Server=localhost;Database=nexovendas;Uid=root;Pwd=root";

        public MySqlConnection ObterConexao()
        {
            return new MySqlConnection(stringConexao);
        }
    }
}