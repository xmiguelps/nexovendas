using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexoVendas
{
    public class Produto
    {
        // Propriedade para o ID do produto
        public int Id { get; set; }
        
        // Propriedade para o nome do produto
        public string Nome { get; set; }
        
        // Propriedade para o preço do produto
        public double Preco { get; set; }

        // Construtor para inicializar o produto
        public Produto(string nome, double preco)
        {
            // Define o nome
            Nome = nome;
            // Define o preço
            Preco = preco;
        }
    }
}