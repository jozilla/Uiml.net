using System;


/// double implementation
class CalcFunc
{
	public static bool empty = true;
	public static double result = 0;
	public static double temp = 0;
	public static string sign = null;
	public static string operation = null;

	public static void RecordNumber(string number)
	{
		temp = Double.Parse(number);
		if(operation != null)
		{
			result = Calculate(temp);
			operation = null;
		}
		else
			result = temp;

	}

	public static double Calculate(double g)
	{
		switch(operation)
		{
			case "+":
				return result + g;
			case "-":
				return result - g;
			case "*":
				return result * g;
			case "/":
				return result / g;		
		}
		return g;
	}

	public static void RecordSign(string psign)
	{
		sign = psign;
		result = temp;
	}

	public static void RecordOperation(string oper)
	{
		operation = oper;
	}

	public static string SwitchSign(String number)
	{
		return -Double.Parse(number) + "";
	}


	public static String CalculateResult()
	{
		sign = null;
	   operation = null;
		empty = true;
		return "" + result;
	}
}
