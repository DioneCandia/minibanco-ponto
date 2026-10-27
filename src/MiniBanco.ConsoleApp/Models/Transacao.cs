using System;

namespace MiniBanco.ConsoleApp.Models
{
    public class Transacao
    {
        public string Descricao { get; private set; }
        public string Tipo { get; private set; }
        public decimal Valor { get; private set; }

        public Transacao(string descricao, string tipo, decimal valor)
        {
            if (string.IsNullOrWhiteSpace(descricao))
            {
                throw new ArgumentException("A descrição da transação não pode ser vazia.");
            }

            if (tipo != "Deposito" && tipo != "Saque" && tipo != "Tarifa")
            {
                throw new ArgumentException("Tipo de transação inválido. Use apenas 'Deposito', 'Saque' ou 'Tarifa'.");
            }

            if (valor <= 0)
            {
                throw new ArgumentException("O valor da transação deve ser maior que zero.");
            }

            this.Descricao = descricao;
            this.Tipo = tipo;
            this.Valor = valor;
        }
    }
}
