using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexoVendas
{
    public class Cliente
    {
        // Propriedade para o ID do cliente
        public int Id { get; set; }
        
        // Propriedade para o nome do cliente
        public string Nome { get; set; }
        
        // Propriedade para o email do cliente
        public string Email { get; set; }

        // Construtor para facilitar a criação do objeto
        public Cliente(string nome, string email)
        {
            // Atribui o valor recebido à propriedade Nome
            Nome = nome;
            // Atribui o valor recebido à propriedade Email
            Email = email;
        }
    }
}