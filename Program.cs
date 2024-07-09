var input = File.ReadAllText("tests/test");

ISolution attempt = new Attempt();

var result = attempt.Solve(input);

if (result.Trim() == File.ReadAllText("tests/expected").Trim())
{
	Console.WriteLine("Test passed 💯");
}
else
{
	Console.WriteLine("Test Failed 💔");
}

public class Attempt : ISolution 
{
	public string Solve(string input)
	{
		return "";
	}
}

