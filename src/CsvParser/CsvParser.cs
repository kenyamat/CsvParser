namespace Csv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Csv Parser class
    /// </summary>
    public class CsvParser
    {
        /// <summary>Return code</summary>
        private const string ReturnCode = "\n";

        /// <summary>csv filed</summary>
        private string csv;

        /// <summary>csv body</summary>
        private List<List<string>> result;

        /// <summary>csv line</summary>
        private List<string> row;

        /// <summary>cell buffer</summary>
        private StringBuilder buffer;

        /// <summary>enclosing flag</summary>
        private bool enclosingFlag;

        /// <summary>Current letter</summary>
        private char currentLetter;

        /// <summary>Current position</summary>
        private int currentPosition;

        /// <summary>SkipKeyWords field</summary>
        private readonly IList<string> skipKeyWords = new List<string>();

        /// <summary>Gets or sets BlankLetter</summary>
        public string BlankLetter { get; set; }

        /// <summary>Gets or sets SkipKeyWords</summary>
        public IList<string> SkipKeyWords
        {
            get { return this.skipKeyWords; }
        }

        /// <summary>Gets or sets a value indicating whether adding empty row</summary>
        public bool SkipEmptyRow { get; set; }

        /// <summary>Gets or sets MinimumColumnCount</summary>
        public int? MinColumnCount { get; set; }

        /// <summary>
        /// Check if all cells are null
        /// </summary>a
        /// <param name="list">list parameter</param>
        /// <returns>true: all cells are null</returns>
        public static bool CheckAllNull(IList<string> list)
        {
            return list.All(string.IsNullOrEmpty);
        }

        /// <summary>
        /// Make all of column length
        /// </summary>
        /// <param name="data">data parameter</param>
        /// <param name="length">length parameter</param>
        public static void MakeAllOfColumnmLength(IList<IList<string>> data, int length)
        {
            foreach (var row in data)
            {
                if (row.Count > length)
                {
                    while (true)
                    {
                        row.RemoveAt(row.Count - 1);
                        if (row.Count == length)
                        {
                            break;
                        }
                    }
                }

                if (row.Count < length)
                {
                    while (true)
                    {
                        row.Add(null);
                        if (row.Count == length)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses CSV
        /// </summary>
        /// <param name="csv">csv parameter</param>
        /// <returns>csv list</returns>
        public List<List<string>> Parse(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) { throw new ArgumentNullException("csv"); }

            this.csv = csv.Replace("\r\n", "\n").Replace("\r", "\n");
            this.result = new List<List<string>>();
            this.row = new List<string>();
            this.buffer = new StringBuilder();
            this.enclosingFlag = false;
            this.currentPosition = 0;

            while (this.currentPosition < this.csv.Length)
            {
                switch (this.currentLetter = this.csv[this.currentPosition])
                {
                    case ',':
                        if (this.enclosingFlag)
                        {
                            this.buffer.Append(this.currentLetter);
                        }
                        else
                        {
                            this.EndValue();
                        }

                        break;
                    case '\"':
                        if (this.enclosingFlag)
                        {
                            if (this.GetNextLetter() == '\"')
                            {
                                this.buffer.Append('\"');
                                this.currentPosition++;
                            }
                            else
                            {
                                this.EndValue();
                                if (this.GetNextLetter() == ',')
                                {
                                    this.currentPosition++;
                                }
                                else if (this.IsNextCharReturnCode())
                                {
                                    this.currentPosition++;
                                    this.EndRow();
                                }
                            }
                        }
                        else
                        {
                            this.enclosingFlag = true;
                        }

                        break;
                    default:
                        if (this.IsReturnCode())
                        {
                            if (this.enclosingFlag)
                            {
                                this.buffer.Append(ReturnCode);
                            }
                            else
                            {
                                this.EndValue();
                                this.EndRow();
                            }

                            break;
                        }

                        this.buffer.Append(this.currentLetter);
                        break;
                }

                this.currentPosition++;
            }

            this.EndValue();
            this.EndRow();

            var lastRow = this.result[this.result.Count - 1];
            if (lastRow == null || CheckAllNull(lastRow))
            {
                this.result.RemoveAt(this.result.Count - 1);
            }

            this.csv = null;
            return this.result;
        }

        /// <summary>
        /// End Value
        /// </summary>
        private void EndValue()
        {
            var value = this.buffer.Length == 0 ? this.BlankLetter : this.buffer.ToString();
            this.row.Add(value);
            this.enclosingFlag = false;
            this.buffer = new StringBuilder();
        }

        /// <summary>
        /// End row
        /// </summary>
        private void EndRow()
        {
            var emptyFlag = CheckAllNull(this.row);

            if (this.SkipEmptyRow && emptyFlag)
            {
                this.row = new List<string>();
            }
            else if (this.SkipKeyWords != null && this.row.Count > 0 && this.SkipKeyWords.Contains(this.row[0]))
            {
            }
            else
            {
                while (this.MinColumnCount != null && this.row.Count < this.MinColumnCount)
                {
                    this.row.Add(this.BlankLetter);
                }

                this.result.Add(this.row);
            }

            this.row = new List<string>();
        }

        /// <summary>
        /// Check if current letter is a return code.
        /// </summary>
        /// <returns>true:return code</returns>
        private bool IsReturnCode()
        {
            return ReturnCode == Convert.ToString(this.currentLetter);
        }

        /// <summary>
        /// Check if next char is a return code.
        /// </summary>
        /// <returns>true:next char is return code</returns>
        private bool IsNextCharReturnCode()
        {
            return ReturnCode.Equals(Convert.ToString(this.GetNextLetter()));
        }

        /// <summary>
        /// Gets next letter
        /// </summary>
        /// <returns>next letter</returns>
        private char GetNextLetter()
        {
            return this.csv.Length > this.currentPosition + 1 ? this.csv[this.currentPosition + 1] : '\0';
        }
    }
}
