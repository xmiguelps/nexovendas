using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexoVendas
{
    public class Produto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public double Preco { get; set; }

        public Produto(string nome, double preco)
        {
            Nome = nome;
            Preco = preco;
        }
    }
}