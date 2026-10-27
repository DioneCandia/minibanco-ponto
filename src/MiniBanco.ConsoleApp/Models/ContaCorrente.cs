using System;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    public class ContaCorrente : Conta
    {
        public decimal LimiteChequeEspecial { get; private set; }

        public ContaCorrente(string titular, int numero, decimal saldoInicial, decimal limiteChequeEspecial)
            : base(titular, numero, saldoInicial)
        {
            if (limiteChequeEspecial < 0)
            {
                throw new ArgumentException("O limite do cheque especial não pode ser negativo.");
            }

            this.LimiteChequeEspecial = limiteChequeEspecial;
        }

        public override void Sacar(decimal valor)
        {
            if (valor <= 0)
            {
                throw new ArgumentException("O valor do saque deve ser maior que zero.");
            }

            decimal saldoDisponivelComLimite = this.Saldo + this.LimiteChequeEspecial;

            if (valor > saldoDisponivelComLimite)
            {
                throw new SaldoInsuficienteException(
                    $"Limite insuficiente na Conta Corrente {this.Numero}. Disponível (Saldo + Limite): R$ {saldoDisponivelComLimite:F2}");
            }

            this.Saldo -= valor;
            this.RegistrarTransacao("Saque", "Saque", valor);
        }

        public override void AplicarFechamentoMensal()
        {
            decimal tarifaManutencao = 10.00m;
            this.Saldo -= tarifaManutencao;
            this.RegistrarTransacao("Tarifa de manutenção", "Tarifa", tarifaManutencao);
        }

        public override string ObterNomeTipoConta()
        {
            return "Conta Corrente";
        }

        public override string ObterDescricaoAtributoEspecifico()
        {
            return $"Limite Cheque Especial: R$ {this.LimiteChequeEspecial:F2}";
        }
    }
}
