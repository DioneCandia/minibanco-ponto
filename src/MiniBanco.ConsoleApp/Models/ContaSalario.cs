using System;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    public class ContaSalario : Conta
    {
        public string Empregador { get; private set; }
        public int LimiteSaquesMensais { get; private set; }
        public int SaquesRealizadosMes { get; private set; }

        public ContaSalario(string titular, int numero, decimal saldoInicial, string empregador, int limiteSaquesMensais)
            : base(titular, numero, saldoInicial)
        {
            if (string.IsNullOrWhiteSpace(empregador))
            {
                throw new ArgumentException("O nome do empregador é obrigatório.");
            }

            if (limiteSaquesMensais < 0)
            {
                throw new ArgumentException("O limite de saques mensais não pode ser negativo.");
            }

            this.Empregador = empregador;
            this.LimiteSaquesMensais = limiteSaquesMensais;
            this.SaquesRealizadosMes = 0;
        }

        public override void Sacar(decimal valor)
        {
            if (valor <= 0)
            {
                throw new ArgumentException("O valor do saque deve ser maior que zero.");
            }

            decimal tarifaExcedente = 5.00m;
            bool excedeuLimite = (this.SaquesRealizadosMes + 1) > this.LimiteSaquesMensais;
            decimal custoTotalOperacao = excedeuLimite ? (valor + tarifaExcedente) : valor;

            if (custoTotalOperacao > this.Saldo)
            {
                throw new SaldoInsuficienteException(
                    $"Saldo insuficiente na Conta Salário {this.Numero} para cobrir o saque" +
                    (excedeuLimite ? " e a tarifa de saque excedente (R$ 5,00)." : "."));
            }

            this.Saldo -= valor;
            this.SaquesRealizadosMes++;
            this.RegistrarTransacao("Saque", "Saque", valor);

            if (excedeuLimite)
            {
                this.Saldo -= tarifaExcedente;
                this.RegistrarTransacao("Tarifa por saque excedente", "Tarifa", tarifaExcedente);
            }
        }

        public override void AplicarFechamentoMensal()
        {
            this.SaquesRealizadosMes = 0;
        }

        public override string ObterNomeTipoConta()
        {
            return "Conta Salário";
        }

        public override string ObterDescricaoAtributoEspecifico()
        {
            return $"Empregador: {this.Empregador} | Limite de Saques: {this.LimiteSaquesMensais}/mês (Realizados: {this.SaquesRealizadosMes})";
        }
    }
}
