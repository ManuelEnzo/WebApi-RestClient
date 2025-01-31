# WebApi RestClient V1.0.0

## Cos'è WebApi RestClient
WebApi RestClient è una libreria pensata per semplificare e rendere fluide 
le chiamate API verso qualsiasi tipo di servizio web. 
La libreria è progettata per integrarsi con l'**HttpClientFactory** di Microsoft, 
sfruttando le sue funzionalità per la gestione dei client HTTP, 
come il pooling delle connessioni, la gestione del ciclo di vita e la configurazione 
centralizzata degli `HttpClient`.
La principale caratteristica di questa libreria è l'architettura **Fluent**, 
che permette di costruire e inviare richieste API in modo intuitivo e leggibile, 
con una sintassi compatta e facilmente configurabile.

## Come si usa ?
Nel file `Program.cs`, puoi registrare i vari **HttpClient** utilizzando il 
metodo `AddHttpClient`. Questo ti permette di impostare le proprietà comuni 
delle chiamate API, mantenendo la logica di creazione degli `HttpClient`
conforme agli standard di **ASP.NET Core**.

Ecco un esempio di configurazione di un client:
```csharp
services.AddHttpClient("api", client =>
{
	client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
	client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

A questo punto è possibile utilizzare la classe **HttpClientBuilderFactory**
che permette di creare un client con il nome specificato che deve 
essere uguale a quello inserito all'interno del metodo AddHttpClient.

```csharp
var client = _httpClientBuilderFactory.CreateClient("api");
```
Il metodo **CreateClient** restituirà un'istanza di **HttpClientBuilder**
la quale ci permette di costruire la nostra chiamata API in modo 
Fluent.

## Esempio di utilizzo

Con l'istanza di HttpClientBuilder, puoi configurare facilmente la chiamata API, 
specificando il metodo HTTP, l'endpoint, i parametri di query, il body e così via, 
utilizzando una sintassi Fluent.

Ecco un esempio di chiamata API con un metodo POST, query parameters e un body JSON:

```csharp
 var response = await client
                      .WithMethod(HttpMethod.Post)
                      .WithEndpoint("login")
                      .WithQueryParameters(new Dictionary<string, string>
                      {
                          { "useCookies", "false" },
                          { "useSessionCookies", "false" }
                      })
                      .WithBody(new { email = "test@mail.com", password = "Passw0rd" })
                      .BuildRequest()
                      .SendAsync<LoginInfoResponse>();
```

In questo esempio, la chiamata POST viene inviata all'endpoint `login` con i parametri
di query e il corpo contenente le credenziali di login. 
La risposta viene poi deserializzata nell'oggetto `LoginInfoResponse`.

## Gestione delle risposte

Quando invii una richiesta, la libreria restituisce un oggetto di tipo 
`RestResponseWithData<T>`, che estende la classe base `RestResponse`. 
Questo oggetto include diverse proprietà utili per gestire lo stato della risposta,
come il codice di stato HTTP, una descrizione dello stato, eventuali errori e i 
dati restituiti dalla chiamata.

```csharp
 public class RestResponse
 {
     /// <summary>
     /// Indica se lo stato della risposta è positivo o no.
     /// </summary>
     public bool IsSuccessful { get; internal set; }

     /// <summary>
     /// Status code della risposta.
     /// </summary>
     public HttpStatusCode StatusCode { get; internal set; }

     /// <summary>
     /// Descrizione dello stato della risposta.
     /// </summary>
     public string? StatusDescription { get; internal set; }

     /// <summary>
     /// Eventuale messaggio d'errore.
     /// </summary>
     public string? ErrorBody { get; internal set; }
 }
```
```csharp
public class RestResponseWithData<T> : RestResponse
{
    /// <summary>
    /// I dati restituiti dalla risposta API.
    /// </summary>
    public T? Data { get; internal set; }
}
```

Questa struttura consente di gestire facilmente gli errori e i dati restituiti 
dalle chiamate API, centralizzando la logica di gestione delle risposte e 
migliorando la leggibilità del codice.

## Caratteristiche principali
- **Fluent API**: costruisci e invia chiamate API in modo intuitivo e leggibile.
- **Supporto per i principali metodi HTTP**: GET, POST, PUT, DELETE, ecc.
- **Gestione centralizzata degli HttpClient**: sfrutta le funzionalità di HttpClientFactory.
- **Deserializzazione automatica della risposta**: deserializza automaticamente i 
        dati restituiti dalla chiamata API in base al tipo specificato.
- **Gestione degli errori**: gestisci facilmente gli errori e le eccezioni delle chiamate API.