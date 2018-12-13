using System;
using System.Text;

// Couldn't be simpler. Call this to generate a pseudorandom name:
// Names.generateName(1)

namespace Kabel {
    class Names {
		public static string generateName(int seed){
			Random rand = new Random(seed);
			int firstName = rand.Next(256);
			int lastName = rand.Next(256);
			
			string nameList = System.IO.File.ReadAllText("name_list.txt");
			string name = "";
			
			int counter = 0;
			string line;
			System.IO.StreamReader file = new System.IO.StreamReader(@"name_list.txt");  
			while((line = file.ReadLine()) != null){  
				//System.Console.WriteLine(line);
				if(counter == firstName)
					name += line + " ";
				if(counter == 256 + lastName)
					name += line;
				counter++;
			}
			
			return name;

		}
	}
}