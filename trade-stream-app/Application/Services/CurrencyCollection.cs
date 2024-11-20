using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services;

public class CurrencyCollection
{
    private readonly ConcurrentBag<string> _currencies;

    public CurrencyCollection()
    {
        _currencies = new ConcurrentBag<string>
        {
            "btcusdt@aggTrade",
            "ethusdt@aggTrade",
            "eurusdt@aggTrade"
        };
    }

    public IEnumerable<string> GetAllCurrencies()
    {
        return _currencies;
    }

    public IEnumerable<string> GetCurrenciesUppercaseTrimmed()
    {
        return _currencies
            .Select(currency => currency.Split('@')[0].ToUpper());
    }

    public void AddCurrency(string currency)
    {
        _currencies.Add(currency);
    }

    public void RemoveCurrency(string currency)
    {
        _currencies.TryTake(out _);
    }
}
