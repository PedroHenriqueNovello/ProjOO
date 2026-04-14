using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExercicioPraticoFachada
{
    internal class Program
    {
        public class TV
        {
            public void Ligar()
            {
                Console.WriteLine("TV: ligada.");
            }
            public void Desligar()
            {
                Console.WriteLine("TV: desligada.");
            }
        }

        public class Projetor
        {
            public void Ligar()
            {
                Console.WriteLine("Projetor: ligado.");
            }
            public void Desligar()
            {
                Console.WriteLine("Projetor: desligado.");
            }
        }

        public class Receiver
        {
            public void Ligar()
            {
                Console.WriteLine("Receiver: ligado.");
            }
            public void DefinirVolume(int volume)
            {
                Console.WriteLine($"Receiver: volume definido para {volume}.");
            }
            public void Desligar()
            {
                Console.WriteLine("Receiver: desligado.");
            }
        }

        public class PlayerDeMidia
        {
            public void Ligar()
            {
                Console.WriteLine("Player de Mídia: ligado.");
            }
            public void Reproduzir(string midia)
            {
                Console.WriteLine($"Player de Mídia: reproduzindo {midia}.");
            }
            public void Parar()
            {
                Console.WriteLine("Player de Mídia: parado.");
            }
            public void Desligar()
            {
                Console.WriteLine("Player de Mídia: desligado.");
            }
        }

        public class SistemaDeSom
        {
            public void Ligar()
            {
                Console.WriteLine("Sistema de Som: Ligado.");
            }
            public void AtivarSurround()
            {
                Console.WriteLine("Sistema de Som: Surround ativado.");
            }
            public void AtivarEstereo()
            {
                Console.WriteLine("Sistema de Som: Estéreo ativado.");
            }
            public void Desligar()
            {
                Console.WriteLine("Sistema de Som: Desligado.");
            }
        }

        public class LuzAmbiente
        {
            public void Escurecer()
            {
                Console.WriteLine("Luz Ambiente: Modo cinema ativado.");
            }
            public void Clarear()
            {
                Console.WriteLine("Luz Ambiente: Modo normal ativado.");
            }
        }

        public class HomeTheaterFacade
        {
            private readonly TV _tv;
            private readonly Projetor _projetor;
            private readonly Receiver _receiver;
            private readonly PlayerDeMidia _player;
            private readonly SistemaDeSom _som;
            private readonly LuzAmbiente _luz;

            public HomeTheaterFacade (TV tv, Projetor projetor, Receiver receiver, PlayerDeMidia player, SistemaDeSom som, LuzAmbiente luz)
            {
                _tv = tv;
                _projetor = projetor;
                _receiver = receiver;
                _player = player;
                _som = som;
                _luz = luz;
            }

            public void AssistirFilme(string filme)
            {
                Console.WriteLine("Preparando para assistir um filme...");
                _luz.Escurecer();
                _tv.Ligar();
                _projetor.Ligar();
                _receiver.Ligar();
                _receiver.DefinirVolume(20);
                _som.Ligar();
                _som.AtivarSurround();
                _player.Ligar();
                _player.Reproduzir(filme);
            }

            public void OuvirMusica(string musica)
            {
                Console.WriteLine("Preparando para ouvir música...");
                _luz.Clarear();
                _receiver.Ligar();
                _receiver.DefinirVolume(15);
                _som.Ligar();
                _som.AtivarEstereo();
                _player.Ligar();
                _player.Reproduzir(musica);
            }

            public void DesligarTudo()
            {
                Console.WriteLine("Desligando Hometheater...");
                _player.Parar();
                _player.Desligar();
                _som.Desligar();
                _receiver.Desligar();
                _projetor.Desligar();
                _tv.Desligar();
                _luz.Clarear();
            }
        }

        static void Main(string[] args)
        {
            TV tv = new TV();
            Projetor projetor = new Projetor();
            Receiver receiver = new Receiver();
            PlayerDeMidia player = new PlayerDeMidia();
            SistemaDeSom som = new SistemaDeSom();
            LuzAmbiente luz = new LuzAmbiente();
            HomeTheaterFacade facade = new HomeTheaterFacade(tv, projetor, receiver, player, som, luz);
            facade.AssistirFilme("O Senhor dos Anéis: A Sociedade do Anel");
            Console.WriteLine();
            facade.OuvirMusica("Pink Floyd - Wish you were here");
            Console.WriteLine();
            facade.DesligarTudo();
        }
    }
}
