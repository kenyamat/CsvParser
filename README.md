#CSVParser.NET
CSV parser library for .NET

```
CsvParser.Parser parser = new CsvParser.Parser();
List<List<string>> result = parser.Parse("a,b,c");
string v1 = result[0][0]; // "a"
```
## Overview
* Parse CSV format string and return a 2 dimensional string List `List<List<string>>`.
* Normalize CR(`\r`) and CRLF(`\r\n`) into LF(`\n`) before parsing. 
* [Deal with double quotes columns.](#dq)
* [Deal with Excel CSV with double quotes and LF.](#lf)
* [Skip rows using `SkipKeyWords`.](#skip)

#### <a name="dq">Deals with quate columns.</a>

|hello world|123|456|
|:--|:--|:--|
|<b>abc,def</b>|&nbsp;|&nbsp;|

```
CsvParser.Parser parser = new CsvParser.Parser();
List<List<string>> result = parser.Parse("\"hello world\",123,456\n" +
                                         "\"abc,def\"");
int rowCount = result.Count; //-> 2
string v1 = result[0][0]; //-> "hello world"
string v2 = result[0][1]; //-> "123"
```

#### <a name="lf">Deal with Excel CSV with double quotes and LF.</a>

|hello<br>world|12,3|456|
|:--|:--|:--|
|<b>abc</b>|<b>def</b>|&nbsp;|

```
CsvParser.Parser parser = new CsvParser.Parser();
List<List<string>> result = parser.Parse("\"hello\n" +
                                         "world\","12,3",456\n" +
                                         "abc,def");
int rowCount = result.Count; //-> 2
string v1 = result[0][0]; //-> "hello\nworld"
string v2 = result[0][1]; //-> "12,3"
```

#### <a name="skip">Skip rows using `SkipKeyWords`.</a>

|hello world|123|456|
|:--|:--|:--|
|REM|<b>def</b>|<b>ghi</b>|

```
CsvParser.Parser parser = new CsvParser.Parser();
parser.SkipKeyWord.Add("REM");
List<List<string>> result = parser.Parse("\"hello world\","123",456\n" +                                      
                                         "REM,def,ghi");
int rowCount = result.Count; //-> 1
string v1 = result[0][0]; //-> "hello world"
```
## nuget package
[http://www.nuget.org/packages/CsvParser.NET/](http://www.nuget.org/packages/CsvParser.NET/)

## <a name="Copyright">Copyright</a>
Copyright (c) 2014 kenyamat. Licensed under MIT.