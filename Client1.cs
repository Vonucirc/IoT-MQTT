using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
namespace MQtt1
{
    class Client1 
    {
        static async Task Main(string[] args) //главная функция, в которой содержится исполняемый код
        {
            Console.WriteLine("Client1"); //вывод сообщения в консоль
            var factory = new MqttFactory(); //создаем экземляр класса чтобы воспользоваться методом этого класса (CreateMqttClient)

            IMqttClient Client = factory.CreateMqttClient(); //создаем клиента

            var options = new MqttClientOptionsBuilder() //настраеваем подключение к серверу (брокеру)
                .WithClientId(Guid.NewGuid().ToString()) //генерируем уникальный id
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCleanSession()
                .Build();

            Client.ApplicationMessageReceivedAsync += e => //функция, которая выполняется в момент получения сообщения
            {
                Console.WriteLine($"Received answer - {e.ApplicationMessage.ConvertPayloadToString()}"); //выводим содержания сообщения на экран
                
                return Task.CompletedTask;
            };

            await Client.ConnectAsync(options); //подключаемся к серверу при помощи настроек, описанных ранее

            Console.WriteLine("The MQTT client is connected."); //выводим сообщение о том, что мы подключились к серверу

            var TopicFilter = new MqttTopicFilterBuilder() //настраиваем те темы, на которые будем подписываться
                .WithTopic("Answer")
                .Build();

            await Client.SubscribeAsync(TopicFilter); //подписываемся на темы

            var Message = new MqttApplicationMessageBuilder() //задаем настройки сообщений, которые будем отправлять
               .WithTopic("Message") //тема сообщения
               .WithPayload("Hello my friend.") //содержание сообщения
               .Build();

            Console.WriteLine("Enter quantity of messages"); //выводим сообщение о том, какое кол-во сообщений будем отправлять 

            int n; // переменная для хранения кол-ва сообщений

            n = Convert.ToInt32(Console.ReadLine()); //считываем введенное кол-во

            for (int i=1; i<=n; i++) //цикл отправлет столько сообщений, сколько ввел пользователь
            {
                await Client.PublishAsync(Message); // отправляем сообщение
            }

            Client.DisconnectedAsync += e => //функция, которая выполняется вовремя отключения от сервера
            {
                Console.WriteLine("The MQTT client is disconnected."); //выводим сообщение, что мы отклчились от сервера
                Console.WriteLine("Press enter to exit."); //выводим сообщение, чтобы выйти нажмите enter 
                return Task.CompletedTask;
            };

            Console.WriteLine("Press enter to disconnect."); //выводим сообщение, чтобы отключится от сервера нажмите enter
            
            Console.ReadLine(); //считываем нажатую клавишу для отключения от сервера
            
            await Client.DisconnectAsync(); //отключаемся от сервера
            
            Console.ReadLine(); // считываем нажатую клавишу для закрытия приложения 
        }
    }
}
