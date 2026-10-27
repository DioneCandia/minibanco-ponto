using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    public abstract class Conta
    {
        public string Titular { get; private set; }
        public int Numero { get; private set; }
        public decimal Saldo { get; protected set; }
        public List<Transacao> Extrato { get; private set; }

        protected Conta(string titular, int numero, decimal saldoInicial)
        {
            if (string.IsNullOrWhiteSpace(titular))
            {
                throw new ArgumentException("O nome do titular é obrigatório.");
            }

            if (numero <= 0)
            {
                throw new ArgumentException("O número da conta deve ser maior que zero.");
            }

            if (saldoInicial < 0)
            {
                throw new ArgumentException("O saldo inicial não pode ser negativo.");
            }

            this.Titular = titular;
            this.Numero = numero;
            this.Saldo = saldoInicial;
            this.Extrato = new List<Transacao>();
        }

        public void RegistrarTransacao(string descricao, string tipo, decimal valor)
        {
            Transacao novaTransacao = new Transacao(descricao, tipo, valor);
            this.Extrato.Add(novaTransacao);
        }

        public void Depositar(decimal valor, string descricao)
        {
            if (valor <= 0)
            {
                throw new ArgumentException("O valor do depósito deve ser maior que zero.");
            }

            this.RegistrarTransacao(descricao, "Deposito", valor);
            this.Saldo += valor;
        }

        public virtual void Sacar(decimal valor)
        {
            if (valor <= 0)
            {
                throw new ArgumentException("O valor do saque deve ser maior que zero.");
            }

            if (valor > this.Saldo)
            {
                throw new SaldoInsuficienteException(
                    $"Saldo insuficiente na conta {this.Numero}. Saldo disponível: R$ {this.Saldo:F2}");
            }

            this.Saldo -= valor;
            this.RegistrarTransacao("Saque", "Saque", valor);
        }

        public virtual void AplicarFechamentoMensal()
        {
            // Implementado de forma específica pelas subclasses via override
        }

        public abstract string ObterNomeTipoConta();
        public abstract string ObterDescricaoAtributoEspecifico();
    }
}
