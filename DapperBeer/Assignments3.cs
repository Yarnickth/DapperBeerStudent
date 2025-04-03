using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using DapperBeer.DTO;
using DapperBeer.Model;
using DapperBeer.Tests;

namespace DapperBeer;

public class Assignments3
{
    // 3.1 Question
    // Tip: Kijk in voorbeelden en sheets voor inspiratie.
    // Deze staan in de directory ExampleFromSheets/Relationships.cs. 
    // De sheets kan je vinden op: https://slides.com/jorislops/dapper/
    // Kijk niet te veel naar de voorbeelden van relaties op https://www.learndapper.com/relationships
    // Deze aanpak is niet altijd de manier de gewenst is!
    
    // 1 op 1 relatie (one-to-one relationship)
    // Een brouwmeester heeft altijd 1 adres. Haal alle brouwmeesters op en zorg ervoor dat het address gevuld is.
    // Sorteer op naam.
    // Met andere woorden een brouwmeester heeft altijd een adres (Property Address van type Address), zie de klasse Brewmaster.
    // Je kan dit doen door een JOIN te gebruiken.
    // Je zult de map functie in Query<Brewmaster, Address, Brewmaster>(sql, map: ...) moeten gebruiken om de Address property van Brewmaster te vullen.
    // Kijk in voorbeelden hoe je dit kan doen. Deze staan in de directory ExampleFromSheets/Relationships.cs.
public static List<Brewmaster> GetAllBrouwmeestersIncludesAddress()
    {
        
        var sql = @"
        SELECT 
            b.BrewmasterId, 
            b.Name, 
            b.BrewerId, 
            b.AddressId,
            '' as AddressSplit,
            a.AddressId, 
            a.Street, 
            a.City, 
            a.Country
        FROM Brewmaster b
        JOIN Address a ON b.AddressId = a.AddressId
        ORDER BY b.Name";

        using IDbConnection connection = DbHelper.GetConnection();
        
        List<Brewmaster> brewmasters = connection.Query<Brewmaster, Address, Brewmaster>(
                sql,
                map: (brewmaster, address) =>
                {
                    brewmaster.Address = address;
                    return brewmaster;
                },
                splitOn: "AddressSplit")
            .ToList();

        return brewmasters;
        /*string sql =
            """
            SELECT 
                c.customer_id AS CustomerId,
                c.store_id AS StoreId,
                c.first_name AS FirstName,
                c.last_name AS LastName,
                c.email AS Email,
                c.address_id AS AddressId,
                c.active AS Active,
                c.create_date AS CreateDate,
                c.last_update AS LastUpdate,
                '' as 'AddressSplit',
                a.address_id AS AddressId,
                a.address AS Address1,
                a.address2 AS Address2,
                a.district AS District,
                a.city_id AS CityId,
                a.postal_code AS PostalCode,
                a.phone AS Phone,
                a.last_update AS LastUpdate
            FROM customer c 
                JOIN address a ON c.address_id = a.address_id
            ORDER BY c.last_name, c.first_name
            Limit 10
            """;

        using IDbConnection connection = DbHelper.GetConnection();
        List<Customer> customers = connection.Query<Customer, Address, Customer>(
                sql,
                map: (customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                },
                splitOn: "AddressSplit")
            .ToList();
        return customers;
    }*/

    }


    // 3.2 Question
    // 1 op 1 relatie (one-to-one relationship)
    // Haal alle brouwmeesters op en zorg ervoor dat de brouwer (Brewer) gevuld is.
    // Sorteer op naam.
    public static List<Brewmaster> GetAllBrewmastersWithBrewery()
    {
        var sql = @"
        SELECT 
            b.BrewmasterId, 
            b.Name, 
            b.BrewerId, 
            b.AddressId,
            '' as BrewerSplit,
            a.BrewerId, 
            a.Name, 
            a.Country
        FROM Brewmaster b
        JOIN Brewer a ON b.BrewerId = a.BrewerId
        ORDER BY b.Name";

        using IDbConnection connection = DbHelper.GetConnection();
        
        List<Brewmaster> brewmasters = connection.Query<Brewmaster, Brewer, Brewmaster>(
                sql,
                map: (brewmaster, brewer) =>
                {
                    brewmaster.Brewer = brewer;
                    return brewmaster;
                },
                splitOn: "BrewerSplit")
            .ToList();

        return brewmasters;
        
    }

