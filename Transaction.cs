
using System;

class Transaction
{
    public DateTime Date { get; set; }
    public string Amount { get; set; }
    public bool IsWithdrawal { get; set; }
    public string Source { get; set; }
}
