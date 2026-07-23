using System;

class Program
{
    static void Main()
    {
        BankAccount acc = new BankAccount("Lily", "ACC113456");
        acc.Deposit(5000);
        acc.PrintStatement();
        acc.Withdraw(-500);
        acc.PrintStatement();
        acc.Withdraw(200.12m);
        acc.PrintStatement();
        acc.Withdraw(5100);
        acc.PrintStatement();
    }
}

class BankAccount
{
    // Properties
    public string AccountHolder { get; private set; }
    public decimal Balance { get; private set; }
    readonly string AccountNumber;

    // Another method for determining insufficient funds.
    // For customized setters, we need backing field, private decimal _balance
    // as it is the real variable while this Balance is actually a Property name.
    //
    // public decimal Balance
    // {
    //     get { return _balance; }
    //     private set
    //     {
    //         if (value < 0) throw new ArgumentException("Insufficient funds");
    //         _balance = value;
    //     }
    // }
    // private decimal _balance;


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
        Console.WriteLine($"\nDepositing {amount}");
        if (amount < 0)
        {
            Console.WriteLine("Amount cannot be negative");
            return Balance;
        }
        Balance += amount;
        return Balance;
    }

    public decimal Withdraw(decimal amount)
    {
        Console.WriteLine($"\nWithdrawing {amount}");
        if (amount < 0)
        {
            Console.WriteLine("Amount cannot be negative");
            return Balance;
        }
        if (amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
            return Balance;
        }
        Balance -= amount;
        return Balance;
    }

    public void PrintStatement()
    {
        Console.WriteLine($"{AccountHolder} (AccNo.: {AccountNumber}) -- ${Balance}");
    }

}