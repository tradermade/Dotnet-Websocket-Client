using System;
using System.Threading;
using Websocket.Client;
using Newtonsoft.Json;

public class quote
        {
            public string symbol { get; set; }
            public long ts { get; set; }
            public double bid { get; set; }
            public double ask { get; set; }
            public double mid { get; set; }
        }


namespace csWebsocket
{
    class Program
    {
        private string streaming_API_Key = "your_api_key";


        static void Main(string[] args)
        {
            Program prg = new Program();
            prg.Initialize();
        }


        private void Initialize()
        {
            Console.CursorVisible = false;


            try
            {
                var exitEvent = new ManualResetEvent(false);
                var url = new Uri("wss://marketdata.tradermade.com/feedadv");
                


                using (var client = new WebsocketClient(url))
                {
                    client.ReconnectTimeout = TimeSpan.FromSeconds(30);


                    client.ReconnectionHappened.Subscribe(info =>
                    {
                        Console.WriteLine("Reconnection happened, type: " + info.Type);
                    });


                    client.MessageReceived.Subscribe(msg =>
                    {
                        Console.WriteLine("Message received: " + msg);


                        if (msg.ToString().ToLower() == "connected")
                        {
                            string data =  "{\"userKey\":\"" + streaming_API_Key + "\", \"symbol\":\"EURUSD,GBPUSD,USDJPY\"}";
                            client.Send(data);
                        }
                        else {
                        string data = msg.Text;
                        var result = JsonConvert.DeserializeObject<quote>(data);
                        Console.WriteLine(result.symbol + " " + result.bid + " " + result.ask);
                        }
                       
                        
                    });


                    client.Start();


                    //Task.Run(() => client.Send("{ message }"));


                    exitEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
            }


            Console.ReadKey();
        }


    }
}
