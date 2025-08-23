using System.Collections.Generic;

namespace Content.Features.CurrencySystem
{
    public interface ICurrencyService
    {
        int GetBalance(CurrencyType currency);
        void Add(CurrencyType currency, int amount);
        void Spend(CurrencyType currency, int amount);
        bool CanAfford(Dictionary<CurrencyType, int> costs);
        bool CanAfford(CurrencyType type, int cost);
    }
}