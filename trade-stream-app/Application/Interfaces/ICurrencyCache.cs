using System.Collections.Generic;

namespace Application.Interfaces;

public interface ICurrencyCache
{
    void UpdateCurrency(string currency, string price);
    IDictionary<string, string> GetSnapshot();
}
