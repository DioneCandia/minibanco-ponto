using System;

namespace MiniBanco.ConsoleApp.Models
{
    public class ContaPoupanca : Conta
    {
        public double TaxaRendimentoMensal { get; private set; }
        public int DiaAniversario { get; private set; }

        public ContaPoupanca(string titular, int numero, decimal saldoInicial, double taxaRendimentoMensal, int diaAniversario)
            : base(titular, numero, saldoInicial)
        {
            if (taxaRendimentoMensal <= 0)
            {
                throw new ArgumentException("A taxa de rendimento mensal deve ser maior que zero.");
            }

            if (diaAniversario < 1 || diaAniversario > 31)
            {
                throw new ArgumentException("O dia de aniversário deve estar entre 1 e 31.");
            }

            this.TaxaRendimentoMensal = taxaRendimentoMensal;
            this.DiaAniversario = diaAniversario;
        }

        public override void AplicarFechamentoMensal()
        {
            if (this.Saldo > 0)
            {
                decimal rendimento = this.Saldo * (decimal)(this.TaxaRendimentoMensal / 100.0);
                rendimento = Math.Round(rendimento, 2);

                if (rendimento > 0)
                {
                    this.Depositar(rendimento, "Rendimento mensal");
                }
            }
        }

        public override string ObterNomeTipoConta()
        {
            return "Conta Poupança";
        }

        public override string ObterDescricaoAtributoEspecifico()
        {
            return $"Rendimento: {this.TaxaRendimentoMensal:F1}% ao mês | Aniversário: dia {this.DiaAniversario}";
        }
    }
}