    // 3.3 Question
    // 1 op 1 (0..1) (one-to-one relationship) 
    // Geef alle brouwers op en zorg ervoor dat de brouwmeester gevuld is.
    // Sorteer op brouwernaam.
    //
    // Niet alle brouwers hebben een brouwmeester.
    // Let op: gebruik het correcte type JOIN (JOIN, LEFT JOIN, RIGHT JOIN).
    // Dapper snapt niet dat het om een 1 - 0..1 relatie gaat.
    // De Query methode ziet er als volgt uit (let op het vraagteken optioneel):
    // Query<Brewer, Brewmaster?, Brewer>(sql, map: ...)
    // Wat je kan doen is in de map functie een controle toevoegen, je zou dit verwachten:
    // if (brewmaster is not null) { brewer.Brewmaster = brewmaster; }
    // !!echter dit werkt niet!!!!
    // Plaats eens een breakpoint en kijk wat er in de brewmaster variabele staat,
    // hoe moet dan je if worden?
    public static List<Brewer> GetAllBrewersIncludeBrewmaster()
    {
        var sql = @"
        SELECT 
            b.BrewerId, 
            b.Name, 
            b.Country,
            '' as BrewerSplit,
            a.BrewmasterId, 
            a.Name, 
            a.BrewerId, 
            a.AddressId
        FROM Brewer b
        LEFT JOIN Brewmaster a ON b.BrewerId = a.BrewerId
        ORDER BY b.Name";

        using IDbConnection connection = DbHelper.GetConnection();
        
        List<Brewer> Brewer = connection.Query<Brewer, Brewmaster?, Brewer>(
                sql,
                map: (brewer, brewmaster) =>
                {
                    if (brewmaster.Name != null)
                    {
                        brewer.Brewmaster = brewmaster;
                    }
                       
                    return brewer;
                },
                splitOn: "BrewerSplit")
            .ToList();

        
        return Brewer;
        
        
    }

    // 3.4 Question
    // 1 op veel relatie (one-to-many relationship)
    // Geef een overzicht van alle bieren. Zorg ervoor dat de property Brewer gevuld is.
    // Sorteer op biernaam en beerId!!!!
    // Zorg ervoor dat bieren van dezelfde brouwerij naar dezelfde instantie van Brouwer verwijzen.
    // Dit kan je doen door een Dictionary<int, Brouwer> te gebruiken.
    // Kijk in voorbeelden hoe je dit kan doen. Deze staan in de directory ExampleFromSheets/Relationships.cs.
    public static List<Beer> GetAllBeersIncludeBrewery()
    {
        string sql = 
            @"  SELECT beer.beerid,
                beer.name,
                beer.type,
                beer.style,
                beer.alcohol,
                beer.brewerid,
                '' as 'BrewerSplit',
                brewer.brewerid,
                brewer.name,
                brewer.Country
                from beer
                join brewer on beer.brewerid = brewer.brewerid
                ORDER BY beer.name, beer.beerid
                LIMIT 10";

        Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();

        using IDbConnection connection = DbHelper.GetConnection();
        List<Beer> beers = connection.Query<Beer, Brewer, Beer>(
            sql,
            map: (beer, brewer) =>
            {
                if (brewerDictionary.ContainsKey(brewer.BrewerId))
                {
                    brewer = brewerDictionary[brewer.BrewerId];
                }
                else
                {
                    brewerDictionary.Add(brewer.BrewerId, brewer);
                }
                
                beer.Brewer = brewer;
                return beer;
            },
            splitOn: "BrewerSplit").ToList();
        return beers;
    }

    
    // 3.5 Question
    // N+1 probleem (1-to-many relationship)
    // Geef een overzicht van alle brouwerijen en hun bieren. Sorteer op brouwerijnaam en daarna op biernaam.
    // Doe dit door eerst een Query<Brewer>(...) te doen die alle brouwerijen ophaalt. (Dit is 1)
    // Loop (foreach) daarna door de brouwerijen en doe voor elke brouwerij een Query<Beer>(...)
    // die de bieren ophaalt voor die brouwerij. (Dit is N)
    // Dit is een N+1 probleem. Hoe los je dit op? Dat zien we in de volgende vragen.
    // Als N groot is (veel brouwerijen) dan kan dit een performance probleem zijn of worden. Probeer dit te voorkomen!
    public static List<Brewer> GetAllBrewersIncludingBeersNPlus1()
    {
        string sql =
            @"  SELECT
                        brewer.brewerid,
                        brewer.name,
                        brewer.country,
                        '' as BeerSplit,
                        beer.name,
                        beer.beerid,
                        beer.type,
                        beer.style,
                        beer.alcohol,
                        beer.brewerid
                        from brewer
                        join beer on brewer.brewerid = beer.brewerid
                        ORDER BY brewer.name, beer.name";

        Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();
        using IDbConnection connection = DbHelper.GetConnection();
        List<Brewer> brewer = connection.Query<Brewer, Beer, Brewer>(
                sql,
                map: (brewer, beer) =>
                {
                    if (brewerDictionary.ContainsKey(brewer.BrewerId))
                    {
                        brewer = brewerDictionary[brewer.BrewerId];
                    }
                    else
                    {
                        brewerDictionary.Add(brewer.BrewerId, brewer);
                    }

                    brewer.Beers.Add(beer);
                    return brewer;
                },
                splitOn: "BeerSplit")
            .Distinct() 
            .ToList();
        return brewer;
    }
    
