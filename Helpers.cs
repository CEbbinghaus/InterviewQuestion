/// <summary>
/// EPK Container Type
/// </summary>
readonly struct EPK(int value)
{
	readonly int value = value;
	public override string ToString() => value.ToString();
	public override int GetHashCode() => value.GetHashCode();

	public static implicit operator EPK(int value) => new(value);
	public static implicit operator EPK(string value) => new(int.Parse(value));
}

/// <summary>
/// PPK Container Type
/// </summary>
readonly struct PPK(int value)
{
	readonly int value = value;
	public override string ToString() => value.ToString();
	public override int GetHashCode() => value.GetHashCode();

	public static implicit operator PPK(int value) => new(value);
	public static implicit operator PPK(string value) => new(int.Parse(value));
}


/// <summary>
/// Packet A
/// Format:
/// A:EPK:Value 
/// <param name="epk"></param>
/// <param name="value"></param>
/// </summary>

class A(EPK epk, string value)
{
	public EPK EPK = epk;
	public string Value = value;

	public static A FromString(string packet)
	{
		return packet.Split("::") switch
		{
		[_, var ppk, var value] => new A(ppk, value),
			_ => throw new Exception("Invalid packet format")
		};
	}

	public override string ToString() => $"A::{EPK}::{Value}";
}

/// <summary>
/// Packet B
/// Format:
/// B:PPK:EPK:Value
/// <param name="ppk"></param>
/// <param name="epk"></param>
/// <param name="value"></param>
/// </summary>

class B(PPK ppk, EPK epk, string value)
{
	public PPK PPK = ppk;
	public EPK EPK = epk;
	public string Value = value;

	public static B FromString(string packet)
	{
		return packet.Split("::") switch
		{
			[_, var ppk, var epk, var value] => new B(ppk, epk, value),
			_ => throw new Exception("Invalid packet format")
		};
	}

	public override string ToString() => $"B::{PPK}::{EPK}::{Value}";
}


/// <summary>
/// Packet C
/// Format:
/// C:PPK:EPK:Value
/// <param name="ppk"></param>
/// <param name="epk"></param>
/// <param name="value"></param>
/// </summary>
class C(PPK ppk, EPK epk, string value)
{
	public PPK PPK = ppk;
	public EPK EPK = epk;
	public string Value = value;

	public static C FromString(string packet)
	{
		return packet.Split("::") switch
		{
		[_, var ppk, var epk, var value] => new C(ppk, epk, value),
			_ => throw new Exception("Invalid packet format")
		};
	}

	public override string ToString() => $"C::{PPK}::{EPK}::{Value}";
}


/// <summary>
/// Packet D
/// Format:
/// D:PPK:Value  
/// <param name="ppk"></param>
/// <param name="value"></param>
/// </summary>

class D(PPK ppk, string value)
{
	public PPK PPK = ppk;
	public string Value = value;

	public static D FromString(string packet)
	{
		return packet.Split("::") switch
		{
		[_, var ppk, var value] => new D(ppk, value),
			_ => throw new Exception("Invalid packet format")
		};
	}

	public override string ToString() => $"D::{PPK}::{Value}";
}


/// <summary>
/// Interface used to define the solution
/// </summary>
public interface ISolution
{
	public string Solve(string input);
}
