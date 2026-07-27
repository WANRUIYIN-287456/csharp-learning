using System;

class Program
{
    static async Task<bool> VerifyAccountAsync()
    {
        await Task.Delay(1000);   // simulate a network/server delay
        return true;   // pretend it always succeeds for this exercise
    }

    static async Task Main()
    {
        BankAccount acc = new BankAccount("Lily", "ACC113456");
        acc.OnTransaction += result => Console.WriteLine(result);

        Console.WriteLine("Verifying account...");
        var verified = await VerifyAccountAsync();
        if (verified) Console.WriteLine("Account verified.");

        acc.Deposit(5000);
        acc.Withdraw(200.12m);
        acc.Withdraw(-500);
        acc.Withdraw(5100);

        Console.WriteLine($"\nDisplay name (no nickname): {acc.GetDisplayName()}");

        acc.Nickname = "Lily's Savings";
        Console.WriteLine($"Display name (with nickname): {acc.GetDisplayName()}");
    }
}

class BankAccount
{
    // Properties
    public string AccountHolder { get; private set; }
    public decimal Balance { get; private set; }
    readonly string AccountNumber;
    public string? Nickname { get; set; }

    // Constructor
    public BankAccount(string accountHolder, string accountNumber)
    {
        AccountHolder = accountHolder;
        AccountNumber = accountNumber;
        Balance = 0;
    }

    // Methods
    public decimal Deposit(decimal amount)
    {
        try
        {
            Console.WriteLine($"\nDepositing {amount}");
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative");
            }
            Balance += amount;
            OnTransaction?.Invoke($"Deposited {amount:F2}, new balance: {Balance:F2}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }

        return Balance;
    }

    public decimal Withdraw(decimal amount)
    {
        try
        {
            Console.WriteLine($"\nWithdrawing {amount}");
            if (amount < 0)
            {
                throw new ArgumentException("Amount cannot be negative");
            }
            if (amount > Balance)
            {
                throw new InsufficientFundsException("Insufficient funds");
            }
            Balance -= amount;
            OnTransaction?.Invoke($"Withdrew {amount:F2}, new balance: {Balance:F2}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
        catch (InsufficientFundsException ex)
        {
            Console.WriteLine($"error: {ex.Message}");
        }
        // Cannot use return/break/continue/goto in finally block
        // direct return Balance without finally here
        return Balance;
    }

    public event Action<string>? OnTransaction;

    public string GetDisplayName()
    {
        return Nickname ?? AccountHolder;
    }
}

class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}