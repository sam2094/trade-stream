using Application.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class CurrencyCache : ICurrencyCache
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();

        public void UpdateCurrency(string currency, string price)
        {
            _cache[currency] = price;
        }

        public IDictionary<string, string> GetSnapshot()
        {
            return new Dictionary<string, string>(_cache);
        }
    }
}