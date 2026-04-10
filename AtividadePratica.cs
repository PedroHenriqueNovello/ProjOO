using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExercicioPraticoProjOO
{
    /*Padrão singleton para configuração global*/

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

    /*Padrão Factory*/
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

    /*Padrão Adapter*/

    public class ServicoSmsExternò
    {
        public void DispararMensagemSms(string numeroTelefone, string textoMensagem)
        {
            Console.WriteLine($"[API EXTERNA SMS] Enviando '{textoMensagem}' para {numeroTelefone}");
        }
    }

    public class SmsExertnoAdapter : INotificacao
    {
        private readonly ServicoSmsExterno _servicoExterno;

        public SmsExternoAdapter(ServicoSmsExterno servicoExterno)
        {
            _servicoExterno = servicoExterno;
        }

        public bool Enviar(string mensagem, string destinatario)
        {
            _servicoExterno.DispararMensagemSms(destinatario, mensagem);
            return true;
        }
    }

    /*Padrão Proxy*/

    public class NotificacaoProxy : INotificacao
    {
        private readonly INotificacao _notificacaoReal;
        private int _tentativasRealizadas = 0;

        public NotificacaoProxy(INotificacao notificacaoReal)
        {
            _notificacaoReal = notificacaoReal;
        }

        public bool Enviar(string mensagem, string destinatario)
        {
            var config = ConfiguracaoGlobal.Instancia;

            if (string.IsNullOrWhiteSpace(destinatario))
            {
                Console.WriteLine("[PROXY - ERRO] Destinatário inválido.");
                return false;
            }

            Console.WriteLine($"[PROXY - LOG] Registrando tentativa {_tentativasRealizadas + 1} para {destinatario}...");
            bool sucesso = _notificacaoReal.Enviar(mensagem, destinatario);

            if (sucesso)
            {
                Console.WriteLine("[PROXY - LOG] Finalizado com sucesso.");
                _tentativasRealizadas++;
            }
            return sucesso;
        }
  
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- TESTANDO FACTORY E SINGLETON ---");
            var notificacao = NotificacaoFactory.CriarNotificacao("EMAIL");
            notificacao.Enviar("Olá! Sua conta foi criada!", "drgiuseppicadura@gmail.com");

            Console.WriteLine("\n--- TESTANDO ADAPTER ---");
            var apiAntiga = new ServicoSmsExterno();
            INotificacao smsAdaptado = new SmsExternoAdapter(apiAntiga);
            smsAdaptado.Enviar("Seu código é 123", "11999999999");

            Console.WriteLine("\n--- TESTANDO PROXY ---");
            var emailOriginal = NotificacaoFactory.CriarNotificacao("EMAIL");
            INotificacao emailComProxy = new NotificacaoProxy(emailOriginal);

            emailComProxy.Enviar("Recibo 1", "drgiuseppicadura@gmail.com"); // Vai passar
            emailComProxy.Enviar("Recibo 2", "drgiuseppicadura@gmail.com"); // Vai passar
            emailComProxy.Enviar("Recibo 3", "drgiuseppicadura@gmail.com"); // Vai passar
            emailComProxy.Enviar("Spam", "drgiuseppicadura@gmail.com");     // Vai bloquear (Limite configurado no Singleton)

            Console.ReadLine();
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

            
            config1.MaxTentativas = 3;
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
            Assert.Throws<ArgumentException>(() => NotificacaoFactory.CriarNotificacao("WHATSAPP"));
        }

        [Fact]
        public void TestAdapter_DeveAdaptarChamadaParaServicoExterno()
        {
            var servicoExterno = new ServicoSmsExterno();
            INotificacao adapter = new SmsExternoAdapter(servicoExterno);
            bool resultado = adapter.Enviar("Teste Adapter", "11999999999");

            Assert.True(resultado);
        }

        [Fact]
        public void TestProxy_DeveBloquearEnvioComDestinatarioVazio()
        {
            var notificacaoFalsa = NotificacaoFactory.CriarNotificacao("SMS");
            INotificacao proxy = new NotificacaoProxy(notificacaoFalsa);
            bool resultado = proxy.Enviar("Teste sem destino", "");

            Assert.False(resultado);
        }

        [Fact]
        public void TestProxy_DeveRespeitarLimiteDeTentativas()
        {
            var config = ConfiguracaoGlobal.Instancia;
            config.MaxTentativas = 2; 

            var notificacaoReal = NotificacaoFactory.CriarNotificacao("EMAIL");
            INotificacao proxy = new NotificacaoProxy(notificacaoReal);

            proxy.Enviar("Msg 1", "cliente@email.com"); 
            proxy.Enviar("Msg 2", "cliente@email.com"); 
            bool tentativa3 = proxy.Enviar("Msg 3", "cliente@email.com"); 

            Assert.False(tentativa3);

            // Retorna ao valor original para não afetar o resto da aplicação
            config.MaxTentativas = 3;
        }
    }
}
