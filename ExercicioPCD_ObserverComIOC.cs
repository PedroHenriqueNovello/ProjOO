using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExercicioPDC_ObserverComIOC
{
    internal class Program
    {
        public class PCD
        {
            /* Substitui a dependência de uma interface específica por um Delegate (Action), tornando o código mais flexível, uma vez que 
            a classe PCD passa a aceitar qualquer método que tenha a assinatura de três parâmetros double */

            private readonly List<Action<double, double, double>> _callbacks = new List<Action<double, double, double>>(); //Desacoplamento

            private double _tempAgua;
            private double _phAgua;
            private double _umidadeAr;
            public string NomePCD { get; private set; }

            public PCD(string nome)
            {
                NomePCD = nome;
            }

            public void Anexar(Action<double, double, double> callback)
            {
                _callbacks.Add(callback);
                Console.WriteLine($"[Sistema] Callback anexado à {NomePCD}.");
            }

            public void Desanexar(Action<double, double, double> callback)
            {
                _callbacks.Remove(callback);
                Console.WriteLine($"[Sistema] Callback desanexado da {NomePCD}.");
            }

            public void Notificar()
            {
                // Princípio de Hollywood em ação. A universidade
                // não controla quanddo o seu próprio método Atualizar é executado
                // A PCD assume esse controle

                foreach (var callback in _callbacks)
                {
                    callback(_tempAgua, _phAgua, _umidadeAr);
                }
            }

            public void SetNovasLeituras(double temperatura, double ph, double umidade)
            {
                Console.WriteLine($"\n --- {NomePCD} registrou novas leituras ---");
                _tempAgua = temperatura;
                _phAgua = ph;
                _umidadeAr = umidade;
                Notificar();
            }
        }

        public class Universidade
        {
            public string NomeInstituicao { get; private set; }
            public string Cidade { get; private set; }

            public Universidade(string nomeInstituicao, string cidade)
            {
                NomeInstituicao = nomeInstituicao;
                Cidade = cidade;
            }

            /* Função de callback
             O método Atualizar tem a assinatura exigida pela PCD
            A universidade "entrega" este bloco de código para a PCD executar depois*/
            public void Atualizar(double tempAgua, double phAgua, double umidadeAr)
            {
                Console.WriteLine($"[{NomeInstituicao} - {Cidade}] Dados recebidos -> Temp: {tempAgua}°C | pH: {phAgua} | Umidade: {umidadeAr}%");
            }
        }

        static void Main(string[] args)
        {
            PCD pcdRioNegro = new PCD("PCD-01 Rio Negro");
            PCD pcdTapajos = new PCD("PCD-02 Tapajós");
            PCD pcdSolimoes = new PCD("PCD-03 Rio Solimões");

            Universidade uniPoa = new Universidade("UFRGS", "Porto Alegre");
            Universidade uniSp = new Universidade("USP", "São Paulo");
            Universidade uniRj = new Universidade("UFRJ", "Rio de Janeiro");
            Universidade uniSjc = new Universidade("UNIFESP", "São José dos Campos");
            Universidade uniBsb = new Universidade("UnB", "Brasília");

            Console.WriteLine("=== Configurando a rede de monitoramento ===\n");

            // registro dos callbacks; é onde a inversão acontece na práica
            // A classe Universidade não chama a PCD, a main injeta o comportamento
            //da universidade para dentro da PCD
            pcdRioNegro.Anexar(uniPoa.Atualizar);
            pcdRioNegro.Anexar(uniSp.Atualizar);

            pcdTapajos.Anexar(uniRj.Atualizar);
            pcdTapajos.Anexar(uniSjc.Atualizar);
            pcdTapajos.Anexar(uniBsb.Atualizar);

            pcdSolimoes.Anexar(uniSp.Atualizar);
            pcdSolimoes.Anexar(uniSjc.Atualizar);

            Console.WriteLine("\n === Iniciando coleta de dados ===\n");

            pcdRioNegro.SetNovasLeituras(temperatura: 28.5, ph: 6.8, umidade: 88.0);
            pcdTapajos.SetNovasLeituras(temperatura: 29.2, ph: 7.1, umidade: 82.5);
            pcdSolimoes.SetNovasLeituras(temperatura: 27.9, ph: 6.5, umidade: 91.0);

            Console.WriteLine("\n=== Atualizando rede de monitoramento ===\n");

            // Para desanexar, passamos exatamente a mesma referência do método.
            pcdTapajos.Desanexar(uniSjc.Atualizar);

            pcdTapajos.SetNovasLeituras(temperatura: 29.5, ph: 7.0, umidade: 80.0);
        }
    }
}