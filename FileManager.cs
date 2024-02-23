
using System;
using System.Collections.Generic;
using System.IO;

class FileManager
{
    public static List<Transaction> LoadTransactionsFromFile(string filePath)
    {
        List<Transaction> transactions = new List<Transaction>();

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        transactions.Add(new Transaction
                        {
                            Date = DateTime.Parse(parts[0]),
                            Amount = parts[1],
                            IsWithdrawal = bool.Parse(parts[2]),
                            Source = parts[3]
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading transactions from file: {ex.Message}");
        }

        return transactions;
    }

    public static void SaveTransactionsToFile(string filePath, List<Transaction> transactions)
    {
        try
        {
            // Write all transactions to the file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var transaction in transactions)
                {
                    writer.WriteLine($"{transaction.Date},{transaction.Amount},{transaction.IsWithdrawal},{transaction.Source}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving transactions to file: {ex.Message}");
        }
    }
}
