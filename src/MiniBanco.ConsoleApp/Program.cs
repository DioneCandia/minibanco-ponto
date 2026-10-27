using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Exceptions;
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Services;

namespace MiniBanco.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<Conta> contas = new List<Conta>();
            MotorAnalise motorAnalise = new MotorAnalise();
            bool executando = true;

            while (executando)
            {
                Console.WriteLine("\n====================================================");
                Console.WriteLine("      MINIBANCO .NET - MENU PRINCIPAL");
                Console.WriteLine("====================================================");
                Console.WriteLine("1 - Abrir Nova Conta");
                Console.WriteLine("2 - Registrar Movimentação (Depósito / Saque)");
                Console.WriteLine("3 - Aplicar Fechamento Mensal");
                Console.WriteLine("4 - Exibir Relatório das Contas");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("====================================================");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine() ?? "";

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            AbrirNovaConta(contas);
                            break;

                        case "2":
                            RegistrarMovimentacao(contas);
                            break;

                        case "3":
                            AplicarFechamentoMensalEmTodas(contas);
                            break;

                        case "4":
                            motorAnalise.ProcessarContas(contas);
                            break;

                        case "0":
                            executando = false;
                            Console.WriteLine("\nEncerrando o MiniBanco.ConsoleApp. Até logo!");
                            break;

                        default:
                            Console.WriteLine("\n[AVISO] Opção inválida. Digite um número entre 0 e 4.");
                            break;
                    }
                }
                catch (SaldoInsuficienteException ex)
                {
                    Console.WriteLine($"\n[ERRO DE SALDO] {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"\n[ERRO DE VALIDAÇÃO] {ex.Message}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("\n[ERRO DE FORMATO] Entrada inválida! Digite apenas números nos campos numéricos.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[ERRO INESPERADO] {ex.Message}");
                }
            }
        }

        private static void AbrirNovaConta(List<Conta> contas)
        {
            Console.WriteLine("\n--- ABERTURA DE NOVA CONTA ---");
            Console.WriteLine("Tipos disponíveis:");
            Console.WriteLine("1 - Conta Corrente");
            Console.WriteLine("2 - Conta Poupança");
            Console.WriteLine("3 - Conta Salário");
            Console.Write("Selecione o tipo da conta (1, 2 ou 3): ");
            string tipoOpcao = Console.ReadLine() ?? "";

            if (tipoOpcao != "1" && tipoOpcao != "2" && tipoOpcao != "3")
            {
                throw new ArgumentException("Tipo de conta inválido. Escolha 1, 2 ou 3.");
            }

            Console.Write("Informe o nome do Titular: ");
            string titular = Console.ReadLine() ?? "";

            Console.Write("Informe o Número da Conta (inteiro): ");
            int numero = int.Parse(Console.ReadLine() ?? "0");

            // Verifica com foreach tradicional se já existe conta com o mesmo número
            Conta? contaExistente = BuscarContaPorNumero(contas, numero);
            if (contaExistente != null)
            {
                throw new ArgumentException($"Já existe uma conta cadastrada com o número {numero}.");
            }

            Console.Write("Informe o Saldo Inicial (R$): ");
            decimal saldoInicial = decimal.Parse(Console.ReadLine() ?? "0");

            Conta novaConta;

            if (tipoOpcao == "1")
            {
                Console.Write("Informe o Limite do Cheque Especial (R$): ");
                decimal limite = decimal.Parse(Console.ReadLine() ?? "0");
                novaConta = new ContaCorrente(titular, numero, saldoInicial, limite);
            }
            else if (tipoOpcao == "2")
            {
                Console.Write("Informe a Taxa de Rendimento Mensal (em %, ex: 0,5): ");
                double taxa = double.Parse(Console.ReadLine() ?? "0");

                Console.Write("Informe o Dia de Aniversário da conta (1 a 31): ");
                int diaAniversario = int.Parse(Console.ReadLine() ?? "1");

                novaConta = new ContaPoupanca(titular, numero, saldoInicial, taxa, diaAniversario);
            }
            else
            {
                Console.Write("Informe o nome do Empregador: ");
                string empregador = Console.ReadLine() ?? "";

                Console.Write("Informe o Limite de Saques Mensais gratuitos: ");
                int limiteSaques = int.Parse(Console.ReadLine() ?? "0");

                novaConta = new ContaSalario(titular, numero, saldoInicial, empregador, limiteSaques);
            }

            contas.Add(novaConta);
            Console.WriteLine($"\n[SUCESSO] {novaConta.ObterNomeTipoConta()} nº {novaConta.Numero} aberta para {novaConta.Titular}!");
        }

        private static void RegistrarMovimentacao(List<Conta> contas)
        {
            Console.WriteLine("\n--- REGISTRAR MOVIMENTAÇÃO ---");
            Console.Write("Informe o Número da Conta: ");
            int numero = int.Parse(Console.ReadLine() ?? "0");

            Conta? conta = BuscarContaPorNumero(contas, numero);
            if (conta == null)
            {
                Console.WriteLine($"\n[AVISO] Conta nº {numero} não encontrada.");
                return;
            }

            Console.WriteLine($"Conta selecionada: {conta.Titular} ({conta.ObterNomeTipoConta()}) | Saldo atual: R$ {conta.Saldo:F2}");
            Console.WriteLine("Operações:");
            Console.WriteLine("1 - Depósito");
            Console.WriteLine("2 - Saque");
            Console.Write("Escolha a operação (1 ou 2): ");
            string operacao = Console.ReadLine() ?? "";

            if (operacao == "1")
            {
                Console.Write("Informe a descrição do depósito (ex: Salário, Transferência recebida): ");
                string descricao = Console.ReadLine() ?? "";

                Console.Write("Informe o valor do depósito (R$): ");
                decimal valor = decimal.Parse(Console.ReadLine() ?? "0");

                conta.Depositar(valor, descricao);
                Console.WriteLine($"\n[SUCESSO] Depósito realizado! Novo saldo: R$ {conta.Saldo:F2}");
            }
            else if (operacao == "2")
            {
                Console.Write("Informe o valor do saque (R$): ");
                decimal valor = decimal.Parse(Console.ReadLine() ?? "0");

                conta.Sacar(valor);
                Console.WriteLine($"\n[SUCESSO] Saque realizado! Novo saldo: R$ {conta.Saldo:F2}");
            }
            else
            {
                throw new ArgumentException("Operação inválida. Escolha 1 para Depósito ou 2 para Saque.");
            }
        }

        private static void AplicarFechamentoMensalEmTodas(List<Conta> contas)
        {
            if (contas.Count == 0)
            {
                Console.WriteLine("\n[AVISO] Nenhuma conta aberta até o momento para aplicar o fechamento.");
                return;
            }

            // Polimorfismo: chama AplicarFechamentoMensal() sem verificar o tipo concreto da conta
            foreach (Conta conta in contas)
            {
                conta.AplicarFechamentoMensal();
            }

            Console.WriteLine($"\n[SUCESSO] Fechamento mensal aplicado polimorficamente em {contas.Count} conta(s)!");
        }

        private static Conta? BuscarContaPorNumero(List<Conta> contas, int numero)
        {
            // RF09 - Busca com laço tradicional (sem LINQ)
            foreach (Conta conta in contas)
            {
                if (conta.Numero == numero)
                {
                    return conta;
                }
            }

            return null;
        }
    }
}