    // 3.6 Question
    // 1 op n relatie (one-to-many relationship)
    // Schrijf een query die een overzicht geeft van alle brouwerijen. Vul per brouwerij de property Beers (List<Beer>) met de bieren van die brouwerij.
    // Sorteer op brouwerijnaam en daarna op biernaam.
    // Gebruik de methode Query<Brewer, Beer, Brewer>(sql, map: ...)
    // Het is belangrijk dat je de map functie gebruikt om de bieren te vullen.
    // De query geeft per brouwerij meerdere bieren terug. Dit is een 1 op veel relatie.
    // Om ervoor te zorgen dat de bieren van dezelfde brouwerij naar dezelfde instantie van Brewer verwijzen,
    // moet je een Dictionary<int, Brewer> gebruiken.
    // Dit is een veel voorkomend patroon in Dapper.
    // Vergeet de Distinct() methode te gebruiken om dubbel brouwerijen (Brewer) te voorkomen.
    //  Query<...>(...).Distinct().ToList().
    
    public static List<Brewer> GetAllBrewersIncludeBeers()
    {
        string sql =
            @"  SELECT
                    brewer.brewerid,
                    brewer.name,
                    brewer.country,
                    '' as BeerSplit,
                    beer.beerid,
                    beer.name,
                    beer.type,
                    beer.style,
                    beer.alcohol,
                    beer.brewerid
                    FROM brewer
                    LEFT JOIN beer ON brewer.brewerid = beer.brewerid
                    ORDER BY brewer.name, beer.name";

        Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();
        using IDbConnection connection = DbHelper.GetConnection();
        List<Brewer> brewers = connection.Query<Brewer, Beer, Brewer>(
                sql,
                map: (brewer, beer) =>
                {
                    if (brewerDictionary.ContainsKey(brewer.BrewerId))
                    {
                        brewer = brewerDictionary[brewer.BrewerId];
                    }
                    else
                    {
                        brewerDictionary.Add(brewer.BrewerId, brewer);
                    }
                
                    brewer.Beers.Add(beer);
                
                    return brewer;
                },
                splitOn: "BeerSplit")
            .Distinct()
            .ToList();
    
        return brewers;
    }
    
    // 3.7 Question
    // Optioneel:
    // Dezelfde vraag als hiervoor, echter kan je nu ook de Beers property van Brewer vullen met de bieren?
    // Hiervoor moet je wat extra logica in map methode schrijven.
    // Let op dat er geen dubbelingen komen in de Beers property van Beer!
    public static List<Beer> GetAllBeersIncludeBreweryAndIncludeBeersInBrewery()
    {
        string sql = 
            @"SELECT 
            beer.beerid,
            beer.name,
            beer.type,
            beer.style,
            beer.alcohol,
            beer.brewerid,
            '' as 'BrewerSplit',
            brewer.brewerid,
            brewer.name,
            brewer.Country
            FROM beer
            JOIN brewer ON beer.brewerid = brewer.brewerid
            ORDER BY beer.name, beer.beerid";

        Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();
        Dictionary<int, Beer> beerDictionary = new Dictionary<int, Beer>();

        using IDbConnection connection = DbHelper.GetConnection();
        List<Beer> beers = connection.Query<Beer, Brewer, Beer>(
            sql,
            map: (beer, brewer) =>
            {
                // Check if brewer already exists in dictionary
                if (!brewerDictionary.TryGetValue(brewer.BrewerId, out var existingBrewer))
                {
                    // If not, add it
                    brewerDictionary[brewer.BrewerId] = brewer;
                    existingBrewer = brewer;
                }
            
                // Check if beer already exists in dictionary
                if (!beerDictionary.TryGetValue(beer.BeerId, out var existingBeer))
                {
                    // If not, add it
                    beer.Brewer = existingBrewer;
                    beerDictionary[beer.BeerId] = beer;
                    existingBeer = beer;
                
                    // Add beer to brewer's beer list
                    existingBrewer.Beers.Add(existingBeer);
                }
            
                return existingBeer;
            },
            splitOn: "BrewerSplit").ToList();
    
        return beers;
    }
    
