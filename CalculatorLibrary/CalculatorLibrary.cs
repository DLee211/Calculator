using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;

namespace CalculatorLibrary
{
    public class Calculator
    {
        private JsonWriter writer;

        public static List<Operation> operations = new List<Operation>();

        public Calculator()
        {
            StreamWriter logFile = File.CreateText("calculatorlog.json");
            logFile.AutoFlush = true;
            writer = new JsonTextWriter(logFile);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("Operations");
            writer.WriteStartArray();
        }

        public double DoOperation(double num1, double num2, string op)
        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            writer.WriteStartObject();
            writer.WritePropertyName("Operand1");
            writer.WriteValue(num1);
            writer.WritePropertyName("Operand2");
            writer.WriteValue(num2);
            writer.WritePropertyName("Operation");
            // Use a switch statement to do the math.
            switch (op)
            {
                case "a":
                    result = num1 + num2;
                    writer.WriteValue("Add");
                    AddToHistory(num1, num2, result, OperationType.Addition);
                    break;
                case "s":
                    result = num1 - num2;
                    writer.WriteValue("Subtract");
                    AddToHistory(num1, num2, result, OperationType.Subtraction);
                    break;
                case "m":
                    result = num1 * num2;
                    writer.WriteValue("Multiply");
                    AddToHistory(num1, num2, result, OperationType.Multiplication);
                    break;
                case "d":
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                    }
                    writer.WriteValue("Divide");
                    AddToHistory(num1, num2, result, OperationType.Division);
                    break;
                // Return text for an incorrect option entry.
                default:
                    break;
            }
            writer.WritePropertyName("Result");
            writer.WriteValue(result);
            writer.WriteEndObject();

            return result;
        }

        public static void PrintOperationHistory()
        {
            var operationsToPrint = operations;
            
            Console.WriteLine("Operation History");
            Console.WriteLine("----------------------------------");

            foreach (var operation in operationsToPrint)
            {
                Console.WriteLine($"DATE: {operation.Date} ---- OPERATION: {operation.operationType} ---- FIRSTNUMBER: {operation.firstNumber} ---- SECONDNUMBER: {operation.secondNumber} ---- RESULT: {operation.operationResult}");
            }

            if (operationsToPrint.Count >= 1)
            {
                DeleteHistory();
            }
        }

        private static void DeleteHistory()
        {
            Console.WriteLine("If you wish to delete the history, press 'c'. If you want to continue, press any key");

            var keyPress = Console.ReadLine();

            switch (keyPress.Trim().ToLower())
            {
                case "c":
                    operations.Clear();
                    break;
                
                default:
                    break;
            }
        }

        public static void AddToHistory(double num1, double num2, double result, OperationType Type)
        {
            operations.Add(new Operation
            {
                Date = DateTime.Now,
                firstNumber = num1,
                secondNumber = num2,
                operationResult = result,
                operationType = Type
            });
        }

        public void Finish()
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Close();
        }
    }

    public class Operation
    {
        public DateTime Date { get; set; }
        public double firstNumber { get; set; }
        public double secondNumber { get; set; }
        public double operationResult { get; set; }
        public OperationType operationType { get; set; }
    }

    public enum OperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
    }
}