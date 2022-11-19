using Degano;

namespace UnitTesting
{
    public class GasStationTesting
    {
        [Fact]
        public async void GasStationListEmpty()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location();
            await Assert.ThrowsAsync<Exception>(() => GasStation.FindGasStation());
        }

        [Fact]
        public async void NoGasStationsInRange()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location();
            GasStation.gasStationList.Add(new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a"));
            await Assert.ThrowsAsync<Exception>(() => GasStation.FindGasStation());
        }

        [Fact]
        public void UserLocationNotDefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location();
            GasStation g = new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a");
            Assert.Throws<Exception>(() => g.GetDistanceToUser());
        }

        [Fact]
        public void GasStationLocationNotDefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location();
            GasStation g = new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a");
            Assert.Throws<Exception>(() => g.GetDistanceToUser());
        }

        [Fact]
        public void GasStationAppealComparisonUndefinedRange()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location();
            GasStation g = new GasStation("a", "a", new Location(0, 0), 1.5, 0, 0, 0, "a");
            GasStation.gasStationList.Add(g);
            GasStation.preferredPriceMax = -1;
            GasStation.preferredPriceMin = -1;
            Assert.Throws<Exception>(() => g.appealCoef);
        }

        [Fact]
        public void GasStationAppealComparisonDistance()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0.01, 0.01), 1.5, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            GasStation.gasStationList.ForEach(g => g.GetDistanceToUser());
            GasStation.preferredPriceMin = -1;
            GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.price95);
            Assert.True(g2.appealCoef > g1.appealCoef);
        }

        [Fact]
        public void GasStationAppealComparisonPrice95()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            GasStation.gasStationList.ForEach(g => g.GetDistanceToUser());
            GasStation.preferredPriceMin = -1;
            GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.price95);
            Assert.True(g2.appealCoef < g1.appealCoef);
        }

        [Fact]
        public void GasStationAppealComparisonDistanceUndefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            Assert.Throws<Exception>(() => g2.appealCoef < g1.appealCoef);
        }
    }
}
