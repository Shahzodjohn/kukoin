using CryptoExchange.Net.CommonObjects;
using Kucoin.Net.Clients;
using Kucoin.Net.Objects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Net.Mime.MediaTypeNames;

namespace kukoin
{
    public class SocketQueries
    {
        private BufferBlock<string> _subscribedbufferBlock { get; set; }
        private BufferBlock<string> _unsubscribedbufferBlock { get; set; }

        KucoinSocketClient _socketClient = new KucoinSocketClient(new KucoinSocketClientOptions
        {
            ApiCredentials = new KucoinApiCredentials("63036ec400c8fa0001d81986", "11867e71-9dcf-4b97-806e-e9a20c28424f", "Shaha919723377"),
            LogLevel = LogLevel.Trace,
        });

        public SocketQueries()
        {
            _subscribedbufferBlock = new BufferBlock<string>();
            _unsubscribedbufferBlock = new BufferBlock<string>();
        }

        public async Task TradeUpdatesAsync()
        {
            await SubscribeToTradeUpdatesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await SubscribeToTradeUpdatesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }
        private async Task SubscribeToTradeUpdatesAsync()
        {

            var subscribeToTrades = await _socketClient.SpotStreams.SubscribeToTradeUpdatesAsync("BTC-USDT", async data =>
            {
                var txt = data.Data.Id + "\n" +
                    data.Data.Quantity + "\n" +
                    data.Data.Side + "\n" +
                    data.Data.Sequence + "\n" +
                    data.Data.MakerOrderId + "\n" +
                    data.Data.Price + "\n" +
                    data.Data.Symbol + "\n" +
                    data.Data.TakerOrderId + "\n" +
                    data.Data.Type + "\n-------------------------";

                 await _subscribedbufferBlock.SendAsync(txt);
            });

            if (!subscribeToTrades.Success)
                await _unsubscribedbufferBlock.SendAsync("Не удалось подписаться.");
        }
//////////////////////////////////////////////////////////////////////////////////////////
        public async Task TickerUpdatesAsync()
        {
            await SubscribeToAllTickerUpdatesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await SubscribeToAllTickerUpdatesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }
        private async Task SubscribeToAllTickerUpdatesAsync()
        {
            var subscribeResult = await _socketClient.SpotStreams.SubscribeToAllTickerUpdatesAsync(async data =>
            {
                var txt = data.Data.Symbol + "\n" +
                    data.Data.LastQuantity + "\n" +
                    data.Data.LastPrice + "\n" +
                    data.Data.Sequence + "\n" +
                    data.Data.Timestamp + "\n" +
                    data.Data.BestAskPrice + "\n и тд. ";

                await _subscribedbufferBlock.SendAsync(txt);
            });

            if (!subscribeResult.Success)
                await _unsubscribedbufferBlock.SendAsync("Не удалось подписаться.");
        }

        //////////////////////////////////////
        public async Task BookOrderUpdatesAsync()
        {
            await SubscribeToOrderBookUpdatesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await SubscribeToOrderBookUpdatesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }

        private async Task SubscribeToOrderBookUpdatesAsync()
        {
            var order = await _socketClient.SpotStreams.SubscribeToOrderBookUpdatesAsync("btc-usdt".ToUpper(), 50, async data =>
            {
                string txt;
                foreach (var orderbook in data.Data.Bids)
                {
                    txt = "bids" + "\nquantity = " + orderbook.Quantity +
                       "\nprice = " + orderbook.Price +
                       "\nsequence = " + orderbook.Sequence +
                       "\n--------------------------------";
                    await _subscribedbufferBlock.SendAsync(txt);
                }

                foreach (var orderbook in data.Data.Asks)
                {
                    txt = "bids" + "\nquantity = " + orderbook.Quantity +
                       "\nprice = " + orderbook.Price +
                       "\nsequence = " + orderbook.Sequence +
                       "\n--------------------------------";
                    await _subscribedbufferBlock.SendAsync(txt);
                }
            });
            if (!order.Success)
                await _unsubscribedbufferBlock.SendAsync("Не удалось подписаться.");
        }   
        ////////////////////////////////////////
        public async Task BalanceUpdateAsync()
        {
            await SubscribeToBalanceUpdatesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await SubscribeToBalanceUpdatesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }

        private async Task SubscribeToBalanceUpdatesAsync()
        {
            var subs = await _socketClient.SpotStreams.SubscribeToBalanceUpdatesAsync(async data =>
            {
                var txt = "Total = " + data.Data.Total
                    + "\n Hold = " + data.Data.Hold +
                    "\n holdChange = " + data.Data.HoldChange +
                    "Available = " + data.Data.Available;
                await _subscribedbufferBlock.SendAsync(txt);
            });
            if (!subs.Success)
                await _unsubscribedbufferBlock.SendAsync("Не удалось подписаться.");
        }
        //////////////////////////////////////////////////////////
        
        public async Task OrderUpdatesAsync()
        {
            await SubscribeToOrderUpdatesAsync();
            var trueSubscribe = new ActionBlock<string>(value =>
            {
                Console.WriteLine(value);
            });
            var falseSubscribe = new ActionBlock<string>(async value =>
            {
                Console.WriteLine(value);
                await SubscribeToOrderUpdatesAsync();
            });
            _subscribedbufferBlock.LinkTo(trueSubscribe);
            _unsubscribedbufferBlock.LinkTo(falseSubscribe);
        }

        private async Task SubscribeToOrderUpdatesAsync()
        {
            string txt;
            var subscribeOpenedOrders = await _socketClient.SpotStreams.SubscribeToOrderUpdatesAsync(async data =>
            {
                txt = "Opened Orders" +
                       "\nQuantity = " + data.Data.Quantity +
                       "\nOldQuantity" + data.Data.OldQuantity +
                       "\nQuantityRemaining = " + data.Data.QuantityRemaining +
                       "\nSide = " + data.Data.Side +
                       "\nQuantityFilled = " + data.Data.QuantityFilled +
                       "\nOrderTime = " + data.Data.OrderTime +
                       "\nPrice = " + data.Data.Price +
                       "\nOrderTime" + data.Data.OrderTime +
                       "\nClientOrderid" + data.Data.ClientOrderid +
                       "------------------------------------------";
                await _subscribedbufferBlock.SendAsync(txt);
            },
            async data =>
            {
                txt = "Matched Orders" +
                     "\nQuantity = " + data.Data.Quantity +
                     "\nOldQuantity" + data.Data.OldQuantity +
                     "\nQuantityRemaining = " + data.Data.QuantityRemaining +
                     "\nSide = " + data.Data.Side +
                     "\nQuantityFilled = " + data.Data.QuantityFilled +
                     "\nOrderTime = " + data.Data.OrderTime +
                     "\nPrice = " + data.Data.Price +
                     "\nOrderTime" + data.Data.OrderTime +
                     "\nClientOrderid" + data.Data.ClientOrderid +
                     "------------------------------------------";
                 await _subscribedbufferBlock.SendAsync(txt);
            });

            if (!subscribeOpenedOrders.Success)
                await _unsubscribedbufferBlock.SendAsync("Не удалось подписаться.");
        }
        /////////////////////////////////
        


    }
    
}
