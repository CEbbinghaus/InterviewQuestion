
class Solution : ISolution {
	public string Solve(string input)
	{
		return Process(input);
	}
	public string Process(string packets) {
		string output = "";
		foreach(var packet in packets.Split("\n"))
		{
			if(string.IsNullOrWhiteSpace(packet))
			{
				continue;
			}

			var packetType = packet.FirstOrDefault();
            output += packetType switch
            {
                'A' => ProcessA(A.FromString(packet)),
                'B' => ProcessB(B.FromString(packet)),
                'C' => ProcessC(C.FromString(packet)),
                'D' => ProcessD(D.FromString(packet)),
                _ => throw new Exception("Invalid packet type"),
            };
        }

		output += string.Join("\n", ppkToD.SelectMany(v => v.Value.Select(d => d.ToString())));
		return output;
	}

	enum ContainerFlags
	{
		None = 0b000,
		A = 0b001,
		B = 0b010,
		AB = 0b011,
		C = 0b100,
		Done = 0b111
	}
	class Container
	{
		public ContainerFlags Flags { get; set; } = ContainerFlags.None;
		public A? A { get; set; }
		public B? B { get; set; }
		public C? C { get; set; }
		public Queue<D> D { get; set; } = [];
	}

	Dictionary<PPK, EPK> ppkToEpk = [];
	Dictionary<PPK, List<D>> ppkToD = [];
	Dictionary<EPK, Container> epkToContainer = [];

	string WritePossiblePackets(Container container)
	{
		if(container.Flags == ContainerFlags.None)
		{
			return "";
		}

		string output = "";
		if((container.Flags & ContainerFlags.A) == ContainerFlags.A && container.A is not null)
		{
			output += $"{container.A}\n";
			container.A = null;
		}

		if((container.Flags & ContainerFlags.AB) == ContainerFlags.AB && container.B is not null)
		{
			output += $"{container.B}\n";
			container.B = null;
		}

		if(container.Flags != ContainerFlags.Done)
		{
			return output;
		}

		if (container.C is not null)
		{
			output += $"{container.C}\n";
			container.C = null;
		}

		while(container.D.TryDequeue(out var next))
		{
			output += $"{next}\n";
		}
		
		return output;
	}

	Container TryAddToContainer(EPK epk)
	{
		epkToContainer.TryAdd(epk, new Container());
		return epkToContainer[epk];
	}

	void EnsurePpkToEpk(PPK ppk, EPK epk, Container container)
	{
		if(ppkToEpk.ContainsKey(ppk))
		{
			return;
		}

		ppkToEpk.Add(ppk, epk);
		if(ppkToD.Remove(ppk, out var ds))
		{
			ds.ForEach(container.D.Enqueue);
		}
	}

	string ProcessA(A a) {
		var container = TryAddToContainer(a.EPK);
		
		container.A = a;
		container.Flags |= ContainerFlags.A;

		return WritePossiblePackets(container);
	}

	string ProcessB(B b) {
		var container = TryAddToContainer(b.EPK);
		EnsurePpkToEpk(b.PPK, b.EPK, container);
		
		container.B = b;
		container.Flags |= ContainerFlags.B;

		return WritePossiblePackets(container);
	}

	string ProcessC(C c) {
		var container = TryAddToContainer(c.EPK);
		EnsurePpkToEpk(c.PPK, c.EPK, container);

		container.C = c;
		container.Flags |= ContainerFlags.C;

		return WritePossiblePackets(container);
	}

	string ProcessD(D d) {
		if(ppkToEpk.TryGetValue(d.PPK, out var epk))
		{
			var container = epkToContainer[epk];
			container.D.Enqueue(d);
			return WritePossiblePackets(container);
		}
		
		ppkToD.TryAdd(d.PPK, []);
		ppkToD[d.PPK].Add(d);
		return "";
	}
	


