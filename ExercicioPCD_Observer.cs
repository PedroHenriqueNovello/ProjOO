using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExercícioDePCD_Observer
{
    internal class Program
    {
        public interface IObserver
        {
            void Atualizar (double tempAgua, double phAgua, double umidadeAr);
        }

        public interface ISubject
        {
            void Anexar(IObserver observador);
            void Desanexar(IObserver observador);
            void Notificar();
        }

        public class PCD : ISubject
        {
            private readonly List<IObserver> _observadores = new List<IObserver>();
            private double _tempAgua;
            private double _phAgua;
            private double _umidadeAr;
            public string NomePCD { get; private set; }

            public PCD(string nome)
            {
                NomePCD = nome;
            }

            public void Anexar(IObserver observador)
            {
                _observadores.Add(observador);
                Console.WriteLine($"[Sistema] Observador anexado à {NomePCD}.");
            }

            public void Desanexar(IObserver observador)
            {
                _observadores.Remove(observador);
                Console.WriteLine($"[Sistema] Observador desanexado da {NomePCD}.");
            }

            public void Notificar()
            {
                foreach (var observador in _observadores)
                {
                    observador.Atualizar(_tempAgua, _phAgua, _umidadeAr);
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

        public class Universidade : IObserver
        {
            public string NomeInstituicao { get; private set; }
            public string Cidade { get; private set; }

            public Universidade (string nomeInstituicao, string cidade)
            {
                NomeInstituicao = nomeInstituicao;
                Cidade = cidade;
            }

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

            Universidade uniPoa = new Universidade ("UFRGS", "Porto Alegre");
            Universidade uniSp = new Universidade ("USP", "São Paulo");
            Universidade uniRj = new Universidade ("UFRJ", "Rio de Janeiro");
            Universidade uniSjc = new Universidade("UNIFESP", "São José dos Campos");
            Universidade uniBsb = new Universidade ("UnB", "Brasília");

            Console.WriteLine("=== Configurando a rede de monitoramento ===\n");

            pcdRioNegro.Anexar(uniPoa);
            pcdRioNegro.Anexar(uniSp);

            pcdTapajos.Anexar(uniRj);
            pcdTapajos.Anexar(uniSjc);
            pcdTapajos.Anexar(uniBsb);

            pcdSolimoes.Anexar(uniSp);
            pcdSolimoes.Anexar(uniSjc);

            Console.WriteLine("\n === Iniciando coleta de dados ===\n");

            pcdRioNegro.SetNovasLeituras(temperatura: 28.5, ph: 6.8, umidade: 88.0);

            pcdTapajos.SetNovasLeituras(temperatura: 29.2, ph: 7.1, umidade: 82.5);

            pcdSolimoes.SetNovasLeituras(temperatura: 27.9, ph: 6.5, umidade: 91.0);

            Console.WriteLine("\n=== Atualizando rede de monitoramento ===\n");

            pcdTapajos.Desanexar(uniSjc);

            pcdTapajos.SetNovasLeituras(temperatura: 29.5, ph: 7.0, umidade: 80.0);
        }
    }
}
