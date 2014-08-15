namespace CsvParser.Sample
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CsvParser;

    class Program
    {
        public static void Main(string[] args)
        {
            Parser parser = new Parser();
            List<List<string>> result;
            
            // single line
            result = parser.Parse("a,b,c");
            Debug.Assert(result.Count == 1);
            Debug.Assert(result[0].Count == 3);
            Debug.Assert(result[0][0] == "a");
            Debug.Assert(result[0][1] == "b");
            Debug.Assert(result[0][2] == "c");

            // multiple lines
            result = parser.Parse("a,b,c\r\n" +
                                  "e,f");
            Debug.Assert(result.Count == 2);
            Debug.Assert(result[0].Count == 3);
            Debug.Assert(result[1][0] == "e");
            Debug.Assert(result[1][1] == "f");

            // double quoted value
            result = parser.Parse("\"a,b\",c\r\n" +
                                  "e,f");
            Debug.Assert(result.Count == 2);
            Debug.Assert(result[0].Count == 2);
            Debug.Assert(result[0][0] == "a,b");

            // double quoted value with return code
            result = parser.Parse("\"a\r\n" +
                                  "b\",c\r\n" +
                                  "e,f");
            Debug.Assert(result.Count == 2);
            Debug.Assert(result[0].Count == 2);
            Debug.Assert(result[0][0] == "a\nb");
        }
    }
}