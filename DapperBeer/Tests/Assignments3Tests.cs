using DapperBeer.Model;
using FluentAssertions;

namespace DapperBeer.Tests;

[TestFixture]
public class Assignments3Tests : TestHelper
{
    // 3.1 Test
    [Test]
    public async Task GetAllBrouwmeestersIncludesAddressTest()
    {
        List<Brewmaster> allBrewmastersIncludeAddress = Assignments3.GetAllBrouwmeestersIncludesAddress();
        await Verify(allBrewmastersIncludeAddress.Take(3));
    }
    
    // 3.2 Test
    [Test]
    public async Task GetAllBrewmastersWithBreweryTest()
    {
        List<Brewmaster> allBrewmastersWithBrewery = Assignments3.GetAllBrewmastersWithBrewery();
        await Verify(allBrewmastersWithBrewery.Take(3));
    }
    
    // 3.3 Test
    [Test]
    public async Task GetAllBrewersIncludeBrewmasterTest()
    {
        List<Brewer> allBrewersIncludeBrewmaster = Assignments3.GetAllBrewersIncludeBrewmaster();
        allBrewersIncludeBrewmaster.Should().HaveCount(677);
        await Verify(allBrewersIncludeBrewmaster.Take(3));
    }
    
    // 3.4 Test
    [Test]
    public async Task GetAllBeersIncludeBreweryTest()
    {
        List<Beer> allBeersIncludeBrewery = Assignments3.GetAllBeersIncludeBrewery();
        await Verify(allBeersIncludeBrewery.Take(3));
    }
    
    // 3.5 Test
    [Test]
    public async Task GetAllBrewersIncludingBeersNPlus1Test()
    {
        List<Brewer> allBrewersIncludingBeersNPlus1 = Assignments3.GetAllBrewersIncludingBeersNPlus1();
        allBrewersIncludingBeersNPlus1.Should().HaveCount(677);
        await Verify(allBrewersIncludingBeersNPlus1.Take(3));
    }
    
    // 3.6 Test
    [Test]
    public async Task GetAllBrewersIncludeBeersTest()
    {
        List<Brewer> allBrewersIncludeBeers = Assignments3.GetAllBrewersIncludeBeers();
        allBrewersIncludeBeers.Should().HaveCount(677);
        await Verify(allBrewersIncludeBeers.Take(3));
    }
    
    // 3.7 Test
    [Test]
    public async Task GetAllBeersIncludeBreweryAndIncludeBeersInBreweryTest()
    {
        List<Beer> allBeersIncludeBreweryAndIncludeBeersInBrewery = Assignments3.GetAllBeersIncludeBreweryAndIncludeBeersInBrewery();
        await Verify(allBeersIncludeBreweryAndIncludeBeersInBrewery);
    }
    
    // 3.8 Test
    [Test]
    public async Task GetAllBrewersIncludeBeersThenIncludeCafesTest()
    { 
        List<Brewer> allBrewersIncludeBeersThenIncludeCafes = Assignments3.GetAllBrewersIncludeBeersThenIncludeCafes();
        allBrewersIncludeBeersThenIncludeCafes.Should().HaveCount(677);
        List<Brewer> brewersWithBeersServedInMultipleCafes =
            allBrewersIncludeBeersThenIncludeCafes
                .Where(x => x.Beers.Any(b => b.Cafes.Count >= 2))
                .Take(1)
                .ToList();
        await Verify(brewersWithBeersServedInMultipleCafes);
    }
    
    // 3.9 Test
    [Test]
    public async Task GetBeerAndBrewersByViewTest()
    {
        List<Beer> result = Assignments3.GetBeerAndBrewersByView();
        result.Should().HaveCount(1617);
        await Verify(result.Take(3));
    }
}