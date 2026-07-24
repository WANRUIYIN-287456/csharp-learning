using System;

class Program
{
    static void Main()
    {
        List<Employee> empList;
        FullTimeEmployee manager = new("Manager Tan", 8000.00m);
        ContractEmployee sales = new("Sales Amir", 0, 160, 22.35m);
        // empList = new List<Employee>{Manager, Sales};
        empList = [manager, sales]; // Simplified Version

        foreach ( Employee n in empList) {
            n.PrintPaySlip();
            if (n is ITaxable taxable)
            {
                Console.WriteLine($"TAX: ${taxable.CalculateTax():F2}\n");
            }
        }

    }
}

interface ITaxable
{
    decimal CalculateTax();
}

abstract class Employee
{
    public string Name { get; set; }
    public decimal BaseSalary { get; set; }

    public Employee (string name, decimal baseSalary)
    {
        Name = name;
        BaseSalary = baseSalary;
    }

    public abstract decimal CalculatePay();

    public void PrintPaySlip()
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("                        PAYSLIP                           ");
        Console.WriteLine("==========================================================");
        Console.WriteLine($"NAME: {Name} \nSALARY: ${CalculatePay():F2}");
    }
}

class FullTimeEmployee : Employee, ITaxable
{
    public FullTimeEmployee(string name, decimal baseSalary) : base(name, baseSalary)
    {
    }

    public override decimal CalculatePay()
    {
        return BaseSalary;
    }

    public decimal CalculateTax()
    {
        return BaseSalary * 0.15m;
    }
}

class ContractEmployee : Employee, ITaxable
{

    public int HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public ContractEmployee(string name, decimal baseSalary, int hoursWorked, decimal hourlyRate) : base(name, baseSalary)
    {
        HoursWorked = hoursWorked;
        HourlyRate = hourlyRate;
    }

    public override decimal CalculatePay()
    {
        return HoursWorked * HourlyRate;
    }

    public decimal CalculateTax()
    {
        return CalculatePay() * 0.1m;
    }
}