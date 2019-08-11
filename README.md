# HttpUtils for .Net framework in C#

A wrapper of HttpClient for a web api REST service that optionally allows
authentication to be added to the header of the request that later to be
checked using the ActionFilter in the web api controller methods.

####Example Usage:
```cs
var httpClientHelper = new HttpClientHelper();
```

####Example method calls:
```cs
var getSingleResult = httpClientHelper.Get("ApiMethod/1").Result;
var postResult = httpClientHelper.Post("ApiMethod/", object).Result;
httpClientHelper.Put("ApiMethod/3", Tobject).Wait();
httpClientHelper.Delete("ApiMethod/3").Wait();
```