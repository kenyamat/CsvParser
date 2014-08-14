#CsvParser
This is a C# csv parser library.

```
CsvParser parser = new CsvParser();
List<List<string>> result = parser.Parse("a,b,c");
string v1 = result[0][0]; // "a"
```

Deals with multiline fields and excaped double quotes format.

hello<br>world|12,3|456|
--|--|--|
<b>abc</b>|<b>def</b>|&nbsp;|

```
CsvParser parser = new CsvParser();
List<List<string>> result = parser.Parse("\"hello\n" +
                                         "world\","12,3",456\n" +
                                         "abc,def");
string v1 = result[0][0]; // "hello\nworld"
string v2 = result[0][1]; // "12,3"
```

## <a name="Copyright">Copyright</a>
Copyright (c) 2014 kenyamat. Licensed under MIT.