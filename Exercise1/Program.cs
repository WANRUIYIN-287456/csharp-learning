//--------------------------------------------
// Week 1 - variables, methods, control flow
//--------------------------------------------

// Create an array of 5 integers
int[] numbers = { 1, 2, 3, 4, 5 };

// Write a method called IsEven that takes an int and returns a bool.
static bool IsEven(int input)
{
    return input % 2 == 0;
}

// BONUS: add a method SumArray that takes an int[] and returns the total, then print the sum at the end.
static int SumArray(int[] numberInputs)
{
    var totalSum = 0;
    foreach (int n in numberInputs)
    {
        totalSum += n;
    }
    return totalSum;
}

// Loop through the array and print each number along with whether it's even or odd.
foreach (int n in numbers)
{
    string oddEven = IsEven(n) ? "even" : "odd";
    Console.WriteLine($"{n} is {oddEven}");
}

Console.WriteLine($"\nSum: {SumArray(numbers)}");