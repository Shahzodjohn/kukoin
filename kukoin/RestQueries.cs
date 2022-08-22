using Kucoin.Net.Clients;
using Kucoin.Net.Enums;
using Kucoin.Net.Interfaces.Clients;
using Kucoin.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace kukoin
{
    public class RestQueries
    {
        private BufferBlock<string> _subscribedbufferBlock { get; set; }
        private BufferBlock<string> _unsubscribedbufferBlock { get; set; }

        KucoinClient _client = new KucoinClient(new KucoinClientOptions
        {
            ApiCredentials = new KucoinApiCredentials("63036ec400c8fa0001d81986", "11867e71-9dcf-4b97-806e-e9a20c28424f", "Shaha919723377"),
            LogLevel = LogLevel.Trace,
        });

        public RestQueries()
        {
            _client = new KucoinClient();
            _subscribedbufferBlock = new BufferBlock<string>();
            _unsubscribedbufferBlock = new BufferBlock<string>();
        }

        public async Task GetKlinesAsync()
        {
            await KlinesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await KlinesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }
        private async Task KlinesAsync()
        {
            var kline = await _client.SpotApi.ExchangeData.GetKlinesAsync("btc-usdt".ToUpper(), KlineInterval.OneMinute);
            string text;
            foreach (var line in kline.Data)
            {
                text = "\nOpenPrice = " + line.OpenPrice +
                "\nHighPrice = " + line.HighPrice +
                "\nClosePrice = " + line.ClosePrice +
                "\nLowPrice = " + line.LowPrice +
                  "\nQuoteVolume = " + line.QuoteVolume +
                  "\nVolume = " + line.Volume;

                await _subscribedbufferBlock.SendAsync(text);
            }
            if (!kline.Success)
                await _unsubscribedbufferBlock.SendAsync("Error while taking values!");
        }

        public async Task PlaceOrderAsync()
        {
            await CreateAnOrder();
            var trueSubscribe = new ActionBlock<string>(value =>
            {       
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await CreateAnOrder();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }
        private async Task CreateAnOrder()
        {
            var orderData = await _client.SpotApi.Trading.PlaceOrderAsync(
                "BTC-USDT",
                OrderSide.Buy,
                NewOrderType.Market,
                quoteQuantity: 50);
            if (orderData.Success)
                await _subscribedbufferBlock.SendAsync("SuccessFully Placed!");
            else
                await _unsubscribedbufferBlock.SendAsync("Order is not placed!");
        }

        public async Task CancelOrderAsync(string orderId)
        {
            await CancelOrder(orderId);
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });

            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await CancelOrder(orderId);
            });

            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }
        private async Task CancelOrder(string Id)
        {
            var order = await _client.SpotApi.Trading.CancelOrderAsync(Id);
            if (order.Success)
                await _subscribedbufferBlock.SendAsync("SuccessFully Cancelled!");
            else
                await _unsubscribedbufferBlock.SendAsync("Error while removing!");
        }
    }
}   