	// public static void Main(string[] args) {
	// 	Action func = args switch {
	// 		["generate", var file] => () => Generate(file),
	// 		["run", var file] => () => Run(file),
	// 		_ => throw new Exception("Invalid arguments")
	// 	};

	// 	func();
	// }

	public static void Generate(string file)
	{
		int numContainers = 50 + Random.Shared.Next(50);
		Queue<int> ids = new(Enumerable.Range(0, numContainers * 10).Select(_ => Random.Shared.Next()).Distinct());
		if(ids.Count < numContainers * 2)
		{
			throw new Exception("BAD");
		}
		//int i = 0;
		Container CreateContainer(int n)
		{
			// var epk = new EPK(Random.Shared.Next());
			// var ppk = new PPK(Random.Shared.Next());
			var epk = new EPK(ids.Dequeue());
			var ppk = new PPK(ids.Dequeue());
			// var value = Guid.NewGuid().ToString("N");
			var value = "";
			// var value = $"Test-{i++}";
			return new Container
			{
				A = new(epk, value),
				B = new(ppk, epk, value),
				C = new(ppk, epk, value),
				D = new(Enumerable.Range(0, 1 + Random.Shared.Next(9)).Select(_ => new D(ppk, value)))
			};
		}
		var containers = Enumerable.Range(0, numContainers).Select(CreateContainer);

		var packets = containers.SelectMany(container => container.D.Select(v => v.ToString()).Concat([$"{container.A}", $"{container.B}", $"{container.C}"])).ToArray();

		Random.Shared.Shuffle(packets);

		File.WriteAllText(file, string.Join("\n", packets));
	}

	static void Run(string file)
	{
		var packets = File.ReadAllText(file);
		var instance = new Solution();
		Console.WriteLine(instance.Process(packets));
	}


	public static bool Test(string packets)
	{
		Dictionary<PPK, EPK> ppktoepk = [];
		Dictionary<EPK, int> epkcount = [];
		
		int i = 0;
		foreach(var packet in packets.Split("\n"))
		{
			++i;
			if(string.IsNullOrWhiteSpace(packet))
			{
				continue;
			}

			var packetType = packet.FirstOrDefault();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
			object[] res = packetType switch {
				'A' => [A.FromString(packet).EPK, null],
				'B' => [B.FromString(packet).EPK, B.FromString(packet).PPK],
				'C' => [C.FromString(packet).EPK, C.FromString(packet).PPK],
				'D' => [null, D.FromString(packet).PPK],
				_ => throw new Exception("Invalid packet type"),
			};
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

			EPK? epk = null;
			PPK? ppk = null;

			switch(res) {
				case [null, null]:
					throw new Exception("Invalid packet format");
				case [EPK localepk, null]:
					epk = localepk;
					if(epkcount.ContainsKey(localepk))
					{
						return false;
					}
					epkcount.Add(localepk, 0);
					break;
				case [EPK localepk, PPK localppk]:
					(epk, ppk) = (localepk, localppk);
					ppktoepk.TryAdd(localppk, localepk);
					break;
				case [null, PPK localppk]:
					ppk = localppk;
					break;
				default:
					throw new Exception("How did we get here???");
			}

			if (epk.ToString() == "-1")
			{
				continue;
			}

			var minVal = packetType switch {
				'A' => 0,
				'B' => 1,
				'C' => 2,
				'D' => 3,
				_ => throw new Exception("Invalid packet type"),
			};

			if(ppk is PPK nonnullppk && !ppktoepk.ContainsKey(nonnullppk))
			{
				return false;
			}

			var key = epk ?? ppktoepk[ppk ?? throw new Exception("How did we get here???")];

			if(!epkcount.ContainsKey(key) || epkcount[key] < minVal)
			{
				return false;
			}

			epkcount[key] = minVal + 1;
		}
		return true;
	}
}


static class RandomExtensions
{
	public static void Shuffle<T>(this Random rng, T[] array)
	{
		int n = array.Length;
		while (n > 1)
		{
			int k = rng.Next(n--);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }
}
