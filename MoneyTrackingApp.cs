
using System;
using System.Collections.Generic;

class MoneyTrackingApp
{
    private static List<Transaction> moneyTransactions;
    private static string filePath = "C:\\Users\\46727\\Desktop\\miniProject w4.txt";

    public MoneyTrackingApp()
    {
        moneyTransactions = FileManager.LoadTransactionsFromFile(filePath);
    }

    public void Run()
    {
        while (true)
        {
            decimal currentAmount = CalculateCurrentAmount();

            // Console.Clear();
            Console.WriteLine("======================================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Current Amount: {currentAmount:C}");
            Console.ResetColor();
            Console.WriteLine("Money Tracking Application");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Add Transaction");
            Console.WriteLine("2. View Transactions (sorted by date)");
            Console.WriteLine("3. Update Transaction");
            Console.WriteLine("4. Remove Transaction");
            Console.WriteLine("5. Exit");
            Console.WriteLine("==============================");

            Console.Write("Choose an option (1-5): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTransaction();
                    FileManager.SaveTransactionsToFile(filePath, moneyTransactions);
                    
                    break;
                case "2":
                    ViewTransactions();
                    break;
                case "3":
                    UpdateTransaction();
                    FileManager.SaveTransactionsToFile(filePath, moneyTransactions);
                   
                    break;
                case "4":
                    RemoveTransaction();
                    FileManager.SaveTransactionsToFile(filePath, moneyTransactions);
                    Console.WriteLine("Transaction removed successfully.");
                    break;
                case "5":
                    FileManager.SaveTransactionsToFile(filePath, moneyTransactions);
                    Console.WriteLine("Transactions saved to file. Exiting...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Please choose again.");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void AddTransaction()
    {
        Console.WriteLine("\nAdd Transaction:");

        Console.Write("Enter date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Invalid date format. Transaction not added.");
            return;
        }

        Console.Write("Enter amount: ");
        string amount = Console.ReadLine();

        Console.Write("Is it a withdrawal? (y/n): ");
        bool isWithdrawal = Console.ReadLine().ToLower() == "y";

        Console.Write("Enter source (e.g., salary, rent, etc.): ");
        string source = Console.ReadLine();

        // Attempt to add the transaction
        try
        {
            moneyTransactions.Add(new Transaction
            {
                Date = date,
                Amount = amount,
                IsWithdrawal = isWithdrawal,
                Source = source
            });

            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("Transaction added successfully.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            // Handle the exception
            Console.WriteLine($"Error adding transaction: {ex.Message}");
        }
    }



    void ViewTransactions()
    {
        List<Transaction> transactions = FileManager.LoadTransactionsFromFile(filePath);

        // Sort transactions by date before displaying
        transactions.Sort((t1, t2) => t2.Date.CompareTo(t1.Date));

        Console.WriteLine("\nMoney Transactions (sorted by date):");

        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions available.");
        }
        else
        {
            Console.WriteLine("{0,-12} {1,-10} {2,-16} {3,-20}", "Date", "Amount", "Expense/Income", "Source");
            Console.WriteLine("===============================================");

            foreach (var transaction in transactions)
            {
                string type = transaction.IsWithdrawal ? "Expense" : "Income";
                Console.WriteLine($"{transaction.Date.ToString("yyyy-MM-dd"),-12} {transaction.Amount,-10} {type,-16} {transaction.Source,-20}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Choose an option for filtering:");
        Console.WriteLine("1. View Expenses");
        Console.WriteLine("2. View Incomes");
        Console.WriteLine("3. Exit to main menu");

        Console.Write("Choose an option (1-3): ");
        string filterChoice = Console.ReadLine();

        switch (filterChoice)
        {
            case "1":
                ViewFilteredTransactions(transactions, isExpense: true);
                break;
            case "2":
                ViewFilteredTransactions(transactions, isExpense: false);
                break;
            case "3":
                //Do nothing, exit to main menu
                break;
            default:
                Console.WriteLine("Invalid option. Displaying all transactions.");
                break;
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }


    static void UpdateTransaction()
    {
        Console.WriteLine("\nUpdate Transaction:");

        if (moneyTransactions.Count == 0)
        {
            Console.WriteLine("No transactions available for update.");
            return;
        }

        // Sort transactions by date before displaying
        moneyTransactions.Sort((t1, t2) => t2.Date.CompareTo(t1.Date));

        Console.WriteLine("{0,-5} {1,-12} {2,-10} {3,-16} {4,-20}", "Num", "Date", "Amount", "Expense/Income", "Source");
        Console.WriteLine("=============================================================");

        for (int i = 0; i < moneyTransactions.Count; i++)
        {
            var transaction = moneyTransactions[i];
            string type = transaction.IsWithdrawal ? "Expense" : "Income";
            Console.WriteLine($"{i + 1,-5} {transaction.Date.ToString("yyyy-MM-dd"),-12} {transaction.Amount,-10} {type,-16} {transaction.Source,-20}");
        }


        Console.Write("Enter the number of the transaction to update: ");
        if (int.TryParse(Console.ReadLine(), out int selectedTransactionIndex) && selectedTransactionIndex >= 1 && selectedTransactionIndex <= moneyTransactions.Count)
        {
            var selectedTransaction = moneyTransactions[selectedTransactionIndex - 1];

            Console.WriteLine($"Selected Transaction: Num: {selectedTransactionIndex}, " +
                              $"Date: {selectedTransaction.Date.ToString("yyyy-MM-dd")}, " +
                              $"Amount: {selectedTransaction.Amount}, " +
                              $"Type: {(selectedTransaction.IsWithdrawal ? "Expense" : "Income")}, " +
                              $"Source: {selectedTransaction.Source}");

            // Create a copy of the transaction before attempting to update
            var originalTransaction = new Transaction
            {
                Date = selectedTransaction.Date,
                Amount = selectedTransaction.Amount,
                IsWithdrawal = selectedTransaction.IsWithdrawal,
                Source = selectedTransaction.Source
            };

            // Flag to track whether the update process should proceed or not
            bool updateSuccessful = true;

            // Attempt to update the transaction
            try
            {
                Console.Write("Enter new date (yyyy-MM-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime newDate))
                {
                    selectedTransaction.Date = newDate;
                }
                else
                {
                    Console.WriteLine("Invalid date format. The date remains unchanged.");
                    updateSuccessful = false;
                }

                Console.Write("Enter new amount: ");
                string newAmount = Console.ReadLine();

                // Validate the new amount format before updating
                if (decimal.TryParse(newAmount, out decimal parsedAmount))
                {
                    selectedTransaction.Amount = newAmount;
                }
                else
                {
                    // If the amount format is invalid, revert changes
                    Console.WriteLine("Invalid amount format. The amount remains unchanged.");
                    selectedTransaction.Date = originalTransaction.Date;
                    selectedTransaction.Amount = originalTransaction.Amount;
                    updateSuccessful = false;
                }

                Console.Write("Is it a withdrawal? (y/n): ");
                string isWithdrawalInput = Console.ReadLine();

                // Validate the new isWithdrawal input
                if (isWithdrawalInput.ToLower() == "y" || isWithdrawalInput.ToLower() == "n")
                {
                    selectedTransaction.IsWithdrawal = isWithdrawalInput.ToLower() == "y";
                }
                else
                {
                    // If the input is invalid, revert changes
                    Console.WriteLine("Invalid input for withdrawal. The withdrawal status remains unchanged.");
                    selectedTransaction.Date = originalTransaction.Date;
                    selectedTransaction.Amount = originalTransaction.Amount;
                    selectedTransaction.IsWithdrawal = originalTransaction.IsWithdrawal;
                    updateSuccessful = false;
                }

                Console.Write("Enter new source (e.g., salary, rent, etc.): ");
                selectedTransaction.Source = Console.ReadLine();

                if (updateSuccessful)
                {
                    Console.WriteLine("Transaction updated successfully.");
                }
                else
                {
                    Console.WriteLine("Transaction not fully updated due to validation errors.");
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine($"Error updating transaction: {ex.Message}");

                // If an error occurs, revert changes
                selectedTransaction.Date = originalTransaction.Date;
                selectedTransaction.Amount = originalTransaction.Amount;
                selectedTransaction.IsWithdrawal = originalTransaction.IsWithdrawal;
                selectedTransaction.Source = originalTransaction.Source;
            }
        }
        else
        {
            Console.WriteLine("Invalid selection. Transaction not updated.");
        }
    }






    void RemoveTransaction()
    {
        Console.WriteLine("\nRemove Transaction:");

        if (moneyTransactions.Count == 0)
        {
            Console.WriteLine("No transactions available for removal.");
            return;
        }

        // Sort transactions by date before displaying
        moneyTransactions.Sort((t1, t2) => t2.Date.CompareTo(t1.Date));

        Console.WriteLine("{0,-5} {1,-12} {2,-10} {3,-16} {4,-20}", "Num", "Date", "Amount", "Expense/Income", "Source");
        Console.WriteLine("=============================================================");

        for (int i = 0; i < moneyTransactions.Count; i++)
        {
            var transaction = moneyTransactions[i];
            string type = transaction.IsWithdrawal ? "Expense" : "Income";
            Console.WriteLine($"{i + 1,-5} {transaction.Date.ToString("yyyy-MM-dd"),-12} {transaction.Amount,-10} {type,-16} {transaction.Source,-20}");
        }

        Console.Write("Enter the number of the transaction to remove: ");
        if (int.TryParse(Console.ReadLine(), out int selectedTransactionIndex) && selectedTransactionIndex >= 1 && selectedTransactionIndex <= moneyTransactions.Count)
        {
            moneyTransactions.RemoveAt(selectedTransactionIndex - 1);
            Console.WriteLine("Transaction removed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid selection. Transaction not removed.");
        }
    }


    static void ViewFilteredTransactions(List<Transaction> transactions, bool isExpense)
    {
        // Filter transactions based on expense or income
        var filteredTransactions = transactions.Where(t => t.IsWithdrawal == isExpense).ToList();

        // Sort filtered transactions by date in descending order (newest to oldest)
        filteredTransactions.Sort((t1, t2) => t2.Date.CompareTo(t1.Date));

        // Display the sorted filtered transactions
        Console.WriteLine($"Money {(isExpense ? "Expenses" : "Incomes")} (sorted by date, newest to oldest):");

        if (filteredTransactions.Count == 0)
        {
            Console.WriteLine("No transactions available.");
        }
        else
        {
            Console.WriteLine("{0,-12} {1,-10} {2,-16} {3,-20}", "Date", "Amount", "Expense/Income", "Source");
            Console.WriteLine("===============================================");

            // Output transactions in the sorted order
            foreach (var transaction in filteredTransactions)
            {
                string type = transaction.IsWithdrawal ? "Expense" : "Income";
                Console.WriteLine($"{transaction.Date.ToString("yyyy-MM-dd"),-12} {transaction.Amount,-10} {type,-16} {transaction.Source,-20}");
            }

        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine(); // Pause before returning to main menu

    }

    static decimal CalculateCurrentAmount()
    {
        return moneyTransactions.Sum(t => t.IsWithdrawal ? -decimal.Parse(t.Amount) : decimal.Parse(t.Amount));
    }


}
   

