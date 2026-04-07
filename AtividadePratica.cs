using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExercicioPraticoProjOO
{
    public sealed class ConfiguracaoGlobal
    {
        private static readonly Lazy<ConfiguracaoGlobal> _instancia = new Lazy<ConfiguracaoGlobal>(() => new ConfiguracaoGlobal());
        public static ConfiguracaoGlobal Instancia => _instancia.Value;

        public string NomeAplicacao { get; set; }
        public string ServidorEnvio { get; set; }
        public int MaxTentativas { get; set; }

        private ConfiguracaoGlobal()
        {
            NomeAplicacao = "Sistema de Notificações Corporativo";
            ServidorEnvio = "smtp.empresa.com";
            MaxTentativas = 3;
        }
    }

    public interface INotificacao
    {
        bool Enviar(string mensagem, string destinatario);
    }

    public class EmailNotificacao : INotificacao
    {
        public bool Enviar(string mensagem, string destinatario)
        {
            var config = ConfiguracaoGlobal.Instancia;
            Console.WriteLine($"[E-MAIL via {config.ServidorEnvio}] Para: {destinatario} | Mensagem: {mensagem}");
            return true;
        }

    }

    public class SmsNotificacao : INotificacao
    {
        public bool Enviar(string mensagem, string destinatario)
        {
            Console.WriteLine($"[SMS] Para: {destinatario} | Mensagem: {mensagem}");
            return true;
        }
    }

    public class PushNotificacao : INotificacao
    {
        public bool Enviar(string mensagem, string destinatario)
        {
            var config = ConfiguracaoGlobal.Instancia;
            Console.WriteLine($"[PUSH - App {config.NomeAplicacao}] Dispositivo: {destinatario} | Mensagem: {mensagem}");
            return true;
        }
    }

    public static class NotificacaoFactory
    {
        public static INotificacao CriarNotificacao(string tipo)
        {
            switch (tipo.ToUpper())
            {
                case "EMAIL":
                    return new EmailNotificacao();
                case "SMS":
                    return new SmsNotificacao();
                case "PUSH":
                    return new PushNotificacao();
                default:
                    throw new ArgumentException($"Tipo de notificação desconhecida: {tipo}");
            }
        }
    }

    internal class Program
    {

        static void Main(string[] args)
        {
            var notificacao = NotificacaoFactory.CriarNotificacao("EMAIL");
            notificacao.Enviar("Olá! Sua conta foi criada!", "drgiuseppicadura@gmail.com");
        }
    }

    public class SistemaNotificacaoTests
    {
        [Fact]
        public void TestSingleton_RetornaMesmaInstancia()
        {
            var config1 = ConfiguracaoGlobal.Instancia;
            var config2 = ConfiguracaoGlobal.Instancia;
            Assert.Same(config1, config2);

            config1.MaxTentativas = 5;
            Assert.Equal(5, config2.MaxTentativas);
        }

        [Fact]
        public void TestFactory_DeveCriarEmail()
        {
            var notificacao = NotificacaoFactory.CriarNotificacao("EMAIL");
            Assert.IsType<EmailNotificacao>(notificacao);

        }

        [Fact]
        public void TestFactory_DeveCriarSmsIgnorandoCase()
        {
            var notificacao = NotificacaoFactory.CriarNotificacao("sms");
            Assert.IsType<SmsNotificacao>(notificacao);
        }

        [Fact]
        public void TestFactory_DeveLancarExcecaoParaTipoNaoExistir()
        {
            Assert.Throws<ArgumentException>(() =>NotificacaoFactory.CriarNotificacao("WHATSAPP"));
        }

        [Fact]
        public void TestEnvioNotificacao_RetornaVerdadeiro()
        {
            var push = NotificacaoFactory.CriarNotificacao("PUSH");
            var resultado = push.Enviar("Você tem uma nova mensagem", "Device_XYZ123");
            Assert.True(resultado);
        }

    }
}
