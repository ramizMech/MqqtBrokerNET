﻿using MQTTnet;
using MQTTnet.Server;
using System;
using System.Linq;
using System.Text;

namespace MQTTProxyBroker
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello MQTT Broker!");
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionValidator(c =>
                {
                    Console.WriteLine($"{c.ClientId} connection validator for c.Endpoint: {c.Endpoint}");
                    c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                })
                .WithApplicationMessageInterceptor(context =>
                {
                    Console.WriteLine("WithApplicationMessageInterceptor block merging data");
                    var newData = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                    var oldData = context.ApplicationMessage.Payload;
                    var mergedData = newData.Concat(oldData).ToArray();
                    context.ApplicationMessage.Payload = mergedData;
                })
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884);

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build()).Wait();

            Console.WriteLine("Broker is Running");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
            mqttServer.StopAsync().Wait();
        }
    }
}
