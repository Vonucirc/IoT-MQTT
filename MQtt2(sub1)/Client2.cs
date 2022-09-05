using MQTTnet;
using MQTTnet.Client;
using System;
using System.Threading.Tasks;

namespace MQtt2_sub1_
{
    class Client2
    {
        static async Task Main(string[] args) //главная функция, в которой содержится исполняемый код
        {
            Console.WriteLine("Client2"); //вывод сообщения в консоль
            var factory = new MqttFactory(); //создаем экземляр класса чтобы воспользоваться методом этого класса (CreateMqttClient)

            IMqttClient Client = factory.CreateMqttClient(); //создаем клиента

            var options = new MqttClientOptionsBuilder() //настраеваем подключение к серверу (брокеру)
                .WithClientId(Guid.NewGuid().ToString()) //генерируем уникальный id
                .WithTcpServer("test.mosquitto.org", 1883)
                .WithCleanSession()
                .Build();


            Client.ApplicationMessageReceivedAsync += e => //функция, которая выполняется в момент получения сообщения
            {
                Console.WriteLine($"Received message - {e.ApplicationMessage.ConvertPayloadToString()}"); //выводим содержания сообщения на экран
                var Message = new MqttApplicationMessageBuilder()  //задаем настройки сообщений, которые будем отправлять
               .WithTopic("Answer")  //тема сообщения
               .WithPayload($"I received your message ->{e.ApplicationMessage.ConvertPayloadToString()}") //содержание сообщения
               .Build();
                Client.PublishAsync(Message); // отправляем сообщение
                return Task.CompletedTask;
            };

            await Client.ConnectAsync(options); //подключаемся к серверу при помощи настроек, описанных ранее

            Console.WriteLine("The MQTT client is connected."); //выводим сообщение о том, что мы подключились к серверу

            var TopicFilter = new MqttTopicFilterBuilder() //настраиваем те темы, на которые будем подписываться
                    .WithTopic("Message")
                    .Build();

            await Client.SubscribeAsync(TopicFilter); //подписываемся на темы

            Client.DisconnectedAsync += e =>  //функция, которая выполняется вовремя отключения от сервера
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
