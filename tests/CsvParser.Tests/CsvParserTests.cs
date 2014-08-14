namespace Csv.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Csv;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsvParserTests
    {
        [TestMethod]
        public void TestParse()
        {
            // Arrange
            const string Csv = "Header,Arrange,,Assertion,";

            // Act
            var result = new CsvParser().Parse(Csv);

            // Assert
            Assert.AreEqual(1, result.Count());
            var row = result.ElementAt(0);
            Assert.AreEqual(5, row.Count());
            Assert.AreEqual("Header", row[0]);
            Assert.AreEqual("Arrange", row[1]);
            Assert.AreEqual(null, row[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseWithNullCsv()
        {
            // Act
            new CsvParser().Parse(null);
        }

        [TestMethod]
        public void TestParseWithDoubleQuote()
        {
            // Arrange
            const string Csv = "\"H\"\"e,ader\",Arrange,,Assertion,aa\"\n";

            // Act
            var result = new CsvParser().Parse(Csv);

            // Assert
            Assert.AreEqual(1, result.Count());
            var row = result.ElementAt(0);
            Assert.AreEqual(5, row.Count());
            Assert.AreEqual("H\"e,ader", row[0]);
            Assert.AreEqual("Arrange", row[1]);
            Assert.AreEqual(null, row[2]);
        }

        [TestMethod]
        public void TestParseWithMultiLine()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,Assertion,\r\n" +
                ",Data1,Data2,\r\n";

            // Act
            var result = new CsvParser().Parse(Csv);

            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual(5, row0.Count());
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);

            // row[1]
            var row1 = result.ElementAt(1);
            Assert.AreEqual(4, row1.Count());
            Assert.AreEqual(null, row1[0]);
            Assert.AreEqual("Data1", row1[1]);
            Assert.AreEqual(null, row1[3]);
        }

        [TestMethod]
        public void TestParseAndIsNextCharReturnCode()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,\"Assertion\"\r\n" +
                ",Data1,Data2,\r\n";

            // Act
            var result = new CsvParser().Parse(Csv);

            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual(4, row0.Count());
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);
            Assert.AreEqual("Assertion", row0[3]);
            Assert.AreEqual(null, row0[2]);

            // row[1]
            var row1 = result.ElementAt(1);
            Assert.AreEqual(4, row1.Count());
            Assert.AreEqual(null, row1[0]);
            Assert.AreEqual("Data1", row1[1]);
            Assert.AreEqual(null, row1[3]);
        }

        [TestMethod]
        public void TestAdjustAllRowsLength()
        {
            // Arrange
            var data = new List<IList<string>>
            {
                new List<string> { string.Empty, string.Empty, string.Empty, string.Empty },
                new List<string> { string.Empty, string.Empty, string.Empty },
                new List<string> { string.Empty, string.Empty },
            };

            // Act
            CsvParser.MakeAllOfColumnmLength(data, 3);

            // Assert
            foreach (var row in data)
            {
                Assert.AreEqual(3, row.Count);
            }
        }

        [TestMethod]
        public void TestParseWithSkipWord()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,\"Assertion\"\r\n" +
                "SKIP,Data1,Data2,\r\n";
            var parser = new CsvParser();
            parser.SkipKeyWords.Add("SKIP");

            // Act
            var result = parser.Parse(Csv);

            // Assert
            Assert.AreEqual(1, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);
            Assert.AreEqual("Assertion", row0[3]);
            Assert.AreEqual(null, row0[2]);
        }

        [TestMethod]
        public void TestParseWithSkipWord2()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,\"Assertion\"\r\n" +
                "SKIP,Data1,Data2,\r\n";
            var parser = new CsvParser();
            parser.SkipKeyWords.Add("SKIP2");

            // Act
            var result = parser.Parse(Csv);

            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);
            Assert.AreEqual("Assertion", row0[3]);
            Assert.AreEqual(null, row0[2]);
        }

        [TestMethod]
        public void TestParseWithSkipEmptyRow()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,\"Assertion\"\r\n" +
                "\r\n" +
                ",Data1,\"Data2\",";
            var parser = new CsvParser();
            parser.SkipEmptyRow = true;

            // Act
            var result = parser.Parse(Csv);

            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);
            Assert.AreEqual("Assertion", row0[3]);
            Assert.AreEqual(null, row0[2]);
        }

        [TestMethod]
        public void TestParseWithMinColumnCount()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,Assertion,\r\n" +
                ",Data1,Data2,\r\n";

            // Act
            var parser = new CsvParser();
            parser.MinColumnCount = 10;
            var result = parser.Parse(Csv);


            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual(10, row0.Count());
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);

            // row[1]
            var row1 = result.ElementAt(1);
            Assert.AreEqual(10, row1.Count());
            Assert.AreEqual(null, row1[0]);
            Assert.AreEqual("Data1", row1[1]);
            Assert.AreEqual(null, row1[3]);
        }

        [TestMethod]
        public void TestParseWithDoubleQuoteEnd()
        {
            // Arrange
            const string Csv =
                "Header,Arrange,,Assertion,\r\n" +
                ",Data1,Data2\"";

            // Act
            var parser = new CsvParser();
            var result = parser.Parse(Csv);


            // Assert
            Assert.AreEqual(2, result.Count());

            // row[0]
            var row0 = result.ElementAt(0);
            Assert.AreEqual(5, row0.Count());
            Assert.AreEqual("Header", row0[0]);
            Assert.AreEqual("Arrange", row0[1]);
            Assert.AreEqual(null, row0[2]);

            // row[1]
            var row1 = result.ElementAt(1);
            Assert.AreEqual(3, row1.Count());
            Assert.AreEqual(null, row1[0]);
            Assert.AreEqual("Data1", row1[1]);
            Assert.AreEqual("Data2", row1[2]);
        }

        [TestMethod]
        public void TestBlankLetter()
        {
            // Arrange
            var parser = new CsvParser();

            // Act
            parser.BlankLetter = " ";

            // Assert
            Assert.AreEqual(" ", parser.BlankLetter);
        }

        [TestMethod]
        public void TestSkipKeyWords()
        {
            // Arrange
            var parser = new CsvParser();

            // Act
            parser.SkipKeyWords.Add("a");

            // Assert
            Assert.AreEqual("a", parser.SkipKeyWords.First());
        }
    }
}