    // 3.8 Question
    // n op n relatie (many-to-many relationship)
    // Geef een overzicht van alle cafés en welke bieren ze schenken.
    // Let op een café kan meerdere bieren schenken. En een bier wordt vaak in meerdere cafe's geschonken. Dit is een n op n relatie.
    // Sommige cafés schenken geen bier. Dus gebruik LEFT JOINS in je query.
    // Bij n op n relaties is er altijd spraken van een tussen-tabel (JOIN-table, associate-table), in dit geval is dat de tabel Sells.
    // Gebruikt de multi-mapper Query<Cafe, Beer, Cafe>("query", splitOn: "splitCol1, splitCol2").
    // Gebruik de klassen Cafe en Beer.
    // De bieren worden opgeslagen in een de property Beers (List<Beer>) van de klasse Cafe.
    // Sorteer op cafénaam en daarna op biernaam.
    
    // Kan je ook uitleggen wat het verschil is tussen de verschillende JOIN's en wat voor gevolg dit heeft voor het resultaat?
    // Het is belangrijk om te weten wat de verschillen zijn tussen de verschillende JOIN's!!!! Als je dit niet weet, zoek het op!
    // Als je dit namelijk verkeerd doet, kan dit grote gevolgen hebben voor je resultaat (je krijgt dan misschien een verkeerde aantal records).
    public static List<Cafe> OverzichtBierenPerKroegLijstMultiMapper()
    {
        string sql = 
            @"SELECT
            c.cafeid,
            c.name,
            c.address,
            c.city,
            '' as 'BeerSplit',
            b.beerid,
            b.name,
            b.type,
            b.style,
            b.alcohol,
            b.brewerid
            FROM cafe c
            LEFT JOIN sells s ON c.cafeid = s.cafeid
            LEFT JOIN beer b ON s.beerid = b.beerid
            ORDER BY c.name, b.name";

        Dictionary<int, Cafe> cafeDictionary = new Dictionary<int, Cafe>();

        using IDbConnection connection = DbHelper.GetConnection();
        List<Cafe> cafes = connection.Query<Cafe, Beer, Cafe>(
                sql,
                map: (cafe, beer) =>
                {
                    if (!cafeDictionary.TryGetValue(cafe.CafeId, out var existingCafe))
                    {
                        cafeDictionary[cafe.CafeId] = cafe;
                        existingCafe = cafe;
                    }
            
                    // Only add beer if it's not null (might be null due to LEFT JOIN)
                    if (beer != null && beer.BeerId != 0)
                    {
                        existingCafe.Beers.Add(beer);
                    }
            
                    return existingCafe;
                },
                splitOn: "BeerSplit")
            .Distinct()
            .ToList();
    
        return cafes;
    }

    // 3.9 Question
    // We gaan nu nog een niveau dieper. Geef een overzicht van alle brouwerijen, met daarin de bieren die ze verkopen,
    // met daarin in welke cafés ze verkocht worden.
    // Sorteer op brouwerijnaam, biernaam en cafenaam. 
    // Gebruik (vul) de class Brewer, Beer en Cafe.
    // Gebruik de methode Query<Brewer, Beer, Cafe, Brewer>(...) met daarin de juiste JOIN's in de query en splitOn parameter.
    // Je zult twee dictionaries moeten gebruiken. Een voor de brouwerijen en een voor de bieren.
    public static List<Brewer> GetAllBrewersIncludeBeersThenIncludeCafes()
{
    string sql = 
        @"SELECT
            brewer.brewerid,
            brewer.name,
            brewer.country,
            '' as 'BeerSplit',
            beer.beerid,
            beer.name,
            beer.type,
            beer.style,
            beer.alcohol,
            beer.brewerid,
            '' as 'CafeSplit',
            cafe.cafeid,
            cafe.name,
            cafe.address,
            cafe.city
            FROM brewer
            LEFT JOIN beer ON brewer.brewerid = beer.brewerid
            LEFT JOIN sells ON beer.beerid = sells.beerid
            LEFT JOIN cafe ON sells.cafeid = cafe.cafeid
            ORDER BY brewer.name, beer.name, cafe.name";

    Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();
    Dictionary<int, Beer> beerDictionary = new Dictionary<int, Beer>();

    using IDbConnection connection = DbHelper.GetConnection();
    List<Brewer> brewers = connection.Query<Brewer, Beer, Cafe, Brewer>(
        sql,
        map: (brewer, beer, cafe) =>
        {
            // Check if brewer already exists in dictionary
            if (!brewerDictionary.TryGetValue(brewer.BrewerId, out var existingBrewer))
            {
                brewerDictionary[brewer.BrewerId] = brewer;
                existingBrewer = brewer;
            }
            
            // Only process beer if it's not null (could be null due to LEFT JOIN)
            if (beer != null && beer.BeerId != 0)
            {
                // Check if beer already exists in dictionary
                if (!beerDictionary.TryGetValue(beer.BeerId, out var existingBeer))
                {
                    beer.Brewer = existingBrewer;
                    beerDictionary[beer.BeerId] = beer;
                    existingBrewer.Beers.Add(beer);
                    existingBeer = beer;
                }
                else
                {
                    existingBeer = beerDictionary[beer.BeerId];
                }
                
                // Only add cafe if it's not null (could be null due to LEFT JOIN)
                if (cafe != null && cafe.CafeId != 0)
                {
                    // Check if cafe is already in the beer's cafe list to avoid duplicates
                    if (!existingBeer.Cafes.Any(c => c.CafeId == cafe.CafeId))
                    {
                        existingBeer.Cafes.Add(cafe);
                    }
                }
            }
            
            return existingBrewer;
        },
        splitOn: "BeerSplit,CafeSplit")
        .Distinct()
        .ToList();
    
    return brewers;
}



