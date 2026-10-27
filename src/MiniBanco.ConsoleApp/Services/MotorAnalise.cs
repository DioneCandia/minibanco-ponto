using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Models;

namespace MiniBanco.ConsoleApp.Services
{
    public class MotorAnalise
    {
        public void ProcessarContas(List<Conta> contas)
        {
            if (contas.Count == 0)
            {
                Console.WriteLine("\nNenhuma conta aberta até o momento.");
                return;
            }

            Console.WriteLine("\n====================================================================");
            Console.WriteLine("        MINIBANCO .NET - MOTOR DE GESTÃO DE CONTAS");
            Console.WriteLine("====================================================================\n");

            for (int i = 0; i < contas.Count; i++)
            {
                Conta conta = contas[i];
                Console.WriteLine($"[{i + 1}/{contas.Count}] PROCESSANDO CONTA");
                Console.WriteLine("--------------------------------------------------------------------");
                Console.WriteLine("> DADOS DA CONTA:");
                Console.WriteLine($"  - Tipo: {conta.ObterNomeTipoConta()}");
                Console.WriteLine($"  - Titular: {conta.Titular} | Conta: {conta.Numero}");
                Console.WriteLine($"  - Atributo Específico: {conta.ObterDescricaoAtributoEspecifico()}");
                Console.WriteLine($"  - Saldo Atual: R$ {conta.Saldo:F2}\n");

                Console.WriteLine($"> MOVIMENTAÇÕES REGISTRADAS ({conta.Extrato.Count} TRANSAÇÕES):");

                int pontuacaoObtida = 0;
                int pontuacaoMaximaPossivel = conta.Extrato.Count * 10;

                List<Transacao> itensCriticosTarifas = new List<Transacao>();
                List<Transacao> itensAtencaoSaques = new List<Transacao>();

                if (conta.Extrato.Count == 0)
                {
                    Console.WriteLine("  Nenhuma movimentação registrada nesta conta.");
                }
                else
                {
                    foreach (Transacao transacao in conta.Extrato)
                    {
                        int pontosTransacao = 0;
                        string marcador = "";

                        if (transacao.Tipo == "Deposito")
                        {
                            pontosTransacao = 10;
                            marcador = "[OK]";
                        }
                        else if (transacao.Tipo == "Saque")
                        {
                            pontosTransacao = 5;
                            marcador = "[ ! ]";
                            itensAtencaoSaques.Add(transacao);
                        }
                        else if (transacao.Tipo == "Tarifa")
                        {
                            pontosTransacao = 0;
                            marcador = "[ X ]";
                            itensCriticosTarifas.Add(transacao);
                        }

                        pontuacaoObtida += pontosTransacao;

                        string descricaoFormatada = transacao.Descricao.PadRight(28, '-');
                        Console.WriteLine($"  {marcador} {descricaoFormatada} {transacao.Tipo}: R$ {transacao.Valor:F2} ({pontosTransacao} pts)");
                    }
                }

                // RF05 - Cálculo do Índice de Saúde com cast (double) e proteção contra divisão por zero
                double percentualSaude = 0.0;
                if (pontuacaoMaximaPossivel > 0)
                {
                    percentualSaude = ((double)pontuacaoObtida / pontuacaoMaximaPossivel) * 100.0;
                }

                // RF06 - Classificação da Conta e Ação Corporativa
                string classificacaoFinal;
                string acaoCorporativa;

                if (percentualSaude >= 90.0)
                {
                    classificacaoFinal = "CONTA SAUDÁVEL";
                    acaoCorporativa = "Oferecer investimentos e avaliar aumento de limite.";
                }
                else if (percentualSaude >= 60.0)
                {
                    classificacaoFinal = "CONTA EM ATENÇÃO";
                    acaoCorporativa = "Contato da gerência para orientação financeira e revisão de tarifas.";
                }
                else
                {
                    classificacaoFinal = "CONTA EM RISCO";
                    acaoCorporativa = "Bloquear aumento de limite e abrir análise de crédito.";
                }

                Console.WriteLine("\n> RESUMO DA PONTUAÇÃO:");
                Console.WriteLine($"  - Pontuação Atingida: {pontuacaoObtida} de {pontuacaoMaximaPossivel} pontos possíveis");
                Console.WriteLine($"  - Índice de Saúde: {percentualSaude:F1}%");
                Console.WriteLine($"  - Classificação Final: [ {classificacaoFinal} ]");
                Console.WriteLine($"  - Ação Corporativa Padrão: {acaoCorporativa}\n");

                // RF07 e RF08 - Relatório de Pendências e Recomendações para a Gerência
                Console.WriteLine("> RELATÓRIO DE PENDÊNCIAS E RECOMENDAÇÕES DA GERÊNCIA:");

                bool possuiPendencias = (itensCriticosTarifas.Count > 0) || (itensAtencaoSaques.Count > 0);

                if (!possuiPendencias)
                {
                    Console.WriteLine("  🟢 Nenhuma pendência identificada. Oferecer produtos de investimento ao cliente.");
                }
                else
                {
                    if (itensCriticosTarifas.Count > 0)
                    {
                        Console.WriteLine("  🔴 ITENS CRÍTICOS (TARIFAS):");
                        foreach (Transacao tarifa in itensCriticosTarifas)
                        {
                            Console.WriteLine($"    - {tarifa.Descricao} (R$ {tarifa.Valor:F2}): Avaliar isenção ou migração para pacote de serviços.");
                        }
                        Console.WriteLine();
                    }

                    if (itensAtencaoSaques.Count > 0)
                    {
                        Console.WriteLine("  🟡 ITENS DE ATENÇÃO (SAQUES):");
                        foreach (Transacao saque in itensAtencaoSaques)
                        {
                            if (conta.Saldo < 0)
                            {
                                Console.WriteLine($"    - Saque de R$ {saque.Valor:F2}: Conta usando limite do cheque especial. Entrar em contato com o cliente.");
                            }
                            else
                            {
                                Console.WriteLine($"    - Saque de R$ {saque.Valor:F2}: Retirada registrada. Acompanhar fluxo de caixa do cliente.");
                            }
                        }
                    }
                }

                Console.WriteLine("--------------------------------------------------------------------\n");
            }

            Console.WriteLine("====================================================================");
            Console.WriteLine("        FIM DO PROCESSAMENTO DE CONTAS");
            Console.WriteLine("====================================================================");
        }
    }
}
