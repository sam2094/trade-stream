using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class BinanceService : IMarketDataService, IDisposable
{
    private readonly Uri _binanceUri;
    private readonly IConfiguration _configuration;
    private readonly CurrencyCollection _currencyCollection;
    private CancellationTokenSource _cancellationTokenSource;
    private ClientWebSocket _webSocket;
    private bool _disposed;
    private readonly ICurrencyCache _currencyCache;

    public BinanceService(IConfiguration configuration, CurrencyCollection currencyCollection, ICurrencyCache currencyCache)
    {
        _configuration = configuration;
        var uriString = _configuration["Binance:WebSocketUrl"];

        if (string.IsNullOrEmpty(uriString))
        {
            throw new ArgumentException("Binance WebSocket URL is not configured.");
        }

        _binanceUri = new Uri(uriString);
        _currencyCollection = currencyCollection;

        _webSocket = new ClientWebSocket();
        _currencyCache = currencyCache;
    }

    public async Task StartAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(BinanceService), "Cannot start a disposed service.");
        }

        if (_webSocket == null || _webSocket.State != WebSocketState.None)
        {
            _webSocket?.Dispose();
            _webSocket = new ClientWebSocket();
        }

        _cancellationTokenSource = new CancellationTokenSource();

        await _webSocket.ConnectAsync(_binanceUri, _cancellationTokenSource.Token);
        Console.WriteLine("Connected to Binance WebSocket");

        await SubscribeToCurrenciesAsync();

        _ = ReceiveMessagesAsync();
    }

    public async Task StopAsync()
    {
        if (_disposed)
        {
            Console.WriteLine("Service is already disposed.");
            return;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();

        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("WebSocket connection closed");
        }

        _webSocket?.Dispose();
        _webSocket = null;
    }

    private async Task SubscribeToCurrenciesAsync()
    {
        var currencies = _currencyCollection.GetAllCurrencies();
        var subscribeMessage = new
        {
            method = "SUBSCRIBE",
            @params = currencies,
            id = 1
        };

        var messageJson = JsonSerializer.Serialize(subscribeMessage);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);

        await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        Console.WriteLine("Subscribed to currencies");
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received message: {message}");

                        ProcessMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("WebSocket connection closed by server");
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Message receiving canceled.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ReceiveMessagesAsync: {ex.Message}");
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            var data = JsonSerializer.Deserialize<BinanceResponse>(message);

            if (data != null && !string.IsNullOrWhiteSpace(data.Currency))
            {
                _currencyCache.UpdateCurrency(data.Currency.ToUpper(), data.Price);
                Console.WriteLine($"Updated cache: {data.Currency} - {data.Price}");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing message: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();

        if (_webSocket != null && _webSocket?.State == WebSocketState.Open)
        {
            _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disposing", CancellationToken.None).Wait();
        }

        _webSocket?.Dispose();
        _webSocket = null;
        _disposed = true;

        Console.WriteLine("BinanceService disposed.");
    }
}