    // 3.10 Question - Er is geen test voor deze vraag
    // Optioneel: Geef een overzicht van alle bieren en hun de bijbehorende brouwerij.
    // Sorteer op brouwerijnaam, biernaam.
    // Gebruik hiervoor een View BeerAndBrewer (maak deze zelf). Deze view bevat alle informatie die je nodig hebt gebruikt join om de tabellen Beer, Brewer.
    // Let op de kolomnamen in de view moeten uniek zijn. Dit moet je dan herstellen in de query waarin je view gebruik zodat Dapper het snap
    // (SELECT BeerId, BeerName as Name, Type, ...). Zie BeerName als voorbeeld hiervan.
    public static List<Beer> GetBeerAndBrewersByView()
{
    // First, we need to create a view - this would typically be done at the database level
    // But for this implementation, we'll use a query that acts like a view
    
    string createViewSql = 
        @"CREATE VIEW IF NOT EXISTS BeerAndBrewer AS
          SELECT 
            beer.beerid AS BeerId,
            beer.name AS BeerName,
            beer.type AS Type,
            beer.style AS Style,
            beer.alcohol AS Alcohol,
            beer.brewerid AS BrewerId,
            brewer.name AS BrewerName,
            brewer.country AS Country
          FROM beer
          JOIN brewer ON beer.brewerid = brewer.brewerid";
    
    string querySql = 
        @"SELECT 
            BeerId,
            BeerName AS Name,
            Type,
            Style,
            Alcohol,
            BrewerId,
            '' AS BrewerSplit,
            BrewerId,
            BrewerName AS Name,
            Country
          FROM BeerAndBrewer
          ORDER BY BrewerName, BeerName";
    
    using IDbConnection connection = DbHelper.GetConnection();
    
    try
    {
        // Try to create the view if it doesn't exist
        connection.Execute(createViewSql);
    }
    catch
    {
        // In case we can't create views in this context, we'll use a direct query instead
        querySql = 
            @"SELECT 
                beer.beerid AS BeerId,
                beer.name AS Name,
                beer.type AS Type,
                beer.style AS Style,
                beer.alcohol AS Alcohol,
                beer.brewerid AS BrewerId,
                '' AS BrewerSplit,
                brewer.brewerid AS BrewerId,
                brewer.name AS Name,
                brewer.country AS Country
              FROM beer
              JOIN brewer ON beer.brewerid = brewer.brewerid
              ORDER BY brewer.name, beer.name";
    }
    
    Dictionary<int, Brewer> brewerDictionary = new Dictionary<int, Brewer>();
    
    List<Beer> beers = connection.Query<Beer, Brewer, Beer>(
        querySql,
        map: (beer, brewer) =>
        {
            if (!brewerDictionary.TryGetValue(brewer.BrewerId, out var existingBrewer))
            {
                brewerDictionary[brewer.BrewerId] = brewer;
                existingBrewer = brewer;
            }
            
            beer.Brewer = existingBrewer;
            return beer;
        },
        splitOn: "BrewerSplit")
        .ToList();
    
    return beers;
}
}