using Degano;
using Degano.Helpers;
using Degano.SqliteDb;
using SQLite;

namespace UnitTesting
{
    public class GasStationTesting
    {
        [Fact]
        public async void GasStationListEmpty()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location();
            await Assert.ThrowsAsync<Exception>(() => GasStation.FindGasStation());
        }

        [Fact]
        public async void NoGasStationsInRange()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location(0, 0);
            GasStation g = new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a");
            g.GetDistanceToUser();
            GasStation.enabledGasStationList.Add(g);

            await Assert.ThrowsAsync<Exception>(() => GasStation.FindGasStation());
        }

        [Fact]
        public void UserLocationNotDefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location();
            GasStation g = new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a");
            Assert.Throws<Exception>(() => g.GetDistanceToUser());
        }

        [Fact]
        public void GasStationLocationNotDefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location();
            GasStation g = new GasStation("a", "a", new Location(54, 27), 0, 0, 0, 0, "a");
            Assert.Throws<Exception>(() => g.GetDistanceToUser());
        }

        [Fact]
        public void GasStationAppealComparisonUndefinedRange()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
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
            GasStation.selectedType = "95";
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0.01, 0.01), 1.5, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            GasStation.gasStationList.ForEach(g => g.GetDistanceToUser());
            GasStation.preferredPriceMin = -1;
            GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.fuelPrice["95"]);
            Assert.True(g2.appealCoef > g1.appealCoef);
        }

        [Fact]
        public void GasStationAppealComparisonPrice95()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            GasStation.gasStationList.ForEach(g => g.GetDistanceToUser());
            GasStation.preferredPriceMin = -1;
            GasStation.preferredPriceMax = GasStation.gasStationList.Max(g => g.fuelPrice["95"]);
            Assert.True(g2.appealCoef < g1.appealCoef);
        }

        [Fact]
        public void GasStationAppealComparisonDistanceUndefined()
        {
            GasStation.gasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location(0, 0);
            GasStation g1 = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(0, 0), 1.5, 0, 0, 0, "b");
            GasStation.gasStationList.Add(g1);
            GasStation.gasStationList.Add(g2);
            Assert.Throws<Exception>(() => g2.appealCoef < g1.appealCoef);
        }

        [Fact]
        public async void GasStationUpdateAllUserLocationInvalid()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation g = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation.enabledGasStationList.Add(g);
            GasStation.selectedType = "95";
            UserLocation.location = new Location(-1, -1);
            await Assert.ThrowsAsync<Exception>(() => GasStation.UpdateAllDistances());
        }

        [Fact]
        public async void GasStationUpdateAllGasStationLocationInvalid()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location(-1, -1);
            GasStation g1 = new GasStation("a", "a", new Location(0, 0), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(-1, -1), 1.5, 0, 0, 0, "b");
            GasStation.enabledGasStationList.Add(g1);
            GasStation.enabledGasStationList.Add(g2);
            await Assert.ThrowsAsync<Exception>(() => GasStation.UpdateAllDistances());
        }

        [Fact]
        public async void GasStationUpdateAllInvokesAll()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "95";
            UserLocation.location = new Location(54.27, 27.54);
            GasStation g1 = new GasStation("a", "a", new Location(54, 27), 1.4, 0, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(54, 27), 1.5, 0, 0, 0, "b");
            GasStation.enabledGasStationList.Add(g1);
            GasStation.enabledGasStationList.Add(g2);
            g1.GetDistanceToUser();
            g2.GetDistanceToUser();
            var g1Dist = g1.distance;
            var g2Dist = g2.distance;
            await GasStation.UpdateAllDistances();
            Assert.True(g1Dist != g1.distance && g2Dist != g2.distance);
        }

        [Fact]
        public async void GasStationAppealConsidersFuelType()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "98";
            UserLocation.location = new Location(54, 27);
            GasStation g1 = new GasStation("a", "a", new Location(54, 27), 1.4, 1.5, 0, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(54, 27), 1.5, 1.4, 0, 0, "b");
            GasStation.enabledGasStationList.Add(g1);
            GasStation.enabledGasStationList.Add(g2);
            g1.GetDistanceToUser();
            g2.GetDistanceToUser();
            Assert.True(await GasStation.FindGasStation() == g2);
        }

        [Fact]
        public async void GasStationAppealConsidersFuelType2()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "Diesel";
            UserLocation.location = new Location(54, 27);
            GasStation g1 = new GasStation("a", "a", new Location(54, 27), 1.4, 0, 1.5, 0, "a");
            GasStation g2 = new GasStation("b", "b", new Location(54, 27), 1.5, 0, 1.4, 0, "b");
            GasStation.enabledGasStationList.Add(g1);
            GasStation.enabledGasStationList.Add(g2);
            g1.GetDistanceToUser();
            g2.GetDistanceToUser();
            Assert.True(await GasStation.FindGasStation() == g2);
        }

        [Fact]
        public async void GasStationAppealConsidersFuelType3()
        {
            GasStation.enabledGasStationList = new List<GasStation>();
            GasStation.selectedType = "LPG";
            UserLocation.location = new Location(54, 27);
            GasStation g1 = new GasStation("a", "a", new Location(54, 27), 1.4, 0, 0, 1.5, "a");
            GasStation g2 = new GasStation("b", "b", new Location(54, 27), 1.5, 0, 0, 1.4, "b");
            GasStation.enabledGasStationList.Add(g1);
            GasStation.enabledGasStationList.Add(g2);
            g1.GetDistanceToUser();
            g2.GetDistanceToUser();
            Assert.True(await GasStation.FindGasStation() == g2);
        }



        [Fact]
        public void RegexReturnsFalse1()
        {
            EmailValidator emailValidator = new EmailValidator();
            Assert.False(emailValidator.IsEmailValid("thisisnotvalidemail"));
            Assert.False(emailValidator.isValid);
        }

        [Fact]
        public void RegexReturnsFalse2()
        {
            EmailValidator emailValidator = new EmailValidator();
            Assert.False(emailValidator.IsEmailValid("thisisnotvalidemail@a"));
            Assert.False(emailValidator.isValid);
        }
        
        [Fact]
        public void RegexReturnsFalse3()
        {
            EmailValidator emailValidator = new EmailValidator();
            Assert.False(emailValidator.IsEmailValid("thisisnotvalidemail@a.a.a"));
            Assert.False(emailValidator.isValid);
        }

        [Fact]
        public void RegexReturnsTrue1()
        {
            EmailValidator emailValidator = new EmailValidator();
            Assert.True(emailValidator.IsEmailValid("thisisvalidemail@a.com"));
            Assert.True(emailValidator.isValid);
        }
        
        [Fact]
        public void RegexReturnsTrue2()
        {
            EmailValidator emailValidator = new EmailValidator();
            Assert.True(emailValidator.IsEmailValid("thisisvalidemail@a.co.uk"));
            Assert.True(emailValidator.isValid);
        }

        [Fact]
        public void PasswordReturnsMessage1()
        {
            PasswordValidator passwordValidator = new PasswordValidator();
            passwordValidator.IsPasswordValid("a");
            Assert.Equal("Password should contain at least 10 characters!", passwordValidator.ErrorMessage);
            Assert.False(passwordValidator.isValid);
        }

        [Fact]
        public void PasswordReturnsMessage2()
        {
            PasswordValidator passwordValidator = new PasswordValidator();
            passwordValidator.IsPasswordValid("a a  fasfasfasfasfasfasfasf");
            Assert.Equal("Password should not contain whitespaces!", passwordValidator.ErrorMessage);
            Assert.False(passwordValidator.isValid);
        }

        [Fact]
        public void PasswordReturnsMessage3()
        {
            PasswordValidator passwordValidator = new PasswordValidator();
            passwordValidator.IsPasswordValid("aaaaaaaaaaaaaaaaaaaaaaaa");
            Assert.Equal("Password should contain at least one digit!", passwordValidator.ErrorMessage);
            Assert.False(passwordValidator.isValid);
        }

        [Fact]
        public void PasswordIsGood()
        {
            PasswordValidator passwordValidator = new PasswordValidator();
            Assert.True(passwordValidator.IsPasswordValid("aaaaaaaaaaaaaaaaaaaaaaaa123"));
            Assert.True(passwordValidator.isValid);
        }

        [Fact]
        public void DatabaseCreation()
        {
            SqliteDatabase db = new SqliteDatabase();
            Assert.NotNull(db);
        }
    }
}
