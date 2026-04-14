using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AtividadePraticaDecorator
{
    internal class Program
    {
        public interface IBebida
        {
            string GetDescricao();
            double CalcularCusto();
        }

        public class CafeExpresso : IBebida
        {
            public string GetDescricao()
            {
                return "Café Expresso";
            }
            public double CalcularCusto()
            {
                return 2.0;
            }
        }

        public class Capuccino : IBebida
        {
            public string GetDescricao()
            {
                return "Capuccino";
            }
            public double CalcularCusto()
            {
                return 7.50;
            }
        }

        public class Cha : IBebida
        {
            public string GetDescricao()
            {
                return "Chá Quente";
            }
            public double CalcularCusto()
            {
                return 4.0;
            }
        }

        public abstract class BebidaDecorator : IBebida
        {
            protected IBebida _bebida;

            public BebidaDecorator (IBebida bebida)
            {
                _bebida = bebida;
            }

            public virtual string GetDescricao()
            {
                return _bebida.GetDescricao();
            }

            public virtual double CalcularCusto()
            {
                return _bebida.CalcularCusto();
            }
        }

        public class Leite : BebidaDecorator
        {
            public Leite(IBebida bebida) : base(bebida) { }

            public override string GetDescricao()
            {
                return _bebida.GetDescricao() + ", Leite";

            }
            public override double CalcularCusto()
            {
                return _bebida.CalcularCusto() + 1.50;
            }
               
        }

        public class Chantilly : BebidaDecorator
        {
            public Chantilly(IBebida bebida) : base(bebida) { }

            public override string GetDescricao()
            {
                return _bebida.GetDescricao() + ", Chantilly";
            }
            public override double CalcularCusto()
            {
                return _bebida.CalcularCusto() + 2.50;
            }
        }

        public class Canela : BebidaDecorator
        {
            public Canela (IBebida bebida) : base(bebida) { }

            public override string GetDescricao()
            {
                return _bebida.GetDescricao();
            }

            public override double CalcularCusto()
            {
                return _bebida.CalcularCusto() + 0.50;
            }
        }

        public class CaldaChocolate : BebidaDecorator
        {
            public CaldaChocolate(IBebida bebida) : base(bebida) { }
            public override string GetDescricao()
            {
                return _bebida.GetDescricao() + ", Calda de Chocolate";
            }
            public override double CalcularCusto()
            {
                return _bebida.CalcularCusto() + 2.00;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("--- Pedido 1 ---");
            IBebida pedido1 = new CafeExpresso();
            Console.WriteLine($"{pedido1.GetDescricao()} | R$ {pedido1.CalcularCusto():F2}");

            Console.WriteLine("\n--- Pedido 2 ---");
            IBebida pedido2 = new Capuccino();
            pedido2 = new Chantilly(pedido2);
            pedido2 = new CaldaChocolate(pedido2);
            Console.WriteLine($"{pedido2.GetDescricao()} | R$ {pedido2.CalcularCusto():F2}");

            Console.WriteLine("\n--- Pedido 3 ---");
            IBebida pedido3 = new Cha();
            pedido3 = new Leite(pedido3);
            pedido3 = new Canela(pedido3);
            pedido3 = new Canela(pedido3);
            Console.WriteLine($"{pedido3.GetDescricao()} | R$ {pedido3.CalcularCusto():F2}"); 
        }
    }
}
